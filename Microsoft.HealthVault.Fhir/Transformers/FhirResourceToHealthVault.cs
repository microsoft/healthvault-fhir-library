using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class FhirResourceToHealthVault
    {
        internal static T ToThingBase<T>(this DomainResource fhirResource) where T : ThingBase, new()
        {
            T baseThing = new T();

            Guid id;
            if (Guid.TryParse(fhirResource.Id, out id))
            {
                baseThing.Key = new ThingKey(id);
            }

            Guid version;
            if (fhirResource.Meta != null && fhirResource.Meta.VersionId != null && Guid.TryParse(fhirResource.Meta.VersionId, out version))
            {
                baseThing.Key.VersionStamp = version;
            }

            ThingFlags flags;
            var extensionFlag = fhirResource.GetExtension(HealthVaultExtensions.FlagsFhirExtensionName);
            if (extensionFlag != null)
            {
                if (extensionFlag.Value is FhirString && Enum.TryParse<ThingFlags>((extensionFlag.Value as FhirString).ToString(), out flags))
                {
                    baseThing.Flags = flags;
                }
            }

            return baseThing;
        }
    }
}
