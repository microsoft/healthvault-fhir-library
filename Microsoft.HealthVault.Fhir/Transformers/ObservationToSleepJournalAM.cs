// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToSleepJournalAm
    {
        internal static SleepJournalAM ToSleepJournalAM(this Observation observation)
        {
            var sleepJournalAm = observation.ToThingBase<SleepJournalAM>();
            
            sleepJournalAm.When = ObservationToHealthVault.GetWhenFromEffective(observation.Effective);

            foreach (var component in observation.Component)
            {
                if (string.IsNullOrEmpty(component.Code?.Text) || component.Value == null)
                {
                    continue;
                }

                switch (component.Code.Text)
                {
                    case HealthVaultVocabularies.SleepJournalAMBedtime:
                        if(component.Value.GetType() == typeof(Time))
                        { 
                            sleepJournalAm.Bedtime = ((Time)component.Value).ToAppoximateTime();
                        }

                        break;
                    case HealthVaultVocabularies.SleepJournalAMWaketime:
                        if (component.Value.GetType() == typeof(Time))
                        {
                            sleepJournalAm.WakeTime = ((Time)component.Value).ToAppoximateTime();
                        }

                        break;
                    case HealthVaultVocabularies.SleepJournalAMSleepMinutes:
                        if (component.Value.GetType() == typeof(SimpleQuantity))
                        {
                            var sleepMinutes = (Quantity)component.Value;
                            if (sleepMinutes.Value.HasValue)
                            {
                                sleepJournalAm.SleepMinutes = (int)sleepMinutes.Value.Value;
                            }
                        }

                        break;
                    case HealthVaultVocabularies.SleepJournalAMSettlingMinutes:
                        if (component.Value.GetType() == typeof(SimpleQuantity))
                        {
                            var settlingMinutes = (Quantity)component.Value;
                            if (settlingMinutes.Value.HasValue)
                            {
                                sleepJournalAm.SettlingMinutes = (int)settlingMinutes.Value.Value;
                            }
                        }

                        break;
                    case HealthVaultVocabularies.SleepJournalAMWakeState:
                        if (component.Value.GetType() == typeof(FhirString))
                        {
                            WakeState wakeState;
                            var wakeStateValue = (FhirString)component.Value;

                            if (Enum.TryParse(wakeStateValue.Value, out wakeState))
                            {
                                sleepJournalAm.WakeState = wakeState;
                            }
                        }

                        break;
                    case HealthVaultVocabularies.SleepJournalAMMedication:
                        if (component.Value.GetType() == typeof(CodeableConcept))
                        {

                            sleepJournalAm.Medications = ((CodeableConcept)component.Value).ToCodableValue();
                        }

                        break;
                    case HealthVaultVocabularies.SleepJournalAMAwakening:
                        if (component.Value.GetType() == typeof(Period))
                        {
                            var startTime = ((Period)component.Value).StartElement.ToDateTimeOffset();
                            var endTime = ((Period)component.Value).EndElement.ToDateTimeOffset();

                            var approximateTime = new ApproximateTime
                            {
                                Hour = startTime.Hour,
                                Minute = startTime.Minute,
                                Second = startTime.Second,
                                Millisecond = startTime.Millisecond
                            };

                            var occurrence = new Occurrence
                            {
                                When = approximateTime,
                                Minutes = (endTime - startTime).Minutes
                            };

                            sleepJournalAm.Awakenings.Add(occurrence);
                        }

                        break;
                }
            }

            return sleepJournalAm;
        }
    }
}
