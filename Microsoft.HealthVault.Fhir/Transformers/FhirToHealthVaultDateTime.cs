// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.ItemTypes;
using System;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class FhirToHealthVaultDateTime
    {
        public static ApproximateDateTime ToAproximateDateTime(this Element fhirElement)
        {
            switch (fhirElement)
            {
                case FhirString fhirString:
                    ItemTypes.ApproximateDateTime approximateDateTime = new ItemTypes.ApproximateDateTime();
                    approximateDateTime.Description = fhirString.ToString();
                    return approximateDateTime;
                case FhirDateTime fhirDateTime:
                    return processDate(fhirDateTime);
                case Period fhirPeriod:
                    return processDate(fhirPeriod.StartElement ?? fhirPeriod.EndElement);
                default:
                    throw new NotSupportedException($"Conversion from {fhirElement.GetType()} to {typeof(ApproximateDateTime)} is not supported");
            }
        }

        private static ApproximateDateTime processDate(FhirDateTime fhirDateTime)
        {
            var dt = fhirDateTime.ToDateTimeOffset();
            switch (FhirDateTimeExtensions.Precision(fhirDateTime))
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

        public static HealthServiceDateTime ToHealthServiceDateTime(this Element effectiveDate)
        {
            var dateTime = effectiveDate.ToAproximateDateTime();

            if (dateTime.ApproximateDate.Day != null)
            {
                var approximateTime = new ApproximateTime(0, 0);
                if (dateTime.ApproximateTime != null)
                {
                    approximateTime = new ApproximateTime(dateTime.ApproximateTime.Hour, dateTime.ApproximateTime.Minute,
                            dateTime.ApproximateTime.Second ?? 0, dateTime.ApproximateTime.Millisecond ?? 0);
                }

                return new HealthServiceDateTime(
                    new HealthServiceDate(dateTime.ApproximateDate.Year, dateTime.ApproximateDate.Month.Value, dateTime.ApproximateDate.Day.Value),
                    approximateTime
                );
            }

            return null;
        }
    }
}