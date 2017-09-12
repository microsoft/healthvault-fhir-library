// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            Assert.AreEqual(readOnlyFlag.ToString(), extendable.GetStringExtension(HealthVaultExtensions.FlagsFhirExtensionName));
        }

        [TestMethod]
        public void WhenHealthvaultThingIsTransformedtoFhirDomainResource_ThenStateIsStoredInExtension()
        {
            var activeState = ThingState.Active;
            ThingBase thing = new Height(34.5); //By default thingstate is active. Use xml deserializer to create deleted thing

            IExtendable extendable = thing.ToFhir() as IExtendable;

            Assert.IsTrue(extendable.HasExtensions());
            Assert.AreEqual(activeState.ToString(), extendable.GetStringExtension(HealthVaultExtensions.StateFhirExtensionName));
        }

        [TestMethod]
        public void WhenAnUnsupportedHealthVaultThingIsTransformedToFhir_NotImplementedExceptionIsThrown()
        {
            ThingBase thing = new ActionPlanWrapper();

            Assert.ThrowsException<NotImplementedException>(() => { thing.ToFhir(); });
        }
    }
}
