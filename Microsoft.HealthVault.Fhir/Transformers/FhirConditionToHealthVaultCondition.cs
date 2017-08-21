// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.Thing;
using HVCondition = Microsoft.HealthVault.ItemTypes.Condition;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class ConditionToHealthVault
    {
        public static HVCondition ToHealthVault(this Condition fhirCondition)
        {
            return fhirCondition.ToCondition();
        }
    }
    internal static class FhirConditionToHealthVaultCondition
    {
        internal static HVCondition ToCondition(this Condition fhirCondition)
        {
            HVCondition hvCondition = new HVCondition();

            Guid id;
            if (Guid.TryParse(fhirCondition.Id, out id))
            {
                hvCondition.Key = new ThingKey(id);
            }

            Guid version;
            if (fhirCondition.Meta != null && fhirCondition.Meta.VersionId != null && Guid.TryParse(fhirCondition.Meta.VersionId, out version))
            {
                hvCondition.Key.VersionStamp = version;
            }

            hvCondition.StopReason = fhirCondition.GetStringExtension(HealthVaultVocabularies.ConditionStopReason);
            hvCondition.CommonData.Source = fhirCondition.GetStringExtension(HealthVaultVocabularies.ConditionSource);
            hvCondition.OnsetDate = fhirCondition.Onset.ToItemBase();
            hvCondition.StopDate = fhirCondition.Abatement.ToItemBase();

            if (fhirCondition.ClinicalStatus.HasValue)
            {
                hvCondition.Status =  new ItemTypes.CodableValue(fhirCondition.ClinicalStatus.Value.ToString());
                hvCondition.Status.Add(new ItemTypes.CodedValue(fhirCondition.ClinicalStatus.Value.ToString(), FhirCategories.Hl7Condition,HealthVaultVocabularies.Fhir, ""));
            }
            else
            {
                hvCondition.Status = new ItemTypes.CodableValue(fhirCondition.GetStringExtension(HealthVaultVocabularies.ConditionOccurrenceExtensionName));
                hvCondition.Status.Add(new ItemTypes.CodedValue(fhirCondition.GetStringExtension
                    (HealthVaultVocabularies.ConditionOccurrenceExtensionName), HealthVaultVocabularies.ConditionOccurrence, HealthVaultVocabularies.Wc, "1"));
            }

            if (fhirCondition.Code != null && fhirCondition.Code.Coding != null)
            {
                foreach (var code in fhirCondition.Code.Coding)
                {
                    var value = code.Code.Split(':');
                    var vocabCode = value.Length == 2 ? value[1] : null;
                    var vocab = value.Length == 2 ? value[0] : HealthVaultVocabularies.Fhir;

                    hvCondition.SetConditionName(
                            code.Display,
                            vocabCode,
                            vocab,
                            code.System,
                            code.Version);
                }
            }

            if (fhirCondition.Note != null)
            {
                foreach (Annotation annotation in fhirCondition.Note)
                {
                    hvCondition.CommonData.Note += annotation.Text;
                }                
            }
            
            return hvCondition;
        }
        private static void SetConditionName(this HVCondition condition, string display, string code, string vocabName, string family, string version)
        {
            if (condition.Name == null || condition.Name.Text == null)
            {
                condition.Name = new ItemTypes.CodableValue(display);
            }

            condition.Name.Add(new ItemTypes.CodedValue(code, vocabName, family, version));
        }
    }
}
