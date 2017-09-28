// Copyright(c) Get Real Health.All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using static Hl7.Fhir.Model.AllergyIntolerance;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class AllergyToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultAllergyTransformedToFhir_ThenValuesEquals()
        {
            var personId = Guid.NewGuid();
            Allergy allergy = new Allergy(new CodableValue("allergy other than medicinal agents", new CodedValue("V1509", "icd9cm", "icd", "1")));
            allergy.Reaction = new CodableValue("difficulty swallowing", new CodedValue("787.2", "reactions", "wc", "1"));
            allergy.FirstObserved = new ApproximateDateTime(new LocalDateTime(2017, 8, 3, 8, 30, 01));
            allergy.AllergenType = new CodableValue("animal", new CodedValue("animal", "allergen-type", "wc", "1"));
            allergy.AllergenCode = new CodableValue("animal", new CodedValue("animal", "allergen", "wc", "1"));
            allergy.TreatmentProvider = new PersonItem
            {
                PersonType = new CodableValue("Provider", new CodedValue("1", "person-types", "wc", "1")),
                Name = new Name("John Doe", "John", "T", "Doe"),
                Organization = "Care Hospitals Inc",
                PersonId = personId.ToString(),
                ProfessionalTraining = "Certified Medical Assistant",
                ContactInformation = new ContactInfo () 
            };
            allergy.TreatmentProvider.ContactInformation.Address.Add(new ItemTypes.Address
            {
                Street = { "1 Back Lane" },
                City = "Holmfirth",
                PostalCode = "HD7 1HQ",
                County = "HUDDERSFIELD",
                Country = "UK",
                Description = "business address",
            });

            allergy.Treatment = new CodableValue("Hotwatertreament");
            allergy.IsNegated = true;
            allergy.Key = new ThingKey(new Guid("1C855AC0-892A-4352-9A82-3DCBD22BF0BC"), new Guid("706CEAFA-D506-43A8-9758-441FD9C3D407"));
            allergy.CommonData.Note = "allergy other than medicinal agents";

            var allergyIntolerance = allergy.ToFhir() as AllergyIntolerance;
            var allergyExtension = allergyIntolerance.GetExtension(HealthVaultExtensions.Allergy);
            Assert.IsNotNull(allergyIntolerance);
            Assert.IsNotNull(allergyIntolerance.Code);
            Assert.IsNotNull(allergyIntolerance.Code.Coding);
            Assert.AreEqual(1, allergyIntolerance.Code.Coding.Count);
            var alleryDateTime = new DateTime(2017, 8, 3, 8, 30, 01);
            Assert.AreEqual(alleryDateTime.ToString("yyyy-MM-ddTHH:mm:sszzz"), allergyIntolerance.Onset.ToString());
            Assert.AreEqual(allergy.Name.Text, allergyIntolerance.Code.Text);
            Assert.AreEqual(allergy.Name[0].Value, allergyIntolerance.Code.Coding[0].Code);
            Assert.AreEqual(allergy.Name[0].Version, allergyIntolerance.Code.Coding[0].Version);
            Assert.AreEqual("http://healthvault.com/fhir/stu3/ValueSet/icd/icd9cm", allergyIntolerance.Code.Coding[0].System);
            Assert.AreEqual(allergy.Reaction.Text, allergyIntolerance.Reaction[0].Manifestation[0].Text);
            Assert.AreEqual("http://healthvault.com/fhir/stu3/ValueSet/wc/reactions", allergyIntolerance.Reaction[0].Manifestation[0].Coding[0].System);
            Assert.AreEqual(allergy.Reaction[0].Version, allergyIntolerance.Reaction[0].Manifestation[0].Coding[0].Version);
            Assert.AreEqual(allergy.Reaction[0].Value, allergyIntolerance.Reaction[0].Manifestation[0].Coding[0].Code);
            Assert.AreEqual("1c855ac0-892a-4352-9a82-3dcbd22bf0bc", allergyIntolerance.Id);
            Assert.AreEqual("706ceafa-d506-43a8-9758-441fd9c3d407", allergyIntolerance.VersionId);
            Assert.AreEqual("animal", allergyExtension.GetStringExtension(HealthVaultExtensions.AllergenType));
            Assert.AreEqual("Hotwatertreament", ((CodeableConcept)allergyExtension.GetExtension(HealthVaultExtensions.AllergyTreatement).Value).Text);
            Assert.IsFalse(allergyIntolerance.Asserter.IsNullOrEmpty());
            Assert.IsFalse(allergyIntolerance.Contained.IsNullOrEmpty());
            Assert.AreEqual(1, allergyIntolerance.Contained.Where(resource => resource.GetType().Equals(typeof(Practitioner)) && resource.Id.StartsWith("#practitioner-")).Count());
            Assert.IsNotNull(((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Name);
            Assert.AreEqual("John Doe",(((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Name[0].Text));
            Assert.AreEqual("Doe", (((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Name[0].Family));
            Assert.IsNotNull(((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Address);
            Assert.AreEqual("Care Hospitals Inc",((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Address[0].Text);
            Assert.AreEqual("work", ((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Address[0].Use.ToString().ToLower());
            Assert.AreEqual("Certified Medical Assistant", ((Practitioner)((DomainResource)allergyIntolerance.Contained[0])).Qualification[0].Code.Text.ToString());
            Assert.AreEqual("allergy other than medicinal agents", allergyIntolerance.Note[0].Text);
            Assert.AreEqual(AllergyIntoleranceClinicalStatus.Resolved, allergyIntolerance.ClinicalStatus);
            Assert.AreEqual(AllergyIntoleranceType.Allergy, allergyIntolerance.Type);
            Assert.AreEqual("animal", ((CodeableConcept)allergyExtension.GetExtension(HealthVaultExtensions.AllergenCode).Value).Text);
            Assert.AreEqual("animal", ((CodeableConcept)allergyExtension.GetExtension(HealthVaultExtensions.AllergenCode).Value).Coding[0].Code);
            Assert.AreEqual("1", ((CodeableConcept)allergyExtension.GetExtension(HealthVaultExtensions.AllergenCode).Value).Coding[0].Version);
            Assert.AreEqual("http://healthvault.com/fhir/stu3/ValueSet/wc/allergen", ((CodeableConcept)allergyExtension.GetExtension(HealthVaultExtensions.AllergenCode).Value).Coding[0].System);
        }
    }
}
