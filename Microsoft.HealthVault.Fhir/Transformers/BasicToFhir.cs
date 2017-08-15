using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Patient ToFhir(this ItemTypes.Basic basic)
        {
            return BasicToFhir.ToFhirInternal(basic, ThingBaseToFhir.ToFhirInternal<Patient>(basic));
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault basic data types into FHIR Observations
    /// </summary>
    internal static class BasicToFhir
    {
        internal static Patient ToFhirInternal(ItemTypes.Basic basic, Patient patient)
        {
            if (basic.Gender.HasValue)
            {
                switch (basic.Gender.Value)
                {
                    case Gender.Male:
                        patient.Gender = AdministrativeGender.Male;
                        break;
                    case Gender.Female:
                        patient.Gender = AdministrativeGender.Female;
                        break;
                    case Gender.Unknown:
                        patient.Gender = AdministrativeGender.Unknown;
                        break;
                }
            }

            if (basic.BirthYear.HasValue)
            {
                patient.Extension.Add(new Extension
                {
                    Url = "https://healthvault.com/extensions/birth-year",
                    Value = new FhirDecimal(basic.BirthYear)
                });
            }

            if (basic.FirstDayOfWeek.HasValue)
            {
                patient.Extension.Add(new Extension
                {
                    Url = "https://healthvault.com/extensions/first-day-of-week",
                    Value = new Coding
                    {
                        Code = ((int)basic.FirstDayOfWeek).ToString(),
                        Display = basic.FirstDayOfWeek.Value.ToString()
                    }
                });
            }

            if (basic.Languages != null && basic.Languages.Count > 0)
            {
                foreach (var language in basic.Languages)
                {
                    patient.Communication.Add(new Patient.CommunicationComponent
                    {
                        Language = new CodeableConcept
                        {
                            Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(language.SpokenLanguage, null)
                        },
                        Preferred = language.IsPrimary
                    });
                }
            }

            patient.Address.Add(new Hl7.Fhir.Model.Address
            {
                City = basic.City,
                State = basic.StateOrProvince,
                PostalCode = basic.PostalCode,
                Country = basic.Country
            });

            return patient;
        }
    }
}
