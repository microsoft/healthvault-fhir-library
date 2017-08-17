// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
