// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
                    patient.Address.Add(address.ToFhir());
                }
            }

            if (!contact.ContactInformation.Email.IsNullOrEmpty())
            {
                foreach (var email in contact.ContactInformation.Email)
                {
                    patient.Telecom.Add(email.ToFhir());
                }
            }

            if (!contact.ContactInformation.Phone.IsNullOrEmpty())
            {
                foreach (var phone in contact.ContactInformation.Phone)
                {
                    patient.Telecom.Add(phone.ToFhir());
                }
            }

            return patient;
        }        
    }
}
