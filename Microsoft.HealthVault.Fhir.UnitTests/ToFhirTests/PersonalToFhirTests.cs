// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class PersonalToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultPersonalTransformedToFhir_ThenValuesEqual()
        {
            var personal = new Personal
            {
                Name = new Name
                {
                    Full = "Dr. John Phillip Doe, Jr.",
                    First = "John",
                    Middle = "Phillip",
                    Last = "Doe",
                    Suffix = new CodableValue("Junior", "Jr", "name-suffixes", "wc", "1"),
                    Title = new CodableValue("Dr.", "Dr", "name-prefixes", "wc", "1"),
                },
                BirthDate = new HealthServiceDateTime
                {
                    Date = new HealthServiceDate(1975, 2, 5),
                    Time = new ApproximateTime(1, 30, 34, 15),
                },
                DateOfDeath = new ApproximateDateTime
                {
                    ApproximateDate = new ApproximateDate(2075, 5, 7),
                },
                SocialSecurityNumber = "000-12-3456",
                BloodType = new CodableValue("A+", "A+", "blood-types", "wc", "1"),
                Religion = new CodableValue("Agnostic", "Agn", "religion", "wc", "1"),
                MaritalStatus = new CodableValue("Never Married", "NM", "marital-status", "wc", "1"),
                EmploymentStatus = "Employed",
                IsDeceased = true,
                IsVeteran = true,
                Ethnicity = new CodableValue("Other Race", "8", "ethnicity-types", "wc", "1"),
                HighestEducationLevel = new CodableValue("College Graduate", "ColG", "Education-level", "wc", "1"),
                OrganDonor = "Organ Donor",
                IsDisabled = false,
            };

            var patient = personal.ToFhir();

            Assert.IsNotNull(patient);
            Assert.AreEqual("Dr. John Phillip Doe, Jr.", patient.Name[0].Text);
            Assert.AreEqual("John", patient.Name[0].Given.ToList()[0]);
            Assert.AreEqual("Phillip", patient.Name[0].Given.ToList()[1]);
            Assert.AreEqual("Doe", patient.Name[0].Family);
            Assert.AreEqual("name-prefixes:Dr", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-title").Value).Coding[0].Code);
            Assert.AreEqual("name-suffixes:Jr", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-suffix").Value).Coding[0].Code);
            Assert.AreEqual("1975-02-05", patient.BirthDate);
            Assert.AreEqual("000-12-3456", patient.Identifier[0].Value);
            Assert.AreEqual("2075-05-07T00:00:00-07:00", ((FhirDateTime)patient.Deceased).Value);
            Assert.AreEqual("blood-types:A+",((CodeableConcept)patient.Extension.First(x => x.Url == "patient-blood-type").Value).Coding[0].Code);
            Assert.AreEqual("religion:Agn", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-religion").Value).Coding[0].Code);
            Assert.AreEqual("marital-status:NM", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-marital-status").Value).Coding[0].Code);
            Assert.AreEqual("ethnicity-types:8", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-ethnicity").Value).Coding[0].Code);
            Assert.AreEqual("Education-level:ColG", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-highest-education-level").Value).Coding[0].Code);
            Assert.AreEqual("Employed", ((FhirString)patient.Extension.First(x => x.Url == "patient-employment-status").Value).Value);
            Assert.AreEqual("Organ Donor", ((FhirString)patient.Extension.First(x => x.Url == "patient-organ-donor").Value).Value);
        }
    }
}
