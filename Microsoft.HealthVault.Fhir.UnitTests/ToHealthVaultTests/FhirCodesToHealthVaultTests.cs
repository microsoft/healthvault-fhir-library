// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(Codes))]
    public class FhirCodesToHealthVaultTests
    {
        [TestMethod]
        public void WhenFhirCodeableConceptConvertedToHealthVault_ThenTextisCopied()
        {
            const string txt = "SomeText";
            var codeableConcept = new CodeableConcept()
            {
                Text = txt
            };

            var codableValue = codeableConcept.GetCodableValue();

            Assert.AreEqual(txt, codableValue.Text);
        }

        [TestMethod]
        public void WhenFhirCodeableConceptConvertedToHealthVault_ThenUniqueDisplayTextIsCopiedToText()
        {
            const string displayText = "DisplayText";
            var codeableConcept = new CodeableConcept()
            {
                Coding = new List<Coding>
                {
                    new Coding("system","code",displayText),
                    new Coding("system1","code2",displayText)
                }
            };
            var codeableConcept2 = new CodeableConcept()
            {
                Coding = new List<Coding>
                {
                    new Coding("system","code",displayText),
                    new Coding("system2","code1","displayText2")
                }
            };

            var codableValue = codeableConcept.GetCodableValue();
            var codableValue2 = codeableConcept2.GetCodableValue();


            Assert.AreEqual(displayText, codableValue.Text);
            Assert.AreNotEqual(displayText, codableValue2.Text);
        }

        [TestMethod]
        public void WhenFhirCodeableConceptConvertedToHealthVault_AndTextExists_ThenDisplayTextIsNotCopied()
        {
            const string displayText = "DisplayText";
            var codeableConcept = new CodeableConcept()
            {
                Text = "SomeText",
                Coding = new List<Coding>
                {
                    new Coding("system","code",displayText),
                    new Coding("system1","code2",displayText)
                }
            };

            var codableValue = codeableConcept.GetCodableValue();


            Assert.AreNotEqual(displayText, codableValue.Text);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_ThenValueAndVocabularyIsColonSeperatedFromCode()
        {
            var coding = new Coding
            {
                Code = string.Format(HealthVaultVocabularies.HealthVaultCodedValueFormat, HealthVaultVocabularies.VitalStatistics, "wgt"),
                Version = "1",
                System = VocabularyUris.HealthVaultVocabulariesUri,
                Display = "Body Weight",
            };

            var codedValue = coding.GetCodedValue();

            Assert.AreEqual(HealthVaultVocabularies.VitalStatistics, codedValue.VocabularyName);
            Assert.AreEqual("wgt", codedValue.Value);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_AndVocabularyIsEmpty_AndSystemIsWellFormedUri_ThenVocabularyIsSetAsFhir()
        {
            var coding = new Coding
            {
                System = FhirCategories.Hl7Observation,
                Code = "vital-signs",
                Display = "Vital Signs",
            };

            var codedValue = coding.GetCodedValue();

            Assert.AreEqual(HealthVaultVocabularies.Fhir, codedValue.VocabularyName);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_ThenSystemIsCopiedAsFamily()
        {
            var coding = new Coding
            {
                System = FhirCategories.Hl7Observation,
                Code = "vital-signs",
                Display = "Vital Signs",
            };

            var codedValue = coding.GetCodedValue();

            Assert.AreEqual(FhirCategories.Hl7Observation, codedValue.Family);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_AndSystemIsSetAsHVVocabularyUri_ThenFamilyIsSetAsWC()
        {
            var coding = new Coding
            {
                Code = string.Format(HealthVaultVocabularies.HealthVaultCodedValueFormat, HealthVaultVocabularies.VitalStatistics, "wgt"),
                Version = "1",
                System = VocabularyUris.HealthVaultVocabulariesUri,
                Display = "Body Weight",
            };

            var codedValue = coding.GetCodedValue();

            Assert.AreEqual(HealthVaultVocabularies.Wc, codedValue.Family);
        }
    }
}
