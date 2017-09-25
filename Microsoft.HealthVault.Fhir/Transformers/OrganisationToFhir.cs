// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using FhirOrganization = Hl7.Fhir.Model.Organization;
using HVOrganization = Microsoft.HealthVault.ItemTypes.Organization;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        // Register the type on the generic ItemBaseToFhir partial class
        public static FhirOrganization ToFhir(this HVOrganization hvOrganization)
        {
            return OrganisationToFhir.ToFhirInternal(hvOrganization);
        }
    }
    internal static class OrganisationToFhir
    {
        internal static FhirOrganization ToFhirInternal(HVOrganization hvOrganization)
        {
            var fhirOrganization = new FhirOrganization();
            fhirOrganization.Name = hvOrganization.Name;

            var contact = hvOrganization.Contact;
            if (contact != null)
            {
                foreach (var address in contact.Address)
                {
                    fhirOrganization.Address.Add(address.ToFhir());
                }
                foreach (var email in contact.Email)
                {
                    fhirOrganization.Telecom.Add(email.ToFhir());
                }
                foreach (var phone in contact.Phone)
                {
                    fhirOrganization.Telecom.Add(phone.ToFhir());
                }
            }

            if (hvOrganization.Type != null)
            {
                fhirOrganization.Type.Add(hvOrganization.Type.ToFhir());
            }

            if (hvOrganization.Website != null)
            {
                fhirOrganization.Telecom.Add(new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Url,
                    Value = hvOrganization.Website.OriginalString
                });
            }

            return fhirOrganization;
        }
    }
}
