// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Reflection;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Units;
using Microsoft.HealthVault.Fhir.Vocabularies;
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
            return observation.ToHealthVault(typeof(T)) as T;
        }

        /// <summary>
        /// This extension method transforms from a FHIR Observation to a HealthVault Thing type
        /// </summary>       
        /// <param name="observation">The observation source</param>
        /// <returns>The HealthVault thing</returns>
        public static ThingBase ToHealthVault(this Observation observation)
        {
            return observation.ToHealthVault(VocabToHealthVaultHelper.DetectHealthVaultTypeFromObservation(observation));
        }

        internal static T ToThingBase<T>(this Observation observation) where T : ThingBase, new() {
            T baseThing = new T();

            Guid id;
            if (Guid.TryParse(observation.Id, out id))
            {
                baseThing.Key = new ThingKey(id);
            }

            Guid version;
            if (observation.Meta != null && observation.Meta.VersionId != null && Guid.TryParse(observation.Meta.VersionId, out version))
            {
                baseThing.Key.VersionStamp = version;
            }

            ThingFlags flags;
            var extensionFlag = observation.GetExtension(HealthVaultVocabularies.FlagsFhirExtensionName);
            if (extensionFlag != null)
            {
                if (extensionFlag.Value is FhirString && Enum.TryParse<ThingFlags>((extensionFlag.Value as FhirString).ToString(), out flags))
                {
                    baseThing.Flags = flags;
                }
            }

            return baseThing;
        }

        internal static T GetThingValueFromQuantity<T>(Quantity quantityValue) where T : Measurement<double>, new()
        {
            if (quantityValue?.Value == null)
            {
                return null;
            }
            
            var unitConversion = UnitResolver.Instance.UnitConversions.FirstOrDefault(x => x.Code.Equals(quantityValue.Code, StringComparison.Ordinal));

            double convertedValue;
            if (unitConversion != null)
            {
                var unitEnum = Enum.Parse(Type.GetType($"UnitsNet.Units.{unitConversion.UnitsNetUnitEnum}, UnitsNet"), unitConversion.UnitsNetSource);
                var unitType = Type.GetType($"UnitsNet.{unitConversion.UnitsNetType}, UnitsNet");
                var destinationObject = unitType.GetMethod("From", new[] {typeof(double), unitEnum.GetType()}).Invoke(null, new[] {(double)quantityValue.Value, unitEnum});
                        
                convertedValue = (double)destinationObject.GetType().GetProperty(unitConversion.UnitsNetDestination).GetValue(destinationObject);
            }
            else
            {
                convertedValue = (double)quantityValue.Value;
            }

            return new T
            {
                Value = convertedValue,
                DisplayValue = new DisplayValue
                {
                    Value = (double)quantityValue.Value,
                    Units = quantityValue.Unit,
                    UnitsCode = quantityValue.Code
                }
            };
        }
    
        internal static HealthServiceDateTime GetHealthVaultTimeFromEffectiveDate(Element effectiveDate)
        {
            FhirDateTime dateTime = null;

            /* Per Spec DSTU3 Effective Date in an observation can only be of type FhirDateTime or FhirPeriod
             * in this transformation if we have a period we will map it to the start time
             */
            if (effectiveDate is FhirDateTime)
            {
                dateTime = effectiveDate as FhirDateTime;
            }
            else if (effectiveDate is Period && ((Period)effectiveDate).Start != null)
            {
                dateTime = new FhirDateTime(((Period)effectiveDate).Start);
            }            
            
            if (dateTime != null)
            {
                var dt = dateTime.ToDateTimeOffset();
                return new HealthServiceDateTime(
                    new HealthServiceDate(dt.Year, dt.Month, dt.Day),
                    new ApproximateTime(dt.Hour, dt.Minute, dt.Second, dt.Millisecond));
            }

            return null;
        }

        private static ThingBase ToHealthVault(this Observation observation, Type type)
        {
            if (type == typeof(Weight))
            {
                return observation.ToWeight();
            }

            if (type == typeof(BloodGlucose))
            {
                return observation.ToBloodGlucose();
            }

            return null;
        }
    }
}
