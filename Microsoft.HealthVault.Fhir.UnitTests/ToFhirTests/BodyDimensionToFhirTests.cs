using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class BodyDimensionToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultBodyDimensionTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var bodyDimension = new BodyDimension(new ApproximateDateTime(new DateTime(2017,8,2,11,13,14)),new CodableValue("Left bicep size", new CodedValue("BicepCircumferenceLeft","body-dimension-measurement-names","wc", "1")),new Length(0.15));

            var observation = bodyDimension.ToFhir();

            Assert.IsNotNull(observation);
            Assert.AreEqual("body-dimension-measurement-names:BicepCircumferenceLeft", observation.Code.Coding[0].Code);

            var value = observation.Value as Quantity;
            Assert.IsNotNull(value);
            Assert.AreEqual((decimal)0.15, value.Value);
            Assert.AreEqual("m", value.Unit);
        }
    }
}
