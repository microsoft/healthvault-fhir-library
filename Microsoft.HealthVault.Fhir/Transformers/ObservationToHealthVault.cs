// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class ObservationToHealthVault
    {
        /// <summary>
        /// This extension method transforms from a FHIR Observation to a HealthVault Thing type
        /// </summary>
        /// <typeparam name="T">The HealthVault thing type to use for the transformation</typeparam>
        /// <param name="observation">The observation source</param>
        /// <returns>The HealthVault thing</returns>
        public static T ToHealthVault<T>(this Observation observation) where T : ThingBase
        {
            return observation.ToHealthVault() as T;
        }

        /// <summary>
        /// This extension method transforms from a FHIR Observation to a HealthVault Thing type
        /// </summary>       
        /// <param name="observation">The observation source</param>
        /// <returns>The HealthVault thing</returns>
        public static ThingBase ToHealthVault(this Observation observation)
        {
            // ToDo: detect the type from the codeable values
            return observation.ToWeight();
        }

        private static Weight ToWeight(this Observation observation)
        {
            var weight = new Weight();
            var value = observation.Value as Quantity;

            // TODO: detect the units from the code (value.Unit)
            if (value != null)
            {
                if (value.Value.HasValue)
                {
                    weight.Value = new WeightValue((double)value.Value);
                    weight.Value.DisplayValue = new DisplayValue();
                    weight.Value.DisplayValue.Value = (double)value.Value;
                    weight.Value.DisplayValue.Units = value.Unit;
                    weight.Value.DisplayValue.UnitsCode = value.Code;
                }
            }

            var effectiveDate = observation.Effective as FhirDateTime;

            if (effectiveDate != null)
            {
                var dateTime = effectiveDate.ToDateTimeOffset();
                weight.When = new HealthServiceDateTime(
                    new HealthServiceDate(dateTime.Year, dateTime.Month, dateTime.Day),
                    new ApproximateTime(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond));
            }

            return weight;
        }
    }
}
