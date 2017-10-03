// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToBloodPressure
    {
        internal static BloodPressure ToBloodPressure(this Observation observation)
        {
            var bloodPressure = observation.ToThingBase<BloodPressure>();

            if (observation.Effective == null)
                throw new ArgumentException("Effective is required");

            bloodPressure.When = observation.Effective.ToHealthServiceDateTime();

            bloodPressure.IrregularHeartbeatDetected = observation.GetBoolExtension(HealthVaultVocabularies.IrregularHeartBeatExtensionName);

            if(observation.Component != null)
            {
                foreach(var component in observation.Component)
                {
                    if(component.Code != null && component.Code.Coding != null)
                    {
                        foreach (var code in component.Code.Coding)
                        {
                            if (code.Code != null)
                            {
                                if (HealthVaultVocabularies.SystemContainsHealthVaultUrl(code.System))
                                {
                                    if (code.Code == HealthVaultVitalStatisticsCodes.BloodPressureDiastolic.Code)
                                    {
                                        SetDiastolic(bloodPressure, component);
                                    }
                                    else if (code.Code == HealthVaultVitalStatisticsCodes.BloodPressureSystolic.Code)
                                    {
                                        SetSystolic(bloodPressure, component);
                                    }
                                    else if (code.Code == HealthVaultVitalStatisticsCodes.HeartRate.Code)
                                    {
                                        SetPulse(bloodPressure, component);                                        
                                    }
                                }
                                else
                                {
                                    switch (code.Code.ToLowerInvariant())
                                    {
                                        // HeartRate component 
                                        case "8867-4":
                                            SetPulse(bloodPressure, component);
                                            break;
                                        // Systolic LOINC, SNOMED, ACME codes
                                        case "8480-6":
                                        case "271649006":
                                        case "bp-s":
                                            SetSystolic(bloodPressure, component);
                                            break;
                                        // Diastolic LOINC code
                                        case "8462-4":
                                            SetDiastolic(bloodPressure, component);
                                            break;
                                        default:
                                            continue;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
                    
            return bloodPressure;
        }

        private static void SetPulse(BloodPressure bloodPressure, Observation.ComponentComponent component)
        {
            var pulse = ObservationToHealthVault.GetValueFromQuantity(component.Value as Quantity);
            if (pulse.HasValue)
            {
                bloodPressure.Pulse = (int)pulse.Value;
            }
        }

        private static void SetSystolic(BloodPressure bloodPressure, Observation.ComponentComponent component)
        {
            var systolic = ObservationToHealthVault.GetValueFromQuantity(component.Value as Quantity);
            if (systolic.HasValue)
            {
                bloodPressure.Systolic = (int)systolic.Value;
            }            
        }

        private static void SetDiastolic(BloodPressure bloodPressure, Observation.ComponentComponent component)
        {
            var diastolic = ObservationToHealthVault.GetValueFromQuantity(component.Value as Quantity);
            if (diastolic.HasValue)
            {
                bloodPressure.Diastolic = (int)diastolic.Value;
            }            
        }        
    }
}
