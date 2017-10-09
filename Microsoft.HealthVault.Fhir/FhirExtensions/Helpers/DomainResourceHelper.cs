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

namespace Microsoft.HealthVault.Fhir.FhirExtensions.Helpers
{
    public static class DomainResourceHelper
    {
        /// <summary>
        /// Returns a reference to a contained resource
        /// </summary>
        /// <see cref="http://hl7.org/fhir/references.html#contained"/>
        /// <param name="resource">Contained resource with an id</param>
        /// <returns>Reference to contained resource</returns>
        public static ResourceReference GetContainerReference(this Resource resource)
        {
            var id = resource.Id;
            return new ResourceReference($"#{id}");
        }

        /// <summary>
        /// Returns contained resource from given domain resource and reference
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="domainResource">Container domain resource</param>
        /// <param name="reference">Reference to contained resource</param>
        /// <returns>Contained resource</returns>
        /// <exception cref="NotImplementedException">Thrown when reference is not internal/contained</exception>
        public static T GetContainedResource<T>(this DomainResource domainResource, ResourceReference reference) where T : Resource
        {
            if (reference.IsContainedReference)
            {
                return domainResource.Contained.FirstOrDefault(resource
                    => reference.Matches(resource.GetContainerReference())) as T;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns referenced resource from given domain resource and reference.
        /// The resolver is called when reference is not internal/contained.
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="domainResource">Container domain resource</param>
        /// <param name="reference">Reference to contained resource</param>
        /// <param name="resolver">A function which returns a resource from external reference
        /// (possibly temperory dummy resource or resource resolved using external endpoints)</param>
        /// <returns>Referenced resource</returns>
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
    }
}
