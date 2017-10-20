// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using FhirOrganization = Hl7.Fhir.Model.Organization;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(ItemTypes.Immunization))]
    public class ImmunizationToFhirTests
    {
        [TestMethod]
        public void WhenImmunizationTransformedToFhir_ThenValuesAreEqual()
        {
            var now = new LocalDateTime(2017, 9, 21, 18, 55, 10, 100);
            var afterOneMonth = new LocalDate(2017, 10, 20);
            var immunization = new ItemTypes.Immunization
            {
                Name = new CodableValue("cholera vaccine", new CodedValue {
                    Value="26",
                    Family = "HL7",
                    VocabularyName = "vaccines-cvx",
                    Version = "2.3 09_2008"
                }),
                DateAdministrated = new ApproximateDateTime(now),
                Administrator = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "Justin Case"
                    }
                },
                Manufacturer = new CodableValue("Baxter Healthcare Corporation", new CodedValue
                {
                    Value = "BA",
                    Family = "HL7",
                    VocabularyName = "vaccine-manufacturers-mvx",
                    Version = "2.3 07_2008"
                }),
                Lot = "AAJN11K",
                Route = new CodableValue("By mouth", new CodedValue
                {
                    Value = "po",
                    Family = "wc",
                    VocabularyName = "medication-routes",
                    Version = "2"
                }),
                ExpirationDate = new ApproximateDate(afterOneMonth),
                Sequence = "Last",
                AnatomicSurface = new CodableValue("Metacarpophalangeal joint structure of index finger", new CodedValue{
                    Value = "289002",
                    VocabularyName = "SnomedBodyLocation",
                    Family = "Snomed",
                    Version = "Jan2008"
                }),
                AdverseEvent = "Something bad happened",
                Consent = "A concent from parent goes here"
            };

            immunization.CommonData.Note = "Some note goes here";

            Hl7.Fhir.Model.Immunization fhirImmunization = immunization.ToFhir();            

            Assert.IsNotNull(fhirImmunization);
            Assert.AreEqual(immunization.Name.Text, fhirImmunization.VaccineCode.Text);
            Assert.AreEqual(now.ToDateTimeUnspecified(), fhirImmunization.DateElement.ToDateTimeOffset());

            var containedPractitioner = fhirImmunization.Contained.Where(resource => resource.GetType().Equals(typeof(Practitioner))).SingleOrDefault() as Practitioner;
            Assert.IsNotNull(containedPractitioner);
            Assert.AreEqual(immunization.Administrator.Name.Full, containedPractitioner.Name.Single().Text);

            var containedOrganization = fhirImmunization.Contained.Where(resource => resource.GetType().Equals(typeof(FhirOrganization))).SingleOrDefault() as FhirOrganization;
            Assert.IsNotNull(containedOrganization);
            Assert.AreEqual(immunization.Manufacturer.Text, containedOrganization.Name);

            Assert.AreEqual(immunization.Lot, fhirImmunization.LotNumber);
            Assert.AreEqual(immunization.Route.Text, fhirImmunization.Route.Text);
            Assert.AreEqual(afterOneMonth.ToDateTimeUnspecified().ToUniversalTime(), fhirImmunization.ExpirationDateElement.ToPartialDateTime().Value.ToUniversalTime());
            Assert.AreEqual(immunization.AnatomicSurface.Text, fhirImmunization.Site.Text);

            var immunizationExtension = fhirImmunization.GetExtension(HealthVaultExtensions.ImmunizationDetail);
            Assert.AreEqual(immunization.AdverseEvent, immunizationExtension.GetStringExtension(HealthVaultExtensions.ImmunizationDetailAdverseEvent));
            Assert.AreEqual(immunization.Consent, immunizationExtension.GetStringExtension(HealthVaultExtensions.ImmunizationDetailConcent));
            Assert.AreEqual(immunization.Sequence, immunizationExtension.GetStringExtension(HealthVaultExtensions.ImmunizationDetailSequence));

            Assert.IsFalse(fhirImmunization.Note.IsNullOrEmpty());
            Assert.AreEqual(immunization.CommonData.Note, fhirImmunization.Note.First().Text);
        }
    }
}
