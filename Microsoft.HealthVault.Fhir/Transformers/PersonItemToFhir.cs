// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        public static Practitioner ToFhir(this PersonItem person)
        {
            return PersonItemToFhir.ToFhirInternal(person);
        }
    }

    public class PersonItemToFhir
    {
        internal static Practitioner ToFhirInternal(PersonItem person)
        {
            var practitioner = new Practitioner();

            HumanName fhirName = person.Name.ToFhir();
            practitioner.Name = new System.Collections.Generic.List<HumanName> { fhirName };

            if (!string.IsNullOrEmpty(person.Organization))
            {
                var fhirAddress = new Hl7.Fhir.Model.Address
                {
                    Text = person.Organization,
                    Use = Hl7.Fhir.Model.Address.AddressUse.Work
                };
                practitioner.Address.Add(fhirAddress);
            }

            if (!string.IsNullOrEmpty(person.ProfessionalTraining))
            {
                var qualificationComponent = new Practitioner.QualificationComponent
                {
                    Code = new CodeableConcept
                    {
                        Text = person.ProfessionalTraining
                    }
                };
                practitioner.Qualification = new System.Collections.Generic.List<Practitioner.QualificationComponent>
                {
                    qualificationComponent
                };
            }

            if (!string.IsNullOrEmpty(person.PersonId))
            {
                practitioner.Identifier = new System.Collections.Generic.List<Identifier>
                {
                   new Identifier
                   {
                       Value = person.PersonId
                   }
                };
            }

            if (person.ContactInformation != null)
            {
                practitioner.Address.AddRange(
                    person.ContactInformation.Address.Select(address => address.ToFhir()));
                practitioner.Telecom.AddRange(
                    person.ContactInformation.Phone.Select(phone => phone.ToFhir()));
                practitioner.Telecom.AddRange(
                    person.ContactInformation.Email.Select(email => email.ToFhir()));
            }

            return practitioner;
        }
    }
}
