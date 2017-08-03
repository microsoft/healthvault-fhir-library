// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class BodyDimensionToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultBodyDimensionTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var testDateTime = new DateTime(2017, 8, 2, 11, 13, 14);

            var bodyDimension = new BodyDimension(
                new ApproximateDateTime(testDateTime),
                new CodableValue("Left bicep size", 
                new CodedValue("BicepCircumferenceLeft","body-dimension-measurement-names","wc", "1")),new Length(0.15)
                );

            var observation = bodyDimension.ToFhir();

            Assert.IsNotNull(observation);
            Assert.AreEqual("body-dimension-measurement-names:BicepCircumferenceLeft", observation.Code.Coding[0].Code);

            var when = observation.Effective as FhirDateTime;
            Assert.IsNotNull(when);
            Assert.AreEqual(testDateTime, when.ToDateTime().Value);

            var value = observation.Value as Quantity;
            Assert.IsNotNull(value);
            Assert.AreEqual((decimal)0.15, value.Value);
            Assert.AreEqual("m", value.Unit);
        }
    }
}
