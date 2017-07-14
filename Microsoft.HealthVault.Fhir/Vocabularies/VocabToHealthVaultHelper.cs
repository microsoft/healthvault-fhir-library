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

namespace Microsoft.HealthVault.Fhir.Vocabularies
{
    internal static class VocabToHealthVaultHelper
    {        
        internal static Type DetectHealthVaultTypeFromObservation(Observation observation)
        {
            if (observation.Code != null && observation.Code.Coding != null)
            {
                foreach (var code in observation.Code.Coding)
                {
                    if (!string.IsNullOrWhiteSpace(code.System) && code.CodeElement != null)
                    {
                        switch (code.System.ToLowerInvariant())
                        {
                            case VocabularyUris.SnomedCd:
                                return DetectFromSnomedCd(code.CodeElement.Value);
                            case VocabularyUris.Loinc:
                                return DetectFromLoincCodes(code.CodeElement.Value);
                            case VocabularyUris.HealthVaultVocabulariesUri:
                                return DetectFromHealthVaultCode(code.CodeElement.Value);
                        }
                    }
                }
            }

            throw new NotSupportedException();
        }

        private static Type DetectFromSnomedCd(string code)
        {
            switch (code.ToLowerInvariant())
            {
                case "434912009":
                    return typeof(BloodGlucose);
                case "5037300":
                    return typeof(Height);
            }

            throw new NotSupportedException("The provided code is not supported");
        }

        private static Type DetectFromHealthVaultCode(string code)
        {
            var vocab = code.Split(':');

            if (vocab.Length == 2)
            {
                var vocabName = vocab[0];
                var vocabValue = vocab[1];

                switch (vocabName.ToLowerInvariant())
                {
                    case HealthVaultVocabularies.VitalStatistics:
                        switch (vocabValue.ToLowerInvariant())
                        {
                            case "wgt":
                                return typeof(Weight);
                            case "hgt":
                                return typeof(Height);
                            case "pls":
                                return typeof(HeartRate);
                        }
                        break;
                    case HealthVaultVocabularies.BloodGlucoseMeasurementContext:
                    case HealthVaultVocabularies.BloodGlucoseMeasurementType:
                        return typeof(BloodGlucose);
                }

            }

            throw new NotSupportedException("The provided code is not supported");
        }
        private static Type DetectFromLoincCodes(string code)
        {            
            switch (code)
            {
                case "29463-7":
                    return typeof(Weight);
                case "15074-8":
                    return typeof(BloodGlucose);
                case "8302-2":
                    return typeof(Height);
                case "8867-4":
                    return typeof(HeartRate);
            }

            throw new NotSupportedException("The provided code is not supported");
        }
    }
}
