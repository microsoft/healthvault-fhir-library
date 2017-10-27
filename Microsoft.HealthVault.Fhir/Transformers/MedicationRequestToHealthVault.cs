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
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class MedicationRequestToHealthVault
    {
        public static ThingBase ToHealthVault(this MedicationRequest medicationRequest)
        {
            var fhirMedication = MedicationRequestHelper.ExtractEmbeddedMedication(medicationRequest);

            if (fhirMedication == null)
            {
                return null;
            }

            var hvMedication = fhirMedication.ToHealthVault() as HVMedication;

            var practitioner = ExtractEmbeddedPractitioner(medicationRequest);
            if (practitioner == null)
            {
                throw new NotSupportedException($"{nameof(MedicationRequest)} needs to have an embedded Requester Agent");
            }

            var prescription = new Prescription(practitioner.ToHealthVault());
            if (medicationRequest.AuthoredOnElement != null)
            {
                var dt = medicationRequest.AuthoredOnElement.ToDateTimeOffset();
                prescription.DatePrescribed = new ApproximateDateTime(
                        new ApproximateDate(dt.Year, dt.Month, dt.Day),
                        new ApproximateTime(dt.Hour, dt.Minute, dt.Second, dt.Millisecond));
            }

            MedicationRequest.DispenseRequestComponent dispenseRequest = medicationRequest.DispenseRequest;
            if (dispenseRequest != null)
            {
                var numerator = dispenseRequest.Quantity;

                if (numerator != null)
                {
                    var structuredMeasurement = new StructuredMeasurement
                    {
                        Value = (double)numerator.Value,
                        Units = CodeToHealthVaultHelper.CreateCodableValueFromQuantityValues(numerator.System,
                               numerator.Code, numerator.Unit)
                    };
                    prescription.AmountPrescribed = new GeneralMeasurement();
                    prescription.AmountPrescribed.Structured.Add(structuredMeasurement);
                }

                prescription.Refills = dispenseRequest.NumberOfRepeatsAllowed;

                prescription.DaysSupply = GetDaysFromDuration(dispenseRequest.ExpectedSupplyDuration);

                FhirDateTime end = dispenseRequest.ValidityPeriod?.EndElement;
                if (end != null)
                {
                    var endDate = end.ToDateTimeOffset();
                    prescription.PrescriptionExpiration = new HealthServiceDate(endDate.Year
                        , endDate.Month, endDate.Day);
                }
            }

            if (medicationRequest.DosageInstruction.Any())
            {
                var dosageInstruction = medicationRequest.DosageInstruction
                    .FirstOrDefault(dosage => dosage.AdditionalInstruction.Any());
                if (dosageInstruction != null)
                {
                    var instruction = dosageInstruction.AdditionalInstruction.First();
                    prescription.Instructions = instruction.ToCodableValue();
                }
            }

            if (medicationRequest.Substitution != null)
            {
                prescription.Substitution = GetSubstitutionCode(medicationRequest, prescription);
            }
            hvMedication.Prescription = prescription;

            return hvMedication;
        }

        private static int? GetDaysFromDuration(Hl7.Fhir.Model.Duration duration)
        {
            if (duration == null)
            {
                return null;
            }
            
            if (duration.Value.HasValue)
            {
                switch (duration.Code)
                {
                    case "day":
                        return decimal.ToInt32(duration.Value.Value);
                    case "week":
                    case "month":
                    case "year":
                        double value = decimal.ToDouble(duration.Value.Value);
                        var fromUnit = UnitsNet.Duration.ParseUnit(duration.Code);
                        var unitsNetDuration = UnitsNet.Duration.From(value, fromUnit);
                        return Convert.ToInt32(unitsNetDuration.Days);
                }
            }
            return null;
        }

        private static CodableValue GetSubstitutionCode(MedicationRequest medicationRequest, Prescription prescription)
        {
            if (medicationRequest.Substitution.Allowed.HasValue)
            {
                switch (medicationRequest.Substitution.Allowed.Value)
                {
                    case true:
                        return HealthVaultMedicationSubstitutionCodes.SubstitutionPermitted;
                    case false:
                        return HealthVaultMedicationSubstitutionCodes.DispenseAsWritten;
                }
            }
            return null;
        }

        private static Practitioner ExtractEmbeddedPractitioner(MedicationRequest medicationRequest)
        {
            var agentReference = medicationRequest.Requester?.Agent;

            if (agentReference == null)
            {
                return null;
            }

            return medicationRequest.GetReferencedResource(agentReference,
                (reference) => new Practitioner
                {
                    Name = new System.Collections.Generic.List<HumanName>
                    {
                        new HumanName
                        {
                            Text = agentReference.Display
                        }
                    }
                });
        }
    }
}
