// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        public static HumanName ToFhir(this Name name)
        {
            return NameToFhir.ToFhirInternal(name);
        }
    }
    internal static class NameToFhir
    {
        internal static HumanName ToFhirInternal(Name hvName)
        {
            if (hvName == null)
            {
                return null;
            }

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

        private static void AddGivenName(HumanName fhirName, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                fhirName.GivenElement.Add(new FhirString(name));
            }
        }
    }
}
