// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation cdas (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Thing;
using static Hl7.Fhir.Model.DocumentReference;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class DocumentReferenceExtensions
    {
        public static ContentComponent GetFirstContentComponentWithData(this DocumentReference documentReference)
        {
            //We are considering only the first ContentComponent with data
            return documentReference.Content.First(o => o.Attachment != null && o.Attachment.Data != null);
        }

        public static void AddCommonData(this DocumentReference documentReference, CommonItemData commonData)
        {
            if (commonData != null && commonData.Note != null)
            {
                documentReference.AddNoteAsText(commonData.Note);
            }
        }

        public static void SetStatusAsCurrent(this DocumentReference documentReference)
        {
            documentReference.Status = DocumentReferenceStatus.Current;
        }

        public static void SetIndexed(this DocumentReference documentReference, NodaTime.LocalDateTime? localDateTime)
        {
            if (localDateTime.HasValue)
            {
                documentReference.Indexed = new DateTimeOffset(localDateTime.Value.Year, localDateTime.Value.Month, localDateTime.Value.Day,
                    localDateTime.Value.Hour, localDateTime.Value.Minute, localDateTime.Value.Second, new TimeSpan());
            }
            else
            {
                documentReference.Indexed = new DateTimeOffset();
            }
        }

        public static void SetType(this DocumentReference documentReference, CodeableConcept codeableConcept = null)
        {
            // Setting a generic Code to every DocumentReference for now
            documentReference.Type = codeableConcept ?? new CodeableConcept(VocabularyUris.Loinc, "51899-3", "Details Document", "Details Document");
        }
    }
}
