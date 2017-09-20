// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        public static ContactPoint ToFhir(this Phone phone)
        {
            return PhoneToFhir.ToFhirInternal(phone);
        }
    }
    internal class PhoneToFhir
    {
        internal static ContactPoint ToFhirInternal(Phone phone)
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
    }
}
