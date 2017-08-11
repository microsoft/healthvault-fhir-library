// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class ObservationToHealthVaultTests
    {
        [TestMethod]
        public void WhenFhirWeightTransformedToHealthVault_ThenValuesEqual()
        {
            var observation = GetObservation("FhirWeight.json");

            var weight = observation.ToHealthVault() as Weight;
            Assert.IsNotNull(weight);
            Assert.AreEqual(67, weight.Value.Kilograms);
            Assert.AreEqual(UnitAbbreviations.Kilogram, weight.Value.DisplayValue.Units);
            Assert.AreEqual(UnitAbbreviations.Kilogram, weight.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void WhenFhirHeightTransformedToHealthvault_ThenValuesEqual()
        {
            var observation = GetObservation("FhirHeight.json");

            var height = observation.ToHealthVault() as Height;
            Assert.IsNotNull(height);
            Assert.AreEqual(1.73, height.Value.Meters);
            Assert.AreEqual(UnitAbbreviations.Meter, height.Value.DisplayValue.Units);
            Assert.AreEqual(UnitAbbreviations.Meter, height.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void WhenFhirWeightInLbsTransformed_ThenStoredInKg()
        {
            var observation = GetObservation("FhirWeightPounds.json");

            var weight = observation.ToHealthVault() as Weight;
            Assert.IsNotNull(weight);
            Assert.AreEqual(78.471480010000008, weight.Value.Kilograms);
            Assert.AreEqual(173, weight.Value.DisplayValue.Value);
            Assert.AreEqual(UnitAbbreviations.Pound, weight.Value.DisplayValue.Units);
            Assert.AreEqual(UnitAbbreviations.PoundUcum, weight.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void WhenFhirWeightInGramsTransformed_ThenStoredInKg()
        {
            var observation = GetObservation("FhirWeightGrams.json");

            var weight = observation.ToHealthVault() as Weight;
            Assert.IsNotNull(weight);
            Assert.AreEqual(75, weight.Value.Kilograms);
            Assert.AreEqual(75000, weight.Value.DisplayValue.Value);
            Assert.AreEqual(UnitAbbreviations.Gram, weight.Value.DisplayValue.Units);
            Assert.AreEqual(UnitAbbreviations.Gram, weight.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void WhenWeightTransformedToFhirTransformedToHealthvault_ThenValuesEqual()
        {
            ThingBase hvWeight = new Weight(new HealthServiceDateTime(), new WeightValue(75.5));

            var observation = hvWeight.ToFhir();

            var weight = observation.ToHealthVault() as Weight;
            Assert.IsNotNull(weight);
            Assert.AreEqual(75.5, weight.Value.Kilograms);
            Assert.AreEqual(UnitAbbreviations.Kilogram, weight.Value.DisplayValue.Units);
            Assert.AreEqual(UnitAbbreviations.Kilogram, weight.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void WhenFhirBloodGlucoseTransformedToHealthvault_ThenValuesEqual()
        {
            var observation = GetObservation("FhirBloodGlucose.json");

            var glucose = observation.ToHealthVault() as BloodGlucose;
            Assert.IsNotNull(glucose);
            Assert.AreEqual(6.3, glucose.Value.Value);
            Assert.AreEqual(UnitAbbreviations.MillimolesPerLiter.ToLower(), glucose.Value.DisplayValue.Units);
            Assert.AreEqual(UnitAbbreviations.MillimolesPerLiter, glucose.Value.DisplayValue.UnitsCode);           
        }

        [TestMethod]
        public void WhenMultipleFhirObservationsTransformedToHealthVault_TheValuesEqual()
        {
            var fhirParse = new FhirJsonParser();

            var samples = new List<string>()
            {
                SampleUtil.GetSampleContent("FhirBloodGlucose.json"),
                SampleUtil.GetSampleContent("FhirWeight.json"),
                SampleUtil.GetSampleContent("FhirHeight.json"),
            };

            var list = new List<ThingBase>();
            foreach (var sample in samples)
            {
                list.Add(fhirParse.Parse<Observation>(sample).ToHealthVault());
            }

            Assert.AreEqual(3, list.Count);

            Assert.AreEqual(BloodGlucose.TypeId, list[0].TypeId);
            Assert.IsTrue(list[0] is BloodGlucose);

            Assert.AreEqual(Weight.TypeId, list[1].TypeId);
            Assert.IsTrue(list[1] is Weight);

            Assert.AreEqual(Height.TypeId, list[2].TypeId);
            Assert.IsTrue(list[2] is Height);
        }

        private static Observation GetObservation(string fileName)
        {
            var json = SampleUtil.GetSampleContent(fileName);

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);
            return observation;
        }
    }
}
