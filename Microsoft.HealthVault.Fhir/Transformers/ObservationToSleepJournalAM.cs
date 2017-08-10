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
    internal static class ObservationToSleepJournalAm
    {
        internal static SleepJournalAM ToSleepJournalAM(this Observation observation)
        {
            var sleepJournalAm = observation.ToThingBase<SleepJournalAM>();

            sleepJournalAm.When = ObservationToHealthVault.GetHealthVaultTimeFromEffectiveDate(observation.Effective);

            foreach(var component in observation.Component)
            {
                switch (component.Code.Coding[0].Code)
                {
                    case HealthVaultVocabularies.SleepJournalAMBedtime:
                        sleepJournalAm.Bedtime = ((Time)component.Value).ToAppoximateTime();
                        break;
                    case HealthVaultVocabularies.SleepJournalAMWaketime:
                        sleepJournalAm.WakeTime = ((Time)component.Value).ToAppoximateTime();
                        break;
                    case HealthVaultVocabularies.SleepJournalAMSleepMinutes:
                        var sleepMinutes = ((Quantity)component.Value);
                        if (sleepMinutes.Value.HasValue)
                        {
                            sleepJournalAm.SleepMinutes = (int)sleepMinutes.Value.Value;
                        }
                        break;
                    case HealthVaultVocabularies.SleepJournalAMSettlingMinutes:
                        var settlingMinutes = ((Quantity)component.Value);
                        if (settlingMinutes.Value.HasValue)
                        {
                            sleepJournalAm.SettlingMinutes = (int)settlingMinutes.Value.Value;
                        }
                        break;
                    case HealthVaultVocabularies.SleepJournalAMWakeState:
                        WakeState wakeState;
                        if(Enum.TryParse(((CodeableConcept)component.Value).Coding[0].Code, out wakeState))
                        {
                            sleepJournalAm.WakeState = wakeState;
                        }
                        break;
                    case HealthVaultVocabularies.SleepJournalAMMedication:
                        
                        var coding = ((CodeableConcept)component.Value).Coding[0];

                        var value = coding.Code.Split(':');
                        var vocabName = value[0];
                        var vocabCode = value.Length == 2 ? value[1] : null;

                        var codedValue = new CodedValue
                        {
                            VocabularyName = vocabName,
                            Value = vocabCode,
                            Family = HealthVaultVocabularies.Medication,
                            Version = coding.Version
                        };

                        if (sleepJournalAm.Medications == null)
                        {
                            sleepJournalAm.Medications = new CodableValue(coding.Display);
                        }

                        sleepJournalAm.Medications.Add(codedValue);
                        break;
                    case HealthVaultVocabularies.SleepJournalAMAwakening:
                        var startTime = ((Period)component.Value).StartElement.ToDateTimeOffset();
                        var endTime = ((Period)component.Value).EndElement.ToDateTimeOffset();

                        var approximateTime = new ApproximateTime
                        {
                            Hour = startTime.Hour,
                            Minute = startTime.Minute,
                            Second = startTime.Second,
                            Millisecond = startTime.Millisecond
                        };

                        var occurance = new Occurrence
                        {
                            When = approximateTime,
                            Minutes = (endTime - startTime).Minutes
                        };

                        sleepJournalAm.Awakenings.Add(occurance);
                        break;
                }
            }

            return sleepJournalAm;
        }
    }
}
