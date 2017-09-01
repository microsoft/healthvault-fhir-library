// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(FhirMedication))]
    public class MedicationToHealthVaultTests
    {
        [TestMethod]
        public void WhenMedicationTransformedToHealthVault_ThenNameIsCopiedFromCode()
        {
            const string medicationName = "Amoxicillin 250mg/5ml Suspension";
            var fhirMedication = new FhirMedication()
            {
                Code = new CodeableConcept()
                {
                    Text = medicationName
                }
            };

            var hvMedication = fhirMedication.ToHealthVault() as HVMedication;

            Assert.AreEqual(medicationName, hvMedication.Name?.Text);
        }

        [TestMethod]
        public void WhenMedicationTransformedToHealthVault_AndOnlyIngredientItemIsCodeableConcept_AndIsDifferentFromCode_ThenGenericNameIsCopiedFromIngredientItemCodeableConcept()
        {
            const string ingredientName = "Capecitabine (substance)";
            var fhirMedication = new FhirMedication()
            {
                Code = new CodeableConcept()
                {
                    Text = "Capecitabine 500mg oral tablet (Xeloda)"
                },
                Ingredient = new System.Collections.Generic.List<FhirMedication.IngredientComponent>
                {
                    new FhirMedication.IngredientComponent()
                    {
                        Item = new CodeableConcept(system:"http://snomed.info/sct",
                            code:"386906001",display: ingredientName,text:null)
                    }
                }
            };

            var hvMedication = fhirMedication.ToHealthVault() as HVMedication;

            Assert.AreEqual(ingredientName, hvMedication.GenericName?.Text);
        }

        [TestMethod]
        public void WhenMedicationTransformedToHealthVault_AndOnlyIngredientItemIsCodeableConcept_AndIsSameAsCode_ThenGenericNameIsNotCopied()
        {
            const string name = "Capecitabine (substance)";
            var fhirMedication = new FhirMedication()
            {
                Code = new CodeableConcept()
                {
                    Text = name
                },
                Ingredient = new System.Collections.Generic.List<FhirMedication.IngredientComponent>
                {
                    new FhirMedication.IngredientComponent()
                    {
                        Item = new CodeableConcept()
                        {
                            Text= name
                        }
                    }
                }
            };

            var hvMedication = fhirMedication.ToHealthVault() as HVMedication;

            Assert.IsNull(hvMedication.GenericName);
        }

        [TestMethod]
        public void WhenMedicationTransformedToHealthVault_AndOnlyIngredientItemIsCodeableConcept_ThenStrengthIsCopiedFromIngredientAmountNumerator()
        {
            const int ingredientAmount = 500;
            var fhirMedication = new FhirMedication()
            {
                Code = new CodeableConcept()
                {
                    Text = "Capecitabine 500mg oral tablet (Xeloda)"
                },
                Ingredient = new System.Collections.Generic.List<FhirMedication.IngredientComponent>
                {
                    new FhirMedication.IngredientComponent()
                    {
                        Item = new CodeableConcept(system:"http://snomed.info/sct",
                            code:"386906001",display: "Capecitabine (substance)",text:null),
                        Amount = new Ratio
                        {
                            Numerator = new Quantity(ingredientAmount,"mg")
                        }
                    }
                }
            };

            var hvMedication = fhirMedication.ToHealthVault() as HVMedication;

            Assert.AreEqual(ingredientAmount, hvMedication.Strength?.Structured?.First()?.Value);
        }
    }
}
