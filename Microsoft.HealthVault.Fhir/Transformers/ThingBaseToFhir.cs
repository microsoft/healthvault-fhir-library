// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    /// <summary>
    /// An extension class that helps transform things into fhir resources
    /// </summary>
    public static partial class ThingBaseToFhir
    {
        /// <summary>
        /// Transforms a HealthVault thing into a FHIR Observation
        /// </summary>
        /// <param name="thing">The HealthVault thing to transform</param>
        /// <returns>A FHIR observation based on the HealthVault thing</returns>
        public static Observation ToFhir(this ThingBase thing)
        {
            // Find registered specific ToFhir method
            var transformerType = typeof(ThingBaseToFhir);
            var method = transformerType.GetRuntimeMethod("ToFhir", new Type[] { thing.GetType() });

            if (method != null && method.GetParameters()[0].ParameterType != typeof(ThingBase))
            {
                return (Observation)method.Invoke(null, new object[] { thing });
            }
            else
            {
                return thing.ToFhirInternal();
            }
        }

        internal static Observation ToFhirInternal(this ThingBase thing)
        {
            var observation = new Observation();
            observation.Meta = new Meta();

            observation.AddExtension(HealthVaultVocabularies.FlagsFhirExtensionName, new FhirString(thing.Flags.ToString()));
            observation.AddExtension(HealthVaultVocabularies.StateFhirExtensionName, new FhirString(thing.State.ToString()));

            observation.Status = ObservationStatus.Final;

            if (thing.Key != null)
            {
                if (thing.Key.Id != null)
                {
                    observation.Id = thing.Key.Id.ToString();
                }

                if (thing.Key.VersionStamp != null)
                {
                    observation.Meta.VersionId = thing.Key.VersionStamp.ToString();
                }
            }

            if (thing.Created != null)
            {
                if (thing.Created.Timestamp != null)
                {
                    observation.Issued = thing.Created.Timestamp;
                }
            }

            if (thing.LastUpdated != null)
            {
                observation.Meta.LastUpdated = new DateTimeOffset?(thing.LastUpdated.Timestamp);
            }

            if (thing.CommonData != null)
            {
                if (!string.IsNullOrEmpty(thing.CommonData.Note))
                {
                    observation.Text = new Narrative() { Div = thing.CommonData.Note };
                }

                if (!string.IsNullOrEmpty(thing.CommonData.Source))
                {
                    observation.Device = new ResourceReference(thing.CommonData.Source);
                }

                if (thing.CommonData.RelatedItems != null && thing.CommonData.RelatedItems.Any())
                {
                    observation.Related = new List<Observation.RelatedComponent>();
                    foreach (var item in thing.CommonData.RelatedItems)
                    {
                        if (item.ItemKey != null)
                        {
                            observation.Related.Add(new Observation.RelatedComponent() { Target = new ResourceReference($"observation/{item.ItemKey.Id}") });
                        }
                    }
                }
            }
            
            return observation;
        }        
    }
}
