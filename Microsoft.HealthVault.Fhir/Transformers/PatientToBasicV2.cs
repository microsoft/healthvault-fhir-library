// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class PatientToBasicV2
    {
        internal static BasicV2 ToBasicV2(this Patient patient)
        {
            var hasValue = false;
            var basicV2 = patient.ToThingBase<BasicV2>();

            if (patient.Gender.HasValue)
            {
                hasValue = true;
                switch (patient.Gender)
                {
                    case AdministrativeGender.Female:
                        basicV2.Gender = Gender.Female;
                        break;
                    case AdministrativeGender.Male:
                        basicV2.Gender = Gender.Male;
                        break;
                    default:
                        basicV2.Gender = Gender.Unknown;
                        break;
                }
            }

            var basicV2Extension = patient.GetExtension(HealthVaultExtensions.PatientBasicV2);

            if (basicV2Extension != null)
            {
                basicV2.BirthYear = basicV2Extension.GetIntegerExtension(HealthVaultExtensions.PatientBirthYear);
                hasValue = basicV2.BirthYear.HasValue || hasValue;

                var stringDayOfWeek = basicV2Extension.GetExtensionValue<Coding>(HealthVaultExtensions.PatientFirstDayOfWeek)?.Display;
                if (Enum.TryParse(stringDayOfWeek, out DayOfWeek dayOfWeek))
                {
                    hasValue = true;
                    basicV2.FirstDayOfWeek = dayOfWeek;
                }

                var basicAddress = basicV2Extension.GetExtension(HealthVaultExtensions.PatientBasicAddress);
                if (basicAddress != null)
                {
                    hasValue = true;

                    basicV2.City = basicAddress.GetStringExtension(HealthVaultExtensions.PatientBasicAddressCity);
                    basicV2.StateOrProvince = basicAddress.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientBasicAddressState)?.ToCodableValue();
                    basicV2.PostalCode = basicAddress.GetStringExtension(HealthVaultExtensions.PatientBasicAddressPostalCode);
                    basicV2.Country = basicAddress.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientBasicAddressCountry)?.ToCodableValue();
                }
            }

            if (!patient.Communication.IsNullOrEmpty())
            {
                hasValue = true;
                foreach (var communication in patient.Communication)
                {
                    var language = new Language
                    {
                        SpokenLanguage = communication.Language.ToCodableValue()
                    };
                    
                    if (communication.Preferred.HasValue)
                    {
                        language.IsPrimary = communication.Preferred.Value;
                    }

                    basicV2.Languages.Add(language);
                }
            }

            return hasValue ? basicV2 : null;
        }
    }
}
