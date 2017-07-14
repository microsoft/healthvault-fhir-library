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
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class ObservationToHealthVaultWeightTests
    {
        [TestMethod]
        public void WeightToHealthVault_Successful()
        {
            var json = SampleUtil.GetSampleContent("FhirWeight.json");

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);
            
            var weight = observation.ToHealthVault() as Weight;
            Assert.IsNotNull(weight);
            Assert.AreEqual(67, weight.Value.Kilograms);
            Assert.AreEqual("kg", weight.Value.DisplayValue.Units);
            Assert.AreEqual("kg", weight.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void WeightToObservationToHealthVault_Successful()
        {
            ThingBase hvWeight = new Weight(new HealthServiceDateTime(), new WeightValue(75.5));

            var observation = hvWeight.ToFhir();

            var weight = observation.ToHealthVault() as Weight;
            Assert.IsNotNull(weight);
            Assert.AreEqual(75.5, weight.Value.Kilograms);
            Assert.AreEqual("kg", weight.Value.DisplayValue.Units);
            Assert.AreEqual("kg", weight.Value.DisplayValue.UnitsCode);
        }

        [TestMethod]
        public void GivenTemperatureTransformedToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirBodyTemperature.json");

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);

            var vitalSigns = observation.ToHealthVault() as VitalSigns;
            Assert.IsNotNull(vitalSigns);
            Assert.AreEqual(1, vitalSigns.VitalSignsResults.Count);
            Assert.AreEqual(36.5, vitalSigns.VitalSignsResults[0].Value);
            Assert.AreEqual("celcius", vitalSigns.VitalSignsResults[0].Unit.Text);
        }

        [TestMethod]
        public void BloodGlucoseToHealthVault_Successful()
        {
            var json = SampleUtil.GetSampleContent("FhirBloodGlucose.json");

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);

            var glucose = observation.ToHealthVault() as BloodGlucose;
            Assert.IsNotNull(glucose);
            Assert.AreEqual(6.3, glucose.Value.Value);
            Assert.AreEqual("mmol/l", glucose.Value.DisplayValue.Units);
            Assert.AreEqual("mmol/L", glucose.Value.DisplayValue.UnitsCode);           
        }

        [TestMethod]
        public void MultipleObservationsToHealthVault_ReturnsCollection()
        {
            var fhirParse = new FhirJsonParser();

            var samples = new List<string>()
            {
                SampleUtil.GetSampleContent("FhirBloodGlucose.json"),
                SampleUtil.GetSampleContent("FhirWeight.json")
            };

            var list = new List<ThingBase>();
            foreach (var sample in samples)
            {
                list.Add(fhirParse.Parse<Observation>(sample).ToHealthVault());
            }

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(BloodGlucose.TypeId, list[0].TypeId);
            Assert.IsTrue(list[0] is BloodGlucose);
            Assert.AreEqual(Weight.TypeId, list[1].TypeId);
            Assert.IsTrue(list[1] is Weight);        
        }
    }
}
