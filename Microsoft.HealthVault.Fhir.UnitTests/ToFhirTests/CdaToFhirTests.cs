// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation documentReferences (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text;
using System.Xml.XPath;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Helpers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class CdaToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultCdaTransformedToFhir_ThenValuesEqual()
        {
            string cdaXmlRaw = System.IO.File.ReadAllText(@"..\..\TestFiles\CDA.xml");

            XPathDocument xpDoc = DocumentReferenceHelper.GetXPathDocumentFromXml(cdaXmlRaw) ?? throw new Exception("Invalid XML");

            CDA cda = new CDA();
            cda.TypeSpecificData = xpDoc;

            var documentReference = cda.ToFhir() as DocumentReference;

            Assert.IsNotNull(documentReference);
            Assert.IsNotNull(documentReference.Type);
            Assert.AreEqual(documentReference.Content.Count, 1);
            Assert.IsNotNull(documentReference.Content[0].Attachment);
            Assert.IsNotNull(documentReference.Content[0].Attachment.Data);
            Assert.IsNotNull(documentReference.Content[0].Attachment.ContentType, "application/xml");

            string cdaXml = DocumentReferenceHelper.GetXmlFromXPathNavigator(cda.TypeSpecificData.CreateNavigator());
            string cdaContentBase64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(cdaXml));

            string fhirXmlRaw = Encoding.UTF8.GetString(documentReference.Content[0].Attachment.Data);

            XPathDocument fhirXPathDoc;
            using (TextReader txtReader = new StringReader(fhirXmlRaw))
            {
                fhirXPathDoc = new XPathDocument(txtReader);
            }

            string fhirXml = DocumentReferenceHelper.GetXmlFromXPathNavigator(xpDoc.CreateNavigator());
            string fhirAttachmentDataBase64Encoded = Convert.ToBase64String(documentReference.Content[0].Attachment.Data);

            Assert.AreEqual(fhirAttachmentDataBase64Encoded, cdaContentBase64Encoded);
        }
    }
}
