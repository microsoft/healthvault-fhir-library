// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class BloodPressureToTests
    {
        [TestMethod]
        public void WhenHealthVaultBloodPressureTransformedToFhir_ThenCodeAndValuesEqual()
        {
            // ToDo, once deserialization is fixed on SDK, use Deserialize

            var bloodPressure = new BloodPressure(new HealthServiceDateTime(), 120, 60);

            var observation = bloodPressure.ToFhir();
            Assert.IsNotNull(observation);
            Assert.IsNotNull(observation.Code);
            Assert.IsNotNull(observation.Code.Coding);
            Assert.AreEqual(2, observation.Code.Coding.Count);
            Assert.AreEqual(HealthVaultVocabularies.BloodPressure.Coding[0], observation.Code.Coding[0]);
            Assert.AreEqual(HealthVaultVocabularies.BloodPressure.Coding[1], observation.Code.Coding[1]);

            var components = observation.Component;
            Assert.AreEqual(2, components.Count);

            var systolic = components.FirstOrDefault(c => c.Code.Coding[0].Code == HealthVaultVitalStatisticsCodes.BloodPressureSystolic.Code);
            Assert.IsNotNull(systolic);
            var systolicValue = systolic.Value as Quantity;
            Assert.IsNotNull(systolicValue);
            Assert.AreEqual(120, systolicValue.Value);
            Assert.AreEqual(UnitAbbreviations.MillimeterOfMecury, systolicValue.Unit);


            var diastolic = components.FirstOrDefault(c => c.Code.Coding[0].Code == HealthVaultVitalStatisticsCodes.BloodPressureDiastolic.Code);
            Assert.IsNotNull(diastolic);
            var diastolicValue = diastolic.Value as Quantity;
            Assert.IsNotNull(diastolicValue);
            Assert.AreEqual(60, diastolicValue.Value);
            Assert.AreEqual(UnitAbbreviations.MillimeterOfMecury, diastolicValue.Unit);
        }

        [TestMethod]
        public void WhenHealthVaultBloodPressureWithPulseTransformedToFhir_ThenCodeAndValuesEqual()
        {
            // ToDo, once deserialization is fixed on SDK, use Deserialize

            var bloodPressure = new BloodPressure(new HealthServiceDateTime(), 120, 60)
            {
                Pulse = 55
            };

            var observation = bloodPressure.ToFhir();
            Assert.IsNotNull(observation);
            Assert.IsNotNull(observation.Code);
            Assert.IsNotNull(observation.Code.Coding);
            Assert.AreEqual(2, observation.Code.Coding.Count);
            Assert.AreEqual(HealthVaultVocabularies.BloodPressure.Coding[0], observation.Code.Coding[0]);
            Assert.AreEqual(HealthVaultVocabularies.BloodPressure.Coding[1], observation.Code.Coding[1]);            

            var components = observation.Component;
            Assert.AreEqual(3, components.Count);

            var systolic = components.FirstOrDefault(c => c.Code.Coding[0].Code == HealthVaultVitalStatisticsCodes.BloodPressureSystolic.Code);
            Assert.IsNotNull(systolic);
            var systolicValue = systolic.Value as Quantity;
            Assert.IsNotNull(systolicValue);
            Assert.AreEqual(120, systolicValue.Value);
            Assert.AreEqual(UnitAbbreviations.MillimeterOfMecury, systolicValue.Unit);

            var diastolic = components.FirstOrDefault(c => c.Code.Coding[0].Code == HealthVaultVitalStatisticsCodes.BloodPressureDiastolic.Code);
            Assert.IsNotNull(diastolic);
            var diastolicValue = diastolic.Value as Quantity;
            Assert.IsNotNull(diastolicValue);
            Assert.AreEqual(60, diastolicValue.Value);
            Assert.AreEqual(UnitAbbreviations.MillimeterOfMecury, diastolicValue.Unit);

            var hr = components.FirstOrDefault(c => c.Code.Coding[0].Code == HealthVaultVitalStatisticsCodes.HeartRate.Code);
            Assert.IsNotNull(hr);
            var hrValue = hr.Value as Quantity;
            Assert.IsNotNull(hrValue);
            Assert.AreEqual(55, hrValue.Value);
            Assert.AreEqual(UnitAbbreviations.PerMinute, hrValue.Unit);
        }
    }
}
