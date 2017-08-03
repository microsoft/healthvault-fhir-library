using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class ObservationToHealthVaultBodyCompositionTests
    {
        [TestMethod]
        public void WhenBodyCompositionToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirBodyComposition.json");

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);

            var bodyComposition = observation.ToHealthVault() as BodyComposition;
            Assert.IsNotNull(bodyComposition);
            Assert.AreEqual(bodyComposition.Site.Text, "Trunk");
            Assert.AreEqual(bodyComposition.MeasurementMethod.Text, "DXA/DEXA");
            Assert.AreEqual(bodyComposition.MeasurementName.Text, "Body fat percentage");
            Assert.AreEqual(bodyComposition.Value.MassValue.Kilograms, 10);
            Assert.AreEqual(bodyComposition.Value.PercentValue, 0.15);
        }
    }
}
