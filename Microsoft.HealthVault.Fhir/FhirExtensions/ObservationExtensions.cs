using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class ObservationExtensions
    {
        public static void AddCommonData(this Observation observation, CommonItemData commonData)
        {
            if (commonData != null)
            {
                observation.AddNoteAsText(commonData.Note);

                observation.AddSourceAsDevice(commonData.Source);

                observation.AddRelated(commonData.RelatedItems);
            }
        }

        public static void AddRelated(this Observation observation, ICollection<ThingRelationship> relatedItems)
        {
            if (relatedItems != null && relatedItems.Any())
            {
                observation.Related = new List<Observation.RelatedComponent>();
                foreach (var item in relatedItems)
                {
                    if (item.ItemKey != null)
                    {
                        observation.Related.Add(new Observation.RelatedComponent() { Target = new ResourceReference($"observation/{item.ItemKey.Id}") });
                    }
                }
            }
        }

        public static void AddSourceAsDevice(this Observation observation, string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                observation.Device = new ResourceReference(source);
            }
        }

        public static void CopyIssuedFromAudit(this Observation observation, HealthServiceAudit created)
        {
            if (created != null)
            {
                observation.Issued = created.Timestamp.ToDateTimeOffset();
            }
        }

        public static void SetStatusAsFinal(this Observation observation)
        {
            observation.Status = ObservationStatus.Final;
        }
    }
}
