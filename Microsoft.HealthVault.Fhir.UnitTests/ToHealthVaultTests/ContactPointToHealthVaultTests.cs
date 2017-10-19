// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory("Fhir to ItemTypes")]
    public class ContactPointToHealthVaultTests
    {
        [TestMethod]
        public void WhenEmailContactPointTransformedToHealthVault_ThenResultEqual()
        {
            var fhirEmail = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Email,
                Value = "john.doe@example.com",
                Rank = 1
            };

            var hvEmail = fhirEmail.ToHealthVault<Email>();

            Assert.IsInstanceOfType(hvEmail, typeof(Email));
            Assert.AreEqual(fhirEmail.Value, hvEmail.Address);
            Assert.IsNotNull(hvEmail.IsPrimary);
            Assert.IsTrue(hvEmail.IsPrimary.Value);
        }
        [TestMethod]
        public void WhenPhoneContactPointTransformedToHealthVault_ThenResultEqual()
        {
            var fhirPhone = new ContactPoint
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = "1-425-555-0100"
            };

            var hvEmail = fhirPhone.ToHealthVault<Phone>();

            Assert.IsInstanceOfType(hvEmail, typeof(Phone));
            Assert.AreEqual(fhirPhone.Value, hvEmail.Number);
            Assert.IsNull(hvEmail.IsPrimary);            
        }
    }

}
