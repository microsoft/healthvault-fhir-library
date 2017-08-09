using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class ResourceExtensions
    {
        public static void SetIdentity(this Resource observation, ThingKey key)
        {
            if (key != null)
            {
                observation._setId(key.Id);
                observation._setVersionStamp(key.VersionStamp);
            }
        }

        private static void _setVersionStamp(this Resource resource, Guid versionStamp)
        {
            if (versionStamp != Guid.Empty)
            {
                resource._createMetaIfNeeded();
                resource.Meta.VersionId = versionStamp.ToString();
            }
        }

        private static void _setId(this Resource resource, Guid id)
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
                resource._createMetaIfNeeded();
                resource.Meta.LastUpdated = new DateTimeOffset?(lastUpdated.Timestamp);
            }
        }

        private static void _createMetaIfNeeded(this Resource resource)
        {
            if (resource.Meta == null)
                resource.Meta = new Meta();
        }
    }
}
