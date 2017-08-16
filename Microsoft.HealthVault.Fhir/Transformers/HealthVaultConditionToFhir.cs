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

        internal static Condition ToFhirInternal(HVCondition cd, Condition fhirCondition)
        {

            var fhirCodes = new List<Coding>();
            if (cd.CommonData != null)
            {
                fhirCondition.AddNoteAsText(cd.CommonData.Note);
                var note = new Annotation();
                note.Text = cd.CommonData.Note;
                fhirCondition.Note = new List<Annotation> {note};
                fhirCondition.AddExtension(HealthVaultVocabularies.ConditionSource, new FhirString(cd.CommonData.Source));
            }

            if (cd.Name != null)
            {
                fhirCondition.Code = new CodeableConcept() { Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(cd.Name, fhirCodes) };
                fhirCondition.Code.Text = fhirCondition.Code.Coding[0].Display;
            }

            if (cd.Status != null)
            {
                fhirCondition.SetClinicalStatusCode(cd.Status);
            }

            if (cd.StopDate != null)
            {
                fhirCondition.SetAbatement(cd.StopDate); 
            }

            if (cd.OnsetDate != null)
            {
                fhirCondition.Onset = cd.OnsetDate.ToFhir();
            }

            if (cd.StopReason != null)
            {
                fhirCondition.AddExtension(HealthVaultVocabularies.ConditionStopReason, new FhirString(cd.StopReason));
            }

            return fhirCondition;
        }

        private static void SetClinicalStatusCode(this Condition fhirCondition, ItemTypes.CodableValue status)
        {
            foreach (ItemTypes.CodedValue cvalue in status)
            {
                Condition.ConditionClinicalStatusCodes clinicalStatusCode;
                if (cvalue.Value != null)
                {
                    if (Enum.TryParse(cvalue.Value, true, out clinicalStatusCode))
                    {
                        fhirCondition.ClinicalStatus = clinicalStatusCode;
                        return;
                    }
                    else
                    {
                        fhirCondition.AddExtension(HealthVaultVocabularies.ConditionOccurenceExentsionName, new FhirString(cvalue.Value));
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
