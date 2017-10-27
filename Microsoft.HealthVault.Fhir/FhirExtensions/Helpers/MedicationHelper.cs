// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;

namespace Microsoft.HealthVault.Fhir.FhirExtensions.Helpers
{
    internal static class MedicationHelper
    {
        internal static Medication ToMedication(this CodeableConcept medicationCodeableConcept)
        {
            return new Medication
            {
                Code = medicationCodeableConcept
            };
        }

        internal static Medication GetMedication(DomainResource domainResource, Element medication)
        {
            switch (medication)
            {
                case ResourceReference medicationReference:
                    return domainResource.GetContainedResource<Medication>(medicationReference);
                case CodeableConcept medicationCodeableConcept:
                    return ToMedication(medicationCodeableConcept);
                default:
                    throw new ArgumentOutOfRangeException(nameof(medication)
                            , $"Allowed {nameof(ResourceReference)} or {nameof(CodeableConcept)} only");
            }
        }
    }
}
