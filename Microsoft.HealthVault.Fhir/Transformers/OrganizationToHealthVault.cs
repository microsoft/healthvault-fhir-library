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
using Microsoft.HealthVault.ItemTypes;
using FhirOrganization = Hl7.Fhir.Model.Organization;
using HVOrganization = Microsoft.HealthVault.ItemTypes.Organization;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class OrganizationToHealthVault
    {
        public static HVOrganization ToHealthVault(this FhirOrganization fhirOrganization)
        {
            var hvOrganization = new HVOrganization();

            if (fhirOrganization.Name == null)
            {
                throw new ArgumentException("Organization in HealthVault requires a non empty name.");
            }

            hvOrganization.Name = fhirOrganization.Name;

            if (fhirOrganization.Type.Any())
            {
                var organizationType = fhirOrganization.Type.First();
                hvOrganization.Type = organizationType.ToCodableValue();
            }

            foreach (var item in fhirOrganization.Telecom
                .Select(contactPoint => contactPoint.ToHealthVault())
                .Where(itemBase => itemBase != null))
            {
                hvOrganization.CreateContactIfNeeded();
                switch (item)
                {
                    case Email email:
                        hvOrganization.Contact.Email.Add(email);
                        break;
                    case Phone phone:
                        hvOrganization.Contact.Phone.Add(phone);
                        break;
                }
            }

            var url = fhirOrganization.Telecom.FirstOrDefault(contactPoint
                => contactPoint.System == ContactPoint.ContactPointSystem.Url)?.Value;
            if (url != null)
            {
                hvOrganization.Website = new Uri(url);
            }

            if (fhirOrganization.Address.Any())
            {
                hvOrganization.CreateContactIfNeeded();
                foreach (var address in fhirOrganization.Address)
                {
                    hvOrganization.Contact.Address.Add(address.ToHealthVault());
                }
            }

            return hvOrganization;
        }

        private static void CreateContactIfNeeded(this HVOrganization organization)
        {
            if (organization.Contact == null)
            {
                organization.Contact = new ContactInfo();
            }
        }
    }
}

