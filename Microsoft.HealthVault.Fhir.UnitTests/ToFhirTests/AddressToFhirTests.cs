// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory("ItemTypes.ToFhir")]
    public class AddressToFhirTests
    {
        [TestMethod]
        public void WhenAddressTransformedToFhir_ThenValuesEqual()
        {
            var address = new ItemTypes.Address
            {
                Description = "John's Office Address",
                IsPrimary = true,
                Street = { "1", "Microsoft Way" },
                City = "KEENE",
                State = "California",
                Country = "US",
                PostalCode = "93531",
                County = "NorthWest"
            };
            var fhirAddress = address.ToFhir();

            Assert.IsNotNull(fhirAddress);
            Assert.AreEqual(address.Description, fhirAddress.Text);
            Assert.AreEqual(address.Street.Count(), fhirAddress.Line.Count());
            Assert.AreEqual(address.City, fhirAddress.City);
            Assert.AreEqual(address.PostalCode, fhirAddress.PostalCode);
            Assert.AreEqual(address.State, fhirAddress.State);
            Assert.AreEqual(address.Country, fhirAddress.Country);
            Assert.AreEqual(address.County, fhirAddress.District);

            Assert.IsInstanceOfType(fhirAddress.Extension.Single().Value, typeof(FhirBoolean));
            var extensionValue = ((FhirBoolean)fhirAddress.Extension.Single().Value).Value;
            Assert.IsNotNull(extensionValue);
            Assert.AreEqual(true, extensionValue);
        }

        [TestMethod]
        public void WhenMinimumAddressTransformedToFhir_ThenValuesEqual()
        {
            var address = new ItemTypes.Address
            {
                Street = { "3804 Lakeland ", "Terrace" },
                City = "KEENE",
                Country = "US",
                PostalCode = "93531"
            };
            var fhirAddress = address.ToFhir();

            Assert.IsNotNull(fhirAddress);
            Assert.IsTrue(string.IsNullOrEmpty(fhirAddress.Text));
            Assert.IsTrue(string.IsNullOrEmpty(fhirAddress.State));
            Assert.IsTrue(string.IsNullOrEmpty(fhirAddress.District));
            Assert.IsTrue(fhirAddress.Extension.IsNullOrEmpty());

            Assert.AreEqual(address.Street.Count(), fhirAddress.Line.Count());
            Assert.AreEqual(address.City, fhirAddress.City);
            Assert.AreEqual(address.PostalCode, fhirAddress.PostalCode);
            Assert.AreEqual(address.Country, fhirAddress.Country);
        }
    }
}
