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
using Microsoft.HealthVault.Fhir.Codings;
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

            var personalExtension = patient.GetExtension(HealthVaultExtensions.PatientPersonal);
            if (personalExtension != null)
            {
                personal.BloodType = personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientBloodType)?.ToCodableValue();
                personal.Ethnicity = personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientEthnicity)?.ToCodableValue();
                personal.HighestEducationLevel = personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientHighestEducationLevel)?.ToCodableValue();
                personal.IsDisabled = personalExtension.GetBoolExtension(HealthVaultExtensions.PatientIsDisabled);
                personal.MaritalStatus = personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientMaritalStatus)?.ToCodableValue();
                personal.IsVeteran = personalExtension.GetBoolExtension(HealthVaultExtensions.PatientIsVeteran);
                personal.OrganDonor = personalExtension.GetStringExtension(HealthVaultExtensions.PatientOrganDonor);
                personal.EmploymentStatus = personalExtension.GetStringExtension(HealthVaultExtensions.PatientEmploymentStatus);
                personal.Religion = personalExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.PatientReligion)?.ToCodableValue();
                personal.SocialSecurityNumber = patient.Identifier.FirstOrDefault(x => x.System == Constants.FhirExtensions.SSN)?.Value;

                if (personal.BloodType != null ||
                    personal.Ethnicity != null ||
                    personal.HighestEducationLevel != null ||
                    personal.IsDisabled != null ||
                    personal.MaritalStatus != null ||
                    personal.IsVeteran != null ||
                    !string.IsNullOrEmpty(personal.OrganDonor) ||
                    !string.IsNullOrEmpty(personal.EmploymentStatus) ||
                    personal.Religion != null ||
                    personal.SocialSecurityNumber != null)
                {
                    hasValue = true;
                }
            }

            if (patient.BirthDateElement != null)
            {
                hasValue = true;
                personal.BirthDate = new HealthServiceDateTime(LocalDateTime.FromDateTime(patient.BirthDateElement.ToDateTime().Value))
                {
                    Time = patient.BirthDateElement.GetExtensionValue<Time>(HealthVaultExtensions.PatientBirthTime)?.ToAppoximateTime()
                };
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
                var patientName = patient.Name.First(); // Take the first name available
                var name = new Name
                {
                    Full = patientName.Text,
                    Last = patientName.Family,
                    Suffix = patientName.Suffix.Any() ? new CodableValue(patientName.Suffix.First()) : null, // Take the first suffix if there are any
                    Title = patientName.Prefix.Any() ? new CodableValue(patientName.Prefix.First()) : null, // Take the first prefix if there are any
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
