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
using Hl7.Fhir.Validation;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(HVMedication))]
    public class MedicationToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultMedicationTransformedToFhir_ThenNameIsCopiedToCode()
        {
            const string medicationName = "Ibuprofen";
            var hvMedication = new HVMedication()
            {
                Name = new CodableValue(medicationName,
                    new CodedValue("df48b0ac-de8b-4bca-8785-dad6897fb53d",
                    family: "wc",
                    vocabularyName: "MayoMedications",
                    version: "1"))
            };

            var fhirMedication = ExtractEmbeddedMedication(hvMedication);
            var medicationCode = fhirMedication.Code;

            Assert.AreEqual(medicationName, medicationCode.Text);
        }

        [TestMethod]
        public void WhenHealthVaultMedicationTransformedToFhir_ThenGenericNameIsCopiedToIngredient()
        {
            const string medicationName = "Ibuprofen / Tolperisone Oral Tablet";
            var hvMedication = new HVMedication()
            {
                Name = new CodableValue("Ibuprofen"),
                GenericName = new CodableValue(medicationName,
                    new CodedValue("1579080",
                    family: "RxNorm",
                    vocabularyName: "RxNorm Obsolete Medicines",
                    version: "09AB_091102F"))
            };

            var fhirMedication = ExtractEmbeddedMedication(hvMedication);

            Assert.IsTrue(fhirMedication.Ingredient.Any());

            var ingredient = fhirMedication.Ingredient.First().Item as CodeableConcept;

            Assert.AreEqual(medicationName, ingredient?.Text);
        }

        [TestMethod]
        public void WhenHealthVaultMedicationTransformedToFhir_ThenStrengthIsCopiedToIngredientAmount()
        {
            var hvMedication = new HVMedication()
            {
                Name = new CodableValue("Ibuprofen"),
                Strength = new GeneralMeasurement("600mg")
            };

            hvMedication.Strength.Structured.Add(
                  new StructuredMeasurement(600,
                      new CodableValue("Milligrams (mg)",
                          new CodedValue("mg",
                              family: "wc",
                              vocabularyName: "medication-strength-unit",
                              version: "1"))));

            var fhirMedication = ExtractEmbeddedMedication(hvMedication);

            Assert.IsTrue(fhirMedication.Ingredient.Any());

            var ingredient = fhirMedication.Ingredient.First();

            Assert.AreEqual(600, ingredient?.Amount?.Numerator?.Value);
        }

        private static FhirMedication ExtractEmbeddedMedication(HVMedication hvMedication)
        {
            var medicationStatement = hvMedication.ToFhir();
            var medicationReference = medicationStatement.Medication as ResourceReference;
            if (medicationReference != null)
            {
                if (medicationReference.IsContainedReference)
                {
                    return medicationStatement.Contained.First(domainResource
                             => medicationReference.Matches(domainResource.GetContainerReference())) as FhirMedication;
                }
                throw new AssertInconclusiveException();
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


        [TestMethod]
        public void WhenMedicationTransformedToFhir_ThenMedicationStatementHasStatus()
        {
            ThingBase medication = getSampleMedication();

            var fhirMedication = medication.ToFhir() as MedicationStatement;

            Assert.IsNotNull(fhirMedication.Status);
        }

        [TestMethod]
        public void WhenMedicationTransformedToFhir_ThenMedicationStatementHasMedication()
        {
            ThingBase medication = getSampleMedication();

            var fhirMedication = medication.ToFhir() as MedicationStatement;

            Assert.IsNotNull(fhirMedication.Medication);
        }

        [TestMethod]
        public void WhenMedicationTransformedToFhir_ThenMedicationStatementHasTaken()
        {
            ThingBase medication = getSampleMedication();

            var fhirMedication = medication.ToFhir() as MedicationStatement;

            Assert.IsNotNull(fhirMedication.Taken);
        }

        private static HVMedication getSampleMedication()
        {
            return new HVMedication()
            {
                Name = new CodableValue("Ibuprofen",
                    new CodedValue()
                    {
                        Value = "df48b0ac-de8b-4bca-8785-dad6897fb53d",
                        Family = "Mayo",
                        VocabularyName = "MayoMedications",
                        Version = "1.0"
                    })
            };
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenMedicationRequestIsEmbedded()
        {
            var hvMedication = getSampleMedication();
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                }
            };

            var fhirMedication = hvMedication.ToFhir() as MedicationStatement;

            Func<Resource, bool> medicationRequestPredicate = resource
                => resource.ResourceType == ResourceType.MedicationRequest;
            Assert.IsTrue(fhirMedication.Contained.Any(medicationRequestPredicate)
                , $"Contained {nameof(MedicationRequest)} not found");
            Assert.IsTrue(fhirMedication.BasedOn.Any(reference
                => reference.IsContainedReference
                && reference.Matches(fhirMedication.Contained
                .First(medicationRequestPredicate).GetContainerReference())));
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenEmbeddedMedicationRequestHasReferenceToPractitioner()
        {
            var hvMedication = getSampleMedication();
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                }
            };

            var fhirMedication = hvMedication.ToFhir() as MedicationStatement;

            Func<Resource, bool> practitionerPredicate = resource
                => resource.ResourceType == ResourceType.Practitioner;
            Assert.IsTrue(fhirMedication.Contained.Any(practitionerPredicate)
                , $"Contained {nameof(Practitioner)} not found");

            var medicationRequest = ExtractEmbeddedMedicationRequest(fhirMedication);
            var practitioner = medicationRequest.Requester?.Agent;

            Assert.IsNotNull(practitioner);
            Assert.IsTrue(practitioner.IsContainedReference
                && practitioner.Matches(fhirMedication.Contained
                .First(practitionerPredicate).GetContainerReference()));
        }

        private MedicationRequest ExtractEmbeddedMedicationRequest(MedicationStatement medicationStatement)
        {
            foreach (var reference in medicationStatement.BasedOn)
            {
                if (reference.IsContainedReference)
                {
                    return medicationStatement.Contained.First(resource
                        => reference.Matches(resource.GetContainerReference())
                        && resource.ResourceType == ResourceType.MedicationRequest) as MedicationRequest;
                }
            }
            throw new AssertInconclusiveException();
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenEmbeddedMedicationRequestHasReferenceToMedication()
        {
            var hvMedication = getSampleMedication();
            var prescribedOn = new LocalDateTime(2017, 9, 11, 8, 8);
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                }
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.IsTrue(medicationRequest.Medication is ResourceReference);
            Assert.IsTrue((medicationRequest.Medication as ResourceReference).IsContainedReference);
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenEmbeddedMedicationRequestHasIntentAsInstanceOrder()
        {
            var hvMedication = getSampleMedication();
            var prescribedOn = new LocalDateTime(2017, 9, 11, 8, 8);
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                }
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.AreEqual(MedicationRequest.MedicationRequestIntent.InstanceOrder,
                medicationRequest.Intent);
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenDatePrescribedIsCopiedToAuthoredOn()
        {
            var hvMedication = getSampleMedication();
            var prescribedOn = new LocalDateTime(2017, 9, 11, 8, 8);
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                DatePrescribed = new ApproximateDateTime(prescribedOn)
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.AreEqual(prescribedOn.ToDateTimeUnspecified(),
                medicationRequest.AuthoredOnElement?.ToDateTimeOffset());
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenAmountPrescribedIsCopiedToDispenseRequestQuantity()
        {
            var hvMedication = getSampleMedication();
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                AmountPrescribed = new GeneralMeasurement("15 tablets")
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.IsNull(medicationRequest.DispenseRequest?.Quantity);

            const int numberOfTablets = 15;
            hvMedication.Prescription.AmountPrescribed.Structured.Add(new StructuredMeasurement
            {
                Value = numberOfTablets,
                Units = new CodableValue("tablets")
            });

            medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.AreEqual(numberOfTablets, medicationRequest.DispenseRequest?.Quantity?.Value);
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenSubstitutionIsCopiedToSubstitution()
        {
            var hvMedication = getSampleMedication();
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                Substitution = HealthVaultMedicationSubstitutionCodes.SubstitutionPermitted
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.IsTrue(medicationRequest.Substitution?.Allowed == true);
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenRefillsIsCopiedToNumberOfRepeatsAllowed()
        {
            var hvMedication = getSampleMedication();
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                Refills = 0
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.IsNull(medicationRequest.DispenseRequest?.NumberOfRepeatsAllowed);

            const int refillsAllowed = 3;
            hvMedication.Prescription.Refills = refillsAllowed;

            medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.AreEqual(refillsAllowed, medicationRequest.DispenseRequest?.NumberOfRepeatsAllowed);
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenDaysSupplyIsCopiedToExpectedSupplyDuration()
        {
            var hvMedication = getSampleMedication();
            const int daysSupply = 3;
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                DaysSupply = daysSupply
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.AreEqual(daysSupply, medicationRequest.DispenseRequest?.ExpectedSupplyDuration?.Value);
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenPrescriptionExpirationIsCopiedToValidityPeriodEnd()
        {
            var hvMedication = getSampleMedication();
            var validityEnd = new LocalDate(2017, 12, 12);
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                PrescriptionExpiration = new HealthServiceDate(2017, 12, 12)
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.AreEqual(validityEnd.ToDateTimeUnspecified(),
                medicationRequest.DispenseRequest?.ValidityPeriod?.EndElement.ToDateTimeOffset());
        }

        [TestMethod]
        public void WhenMedicationWithPrescriptionTransformedToFhir_ThenInstructionsIsCopiedToDosageInstruction()
        {
            var hvMedication = getSampleMedication();
            var validityEnd = new LocalDate(2017, 12, 12);
            hvMedication.Prescription = new Prescription
            {
                PrescribedBy = new PersonItem
                {
                    Name = new Name
                    {
                        Full = "John Mc Kense"
                    }
                },
                Instructions = new CodableValue("3 tablets/day, have it after dinner.")
            };

            var medicationRequest = ExtractEmbeddedMedicationRequest(hvMedication);

            Assert.IsTrue(medicationRequest.DosageInstruction.Any());
            Assert.IsTrue(medicationRequest.DosageInstruction.First().AdditionalInstruction.Any());
        }

        private MedicationRequest ExtractEmbeddedMedicationRequest(HVMedication hvMedication)
        {
            return ExtractEmbeddedMedicationRequest(hvMedication.ToFhir());            
        }
    }
}
