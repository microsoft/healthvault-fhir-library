// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class ResourceExtensions
    {
        public static void SetIdentity(this Resource resource, ThingKey key)
        {
            if (key != null)
            {
                resource.SetId(key.Id);
                resource.SetVersionStamp(key.VersionStamp);
            }
        }

        private static void SetVersionStamp(this Resource resource, Guid versionStamp)
        {
            if (versionStamp != Guid.Empty)
            {
                resource.CreateMetaIfNeeded();
                resource.Meta.VersionId = versionStamp.ToString();
            }
        }

        private static void SetId(this Resource resource, Guid id)
        {
            if (id != Guid.Empty)
            {
                resource.Id = id.ToString();
            }
        }

        public static void CopyLastUpdatedFromAudit(this Resource resource, HealthServiceAudit lastUpdated)
        {
            if (lastUpdated != null)
            {
                resource.CreateMetaIfNeeded();
                resource.Meta.LastUpdated = lastUpdated.Timestamp.ToDateTimeOffset();
            }
        }

        private static void CreateMetaIfNeeded(this Resource resource)
        {
            if (resource.Meta == null)
            {
                resource.Meta = new Meta();
            }
        }
    }
}
