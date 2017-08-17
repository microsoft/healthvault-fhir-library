using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Address = Microsoft.HealthVault.ItemTypes.Address;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class MultipleToFhirPatientTests
    {
        [TestMethod]
        public void WhenMultipleHeathVaultThingsTransformedToFhirPatient_ThenCodeAndValuesEqual()
        {
            var basic = new ItemTypes.Basic
            {
                Gender = Gender.Female,
                BirthYear = 1975,
                City = "Redmond",
                StateOrProvince = "WA",
                PostalCode = "98052",
                Country = "USA",
                FirstDayOfWeek = DayOfWeek.Sunday,
                Languages =
                {
                    new Language(new CodableValue("English", "en", "iso639-1", "iso", "1"), true),
                    new Language(new CodableValue("French", "fr", "iso639-1", "iso", "1"), false),
                }
            };

            var patient = basic.ToFhir<Patient>();

            var contact = new Contact();
            contact.ContactInformation.Address.Add(new Address
            {
                Street = { "123 Main St.", "Apt. 3B" },
                City = "Redmond",
                PostalCode = "98052",
                County = "King",
                State = "WA",
                Country = "USA",
                Description = "Home address",
                IsPrimary = true,
            });

            contact.ContactInformation.Address.Add(new Address
            {
                Street = { "1 Back Lane" },
                City = "Holmfirth",
                PostalCode = "HD7 1HQ",
                County = "HUDDERSFIELD",
                Country = "UK",
                Description = "business address",
            });

            contact.ContactInformation.Email.Add(new Email
            {
                Address = "person1@example.com",
                Description = "Address 1",
                IsPrimary = true,
            });

            contact.ContactInformation.Email.Add(new Email
            {
                Address = "person2@example.com",
                Description = "Address 2",
            });

            contact.ContactInformation.Phone.Add(new Phone
            {
                Number = "1-425-555-0100",
                Description = "Phone 1",
                IsPrimary = true,
            });

            contact.ContactInformation.Phone.Add(new Phone
            {
                Number = "0491 570 156",
                Description = "Phone 2",
            });


            contact.ToFhir(patient);

            var personalImage = new PersonalImage();
            string resourceName = "Microsoft.HealthVault.Fhir.UnitTests.Samples.HealthVaultIcon.png";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        personalImage.WriteImage(reader.BaseStream, "image/png");
                    }
                }
            }

            personalImage.ToFhir(patient);

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

            personal.ToFhir(patient);

            Assert.IsNotNull(patient);
            // Basic Portion
            Assert.AreEqual(AdministrativeGender.Female, patient.Gender.Value);
            Assert.AreEqual(1975, ((FhirDecimal)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/birth-year").Value).Value);
            Assert.AreEqual("0", ((Coding)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/first-day-of-week").Value).Code);
            Assert.AreEqual("Sunday", ((Coding)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/first-day-of-week").Value).Display);

            Assert.AreEqual(3, patient.Address.Count);
            Assert.AreEqual("Redmond", patient.Address[0].City);
            Assert.AreEqual("WA", patient.Address[0].State);
            Assert.AreEqual("98052", patient.Address[0].PostalCode);
            Assert.AreEqual("USA", patient.Address[0].Country);

            Assert.AreEqual(2, patient.Communication.Count);
            Assert.AreEqual("English", patient.Communication[0].Language.Coding[0].Display);
            Assert.AreEqual(true, patient.Communication[0].Preferred);

            // Contact portion
            var address1 = patient.Address[1];
            Assert.AreEqual(2, address1.Line.Count());
            Assert.AreEqual("123 Main St.", address1.Line.First());
            Assert.AreEqual("Redmond", address1.City);
            Assert.AreEqual("King", address1.District);
            Assert.AreEqual("WA", address1.State);
            Assert.AreEqual("98052", address1.PostalCode);
            Assert.AreEqual("USA", address1.Country);
            Assert.AreEqual("Home address", address1.Text);
            Assert.AreEqual(true, ((FhirBoolean)address1.Extension.First(x => x.Url == "is-primary").Value).Value);

            Assert.AreEqual(4, patient.Telecom.Count);
            var email1 = patient.Telecom[0];
            Assert.AreEqual("person1@example.com", email1.Value);
            Assert.AreEqual("Address 1", ((FhirString)email1.Extension.First(x => x.Url == "description").Value).Value);
            Assert.AreEqual(1, email1.Rank);

            var phone1 = patient.Telecom[2];
            Assert.AreEqual("1-425-555-0100", phone1.Value);
            Assert.AreEqual("Phone 1", ((FhirString)phone1.Extension.First(x => x.Url == "description").Value).Value);
            Assert.AreEqual(1, phone1.Rank);

            // Personal Image portion
            Assert.IsNotNull(patient);
            Assert.AreEqual(1757, patient.Photo[0].Data.Length);

            // Personal portion
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
            Assert.AreEqual("blood-types:A+", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-blood-type").Value).Coding[0].Code);
            Assert.AreEqual("religion:Agn", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-religion").Value).Coding[0].Code);
            Assert.AreEqual("marital-status:NM", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-marital-status").Value).Coding[0].Code);
            Assert.AreEqual("ethnicity-types:8", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-ethnicity").Value).Coding[0].Code);
            Assert.AreEqual("Education-level:ColG", ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-highest-education-level").Value).Coding[0].Code);
            Assert.AreEqual("Employed", ((FhirString)patient.Extension.First(x => x.Url == "patient-employment-status").Value).Value);
            Assert.AreEqual("Organ Donor", ((FhirString)patient.Extension.First(x => x.Url == "patient-organ-donor").Value).Value);
        }
    }
}
