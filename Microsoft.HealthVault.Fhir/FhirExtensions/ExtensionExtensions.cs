using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class ExtensionExtensions
    {
        public static Extension AddStringAsExtension(this IExtendable extendable, string uri, string value, bool isModifier = false)
        {
            return extendable.AddExtension(uri, new FhirString(value), isModifier);
        }

        public static Extension AddFlagsAsExtension(this IExtendable extendable, ThingFlags flags)
        {
            return extendable.AddStringAsExtension(HealthVaultVocabularies.FlagsFhirExtensionName, flags.ToString());
        }

        public static Extension AddStateAsExtension(this IExtendable extendable, ThingState state)
        {
            return extendable.AddStringAsExtension(HealthVaultVocabularies.StateFhirExtensionName, state.ToString());
        }
    }
}
