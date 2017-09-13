// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
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

            var codableValue = codeableConcept.ToCodableValue();

            Assert.AreEqual(txt, codableValue.Text);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_ThenValueIsCopiedFromCode()
        {
            var coding = HealthVaultThingTypeNameCodes.BloodGlucose;

            var codedValue = coding.ToCodedValue();
            
            Assert.AreEqual(HealthVaultThingTypeNameCodes.BloodGlucoseCode, codedValue.Value);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_ThenSystemIsCopiedAsVocabulary()
        {
            var coding = new Coding
            {
                System = FhirCategories.Hl7Observation,
                Code = "vital-signs",
                Display = "Vital Signs",
            };

            var codedValue = coding.ToCodedValue();

            Assert.AreEqual(FhirCategories.Hl7Observation, codedValue.VocabularyName);
        }

        [TestMethod]
        public void WhenFhirCodingConvertedToHealthVault_AndVocabularyIsFromHealthVault_ThenVocabularyIsExtractedFromSystem()
        {
            var coding = HealthVaultVitalStatisticsCodes.BodyHeight;

            var codedValue = coding.ToCodedValue();

            Assert.AreEqual(HealthVaultVocabularies.VitalStatistics, codedValue.VocabularyName);
        }
    }
}
