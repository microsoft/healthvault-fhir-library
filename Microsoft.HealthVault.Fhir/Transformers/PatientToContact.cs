// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class PatientToContact
    {
        internal static Contact ToContact(this Patient patient)
        {
            var contact = patient.ToThingBase<ItemTypes.Contact>();

            foreach (var address in patient.Address)
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
                hvAddress.County = address.Country;

                hvAddress.Description = address.Text;
                hvAddress.IsPrimary = address.Extension.Any(x => x.Url == "is-primary");

                contact.ContactInformation.Address.Add(hvAddress);
            }

            foreach (var contactPoint in patient.Telecom)
            {
                switch (contactPoint.System)
                {
                    case ContactPoint.ContactPointSystem.Email:
                        contact.ContactInformation.Email.Add(ConvertContactPointToEmail(contactPoint));
                        break;
                    case ContactPoint.ContactPointSystem.Phone:
                        contact.ContactInformation.Phone.Add(ConvertContactPointToPhone(contactPoint));
                        break;
                }
            }

            return contact;
        }

        private static Email ConvertContactPointToEmail(ContactPoint contactPoint)
        {
            return new Email
            {
                Address = contactPoint.Value,
                IsPrimary = contactPoint.Rank.HasValue ? contactPoint.Rank == 1 : (bool?)null,
                Description = ((FhirString)contactPoint.Extension.First(x => x.Url == "Description").Value).Value
            };
        }

        private static Phone ConvertContactPointToPhone(ContactPoint contactPoint)
        {
            return new Phone
            {
                Number = contactPoint.Value,
                IsPrimary = contactPoint.Rank.HasValue ? contactPoint.Rank == 1 : (bool?)null,
                Description = ((FhirString)contactPoint.Extension.First(x => x.Url == "Description").Value).Value
            };
        }
    }
}
