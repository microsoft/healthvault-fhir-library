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
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class PractitionerToHealthVault
    {
        public static PersonItem ToHealthVault(this Practitioner practitioner)
        {
            var person = new PersonItem();

            if (!practitioner.Name.Any())
            {
                throw new NotSupportedException($"{practitioner} needs to have a {nameof(HumanName)}");
            }
            person.Name = ToHealthVault(practitioner.Name.First());

            person.Organization = practitioner.GetStringExtension(HealthVaultExtensions.Organisation);

            if (practitioner.Qualification.Any())
            {
                var qualification = practitioner.Qualification.First();
                person.ProfessionalTraining = qualification.Code.Text
                    ?? qualification.Code.Coding.First(coding => string.IsNullOrEmpty(coding.Display))?.Display;
            }

            if (practitioner.Identifier.Any())
            {
                person.PersonId = practitioner.Identifier.First().Value;
            }

            if (practitioner.Telecom.Any() || practitioner.Address.Any())
            {
                var contactInfo = new ContactInfo();

                foreach (var fhirAddress in practitioner.Address)
                {
                    var address = new ItemTypes.Address
                    {
                        Description = fhirAddress.Text,
                        City = fhirAddress.City,
                        State = fhirAddress.State,
                        County = fhirAddress.District,
                        Country = fhirAddress.Country,
                        PostalCode = fhirAddress.PostalCode,
                        IsPrimary = fhirAddress.GetBoolExtension(HealthVaultExtensions.IsPrimary),
                    };
                    foreach (var line in fhirAddress.Line)
                    {
                        address.Street.Add(line);
                    }
                    contactInfo.Address.Add(address);
                }

                foreach (var contactPoint in practitioner.Telecom)
                {
                    switch (contactPoint.System)
                    {
                        case ContactPoint.ContactPointSystem.Email:
                            contactInfo.Email.Add(ConvertContactPointToEmail(contactPoint));
                            break;
                        case ContactPoint.ContactPointSystem.Phone:
                            contactInfo.Phone.Add(ConvertContactPointToPhone(contactPoint));
                            break;
                    }
                }

                person.ContactInformation = contactInfo;
            }

            return person;
        }

        private static Name ToHealthVault(HumanName fhirName)
        {
            var hvName = new Name
            {
                Last = fhirName.Family,
                First = fhirName.Given.FirstOrDefault() ?? string.Empty,
                Middle = fhirName.Given.ElementAtOrDefault(1) ?? string.Empty,
                Suffix = fhirName.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientSuffix)
                    ?.ToCodableValue(),//GetCodableValue(),
                Title = fhirName.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientTitle)
                    ?.ToCodableValue()//.GetCodableValue()
            };

            if (!string.IsNullOrEmpty(fhirName.Text))
            {
                hvName.Full = fhirName.Text;
            }
            else
            {
                //TODO Calculate Full Name
            }

            return hvName;
        }

        private static Email ConvertContactPointToEmail(ContactPoint contactPoint)
        {
            return new Email
            {
                Address = contactPoint.Value,
                IsPrimary = contactPoint.Rank.HasValue ? contactPoint.Rank == 1 : (bool?)null,
                Description = contactPoint.GetStringExtension(HealthVaultExtensions.Description),
            };
        }

        private static Phone ConvertContactPointToPhone(ContactPoint contactPoint)
        {
            return new Phone
            {
                Number = contactPoint.Value,
                IsPrimary = contactPoint.Rank.HasValue ? contactPoint.Rank == 1 : (bool?)null,
                Description = contactPoint.GetStringExtension(HealthVaultExtensions.Description),
            };
        }
    }
}
