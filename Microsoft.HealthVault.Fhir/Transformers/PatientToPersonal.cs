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
using Microsoft.HealthVault.ItemTypes;
using NodaTime;
using NodaTime.Text;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class PatientToPersonal
    {
        internal static Personal ToPersonal(this Patient patient)
        {
            var personal = patient.ToThingBase<ItemTypes.Personal>();
            
            if (patient.BirthDateElement != null)
            {
                personal.BirthDate = new HealthServiceDateTime(LocalDateTime.FromDateTime(patient.BirthDateElement.ToDateTime().Value));
            }

            if (patient.Extension.Any(x => x.Url == "patient-birth-time"))
            {
                personal.BirthDate.Time = ((Time)patient.Extension.First(x => x.Url == "patient-birth-time").Value).ToAppoximateTime();
            }

            if (patient.Deceased != null)
            {
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

            if (patient.Extension.Any(x => x.Url == "patient-blood-type"))
            {
                personal.BloodType = ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-blood-type").Value).ToCodeableConcept();
            }

            if(patient.Extension.Any(x => x.Url == "patient-employment-status"))
            {
                personal.OrganDonor = ((FhirString)patient.Extension.First(x => x.Url == "patient-employment-status").Value).Value;
            }

            if (patient.Extension.Any(x => x.Url == "patient-ethnicity"))
            {
                personal.Ethnicity = ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-ethnicity").Value).ToCodeableConcept();
            }

            if (patient.Extension.Any(x => x.Url == "patient-highest-education-level"))
            {
                personal.HighestEducationLevel = ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-highest-education-level").Value).ToCodeableConcept();
            }

            if (patient.Extension.Any(x => x.Url == "patient-is-disabled"))
            {
                personal.IsVeteran = ((FhirBoolean)patient.Extension.First(x => x.Url == "patient-is-disabled").Value).Value;
            }

            if (patient.Extension.Any(x => x.Url == "patient-is-veteran"))
            {
                personal.IsVeteran = ((FhirBoolean)patient.Extension.First(x => x.Url == "patient-is-veteran").Value).Value;
            }

            if (patient.Extension.Any(x => x.Url == "patient-marital-status"))
            {
                personal.MaritalStatus = ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-marital-status").Value).ToCodeableConcept();
            }

            if (patient.Extension.Any(x => x.Url == "patient-organ-donor"))
            {
                personal.OrganDonor = ((FhirString)patient.Extension.First(x => x.Url == "patient-organ-donor").Value).Value;
            }

            if (patient.Extension.Any(x => x.Url == "patient-religion"))
            {
                personal.Religion = ((CodeableConcept)patient.Extension.First(x => x.Url == "patient-religion").Value).ToCodeableConcept();
            }

            if (!patient.Name.IsNullOrEmpty())
            {
                var patientName = patient.Name[0];
                var name = new Name();

                //todo: figure out how to extend the names so we can be sure to map first and middle correctly
                if (patientName.Given.Any())
                {
                    name.First = patientName.Given.ElementAt(0);

                    if (patientName.Given.Count() > 1)
                    {
                        name.Middle = patientName.Given.ElementAt(1);
                    }
                }

                name.Last = patientName.Family;

                name.Suffix = ((CodeableConcept)patientName.Extension.FirstOrDefault(x => x.Url == "patient-suffix").Value).ToCodeableConcept();
                name.Title = ((CodeableConcept)patientName.Extension.FirstOrDefault(x => x.Url == "patient-title").Value).ToCodeableConcept();

                personal.Name = name;
            }

            if (!patient.Identifier.IsNullOrEmpty())
            {
                personal.SocialSecurityNumber = patient.Identifier.First(x => x.System == "http://hl7.org/fhir/sid/us-ssn").Value;
            }

            return personal;
        }

        private static CodableValue ToCodeableConcept(this CodeableConcept codeableConcept)
        {
            var code = codeableConcept.Coding[0];
            var value = code.Code.Split(':');
            var vocabName = value[0];
            var vocabCode = value.Length == 2 ? value[1] : null;

            var codableValue = new CodableValue(
                codeableConcept.Text,
                vocabCode,
                vocabName,
                code.System,
                code.Version
            );

            return codableValue;
        }
    }
}
