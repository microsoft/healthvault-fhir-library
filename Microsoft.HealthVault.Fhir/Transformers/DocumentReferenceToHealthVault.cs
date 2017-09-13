// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class DocumentReferenceToHealthVault
    {
        /// <summary>
        /// This extension method transforms from a FHIR DocumentReference to a HealthVault Thing type
        /// </summary>
        /// <typeparam name="T">The HealthVault thing type to use for the transformation</typeparam>
        /// <param name="documentReference">The DocumentReference source</param>
        /// <returns>The HealthVault thing</returns>
        public static T ToHealthVault<T>(this DocumentReference documentReference) where T : ThingBase
        {
            return documentReference.ToHealthVault(typeof(T)) as T;
        }

        /// <summary>
        /// This extension method transforms from a FHIR DocumentReference to a HealthVault Thing type
        /// </summary>
        /// <param name="documentReference">The DocumentReference source</param>
        /// <returns>The HealthVault thing</returns>
        public static ThingBase ToHealthVault(this DocumentReference documentReference)
        {
            return documentReference.ToHealthVault(CodeToHealthVaultHelper.DetectHealthVaultTypeFromDocumentReference(documentReference));
        }

        internal static T ToThingBase<T>(this DocumentReference documentReference) where T : ThingBase, new()
        {
            T baseThing = new T();

            Guid id;
            if (Guid.TryParse(documentReference.Id, out id))
            {
                baseThing.Key = new ThingKey(id);
            }

            Guid version;
            if (documentReference.Meta != null && documentReference.Meta.VersionId != null && Guid.TryParse(documentReference.Meta.VersionId, out version))
            {
                baseThing.Key.VersionStamp = version;
            }

            ThingFlags flags;
            var extensionFlag = documentReference.GetExtension(HealthVaultExtensions.FlagsFhirExtensionName);
            if (extensionFlag != null)
            {
                if (extensionFlag.Value is FhirString && Enum.TryParse<ThingFlags>((extensionFlag.Value as FhirString).ToString(), out flags))
                {
                    baseThing.Flags = flags;
                }
            }

            if (documentReference.Indexed.HasValue)
            {
                var indexed = documentReference.Indexed.Value;
                baseThing.EffectiveDate = new NodaTime.LocalDateTime(indexed.Year, indexed.Month, indexed.Day, indexed.Hour, indexed.Minute, indexed.Second);
            }

            return baseThing;
        }

        private static ThingBase ToHealthVault(this DocumentReference documentReference, Type type)
        {
            if (type == typeof(File))
            {
                return documentReference.ToFile();
            }

            if (type == typeof(CCR))
            {
                return documentReference.ToCCR();
            }

            if (type == typeof(CDA))
            {
                return documentReference.ToCDA();
            }

            return null;
        }
    }
}
