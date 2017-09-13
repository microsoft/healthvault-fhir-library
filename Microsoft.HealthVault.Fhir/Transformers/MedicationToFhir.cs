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
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
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

            if (hvMedication.Prescription != null)
            {
                var embeddedMedicationRequest = new MedicationRequest();
                embeddedMedicationRequest.Id = "medReq" + Guid.NewGuid();

                MedicationRequest request = ToFhirInternal(hvMedication.Prescription, embeddedMedicationRequest);
                request.Medication = embeddedMedication.GetContainerReference();
                medicationStatement.Contained.Add(request);
                medicationStatement.BasedOn.Add(embeddedMedicationRequest.GetContainerReference());
            }

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
            fhirMedication.Code = hvMedication.Name.ToFhir();

            if (hvMedication.GenericName != null)
            {
                var ingredientItem = hvMedication.GenericName.ToFhir();
                fhirMedication.Ingredient = new List<FhirMedication.IngredientComponent> {
                    new FhirMedication.IngredientComponent(){
                        Item= ingredientItem
                }};
            }

            if (hvMedication.Strength != null)
            {
                if (!fhirMedication.Ingredient.Any())
                {
                    var ingredientItem = hvMedication.Name.ToFhir();
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

        internal static MedicationRequest ToFhirInternal(Prescription prescription, MedicationRequest medicationRequest)
        {
            medicationRequest.SetIntentAsInstanceOrder();
            //hvMedication.PrescribedBy
            medicationRequest.AuthoredOnElement = prescription.DatePrescribed?.ToFhir();
            if (prescription.AmountPrescribed != null
                || prescription.Refills.HasValue
                || prescription.DaysSupply.HasValue
                || prescription.PrescriptionExpiration != null)
            {
                medicationRequest.DispenseRequest = new MedicationRequest.DispenseRequestComponent
                {
                    Quantity = HealthVaultCodesToFhir.GetSimpleQuantity(prescription.AmountPrescribed),
                    NumberOfRepeatsAllowed = prescription.Refills == 0 ? null : prescription.Refills,
                    ExpectedSupplyDuration = new Duration()
                    {
                        Value = prescription.DaysSupply,
                        Code = nameof(UnitsNet.Units.DurationUnit.Day)
                    },
                    ValidityPeriod = new Period
                    {
                        EndElement = prescription.PrescriptionExpiration?.ToFhir()
                    }
                };
            }
            if (prescription.Substitution != null)
            {
                medicationRequest.Substitution = new MedicationRequest.SubstitutionComponent
                {
                    Allowed = IsAllowed(prescription.Substitution)
                };
            }
            if (prescription.Instructions != null)
            {
                var dosage = new Dosage();
                dosage.AdditionalInstruction.Add(prescription.Instructions.ToFhir());
                medicationRequest.DosageInstruction.Add(dosage);
            }

            return medicationRequest;
        }

        private static void SetIntentAsInstanceOrder(this MedicationRequest medicationRequest)
        {
            medicationRequest.Intent = MedicationRequest.MedicationRequestIntent.InstanceOrder;
        }

        private static bool? IsAllowed(CodableValue substitution)
        {
            Func<CodedValue, bool> medicationSubstitutionPredicate =
                coded => coded.VocabularyName == HealthVaultVocabularies.MedicationSubstitution;
            if (substitution.Any(medicationSubstitutionPredicate))
            {
                var coded = substitution.First(medicationSubstitutionPredicate);
                switch (coded.Value)
                {
                    case HealthVaultMedicationSubstitutionCodes.DispenseAsWrittenCode:
                        return false;
                    case HealthVaultMedicationSubstitutionCodes.SubstitutionPermittedCode:
                        return true;
                }
            }
            throw new NotImplementedException();
        }
    }
}
