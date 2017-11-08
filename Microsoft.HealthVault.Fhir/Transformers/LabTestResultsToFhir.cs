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
using Microsoft.HealthVault.Fhir.FhirExtensions;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.ItemTypes;
using FhirOrganization = Hl7.Fhir.Model.Organization;
using HVOrganization = Microsoft.HealthVault.ItemTypes.Organization;

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
            SetIssued(diagnosticReport, labTestResults.When);

            foreach (LabTestResultGroup labTestResultGroup in labTestResults.Groups)
            {
                AddLabTestResultGroup(diagnosticReport, labTestResultGroup);
            }

            AddOrderedBy(diagnosticReport, labTestResults.OrderedBy);

            return diagnosticReport;
        }

        private static void AddLabTestResultGroup(DiagnosticReport diagnosticReport,
            LabTestResultGroup labTestResultGroup)
        {
            var observation = new Observation
            {
                Id = "obs" + Guid.NewGuid()
            };
            ToFhirInternal(labTestResultGroup, observation);
            diagnosticReport.AddDomainResourceToContainer(observation);
            diagnosticReport.Result.Add(observation.GetContainerReference());
        }

        private static void ToFhirInternal(LabTestResultGroup labTestResultGroup, Observation observation)
        {
            if (labTestResultGroup.GroupName == null)
            {
                throw new ArgumentException($"{nameof(LabTestResultGroup)} should " +
                    $"have a {nameof(LabTestResultGroup.GroupName)}");
            }
            observation.Code = labTestResultGroup.GroupName.ToFhir();

            if (labTestResultGroup.LaboratoryName != null)
            {
                FhirOrganization fhirOrganization = labTestResultGroup.LaboratoryName.ToFhir();
                observation.Contained.Add(fhirOrganization);
                observation.Performer.Add(fhirOrganization.GetContainerReference());
            }

            observation.Status = GetStatus(labTestResultGroup.Status);

            foreach (var subGroup in labTestResultGroup.SubGroups)
            {
                var subObservation = new Observation() { Id = "subObs" + Guid.NewGuid() };
                ToFhirInternal(subGroup, subObservation);
                observation.AddDomainResourceToContainer(subObservation);
                observation.Related.Add(new Observation.RelatedComponent
                {
                    Type = Observation.ObservationRelationshipType.HasMember,
                    Target = subObservation.GetContainerReference()
                });
            }

            foreach (var resultDetail in labTestResultGroup.Results)
            {
                var resultObservation = new Observation() { Id = "resultObs" + Guid.NewGuid() };
                ToFhirInternal(resultDetail, resultObservation);
                observation.AddDomainResourceToContainer(resultObservation);
                observation.Related.Add(new Observation.RelatedComponent
                {
                    Type = Observation.ObservationRelationshipType.DerivedFrom,
                    Target = resultObservation.GetContainerReference()
                });
            }
        }

        private static void ToFhirInternal(LabTestResultDetails resultDetail, Observation resultObservation)
        {
            resultObservation.Effective = resultDetail.When?.ToFhir();

            if (!string.IsNullOrEmpty(resultDetail.Name))
            {
                resultObservation.SetStringExtension(HealthVaultExtensions.LabTestResultName, resultDetail.Name);
            }

            if (resultDetail.Substance != null || resultDetail.CollectionMethod != null)
            {
                var specimen = new Specimen
                {
                    Id = "specimen" + Guid.NewGuid(),
                    Type = resultDetail.Substance?.ToFhir(),
                    Collection = resultDetail.CollectionMethod == null ? null : new Specimen.CollectionComponent
                    {
                        Method = resultDetail.CollectionMethod.ToFhir()
                    }
                };
                resultObservation.AddDomainResourceToContainer(specimen);
                resultObservation.Specimen = specimen.GetContainerReference();
            }

            if (resultDetail.ClinicalCode != null)
            {
                resultObservation.Code = resultDetail.ClinicalCode.ToFhir();
            }

            if (resultDetail.Value != null)
            {
                AddLabTestResultValue(resultDetail.Value, resultObservation);                
            }

            if (resultDetail.Status != null)
            {
                resultObservation.Status = GetStatus(resultDetail.Status);
            }

            resultObservation.Comment = resultDetail.Note;
        }

        private static void AddLabTestResultValue(LabTestResultValue labTestResultValue, Observation resultObservation)
        {
            resultObservation.Value = new FhirString(labTestResultValue.Measurement.Display);

            foreach (var structuredMesurement in labTestResultValue.Measurement.Structured)
            {
                var quantity = new Quantity
                {
                    Value = (decimal)structuredMesurement.Value,
                    Unit = structuredMesurement.Units.Text
                };
                if (structuredMesurement.Units.Any())
                {
                    var codedUnit = structuredMesurement.Units.First();
                    quantity.Code = codedUnit.Value;
                    quantity.System = HealthVaultVocabularies.GenerateSystemUrl(codedUnit.VocabularyName, codedUnit.Family);
                }
                var valueDetailExtension = new Extension
                {
                    Url = HealthVaultExtensions.LabTestResultValueDetail,
                    Value = quantity
                };
                resultObservation.Value.AddExtension(HealthVaultExtensions.LabTestResultValueDetail, quantity);
            }

            foreach (var range in labTestResultValue.Ranges)
            {

                var rangeComponent = new Observation.ReferenceRangeComponent
                {
                    Type = range.RangeType.ToFhir(),
                    Text = range.Text.Text
                };
                rangeComponent.TextElement.AddExtension(HealthVaultExtensions.LabTestResultValueRangeText, range.Text.ToFhir());

                if (range.Value != null)
                {
                    rangeComponent.Low = GetSimpleQuantity(range.Value.Minimum);
                    rangeComponent.High = GetSimpleQuantity(range.Value.Maximum);

                    SimpleQuantity GetSimpleQuantity(double? value)
                    {
                        return value == null ? null : new SimpleQuantity() { Value = (decimal)value };
                    }
                }

                resultObservation.ReferenceRange.Add(rangeComponent);
            }

            foreach (var flag in labTestResultValue.Flag)
            {
                resultObservation.AddExtension(HealthVaultExtensions.LabTestResultValueFlag, flag.ToFhir());
            }
        }

        private static ObservationStatus? GetStatus(CodableValue status)
        {
            if (status == null)
            {
                return ObservationStatus.Unknown;
            }

            Func<CodedValue, bool> labStatusPredicate =
                coded => coded.VocabularyName == HealthVaultVocabularies.LabStatus;
            if (status.Any(labStatusPredicate))
            {
                CodedValue coded = status.First(labStatusPredicate);
                switch (coded.Value)
                {
                    case HealthVaultLabStatusCodes.CompleteCode:
                        return ObservationStatus.Final;
                    case HealthVaultLabStatusCodes.PendingCode:
                        return ObservationStatus.Registered;
                    case HealthVaultLabStatusCodes.PatientRefusedTestCode:
                        return ObservationStatus.Cancelled;
                    case HealthVaultLabStatusCodes.QuantityNotSufficientCode:
                    default:
                        return ObservationStatus.Unknown;
                }
            }
            return ObservationStatus.Unknown;
        }

        private static void AddOrderedBy(DiagnosticReport diagnosticReport, HVOrganization orderedBy)
        {
            if (orderedBy == null)
            {
                return;
            }

            var orderedByOrganization = orderedBy.ToFhir();
            orderedByOrganization.Id = "org" + Guid.NewGuid();

            var orderedByReferenceExtension = new Extension
            {
                Url = HealthVaultExtensions.LabTestResultOrderBy,
                Value = orderedByOrganization.GetContainerReference()
            };

            diagnosticReport.Contained.Add(orderedByOrganization);
            diagnosticReport.Extension.Add(orderedByReferenceExtension);
        }

        private static void SetIssued(DiagnosticReport diagnosticReport, ApproximateDateTime when)
        {
            diagnosticReport.Issued = when?.ToFhir()?.ToDateTimeOffset();
        }
    }
}
