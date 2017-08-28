// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation cdas (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;
using System.Xml.XPath;
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
    public class DocumentReferenceToHealthVaultCdaTests
    {
        [TestMethod]
        public void WhenFhirCdaTransformedToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirCDA.json");

            var fhirParser = new FhirJsonParser();
            var documentReference = fhirParser.Parse<DocumentReference>(json);

            string fhirAttachmentDataBase64Encoded = JObject.Parse(json)["content"][0]["attachment"]["data"].ToString();

            string fhirXmlRaw = Encoding.UTF8.GetString(Convert.FromBase64String(fhirAttachmentDataBase64Encoded));

            XPathDocument fhirXPathDoc = DocumentReferenceHelper.GetXPathDocumentFromXml(fhirXmlRaw) ?? throw new Exception("Invalid XML");

            // XML gets generated using a common method in order to use in Assert.AreEqual
            string fhirXml = DocumentReferenceHelper.GetXmlFromXPathNavigator(fhirXPathDoc.CreateNavigator());

            var cda = documentReference.ToHealthVault() as CDA;

            Assert.IsNotNull(cda);
            Assert.IsNotNull(cda.TypeSpecificData);

            string cdaXml =  DocumentReferenceHelper.GetXmlFromXPathNavigator(cda.TypeSpecificData.CreateNavigator());
            Assert.AreEqual(fhirXml, cdaXml);
        }

        [TestMethod]
        public void WhenHealthVaultCdaToFhirToHealthVault_ThenValuesEqual()
        {
            string inputCdaXmlRaw = System.IO.File.ReadAllText(@"..\..\TestFiles\CDA.xml");

            XPathDocument xpDoc = DocumentReferenceHelper.GetXPathDocumentFromXml(inputCdaXmlRaw);

            CDA inputCda = new CDA();
            inputCda.TypeSpecificData = xpDoc;

            var documentReference = inputCda.ToFhir() as DocumentReference;

            var cda = documentReference.ToHealthVault() as CDA;

            XPathDocument cdaXPathDoc = DocumentReferenceHelper.GetXPathDocumentFromXml(inputCdaXmlRaw) ?? throw new Exception("Invalid XML");

            // XML gets generated using a common method in order to use in Assert.AreEqual
            string inputCdaXml = DocumentReferenceHelper.GetXmlFromXPathNavigator(cdaXPathDoc.CreateNavigator());

            Assert.IsNotNull(cda);
            Assert.IsNotNull(cda.TypeSpecificData);

            string cdaXml =  DocumentReferenceHelper.GetXmlFromXPathNavigator(cda.TypeSpecificData.CreateNavigator());
            Assert.AreEqual(inputCdaXml, cdaXml);
        }
    }
}
