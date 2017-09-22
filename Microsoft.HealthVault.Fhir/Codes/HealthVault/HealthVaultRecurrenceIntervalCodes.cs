// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Codes.HealthVault
{
    public static class HealthVaultRecurrenceIntervalCodes
    {
        public const string SecondCode = "second";

        public static CodableValue Second => GetRecurrenceCode(SecondCode);

        public const string MinuteCode = "minute";

        public static CodableValue Minute => GetRecurrenceCode(MinuteCode);

        public const string HourCode = "hour";

        public static CodableValue Hour => GetRecurrenceCode(HourCode);

        public const string DayCode = "day";

        public static CodableValue Day => GetRecurrenceCode(DayCode);

        public const string WeekCode = "week";

        public static CodableValue Week => GetRecurrenceCode(WeekCode);

        public const string MonthCode = "month";

        public static CodableValue Month => GetRecurrenceCode(MonthCode);

        public const string YearCode = "year";

        public static CodableValue Year => GetRecurrenceCode(YearCode);

        public static CodableValue GetRecurrenceCode(string code)
        {
            return new CodableValue(code,
                code: code,
                family: HealthVaultVocabularies.RecurrenceIntervals,
                vocabularyName: HealthVaultVocabularies.Wc,
                version: "1");
        }
    }
}
