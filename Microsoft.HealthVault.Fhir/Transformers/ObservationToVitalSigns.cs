// Copyright (c) Microsoft Corporation.  All rights reserved.
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

// Converts FHIR Observation to HealthVault Vital Sign

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToVitalSigns
    {
        internal static VitalSigns ToVitalSigns(this Observation observation)
        {
            var vitalSigns = observation.ToThingBase<VitalSigns>();
            vitalSigns.When = ObservationToHealthVault.GetHealthVaultTimeFromEffectiveDate(observation.Effective);

            foreach (Observation vitalSignObservation in observation.Contained)
            {
                var vitalSignsResult = new VitalSignsResultType();
                vitalSignsResult.Title = vitalSignObservation.Code.ToCodableValue();

                if (vitalSignObservation.ReferenceRange.Count != 0)
                {
                    var referenceRangeValue = vitalSignObservation.ReferenceRange[0];

                    if (referenceRangeValue == null)
                    {
                        throw new Exception("Reference Range object must have a range");
                    }

                    if (referenceRangeValue.Low != null)
                    {
                        vitalSignsResult.ReferenceMinimum = (double)referenceRangeValue.Low.Value;
                    }
                    if (referenceRangeValue.High != null)
                    {
                        vitalSignsResult.ReferenceMaximum = (double)referenceRangeValue.High.Value;
                    }
                }

                var vitalSignValue = vitalSignObservation.Value as Quantity;
                if (vitalSignValue?.Value == null)
                {
                    throw new ArgumentException("Value quantity must have a value.");
                }

                vitalSignsResult.Value = (int)vitalSignValue.Value.Value;
                var vitalSignsUnitExtension = vitalSignValue.GetExtension(HealthVaultExtensions.CodableUnit);
                vitalSignsResult.Unit = vitalSignsUnitExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.CodableUnit).ToCodableValue();
            }

            return vitalSigns;
        }
    }
}
