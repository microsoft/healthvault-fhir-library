// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class PatientToHealthVaultTests
    {
        [TestMethod]
        public void WhenEmptyPatientTransformedToHealthVault_ThenEmptyList()
        {
            var patient = new Patient();

            var things = patient.ToHealthVault();

            Assert.AreEqual(0, things.Count);
        }

        [TestMethod]
        public void WhenFhirFullPatientTransformedToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirPatientFull.json");
            var fhirParser = new FhirJsonParser();
            var patient = fhirParser.Parse<Patient>(json);

            var things = patient.ToHealthVault();

            Assert.AreEqual(4, things.Count);

            foreach (var thingBase in things)
            {
                switch (thingBase)
                {
                    case BasicV2 basic:
                        Assert.IsNotNull(basic);
                        Assert.AreEqual(1975, basic.BirthYear);
                        Assert.AreEqual(Gender.Female, basic.Gender);
                        Assert.AreEqual(DayOfWeek.Sunday, basic.FirstDayOfWeek);
                        Assert.AreEqual("Redmond", basic.City);
                        Assert.AreEqual("Washington", basic.StateOrProvince.Text);
                        Assert.AreEqual("98052", basic.PostalCode);
                        Assert.AreEqual("United States of America", basic.Country.Text);
                        Assert.AreEqual(2, basic.Languages.Count);
                        Assert.AreEqual("English", basic.Languages[0].SpokenLanguage.Text);
                        Assert.AreEqual(true, basic.Languages[0].IsPrimary);
                        Assert.AreEqual("French", basic.Languages[1].SpokenLanguage.Text);
                        break;
                    case Contact contact:
                        Assert.IsNotNull(contact);

                        Assert.AreEqual(2, contact.ContactInformation.Address.Count);
                        var address1 = contact.ContactInformation.Address[0];
                        Assert.AreEqual("Home address", address1.Description);
                        Assert.AreEqual("123 Main St.", address1.Street[0]);
                        Assert.AreEqual("Apt. 3B", address1.Street[1]);
                        Assert.AreEqual("Redmond", address1.City);
                        Assert.AreEqual("WA", address1.State);
                        Assert.AreEqual("98052", address1.PostalCode);
                        Assert.AreEqual("USA", address1.Country);
                        Assert.AreEqual("King", address1.County);
                        Assert.AreEqual(true, address1.IsPrimary);

                        Assert.AreEqual(2, contact.ContactInformation.Email.Count);
                        var email1 = contact.ContactInformation.Email[0];
                        Assert.AreEqual("Address 1", email1.Description);
                        Assert.AreEqual("person1@example.com", email1.Address);
                        Assert.AreEqual(true, email1.IsPrimary);

                        Assert.AreEqual(2, contact.ContactInformation.Phone.Count);
                        var phone1 = contact.ContactInformation.Phone[0];
                        Assert.AreEqual("Phone 1", phone1.Description);
                        Assert.AreEqual("1-425-555-0100", phone1.Number);
                        Assert.AreEqual(true, phone1.IsPrimary);
                        break;
                    case PersonalImage personalImage:
                        Assert.IsNotNull(personalImage);

                        using (Stream currentImageStream = personalImage.ReadImage())
                        {
                           Assert.AreEqual(1757, currentImageStream.Length);
                        }
                        break;
                    case Personal personal:
                        Assert.IsNotNull(personal);
                        
                        Assert.AreEqual("Dr.", personal.Name.Title.Text);
                        Assert.AreEqual("John", personal.Name.First);
                        Assert.AreEqual("Phillip", personal.Name.Middle);
                        Assert.AreEqual("Doe", personal.Name.Last);
                        Assert.AreEqual("Junior", personal.Name.Suffix.Text);
                        Assert.AreEqual("Dr. John Phillip Doe, Jr.", personal.Name.Full);

                        Assert.AreEqual(new HealthServiceDateTime(new LocalDateTime(1975,2,5, 1,30, 34, 015)), personal.BirthDate);
                        Assert.AreEqual(new ApproximateDateTime(new LocalDateTime(2075,5,7,0,0)), personal.DateOfDeath);
                        Assert.AreEqual("Employed", personal.EmploymentStatus);
                        Assert.AreEqual("Other Race", personal.Ethnicity.Text);
                        Assert.AreEqual("College Graduate", personal.HighestEducationLevel.Text);
                        Assert.AreEqual(false, personal.IsDisabled);
                        Assert.AreEqual(true, personal.IsVeteran);
                        Assert.AreEqual("Never Married", personal.MaritalStatus.Text);
                        Assert.AreEqual("Organ Donor", personal.OrganDonor);
                        Assert.AreEqual("Agnostic", personal.Religion.Text);
                        Assert.AreEqual("000-12-3456", personal.SocialSecurityNumber);
                        break;
                }
            }
        }
    }
}
