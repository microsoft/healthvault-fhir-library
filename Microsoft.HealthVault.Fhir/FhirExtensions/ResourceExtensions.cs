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
