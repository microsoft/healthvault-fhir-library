// Copyright(c) Get Real Health.All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class AllergyInToleranceToHealthVault
    {
        [TestMethod]
        public void WhenAllergyInToleranceToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirAllergy.json");
            var fhirParser = new FhirJsonParser();
            var fhirAllergy = fhirParser.Parse<AllergyIntolerance>(json);

            var allergy = fhirAllergy.ToHealthVault();
            Assert.IsNotNull(allergy);
            Assert.AreEqual(new Guid("1c855ac0-892a-4352-9a82-3dcbd22bf0bc") , allergy.Key.Id);
            Assert.AreEqual(new Guid("706ceafa-d506-43a8-9758-441fd9c3d407") , allergy.Key.VersionStamp);
            Assert.AreEqual(fhirAllergy.Code.Text, allergy.Name.Text);
            Assert.AreEqual(fhirAllergy.Code.Coding[0].Code, allergy.Name[0].Value);
            Assert.AreEqual(fhirAllergy.Code.Coding[0].System, allergy.Name[0].Family);
            Assert.AreEqual("Food", allergy.AllergenType.Text);
            Assert.AreEqual("39579001", allergy.Reaction[0].Value);
            Assert.AreEqual("Anaphylactic reaction", allergy.Reaction.Text);
            Assert.AreEqual(2004,allergy.FirstObserved.ApproximateDate.Year );
            Assert.AreEqual(false, allergy.IsNegated.Value);
            Assert.AreEqual("Hotwatertreament", allergy.Treatment.Text);
            Assert.AreEqual("animal", allergy.AllergenCode.Text);
            Assert.AreEqual("wc", allergy.AllergenCode[0].Family);
            Assert.AreEqual("animal", allergy.AllergenCode[0].Value);
            Assert.AreEqual("1", allergy.AllergenCode[0].Version);
            Assert.IsNotNull(allergy.TreatmentProvider);
            Assert.AreEqual("John Doe", allergy.TreatmentProvider.Name.ToString());
            Assert.AreEqual("1 Back Lane", allergy.TreatmentProvider.ContactInformation.Address[0].Street[0]);
            Assert.AreEqual("Holmfirth", allergy.TreatmentProvider.ContactInformation.Address[0].City);
            Assert.AreEqual("HD7 1HQ", allergy.TreatmentProvider.ContactInformation.Address[0].PostalCode);
            Assert.AreEqual("UK", allergy.TreatmentProvider.ContactInformation.Address[0].Country);
        }
    }
}
