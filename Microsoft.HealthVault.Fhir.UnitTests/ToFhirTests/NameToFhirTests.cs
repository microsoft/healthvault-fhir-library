// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System.Linq;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory("ItemTypes.ToFhir")]
    public class NameToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultNameTransformedToFhir_ThenValuesEqual()
        {
            var name = new Name
            {
                First = "John",
                Middle = "M",
                Last = "Doe",
                Full = "John Doe",
                Suffix = new CodableValue("The Second", "II", "name-suffixes", "wc", "1"),
                Title = new CodableValue("Mr.", "Mr", "name-prefixes", "wc", "1")
            };

            var fhirName = name.ToFhir();

            Assert.IsNotNull(fhirName);
            Assert.AreEqual(2, fhirName.Given.Count());
            Assert.AreEqual(name.First, fhirName.Given.First());
            Assert.AreEqual(name.Full, fhirName.Text);
            Assert.AreEqual(name.Last, fhirName.Family);
            Assert.AreEqual(1, fhirName.Suffix.Count());
            Assert.AreEqual(name.Suffix.Text, fhirName.Suffix.First());
            Assert.AreEqual(1, fhirName.Prefix.Count());
            Assert.AreEqual(name.Title.Text, fhirName.Prefix.First());
        }

        [TestMethod]
        public void WhenMinimumNameTransformedToFhir_ThenValuesEqual()
        {
            var name = new Name
            {
                Full = "John Doe",
            };

            var fhirName = name.ToFhir();

            Assert.IsNotNull(fhirName);
            Assert.AreEqual(0, fhirName.Given.Count());
            Assert.AreEqual(0, fhirName.Suffix.Count());
            Assert.AreEqual(0, fhirName.Prefix.Count());
            Assert.AreEqual(name.Full, fhirName.Text);
            Assert.IsTrue(string.IsNullOrEmpty(fhirName.Family));            
        }
    }
}

