// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;

namespace Microsoft.HealthVault.Fhir.Codes.HealthVault
{
    /// <summary>
    /// This class is used to define the codeable values related to HealthVault thing types
    /// </summary>
    public static class HealthVaultThingTypeNameCodes
    {
        public const string BloodGlucoseCode = "txtNameblood-glucose";
        public static readonly Coding BloodGlucose = new Coding
        {
            Code = BloodGlucoseCode,
            Version = "1",
            System = HealthVaultVocabularies.GenerateSystemUrl(HealthVaultVocabularies.ThingTypeNames, HealthVaultVocabularies.Wc),
            Display = "Blood glucose",
        };

        public const string BodyCompositionCode = "txtNamebody-composition";
        public static readonly Coding BodyComposition = new Coding
        {
            Code = BodyCompositionCode,
            Version = "1",
            System = HealthVaultVocabularies.GenerateSystemUrl(HealthVaultVocabularies.ThingTypeNames, HealthVaultVocabularies.Wc),
            Display = "Body composition",
        };

        public const string BodyDimensionCode = "txtNamebody-dimension";
        public static readonly Coding BodyDimension = new Coding
        {
            Code = BodyDimensionCode,
            Version = "1",
            System = HealthVaultVocabularies.GenerateSystemUrl(HealthVaultVocabularies.ThingTypeNames, HealthVaultVocabularies.Wc),
            Display = "Body dimension",
        };

        public const string ExerciseCode = "txtNameexercise";
        public static readonly Coding Exercise = new Coding
        {
            Code = ExerciseCode,
            Version = "1",
            System = HealthVaultVocabularies.GenerateSystemUrl(HealthVaultVocabularies.ThingTypeNames, HealthVaultVocabularies.Wc),
            Display = "Exercise",
        };

        public const string SleepJournalAMCode = "txtNamesleepjournal-am";
        public static readonly Coding SleepJournalAM = new Coding
        {
            Code = SleepJournalAMCode,
            Version = "1",
            System = HealthVaultVocabularies.GenerateSystemUrl(HealthVaultVocabularies.ThingTypeNames, HealthVaultVocabularies.Wc),
            Display = "	Sleep session",
        };
    }
}
