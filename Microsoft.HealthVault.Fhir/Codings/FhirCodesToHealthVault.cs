// Copyright (c) Get Real Health.  All rights reserved.
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
    /// <summary>
    /// Fhir have CodeableConcept
    /// with a list of codings and/or text representation
    /// Coding have a system, version, code and display
    /// HV have CodableValue 
    /// with a list of codedValue and Text
    /// CodedValue has value, vocabularyName, family and version
    /// HV also have a concept of vocabularyItem
    /// with vocabularyName, version, family, value
    /// displaytext and abbrevationtext
    /// </summary>
    public static class FhirCodesToHealthVault
    {
        public static CodableValue GetCodableValue(this CodeableConcept codeableConcept)
        {
            var codableValue = new CodableValue();
            if (!string.IsNullOrEmpty(codeableConcept.Text))
            {
                codableValue.Text = codeableConcept.Text;
            }

            if (string.IsNullOrEmpty(codableValue.Text))
            {
                IEnumerable<string> displayTexts = codeableConcept.Coding.Select(coding => coding.Display).Distinct();
                if (displayTexts.Any() && displayTexts.Count() == 1)
                {
                    codableValue.Text = displayTexts.Single();
                }
            }

            foreach (var coding in codeableConcept.Coding)
            {
                CodedValue codedValue = GetCodedValue(coding);
                codableValue.Add(codedValue);
            }
            return codableValue;
        }

        public static CodedValue GetCodedValue(this Coding coding)
        {
            var (value, vocabulary) = GetValueVocabularyPair(coding.Code);

            string system = coding.System;
            if (system.Equals(VocabularyUris.HealthVaultVocabulariesUri, StringComparison.OrdinalIgnoreCase))
            {
                system = HealthVaultVocabularies.Wc;
            }

            if (Uri.IsWellFormedUriString(system, UriKind.Absolute)
                && string.IsNullOrEmpty(vocabulary))
            {
                vocabulary = HealthVaultVocabularies.Fhir;
            }

            var version = coding.Version;

            var codedValue = new CodedValue();
            if (!string.IsNullOrEmpty(value))
            {
                codedValue.Value = value;
            }
            if (!string.IsNullOrEmpty(vocabulary))
            {
                codedValue.VocabularyName = vocabulary;
            }
            if (!string.IsNullOrEmpty(system))
            {
                codedValue.Family = system;
            }
            if (!string.IsNullOrEmpty(version))
            {
                codedValue.Version = version;
            }
            return codedValue;
        }

        private static (string value, string vocabulary) GetValueVocabularyPair(string code)
        {
            var vocabularyValuePair = code.Split(':');
            if (vocabularyValuePair.Count() == 2)
                return (vocabularyValuePair[1], vocabularyValuePair[0]);
            return (code, null);
        }

        internal static CodableValue GetRecurrenceIntervalFromPeriodUnit(Timing.UnitsOfTime period)
        {
            switch (period)
            {
                case Timing.UnitsOfTime.S:
                    return HealthVaultRecurrenceIntervalCodes.Second;
                case Timing.UnitsOfTime.Min:
                    return HealthVaultRecurrenceIntervalCodes.Minute;
                case Timing.UnitsOfTime.H:
                    return HealthVaultRecurrenceIntervalCodes.Hour;
                case Timing.UnitsOfTime.D:
                    return HealthVaultRecurrenceIntervalCodes.Day;
                case Timing.UnitsOfTime.Wk:
                    return HealthVaultRecurrenceIntervalCodes.Week;
                case Timing.UnitsOfTime.Mo:
                    return HealthVaultRecurrenceIntervalCodes.Month;
                case Timing.UnitsOfTime.A:
                    return HealthVaultRecurrenceIntervalCodes.Year;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
