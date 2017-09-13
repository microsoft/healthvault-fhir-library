// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class PractitionerToHealthVault
    {
        public static ItemTypes.PersonItem ToHealthVault(this Hl7.Fhir.Model.Practitioner fhirPractitioner)
        {
            return fhirPractitioner.ToPerson();
        }

        private static ItemTypes.PersonItem ToPerson(this Hl7.Fhir.Model.Practitioner fhirPractitioner)
        {
            if (fhirPractitioner == null || fhirPractitioner.Name.IsNullOrEmpty())
                return null;
            
            var practitionerName = fhirPractitioner.Name.First(); // Let's consider just the first item

            //name
            var person = new ItemTypes.PersonItem()
            {
                PersonType = new CodableValue("Provider", new CodedValue("1", "person-types")),
                Name = new ItemTypes.Name()
                {
                    Last = practitionerName.Family,
                    Suffix = practitionerName.Suffix.Any() ? new ItemTypes.CodableValue(practitionerName.Suffix.First()) : null,
                    Title = practitionerName.Prefix.Any() ? new ItemTypes.CodableValue(practitionerName.Prefix.First()) : null,
                    First = practitionerName.Given.FirstOrDefault() ?? string.Empty,
                    Middle = practitionerName.Given.ElementAtOrDefault(1) ?? string.Empty,
                },
                ContactInformation = new ContactInfo()
            };            
            if (!string.IsNullOrEmpty(practitionerName.Text))
            {
                person.Name.Full = practitionerName.Text;
            }
                              

            //address
            if (!fhirPractitioner.Address.IsNullOrEmpty())
            {
                foreach (var address in fhirPractitioner.Address)
                {
                    var hvAddress = new ItemTypes.Address();

                    foreach (var line in address.Line)
                    {
                        hvAddress.Street.Add(line);
                    }
                    hvAddress.City = address.City;
                    hvAddress.State = address.State;
                    hvAddress.County = address.District;
                    hvAddress.PostalCode = address.PostalCode;
                    if (!string.IsNullOrEmpty(address.Country))
                    {
                        hvAddress.Country = address.Country;
                    }
                        

                    hvAddress.Description = address.Text;
                    hvAddress.IsPrimary = address.GetBoolExtension(HealthVaultExtensions.IsPrimary);

                    person.ContactInformation.Address.Add(hvAddress);
                }
            }

            //telecom
            if (!fhirPractitioner.Telecom.IsNullOrEmpty())
            {
                foreach (var contactPoint in fhirPractitioner.Telecom)
                {
                    switch (contactPoint.System)
                    {
                        case ContactPoint.ContactPointSystem.Email:
                            person.ContactInformation.Email.Add(ConvertContactPointToEmail(contactPoint));
                            break;
                        case ContactPoint.ContactPointSystem.Phone:
                            person.ContactInformation.Phone.Add(ConvertContactPointToPhone(contactPoint));
                            break;
                    }
                }
            }

            //qualification               
            if (!fhirPractitioner.Qualification.IsNullOrEmpty())
            {
                var firstQualification = fhirPractitioner.Qualification.FirstOrDefault(); //Let's take just the first one
                if(!string.IsNullOrEmpty(firstQualification.Code.Text))
                {
                    person.ProfessionalTraining = firstQualification.Code.Text;
                }
                else if(!firstQualification.Code.Coding.IsNullOrEmpty())
                {
                    person.ProfessionalTraining = firstQualification.Code.Coding.First().Display;
                }
            }
            
            return person;
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
