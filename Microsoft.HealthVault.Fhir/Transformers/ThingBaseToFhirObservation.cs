using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        public static void DoTransforms(this Observation observation, ThingBase thing)
        {
            ThingBaseToFhirObservation.DoTransformInternal(observation, thing);
        }
    }
    internal static class ThingBaseToFhirObservation
    {
        internal static Observation DoTransformInternal(Observation observation, ThingBase thing)
        {
            observation.SetStatusAsFinal();

            observation.CopyIssuedFromAudit(thing.Created);

            observation.AddCommonData(thing.CommonData);

            return observation;
        }
    }
}
