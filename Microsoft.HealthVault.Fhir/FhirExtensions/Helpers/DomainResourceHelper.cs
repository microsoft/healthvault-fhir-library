﻿// Copyright (c) Get Real Health.  All rights reserved.
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
    }
}
