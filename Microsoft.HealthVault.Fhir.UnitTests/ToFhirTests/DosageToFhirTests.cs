// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(Dosage))]
    public class DosageToFhirTests
    {
        [TestMethod]
        public void WhenDosageCreatedFromHV_RouteIsCopied()
        {
            const string routeText = "By mouth";
            var hvRoute = new CodableValue(routeText);

            var dosage = HealthVaultCodesToFhir.GetDosage(null, null, hvRoute);

            Assert.AreEqual(routeText, dosage.Route.Text);
        }

        [TestMethod]
        public void WhenDosageCreatedFromHV_DoseIsCopiedToDoseQuantity()
        {
            var hvDose = new GeneralMeasurement("3tablets/day");
            const int value = 3;
            const string code = "tablets";
            const string unitText = "Tablets";

            var dosage = HealthVaultCodesToFhir.GetDosage(hvDose, null, null);

            Assert.IsNull(dosage.Dose);

            hvDose.Structured.Add(
                new StructuredMeasurement(value,
                    new CodableValue(unitText,
                        new CodedValue(code,
                            vocabularyName: "medication-dose-units",
                            family: "wc",
                            version: "1"))));

            dosage = HealthVaultCodesToFhir.GetDosage(hvDose, null, null);

            var dose = dosage.Dose;

            Assert.IsNotNull(dose);
            Assert.IsInstanceOfType(dose, typeof(Quantity));

            var doseQuantity = dose as Quantity;

            Assert.AreEqual(value, doseQuantity.Value);
            Assert.AreEqual(code, doseQuantity.Code);
            Assert.AreEqual(unitText, doseQuantity.Unit);
        }

        [TestMethod]
        public void WhenDosageCreatedFromHV_FrequencyIsCopiedToTiming()
        {
            var frequency = new GeneralMeasurement("1 tablet every 8 hrs");
            const int value = 8;
            const string unitText = HealthVaultRecurrenceIntervalCodes.HourCode;
            const string code = "hour";

            var dosage = HealthVaultCodesToFhir.GetDosage(null, frequency, null);

            Assert.IsNull(dosage.Timing);

            frequency.Structured.Add(
                new StructuredMeasurement(value,
                    new CodableValue(unitText,
                        new CodedValue(code,
                            vocabularyName: HealthVaultVocabularies.RecurrenceIntervals,
                            family: HealthVaultVocabularies.Wc,
                            version: "1"))));

            dosage = HealthVaultCodesToFhir.GetDosage(null, frequency, null);

            var timing = dosage.Timing;

            Assert.IsNotNull(timing);

            Assert.AreEqual(value, timing.Repeat.Period);
            Assert.AreEqual("H", timing.Repeat.PeriodUnit.ToString());
        }

        [TestMethod]
        public void WhenDosageCreatedFromHV_UnknownFrequencyThowsError()
        {
            var frequency = new GeneralMeasurement("1 tablet every 8 hrs");

            frequency.Structured.Add(
                new StructuredMeasurement(8,
                    new CodableValue("Decade",
                        new CodedValue("decade", HealthVaultVocabularies.RecurrenceIntervals))));

            Assert.ThrowsException<NotImplementedException>(()
                => HealthVaultCodesToFhir.GetDosage(null, frequency, null));

            frequency.Structured.Add(
                new StructuredMeasurement(8,
                    new CodableValue("Hour",
                        new CodedValue("hour", HealthVaultVocabularies.Fhir))));

            Assert.ThrowsException<NotImplementedException>(()
                => HealthVaultCodesToFhir.GetDosage(null, frequency, null));
        }
    }
}
