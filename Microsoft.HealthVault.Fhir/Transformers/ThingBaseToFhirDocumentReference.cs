// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        public static void DoTransforms(this DocumentReference documentReference, ThingBase thing)
        {
            ThingBaseToFhirDocumentReference.DoTransformInternal(documentReference, thing);
        }
    }
    internal static class ThingBaseToFhirDocumentReference
    {
        internal static DocumentReference DoTransformInternal(DocumentReference documentReference, ThingBase thing)
        {
            documentReference.SetStatusAsCurrent();

            documentReference.SetIndexed(thing.EffectiveDate);

            documentReference.SetType();

            documentReference.AddCommonData(thing.CommonData);

            return documentReference;
        }

        internal static DocumentReference XmlToDocumentReference(DocumentReference documentReference, string xml)
        {
            var content = new DocumentReference.ContentComponent();

            content.Attachment = new Attachment();

            content.Attachment.ContentType = "application/xml";

            byte[] bytesXml = Encoding.UTF8.GetBytes(xml);
            content.Attachment.Data = new byte[bytesXml.Length];

            bytesXml.CopyTo(content.Attachment.Data, 0);

            documentReference.Content.Add(content);

            return documentReference;
        }

        internal static string GetXmlFromXPathNavigator(XPathNavigator xpNav)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                xpNav.WriteSubtree(xmlWriter);
            }

            return sb.ToString();
        }

        internal static XPathDocument GetXPathNavigatorFromXml(string xml)
        {
            XPathDocument xpDoc = null;
            using (TextReader txtReader = new StringReader(xml))
            {
                xpDoc = new XPathDocument(txtReader);
            }

            return xpDoc;
        }
    }
}
