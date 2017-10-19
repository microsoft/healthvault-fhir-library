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
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
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

            if (!fhirPractitioner.Name.Any())
            {
                throw new NotSupportedException($"{fhirPractitioner} needs to have a {nameof(HumanName)}");
            }

            var practitionerName = fhirPractitioner.Name.First(); // Let's consider just the first item

            //name
            var person = new ItemTypes.PersonItem()
            {
                PersonType = HealthVaultThingPersonTypesCodes.Provider,
                Name = practitionerName.ToHealthVault(),
                ContactInformation = new ContactInfo()
            };                                                      

            //address
            if (!fhirPractitioner.Address.IsNullOrEmpty())
            {
                foreach (var address in fhirPractitioner.Address)
                {
                    ItemTypes.Address hvAddress = address.ToHealthVault();

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
                            person.ContactInformation.Email.Add(contactPoint.ToHealthVault<Email>());
                            break;
                        case ContactPoint.ContactPointSystem.Phone:
                            person.ContactInformation.Phone.Add(contactPoint.ToHealthVault<Phone>());
                            break;
                    }
                }
            }

            //qualification               
            if (!fhirPractitioner.Qualification.IsNullOrEmpty())
            {
                var firstQualification = fhirPractitioner.Qualification.First(); //Let's take just the first one
                if(!string.IsNullOrEmpty(firstQualification.Code.Text))
                {
                    person.ProfessionalTraining = firstQualification.Code.Text;
                }
                else if(!firstQualification.Code.Coding.IsNullOrEmpty())
                {
                    person.ProfessionalTraining = firstQualification.Code.Coding.First().Display;
                }
            }

            person.Organization = fhirPractitioner.GetStringExtension(HealthVaultExtensions.Organization);

            if (fhirPractitioner.Identifier.Any())
            {
                person.PersonId = fhirPractitioner.Identifier.First().Value;
            }

            return person;
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
