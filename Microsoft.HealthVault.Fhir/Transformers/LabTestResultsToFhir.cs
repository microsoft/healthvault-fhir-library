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
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        // Register the type on the generic ThingToFhir partial class
        public static DiagnosticReport ToFhir(this LabTestResults labTestResults)
        {
            return LabTestResultsToFhir.ToFhirInternal(labTestResults, ToFhirInternal<DiagnosticReport>(labTestResults));
        }
    }

    internal static class LabTestResultsToFhir
    {
        internal static DiagnosticReport ToFhirInternal(LabTestResults labTestResults, DiagnosticReport diagnosticReport)
        {
            ////labTestResults.When
            SetIssued(diagnosticReport, labTestResults.When);
            ////*+labTestResults.Groups
            //var grp = labTestResults.Groups.First();
            ////*grp.GroupName
            ////grp.LaboratoryName
            ////grp.Status
            ////+grp.SubGroups -=> grp 
            ////+grp.Results
            //var result = grp.Results.First();
            ////result.When
            ////result.Name
            ////result.Substance
            ////result.CollectionMethod
            ////result.ClinicalCode
            //var value = result.Value;
            ////*value.Measurement
            ////+value.Ranges
            //var range = value.Ranges.First();
            ////*range.RangeType
            ////*range.Text
            ////range.Value
            //var rValue = range.Value;
            ////rValue.Minimum
            ////rValue.Maximum
            ////+value.Flag
            ////result.Status
            ////result.Note
            ////labTestResults.OrderedBy


            //throw new NotImplementedException();
            return diagnosticReport;
        }

        private static void SetIssued(DiagnosticReport diagnosticReport, ApproximateDateTime when)
        {
            diagnosticReport.Issued = when.ToFhir().ToDateTimeOffset();
        }
    }
}
