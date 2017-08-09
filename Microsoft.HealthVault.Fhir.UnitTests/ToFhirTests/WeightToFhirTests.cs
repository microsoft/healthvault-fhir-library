// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class WeightToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultWeightTransformedToFhir_ThenValuesEqual()
        {
            // ToDo, once deserialization is fixed on SDK, use Deserialize
            //string xml = @"<?xml version='1.0' encoding='utf-16'?>
            //    <thing><thing-id version-stamp='d7f6d27a-4ad9-466c-afe7-03d95a0ef918'>31501360-362b-4791-ae12-141386ac5da6</thing-id><type-id name='Weight'>3d34d87e-7fc1-4153-800f-f56592cb0d17</type-id><thing-state>Active</thing-state><flags>0</flags><eff-date>2017-06-02T00:00:00Z</eff-date><created><timestamp>2017-06-02T22:26:22.07Z</timestamp><app-id name='Microsoft HealthVault'>9ca84d74-1473-471d-940f-2699cb7198df</app-id><person-id name='Health  Insights'>64141445-0ed8-47eb-a9bc-6081628f9357</person-id><access-avenue>Online</access-avenue><audit-action>Created</audit-action></created><updated><timestamp>2017-06-02T22:26:22.07Z</timestamp><app-id name='Microsoft HealthVault'>9ca84d74-1473-471d-940f-2699cb7198df</app-id><person-id name='Health  Insights'>64141445-0ed8-47eb-a9bc-6081628f9357</person-id><access-avenue>Online</access-avenue><audit-action>Created</audit-action></updated><data-xml><weight><when><date><y>2017</y><m>6</m><d>2</d></date></when><value><kg>70</kg><display units='kg' units-code='kg'>70</display></value></weight><common /></data-xml></thing>";
            // var weight = Weight.Deserialize(xml) as Weight;

            ThingBase weight = new Weight(new HealthServiceDateTime(), new WeightValue(75.5));

            var observation = weight.ToFhir() as Observation;
            Assert.IsNotNull(observation);
            Assert.AreEqual(HealthVaultVocabularies.BodyWeight, observation.Code);

            var value = observation.Value as Quantity;
            Assert.IsNotNull(value);
            Assert.AreEqual((decimal)75.5, value.Value);
            Assert.AreEqual("kg", value.Unit);            
        }
    }
}
