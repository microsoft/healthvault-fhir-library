// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Codings;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static Observation ToFhir(this SleepJournalAM sleepJournalAM)
        {
            return SleepJournalAMToFhir.ToFhirInternal(sleepJournalAM, ThingBaseToFhir.ToFhirInternal(sleepJournalAM));
        }
    }

    /// <summary>
    /// An extension class that transforms HealthVault sleep journal am data types into FHIR Observations
    /// </summary>
    internal static class SleepJournalAMToFhir
    {
        internal static Observation ToFhirInternal(SleepJournalAM sleepJournalAM, Observation observation)
        {
            observation.Category = new List<CodeableConcept>() { FhirCategories.VitalSigns };

            observation.Code = new CodeableConcept(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAM);
            
            observation.Effective = new FhirDateTime(sleepJournalAM.When.ToDateTime());

            observation.Component = new List<Observation.ComponentComponent>();

            var bedtimeComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMBedtime) } },
                Value = sleepJournalAM.Bedtime.ToFhir()
            };
            observation.Component.Add(bedtimeComponent);

            var waketimeComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMWaketime) } },
                Value = sleepJournalAM.WakeTime.ToFhir()
            };
            observation.Component.Add(waketimeComponent);

            var sleepMinutesComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMSleepMinutes) } },
                Value = new Quantity(sleepJournalAM.SleepMinutes, "min")
            };
            observation.Component.Add(sleepMinutesComponent);

            var settlingMinutesComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMSettlingMinutes) } },
                Value = new Quantity(sleepJournalAM.SettlingMinutes, "min")
            };
            observation.Component.Add(settlingMinutesComponent);

            if(sleepJournalAM.Awakenings != null)
            {
                foreach(var awakening in sleepJournalAM.Awakenings)
                {
                    var dummyDateTimeStart = new DateTime(1900,01,01);
                    dummyDateTimeStart = dummyDateTimeStart.AddHours(awakening.When.Hour);
                    dummyDateTimeStart = dummyDateTimeStart.AddMinutes(awakening.When.Minute);
                    if (awakening.When.Second.HasValue)
                    {
                        dummyDateTimeStart = dummyDateTimeStart.AddSeconds(awakening.When.Second.Value);
                    }

                    if (awakening.When.Millisecond.HasValue)
                    {
                        dummyDateTimeStart = dummyDateTimeStart.AddSeconds(awakening.When.Millisecond.Value);
                    }

                    var dummyDateTimeEnd = dummyDateTimeStart;
                    dummyDateTimeEnd = dummyDateTimeEnd.AddMinutes(awakening.Minutes);

                    var awakeningComponent = new Observation.ComponentComponent
                    {
                        Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMAwakening) } },
                        Value = new Period(new FhirDateTime(dummyDateTimeStart), new FhirDateTime(dummyDateTimeEnd))
                    };
                    observation.Component.Add(awakeningComponent);
                }
            }

            var wakeStateComponent = new Observation.ComponentComponent
            {
                Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMWakeState) } },
                Value = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, sleepJournalAM.WakeState.ToString()) } }
            };
            observation.Component.Add(wakeStateComponent);

            if(sleepJournalAM.Medications != null)
            {
                var medicationComponenet = new Observation.ComponentComponent
                {
                    Code = new CodeableConcept { Coding = new List<Coding> { new Coding(VocabularyUris.HealthVaultVocabulariesUri, HealthVaultVocabularies.SleepJournalAMMedication) } },
                    Value = new CodeableConcept { Coding = HealthVaultCodesToFhir.ConvertCodableValueToFhir(sleepJournalAM.Medications, null) }
                };
                observation.Component.Add(medicationComponenet);
            }

            return observation;
        }
    }
}
