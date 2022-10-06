using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Newtonsoft.Json;

using IndicoToolkit.Types;

namespace IndicoToolkit.Association
{
    /// <summary>
    /// Class <c>Line Items</c> associates line items given extraction predictions and ondocument OCR tokens
    /// </summary>
    public class LineItems
    {
        public List<Prediction> Predictions { get; private set; }
        public List<string> LineItemFields { get; private set; }
        public List<MappedPrediction> MappedPositions { get; private set; }
        public List<Prediction> UnmappedPositions { get; private set; }
        public List<Prediction> ErroredPredictions { get; private set; }
        public LineItems(List<Prediction> predictions, List<string> lineItemFields)
        {
            Predictions = predictions;
            LineItemFields = lineItemFields;
            MappedPositions = new List<MappedPrediction>();
            UnmappedPositions = new List<Prediction>();
            ErroredPredictions = new List<Prediction>();
        }

        public Extractions updatedPredictions()
        {
            List<Prediction> updatedPredictions = MappedPositions.Concat(UnmappedPositions).Concat(ErroredPredictions).ToList();
            return new Extractions(updatedPredictions);
        }

        public int matchPredToToken(Prediction pred, List<OcrToken> OcrTokens)
        {
            bool noMatch = true;
            int matchTokenIndex = 0;
            foreach (OcrToken token in OcrTokens)
            {
                if (noMatch && sequencesOverlap(token.DocOffset, pred))
                {
                    addBoundingMetadataToPred(pred, token);
                    noMatch = false;
                    matchTokenIndex = token.Index;
                }
                else if (sequencesOverlap(token.DocOffset, pred))
                {
                    updateBoundingMetadataForPred(pred, token);
                }
                else if (token.DocOffset["start"] > pred.End)
                {
                    break;
                }
            }
            checkIfTokenMatchFound(pred, noMatch);
            return matchTokenIndex;
        }

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
                    MappedPositions.Add(pred);
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

        public void assignRowNumber()
        {

        }

        public List<Prediction> groupedLineItems()
        {
            return null;
        }

        public List<Prediction> removeUnneededPredictions(List<Prediction> predictions)
        {
            return null;
        }

        public bool isLineItemPred(Prediction pred)
        {
            return false;
        }

        public Prediction getFirstValidLineItemPred(Prediction pred)
        {
            return pred;
        }

        public void addBoundingMetadataToPred(Prediction pred, OcrToken token)
        {
            pred.BbTop
        }

        public void updateBoundingMetadataForPred(Prediction pred, OcrToken token)
        {

        }

    }
}