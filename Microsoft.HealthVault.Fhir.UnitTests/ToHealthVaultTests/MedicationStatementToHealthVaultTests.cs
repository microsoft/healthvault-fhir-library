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
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FhirMedication = Hl7.Fhir.Model.Medication;
using HVMedication = Microsoft.HealthVault.ItemTypes.Medication;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(MedicationStatement))]
    public class MedicationStatementToHealthVaultTests
    {
        [TestMethod]
        public void WhenMedicationStatementTransformedToHealthVault_ThenDosageIsCopiedToDoseFrequencyAndRoute()
        {
            var embeddedMedication = new FhirMedication()
            {
                Code = new CodeableConcept()
                {
                    Text = "Amoxicillin 250mg/5ml Suspension"
                }
            };
            string embeddedMedicationId = "med" + Guid.NewGuid();
            embeddedMedication.Id = embeddedMedicationId;

            const int dosage = 1;
            const int period = 8;
            const string routeCode = "po";
            var medicationStatement = new MedicationStatement()
            {
                Medication = new ResourceReference(embeddedMedicationId),
                Dosage = new System.Collections.Generic.List<Dosage>
               {
                   new Dosage
                   {
                       Dose = new SimpleQuantity()
                       {
                           Value = dosage,
                           Unit = "Tablets",
                           System = HealthVaultCodesToFhir.GetVocabularyUrl("medication-dose-units","1"),
                           Code = "tablet"
                       },
                       Timing = new Timing
                       {
                           Repeat = new Timing.RepeatComponent
                           {
                               Period = period,
                               PeriodUnit = Timing.UnitsOfTime.H
                           }
                       },
                       Route=new CodeableConcept
                       {
                           Text="By mouth",
                           Coding = new System.Collections.Generic.List<Coding>
                           {
                               new Coding
                               {
                                   Code=$"medication-routes:{routeCode}",
                                   System=VocabularyUris.HealthVaultVocabulariesUri,
                                   Version="2"
                               }
                           }
                       }
                   }
               }
            };
            medicationStatement.Contained.Add(embeddedMedication);

            var hvMedication = medicationStatement.ToHealthVault() as HVMedication;

            Assert.AreEqual(dosage, hvMedication?.Dose?.Structured?.First()?.Value);
            Assert.AreEqual(period, hvMedication?.Frequency?.Structured?.First()?.Value);
            Assert.AreEqual(routeCode, hvMedication?.Route?.First()?.Value);
        }
    }
}
