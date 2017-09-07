// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.ItemTypes;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static MedicationStatement ToFhir(this HVMedication hvMedication)
        {
            return MedicationToFhir.ToFhirInternal(hvMedication, ToFhirInternal<MedicationStatement>(hvMedication));
        }
    }

    internal static class MedicationToFhir
    {
        internal static MedicationStatement ToFhirInternal(HVMedication hvMedication, MedicationStatement medicationStatement)
        {
            var embeddedMedication = new FhirMedication();
            embeddedMedication.Id = "med" + Guid.NewGuid();

            medicationStatement.Contained.Add(ToFhirInternal(hvMedication, embeddedMedication));
            medicationStatement.Medication = embeddedMedication.GetContainerReference();

            medicationStatement.SetStatusAsActive();
            medicationStatement.SetTakenAsNotApplicable();

            medicationStatement.Dosage = AddDosage(hvMedication.Dose, hvMedication.Frequency, hvMedication.Route);

            return medicationStatement;
        }

        private static List<Dosage> AddDosage(GeneralMeasurement dose, GeneralMeasurement frequency, CodableValue route)
        {
            return new List<Dosage> {
                HealthVaultCodesToFhir.GetDosage(dose, frequency, route)
                };
        }

        private static void SetStatusAsActive(this MedicationStatement medicationStatement)
        {
            medicationStatement.Status = MedicationStatement.MedicationStatementStatus.Active;
        }

        private static void SetTakenAsNotApplicable(this MedicationStatement medicationStatement)
        {
            medicationStatement.Taken = MedicationStatement.MedicationStatementTaken.Na;
        }

        internal static FhirMedication ToFhirInternal(HVMedication hvMedication, FhirMedication fhirMedication)
        {
            fhirMedication.Code = HealthVaultCodesToFhir.ConvertCodableValueToFhir(hvMedication.Name);

            if (hvMedication.GenericName != null)
            {
                var ingredientItem = HealthVaultCodesToFhir.ConvertCodableValueToFhir(hvMedication.GenericName);
                fhirMedication.Ingredient = new List<FhirMedication.IngredientComponent> {
                    new FhirMedication.IngredientComponent(){
                        Item= ingredientItem
                }};
            }

            if (hvMedication.Strength != null)
            {
                if (!fhirMedication.Ingredient.Any())
                {
                    var ingredientItem = HealthVaultCodesToFhir.ConvertCodableValueToFhir(hvMedication.Name);
                    fhirMedication.Ingredient = new List<FhirMedication.IngredientComponent> {
                        new FhirMedication.IngredientComponent{
                            Item = ingredientItem
                    }};
                }

                if (hvMedication.Strength.Structured.Any())
                {
                    var amount = HealthVaultCodesToFhir.GetSimpleQuantity(hvMedication.Strength);

                    fhirMedication.Ingredient.First().Amount = new Ratio() { Numerator = amount };
                }
            }

            return fhirMedication;
        }
    }
}
