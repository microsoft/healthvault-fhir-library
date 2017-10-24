// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Reflection;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Helpers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class DocumentReferenceToHealthVaultFileTests
    {
        [TestMethod]
        public void WhenFhirFileTransformedToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirFile.json");

            var fhirParser = new FhirJsonParser();
            var documentReference = fhirParser.Parse<DocumentReference>(json);

            var file = documentReference.ToHealthVault() as File;

            Assert.IsNotNull(file);
            Assert.IsNotNull(file.ContentType);
            Assert.AreEqual(file.ContentType.ToString(), documentReference.Content[0].Attachment.ContentType);

            string fhirAttachmentDataBase64Encoded = JObject.Parse(json)["content"][0]["attachment"]["data"].ToString();
            string hvFileContentBase64Encoded = Convert.ToBase64String(file.Content);
            Assert.AreEqual(fhirAttachmentDataBase64Encoded, hvFileContentBase64Encoded);
        }

        [TestMethod]
        public void WhenHealthVaultFileTransformedToFhirToHealthVault_ThenValuesEqual()
        {
            File inputFile = new File();

            string resourceName = "Microsoft.HealthVault.Fhir.UnitTests.Samples.HealthVaultIcon.png";
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                DocumentReferenceHelper.WriteByteArrayToHealthVaultFile(inputFile, DocumentReferenceHelper.StreamToByteArray(stream));
            }

            inputFile.ContentType = new CodableValue("image/png");
            inputFile.EffectiveDate = new NodaTime.LocalDateTime(2016, 12, 10, 2, 45, 30);

            var documentReference = inputFile.ToFhir() as DocumentReference;

            var file = documentReference.ToHealthVault() as File;

            Assert.IsNotNull(file);
            Assert.IsNotNull(file.ContentType);
            Assert.AreEqual(file.ContentType.ToString(), inputFile.ContentType.ToString());

            string inputFileContentBase64Encoded = Convert.ToBase64String(inputFile.Content);
            string hvFileContentBase64Encoded = Convert.ToBase64String(file.Content);
            Assert.AreEqual(inputFileContentBase64Encoded, hvFileContentBase64Encoded);
        }
    }
}
