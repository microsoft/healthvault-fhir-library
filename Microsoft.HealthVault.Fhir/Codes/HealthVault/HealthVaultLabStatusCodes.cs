// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Codes.HealthVault
{
    public static class HealthVaultLabStatusCodes
    {
        public const string CompleteCode = "C";
        public static CodableValue Complete => GetLabStatus(CompleteCode);

        public const string PendingCode = "P";
        public static CodableValue Pending => GetLabStatus(PendingCode);

        public const string PatientRefusedTestCode = "R";
        public static CodableValue PatientRefusedTest => GetLabStatus(PatientRefusedTestCode);

        public const string QuantityNotSufficientCode = "QNS";
        public static CodableValue QuantityNotSufficient => GetLabStatus(QuantityNotSufficientCode);

        private static CodableValue GetLabStatus(string code)
        {
            return new CodableValue(LabStatusText[code],
               code: code,
               family: HealthVaultVocabularies.Wc,
               vocabularyName: HealthVaultVocabularies.LabStatus,
               version: "1");
        }

        private static Dictionary<string, string> LabStatusText = new Dictionary<string, string>
        {
            [CompleteCode] = "Complete",
            [PendingCode] = "Pending",
            [PatientRefusedTestCode] = "Patient Refused Test",
            [QuantityNotSufficientCode] = "Quantity not sufficient"
        };
    }
}
