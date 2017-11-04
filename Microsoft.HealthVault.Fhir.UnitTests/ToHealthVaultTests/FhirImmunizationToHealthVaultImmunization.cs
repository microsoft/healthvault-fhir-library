// Copyright (c) Get Real Health.  All rights reserved.
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

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(Immunization))]
    public class FhirImmunizationToHealthVaultImmunization
    {
        [TestMethod]
        public void WhenExampleFhirImmunizationToHealthVaultImmunization_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirImmunization.json");

            var fhirParser = new FhirJsonParser();
            var fhirImmunization = fhirParser.Parse<Immunization>(json);

            var hvImmunization = fhirImmunization.ToHealthVault();

            Assert.IsNotNull(hvImmunization);
            Assert.AreEqual("Fluvax (Influenza)", hvImmunization.Name.Text);
            Assert.IsNotNull(hvImmunization.DateAdministrated.ApproximateDate);
            Assert.AreEqual(2013, hvImmunization.DateAdministrated.ApproximateDate.Year);
            Assert.IsNotNull(hvImmunization.DateAdministrated.ApproximateDate.Month);
            Assert.AreEqual(1, hvImmunization.DateAdministrated.ApproximateDate.Month.Value);
            Assert.IsNotNull(hvImmunization.DateAdministrated.ApproximateDate.Day);
            Assert.AreEqual(10, hvImmunization.DateAdministrated.ApproximateDate.Day.Value);
            Assert.AreEqual("Injection, intramuscular", hvImmunization.Route.Text);
            Assert.AreEqual("Notes on adminstration of vaccine", hvImmunization.CommonData.Note);
            Assert.AreEqual("AAJN11K", hvImmunization.Lot);
            Assert.IsNotNull(hvImmunization.ExpirationDate);
            Assert.AreEqual(2015, hvImmunization.ExpirationDate.Year);
            Assert.IsNotNull(hvImmunization.ExpirationDate.Month);
            Assert.AreEqual(2, hvImmunization.ExpirationDate.Month.Value);
            Assert.IsNotNull(hvImmunization.ExpirationDate.Day);
            Assert.AreEqual(15, hvImmunization.ExpirationDate.Day.Value);
            Assert.AreEqual("left arm", hvImmunization.AnatomicSurface.Text);

            Assert.IsNull(hvImmunization.Administrator);
            Assert.IsNull(hvImmunization.Manufacturer);
        }

        [TestMethod]
        public void WhenFhirImmunizationWithExtensionToHealthVaultImmunization_ThenExtendedValuesAreParsedAndAreEqual()
        {
            var json = SampleUtil.GetSampleContent("HealthVaultToFhirImmunization.json");

            var fhirParser = new FhirJsonParser();
            var fhirImmunization = fhirParser.Parse<Immunization>(json);

            var hvImmunization = fhirImmunization.ToHealthVault();

            Assert.IsNotNull(hvImmunization);
            Assert.AreEqual("cholera vaccine", hvImmunization.Name.Text);
            Assert.IsNotNull(hvImmunization.DateAdministrated.ApproximateDate);
            Assert.AreEqual(2017, hvImmunization.DateAdministrated.ApproximateDate.Year);
            Assert.IsNotNull(hvImmunization.DateAdministrated.ApproximateDate.Month);
            Assert.AreEqual(9, hvImmunization.DateAdministrated.ApproximateDate.Month.Value);
            Assert.IsNotNull(hvImmunization.DateAdministrated.ApproximateDate.Day);
            Assert.AreEqual(21, hvImmunization.DateAdministrated.ApproximateDate.Day.Value);
            Assert.AreEqual("By mouth", hvImmunization.Route.Text);            
            Assert.AreEqual("AAJN11K", hvImmunization.Lot);
            Assert.IsNotNull(hvImmunization.ExpirationDate);
            Assert.AreEqual(2017, hvImmunization.ExpirationDate.Year);
            Assert.IsNotNull(hvImmunization.ExpirationDate.Month);
            Assert.AreEqual(10, hvImmunization.ExpirationDate.Month.Value);
            Assert.IsNotNull(hvImmunization.ExpirationDate.Day);
            Assert.AreEqual(20, hvImmunization.ExpirationDate.Day.Value);
            Assert.AreEqual("Metacarpophalangeal joint structure of index finger", hvImmunization.AnatomicSurface.Text);

            Assert.IsNotNull(hvImmunization.Administrator);
            Assert.AreEqual("Justin Case", hvImmunization.Administrator.Name.Full);
            Assert.IsNotNull(hvImmunization.Manufacturer);
            Assert.AreEqual("Baxter Healthcare Corporation", hvImmunization.Manufacturer.Text);

            //Extensions
            Assert.AreEqual("Last", hvImmunization.Sequence);
            Assert.AreEqual("A concent from parent goes here", hvImmunization.Consent);
            Assert.AreEqual("Something bad happened", hvImmunization.AdverseEvent);
        }
    }
}
