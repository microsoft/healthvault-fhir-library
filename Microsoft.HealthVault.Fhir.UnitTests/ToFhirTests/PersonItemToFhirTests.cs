// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(PersonItem))]
    public class PersonItemToFhirTests
    {
        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenFullNameIsCopiedToText()
        {
            const string fullName = "John Mc Kense";
            PersonItem person = GetSamplePerson(fullName);

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Name.Any());
            Assert.AreEqual(fullName, practitioner.Name.First().Text);
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenTitleIsAddedAsAnExtension()
        {
            const string title = "Mr.";
            PersonItem person = GetSamplePerson();
            person.Name.Title = new CodableValue(title);

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Name.First().Extension.Any(ext
                => ext.Url == HealthVaultExtensions.PatientTitle));
            Assert.AreEqual(title, practitioner.Name.First()
                .GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientTitle).Text);
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenFirstNameIsAddedAsGivenName()
        {
            const string firstName = "John";
            PersonItem person = GetSamplePerson();
            person.Name.First = firstName;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Name.First().Given.Any());
            Assert.AreEqual(firstName, practitioner.Name.First().Given.First());
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenMiddleNameIsAddedAsGivenName()
        {
            const string middleName = "Mc";
            PersonItem person = GetSamplePerson();
            person.Name.Middle = middleName;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Name.First().Given.Any());
            Assert.AreEqual(middleName, practitioner.Name.First().Given.First());
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenLastNameIsAddedAsFamily()
        {
            const string lastName = "Mc";
            PersonItem person = GetSamplePerson();
            person.Name.Last = lastName;

            var practitioner = person.ToFhir();

            Assert.AreEqual(lastName, practitioner.Name.First().Family);
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenSuffixIsAddedAsAnExtension()
        {
            const string suffix = "Jr.";
            PersonItem person = GetSamplePerson();
            person.Name.Suffix = new CodableValue(suffix);

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Name.First().Extension.Any(ext
                    => ext.Url == HealthVaultExtensions.PatientSuffix));
            Assert.AreEqual(suffix, (practitioner.Name.First()
                .GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientSuffix).Text));
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenOrganisationIsAddedAsAnExtension()
        {
            const string organisation = "Interlake Hospitals";
            PersonItem person = GetSamplePerson();
            person.Organization = organisation;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Extension.Any(ext => ext.Url == HealthVaultExtensions.Organisation));
            Assert.AreEqual(organisation, practitioner.GetStringExtension(HealthVaultExtensions.Organisation));
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenProffessionalTrainingIsAddedAsQualification()
        {
            const string training = "AtoZ Training";
            PersonItem person = GetSamplePerson();
            person.ProfessionalTraining = training;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Qualification.Any());
            Assert.AreEqual(training, practitioner.Qualification.First().Code.Text);
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenIdIsCopiedToIdentifier()
        {
            const string id = "DOC3467";
            PersonItem person = GetSamplePerson();
            person.PersonId = id;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Identifier.Any());
            Assert.AreEqual(id, practitioner.Identifier.First().Value);
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenContactAddressIsCopiedToAddress()
        {
            const string description = "14503 Apt# 102";
            const bool isPrimary = true;
            const string street = "SE 140th St";//Required Multi
            const string city = "DC";//Required
            const string state = "WA";
            const string postCode = "98008";//Required
            const string country = "USA";//Required
            const string county = "County";
            PersonItem person = GetSamplePerson();
            var address = new ItemTypes.Address
            {
                Description = description,
                IsPrimary = isPrimary,
                City = city,
                State = state,
                PostalCode = postCode,
                Country = country,
                County = county
            };
            address.Street.Add(street);
            var contactInfo = new ContactInfo();
            contactInfo.Address.Add(address);
            person.ContactInformation = contactInfo;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Address.Any());

            var fhirAddress = practitioner.Address.First();

            Assert.AreEqual(description, fhirAddress.Text);
            Assert.AreEqual(street, fhirAddress.Line.First());
            Assert.AreEqual(city, fhirAddress.City);
            Assert.AreEqual(state, fhirAddress.State);
            Assert.AreEqual(postCode, fhirAddress.PostalCode);
            Assert.AreEqual(country, fhirAddress.Country);
            Assert.AreEqual(county, fhirAddress.District);

            Assert.IsTrue(fhirAddress.Extension.Any(ext => ext.Url == HealthVaultExtensions.IsPrimary));
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenContactPhoneIsCopiedToTelecom()
        {
            var description = "Office";
            var isPrimary = true;
            var number = "4254485432";
            PersonItem person = GetSamplePerson();
            var phone = new Phone
            {
                Description = description,
                IsPrimary = isPrimary,
                Number = number
            };
            var contactInfo = new ContactInfo();
            contactInfo.Phone.Add(phone);
            person.ContactInformation = contactInfo;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Telecom.Any());

            ContactPoint fhirEmail = practitioner.Telecom.First();
            Assert.AreEqual(1, fhirEmail.Rank);
            Assert.AreEqual(number, fhirEmail.Value);
            Assert.AreEqual(ContactPoint.ContactPointSystem.Phone, fhirEmail.System);
            Assert.IsTrue(fhirEmail.Extension.Any(ext => ext.Url == HealthVaultExtensions.Description));
        }

        [TestMethod]
        public void WhenPersonItemTransformedToFhir_ThenContactEmailIsCopiedToTelecom()
        {
            var description = "Mail ID";
            var isPrimary = true;
            var address = "john@live.com";
            PersonItem person = GetSamplePerson();
            var email = new Email
            {
                Description = description,
                IsPrimary = isPrimary,
                Address = address
            };
            var contactInfo = new ContactInfo();
            contactInfo.Email.Add(email);
            person.ContactInformation = contactInfo;

            var practitioner = person.ToFhir();

            Assert.IsTrue(practitioner.Telecom.Any());

            ContactPoint fhirEmail = practitioner.Telecom.First();
            Assert.AreEqual(1, fhirEmail.Rank);
            Assert.AreEqual(address, fhirEmail.Value);
            Assert.AreEqual(ContactPoint.ContactPointSystem.Email, fhirEmail.System);
            Assert.IsTrue(fhirEmail.Extension.Any(ext => ext.Url == HealthVaultExtensions.Description));
        }

        private static PersonItem GetSamplePerson(string fullName = "John Mc Kense")
        {
            return new PersonItem()
            {
                Name = new Name
                {
                    Full = fullName
                }
            };
        }
    }
}
