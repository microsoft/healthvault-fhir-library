// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation things (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class DocumentReferenceToHealthVaultTests
    {
        [TestMethod]
        public void WhenFhirDocumentReferenceTransformedToHealthVault_ThenEffectiveDateIsSet()
        {
            var json = SampleUtil.GetSampleContent("FhirFile.json");

            var fhirParser = new FhirJsonParser();
            var documentReference = fhirParser.Parse<DocumentReference>(json);

            var thing = documentReference.ToHealthVault() as ThingBase;

            Assert.IsNotNull(thing);
            Assert.IsNotNull(thing.EffectiveDate);

            var indexed = documentReference.Indexed.Value;
            var indexedLocalDateTime = new NodaTime.LocalDateTime(indexed.Year, indexed.Month, indexed.Day, indexed.Hour, indexed.Minute, indexed.Second);
            Assert.AreEqual(thing.EffectiveDate.Value, indexedLocalDateTime);
        }
    }
}
