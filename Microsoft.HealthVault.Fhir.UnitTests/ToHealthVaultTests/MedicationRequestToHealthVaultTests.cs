// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(MedicationRequest))]
    public class MedicationRequestToHealthVaultTests
    {
        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenPractitionerIsCopiedToPrescription()
        {
            MedicationRequest medicationRequest = GetSampleRequest();

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.IsNotNull(hvMedication.Prescription.PrescribedBy);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenAuthoredOnIsCopiedToDatePrescribed()
        {
            var prescribedOn = new LocalDate(2017, 08, 10);
            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.AuthoredOnElement = new FhirDateTime(prescribedOn.ToDateTimeUnspecified());

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.IsTrue(hvMedication.Prescription?.DatePrescribed.ApproximateDate.CompareTo(prescribedOn) == 0);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenDispenseRequestQuantityIsCopiedToAmountPrescribed()
        {
            const int amountPrescribed = 15;
            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.DispenseRequest = new MedicationRequest.DispenseRequestComponent
            {
                Quantity = new Quantity(amountPrescribed, "tablets").CopyTo(new SimpleQuantity()) as SimpleQuantity
            };

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.AreEqual(amountPrescribed, hvMedication.Prescription?.AmountPrescribed?.Structured.First()?.Value);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenSubstitutionIsCopiedToSubstitution()
        {
            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.Substitution = new MedicationRequest.SubstitutionComponent
            {
                Allowed = true
            };

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.AreEqual(HealthVaultMedicationSubstitutionCodes.SubstitutionPermittedCode
                , hvMedication.Prescription?.Substitution.First().Value);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenNumberOfRepeatsAllowedIsCopiedToRefills()
        {
            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.DispenseRequest = new MedicationRequest.DispenseRequestComponent();

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.AreEqual(0, hvMedication.Prescription?.Refills);

            const int refillsAllowed = 3;
            medicationRequest.DispenseRequest = new MedicationRequest.DispenseRequestComponent
            {
                NumberOfRepeatsAllowed = refillsAllowed
            };

            hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.AreEqual(refillsAllowed, hvMedication.Prescription.Refills);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenExpectedSupplyDurationIsCopiedToDaysSupply()
        {
            var daysSupply = 12;

            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.DispenseRequest = new MedicationRequest.DispenseRequestComponent
            {
                ExpectedSupplyDuration = new Quantity(daysSupply, "day")
                    .CopyTo(new Hl7.Fhir.Model.Duration()) as Hl7.Fhir.Model.Duration
            };

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.AreEqual(daysSupply, hvMedication.Prescription?.DaysSupply);

            medicationRequest.DispenseRequest.ExpectedSupplyDuration = new Quantity(1, "month")
                    .CopyTo(new Hl7.Fhir.Model.Duration()) as Hl7.Fhir.Model.Duration;

            hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.AreEqual(30, hvMedication.Prescription?.DaysSupply);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenValidityPeriodEndIsCopiedToPrescriptionExpiration()
        {
            var expiration = new LocalDate(2017, 12, 12);
            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.DispenseRequest = new MedicationRequest.DispenseRequestComponent
            {
                ValidityPeriod = new Hl7.Fhir.Model.Period
                {
                    EndElement = new FhirDateTime(expiration.ToDateTimeUnspecified())
                }
            };

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.IsTrue(hvMedication.Prescription?.PrescriptionExpiration.CompareTo(expiration) == 0);
        }

        [TestMethod]
        public void WhenMedicationRequestTransformedToHealthVault_ThenDosageInstructionIsCopiedToInstructions()
        {
            MedicationRequest medicationRequest = GetSampleRequest();
            medicationRequest.DosageInstruction = new System.Collections.Generic.List<Dosage>
            {
                new Dosage
                {
                    AdditionalInstruction = new System.Collections.Generic.List<CodeableConcept>
                    {
                        new CodeableConcept
                        {
                            Text = "3 tablets/day, have it after dinner."
                        }
                    }
                }
            };

            var hvMedication = medicationRequest.ToHealthVault() as HVMedication;

            Assert.IsNotNull(hvMedication.Prescription?.Instructions);
        }

        private static MedicationRequest GetSampleRequest()
        {
            return new MedicationRequest
            {
                Contained = new System.Collections.Generic.List<Resource>
                {
                    new Practitioner
                    {
                        Id = "agent",
                        Name = new System.Collections.Generic.List<HumanName>
                        {
                            new HumanName
                            {
                                Text = "John Sam"
                            }
                        }
                    }
                },
                Medication = new CodeableConcept
                {
                    Text = "Amoxicillin 250mg/5ml Suspension"
                },
                Requester = new MedicationRequest.RequesterComponent
                {
                    Agent = new ResourceReference("#agent")
                }
            };
        }
    }
}
