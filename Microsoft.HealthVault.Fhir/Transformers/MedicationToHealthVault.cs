// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class MedicationToHealthVault
    {
        public static ThingBase ToHealthVault(this FhirMedication fhirMedication)
        {
            var hvMedication = new HVMedication();

            var name = fhirMedication.Code.ToCodableValue();
            if (name == null)
            {
                throw new NotSupportedException($"{nameof(FhirMedication)} should" +
                    $" have {nameof(fhirMedication.Code)}");
            }
            hvMedication.Name = name;

            var hasSingleIngredient = fhirMedication.Ingredient.Count == 1;
            if (hasSingleIngredient)
            {
                var ingredientComponent = fhirMedication.Ingredient.Single();
                var ingredientItemIsCodeableConcept = ingredientComponent.Item is CodeableConcept;
                if (ingredientItemIsCodeableConcept)
                {
                    var ingredientItemAsCodeableConcept = ingredientComponent.Item as CodeableConcept;
                    var isIngredientItemNotEquivalentToCode = !ingredientItemAsCodeableConcept
                        .Matches(fhirMedication.Code);
                    if (isIngredientItemNotEquivalentToCode)
                    {
                        hvMedication.GenericName = ingredientItemAsCodeableConcept.ToCodableValue();
                    }

                    var ingredientAmount = ingredientComponent.Amount;
                    if (ingredientAmount != null)
                    {
                        var numerator = ingredientAmount.Numerator;

                        var structuredMeasurement = new StructuredMeasurement
                        {
                            Value = (double)numerator.Value,
                            Units = CodeToHealthVaultHelper.CreateCodableValueFromQuantityValues(numerator.System,
                        numerator.Code, numerator.Unit)
                        };
                        hvMedication.Strength = new GeneralMeasurement();
                        hvMedication.Strength.Structured.Add(structuredMeasurement);
                    }
                }
            }

            return hvMedication;
        }
    }
}
