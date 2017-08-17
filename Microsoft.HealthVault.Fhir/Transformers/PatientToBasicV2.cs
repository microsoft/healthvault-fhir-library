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
using Hl7.Fhir.Support;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class PatientToBasicV2
    {
        internal static BasicV2 ToBasicV2(this Patient patient)
        {
            var basicV2 = patient.ToThingBase<ItemTypes.BasicV2>();

            if (patient.Gender.HasValue)
            {
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

            if (patient.Extension.Any(x => x.Url == "https://healthvault.com/extensions/birth-year"))
            {
                basicV2.BirthYear = (int?)((FhirDecimal)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/birth-year").Value).Value;
            }

            if (patient.Extension.Any(x => x.Url == "https://healthvault.com/extensions/first-day-of-week"))
            {
                DayOfWeek dayOfWeek;
                var stringDayOfWeek = ((Coding)patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/first-day-of-week").Value).Display;

                if (Enum.TryParse(stringDayOfWeek, out dayOfWeek))
                {
                    basicV2.FirstDayOfWeek = dayOfWeek;
                }
            }

            if (patient.Extension.Any(x => x.Url == "https://healthvault.com/extensions/basic-address"))
            {
                var basicAddress = patient.Extension.First(x => x.Url == "https://healthvault.com/extensions/basic-address");


                // todo: implement this
                //basicV2.City = ((FhirString)basicAddress.Extension.First(c => c.Url == "basic-address-city")?.Value)?.Value;
                //basicV2.PostalCode = ((FhirString)basicAddress.Extension.First(c => c.Url == "basic-address-postalCode")?.Value)?.Value;
            }

            if (!patient.Communication.IsNullOrEmpty())
            {
                foreach (var communication in patient.Communication)
                {
                    var language = new Language();

                    var code = communication.Language.Coding[0];
                    var value = code.Code.Split(':');
                    var vocabName = value[0];
                    var vocabCode = value.Length == 2 ? value[1] : null;

                    language.SpokenLanguage = new CodableValue(
                        communication.Language.Text,
                        vocabCode,
                        vocabName,
                        code.System,
                        code.Version
                        );

                    if (communication.Preferred.HasValue)
                    {
                        language.IsPrimary = communication.Preferred.Value;
                    }

                    basicV2.Languages.Add(language);
                }
            }

            return basicV2;
        }
    }
}
