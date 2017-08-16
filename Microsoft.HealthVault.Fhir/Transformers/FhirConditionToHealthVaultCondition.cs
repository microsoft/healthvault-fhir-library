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
        public static HVCondition ToHealthVault(this Condition fhircondition)
        {
            return fhircondition.ToCondition();
        }
    }
    internal static class FhirConditionToHealthVaultCondition
    {
        internal static HVCondition ToCondition(this Condition fhircondition)
        {
            HVCondition hvCondition = new HVCondition();

            Guid id;
            if (Guid.TryParse(fhircondition.Id, out id))
            {
                hvCondition.Key = new ThingKey(id);
            }

            Guid version;
            if (fhircondition.Meta != null && fhircondition.Meta.VersionId != null && Guid.TryParse(fhircondition.Meta.VersionId, out version))
            {
                hvCondition.Key.VersionStamp = version;
            }

            hvCondition.StopReason = fhircondition.GetStringExtension(HealthVaultVocabularies.ConditionStopReason);
            hvCondition.CommonData.Source = fhircondition.GetStringExtension(HealthVaultVocabularies.ConditionSource);
            hvCondition.OnsetDate = GetHealthVaultApproximateTimeFromEffectiveDate(fhircondition.Onset);
            hvCondition.StopDate = GetHealthVaultApproximateTimeFromEffectiveDate(fhircondition.Abatement);

            if (fhircondition.ClinicalStatus.HasValue)
            {
                hvCondition.Status =  new ItemTypes.CodableValue(fhircondition.ClinicalStatus.Value.ToString());
                hvCondition.Status.Add(new ItemTypes.CodedValue(fhircondition.ClinicalStatus.Value.ToString(), FhirCategories.Hl7Condition,HealthVaultVocabularies.Fhir, ""));
            }
            else
            {
                hvCondition.Status = new ItemTypes.CodableValue(fhircondition.GetStringExtension(HealthVaultVocabularies.ConditionOccurenceExentsionName));
                hvCondition.Status.Add(new ItemTypes.CodedValue(fhircondition.GetStringExtension
                    (HealthVaultVocabularies.ConditionOccurenceExentsionName), HealthVaultVocabularies.ConditionOccurence, HealthVaultVocabularies.Wc, "1"));
            }

            if (fhircondition.Code != null && fhircondition.Code.Coding != null)
            {
                foreach (var code in fhircondition.Code.Coding)
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

            if (fhircondition.Note != null)
            {
                foreach (Annotation annotation in fhircondition.Note)
                {
                    hvCondition.CommonData.Note += annotation.Text;
                }                
            }
            
            return hvCondition;
        }
        private static ItemTypes.ApproximateDateTime GetHealthVaultApproximateTimeFromEffectiveDate(Element fhirElement)
        {
            FhirDateTime fhirdateTime = null;
            FhirString fhirString = null;

            if (fhirElement is FhirDateTime)
            {
                fhirdateTime = fhirElement as FhirDateTime;
            }

            if (fhirElement is FhirString)
            {
                fhirString = fhirElement as FhirString;
            }

            if (fhirString != null)
            {
                ItemTypes.ApproximateDateTime approximateDateTime = new ItemTypes.ApproximateDateTime();
                approximateDateTime.Description = fhirString.ToString();
                return approximateDateTime;
            }

            if (fhirdateTime != null)
            {
                var dt = fhirdateTime.ToDateTimeOffset();
                switch (FhirDateTimeExtensions.Precision(fhirdateTime))
                {
                    case FhirDateTimeExtensions.FhirDateTimePrecision.Year:
                        return new ItemTypes.ApproximateDateTime()
                        {
                            ApproximateDate = new ItemTypes.ApproximateDate(dt.Year)
                        };
                    case FhirDateTimeExtensions.FhirDateTimePrecision.Month:
                        return new ItemTypes.ApproximateDateTime()
                        {
                            ApproximateDate = new ItemTypes.ApproximateDate(dt.Year, dt.Month)
                        };
                    case FhirDateTimeExtensions.FhirDateTimePrecision.Day:
                        return new ItemTypes.ApproximateDateTime()
                        {
                            ApproximateDate = new ItemTypes.ApproximateDate(dt.Year, dt.Month, dt.Day)
                        };
                    case FhirDateTimeExtensions.FhirDateTimePrecision.Minute:
                        return new ItemTypes.ApproximateDateTime()
                        {
                            ApproximateDate = new ItemTypes.ApproximateDate(dt.Year, dt.Month, dt.Day),
                            ApproximateTime = new ItemTypes.ApproximateTime(dt.Hour, dt.Minute)
                        };
                    case FhirDateTimeExtensions.FhirDateTimePrecision.Second:
                        return new ItemTypes.ApproximateDateTime()
                        {
                            ApproximateDate = new ItemTypes.ApproximateDate(dt.Year, dt.Month, dt.Day),
                            ApproximateTime = new ItemTypes.ApproximateTime(dt.Hour, dt.Minute, dt.Second)
                        };
                    default:
                        return new ItemTypes.ApproximateDateTime()
                        {
                            ApproximateDate = new ItemTypes.ApproximateDate(dt.Year, dt.Month, dt.Day),
                            ApproximateTime = new ItemTypes.ApproximateTime(dt.Hour, dt.Minute, dt.Second, dt.Millisecond)
                        };
                }
            }
            return null;
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
