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
        public override (Prediction, int) matchPredToToken(Prediction pred, List<OcrToken> OcrTokens, int predIndex = 0)
        {
            bool noMatch = true;
            Prediction newPred = null;
            int matchTokenIndex = 0;
            foreach (var (index, token) in Enumerate(OcrTokens))
            {
                if (noMatch && sequencesOverlap(token.DocOffset, pred))
                {
                    newPred = addBoundingMetadataToPred(pred, token);
                    noMatch = false;
                    matchTokenIndex = index;
                }
                else if (sequencesOverlap(token.DocOffset, pred))
                {
                    newPred = updateBoundingMetadataForPred(pred, token);
                }
                else if (token.DocOffset.Start > pred.End)
                {
                    break;
                }
            }
            checkIfTokenMatchFound(newPred, noMatch);
            return (newPred, matchTokenIndex);
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
            for (int i = 0; i < predictions.Count; i++)
            {
                try
                {
                    (predictions[i], matchIndex) = matchPredToToken(predictions[i], ocrTokens.GetRange(matchIndex, ocrTokens.Count - matchIndex));
                    MappedPositions.Add(predictions[i]);
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
                        ErroredPredictions.Add(predictions[i]);
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
            Prediction startingPred = getFirstValidLineItemPred();
            float maxBot = startingPred.BbBot;
            int pageNumber = startingPred.PageNum;
            int rowNumber = 1;
            foreach (Prediction pred in MappedPositions)
            {
                if (pred.BbTop >= maxBot || pred.PageNum != pageNumber)
                {
                    rowNumber += 1;
                    pageNumber = pred.PageNum;
                    maxBot = pred.BbBot;
                }
                else
                {
                    maxBot = Math.Max(pred.BbBot, maxBot);
                }
                pred.RowNumber = rowNumber;
            }

        }

        /// <summary>
        /// After row number has been assigned to predictions, returns line item predictions
        /// as a list of lists where each list is a row.
        /// </summary>
        public List<List<Prediction>> groupedLineItems()
        {
            Dictionary<int, List<Prediction>> rows = new Dictionary<int, List<Prediction>>();
            foreach (Prediction pred in MappedPositions)
            {
                if (rows.ContainsKey(pred.RowNumber))
                {
                    rows[pred.RowNumber].Add(pred);
                }
                else
                {
                    rows.Add(pred.RowNumber, new List<Prediction>() { pred });
                }
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

        public Prediction getFirstValidLineItemPred()
        {
            if (MappedPositions.Count is 0)
            {
                throw new System.Exception("Whoops! You have no lineItemFields predictions. Did you run getBoundingBoxes?");
            }
            return MappedPositions[0];
        }

        public Prediction addBoundingMetadataToPred(Prediction pred, OcrToken token)
        {
            pred.BbTop = token.Position.bbTop;
            pred.BbBot = token.Position.bbBot;
            pred.BbLeft = token.Position.bbLeft;
            pred.BbRight = token.Position.bbRight;
            pred.PageNum = token.PageNum;
            return pred;
        }

        public Prediction updateBoundingMetadataForPred(Prediction pred, OcrToken token)
        {
            pred.BbTop = Math.Min(token.Position.bbTop, pred.BbTop);
            pred.BbBot = Math.Max(token.Position.bbBot, pred.BbBot);
            pred.BbLeft = Math.Min(token.Position.bbLeft, pred.BbLeft);
            pred.BbRight = Math.Max(token.Position.bbRight, pred.BbRight);
            return pred;
        }

        public static IEnumerable<(int, T)> Enumerate<T>(
            IEnumerable<T> input,
            int start = 0
        )
        {
            int i = start;
            foreach (var t in input)
            {
                yield return (i++, t);
            }
        }

    }
}