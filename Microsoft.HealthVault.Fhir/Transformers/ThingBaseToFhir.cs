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
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    /// <summary>
    /// An extension class that helps transform things into fhir resources
    /// </summary>
    public static partial class ThingBaseToFhir
    {
        /// <summary>
        /// Transforms a HealthVault thing into a FHIR Resource
        /// </summary>
        /// <param name="thing">The HealthVault thing to transform</param>
        /// <returns>A FHIR resource based on the HealthVault thing</returns>
        public static Resource ToFhir(this ThingBase thing)
        {
            var transformerType = typeof(ThingBaseToFhir);
            var thingType = thing.GetType();
            var method = transformerType.GetRuntimeMethod("ToFhir", new Type[] { thingType });

            if (method != null && method.GetParameters()[0].ParameterType != typeof(ThingBase))
            {
                return (Resource)method.Invoke(null, new object[] { thing });
            }
            else
            {
                throw new NotImplementedException($"Unable to find tranformer for '{thingType}'");
            }
        }

        internal static T ToFhirInternal<T>(this ThingBase thing) where T : Resource, IExtendable, new()
        {
            var resource = new T();

            resource.SetIdentity(thing.Key);
            resource.CopyLastUpdatedFromAudit(thing.LastUpdated);

            //Extensions
            resource.AddFlagsAsExtension(thing.Flags);
            resource.AddStateAsExtension(thing.State);

            resource.DoTransforms(thing);

            return resource;
        }

        /// <summary>
        /// Do transforms from thing to existing resource
        /// </summary>
        /// <param name="resource">A Fhir resource to add transforms to</param>
        /// <param name="thing">The healthvault thing to get tranforms from</param>
        public static void DoTransforms(this Resource resource, ThingBase thing)
        {
            var transformerType = typeof(ThingBaseToFhir);
            var method = transformerType.GetRuntimeMethod("DoTransforms", new Type[] { resource.GetType(), thing.GetType() });

            if (method != null && method.GetParameters()[0].ParameterType != typeof(Resource))
            {
                method.Invoke(null, new object[] { resource, thing });
            }
            else
            {
                resource.DoTransformInternal(thing);
            }
        }

        public static void DoTransformInternal(this Resource observation, ThingBase thing)
        {
            //NOP
        }
    }
}
