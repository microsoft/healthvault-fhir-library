// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        public static Hl7.Fhir.Model.Address ToFhir(this ItemTypes.Address hvAddress)
        {
            return AddressToFhir.ToFhirInternal(hvAddress);
        }
    }
    internal class AddressToFhir
    {
        internal static Hl7.Fhir.Model.Address ToFhirInternal(ItemTypes.Address hvAddress)
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
                address.AddExtension(HealthVaultExtensions.IsPrimary, new FhirBoolean(true));
            }

            return address;
        }
    }
}
