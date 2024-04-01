using Xunit;
using System;
using System.Collections.Generic;

using IndicoToolkit.Association;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class LineItemsFixture : IDisposable
    {
        public List<Prediction> ThreeRowInvoicePreds { get; set; }
        public List<OcrToken> ThreeRowInvoiceTokens { get; set; }
        public List<string> LineItemFields { get; set; }

        public LineItemsFixture()
        {
            ThreeRowInvoicePreds = Utils.LoadJsonIntoObject<Prediction>("data/line_items/three_row_invoice_preds.json");
            ThreeRowInvoiceTokens = Utils.LoadJsonIntoObject<OcrToken>("data/line_items/three_row_invoice_tokens.json");
            LineItemFields = new List<string>()
            {
                "work_order_number",
                "line_date",
                "work_order_tonnage"
            };
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

    }
    public class LineItemsTests : IClassFixture<LineItemsFixture>
    {

        LineItemsFixture Fixture { get; }

        public LineItemsTests(LineItemsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void AssignRowNumber_GrouperRowNumberFalse_ShouldAddToAll()
        {
            LineItems lineItems = new LineItems(Fixture.ThreeRowInvoicePreds, Fixture.LineItemFields);
            lineItems.getBoundingBoxes(Fixture.ThreeRowInvoiceTokens);
            lineItems.assignRowNumber();
            Assert.Equal(lineItems.MappedPositions.Count, 5);
            Assert.Equal(lineItems.UnmappedPositions.Count, 1);
            Assert.Equal(Fixture.ThreeRowInvoicePreds.Count, lineItems.updatedPredictions().Preds.Count);
            Assert.Equal(lineItems.UnmappedPositions[0].Label, "should be ignored");
            foreach (Prediction pred in lineItems.MappedPositions)
            {
                if (pred.Text.Contains("row 1"))
                {
                    Assert.Equal(pred.RowNumber, 1);
                }
                else if (pred.Text.Contains("row 2"))
                {
                    Assert.Equal(pred.RowNumber, 2);
                }
                else if (pred.Text.Contains("row 3"))
                {
                    Assert.Equal(pred.RowNumber, 3);
                }
            }
        }

        [Fact]
        public void GetBoundingBoxes_NoTokenMatch_ShouldRaiseException()
        {
            List<OcrToken> updatedTokens = Utils.LoadJsonIntoObject<OcrToken>("data/line_items/three_row_invoice_tokens.json"); ;
            updatedTokens.RemoveAt(0);
            LineItems lineItems = new LineItems(Fixture.ThreeRowInvoicePreds, Fixture.LineItemFields);
            Assert.Throws<NullReferenceException>(() => lineItems.getBoundingBoxes(updatedTokens));
        }

        [Fact]
        public void GetBoundingBoxes_NoTokenMatchRaiseForMatchFalse_ShouldNotRaiseException()
        {
            List<OcrToken> updatedTokens = Utils.LoadJsonIntoObject<OcrToken>("data/line_items/three_row_invoice_tokens.json"); ;
            updatedTokens.RemoveAt(0);
            LineItems lineItems = new LineItems(Fixture.ThreeRowInvoicePreds, Fixture.LineItemFields);
            lineItems.getBoundingBoxes(updatedTokens, raiseForNoMatch: false);
            lineItems.assignRowNumber();
            Assert.Equal(lineItems.ErroredPredictions.Count, 1);
            Assert.True(lineItems.ErroredPredictions[0].Error != "");
            Assert.Equal(lineItems.ErroredPredictions[0].RowNumber, 0);
        }

        [Fact]
        public void GetLineItemsInGroups_ShouldGroup()
        {
            LineItems lineItems = new LineItems(Fixture.ThreeRowInvoicePreds, Fixture.LineItemFields);
            lineItems.getBoundingBoxes(Fixture.ThreeRowInvoiceTokens, raiseForNoMatch: false);
            lineItems.assignRowNumber();
            List<List<Prediction>> groupedRows = lineItems.groupedLineItems();
            Assert.Equal(groupedRows.Count, 3);
        }

        [Fact]
        public void PredictionReordering_ShouldNotAffectGrouping()
        {
            List<Prediction> reorderedPredictions = Utils.LoadJsonIntoObject<Prediction>("data/line_items/three_row_invoice_preds.json");
            Prediction poppedPrediction = reorderedPredictions[reorderedPredictions.Count - 1];
            reorderedPredictions.RemoveAt(reorderedPredictions.Count - 1);
            reorderedPredictions.Insert(0, poppedPrediction);

            LineItems lineItems = new LineItems(reorderedPredictions, Fixture.LineItemFields);
            lineItems.getBoundingBoxes(Fixture.ThreeRowInvoiceTokens, raiseForNoMatch: false);
            lineItems.assignRowNumber();
            List<List<Prediction>> groupedRows = lineItems.groupedLineItems();
            Assert.Equal(groupedRows.Count, 3);
        }

        [Fact]
        public void MappedPositionsByPage_ShouldMap()
        {
            LineItems lineItems = new LineItems(Fixture.ThreeRowInvoicePreds, Fixture.LineItemFields);
            lineItems.getBoundingBoxes(Fixture.ThreeRowInvoiceTokens);
            Dictionary<int, List<Prediction>> mappedPositionsByPage = lineItems.mappedPositionsByPage();
            Assert.Equal(mappedPositionsByPage.Count, 2);
            Assert.Equal(mappedPositionsByPage[0].Count, 3);
            Assert.Equal(mappedPositionsByPage[1].Count, 2);
        }

    }
}