using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ToFhirResourceTests
    {
        [TestMethod]
        public void WhenHealthvaultThingIsTransformedtoFhirResource_ThenIdIsCopied()
        {
            var keyId = Guid.NewGuid();
            ThingBase thing = new Height(2.3)
            {
                Key = new ThingKey(keyId)
            };

            Resource resource = thing.ToFhir();

            Assert.AreEqual(keyId.ToString(), resource.Id);
        }

        [TestMethod]
        public void WhenHealthvaultThingIsTransformedtoFhirResource_ThenVersionStampIsStoredInMeta()
        {
            var versionStamp = Guid.NewGuid();
            ThingBase thing = new Height(2.3)
            {
                Key = new ThingKey(Guid.NewGuid(), versionStamp)
            };

            Resource resource = thing.ToFhir();

            Assert.AreEqual(versionStamp.ToString(), resource.Meta.VersionId);
        }

        [TestMethod]
        public void WhenHealthvaultThingIsTransformedtoFhirResource_ThenLastUpdatedTimeStampIsStoredInMeta()
        {
            throw new AssertInconclusiveException("Need thing created using xml deserializer to set last updated audit info");
            var lastUpdated = DateTimeOffset.Now;
            ThingBase thing = new Height(2.4);

            Resource resource = thing.ToFhir();

            Assert.AreEqual(lastUpdated, resource.Meta.LastUpdated);
        }

        [TestMethod]
        public void WhenHealthvaultThingIsTransformedtoFhirDomainResource_ThenFlagsAreStoredInExtension()
        {
            var readOnlyFlag = ThingFlags.ReadOnly;
            ThingBase thing = new Height(34.5)
            {
                Flags = readOnlyFlag
            };

            IExtendable extendable = thing.ToFhir() as IExtendable;

            Assert.IsTrue(extendable.HasExtensions());
            Assert.AreEqual(readOnlyFlag.ToString(), extendable.GetStringExtension(HealthVaultVocabularies.FlagsFhirExtensionName));
        }

        [TestMethod]
        public void WhenHealthvaultThingIsTransformedtoFhirDomainResource_ThenStateIsStoredInExtension()
        {
            var activeState = ThingState.Active;
            ThingBase thing = new Height(34.5); //By default thingstate is active. Use xml deserializer to create deleted thing

            IExtendable extendable = thing.ToFhir() as IExtendable;

            Assert.IsTrue(extendable.HasExtensions());
            Assert.AreEqual(activeState.ToString(), extendable.GetStringExtension(HealthVaultVocabularies.StateFhirExtensionName));
        }

    }
}
