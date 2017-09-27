// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Reflection;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class FileToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultFileTransformedToFhir_ThenValuesEqual()
        {
            File file = new File();

            string resourceName = "Microsoft.HealthVault.Fhir.UnitTests.Samples.HealthVaultIcon.png";
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                DocumentReferenceHelper.WriteByteArrayToHealthVaultFile(file, DocumentReferenceHelper.StreamToByteArray(stream));
            }

            file.ContentType = new CodableValue("image/png");

            var documentReference = file.ToFhir() as DocumentReference;

            Assert.IsNotNull(documentReference);
            Assert.IsNotNull(documentReference.Type);
            Assert.AreEqual(documentReference.Content.Count, 1);
            Assert.IsNotNull(documentReference.Content[0].Attachment);
            Assert.IsNotNull(documentReference.Content[0].Attachment.Data);
            Assert.AreEqual(documentReference.Content[0].Attachment.ContentType, file.ContentType.ToString());

            string fileContentBase64Encoded = Convert.ToBase64String(file.Content);
            string fhirAttachmentDataBase64Encoded = Convert.ToBase64String(documentReference.Content[0].Attachment.Data);
            Assert.AreEqual(fhirAttachmentDataBase64Encoded, fileContentBase64Encoded);
        }
    }
}
