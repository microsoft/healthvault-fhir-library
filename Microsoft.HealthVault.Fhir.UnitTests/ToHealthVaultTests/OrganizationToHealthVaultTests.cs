// Copyright(c) Get Real Health.All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FhirOrganization = Hl7.Fhir.Model.Organization;
using HVOrganization = Microsoft.HealthVault.ItemTypes.Organization;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(FhirOrganization))]
    public class OrganizationToHealthVaultTests
    {
        [TestMethod]
        public void WhenOrganizationTransformedToHealthVault_ThenNameIsCopied()
        {
            const string organizationName = "Fabrikam analysis";
            var fhirOrganization = new FhirOrganization
            {
                Name = organizationName
            };

            var hvOrganization = fhirOrganization.ToHealthVault();

            Assert.AreEqual(organizationName, hvOrganization.Name);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToHealthVault_ThenTypeIsCopied()
        {
            const string organizationType = "Hospice";
            var fhirOrganization = new FhirOrganization
            {
                Name = "Fabrikam analysis",
                Type = new List<CodeableConcept>
                {
                    new CodeableConcept
                    {
                        Text = organizationType
                    }
                }
            };

            var hvOrganization = fhirOrganization.ToHealthVault();

            Assert.AreEqual(organizationType, hvOrganization.Type?.Text);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToHealthVault_ThenPhoneIsCopied()
        {
            const string phoneNumber = "123-12345-5";
            var fhirOrganization = new FhirOrganization
            {
                Name = "Fabrikam analysis",
                Telecom = new List<ContactPoint>
                {
                    new ContactPoint
                    {
                        Value = phoneNumber,
                        System = ContactPoint.ContactPointSystem.Phone
                    }
                }
            };

            var hvOrganization = fhirOrganization.ToHealthVault();

            Assert.AreEqual(phoneNumber, hvOrganization.Contact?.Phone?.First()?.Number);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToHealthVault_ThenEmailIsCopied()
        {
            const string emailAddress = "joe@example.com";
            var fhirOrganization = new FhirOrganization
            {
                Name = "Fabrikam analysis",
                Telecom = new List<ContactPoint>
                {
                    new ContactPoint
                    {
                        Value = emailAddress,
                        System = ContactPoint.ContactPointSystem.Email
                    }
                }
            };

            var hvOrganization = fhirOrganization.ToHealthVault();

            Assert.AreEqual(emailAddress, hvOrganization.Contact?.Email?.First()?.Address);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToHealthVault_ThenWebSiteIsCopied()
        {
            const string websiteUri = "http://www.fabrikam.com";
            var fhirOrganization = new FhirOrganization
            {
                Name = "Fabrikam analysis",
                Telecom = new List<ContactPoint>
                {
                    new ContactPoint
                    {
                        Value = websiteUri,
                        System = ContactPoint.ContactPointSystem.Url
                    }
                }
            };

            var hvOrganization = fhirOrganization.ToHealthVault();

            Assert.AreEqual(websiteUri, hvOrganization.Website?.OriginalString);
        }

        [TestMethod]
        public void WhenOrganizationTransformedToHealthVault_ThenAddressIsCopied()
        {
            const string cityName = "KEENE";
            var fhirOrganization = new FhirOrganization
            {
                Name = "Fabrikam analysis",
                Address = new List<Address>
                {
                    new Address
                    {
                        Line= new string [] { "3804 Lakeland ", "Terrace" },
                        City=cityName,
                        State="California",
                        Country = "US",
                        PostalCode = "93531"
                    }
                }
            };

            var hvOrganization = fhirOrganization.ToHealthVault();

            Assert.AreEqual(cityName, hvOrganization.Contact?.Address.First().City);
        }
    }
}
