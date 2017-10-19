// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class ContactPointToHealthVault
    {
        public static T ToHealthVault<T>(this ContactPoint contactPoint) where T : new()
        {
            var result = new T();
            switch (result)
            {
                case Email email:
                    email.Address = contactPoint.Value;
                    email.IsPrimary = contactPoint.Rank.HasValue ? contactPoint.Rank == 1 : (bool?)null;
                    email.Description = contactPoint.GetStringExtension(HealthVaultExtensions.Description);
                    break;
                case Phone phone:
                    phone.Number = contactPoint.Value;
                    phone.IsPrimary = contactPoint.Rank.HasValue ? contactPoint.Rank == 1 : (bool?)null;
                    phone.Description = contactPoint.GetStringExtension(HealthVaultExtensions.Description);
                    break;
                default:
                    throw new NotSupportedException($"Transformation from {contactPoint.GetType()} to {typeof(T)} is not supported");
            }

            return result;
        }
    }
}
