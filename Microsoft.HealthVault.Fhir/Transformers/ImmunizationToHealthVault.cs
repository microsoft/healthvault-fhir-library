// Copyright (c) Get Real Health.  All rights reserved.
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
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class ImmunizationToHealthVault
    {
        public static ItemTypes.Immunization ToHealthVault(this Hl7.Fhir.Model.Immunization fhirImmunization)
        {
            ItemTypes.Immunization hvImmunization = fhirImmunization.ToThingBase<ItemTypes.Immunization>();

            hvImmunization.Name = fhirImmunization.VaccineCode.ToCodableValue();

            if (!fhirImmunization.DateElement.IsNullOrEmpty())
            {
                hvImmunization.DateAdministrated = fhirImmunization.DateElement.ToAproximateDateTime();
            }            

            if (!fhirImmunization.Practitioner.IsNullOrEmpty())
            {
                hvImmunization.Administrator = GetProvider(fhirImmunization);
            }

            if (!fhirImmunization.Manufacturer.IsNullOrEmpty())
            {
                hvImmunization.Manufacturer = GetManufacturer(fhirImmunization);
            }

            hvImmunization.Lot = fhirImmunization.LotNumber;
            hvImmunization.Route = fhirImmunization.Route.ToCodableValue();
            if (fhirImmunization.ExpirationDateElement != null)
            {
                hvImmunization.ExpirationDate = fhirImmunization.ExpirationDateElement.ToAproximateDateTime().ApproximateDate;
            }
            hvImmunization.AnatomicSurface = fhirImmunization.Site.ToCodableValue();

            Extension immunizationExtension = fhirImmunization.GetExtension(HealthVaultExtensions.ImmunizationDetail);
            if (immunizationExtension != null)
            {
                hvImmunization.Sequence = immunizationExtension.GetStringExtension(HealthVaultExtensions.ImmunizationDetailSequence);
                hvImmunization.Consent = immunizationExtension.GetStringExtension(HealthVaultExtensions.ImmunizationDetailConcent);
                hvImmunization.AdverseEvent = immunizationExtension.GetStringExtension(HealthVaultExtensions.ImmunizationDetailAdverseEvent);
            }

            if (!fhirImmunization.Note.IsNullOrEmpty())
            {
                fhirImmunization.Note.ForEach(note =>
                {
                    string separator = string.Empty;
                    if (!string.IsNullOrEmpty(hvImmunization.CommonData.Note))
                    {
                        separator = Environment.NewLine; //Let's separate each note with new line
                    }

                    hvImmunization.CommonData.Note += $"{separator}{note.Text}";
                });
            }

            return hvImmunization;
        }

        private static CodableValue GetManufacturer(Hl7.Fhir.Model.Immunization fhirImmunization)
        {
            var manufacturerComponent = fhirImmunization.Manufacturer;
            if (manufacturerComponent.IsContainedReference)
            {
                var containedReference = fhirImmunization.Contained.SingleOrDefault(resouce =>
                    resouce.Id.Equals(manufacturerComponent.Reference) && resouce.GetType().Equals(typeof(Hl7.Fhir.Model.Organization)));

                if (containedReference == null)
                {
                    return null;
                }

                return new CodableValue((containedReference as Hl7.Fhir.Model.Organization).Name);
            }

            if (string.IsNullOrEmpty(manufacturerComponent.Display))
            {
                return null;
            }

            return new CodableValue(manufacturerComponent.Display);
        }

        private static PersonItem GetProvider(Hl7.Fhir.Model.Immunization fhirImmunization)
        {
            var practitionerComponent = fhirImmunization.Practitioner.First(); //Let's take only the first one
            if (practitionerComponent.Actor.IsContainedReference)
            {
                var containedReference = fhirImmunization.Contained.SingleOrDefault(resouce =>
                    resouce.Id.Equals(practitionerComponent.Actor.Reference) && resouce.GetType().Equals(typeof(Practitioner)));

                if (containedReference == null)
                {
                    return null;
                }

                return (containedReference as Practitioner).ToHealthVault();
            }

            if (string.IsNullOrEmpty(practitionerComponent.Actor.Display))
            {
                return null;
            }

            return new PersonItem()
            {
                Name = new Name(practitionerComponent.Actor.Display)
            };
        }
    }
}

