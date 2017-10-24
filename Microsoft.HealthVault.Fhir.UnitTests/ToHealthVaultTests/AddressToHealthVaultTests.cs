// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory("Fhir to ItemTypes")]
    public class AddressToHealthVaultTests
    {
        [TestMethod]
        public void WhenAddressTransformedToHealthVault_ThenValueEqual()
        {
            string[] lines = { "1", "Microsoft Way" };
            var fhirAddress = new Hl7.Fhir.Model.Address
            {
                Type = Hl7.Fhir.Model.Address.AddressType.Postal,
                Text = "John's Postal Address",
                Line = lines.ToList(),
                City = "Redmond",
                District = "King County",
                State = "WA",
                Country = "US",
                PostalCode = "98052"
            };

            var hvAddress = fhirAddress.ToHealthVault();

            Assert.IsNotNull(hvAddress);
            Assert.AreEqual(hvAddress.Description, fhirAddress.Text);
            Assert.AreEqual(fhirAddress.Line.Count(), hvAddress.Street.Count());
            Assert.AreEqual(fhirAddress.City, hvAddress.City);
            Assert.AreEqual(fhirAddress.PostalCode, hvAddress.PostalCode);
            Assert.AreEqual(fhirAddress.State, hvAddress.State);
            Assert.AreEqual(fhirAddress.Country, hvAddress.Country);
            Assert.AreEqual(fhirAddress.District, hvAddress.County);
        }
    }
}
