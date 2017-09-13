// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(Practitioner))]
    public class PractitionerToHealthVaultTests
    {
        private static Practitioner GetPractionerWithName(HumanName humanName)
        {
            var practitioner = new Practitioner();
            practitioner.Name = new System.Collections.Generic.List<HumanName>
            {
                humanName
            };
            return practitioner;
        }

        private static Practitioner GetSamplePractitioner()
        {
            return GetPractionerWithName(new HumanName());
        }

        [TestMethod]
        public void WhenPractitionerWithOutNameTransformedToHealthVault_ThenExceptionIsThrown()
        {
            var practitioner = new Practitioner();

            Assert.ThrowsException<NotSupportedException>(() =>
                practitioner.ToHealthVault());
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenTextIsCopiedToFullName()
        {
            var fullName = "John Mc Kense";
            var practitioner = GetPractionerWithName(new HumanName
            {
                Text = fullName
            });

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(fullName, person.Name.Full);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenTitleIsCopiedFromExtension()
        {
            var title = "Mr.";
            var practitioner = GetPractionerWithName(new HumanName
            {
                Extension = new System.Collections.Generic.List<Extension>
                    {
                        new Extension{
                            Url = HealthVaultExtensions.PatientTitle,
                            Value = new CodeableConcept
                            {
                                Text = title
                            }
                        }
                    }
            });

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(title, person.Name.Title.Text);
        }


        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenFirstNameIsCopiedFromGivenName()
        {
            var firstName = "John";
            var practitioner = GetPractionerWithName(new HumanName
            {
                GivenElement = new System.Collections.Generic.List<FhirString>
                {
                    new FhirString(firstName)
                }
            });

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(firstName, person.Name.First);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenMiddleNameIsCopiedFromGivenName()
        {
            var middleName = "Mc";
            var practitioner = GetPractionerWithName(new HumanName
            {
                GivenElement = new System.Collections.Generic.List<FhirString>
                {
                    new FhirString("firstName"),
                    new FhirString(middleName)
                }
            });

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(middleName, person.Name.Middle);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenLastNameIsCopiedFromFamilyName()
        {
            var lastName = "Mc";
            var practitioner = GetPractionerWithName(new HumanName
            {
                Family = lastName
            });

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(lastName, person.Name.Last);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenSuffixIsCopiedFromExtension()
        {
            var suffix = "Jr.";
            var practitioner = GetPractionerWithName(new HumanName
            {
                Extension = new System.Collections.Generic.List<Extension>
                    {
                        new Extension{
                            Url = HealthVaultExtensions.PatientSuffix,
                            Value = new CodeableConcept
                            {
                                Text = suffix
                            }
                        }
                    }
            });

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(suffix, person.Name.Suffix.Text);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenOrganisationIsCopiedFromExtension()
        {
            var organisation = "Interlake Hospitals";
            var practitioner = GetSamplePractitioner();
            practitioner.Extension = new System.Collections.Generic.List<Extension>
            {
                new Extension(HealthVaultExtensions.PersonOrganisation,new FhirString(organisation))
            };

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(organisation, person.Organization);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenQualificationIsCopiedToProffesionalTraining()
        {
            var training = "AtoZ Training";
            var practitioner = GetSamplePractitioner();
            practitioner.Qualification = new System.Collections.Generic.List<Practitioner.QualificationComponent>
            {
                new Practitioner.QualificationComponent
                {
                    Code = new CodeableConcept
                    {
                        Text = training
                    }
                }
            };

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(training, person.ProfessionalTraining);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenIdentifierIsCopiedToId()
        {
            var id = "ID2376ks";
            var practitioner = GetSamplePractitioner();
            practitioner.Identifier = new System.Collections.Generic.List<Identifier>
            {
                new Identifier
                {
                    Value = id
                }
            };

            var person = practitioner.ToHealthVault();

            Assert.AreEqual(id, person.PersonId);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenAddressIsCopiedToContactAddress()
        {
            const string description = "14503 Apt# 102";
            const bool isPrimary = true;
            const string street = "SE 140th St";//Required Multi
            const string city = "DC";//Required
            const string state = "WA";
            const string postCode = "98008";//Required
            const string country = "USA";//Required
            const string county = "County";
            var practitioner = GetSamplePractitioner();
            practitioner.Address = new System.Collections.Generic.List<Address>
            {
                new Address
                {
                    Text = description,
                    LineElement = new System.Collections.Generic.List<FhirString>
                    {
                        new FhirString(street)
                    },
                    City = city,
                    State =state,
                    PostalCode=postCode,
                    Country=country,
                    Extension = new System.Collections.Generic.List<Extension>
                    {
                        new Extension(HealthVaultExtensions.IsPrimary,new FhirBoolean(isPrimary))
                    },
                    District = county
                }
            };

            var person = practitioner.ToHealthVault();

            Assert.IsTrue(person.ContactInformation.Address.Any());

            var hvAddress = person.ContactInformation.Address.First();

            Assert.AreEqual(description, hvAddress.Description);
            Assert.AreEqual(street, hvAddress.Street.First());
            Assert.AreEqual(city, hvAddress.City);
            Assert.AreEqual(state, hvAddress.State);
            Assert.AreEqual(postCode, hvAddress.PostalCode);
            Assert.AreEqual(country, hvAddress.Country);
            Assert.AreEqual(county, hvAddress.County);
            Assert.AreEqual(isPrimary, hvAddress.IsPrimary);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenTelecomPhoneIsCopiedToContactPhone()
        {
            var description = "Office";
            var isPrimary = true;
            var number = "4254485432";
            var practitioner = GetSamplePractitioner();
            practitioner.Telecom = new System.Collections.Generic.List<ContactPoint>
            {
                new ContactPoint
                {
                    Rank = isPrimary?1:(int?)null,
                    Value = number,
                    System = ContactPoint.ContactPointSystem.Phone,
                    Extension = new System.Collections.Generic.List<Extension>
                    {
                        new Extension(HealthVaultExtensions.Description,new FhirString(description))
                    }
                }
            };

            var person = practitioner.ToHealthVault();

            Assert.IsTrue(person.ContactInformation.Phone.Any());

            var phone = person.ContactInformation.Phone.First();

            Assert.AreEqual(description, phone.Description);
            Assert.AreEqual(number, phone.Number);
            Assert.AreEqual(isPrimary, phone.IsPrimary);
        }

        [TestMethod]
        public void WhenPractitionerTransformedToHealthVault_ThenTelecomEmailIsCopiedToContactEmail()
        {
            var description = "Mail ID";
            var isPrimary = true;
            var address = "john@live.com";
            var practitioner = GetSamplePractitioner();
            practitioner.Telecom = new System.Collections.Generic.List<ContactPoint>
            {
                new ContactPoint
                {
                    Rank = isPrimary?1:(int?)null,
                    Value = address,
                    System = ContactPoint.ContactPointSystem.Email,
                    Extension = new System.Collections.Generic.List<Extension>
                    {
                        new Extension(HealthVaultExtensions.Description,new FhirString(description))
                    }
                }
            };

            var person = practitioner.ToHealthVault();

            Assert.IsTrue(person.ContactInformation.Email.Any());

            var email = person.ContactInformation.Email.First();

            Assert.AreEqual(description, email.Description);
            Assert.AreEqual(address, email.Address);
            Assert.AreEqual(isPrimary, email.IsPrimary);
        }
    }
}
