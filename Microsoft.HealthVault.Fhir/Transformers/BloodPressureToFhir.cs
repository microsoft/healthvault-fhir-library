// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Observation ToFhir(this BloodPressure bp)
        {
            return BloodPressureToFhir.ToFhirInternal(bp, ToFhirInternal<Observation>(bp));
        }
    }

    internal static class BloodPressureToFhir
    {
        internal static Observation ToFhirInternal(BloodPressure bp, Observation observation)
        {
            if (bp.IrregularHeartbeatDetected.HasValue)
            {
                observation.AddExtension(HealthVaultVocabularies.IrregularHeartBeatExtensionName, new FhirBoolean(bp.IrregularHeartbeatDetected.Value));
            }

            var diastolicComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept() { Coding = new List<Coding> { HealthVaultVitalStatisticsCodes.BloodPressureDiastolic } },
                Value = new Quantity((decimal)bp.Diastolic, "mmHg")
            };

            var systolicComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept() { Coding = new List<Coding> { HealthVaultVitalStatisticsCodes.BloodPressureSystolic } },
                Value = new Quantity((decimal)bp.Systolic, "mmHg")
            };

            observation.Component = new List<Observation.ComponentComponent> { diastolicComponent, systolicComponent };

            if (bp.Pulse != null)
            {
                observation.Component.Add(new Observation.ComponentComponent
                {
                    Code = new CodeableConcept() { Coding = new List<Coding> { HealthVaultVitalStatisticsCodes.HeartRate } },
                    Value = new Quantity((decimal)bp.Pulse, "/min")
                });
            }

            observation.Effective = new FhirDateTime(bp.When.ToDateTime());
            observation.Code = HealthVaultVocabularies.BloodPressure;

            return observation;
        }
    }
}
