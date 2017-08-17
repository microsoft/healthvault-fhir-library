using System;
// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class PatientToHealthVault
    {
        public static List<ThingBase> ToHealthVault(this Patient patient)
        {
            var resultList = new List<ThingBase>();

            resultList.Add(patient.ToBasicV2());
            resultList.Add(patient.ToContact());
            resultList.Add(patient.ToPersonal());
            resultList.AddRange(patient.ToPersonalImage());

            return resultList;
        }

        internal static T ToThingBase<T>(this Patient patient) where T : ThingBase, new()
        {
            T baseThing = new T();

            Guid id;
            if (Guid.TryParse(patient.Id, out id))
            {
                baseThing.Key = new ThingKey(id);
            }

            Guid version;
            if (patient.Meta != null && patient.Meta.VersionId != null && Guid.TryParse(patient.Meta.VersionId, out version))
            {
                baseThing.Key.VersionStamp = version;
            }

            ThingFlags flags;
            var extensionFlag = patient.GetExtension(HealthVaultVocabularies.FlagsFhirExtensionName);
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
