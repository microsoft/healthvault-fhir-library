// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory("Fhir to ItemTypes")]
    public class HumanNameToNameTests
    {
        [TestMethod]
        public void WhenHumanNameTransformedToHealthVault_ThenValuesEqual()
        {
            string[] givenNames = { "John", "M" };
            string[] prefixes = { "Mr.", "Mr" };
            string[] suffixes = { "II", "The Second" };
            var fhirName = new HumanName
            {
                Text = "John Doe",
                Family = "Doe",
                Given = givenNames.ToList(),
                Prefix = prefixes.ToList(),
                Suffix = suffixes.ToList()
            };

            var hvName = fhirName.ToHealthVault();

            Assert.AreEqual(fhirName.Family, hvName.Last);
            Assert.IsNotNull(hvName.Title);
            Assert.AreEqual(fhirName.Prefix.First(), hvName.Title.Text);
            Assert.IsNotNull(hvName.Suffix);
            Assert.AreEqual(fhirName.Suffix.First(), hvName.Suffix.Text);
            Assert.AreEqual(fhirName.Given.First(), hvName.First);
            Assert.AreEqual(fhirName.Given.ElementAt(1), hvName.Middle);
            Assert.AreEqual(fhirName.Text, hvName.Full);
        }

        [TestMethod]
        public void WhenMinimumHumanNameTransformedToHealthVault_ThenValuesEqual()
        {
            var fhirName = new HumanName
            {
                Text = "John Doe"
            };

            var hvName = fhirName.ToHealthVault();

            Assert.IsTrue(string.IsNullOrEmpty(hvName.Last));
            Assert.IsNull(hvName.Title);
            Assert.IsTrue(string.IsNullOrEmpty(hvName.First));
            Assert.IsTrue(string.IsNullOrEmpty(hvName.Middle));
            Assert.IsNull(hvName.Suffix);
            Assert.AreEqual(fhirName.Text, hvName.Full);
        }
    }
}
