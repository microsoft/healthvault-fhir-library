// Copyright(c) Get Real Health.All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVItemTypes = Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class ConditionToHealthVaultCondition
    {
        [TestMethod]
        public void WhenConditionToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirCondition.json");
            var fhirParser = new FhirJsonParser();
            var fhirCondition = fhirParser.Parse<Condition>(json);
            var cd = fhirCondition.ToHealthVault();
            Assert.IsNotNull(cd);
            Assert.AreEqual("Acute renal insufficiency specified as due to procedure", cd.Name.Text);
            Assert.AreEqual(new HVItemTypes.ApproximateDateTime() {ApproximateDate = new HVItemTypes.ApproximateDate(2013, 03, 11)}, cd.OnsetDate);
            Assert.AreEqual("In Control", cd.StopReason);
            Assert.AreEqual("intermittent", cd.Status.Text);
            Assert.AreEqual("The patient is anuric.The patient state is critcal.",cd.CommonData.Note);
            Assert.AreEqual(new HVItemTypes.ApproximateDateTime() { ApproximateDate = new HVItemTypes.ApproximateDate(2015)}, cd.StopDate);
            Assert.AreEqual("patient via InstantPHR patient portal", cd.CommonData.Source);
        }
             
    }
}
