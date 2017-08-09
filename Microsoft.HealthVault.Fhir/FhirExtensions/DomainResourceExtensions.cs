using Hl7.Fhir.Model;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class DomainResourceExtensions
    {

        public static void AddNoteAsText(this DomainResource domainResource, string note)
        {
            if (!string.IsNullOrEmpty(note))
            {
                domainResource.Text = new Narrative() { Div = note };
            }
        }
    }
}
