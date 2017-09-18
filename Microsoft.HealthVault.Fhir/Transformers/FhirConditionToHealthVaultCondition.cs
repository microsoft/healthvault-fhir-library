// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using HVCondition = Microsoft.HealthVault.ItemTypes.Condition;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class ConditionToHealthVault
    {
        public static HVCondition ToHealthVault(this Condition fhirCondition)
        {
            HVCondition hvCondition = fhirCondition.ToThingBase<HVCondition>();

            string conditionOccurence = string.Empty;
            var conditionExtension = fhirCondition.GetExtension(HealthVaultExtensions.Condition);
            if (conditionExtension != null)
            {
                hvCondition.StopReason = conditionExtension.GetStringExtension(HealthVaultExtensions.ConditionStopReason);
                conditionOccurence = conditionExtension.GetStringExtension(HealthVaultExtensions.ConditionOccurrence);
            }


            hvCondition.OnsetDate = fhirCondition.Onset.ToAproximateDateTime();
            hvCondition.StopDate = fhirCondition.Abatement.ToAproximateDateTime();


            if (fhirCondition.ClinicalStatus.HasValue)
            {
                hvCondition.Status = new ItemTypes.CodableValue(fhirCondition.ClinicalStatus.Value.ToString());
                hvCondition.Status.Add(new ItemTypes.CodedValue(fhirCondition.ClinicalStatus.Value.ToString(), FhirCategories.Hl7Condition, HealthVaultVocabularies.Fhir, ""));
            }
            else if (!string.IsNullOrWhiteSpace(conditionOccurence))
            {
                hvCondition.Status = new ItemTypes.CodableValue(conditionOccurence);
                hvCondition.Status.Add(new ItemTypes.CodedValue(conditionOccurence, HealthVaultVocabularies.ConditionOccurrence, HealthVaultVocabularies.Wc, "1"));
            }

            if (fhirCondition.Code.IsNullOrEmpty())
                throw new System.ArgumentNullException($"Can not transform a {typeof(Condition)} with no code into {typeof(HVCondition)}");

            hvCondition.Name = fhirCondition.Code.ToCodableValue();

            if (fhirCondition.Note != null)
            {
                foreach (Annotation annotation in fhirCondition.Note)
                {
                    hvCondition.CommonData.Note += annotation.Text;
                }
            }

            return hvCondition;
        }
    }
}
    
      