// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Fhir.Constants
{
    public static class HealthVaultExtensions
    {
        public const string HealthVaultExtensionsUri = "http://healthvault.com/fhir/stu3/StructuredDefinition/";

        public const string StateFhirExtensionName = HealthVaultExtensionsUri + "thing-state";
        public const string FlagsFhirExtensionName = HealthVaultExtensionsUri + "thing-flags";

        public const string BloodGlucose = HealthVaultExtensionsUri + "blood-glucose";
        public const string OutsideOperatingTemperatureExtensionName = "outside-operating-temperature";
        public const string ReadingNormalcyExtensionName = "reading-normalcy";
        public const string IsControlTestExtensionName = "is-control-test";
        public const string BloodGlucoseMeasurementContext = "measurement-context";

        public const string IsPrimary = HealthVaultExtensionsUri + "is-primary";
        public const string Description = HealthVaultExtensionsUri + "description";

        public const string ExerciseDetail = HealthVaultExtensionsUri + "exercise-detail";
        public const string ExerciseDetailName = "name";
        public const string ExerciseDetailType = "type";
        public const string ExerciseDetailValue = "value";

        public const string ExerciseSegment = HealthVaultExtensionsUri + "exercise-segment";
        public const string ExerciseSegmentActivity = "activity";
        public const string ExerciseSegmentTitle = "title";
        public const string ExerciseSegmentDuration = "duration";
        public const string ExerciseSegmentDistance = "distance";
        public const string ExerciseSegmentOffset = "offset";

        public const string PatientBasicV2 = HealthVaultExtensionsUri + "basic-v2";
        public const string PatientBirthYear = "birth-year";
        public const string PatientFirstDayOfWeek = "first-day-of-week";
        public const string PatientBasicAddress = "basic-address";
        public const string PatientBasicAddressCity = "city";
        public const string PatientBasicAddressState = "state";
        public const string PatientBasicAddressPostalCode = "postal-code";
        public const string PatientBasicAddressCountry = "country";

        public const string PatientPersonal = HealthVaultExtensionsUri + "personal";
        public const string PatientBloodType = "blood-type";
        public const string PatientEmploymentStatus = "employment-status";
        public const string PatientEthnicity = "ethnicity";
        public const string PatientHighestEducationLevel = "highest-education-level";
        public const string PatientIsDisabled = "is-disabled";
        public const string PatientIsVeteran = "is-veteran";
        public const string PatientMaritalStatus = "marital-status";
        public const string PatientOrganDonor = "organ-donor";
        public const string PatientReligion = "religion";

        public const string PatientBirthTime = HealthVaultExtensionsUri + "birth-time";
        public const string PatientTitle = "codable-title";
        public const string PatientSuffix = "codable-suffix";
        public const string Condition = HealthVaultExtensionsUri + "condition";
        public const string ConditionOccurrence =  "occurrence";
        public const string ConditionStopReason = "stop-reason";

        public const string Organization = HealthVaultExtensionsUri + "organization";
    }
}
