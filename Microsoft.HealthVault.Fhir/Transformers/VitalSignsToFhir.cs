// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Observation ToFhir(this VitalSigns vitalSigns)
        {
            return VitalSignsToFhir.ToFhirInternal(vitalSigns, ToFhirInternal<Observation>(vitalSigns));
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault vitalSigns data types into FHIR Observations
    /// </summary>
    internal static class VitalSignsToFhir
    {
        internal static Observation ToFhirInternal(VitalSigns vitalSigns, Observation observation)
        {
            observation.Category = new List<CodeableConcept> { FhirCategories.VitalSigns };
            observation.Effective = new FhirDateTime(vitalSigns.When.ToLocalDateTime().ToDateTimeUnspecified());
            observation.Code = HealthVaultVocabularies.VitalSigns;

            foreach (VitalSignsResultType vitalSignsResult in vitalSigns.VitalSignsResults)
            {
                var vitalSign = VitalSignsResultToFhir(vitalSignsResult);
                vitalSign.Id = $"vitalsign-{Guid.NewGuid()}";
                observation.Contained.Add(vitalSign);

                var related = new Observation.RelatedComponent();
                related.Type = Observation.ObservationRelationshipType.HasMember;
                related.Target = new ResourceReference(vitalSign.Id);
            }

            return observation;
        }

        private static Observation VitalSignsResultToFhir(VitalSignsResultType vitalSignsResult)
        {
            var observation = new Observation();
            observation.Category = new List<CodeableConcept> { FhirCategories.VitalSigns };
            observation.Code = vitalSignsResult.Title.ToFhir();
            observation.Status = ObservationStatus.Final;

            var referenceRangeValue = new Observation.ReferenceRangeComponent();
            referenceRangeValue.Low = new Quantity((decimal)vitalSignsResult.ReferenceMinimum, vitalSignsResult.Unit.Text) as SimpleQuantity;
            referenceRangeValue.High = new Quantity((decimal)vitalSignsResult.ReferenceMaximum, vitalSignsResult.Unit.Text) as SimpleQuantity;
            observation.ReferenceRange.Add(referenceRangeValue);

            var quantity = new Quantity((decimal)vitalSignsResult.Value, vitalSignsResult.Unit.Text);

            var codableUnitExtension = new Extension
            {
                Url = HealthVaultExtensions.CodableUnit
            };

            codableUnitExtension.Value = vitalSignsResult.Unit.ToFhir();
            quantity.Extension.Add(codableUnitExtension);
            observation.Value = quantity;

            return observation;
        }
    }
}
