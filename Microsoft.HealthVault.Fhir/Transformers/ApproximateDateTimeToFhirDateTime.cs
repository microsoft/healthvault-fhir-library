// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        // Register the type on the generic ItemBaseToFhir partial class
        public static FhirDateTime ToFhir(this ApproximateDateTime approximateDateTime)
        {
            return ApproximateDateTimeToFhir.ToFhirInternal(approximateDateTime);
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault approximate date times into FHIR date times
    /// </summary>
    public class ApproximateDateTimeToFhir
    {
        internal static FhirDateTime ToFhirInternal(ApproximateDateTime approximateDateTime)
        {
            return new FhirDateTime(
                approximateDateTime.ApproximateDate.Year,
                approximateDateTime.ApproximateDate.Month ?? 1,
                approximateDateTime.ApproximateDate.Day ?? 1,
                approximateDateTime.ApproximateTime.Hour,
                approximateDateTime.ApproximateTime.Minute,
                approximateDateTime.ApproximateTime.Second ?? 0);
        }
    }
}
