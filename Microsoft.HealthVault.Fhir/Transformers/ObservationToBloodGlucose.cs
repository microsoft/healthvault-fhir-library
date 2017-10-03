// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToBloodGlucose
    {
        internal static BloodGlucose ToBloodGlucose(this Observation observation)
        {
            var bloodGlucose = observation.ToThingBase<BloodGlucose>();

            bloodGlucose.Value = ObservationToHealthVault.GetThingValueFromQuantity<BloodGlucoseMeasurement>(observation.Value as Quantity);

            if (observation.Effective == null)
                throw new ArgumentException("Effective is required");

            bloodGlucose.When = observation.Effective.ToHealthServiceDateTime();

            var bloodGlucoseExtension = observation.GetExtension(HealthVaultExtensions.BloodGlucose);

            if (bloodGlucoseExtension != null)
            {
                bloodGlucose.MeasurementContext = bloodGlucoseExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.BloodGlucoseMeasurementContext).ToCodableValue();
                bloodGlucose.OutsideOperatingTemperature = bloodGlucoseExtension.GetBoolExtension(HealthVaultExtensions.OutsideOperatingTemperatureExtensionName);
                bloodGlucose.IsControlTest = bloodGlucoseExtension.GetBoolExtension(HealthVaultExtensions.IsControlTestExtensionName);

                Normalcy normalcy;
                if (Enum.TryParse<Normalcy>(bloodGlucoseExtension.GetStringExtension(HealthVaultExtensions.ReadingNormalcyExtensionName), out normalcy))
                {
                    bloodGlucose.ReadingNormalcy = normalcy;
                }
            }

            if (observation.Code != null && observation.Code.Coding != null)
            {
                foreach (var code in observation.Code.Coding)
                {
                    if (HealthVaultVocabularies.SystemContainsHealthVaultUrl(code.System))
                    {
                        bloodGlucose.GlucoseMeasurementType = observation.Method.ToCodableValue();
                    }
                    else
                    {
                        bloodGlucose.SetGlucoseMeasurementType(
                            code.Display, 
                            code.Code, 
                            HealthVaultVocabularies.Fhir, 
                            code.System, 
                            code.Version);                        
                    }
                }
            }

            return bloodGlucose;
        }

        private static void SetGlucoseMeasurementType(this BloodGlucose bloodGlucose, string display, string code, string vocabName, string family, string version)
        {
            if (bloodGlucose.GlucoseMeasurementType == null)
            {
                bloodGlucose.GlucoseMeasurementType = new CodableValue(display);                
            }

            bloodGlucose.GlucoseMeasurementType.Add(new CodedValue(code, vocabName, family, version));            
        }

        private static void SetGlucoseMeasurementContext(this BloodGlucose bloodGlucose, string display, string code, string vocabName, string family, string version)
        {
            if (bloodGlucose.MeasurementContext == null)
            {
                bloodGlucose.MeasurementContext = new CodableValue(display);
            }
                        
            bloodGlucose.MeasurementContext.Add(new CodedValue(code, vocabName, family, version));            
        }
    }
}
