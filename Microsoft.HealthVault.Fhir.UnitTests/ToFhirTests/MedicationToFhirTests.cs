// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void WhenMedicationTransformedToFHIR_ThenMedicationStatementHasStatus()
        {
            ThingBase medication = getSampleMedication();

            var fhirMedication = medication.ToFhir() as MedicationStatement;

            Assert.IsNotNull(fhirMedication.Status);
        }

        [TestMethod]
        public void WhenMedicationTransformedToFHIR_ThenMedicationStatementHasMedication()
        {
            ThingBase medication = getSampleMedication();

            var fhirMedication = medication.ToFhir() as MedicationStatement;

            Assert.IsNotNull(fhirMedication.Medication);
        }

        [TestMethod]
        public void WhenMedicationTransformedToFHIR_ThenMedicationStatementHasTaken()
        {
            ThingBase medication = getSampleMedication();

            var fhirMedication = medication.ToFhir() as MedicationStatement;

            Assert.IsNotNull(fhirMedication.Taken);
        }

        private static ThingBase getSampleMedication()
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
    }
}
