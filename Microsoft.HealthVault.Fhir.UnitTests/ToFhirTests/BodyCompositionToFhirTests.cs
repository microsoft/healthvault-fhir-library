// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
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
            Assert.AreEqual("body-composition-measurement-methods:DXA", observation.Code.Coding[0].Code);
            Assert.AreEqual("body-composition-sites:Trunk", observation.Code.Coding[1].Code);
            Assert.AreEqual("body-composition-measurement-names:fat-percent", observation.Code.Coding[2].Code);

            var massValue = observation.Component[0].Value as Quantity;
            Assert.IsNotNull(massValue);
            Assert.AreEqual(10, massValue.Value);
            Assert.AreEqual("kg", massValue.Unit);

            var percentageValue = observation.Component[1].Value as Quantity;
            Assert.IsNotNull(percentageValue);
            Assert.AreEqual((decimal)0.15, percentageValue.Value);
            Assert.AreEqual("%", percentageValue.Unit);
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
            Assert.AreEqual("body-composition-measurement-names:fat-percent", observation.Code.Coding[0].Code);

            Assert.AreEqual(1, observation.Component.Count);
            var massValue = observation.Component[0].Value as Quantity;
            Assert.IsNotNull(massValue);
            Assert.AreEqual(10, massValue.Value);
            Assert.AreEqual("kg", massValue.Unit);
        }
    }
}
