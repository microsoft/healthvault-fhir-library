// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;

namespace Microsoft.HealthVault.Fhir.FhirExtensions.Helpers
{
    public static class DomainResourceHelper
    {
        public static ResourceReference GetContainerReference(this Resource resource)
        {
            var id = resource.Id;
            return new ResourceReference($"#{id}");
        }

        public static T GetContainedResource<T>(this DomainResource domainResource, ResourceReference reference) where T : Resource
        {
            if (reference.IsContainedReference)
            {
                return domainResource.Contained.FirstOrDefault(resource
                    => reference.Matches(resource.GetContainerReference())) as T;
            }
            throw new NotImplementedException();
        }

        public static T GetReferencedResource<T>(this DomainResource domainResource,
            ResourceReference reference, Func<ResourceReference, T> resolver) where T : Resource
        {
            if (reference.IsContainedReference)
            {
                return GetContainedResource<T>(domainResource, reference);
            }
            else
            {
                return resolver(reference);
            }
        }

        internal static T GetElementAsT<T, TElement>(this DomainResource domainResource,
            Func<DomainResource, Element> propertyMapper, IEnumerable<Func<TElement, T>> convertors)
            where T : DomainResource
            where TElement : Element
        {
            var prop = propertyMapper(domainResource);
            if (prop == null)
            {
                return null;
            }

            foreach (var convertor in convertors)
            {
                if (prop is TElement)
                {
                    return convertor(prop as TElement);
                }
            }

            throw new ArgumentOutOfRangeException();
        }
    }

    public static class MedicationHelper
    {
        public static Medication ConvertTo(CodeableConcept medicationCodeableConcept)
        {
            return new Medication
            {
                Code = medicationCodeableConcept
            };
        }
    }

    public static class MedicationStatementHelper
    {
        public static Medication ExtractEmbeddedMedication(this MedicationStatement medicationStatement)
        {
            Element medication = medicationStatement.Medication;
            return GetMedication(medicationStatement, medication);
        }

        private static Medication GetMedication(DomainResource medicationStatement, Element medication)
        {
            switch (medication)
            {
                case ResourceReference medicationReference:
                    return medicationStatement.GetContainedResource<Medication>(medicationReference);
                case CodeableConcept medicationCodeableConcept:
                    return MedicationHelper.ConvertTo(medicationCodeableConcept);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public static class MedicationRequestHelper
    {
        public static Medication ExtractEmbeddedMedication(this MedicationRequest medicationRequest)
        {
            Element medication = medicationRequest.Medication;
            switch (medication)
            {
                case ResourceReference medicationReference:
                    if (!medicationReference.IsContainedReference)
                    {
                        throw new NotImplementedException();
                    }
                    return medicationRequest.Contained.FirstOrDefault(domainResource
                             => medicationReference.Matches(domainResource.GetContainerReference())) as Medication ?? new Medication();
                case CodeableConcept medicationCodeableConcept:
                    return new Medication
                    {
                        Code = medicationCodeableConcept
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
