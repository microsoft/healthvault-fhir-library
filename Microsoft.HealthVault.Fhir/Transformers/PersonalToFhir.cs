// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        public static Patient ToFhir(this Personal personal)
        {
            return PersonalToFhir.ToFhirInternal(personal, ThingBaseToFhir.ToFhirInternal<Patient>(personal));
        }

        // Register the type on the generic ThingToFhir partial class
        public static Patient ToFhir(this Personal personal, Patient patient)
        {
            return PersonalToFhir.ToFhirInternal(personal, patient);
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault personal data types into FHIR Patient
    /// </summary>
    internal static class PersonalToFhir
    {
        internal static Patient ToFhirInternal(Personal personal, Patient patient)
        {
            if (personal.BirthDate?.Date != null)
            {
                patient.BirthDateElement = new Date(personal.BirthDate.Date.Year, personal.BirthDate.Date.Month, personal.BirthDate.Date.Day);
            }

            if (personal.BirthDate?.Time != null)
            {
                patient.Extension.Add(new Extension("patient-birth-time", new Time(personal.BirthDate.Time.ToString())));
            }

            if (personal.DateOfDeath != null)
            {
                patient.Deceased = personal.DateOfDeath.ToFhir();
            }
            else
            {
                patient.Deceased = new FhirBoolean(personal.IsDeceased);
            }

            if (personal.BloodType != null)
            {
                patient.Extension.Add(new Extension(
                    "patient-blood-type",
                    new CodeableConcept{Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.BloodType, null)
                    }));
            }

            if (!string.IsNullOrEmpty(personal.EmploymentStatus))
            {
                patient.Extension.Add(new Extension("patient-employment-status", new FhirString(personal.EmploymentStatus)));
            }

            if (personal.Ethnicity != null)
            {
                patient.Extension.Add(new Extension(
                    "patient-ethnicity",
                    new CodeableConcept
                    {
                        Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.Ethnicity, null)
                    }));
            }

            if (personal.HighestEducationLevel != null)
            {
                patient.Extension.Add(new Extension(
                    "patient-highest-education-level",
                    new CodeableConcept
                    {
                        Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.HighestEducationLevel, null)
                    }));
            }

            if (personal.IsDisabled.HasValue)
            {
                patient.Extension.Add(new Extension("patient-is-disabled", new FhirBoolean(personal.IsDisabled)));
            }

            if (personal.IsVeteran.HasValue)
            {
                patient.Extension.Add(new Extension("patient-is-veteran", new FhirBoolean(personal.IsVeteran)));
            }

            if (personal.MaritalStatus != null)
            {
                patient.Extension.Add(new Extension(
                    "patient-marital-status",
                    new CodeableConcept
                    {
                        Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.MaritalStatus, null)
                    }));
            }

            if (personal.Name != null)
            {
                var humanName = new HumanName
                {
                    Family = personal.Name.Last,
                    Text = personal.Name.Full,
                };

                var givenNames = new List<string>();
                if (!string.IsNullOrEmpty(personal.Name.First))
                {
                    givenNames.Add(personal.Name.First);
                }
                if (!string.IsNullOrEmpty(personal.Name.Middle))
                {
                    givenNames.Add(personal.Name.Middle);
                }
                humanName.Given = givenNames;

                patient.Name.Add(humanName);

                if (personal.Name.Title != null)
                {
                    patient.Extension.Add(new Extension(
                        "patient-title",
                        new CodeableConcept
                        {
                            Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.Name.Title, null)
                        }));
                }

                if (personal.Name.Suffix != null)
                {
                    patient.Extension.Add(new Extension(
                        "patient-suffix",
                        new CodeableConcept
                        {
                            Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.Name.Suffix, null)
                        }));
                }
            }

            if (!string.IsNullOrEmpty(personal.OrganDonor))
            {
                patient.Extension.Add(new Extension("patient-organ-donor", new FhirString(personal.OrganDonor)));
            }

            if (personal.Religion != null)
            {
                patient.Extension.Add(new Extension(
                    "patient-religion",
                    new CodeableConcept
                    {
                        Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(personal.Religion, null)
                    }));
            }

            if (!string.IsNullOrEmpty(personal.SocialSecurityNumber))
            {
                patient.Identifier.Add(new Identifier
                {
                    Value = personal.SocialSecurityNumber,
                    System = "http://hl7.org/fhir/sid/us-ssn",
                });
            }
            return patient;
        }
    }
}
