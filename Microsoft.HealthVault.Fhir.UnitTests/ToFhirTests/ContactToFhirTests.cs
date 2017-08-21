// Copyright (c) Microsoft Corporation.  All rights reserved.
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
    public class ContactToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultContactTransformedToFhir_ThenValuesEqual()
        {
            var contact = new Contact();
            contact.ContactInformation.Address.Add(new ItemTypes.Address
            {
                Street = { "123 Main St.", "Apt. 3B"},
                City = "Redmond",
                PostalCode = "98052",
                County = "King",
                State = "WA",
                Country = "USA",
                Description = "Home address",
                IsPrimary = true,
            });

            contact.ContactInformation.Address.Add(new ItemTypes.Address
            {
                Street = { "1 Back Lane" },
                City = "Holmfirth",
                PostalCode = "HD7 1HQ",
                County = "HUDDERSFIELD",
                Country = "UK",
                Description = "business address",
            });

            contact.ContactInformation.Email.Add(new Email
            {
                Address = "person1@example.com",
                Description = "Address 1",
                IsPrimary = true,
            });

            contact.ContactInformation.Email.Add(new Email
            {
                Address = "person2@example.com",
                Description = "Address 2",
            });

            contact.ContactInformation.Phone.Add(new Phone
            {
                Number = "1-425-555-0100",
                Description = "Phone 1",
                IsPrimary = true,
            });

            contact.ContactInformation.Phone.Add(new Phone
            {
                Number = "0491 570 156",
                Description = "Phone 2",
            });

            var patient = contact.ToFhir();
            Assert.IsNotNull(patient);

            Assert.AreEqual(2, patient.Address.Count);
            var address1 = patient.Address[0];
            Assert.AreEqual(2, address1.Line.Count());
            Assert.AreEqual("123 Main St.", address1.Line.First());
            Assert.AreEqual("Redmond", address1.City);
            Assert.AreEqual("King", address1.District);
            Assert.AreEqual("WA", address1.State);
            Assert.AreEqual("98052", address1.PostalCode);
            Assert.AreEqual("USA", address1.Country);
            Assert.AreEqual("Home address", address1.Text);
            Assert.AreEqual(true, ((FhirBoolean)address1.Extension.First(x => x.Url == HealthVaultExtensions.IsPrimary).Value).Value);

            Assert.AreEqual(4, patient.Telecom.Count);
            var email1 = patient.Telecom.First(x => x.System == ContactPoint.ContactPointSystem.Email);
            Assert.AreEqual("person1@example.com", email1.Value);
            Assert.AreEqual("Address 1", ((FhirString)email1.Extension.First(x => x.Url == HealthVaultExtensions.Description).Value).Value);
            Assert.AreEqual(1, email1.Rank);

            var phone1 = patient.Telecom.First(x => x.System == ContactPoint.ContactPointSystem.Phone);
            Assert.AreEqual("1-425-555-0100", phone1.Value);
            Assert.AreEqual("Phone 1", ((FhirString)phone1.Extension.First(x => x.Url == HealthVaultExtensions.Description).Value).Value);
            Assert.AreEqual(1, phone1.Rank);
        }
    }
}
