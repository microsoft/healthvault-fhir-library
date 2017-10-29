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
using Microsoft.HealthVault.Fhir.Constants;
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

            var medicationExtension = fhirMedication.GetExtension(HealthVaultExtensions.Medication);

            if (medicationExtension != null)
            {
                var genericName = medicationExtension
                    .GetExtensionValue<CodeableConcept>(HealthVaultExtensions.MedicationGenericName);
                hvMedication.GenericName = genericName?.ToCodableValue();

                var strengthExtension = medicationExtension.GetExtension(HealthVaultExtensions.MedicationStrength);
                if (strengthExtension != null)
                {
                    string display = strengthExtension.GetStringExtension(HealthVaultExtensions.MedicationStrengthDisplay);
                    var strength = new GeneralMeasurement(display);
                    foreach (var quantityExtension
                        in strengthExtension.GetExtensions(HealthVaultExtensions.MedicationStrengthQuantity))
                    {
                        var quantity = quantityExtension.Value as Quantity;
                        if (quantity == null)
                        {
                            continue;
                        }
                        strength.Structured.Add(new StructuredMeasurement
                        {
                            Value = (double)quantity.Value,
                            Units = CodeToHealthVaultHelper.CreateCodableValueFromQuantityValues(
                                quantity.System,quantity.Code,quantity.Unit)
                        });
                    }
                    hvMedication.Strength = strength;
                }
            }

            return hvMedication;
        }
    }
}
