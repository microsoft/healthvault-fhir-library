// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class MedicationStatementToHealthVault
    {
        public static ThingBase ToHealthVault(this MedicationStatement medicationStatement)
        {
            var fhirMedication = ExtractEmbeddedMedication(medicationStatement);

            var hvMedication = fhirMedication.ToHealthVault() as HVMedication;

            var dosage = medicationStatement.Dosage.First();

            var doseQuantity = dosage.Dose as SimpleQuantity;
            var dose = new GeneralMeasurement { };
            dose.Structured.Add(new StructuredMeasurement
            {
                Value = (double)doseQuantity.Value,
                Units = CodeToHealthVaultHelper.CreateCodableValueFromQuantityValues(doseQuantity.System, doseQuantity.Code, doseQuantity.Unit)
            });
            hvMedication.Dose = dose;

            Timing.RepeatComponent repeat = dosage.Timing.Repeat;
            var frequency = new GeneralMeasurement { };
            frequency.Structured.Add(new StructuredMeasurement
            {
                Value = (double)repeat.Period,
                Units= FhirCodesToHealthVault.GetRecurrenceIntervalFromPeriodUnit(repeat.PeriodUnit)
            });
            hvMedication.Frequency = frequency;

            var route = dosage.Route.GetCodableValue();
            hvMedication.Route = route;

            return hvMedication;
        }

        private static FhirMedication ExtractEmbeddedMedication(MedicationStatement medicationStatement)
        {
            var medicationReference = medicationStatement.Medication as ResourceReference;
            if (medicationReference != null)
            {
                var fhirMedication = medicationStatement.Contained.FirstOrDefault(resource
                    => medicationReference.Reference == resource.Id) as FhirMedication;
                return fhirMedication ?? new FhirMedication();
            }
            else
            {
                var medicationCodeableConcept = medicationStatement.Medication as CodeableConcept;
                return new FhirMedication
                {
                    Code = medicationCodeableConcept
                };
            }
        }
    }
}