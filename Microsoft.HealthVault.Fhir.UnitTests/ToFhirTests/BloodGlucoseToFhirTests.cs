// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class BloodGlucoseToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultBloodGlucoseTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var when = new HealthServiceDateTime();
            var bloodGlucose = new BloodGlucose
            {
                When = when,
                Value = new BloodGlucoseMeasurement(101),
                GlucoseMeasurementType = new CodableValue("Whole blood", "wb", "glucose-meaurement-type", "wc", "1"),
                OutsideOperatingTemperature = true,
                IsControlTest = false,
                ReadingNormalcy = Normalcy.Normal,
                MeasurementContext = new CodableValue("Before meal", "BeforeMeal", "glucose-measurement-context", "wc", "1"),
            };

            var observation = bloodGlucose.ToFhir();

            var json = FhirSerializer.SerializeToJson(observation);
            Assert.IsNotNull(observation);
            Assert.AreEqual(101, ((Quantity)observation.Value).Value);
            Assert.AreEqual("Whole blood", observation.Method.Text);

            var bloodGlucoseExtension = observation.GetExtension(HealthVaultExtensions.BloodGlucose);
            Assert.AreEqual("Before meal", bloodGlucoseExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.BloodGlucoseMeasurementContext).Text);
            Assert.AreEqual(true, bloodGlucoseExtension.GetBoolExtension(HealthVaultExtensions.OutsideOperatingTemperatureExtensionName));
            Assert.AreEqual(false, bloodGlucoseExtension.GetBoolExtension(HealthVaultExtensions.IsControlTestExtensionName));
            Assert.AreEqual("Normal", bloodGlucoseExtension.GetStringExtension(HealthVaultExtensions.ReadingNormalcyExtensionName));
        }
    }
}
