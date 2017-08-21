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

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class FhirToItemBase
    {
        // Register the type on the generic ItemBaseToFhir partial class
        public static ApproximateDateTime ToItemBase(this Element fhirElement)
        {
            return FhirToApproximateDateTime.ToItemBaseInternal(fhirElement);
        }
    }
    internal static class FhirToApproximateDateTime
    {
        internal static ApproximateDateTime ToItemBaseInternal(Element fhirElement)
        {
            FhirDateTime fhirDateTime = null;
            FhirString fhirString = null;

            if (fhirElement is FhirDateTime)
            {
                fhirDateTime = fhirElement as FhirDateTime;
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

            if (fhirDateTime != null)
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
            return null;
        }
    }
}
