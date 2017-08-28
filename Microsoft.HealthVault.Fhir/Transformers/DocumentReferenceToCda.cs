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
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Fhir.FhirExtensions;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class DocumentReferenceToCda
    {
        internal static CDA ToCDA(this DocumentReference documentReference)
        {
            var contentComponent = documentReference.GetFirstContentComponentWithData();

            if (contentComponent == null)
            {
                throw new ArgumentException("DocumentReference must have a content component with data");
            }

            CDA cda = documentReference.ToThingBase<CDA>();

            string xml = Encoding.UTF8.GetString(contentComponent.Attachment.Data);
            XPathDocument xpDoc = ThingBaseToFhirDocumentReference.GetXPathNavigatorFromXml(xml);

            cda.TypeSpecificData = xpDoc;

            return cda;
        }

    }
}
