// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using NodaTime;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class PatientToPersonal
    {
        internal static Personal ToPersonal(this Patient patient)
        {
            var personal = patient.ToThingBase<Personal>();
            var hasValue = false;

            personal.BloodType = ((CodeableConcept)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientBloodType)?.Value)?.ToCodableValue();
            personal.Ethnicity = ((CodeableConcept)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientEthnicity)?.Value)?.ToCodableValue();
            personal.HighestEducationLevel = ((CodeableConcept)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientHighestEducationLevel)?.Value)?.ToCodableValue();
            personal.IsDisabled = ((FhirBoolean)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientIsDisabled)?.Value)?.Value;
            personal.MaritalStatus = ((CodeableConcept)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientMaritalStatus)?.Value)?.ToCodableValue();
            personal.IsVeteran = ((FhirBoolean)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientIsVeteran)?.Value)?.Value;
            personal.OrganDonor = ((FhirString)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientOrganDonor)?.Value)?.Value;
            personal.EmploymentStatus = ((FhirString)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientEmploymentStatus)?.Value)?.Value;
            personal.Religion = ((CodeableConcept)patient.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientReligion)?.Value)?.ToCodableValue();
            personal.SocialSecurityNumber = patient.Identifier.FirstOrDefault(x => x.System == Constants.FhirExtensions.SSN)?.Value;

            if (personal.BloodType != null ||
                personal.Ethnicity != null ||
                personal.HighestEducationLevel != null ||
                personal.IsDisabled != null ||
                personal.MaritalStatus != null ||
                personal.IsVeteran != null ||
                personal.OrganDonor != null ||
                personal.EmploymentStatus != null ||
                personal.Religion != null ||
                personal.SocialSecurityNumber != null)
            {
                hasValue = true;
            }

            if (patient.BirthDateElement != null)
            {
                hasValue = true;
                personal.BirthDate = new HealthServiceDateTime(LocalDateTime.FromDateTime(patient.BirthDateElement.ToDateTime().Value));
                
                if (patient.BirthDateElement.Extension.Any(x => x.Url == HealthVaultExtensions.PatientBirthTime))
                {
                    personal.BirthDate.Time = ((Time)patient.BirthDateElement.Extension.First(x => x.Url == HealthVaultExtensions.PatientBirthTime).Value).ToAppoximateTime();
                }
            }

            if (patient.Deceased != null)
            {
                hasValue = true;
                switch (patient.Deceased)
                {
                    case FhirBoolean b:
                        personal.IsDeceased = b.Value;
                        break;
                    case FhirDateTime d:
                        personal.IsDeceased = true;
                        var dt = d.ToDateTimeOffset();
                        personal.DateOfDeath = new ApproximateDateTime(
                            new ApproximateDate(dt.Year, dt.Month, dt.Day),
                            new ApproximateTime(dt.Hour, dt.Minute, dt.Second, dt.Millisecond));
                        break;
                }
            }

            if (!patient.Name.IsNullOrEmpty())
            {
                hasValue = true;
                var patientName = patient.Name.First();
                var name = new Name
                {
                    Full = patientName.Text,
                    Last = patientName.Family,
                    Suffix = ((CodeableConcept)patientName.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientSuffix)?.Value)?.ToCodableValue(),
                    Title = ((CodeableConcept)patientName.Extension.FirstOrDefault(x => x.Url == HealthVaultExtensions.PatientTitle)?.Value)?.ToCodableValue(),
                };

                //todo: figure out how to extend the names so we can be sure to map first and middle correctly
                if (patientName.Given.Any())
                {
                    name.First = patientName.Given.ElementAt(0);

                    if (patientName.Given.Count() > 1)
                    {
                        name.Middle = patientName.Given.ElementAt(1);
                    }
                }

                personal.Name = name;
            }

            return hasValue ? personal : null;
        }
    }
}
