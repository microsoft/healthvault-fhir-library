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
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.ItemTypes;
using FhirOrganization = Hl7.Fhir.Model.Organization;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class DiagnosticReportToHealthVault
    {
        public static LabTestResults ToHealthVault(this DiagnosticReport diagnosticReport)
        {
            var labTestResults = diagnosticReport.ToThingBase<LabTestResults>();

            if (diagnosticReport.Issued.HasValue)
            {
                labTestResults.When = new FhirDateTime(diagnosticReport.Issued.Value).ToAproximateDateTime();
            }

            var orderByReference = diagnosticReport
                   .GetExtensionValue<ResourceReference>(HealthVaultExtensions.LabTestResultOrderBy);
            if (orderByReference != null)
            {
                var fhirOrganization = diagnosticReport.GetReferencedResource(orderByReference,
                     reference => new FhirOrganization { Name = reference.Display });
                labTestResults.OrderedBy = fhirOrganization.ToHealthVault();
            }

            foreach (var reference in diagnosticReport.Result)
            {
                var labTestResultGroup = GetLabTestResultGroup(diagnosticReport, reference);

                labTestResults.Groups.Add(labTestResultGroup);
            }

            return labTestResults;
        }

        private static LabTestResultGroup GetLabTestResultGroup(DiagnosticReport diagnosticReport, ResourceReference reference)
        {
            var observation = diagnosticReport.GetContainedResource<Observation>(reference);

            var labTestResultGroup = new LabTestResultGroup { };

            labTestResultGroup.GroupName = observation.Code.ToCodableValue();

            if (observation.Performer.Any())
            {
                var fhirOrganization = diagnosticReport.ExtractFirstContainedResource(observation.Performer,
                     () =>
                     {
                         var resource = observation.Performer.FirstOrDefault(performerReference
                                         => string.IsNullOrEmpty(performerReference.Display));
                         if (resource != null)
                         {
                             return new FhirOrganization
                             {
                                 Name = resource.Display
                             };
                         }
                         return null;
                     });
                labTestResultGroup.LaboratoryName = fhirOrganization?.ToHealthVault();
            }

            labTestResultGroup.Status = GetStatus(observation.Status);

            foreach (var relatedObservationLink in observation.Related)
            {
                switch (relatedObservationLink.Type)
                {
                    case null:
                    case Observation.ObservationRelationshipType.HasMember:
                        var labTestResultSubGroup = GetLabTestResultGroup(diagnosticReport, relatedObservationLink.Target);
                        labTestResultGroup.SubGroups.Add(labTestResultSubGroup);
                        break;
                    case Observation.ObservationRelationshipType.DerivedFrom:
                        var labTestResultDetails = GetLabTestResultDetails(diagnosticReport, relatedObservationLink.Target);
                        labTestResultGroup.Results.Add(labTestResultDetails);
                        break;
                    default:
                        break;
                }
            }

            return labTestResultGroup;
        }

        private static LabTestResultDetails GetLabTestResultDetails(DiagnosticReport diagnosticReport, ResourceReference target)
        {
            var detailObservation = diagnosticReport.GetContainedResource<Observation>(target);

            var labTestResultDetails = new LabTestResultDetails
            {
                When = detailObservation.Effective?.ToAproximateDateTime()
            };

            if (detailObservation.HasExtensions(HealthVaultExtensions.LabTestResultName))
            {
                labTestResultDetails.Name = detailObservation.GetStringExtension(HealthVaultExtensions.LabTestResultName);
            }

            if (detailObservation.Specimen != null)
            {
                var specimen = diagnosticReport.GetReferencedResource(detailObservation.Specimen,
                    reference => new Specimen
                    {
                        Type = new CodeableConcept { Text = reference.Display }
                    });

                labTestResultDetails.Substance = specimen.Type?.ToCodableValue();

                labTestResultDetails.CollectionMethod = specimen.Collection?.Method?.ToCodableValue();
            }

            labTestResultDetails.ClinicalCode = detailObservation.Code.ToCodableValue();

            switch (detailObservation.Value)
            {
                case FhirString valueString:
                    var measure = new GeneralMeasurement(valueString.Value);
                    if (detailObservation.Value.HasExtensions(HealthVaultExtensions.LabTestResultValueDetail))
                    {
                        foreach (var detailExtension in detailObservation.Value
                            .GetExtensions(HealthVaultExtensions.LabTestResultValueDetail))
                        {
                            var quantity = detailExtension.Value as Quantity;
                            if (quantity == null)
                            {
                                continue;
                            }
                            if (quantity.Value.HasValue && !string.IsNullOrEmpty(quantity.Unit))
                            {
                                var value = (double)quantity.Value;
                                var units = new CodableValue(quantity.Unit);
                                if (!string.IsNullOrEmpty(quantity.Code)
                                    && !string.IsNullOrEmpty(quantity.System))
                                {
                                    var familyVocab = HealthVaultVocabularies
                                        .ExtractFamilyAndVocabularyFromSystemUrl(quantity.System);
                                    units.Add(new CodedValue(quantity.Code, familyVocab.vocabulary, familyVocab.family, null));
                                }
                                var structuredmeasure = new StructuredMeasurement(value, units);
                                measure.Structured.Add(structuredmeasure);
                            }
                        }
                    }
                    labTestResultDetails.Value = new LabTestResultValue
                    {
                        Measurement = measure
                    };
                    break;
                default: //We are not supportin other types now
                    break;
            }

            foreach (var referenceRange in detailObservation.ReferenceRange)
            {
                if (referenceRange.Type != null &&
                   !string.IsNullOrEmpty(referenceRange.Text))
                {
                    var range = new TestResultRange()
                    {
                        RangeType = referenceRange.Type.ToCodableValue(),
                        Text = new CodableValue(referenceRange.Text)
                    };
                    var textDetail = referenceRange
                        .GetExtensionValue<CodeableConcept>(HealthVaultExtensions.LabTestResultValueRangeText);
                    if (textDetail != null)
                    {
                        range.Text = textDetail.ToCodableValue();
                    }

                    if (referenceRange.Low != null || referenceRange.High != null)
                    {
                        range.Value = new TestResultRangeValue
                        {
                            Minimum = (double?)referenceRange.Low?.Value ?? null,
                            Maximum = (double?)referenceRange.High?.Value ?? null
                        };
                    }

                    labTestResultDetails.Value.Ranges.Add(range);
                }
            }

            foreach (var flagExtension in detailObservation.GetExtensions(HealthVaultExtensions.LabTestResultValueFlag))
            {
                labTestResultDetails.Value.Flag.Add((flagExtension.Value as CodeableConcept)?.ToCodableValue());
            }

            labTestResultDetails.Status = GetStatus(detailObservation.Status);

            labTestResultDetails.Note = detailObservation.Comment;

            return labTestResultDetails;
        }

        private static CodableValue GetStatus(ObservationStatus? status)
        {
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case ObservationStatus.Final:
                        return HealthVaultLabStatusCodes.Complete;
                    case ObservationStatus.Registered:
                        return HealthVaultLabStatusCodes.Pending;
                    case ObservationStatus.Cancelled:
                    case ObservationStatus.Amended:
                    case ObservationStatus.Corrected:
                    case ObservationStatus.EnteredInError:
                    case ObservationStatus.Preliminary:
                    case ObservationStatus.Unknown:
                    default:
                        return null;
                }
            }
            return null;
        }

        private static TResource ExtractFirstContainedResource<TDomainResource, TResource>(this TDomainResource domainResource,
            IEnumerable<ResourceReference> referenceList, Func<TResource> defaultResolver)
            where TResource : DomainResource, new()
            where TDomainResource : DomainResource
        {
            var sample = new TResource();
            foreach (var reference in referenceList)
            {
                if (reference.IsContainedReference)
                {
                    return domainResource.Contained.FirstOrDefault(resource
                        => reference.Matches(resource.GetContainerReference())
                        && resource.ResourceType == sample.ResourceType) as TResource;
                }
            }
            return defaultResolver();
        }
    }
}

