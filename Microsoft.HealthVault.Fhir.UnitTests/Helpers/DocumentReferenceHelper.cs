// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation documentReferences (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Fhir.UnitTests.Helpers
{
    class DocumentReferenceHelper
    {
        public static string GetXmlFromXPathNavigator(XPathNavigator xpNav)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                xpNav.WriteSubtree(xmlWriter);
            }
            return sb.ToString();
        }

        public static XPathDocument GetXPathDocumentFromXml(string xml)
        {
            XPathDocument xpDoc;

            using (TextReader txtReader = new StringReader(xml))
            {
                xpDoc = new XPathDocument(txtReader);
            }

            return xpDoc;
        }

        public static byte[] StreamToByteArray(System.IO.Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, read);
                }
                buffer = memoryStream.ToArray();
            }

            return buffer;
        }

        public static void WriteByteArrayToHealthVaultFile(ItemTypes.File file, byte[] bytesArray)
        {
            MethodInfo getBlobStoreMethod = file.GetType().GetMethod("GetBlobStore", BindingFlags.NonPublic | BindingFlags.Instance);
            Type healthRecordAccessorType = Type.GetType("Microsoft.HealthVault.Thing.HealthRecordAccessor, Microsoft.HealthVault");
            var healthRecordAccessor = healthRecordAccessorType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).First(constructor => constructor.GetParameters().Length == 0).Invoke(null);
            var blobStore = getBlobStoreMethod.Invoke(file, new object[] { healthRecordAccessor });
            MethodInfo newBlob = blobStore.GetType().GetMethod("NewBlob", new Type[] { typeof(string), typeof(string) });

            ItemTypes.Blob blob = (ItemTypes.Blob)newBlob.Invoke(blobStore, new object[] { string.Empty, "image/jpeg"});
            blob.WriteInline(bytesArray);
        }
    }
}
