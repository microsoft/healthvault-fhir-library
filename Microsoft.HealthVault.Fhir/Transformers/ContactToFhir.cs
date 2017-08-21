// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Patient ToFhir(this Contact contact)
        {
            return ContactToFhir.ToFhirInternal(contact, ThingBaseToFhir.ToFhirInternal<Patient>(contact));
        }

        // Register the type on the generic ThingToFhir partial class
        public static Patient ToFhir(this Contact contact, Patient patient)
        {
            return ContactToFhir.ToFhirInternal(contact, patient);
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault contact data types into FHIR patients
    /// </summary>
    internal static class ContactToFhir
    {
        internal static Patient ToFhirInternal(Contact contact, Patient patient)
        {
            if (contact.ContactInformation == null)
            {
                return patient;
            }

            if (!contact.ContactInformation.Address.IsNullOrEmpty())
            {
                foreach (var address in contact.ContactInformation.Address)
                {
                    patient.Address.Add(ConvertAddress(address));
                }
            }

            if (!contact.ContactInformation.Email.IsNullOrEmpty())
            {
                foreach (var email in contact.ContactInformation.Email)
                {
                    patient.Telecom.Add(ConvertEmail(email));
                }
            }

            if (!contact.ContactInformation.Phone.IsNullOrEmpty())
            {
                foreach (var phone in contact.ContactInformation.Phone)
                {
                    patient.Telecom.Add(ConvertPhone(phone));
                }
            }

            return patient;
        }

        private static Hl7.Fhir.Model.Address ConvertAddress(ItemTypes.Address hvAddress)
        {
            var address = new Hl7.Fhir.Model.Address
            {
                Text = hvAddress.Description,
                Line = hvAddress.Street,
                City = hvAddress.City,
                PostalCode = hvAddress.PostalCode,
                District = hvAddress.County,
                State = hvAddress.State,
                Country = hvAddress.Country,
            };

            if (hvAddress.IsPrimary.HasValue && hvAddress.IsPrimary.Value)
            {
                address.Extension.Add(new Extension(HealthVaultExtensions.IsPrimary, new FhirBoolean(true)));
            }

            return address;
        }

        private static ContactPoint ConvertEmail(Email email)
        {
            var contactPoint = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Value = email.Address,
                Rank = email.IsPrimary.HasValue ? 1 : (int?)null,
            };

            if (!string.IsNullOrEmpty(email.Description))
            {
                contactPoint.Extension.Add(new Extension(HealthVaultExtensions.Description, new FhirString(email.Description)));
            }

            return contactPoint;
        }

        private static ContactPoint ConvertPhone(Phone phone)
        {
            var contactPoint = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = phone.Number,
                Rank = phone.IsPrimary.HasValue ? 1 : (int?)null,
            };

            if (!string.IsNullOrEmpty(phone.Description))
            {
                contactPoint.Extension.Add(new Extension(HealthVaultExtensions.Description, new FhirString(phone.Description)));
            }

            return contactPoint;
        }
    }
}
