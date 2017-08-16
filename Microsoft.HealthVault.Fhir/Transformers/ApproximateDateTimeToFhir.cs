using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ItemBaseToFhir
    {
        // Register the type on the generic ItemBaseToFhir partial class
        public static FhirDateTime ToFhir(this ApproximateDateTime approximateDateTime)
        {
            return ApproximateDateTimeToFhir.ToFhirInternal(approximateDateTime);
        }
    }
    internal static class ApproximateDateTimeToFhir
    {
        internal static FhirDateTime ToFhirInternal(ApproximateDateTime approximateDateTime)
        {
            if (!approximateDateTime.ApproximateDate.Month.HasValue && !approximateDateTime.ApproximateDate.Day.HasValue)
                return new FhirDateTime(approximateDateTime.ApproximateDate.Year);
            else if (approximateDateTime.ApproximateDate.Month.HasValue && !approximateDateTime.ApproximateDate.Day.HasValue)
                return new FhirDateTime(approximateDateTime.ApproximateDate.Year, approximateDateTime.ApproximateDate.Month.Value);
            else if (approximateDateTime.ApproximateDate.Month.HasValue && approximateDateTime.ApproximateDate.Day.HasValue && (approximateDateTime.ApproximateTime == null || !approximateDateTime.ApproximateTime.HasValue))
                return new FhirDateTime(approximateDateTime.ApproximateDate.Year, approximateDateTime.ApproximateDate.Month.Value, approximateDateTime.ApproximateDate.Day.Value);
            else
                return new FhirDateTime(
                   approximateDateTime.ApproximateDate.Year,
                   approximateDateTime.ApproximateDate.Month ?? 1,
                   approximateDateTime.ApproximateDate.Day ?? 1,
                   approximateDateTime.ApproximateTime?.Hour ?? 0,
                   approximateDateTime.ApproximateTime?.Minute ?? 0,
                   approximateDateTime.ApproximateTime?.Second ?? 0);
        }
    }
}
