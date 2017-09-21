// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        public static Practitioner ToFhir(this PersonItem person)
        {
            return PersonItemToFhir.ToFhirInternal(person);
        }
    }
    public class PersonItemToFhir
    {
        internal static Practitioner ToFhirInternal(PersonItem person)
        {
            var practitioner = new Practitioner();

            HumanName fhirName = ToFhirInternal(person.Name);
            practitioner.Name = new System.Collections.Generic.List<HumanName> { fhirName };

            if (!string.IsNullOrEmpty(person.Organization))
            {
                practitioner.SetStringExtension(HealthVaultExtensions.Organization, person.Organization);
            }

            if (!string.IsNullOrEmpty(person.ProfessionalTraining))
            {
                var qualificationComponent = new Practitioner.QualificationComponent
                {
                    Code = new CodeableConcept
                    {
                        Text = person.ProfessionalTraining
                    }
                };
                practitioner.Qualification = new System.Collections.Generic.List<Practitioner.QualificationComponent>
                {
                    qualificationComponent
                };
            }

            if (!string.IsNullOrEmpty(person.PersonId))
            {
                practitioner.Identifier = new System.Collections.Generic.List<Identifier>
                {
                   new Identifier
                   {
                       Value = person.PersonId
                   }
                };
            }

            if (person.ContactInformation != null)
            {
                practitioner.Address.AddRange(
                    person.ContactInformation.Address.Select(address => ToFhirInternal(address)));
                practitioner.Telecom.AddRange(
                    person.ContactInformation.Phone.Select(phone => ToFhirInternal(phone)));
                practitioner.Telecom.AddRange(
                    person.ContactInformation.Email.Select(email => ToFhirInternal(email)));
            }

            return practitioner;
        }

        private static HumanName ToFhirInternal(Name hvName)
        {
            var fhirName = new HumanName
            {
                Text = hvName.Full,
                Family = hvName.Last
            };

            if (hvName.Title != null)
            {
                fhirName.Prefix = new List<string> { hvName.Title.Text };
            }

            AddGivenName(fhirName, hvName.First);
            AddGivenName(fhirName, hvName.Middle);

            if (hvName.Suffix != null)
            {
                fhirName.Suffix = new List<string> { hvName.Suffix.Text };
            }

            return fhirName;
        }

        private static void AddGivenName(HumanName fhirName, string first)
        {
            if (!string.IsNullOrEmpty(first))
            {
                fhirName.GivenElement.Add(new FhirString(first));
            }
        }

        private static Hl7.Fhir.Model.Address ToFhirInternal(ItemTypes.Address hvAddress)
        {
            var address = new Hl7.Fhir.Model.Address
            {
                Text = hvAddress.Description,
                Line = hvAddress.Street,
                City = hvAddress.City,
                PostalCode = hvAddress.PostalCode,
                District = hvAddress.County,
                State = hvAddress.State,
                Country = hvAddress.Country
            };

            if (hvAddress.IsPrimary.HasValue && hvAddress.IsPrimary.Value)
            {
                address.AddExtension(HealthVaultExtensions.IsPrimary, new FhirBoolean(true));
            }

            return address;
        }

        private static ContactPoint ToFhirInternal(Phone phone)
        {
            var contactPoint = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = phone.Number,
                Rank = phone.IsPrimary.HasValue && phone.IsPrimary.Value ? 1 : (int?)null,
            };

            if (!string.IsNullOrEmpty(phone.Description))
            {
                contactPoint.AddExtension(HealthVaultExtensions.Description, new FhirString(phone.Description));
            }

            return contactPoint;
        }

        private static ContactPoint ToFhirInternal(Email email)
        {
            var contactPoint = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Value = email.Address,
                Rank = email.IsPrimary.HasValue && email.IsPrimary.Value ? 1 : (int?)null,
            };

            if (!string.IsNullOrEmpty(email.Description))
            {
                contactPoint.AddExtension(HealthVaultExtensions.Description, new FhirString(email.Description));
            }

            return contactPoint;
        }
    }
}
