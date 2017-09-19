// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.FhirExtensions;
using HVCondition = Microsoft.HealthVault.ItemTypes.Condition;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Condition ToFhir(this HVCondition cd)
        {
            return HealthVaultConditionToFhir.ToFhirInternal(cd, ToFhirInternal<Condition>(cd));
        }
    }
    /// <summary>
    /// An extension class that transforms HealthVault condition data types into FHIR Condition
    /// </summary>
    internal static class HealthVaultConditionToFhir
    {
        internal static Condition ToFhirInternal(HVCondition hvCondition, Condition fhirCondition)
        {
            var conditionExtension = new Extension
            {
                Url = HealthVaultExtensions.Condition
            };

            if (hvCondition.CommonData != null)
            {
                fhirCondition.AddNoteAsText(hvCondition.CommonData.Note);
                var note = new Annotation();
                note.Text = hvCondition.CommonData.Note;
                fhirCondition.Note = new List<Annotation> {note};
            }
                    
            fhirCondition.Code = hvCondition.Name.ToFhir();
            
            if (hvCondition.Status != null)
            {
                fhirCondition.SetClinicalStatusCode(hvCondition.Status,conditionExtension);
            }

            if (hvCondition.StopDate != null)
            {
                fhirCondition.SetAbatement(hvCondition.StopDate); 
            }

            if (hvCondition.OnsetDate != null)
            {
                fhirCondition.Onset = hvCondition.OnsetDate.ToFhir();
            }

            if (hvCondition.StopReason != null)
            {
                conditionExtension.AddExtension(HealthVaultExtensions.ConditionStopReason, new FhirString(hvCondition.StopReason));
            }

            fhirCondition.Extension.Add(conditionExtension);

            return fhirCondition;
        }

        private static void SetClinicalStatusCode(this Condition fhirCondition, ItemTypes.CodableValue status, Extension conditionExtension)
        {
            foreach (ItemTypes.CodedValue cValue in status)
            {
                Condition.ConditionClinicalStatusCodes clinicalStatusCode;
                if (cValue.Value != null)
                {
                    if (Enum.TryParse(cValue.Value, true, out clinicalStatusCode))
                    {
                        fhirCondition.ClinicalStatus = clinicalStatusCode;
                        return;
                    }
                    else
                    {
                        conditionExtension.AddExtension(HealthVaultExtensions.ConditionOccurrence, new FhirString(cValue.Value));
                        return;
                    }
                }
            }

        }

        private static void SetAbatement(this Condition fhirCondition, ItemTypes.ApproximateDateTime approximateDateTime)
        {            
            if (approximateDateTime.ApproximateDate != null)
            {             
                fhirCondition.Abatement = approximateDateTime.ToFhir();  
            }
            else
            {
                fhirCondition.Abatement = new FhirString(approximateDateTime.Description.ToString());
            }           
        }

       
    }
}
