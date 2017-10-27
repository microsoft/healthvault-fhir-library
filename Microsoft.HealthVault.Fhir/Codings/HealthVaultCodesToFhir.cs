// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.HealthVault.Fhir.UnitTests")]

namespace Microsoft.HealthVault.Fhir.Codings
{
    internal class HealthVaultCodesToFhir
    {
        internal static Dosage GetDosage(GeneralMeasurement dose, GeneralMeasurement frequency, CodableValue route)
        {
            var dosage = new Dosage();

            if (route != null)
            {
                dosage.Route = route.ToFhir();
            }

            if (frequency != null)
            {
                dosage.Text = frequency.Display;

                dosage.Timing = GetTiming(frequency);
            }

            if (dose != null)
            {
                dosage.Text = string.Join(Environment.NewLine, dose.Display, dosage.Text);

                dosage.Dose = GetSimpleQuantity(dose);
            }

            return dosage;
        }

        private static Timing GetTiming(GeneralMeasurement frequency)
        {
            if (frequency.Structured.Any())
            {
                StructuredMeasurement frequencyMeasurement = frequency.Structured.First();

                var repeatComponent = new Timing.RepeatComponent();
                repeatComponent.Period = new decimal(frequencyMeasurement.Value);
                repeatComponent.PeriodUnit = GetPeriodUnitFromFromRecurrenceIntervals(frequencyMeasurement.Units);

                return new Timing()
                {
                    Repeat = repeatComponent
                };
            }
            return null;
        }

        public static SimpleQuantity GetSimpleQuantity(GeneralMeasurement measurement)
        {
            if (measurement?.Structured.Any() == true)
            {
                StructuredMeasurement structuredMeasurement = measurement.Structured.First();

                var simpleQuantity = new SimpleQuantity()
                {
                    Value = new decimal(structuredMeasurement.Value),
                    Unit = structuredMeasurement.Units.Text
                };

                if (structuredMeasurement.Units.Any())
                {
                    CodedValue measurementUnit = structuredMeasurement.Units.First();

                    simpleQuantity.Code = measurementUnit.Value;
                    simpleQuantity.System = HealthVaultVocabularies.GenerateSystemUrl(measurementUnit.VocabularyName, measurementUnit.Family);
                }

                return simpleQuantity;
            }
            return null;
        }

        private static Timing.UnitsOfTime? GetPeriodUnitFromFromRecurrenceIntervals(CodableValue units)
        {
            Func<CodedValue, bool> recurrenceIntervalsPredicate =
                coded => coded.VocabularyName == HealthVaultVocabularies.RecurrenceIntervals;
            if (units.Any(recurrenceIntervalsPredicate))
            {
                var coded = units.First(recurrenceIntervalsPredicate);
                switch (coded.Value)
                {
                    case HealthVaultRecurrenceIntervalCodes.SecondCode:
                        return Timing.UnitsOfTime.S;
                    case HealthVaultRecurrenceIntervalCodes.MinuteCode:
                        return Timing.UnitsOfTime.Min;
                    case HealthVaultRecurrenceIntervalCodes.HourCode:
                        return Timing.UnitsOfTime.H;
                    case HealthVaultRecurrenceIntervalCodes.DayCode:
                        return Timing.UnitsOfTime.D;
                    case HealthVaultRecurrenceIntervalCodes.WeekCode:
                        return Timing.UnitsOfTime.Wk;
                    case HealthVaultRecurrenceIntervalCodes.MonthCode:
                        return Timing.UnitsOfTime.Mo;
                    case HealthVaultRecurrenceIntervalCodes.YearCode:
                        return Timing.UnitsOfTime.A;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
