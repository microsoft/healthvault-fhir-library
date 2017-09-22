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
    public class EmailToFhirTests
    {
        [TestMethod]
        public void WhenEmailTransformedToFhir_ThenValuesEqual()
        {
            var email = new Email
            {
                Description = "John's work email",
                Address = "johh.doe@example.com",
                IsPrimary = true
            };

            var fhirEmail = email.ToFhir();

            Assert.IsNotNull(fhirEmail);
            Assert.IsNotNull(fhirEmail.System);
            Assert.AreEqual(ContactPoint.ContactPointSystem.Email, fhirEmail.System);
            Assert.AreEqual(email.Address, fhirEmail.Value);
            Assert.IsNotNull(fhirEmail.Rank);
            Assert.AreEqual(1, fhirEmail.Rank.Value);

            Assert.IsInstanceOfType(fhirEmail.Extension.Single().Value, typeof(FhirString));
            var extensionValue = ((FhirString)fhirEmail.Extension.Single().Value).Value;
            Assert.IsFalse(string.IsNullOrEmpty(extensionValue));
            Assert.AreEqual(email.Description, extensionValue);
        }
        [TestMethod]
        public void WhenMinimumEmailTransformedToFhir_ThenValuesEqual()
        {
            var email = new Email
            {
                Address = "johh.doe@example.com",
            };

            var fhirEmail = email.ToFhir();

            Assert.IsNotNull(fhirEmail);
            Assert.IsNotNull(fhirEmail.System);
            Assert.AreEqual(ContactPoint.ContactPointSystem.Email, fhirEmail.System);
            Assert.AreEqual(email.Address, fhirEmail.Value);
            Assert.IsNull(fhirEmail.Rank);
            Assert.IsTrue(fhirEmail.Extension.IsNullOrEmpty());
        }
    }
}

