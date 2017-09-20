// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HVAddress = Microsoft.HealthVault.ItemTypes.Address;
using HVProcedure = Microsoft.HealthVault.ItemTypes.Procedure;
using FhirProcedure = Hl7.Fhir.Model.Procedure;
using System.Linq;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Transformers;
using NodaTime;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory("Procedure")]
    public class ProcedureToFhirTests
    {
        [TestMethod]
        public void WhenHealthVaultProcedureTransformedToFhir_ThenValuesAndCodesEqual()
        {
            #region Settingup objects
            var now = new LocalDateTime(2017,09,20,16,30,10,100);            
            var personId = Guid.NewGuid();
            var hvProcedure = new HVProcedure
            {
                When = new ApproximateDateTime
                {
                    ApproximateDate = new ApproximateDate(now.Year, now.Month, now.Day),
                    ApproximateTime = new ApproximateTime(now.Hour, now.Minute, now.Second, now.Millisecond)
                },
                Name = new CodableValue("Operative procedure on fingers", new CodedValue("215000", "SnomedProcedures", "Snomed", "Jan2008")),
                AnatomicLocation = new CodableValue("Metacarpophalangeal joint structure of index finger", new CodedValue("289002", "SnomedBodyLocation", "Snomed", "Jan2008")),
                PrimaryProvider = new PersonItem
                {
                    PersonType = new CodableValue("Provider", new CodedValue("1", "person-types", "wc", "1")),
                    Name = new Name("John Doe", "John", "T", "Doe"),
                    Organization = "Care Hospitals Inc",
                    PersonId = personId.ToString(),
                    ProfessionalTraining = "Certified Medical Assistant",
                    ContactInformation = new ContactInfo()
                },
                SecondaryProvider = new PersonItem
                {
                    PersonType = new CodableValue("Provider", new CodedValue("1", "person-types", "wc", "1")),
                    Name = new Name("Justin Case", "Just", "in", "Case")
                }
            };

            var address1 = new HVAddress
            {
                City = "John's Work City",
                State = "John's Work State",
                County = "John's Work County",
                PostalCode = "J12309",
                Country = "Kingdom of John",
                IsPrimary = true,
                Description = "This is John's Primary (Work) Address"
            };
            address1.Street.Add("John's Clinic");
            address1.Street.Add("John's Lane");
            hvProcedure.PrimaryProvider.ContactInformation.Address.Add(address1);

            var address2 = new HVAddress
            {
                City = "John's Home City",
                State = "John's Home State",
                County = "John's Home County",
                PostalCode = "J12310",
                Country = "Kingdom of John",
                Description = "This is John's Home Address"
            };
            address1.Street.Add("John's Home");
            hvProcedure.PrimaryProvider.ContactInformation.Address.Add(address2);
            #endregion

            FhirProcedure fhirProcedure = hvProcedure.ToFhir();

            Assert.IsNotNull(fhirProcedure);

            //Code
            Assert.IsFalse(fhirProcedure.Code.IsNullOrEmpty());
            Assert.AreEqual("Operative procedure on fingers", fhirProcedure.Code.Text);
            Assert.IsNotNull(fhirProcedure.Code.Coding.FirstOrDefault());
            //Assert.AreEqual("Operative procedure on fingers", fhirProcedure.Code.Coding[0].Display);
            Assert.AreEqual("215000", fhirProcedure.Code.Coding[0].Code);
            Assert.IsTrue(fhirProcedure.Code.Coding[0].System.EndsWith("Snomed/SnomedProcedures"));
            Assert.AreEqual("Jan2008", fhirProcedure.Code.Coding[0].Version);
            
            //Status - Since HV does not know which of the status values correctly applies here
            Assert.AreEqual(EventStatus.Unknown, fhirProcedure.Status);

            //Performed
            Assert.IsInstanceOfType(fhirProcedure.Performed, typeof(FhirDateTime));
            Assert.AreEqual(now.ToDateTimeUnspecified(), ((FhirDateTime)fhirProcedure.Performed).ToDateTimeOffset());

            //Performer
            Assert.IsFalse(fhirProcedure.Performer.IsNullOrEmpty());
            Assert.IsFalse(fhirProcedure.Contained.IsNullOrEmpty());            
            Assert.AreEqual(2, fhirProcedure.Performer.Count);
            Assert.AreEqual(2, fhirProcedure.Contained.Where(resource=>resource.GetType().Equals(typeof(Practitioner)) && resource.Id.StartsWith("#practitioner-")).Count());
            Assert.AreEqual(2, fhirProcedure.Performer.Where(performer => performer.Actor.Reference.StartsWith("#practitioner-")).Count());            
            
            //BodySite
            Assert.IsFalse(fhirProcedure.BodySite.IsNullOrEmpty());            
            Assert.AreEqual("Metacarpophalangeal joint structure of index finger", fhirProcedure.BodySite[0].Text);
            Assert.IsNotNull(fhirProcedure.BodySite[0].Coding.FirstOrDefault());
            //Assert.AreEqual("Metacarpophalangeal joint structure of index finger", fhirProcedure.BodySite[0].Coding[0].Display);
            Assert.AreEqual("289002", fhirProcedure.BodySite[0].Coding[0].Code);
            Assert.IsTrue(fhirProcedure.BodySite[0].Coding[0].System.EndsWith("Snomed/SnomedBodyLocation"));
            Assert.AreEqual("Jan2008", fhirProcedure.BodySite[0].Coding[0].Version);
        }
    }
}
