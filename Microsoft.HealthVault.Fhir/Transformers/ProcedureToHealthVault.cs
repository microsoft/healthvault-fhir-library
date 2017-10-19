// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using FhirProcedure = Hl7.Fhir.Model.Procedure;
using Microsoft.HealthVault.ItemTypes;
using Hl7.Fhir.Support;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class ProcedureToHealthVault
    {
        public static Procedure ToHealthVault(this FhirProcedure fhirProcedure)
        {
            Procedure hvProcedure = fhirProcedure.ToThingBase<Procedure>();

            //Populate when (if present)
            if (fhirProcedure.Performed != null)
            {
                hvProcedure.When = fhirProcedure.Performed.ToAproximateDateTime();
            }

            if (fhirProcedure.Code.IsNullOrEmpty())
                throw new System.ArgumentException($"Can not transform a {typeof(FhirProcedure)} with no code into {typeof(Procedure)}");

            hvProcedure.Name = fhirProcedure.Code.ToCodableValue();
            hvProcedure.AnatomicLocation = fhirProcedure.BodySite?.FirstOrDefault()?.ToCodableValue();

            if (!fhirProcedure.Performer.IsNullOrEmpty())
            {
                hvProcedure.PrimaryProvider = GetProvider(fhirProcedure, 0);

                if (fhirProcedure.Performer.Count > 1)
                    hvProcedure.SecondaryProvider = GetProvider(fhirProcedure, 1);
            }

            return hvProcedure;
        }

        private static PersonItem GetProvider(FhirProcedure fhirProcedure, int index)
        {
            var performerComponent = fhirProcedure.Performer[index];
            if (performerComponent.Actor.IsContainedReference)
            {
                var containedReference = fhirProcedure.Contained.SingleOrDefault(resouce =>
                    resouce.Id.Equals(performerComponent.Actor.Reference) && resouce.GetType().Equals(typeof(Hl7.Fhir.Model.Practitioner)));

                if (containedReference == null)
                {
                    return null;
                }

                return (containedReference as Hl7.Fhir.Model.Practitioner).ToHealthVault();
            }

            if (string.IsNullOrEmpty(performerComponent.Actor.Display))
                return null;

            return new PersonItem()
            {
                Name = new Name(performerComponent.Actor.Display)
            };
        }
    }
}
