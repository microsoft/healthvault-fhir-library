// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class BodyCompositionToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultBodyCompositionTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var bodyComposition = new BodyComposition(
                new ApproximateDateTime(new LocalDateTime(2017, 8, 3, 8, 30, 01)),
                new CodableValue("Body fat percentage", new CodedValue("fat-percent", "body-composition-measurement-names", "wc", "1")),
                new BodyCompositionValue{ MassValue = new WeightValue(10), PercentValue = 0.15 }
            )
            {
                MeasurementMethod = new CodableValue("DXA/DEXA", new CodedValue("DXA", "body-composition-measurement-methods", "wc", "1")),
                Site = new CodableValue("Trunk", new CodedValue("Trunk", "body-composition-sites", "wc", "1"))
            };

            var observation = bodyComposition.ToFhir();
            
            Assert.IsNotNull(observation);
            Assert.AreEqual(HealthVaultThingTypeNameCodes.BodyComposition, observation.Code.Coding[0]);
            Assert.AreEqual("DXA", observation.Method.Coding[0].Code);
            Assert.AreEqual("Trunk", observation.BodySite.Coding[0].Code);
            Assert.AreEqual("fat-percent", observation.Component[0].Code.Coding[0].Code);

            var massValue = observation.Component[1].Value as Quantity;
            Assert.IsNotNull(massValue);
            Assert.AreEqual(10, massValue.Value);
            Assert.AreEqual(UnitAbbreviations.Kilogram, massValue.Unit);

            var percentageValue = observation.Component[2].Value as Quantity;
            Assert.IsNotNull(percentageValue);
            Assert.AreEqual((decimal)0.15, percentageValue.Value);
            Assert.AreEqual(UnitAbbreviations.Percent, percentageValue.Unit);
        }

        [TestMethod]
        public void WhenHeathVaultBodyCompositionOnlyWeightTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var bodyComposition = new BodyComposition(
                new ApproximateDateTime(new LocalDateTime(2017, 8, 3, 8, 30, 01)),
                new CodableValue("Body fat percentage", new CodedValue("fat-percent", "body-composition-measurement-names", "wc", "1")),
                new BodyCompositionValue { MassValue = new WeightValue(10) }
            );

            var observation = bodyComposition.ToFhir();
            Assert.IsNotNull(observation);
            Assert.AreEqual(1, observation.Code.Coding.Count);
            Assert.AreEqual("fat-percent", observation.Component[0].Code.Coding[0].Code);

            Assert.AreEqual(2, observation.Component.Count);
            var massValue = observation.Component[1].Value as Quantity;
            Assert.IsNotNull(massValue);
            Assert.AreEqual(10, massValue.Value);
            Assert.AreEqual(UnitAbbreviations.Kilogram, massValue.Unit);
        }
    }
}
