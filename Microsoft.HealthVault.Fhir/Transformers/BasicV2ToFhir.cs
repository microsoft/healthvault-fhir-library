// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Patient ToFhir(this ItemTypes.BasicV2 basic)
        {
            return BasicV2ToFhir.ToFhirInternal(basic, ThingBaseToFhir.ToFhirInternal<Patient>(basic));
        }

        // Register the type on the generic ThingToFhir partial class
        public static Patient ToFhir(this ItemTypes.BasicV2 basic, Patient patient)
        {
            return BasicV2ToFhir.ToFhirInternal(basic, patient);
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault basic data types into FHIR Patient
    /// </summary>
    internal static class BasicV2ToFhir
    {
        internal static Patient ToFhirInternal(ItemTypes.BasicV2 basic, Patient patient)
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
                patient.Extension.Add(
                    new Extension
                    {
                        Url = HealthVaultExtensions.PatientBirthYear,
                        Value = new FhirDecimal(basic.BirthYear)
                    }
                );
            }

            if (basic.FirstDayOfWeek.HasValue)
            {
                patient.Extension.Add(
                    new Extension
                    {
                        Url = HealthVaultExtensions.FirstDayOfWeek,
                        Value = new Coding
                        {
                            Code = ((int)basic.FirstDayOfWeek).ToString(),
                            Display = basic.FirstDayOfWeek.Value.ToString()
                        }
                    }
                );
            }

            if (basic.Languages != null && basic.Languages.Count > 0)
            {
                foreach (var language in basic.Languages)
                {
                    patient.Communication.Add(
                        new Patient.CommunicationComponent
                        {
                            Language = new CodeableConcept
                            {
                                Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(language.SpokenLanguage, null)
                            },
                            Preferred = language.IsPrimary
                        }
                    );
                }
            }
            
            var basicAddress = new Extension
            {
                Url = HealthVaultExtensions.PatientBasicAddress
            };

            basicAddress.Extension.Add(new Extension(HealthVaultExtensions.PatientBasicAddressCity, new FhirString(basic.City)));
            basicAddress.Extension.Add(new Extension(HealthVaultExtensions.PatientBasicAddressState, basic.StateOrProvince.ToFhir()));
            basicAddress.Extension.Add(new Extension(HealthVaultExtensions.PatientBasicAddressPostalCode, new FhirString(basic.PostalCode)));
            basicAddress.Extension.Add(new Extension(HealthVaultExtensions.PatientBasicAddressCountry, basic.Country.ToFhir()));
            patient.Extension.Add(basicAddress);

            return patient;
        }
    }
}
