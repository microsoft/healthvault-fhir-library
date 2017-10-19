// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;

namespace Microsoft.HealthVault.Fhir.Codes.HealthVault
{
    /// <summary>
    /// This class is used to define the codeable values related to HealthVault Vital Statistics
    /// </summary>
    public static class HealthVaultVitalStatisticsCodes
    {
        private static string System = $"{HealthVaultVocabularies.BaseUri}{HealthVaultVocabularies.Wc}/{HealthVaultVocabularies.VitalStatistics}";

        public const string BodyWeightCode = "wgt";
        public static readonly Coding BodyWeight = new Coding
        {
            Code = BodyWeightCode,
            Version = "1",
            System = System,
            Display = "Body Weight",
        };

        public const string BodyHeightCode = "hgt";
        public static readonly Coding BodyHeight = new Coding
        {
            Code = BodyHeightCode,
            Version = "1",
            System = System,
            Display = "Body Height",
        };

        public const string HeartRateCode = "pls";
        public static readonly Coding HeartRate = new Coding
        {
            Code = HeartRateCode,
            Version = "1",
            System = System,
            Display = "Heart Rate"
        };

        public const string BloodPressureDiastolicCode = "bpd";
        public static readonly Coding BloodPressureDiastolic = new Coding
        {
            Code = BloodPressureDiastolicCode,
            Version = "1",
            System = System,
            Display = " Blood Pressure - Diastolic",
        };

        public const string BloodPressureSystolicCode = "bps";
        public static readonly Coding BloodPressureSystolic = new Coding
        {
            Code = BloodPressureSystolicCode,
            Version = "1",
            System = System,
            Display = " Blood Pressure - Systolic",
        };

        public const string VitalSignsCode = "vitals-panel";
        public static readonly Coding VitalSigns = new Coding
        {
            Code = VitalSignsCode,
            Version = "1",
            System = System,
            Display = "Vital Signs Panel"
        };
    }
}
