// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVOrganization = Microsoft.HealthVault.ItemTypes.Organization;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(HVOrganization))]
    public class OrganisationToFhirTests
    {
        [TestMethod]
        public void WhenOrganizationTransformedToFhir_ThenNameIsCopied()
        {
            const string organizationName = "Fabrikam analysis";
            var hvOrganization = new HVOrganization
            {
                Name = organizationName
            };

            var fhirOrganization = hvOrganization.ToFhir();

            Assert.AreEqual(organizationName, fhirOrganization.Name);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToFhir_ThenAddressIsCopied()
        {
            var contactInfo = new ContactInfo();
            var hvOrganization = new HVOrganization
            {
                Contact = contactInfo
            };

            var fhirOrganization = hvOrganization.ToFhir();

            Assert.IsFalse(fhirOrganization.Address.Any());

            const string cityName = "KEENE";
            contactInfo.Address.Add(new Address
            {
                Street = { "3804 Lakeland ", "Terrace" },
                City = cityName,
                State = "California",
                Country = "US",
                PostalCode = "93531"
            });

            fhirOrganization = hvOrganization.ToFhir();
            var address = fhirOrganization.Address.FirstOrDefault();

            Assert.IsNotNull(address);
            Assert.AreEqual(cityName, address.City);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToFhir_ThenEmailIsCopied()
        {
            var contactInfo = new ContactInfo();
            var hvOrganization = new HVOrganization
            {
                Contact = contactInfo
            };

            var fhirOrganization = hvOrganization.ToFhir();

            Assert.IsFalse(fhirOrganization.Telecom.Any());

            const string emailAddress = "joe@example.com";
            contactInfo.Email.Add(new Email
            {
                Address = emailAddress
            });

            fhirOrganization = hvOrganization.ToFhir();
            var email = fhirOrganization.Telecom.FirstOrDefault();

            Assert.IsNotNull(email);
            Assert.AreEqual(emailAddress, email.Value);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToFhir_ThenPhoneIsCopied()
        {
            var contactInfo = new ContactInfo();
            var hvOrganization = new HVOrganization
            {
                Contact = contactInfo
            };

            var fhirOrganization = hvOrganization.ToFhir();

            Assert.IsFalse(fhirOrganization.Telecom.Any());

            const string phoneNumber = "123-12345-5";
            contactInfo.Phone.Add(new Phone
            {
                Number = phoneNumber
            });

            fhirOrganization = hvOrganization.ToFhir();
            var phone = fhirOrganization.Telecom.FirstOrDefault();

            Assert.IsNotNull(phone);
            Assert.AreEqual(phoneNumber, phone.Value);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToFhir_ThenTypeIsCopied()
        {
            const string organizationType = "Hospice";
            var hvOrganization = new HVOrganization
            {
                Type = new CodableValue(organizationType)
            };

            var fhirOrganization = hvOrganization.ToFhir();

            Assert.AreEqual(organizationType, fhirOrganization.Type.First().Text);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToFhir_TheWebSiteIsCopied()
        {
            const string websiteUri = "http://www.fabrikam.com";
            var hvOrganization = new HVOrganization
            {
                Website = new System.Uri(websiteUri)
            };

            var fhirOrganization = hvOrganization.ToFhir();
            var website = fhirOrganization.Telecom.FirstOrDefault();

            Assert.IsNotNull(website);
            Assert.AreEqual(websiteUri, website.Value);
        }
    }
}
