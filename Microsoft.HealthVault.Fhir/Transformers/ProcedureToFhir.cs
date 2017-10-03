// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;
using FhirProcedure = Hl7.Fhir.Model.Procedure;
using HVProcedure = Microsoft.HealthVault.ItemTypes.Procedure;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        public static FhirProcedure ToFhir(this HVProcedure hvProcedure)
        {
            return ProcedureToFhir.ToFhirInternal(hvProcedure, hvProcedure.ToFhirInternal<FhirProcedure>());
        }
    }

    internal static class ProcedureToFhir
    {
       internal static FhirProcedure ToFhirInternal(HVProcedure hvProcedure, FhirProcedure fhirProcedure)
        {
            //Status is a required field; however HealthVault doesn't have 
            //an equivalent for this
            fhirProcedure.Status = EventStatus.Unknown;
            fhirProcedure.Code = hvProcedure.Name.ToFhir();

            addPerformer(fhirProcedure, hvProcedure.PrimaryProvider?.ToFhir());
            addPerformer(fhirProcedure, hvProcedure.SecondaryProvider?.ToFhir());

            if (hvProcedure.AnatomicLocation != null)
            {
                fhirProcedure.BodySite.Add(hvProcedure.AnatomicLocation.ToFhir());
            }

            var fhirDate = hvProcedure.When?.ToFhir();
            if(fhirDate != null)
            {                
                fhirProcedure.Performed = fhirDate;
            }

            return fhirProcedure;
        }

        private static void addPerformer(FhirProcedure fhirProcedure, Practitioner practitioner)
        {
            if (practitioner != null)
            {
                practitioner.Id = $"#practitioner-{Guid.NewGuid()}";
                fhirProcedure.Contained.Add(practitioner);
                fhirProcedure.Performer.Add(new FhirProcedure.PerformerComponent
                {
                    Actor = new ResourceReference
                    {
                        Reference = practitioner.Id
                    }
                });
            }
        }
    }
}
