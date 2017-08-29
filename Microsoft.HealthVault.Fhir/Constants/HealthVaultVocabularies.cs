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

namespace Microsoft.HealthVault.Fhir.Constants
{
    /// <summary>
    /// This class defines easy to use HealthVault vocabs as codeable concepts
    /// </summary>
    public static class HealthVaultVocabularies
    {
        public const string BaseUri = "http://healthvault.com/fhir/stu3/ValueSet/";

        // Vocab families
        public const string Fhir = "fhir";
        public const string Wc = "wc";
        public const string RxNorm = "RxNorm";
        public const string Dmd = "dmd";

        public const string HealthVaultCodedValueFormat = "{0}:{1}";

        public const string VitalStatistics = "vital-statistics";
        public const string ThingTypeNames = "thing-type-names";
        public const string BloodGlucoseMeasurementContext = BaseUri + "glucose-measurement-context";
        public const string BloodGlucoseMeasurementType = "glucose-measurement-type";

        public const string BodyCompositionMeasurementMethods = "body-composition-measurement-methods";
        public const string BodyCompositionSites = "body-composition-sites";
        public const string BodyCompositionMeasurementNames = "body-composition-measurement-names";

        public const string BodyDimensionMeasurementNames = "body-dimension-measurement-names";

        public const string Exercise = "exercise";
        public const string ExerciseActivity = "exercise-activity";
        public const string ExerciseDistance = "exercise-distance";
        public const string ExerciseDuration = "exercise-duration";

        public const string SleepJournalAM = "sleep-journal-am";
        public const string SleepJournalAMBedtime = "sleep-journal-bed-time";
        public const string SleepJournalAMWaketime = "sleep-journal-wake-time";
        public const string SleepJournalAMSleepMinutes = "sleep-journal-sleep-minutes";
        public const string SleepJournalAMSettlingMinutes = "sleep-journal-settling-minutes";
        public const string SleepJournalAMAwakening = "sleep-journal-awakening";
        public const string SleepJournalAMWakeState = "sleep-journal-wake-state";
        public const string SleepJournalAMMedication = "sleep-journal-medication";

        public const string StateFhirExtensionName = BaseUri + "fhir-extensions/thing-state";
        public const string FlagsFhirExtensionName = BaseUri + "fhir-extensions/thing-flags";
        
        public const string OutsideOperatingTemperatureExtensionName = BaseUri + "blood-glucose/outside-operating-temperature";
        public const string ReadingNormalcyExtensionName = BaseUri + "blood-glucose/reading-normalcy";
        public const string IsControlTestExtensionName = BaseUri + "blood-glucose/is-control-test";

        public const string IrregularHeartBeatExtensionName = BaseUri + "vital-signs/blood-pressure/irregular-heartbeat";
     
        public static CodeableConcept BodyWeight = new CodeableConcept()
        {
            Coding = new List<Coding> { HealthVaultVitalStatisticsCodes.BodyWeight }
        };

        public static CodeableConcept BodyHeight = new CodeableConcept()
        {
            Coding = new List<Coding> { HealthVaultVitalStatisticsCodes.BodyHeight }
        };

        public static CodeableConcept HeartRate = new CodeableConcept
        {
            Coding = new List<Coding> { HealthVaultVitalStatisticsCodes.HeartRate }
        };

        public static CodeableConcept BloodPressure = new CodeableConcept
        {
            Coding = new List<Coding>
            {
                HealthVaultVitalStatisticsCodes.BloodPressureSystolic,
                HealthVaultVitalStatisticsCodes.BloodPressureDiastolic
            }
        };

        /// <summary>
        /// Helper to transform a coding into a codeable concept
        /// </summary>
        /// <param name="coding">The coding for the codeable concept</param>
        /// <returns>A codeable concept containing the passed in coding</returns>
        public static CodeableConcept GenerateCodeableConcept(Coding coding)
        {
            return new CodeableConcept{ Coding = new List<Coding> { coding }};
        }

        /// <summary>
        /// Helper to generate the system url for a given vocabulary within the healthvault vocabulary set
        /// </summary>
        /// <param name="vocabularyName">The vocab name to use in the url</param>
        /// <param name="family">Optional family name to include in the url</param>
        /// <returns>A url for the healthvault vocabulary</returns>
        public static string GenerateSystemUrl(string vocabularyName, string family = null)
        {
            if (string.IsNullOrEmpty(family))
            {
                return $"{BaseUri}{vocabularyName}";
            }
            return $"{BaseUri}{family}/{vocabularyName}";
        }

        /// <summary>
        /// Helper to return if the given system contains the healthvault url
        /// </summary>
        /// <param name="system">The system to check</param>
        /// <returns></returns>
        public static bool SystemContainsHealthVaultUrl(string system)
        {
            return system?.IndexOf(BaseUri, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}
