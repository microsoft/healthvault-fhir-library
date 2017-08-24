// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        public void WhenHealthvaultThingIsTransformedToFhirObservation_ThenCreatedTimeStampIsStoredAsIssued()
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
