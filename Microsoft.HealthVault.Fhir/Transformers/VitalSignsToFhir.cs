// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static List<Observation> ToFhir(this VitalSigns vitalSigns)
        {
            return VitalSignsToFhir.ToFhirInternal(vitalSigns);
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault vitalSigns data types into FHIR Observations
    /// </summary>
    internal static class VitalSignsToFhir
    {
        internal static List<Observation> ToFhirInternal(VitalSigns vitalSigns)
        {
            var baseObservation = ThingBaseToFhir.ToFhirInternal(vitalSigns);

            var observations = new List<Observation>();

            foreach (var vitalSignResult in vitalSigns.VitalSignsResults)
            {
                switch (vitalSignResult.Title.Text)
                {
                    case "Temperature":
                        var temperatureObservation = new Observation();
                        baseObservation.CopyTo(temperatureObservation);

                        temperatureObservation.Category = new List<CodeableConcept>
                        {
                            FhirCategories.VitalSigns
                        };
                        temperatureObservation.Code = HealthVaultVocabularies.BodyTemperature;

                        if (vitalSignResult.Value.HasValue)
                        { 
                            var quantity = new Quantity((decimal)vitalSignResult.Value, "C");
                            temperatureObservation.Value = quantity;
                        }

                        temperatureObservation.Effective = new FhirDateTime(vitalSigns.When.ToDateTime());

                        observations.Add(temperatureObservation);
                        break;
                }
            }

            return observations;
        }
    }
}
