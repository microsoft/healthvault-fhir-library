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
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using FhirOrganization = Hl7.Fhir.Model.Organization;
using HVOrganization = Microsoft.HealthVault.ItemTypes.Organization;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(LabTestResults))]
    public class LabTestResultsToFhirTests
    {
        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenWhenIsCopiedToIssued()
        {
            var resultDateTime = new LocalDateTime(2017, 12, 12, 12, 12);
            var labTestReport = new LabTestResults()
            {
                When = new ApproximateDateTime(resultDateTime)
            };

            var diagnosticReport = labTestReport.ToFhir();

            Assert.AreEqual(resultDateTime.ToDateTimeUnspecified(), diagnosticReport.Issued);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenOrderByIsCopiedAsAnExtension()
        {
            const string name = "Dr. Roger Jones";
            var labTestReport = new LabTestResults()
            {
                OrderedBy = new HVOrganization(name)
            };

            var diagnosticReport = labTestReport.ToFhir();

            Assert.IsTrue(diagnosticReport.HasExtensions(HealthVaultExtensions.LabTestResultOrderBy),
                "OrderBy organization reference missing");

            var organization = ExtractOrderByOrganisation(diagnosticReport);

            Assert.AreEqual(name, organization?.Name);
        }

        private FhirOrganization ExtractOrderByOrganisation(DiagnosticReport diagnosticReport)
        {
            var resourceReference = diagnosticReport
                   .GetExtensionValue<ResourceReference>(HealthVaultExtensions.LabTestResultOrderBy);
            return diagnosticReport.GetReferencedResource<FhirOrganization>(resourceReference,
                reference => throw new AssertFailedException("OrderBy organization missing"));
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabGroupsAreAddedAsObservations()
        {
            var labTestReport = new LabTestResults();
            int groupsNumber = 3;
            for (int i = 0; i < groupsNumber; i++)
            {
                labTestReport.Groups.Add(new LabTestResultGroup(new CodableValue(text: "lab" + i)));
            }

            var diagnosticReport = labTestReport.ToFhir();

            Assert.AreEqual(groupsNumber, diagnosticReport.Contained.Count);
            Assert.AreEqual(groupsNumber, diagnosticReport.Result.Count);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabGroupNameIsCopiedAsObservationCode()
        {
            var labTestReport = new LabTestResults();
            const string panelName = "lab panel";
            labTestReport.Groups.Add(new LabTestResultGroup
            {
                GroupName = new CodableValue(panelName)
            });

            var diagnosticReport = labTestReport.ToFhir();
            var observation = ExtractResultObservation(diagnosticReport);

            Assert.AreEqual(panelName, observation.Code.Text);
        }

        private Observation ExtractResultObservation(DiagnosticReport diagnosticReport)
        {
            return ExtractFirstReferencedResource<DiagnosticReport, Observation>(diagnosticReport,
                domainResource => domainResource.Result,
                domainResource
                    => throw new AssertFailedException($"{nameof(Observation)} not found " +
                    $"in {diagnosticReport}"));
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabGroupLaboratoryIsCopiedAsPerformer()
        {
            var labTestReport = new LabTestResults();
            const string laboratoryName = "Fabrikam analysis";
            labTestReport.Groups.Add(new LabTestResultGroup
            {
                GroupName = new CodableValue("Lab"),
                LaboratoryName = new ItemTypes.Organization
                {
                    Name = laboratoryName
                }
            });

            var diagnosticReport = labTestReport.ToFhir();
            var organisation = ExtractPerformerOrganisation(diagnosticReport);

            Assert.AreEqual(laboratoryName, organisation?.Name);
        }

        private FhirOrganization ExtractPerformerOrganisation(DiagnosticReport diagnosticReport)
        {
            return ExtractFirstReferencedResource<DiagnosticReport, FhirOrganization>(diagnosticReport,
               domainResource =>
               {
                   var observation = ExtractResultObservation(diagnosticReport);
                   return observation.Performer;
               },
               domainResource
                   => throw new AssertFailedException($"{nameof(FhirOrganization)} not found " +
                   $"in {diagnosticReport}"));
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabGroupStatusIsCopied()
        {
            var labTestReport = new LabTestResults();
            labTestReport.Groups.Add(new LabTestResultGroup
            {
                GroupName = new CodableValue("Lab"),
                Status = HealthVaultLabStatusCodes.Complete
            });

            var diagnosticReport = labTestReport.ToFhir();
            var observation = ExtractResultObservation(diagnosticReport);

            Assert.AreEqual(ObservationStatus.Final, observation?.Status);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabSubGroupsAreAddedAsRelatedMemberObservations()
        {
            var labTestReport = new LabTestResults();
            var resultGroup = new LabTestResultGroup
            {
                GroupName = new CodableValue("Lab"),
            };
            const string subResultGroupName = "Lab sub 1";
            resultGroup.SubGroups.Add(new LabTestResultGroup
            {
                GroupName = new CodableValue(subResultGroupName)
            });
            labTestReport.Groups.Add(resultGroup);

            var diagnosticReport = labTestReport.ToFhir();
            var observation = ExtractResultObservation(diagnosticReport);

            Assert.IsTrue(observation.Related.Any());

            var memberObservation = ExtractMemberObservation(diagnosticReport);

            Assert.AreEqual(subResultGroupName, memberObservation.Code.Text);
        }

        private Observation ExtractMemberObservation(DiagnosticReport diagnosticReport)
        {
            return ExtractFirstReferencedResource<DiagnosticReport, Observation>(diagnosticReport,
                domainResource =>
                {
                    var observation = ExtractResultObservation(diagnosticReport);
                    return observation.Related
                        .Where(related => related.Type == Observation.ObservationRelationshipType.HasMember)
                        .Select(related => related.Target);
                },
                domainResource => throw new AssertFailedException($"Member{nameof(Observation)} not found " +
                $"in {diagnosticReport}"));
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultDateTimeIsCopied()
        {
            var labTestReport = new LabTestResults();
            var resultGroup = new LabTestResultGroup
            {
                GroupName = new CodableValue("Lab"),
            };
            var labtestDateTime = new LocalDateTime(2017, 12, 12, 12, 12);
            resultGroup.Results.Add(new LabTestResultDetails()
            {
                When = new ApproximateDateTime(labtestDateTime)
            });
            labTestReport.Groups.Add(resultGroup);

            var diagnosticReport = labTestReport.ToFhir();
            var observation = ExtractResultObservation(diagnosticReport);

            Assert.IsTrue(observation.Related.Any());

            var detailObservation = ExtractDetailObservation(diagnosticReport);

            Assert.AreEqual(labtestDateTime.ToDateTimeUnspecified(),
                (detailObservation.Effective as FhirDateTime).ToDateTimeOffset());
        }

        private Observation ExtractDetailObservation(DiagnosticReport diagnosticReport)
        {
            return ExtractFirstReferencedResource<DiagnosticReport, Observation>(diagnosticReport,
                domainResource =>
                {
                    var observation = ExtractResultObservation(diagnosticReport);
                    return observation.Related
                        .Where(related => related.Type == Observation.ObservationRelationshipType.DerivedFrom)
                        .Select(related => related.Target);
                },
                domainResource => throw new AssertFailedException($"Source{nameof(Observation)} not found " +
                $"in {diagnosticReport}"));
        }

        private Observation ExtractDetailObservation(LabTestResults labTestReport)
        {
            var diagnosticReport = labTestReport.ToFhir();
            return ExtractDetailObservation(diagnosticReport);
        }

        private static LabTestResults GetSampleLabTestResults(LabTestResultDetails resultDetail)
        {
            var labTestReport = new LabTestResults();
            var resultGroup = new LabTestResultGroup
            {
                GroupName = new CodableValue("Lab"),
            };
            resultGroup.Results.Add(resultDetail);
            labTestReport.Groups.Add(resultGroup);
            return labTestReport;
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultNameIsCopied()
        {
            const string labResultName = "Lab Name";
            var resultDetail = new LabTestResultDetails()
            {
                Name = labResultName
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            Observation sourceObservation = ExtractDetailObservation(labTestReport);

            Assert.AreEqual(labResultName,
                sourceObservation.GetStringExtension(HealthVaultExtensions.LabTestResultName));
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultSubstanceIsCopiedToSpecimen()
        {
            const string substanceName = "Blood";
            var resultDetail = new LabTestResultDetails()
            {
                Substance = new CodableValue(substanceName)
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.IsNotNull(detailObservation.Specimen, "Specimen reference not found");

            Specimen specimen = ExtractSpecimen(labTestReport);

            Assert.AreEqual(substanceName, specimen.Type.Text);
        }

        private Specimen ExtractSpecimen(LabTestResults labTestReport)
        {
            var diagnosticReport = labTestReport.ToFhir();
            var detailObservation = ExtractDetailObservation(diagnosticReport);
            return diagnosticReport.GetReferencedResource<Specimen>(detailObservation.Specimen,
                specimenReference => throw new AssertFailedException("Specimen not found"));
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultCollectionMethodIsCopiedToSpecimen()
        {
            const string collectionMethod = "Smear procedure";
            var resultDetail = new LabTestResultDetails()
            {
                CollectionMethod = new CodableValue(collectionMethod)
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.IsNotNull(detailObservation.Specimen, "Specimen reference not found");

            Specimen specimen = ExtractSpecimen(labTestReport);

            Assert.AreEqual(collectionMethod, specimen.Collection.Method.Text);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultClinicalCodeIsCopied()
        {
            const string loincCode = "1 minute Apgar Color";
            var resultDetail = new LabTestResultDetails()
            {
                ClinicalCode = new CodableValue(loincCode)
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.AreEqual(loincCode, detailObservation.Code?.Text);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultValueIsCopied()
        {
            const string measure = "221 mg/dL";
            var resultDetail = new LabTestResultDetails()
            {
                Value = new LabTestResultValue
                {
                    Measurement = new GeneralMeasurement(measure)
                }
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.AreEqual(measure, (detailObservation.Value as FhirString)?.Value);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultValueDetailsAreCopiedAsExtension()
        {
            const string measure = "221 mg/dL";
            var generalMeasurement = new GeneralMeasurement(measure);
            const int measureValue = 221;
            generalMeasurement.Structured.Add(new StructuredMeasurement
            {
                Value = measureValue,
                Units = new CodableValue(text: "mg/dL", code: "mg-per-dl",
                family: "wc", vocabularyName: "lab-result-unit", version: "1")
            });
            var resultDetail = new LabTestResultDetails()
            {
                Value = new LabTestResultValue
                {
                    Measurement = generalMeasurement
                }
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.AreEqual(measure, (detailObservation.Value as FhirString)?.Value);

            Assert.IsTrue(detailObservation.Value.HasExtensions(HealthVaultExtensions.LabTestResultValueDetail),
                "Value details not found");

            Assert.AreEqual(measureValue, detailObservation.Value
                .GetExtensionValue<Quantity>(HealthVaultExtensions.LabTestResultValueDetail).Value);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultRangesAreCopied()
        {
            var labTestResultValue = new LabTestResultValue
            {
                Measurement = new GeneralMeasurement("221 mg/dL")
            };
            const string rangeTypeText = "Result Range";
            const string RangeTextText = "100-199";
            labTestResultValue.Ranges.Add(new TestResultRange
            {
                RangeType = new CodableValue(rangeTypeText),
                Text = new CodableValue(RangeTextText)
            });
            var resultDetail = new LabTestResultDetails()
            {
                Value = labTestResultValue
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);
            var rangeComponent = detailObservation.ReferenceRange.FirstOrDefault();

            Assert.IsNotNull(rangeComponent, "Range component missing");
            Assert.AreEqual(rangeTypeText, rangeComponent.Type?.Text);
            Assert.AreEqual(RangeTextText, rangeComponent.Text);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultRangeTextIsCopiedAsAnExtension()
        {
            var labTestResultValue = new LabTestResultValue
            {
                Measurement = new GeneralMeasurement("clear")
            };
            const string rangeTextText = "clear to yellow";
            labTestResultValue.Ranges.Add(new TestResultRange
            {
                RangeType = new CodableValue("Result Range"),
                Text = new CodableValue(rangeTextText)
            });
            var resultDetail = new LabTestResultDetails()
            {
                Value = labTestResultValue
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);
            var rangeComponent = detailObservation.ReferenceRange.FirstOrDefault();

            Assert.IsNotNull(rangeComponent, "Range component missing");
            Assert.IsTrue(rangeComponent.TextElement.HasExtensions(HealthVaultExtensions.LabTestResultValueRangeText),
                "Range text extension missing");

            Assert.AreEqual(rangeTextText, rangeComponent.TextElement
                .GetExtensionValue<CodeableConcept>(HealthVaultExtensions.LabTestResultValueRangeText).Text);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultRangesValuesAreCopied()
        {
            var labTestResultValue = new LabTestResultValue
            {
                Measurement = new GeneralMeasurement("221 mg/dL")
            };
            const int minimumValue = 100;
            labTestResultValue.Ranges.Add(new TestResultRange
            {
                RangeType = new CodableValue("Result Range"),
                Text = new CodableValue("100-199"),
                Value = new TestResultRangeValue
                {
                    Minimum = minimumValue
                }
            });
            const int maximumValue = 199;
            labTestResultValue.Ranges.Add(new TestResultRange
            {
                RangeType = new CodableValue("Result Range"),
                Text = new CodableValue("100-199"),
                Value = new TestResultRangeValue
                {
                    Maximum = maximumValue
                }
            });
            var resultDetail = new LabTestResultDetails()
            {
                Value = labTestResultValue
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);
            var rangeComponent = detailObservation.ReferenceRange.FirstOrDefault();

            Assert.IsNotNull(rangeComponent, "Range component missing");
            Assert.AreEqual(minimumValue, rangeComponent.Low?.Value);
            Assert.IsNull(rangeComponent.High, "Unexpected high value");

            var rangeComponent2 = detailObservation.ReferenceRange.ElementAtOrDefault(1);

            Assert.IsNotNull(rangeComponent2, "Second range component missing");
            Assert.AreEqual(maximumValue, rangeComponent2.High?.Value);
            Assert.IsNull(rangeComponent2.Low,"Unexpected low value");
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultValueFlagsAreCopiedAsExtension()
        {
            const string measure = "221 mg/dL";
            var labTestResultValue = new LabTestResultValue
            {
                Measurement = new GeneralMeasurement(measure)
            };
            const string flagText = "High";
            labTestResultValue.Flag.Add(new CodableValue(flagText));
            var resultDetail = new LabTestResultDetails()
            {
                Value = labTestResultValue
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.IsTrue(detailObservation.HasExtensions(HealthVaultExtensions.LabTestResultValueFlag),
                "Flag extension missing");
            Assert.AreEqual(flagText, detailObservation
                .GetExtensionValue<CodeableConcept>(HealthVaultExtensions.LabTestResultValueFlag)?.Text);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultStatusIsCopied()
        {
            var resultDetail = new LabTestResultDetails()
            {
                Status = HealthVaultLabStatusCodes.Complete
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.AreEqual(ObservationStatus.Final, detailObservation?.Status);
        }

        [TestMethod]
        public void WhenLabTestReportTransformedToFhir_ThenLabResultNoteIsCopied()
        {
            const string comment = "Great Results";
            var resultDetail = new LabTestResultDetails()
            {
                Note = comment
            };
            LabTestResults labTestReport = GetSampleLabTestResults(resultDetail);

            var detailObservation = ExtractDetailObservation(labTestReport);

            Assert.AreEqual(comment, detailObservation.Comment);
        }

        private TResource ExtractFirstReferencedResource<TDomainResource, TResource>(TDomainResource domainResource,
            Func<TDomainResource, IEnumerable<ResourceReference>> referenceListBuilder,
            Func<TDomainResource, TResource> defaultResolver)
            where TResource : DomainResource, new()
            where TDomainResource : DomainResource
        {
            var sample = new TResource();
            foreach (var reference in referenceListBuilder(domainResource))
            {
                if (reference.IsContainedReference)
                {
                    return domainResource.Contained.First(resource
                        => reference.Matches(resource.GetContainerReference())
                        && resource.ResourceType == sample.ResourceType) as TResource;
                }
            }
            return defaultResolver(domainResource);
        }
    }
}
