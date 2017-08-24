// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using UnitsNet;
using UnitsNet.Units;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingBaseToFhir partial class
        public static Observation ToFhir(this BodyComposition bodyComposition)
        {
            return BodyCompositionToFhir.ToFhirInternal(bodyComposition, ToFhirInternal<Observation>(bodyComposition));
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault body composition data types into FHIR Observations
    /// </summary>
    internal static class BodyCompositionToFhir
    {
        internal static Observation ToFhirInternal(BodyComposition bodyComposition, Observation observation)
        {
            observation.Category = new List<CodeableConcept> { FhirCategories.VitalSigns };
            
            var codings = new List<Coding>();
            HealthVaultCodesToFhir.ConvertCodableValueToFhir(bodyComposition.MeasurementMethod, codings);
            HealthVaultCodesToFhir.ConvertCodableValueToFhir(bodyComposition.Site, codings);
            HealthVaultCodesToFhir.ConvertCodableValueToFhir(bodyComposition.MeasurementName, codings);

            observation.Component = new List<Observation.ComponentComponent>();

            if (bodyComposition.Value.MassValue != null)
            {
                var massValue = new Observation.ComponentComponent
                {
                    Code = new CodeableConcept { Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(bodyComposition.MeasurementName, null) },
                    Value = new Quantity((decimal)bodyComposition.Value.MassValue.Kilograms, UnitAbbreviations.Kilogram)
                };
                observation.Component.Add(massValue);
            }

            if (bodyComposition.Value.PercentValue.HasValue)
            { 
                var percentageValue = new Observation.ComponentComponent
                {
                    Code = new CodeableConcept { Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(bodyComposition.MeasurementName, null) },
                    Value = new Quantity((decimal)bodyComposition.Value.PercentValue.Value, UnitAbbreviations.Percent)
                };
                observation.Component.Add(percentageValue);
            }
            
            observation.Effective = bodyComposition.When.ToFhir();

            observation.Code = new CodeableConcept
            {
                Coding = codings
            };

            return observation;
        }
    }
}
