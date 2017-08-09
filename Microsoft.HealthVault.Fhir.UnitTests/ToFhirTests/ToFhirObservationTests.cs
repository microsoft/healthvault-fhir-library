using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ToFhirObservationTests
    {
        [TestMethod]
        public void WhenHealthvaultThingIsTransformedToFhirObservation_thenCreatedTimeStampIsStoredAsIssued()
        {
            throw new AssertInconclusiveException("Need thing created using xml deserializer to set created audit info");
            var created = DateTimeOffset.Now;
            ThingBase thing = new Height(2.3);

            Observation observation = thing.ToFhir() as Observation;

            Assert.AreEqual(created, observation.Issued);
        }

        [TestMethod]
        public void WhenHealthVaultThingIsTransformedToFhirObservation_ThenObservationStatusIssetAsFinal()
        {
            ThingBase thing = new Height(34.4);

            Observation observation = thing.ToFhir() as Observation;

            Assert.AreEqual(ObservationStatus.Final, observation.Status);
        }

        [TestMethod]
        public void WhenHealthVaultThingIsTransformedToFhirObservation_ThenRelatedItemsAreCopied()
        {
            var keyId = Guid.NewGuid();
            ThingBase thing = new Height(2.3);
            thing.CommonData.RelatedItems.Add(new ThingRelationship()
            {
                ItemKey = new ThingKey(keyId)
            });

            Observation observation = thing.ToFhir() as Observation;

            Assert.IsTrue(observation.Related.Exists(comp
                => comp.Target.Reference.Contains(keyId.ToString())));
        }

        [TestMethod]
        public void WhenHealthVaultThingIsTransformedToFhirObservation_ThenSourceIsStoredAsDevice()
        {
            var deviceName = "HiteRiteStadioMeter";
            ThingBase thing = new Height(2.4);
            thing.CommonData.Source = deviceName;

            Observation observation = thing.ToFhir() as Observation;

            Assert.AreEqual(deviceName, observation.Device.Reference);
        }
    }
}
