// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Fhir.Constants;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        public static Hl7.Fhir.Model.Immunization ToFhir(this ItemTypes.Immunization hvImmunization)
        {
            return ImmunizationToFhir.ToFhirInternal(hvImmunization, hvImmunization.ToFhirInternal<Hl7.Fhir.Model.Immunization>());
        }
    }
    internal static class ImmunizationToFhir
    {
        internal static Hl7.Fhir.Model.Immunization ToFhirInternal(ItemTypes.Immunization hvImmunization, Hl7.Fhir.Model.Immunization fhirImmunization)
        {
            fhirImmunization.VaccineCode = hvImmunization.Name.ToFhir();
            fhirImmunization.DateElement = hvImmunization.DateAdministrated.ToFhir();

            if (hvImmunization.Administrator != null)
            {
                fhirImmunization.Practitioner.Add(new Hl7.Fhir.Model.Immunization.PractitionerComponent
                {
                    Actor = AddContainedResource(fhirImmunization, hvImmunization.Administrator.ToFhir())
                });
            }

            if (hvImmunization.Manufacturer != null)
            {
                fhirImmunization.Manufacturer = AddContainedResource(fhirImmunization, new Hl7.Fhir.Model.Organization
                {
                    Name = hvImmunization.Manufacturer.Text
                });
            }

            fhirImmunization.LotNumber = hvImmunization.Lot;
            fhirImmunization.Route = hvImmunization.Route.ToFhir();
            fhirImmunization.ExpirationDateElement = hvImmunization.ExpirationDate.ToFhir();

            if (!string.IsNullOrEmpty(hvImmunization.Sequence)
                || !string.IsNullOrEmpty(hvImmunization.AdverseEvent)
                || !string.IsNullOrEmpty(hvImmunization.Consent))
            {
                var immunizationExtension = new Extension
                {
                    Url = HealthVaultExtensions.ImmunizationDetail
                };

                if (!string.IsNullOrEmpty(hvImmunization.Sequence))
                {
                    immunizationExtension.AddExtension(HealthVaultExtensions.ImmunizationDetailSequence, new FhirString(hvImmunization.Sequence));
                }
                if (!string.IsNullOrEmpty(hvImmunization.Consent))
                {
                    immunizationExtension.AddExtension(HealthVaultExtensions.ImmunizationDetailConcent, new FhirString(hvImmunization.Consent));
                }
                if (!string.IsNullOrEmpty(hvImmunization.AdverseEvent))
                {
                    immunizationExtension.AddExtension(HealthVaultExtensions.ImmunizationDetailAdverseEvent, new FhirString(hvImmunization.AdverseEvent));
                }
                fhirImmunization.Extension.Add(immunizationExtension);
            }

            fhirImmunization.Site = hvImmunization.AnatomicSurface.ToFhir();
            return fhirImmunization;
        }

        private static ResourceReference AddContainedResource(DomainResource domainResource, Resource resource, string id = null)
        {
            if (resource != null)
            {
                resource.Id = id ?? $"#{Guid.NewGuid()}";
                domainResource.Contained.Add(resource);
                return new ResourceReference
                {
                    Reference = resource.Id
                };
            }
            return null;
        }
    }
}
