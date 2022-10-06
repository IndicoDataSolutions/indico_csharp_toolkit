using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;

using IndicoToolkit.Types;

namespace IndicoToolkit.Association
{
    /// <summary>
    /// Class <c>Line Items</c> associates line items given extraction predictions and ondocument OCR tokens
    /// </summary>
    public class LineItems : Association
    {
        public List<string> LineItemFields { get; private set; }
        public List<Prediction> UnmappedPositions { get; private set; }
        public LineItems(
            List<Prediction> predictions,
            List<string> lineItemFields
        ) : base(predictions)
        {
            LineItemFields = lineItemFields;
            UnmappedPositions = new List<Prediction>();
        }

        public Extractions updatedPredictions()
        {
            List<Prediction> updatedPredictions = MappedPositions.Concat(UnmappedPositions).Concat(ErroredPredictions).ToList();
            return new Extractions(updatedPredictions);
        }

        /// <summary>
        /// Match and add bounding box metadata to prediction.
        /// </summary>
        /// <param name="pred">Indico extraction model prediction</param>
        /// <param name="OcrTokens">List of OCR tokens</param>
        /// <param name="predIndex">prediction index. Defaults to 0</param>
        /// <returns>Index in ocr tokens where prediction matched</returns>
        public override int matchPredToToken(Prediction pred, List<OcrToken> OcrTokens, int predIndex = 0)
        {
            bool noMatch = true;
            int matchTokenIndex = 0;
            foreach (OcrToken token in OcrTokens)
            {
                if (noMatch && sequencesOverlap(token.DocOffset, pred))
                {
                    pred = addBoundingMetadataToPred(pred, token);
                    noMatch = false;
                    matchTokenIndex = token.Index;
                }
                else if (sequencesOverlap(token.DocOffset, pred))
                {
                    pred = updateBoundingMetadataForPred(new MappedPrediction(pred), token);
                }
                else if (token.DocOffset.Start > pred.End)
                {
                    break;
                }
            }
            checkIfTokenMatchFound(pred, noMatch);
            return matchTokenIndex;
        }

        /// <summary>
        /// Adds keys for bounding box top/bottom/left/right and page number to line item predictions
        /// </summary>
        /// <param name="ocrTokens">Tokens from 'ondocument' OCR config output</param>
        /// <param name="raiseForNoMatch">raise exception if a matching token isn't found for a prediction</param>
        public void getBoundingBoxes(List<OcrToken> ocrTokens, bool raiseForNoMatch = true)
        {
            List<Prediction> predictions = new List<Prediction>();
            foreach (Prediction pred in Predictions)
            {
                Prediction newPred = JsonConvert.DeserializeObject<Prediction>(
                JsonConvert.SerializeObject(pred));
                predictions.Add(newPred);
            }
            predictions = removeUnneededPredictions(predictions);
            predictions = sortPredictionsByStartIndex(predictions);
            int matchIndex = 0;
            foreach (Prediction pred in predictions)
            {
                try
                {
                    matchIndex = matchPredToToken(pred, ocrTokens.GetRange(matchIndex, ocrTokens.Count - 1));
                    MappedPositions.Add(new MappedPrediction(pred));
                }
                catch (System.Exception e)
                {
                    if (raiseForNoMatch)
                    {
                        throw e;
                    }
                    else
                    {
                        Console.WriteLine($"Ignoring Error: {e}");
                        ErroredPredictions.Add(pred);
                    }
                }
            }
        }

        /// <summary>
        /// Sets rowNumber property based on bounding box position and page
        /// </summary>
        public void assignRowNumber()
        {
            MappedPositions = MappedPositions.OrderBy(x => x.PageNum).ThenBy(x => x.BbTop).ThenBy(x => x.BbLeft).ToList();
            MappedPrediction startingPred = getFirstValidLineItemPred();
            float maxTop = startingPred.BbTop;
            float minBot = startingPred.BbBot;
            int pageNumber = startingPred.PageNum;
            int rowNumber = 1;
            foreach (MappedPrediction pred in MappedPositions)
            {
                if (pred.BbTop > minBot || pred.PageNum != pageNumber)
                {
                    rowNumber += 1;
                    pageNumber = pred.PageNum;
                    maxTop = pred.BbTop;
                    minBot = pred.BbBot;
                }
                else
                {
                    maxTop = Math.Min(pred.BbTop, maxTop);
                    minBot = Math.Max(pred.BbBot, minBot);
                }
                pred.RowNumber = rowNumber;
            }

        }

        /// <summary>
        /// After row number has been assigned to predictions, returns line item predictions
        /// as a list of lists where each list is a row.
        /// </summary>
        public List<List<MappedPrediction>> groupedLineItems()
        {
            Dictionary<int, List<MappedPrediction>> rows = new Dictionary<int, List<MappedPrediction>>();
            foreach (MappedPrediction pred in MappedPositions)
            {
                rows[pred.RowNumber].Add(pred);
            }
            return rows.Values.ToList();
        }

        public List<Prediction> removeUnneededPredictions(List<Prediction> predictions)
        {
            List<Prediction> validLineItemPreds = new List<Prediction>();
            foreach (Prediction pred in predictions)
            {
                if (!isLineItemPred(pred))
                {
                    UnmappedPositions.Add(pred);
                }
                else if (isManuallyAddedPred(pred))
                {
                    pred.Error = "Can't match tokens for manually added prediction";
                    ErroredPredictions.Add(pred);
                }
                else
                {
                    validLineItemPreds.Add(pred);
                }
            }
            return validLineItemPreds;
        }

        public bool isLineItemPred(Prediction pred)
        {
            if (LineItemFields.Contains(pred.Label))
            {
                return true;
            }
            return false;
        }

        public MappedPrediction getFirstValidLineItemPred()
        {
            if (MappedPositions.Count is 0)
            {
                throw new System.Exception("Whoops! You have no lineItemFields predictions. Did you run getBoundingBoxes?");
            }
            return MappedPositions[0];
        }

        public MappedPrediction addBoundingMetadataToPred(Prediction pred, OcrToken token)
        {
            MappedPrediction mappedPred = new MappedPrediction(pred);
            mappedPred.BbTop = token.Position.bbTop;
            mappedPred.BbBot = token.Position.bbBot;
            mappedPred.BbLeft = token.Position.bbLeft;
            mappedPred.BbRight = token.Position.bbRight;
            mappedPred.PageNum = token.Position.pageNum;
            return mappedPred;
        }

        public MappedPrediction updateBoundingMetadataForPred(MappedPrediction pred, OcrToken token)
        {
            pred.BbTop = Math.Min(token.Position.bbTop, pred.BbTop);
            pred.BbBot = Math.Max(token.Position.bbBot, pred.BbBot);
            pred.BbLeft = Math.Min(token.Position.bbLeft, pred.BbLeft);
            pred.BbRight = Math.Max(token.Position.bbRight, pred.BbRight);
            return pred;
        }

    }
}