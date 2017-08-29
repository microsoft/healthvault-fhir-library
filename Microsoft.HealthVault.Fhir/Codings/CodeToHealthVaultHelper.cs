// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Codings
{
    internal static class CodeToHealthVaultHelper
    {
        internal static CodableValue CreateCodableValueFromQuantityValues(string system, string code, string unit)
        {
            var segments = system.Replace(VocabularyUris.HealthVaultVocabulariesUri, "").Split('/');

            var vocabName = segments[0];
            var version = segments.Length == 2 ? segments[1] : null;

            return new CodableValue(unit, code, vocabName, HealthVaultVocabularies.Wc, version);
        }

        internal static Type DetectHealthVaultTypeFromObservation(Observation observation)
        {
            if (observation.Code != null && observation.Code.Coding != null)
            {
                foreach (var code in observation.Code.Coding)
                {
                    if (!string.IsNullOrWhiteSpace(code.System) && code.CodeElement != null)
                    {
                        if (code.System.ToLowerInvariant().Contains("healthvault.com"))
                        {
                            var uri = new Uri(code.System.ToLowerInvariant());
                            return DetectFromHealthVaultCode(uri.Segments.Last(), code.CodeElement.Value);
                        }

                        switch (code.System.ToLowerInvariant())
                        {
                            case VocabularyUris.SnomedCd:
                                return DetectFromSnomedCd(code.CodeElement.Value);
                            case VocabularyUris.Loinc:
                                return DetectFromLoincCodes(code.CodeElement.Value);
                        }
                    }
                }
            }

            throw new NotSupportedException();
        }

        private static Type DetectType(Dictionary<string, string> codeDictionary, string code)
        {            
            if (codeDictionary != null && codeDictionary.ContainsKey(code))
            {
                return Type.GetType($"{codeDictionary[code]}, Microsoft.HealthVault");
            }

            throw new NotSupportedException("The provided code is not supported");
        }

        private static Type DetectFromSnomedCd(string code)
        {
            return DetectType(CodeToHealthVaultDictionaries.Instance.Snomed, code);
        }

        private static Type DetectFromLoincCodes(string code)
        {
            return DetectType(CodeToHealthVaultDictionaries.Instance.Loinc, code);
        }

        private static Type DetectFromHealthVaultCode(string vocabName, string code)
        {

            switch (vocabName.ToLowerInvariant())
            {
                case HealthVaultVocabularies.VitalStatistics:
                    switch (code.ToLowerInvariant())
                    {
                        case "wgt":
                            return typeof(Weight);
                        case "hgt":
                            return typeof(Height);
                        case "pls":
                            return typeof(HeartRate);
                        case "bpd":
                        case "bps":
                            return typeof(BloodPressure);
                    }

                    break;
                case HealthVaultVocabularies.BloodGlucoseMeasurementContext:
                case HealthVaultVocabularies.BloodGlucoseMeasurementType:
                    return typeof(BloodGlucose);

                case HealthVaultVocabularies.ThingTypeNames:
                    switch (code)
                    {
                        case HealthVaultThingTypeNameCodes.ExerciseCode:
                            return typeof(Exercise);
                        case HealthVaultThingTypeNameCodes.SleepJournalAMCode:
                            return typeof(SleepJournalAM);
                        case HealthVaultThingTypeNameCodes.BodyCompositionCode:
                            return typeof(BodyComposition);
                        case HealthVaultThingTypeNameCodes.BodyDimensionCode:
                            return typeof(BodyDimension);;
                    }

                    break;
            }

            throw new NotSupportedException("The provided code is not supported");
        }
    }
}
