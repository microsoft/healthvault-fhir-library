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
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory("ItemTypes.ToFhir")]
    public class PhoneToFhirTests
    {
        [TestMethod]
        public void WhenPhoneTransformedToFhir_ThenValuesEqual()
        {
            var phone = new Phone
            {
                Description = "John's work phone",
                Number = "555-1212",
                IsPrimary = true
            };

            var fhirPhone = phone.ToFhir();

            Assert.IsNotNull(fhirPhone);
            Assert.IsNotNull(fhirPhone.System);
            Assert.AreEqual(ContactPoint.ContactPointSystem.Phone, fhirPhone.System);
            Assert.AreEqual(phone.Number, fhirPhone.Value);
            Assert.IsNotNull(fhirPhone.Rank);
            Assert.AreEqual(1, fhirPhone.Rank.Value);            

            Assert.IsInstanceOfType(fhirPhone.Extension.Single().Value, typeof(FhirString));
            var extensionValue = ((FhirString)fhirPhone.Extension.Single().Value).Value;
            Assert.IsNotNull(extensionValue);
            Assert.AreEqual(phone.Description, extensionValue);
        }

        [TestMethod]
        public void WhenMinimumPhoneTransformedToFhir_ThenValuesEqual()
        {
            var phone = new Phone
            {
                Number = "555-1212",
            };

            var fhirPhone = phone.ToFhir();

            Assert.IsNotNull(fhirPhone);
            Assert.IsNotNull(fhirPhone.System);
            Assert.AreEqual(ContactPoint.ContactPointSystem.Phone, fhirPhone.System);
            Assert.AreEqual(phone.Number, fhirPhone.Value);
            Assert.IsNull(fhirPhone.Rank);
            Assert.IsTrue(fhirPhone.Extension.IsNullOrEmpty());
        }
    }
}

