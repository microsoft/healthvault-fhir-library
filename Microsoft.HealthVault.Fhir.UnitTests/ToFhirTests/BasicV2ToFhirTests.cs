// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class BasicV2ToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultBasicV2TransformedToFhir_ThenCodeAndValuesEqual()
        {
            var basic = new ItemTypes.BasicV2
            {
                Gender = Gender.Female,
                BirthYear = 1975,
                City = "Redmond",
                StateOrProvince = new CodableValue("Washington", "WA", "states", "wc", "1"),
                PostalCode = "98052",
                Country = new CodableValue("United States of America", "US", "iso3166", "iso", "1"),
                FirstDayOfWeek = DayOfWeek.Sunday,
                Languages =
                {
                    new Language(new CodableValue("English", "en", "iso639-1", "iso", "1"), true),
                    new Language(new CodableValue("French", "fr", "iso639-1", "iso", "1"), false),
                }
            };
            
            var patient = basic.ToFhir();

            Assert.IsNotNull(patient);
            Assert.AreEqual(AdministrativeGender.Female, patient.Gender.Value);
            Assert.AreEqual(1975, ((FhirDecimal)patient.Extension.First(x => x.Url == HealthVaultExtensions.PatientBirthYear).Value).Value);
            Assert.AreEqual("0", ((Coding)patient.Extension.First(x => x.Url == HealthVaultExtensions.FirstDayOfWeek).Value).Code);
            Assert.AreEqual("Sunday", ((Coding)patient.Extension.First(x => x.Url == HealthVaultExtensions.FirstDayOfWeek).Value).Display);

            var basicAddress = patient.Extension.First(x => x.Url == HealthVaultExtensions.PatientBasicAddress);
            Assert.AreEqual("Redmond", ((FhirString)basicAddress.Extension.First(x => x.Url == HealthVaultExtensions.PatientBasicAddressCity).Value).Value);
            Assert.AreEqual("WA", ((CodeableConcept)basicAddress.Extension.First(x => x.Url == HealthVaultExtensions.PatientBasicAddressState).Value).Coding[0].Code);
            Assert.AreEqual("98052", ((FhirString)basicAddress.Extension.First(x => x.Url == HealthVaultExtensions.PatientBasicAddressPostalCode).Value).Value);
            Assert.AreEqual("US", ((CodeableConcept)basicAddress.Extension.First(x => x.Url == HealthVaultExtensions.PatientBasicAddressCountry).Value).Coding[0].Code);

            Assert.AreEqual(2, patient.Communication.Count);
            Assert.AreEqual("English", patient.Communication[0].Language.Text);
            Assert.AreEqual(true, patient.Communication[0].Preferred);
        }
    }
}
