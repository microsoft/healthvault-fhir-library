// Copyright(c) Get Real Health.All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using FhirOrganization = Hl7.Fhir.Model.Organization;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory(nameof(DiagnosticReport))]
    public class DiagnosticReportToHealthVaultTests
    {
        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenWhenIsCopiedFromIssued()
        {
            var issued = new LocalDateTime(2017, 12, 12, 12, 12);
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = new Hl7.Fhir.Model.Instant(issued.ToDateTimeUnspecified())
            };

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.IsTrue(labTestReport.When?.Equals(issued) == true);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenOrderByIsCopiedFromAnExtension()
        {
            const string organizationName = "Dr. Roger Jones";
            var fhirOrganization = new Organization
            {
                Id = "123",
                Name = organizationName
            };
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
                Extension = new List<Extension>
                {
                    new Extension
                    {
                        Url = HealthVaultExtensions.LabTestResultOrderBy,
                        Value = fhirOrganization.GetContainerReference()
                    }
                },
                Contained = new List<Resource>
                {
                    fhirOrganization
                }
            };

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.AreEqual(organizationName, labTestReport.OrderedBy?.Name);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabGroupsAreAddedFromObservations()
        {
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
            };
            int groupsNumber = 3;
            for (int i = 0; i < groupsNumber; i++)
            {
                var observation = new Observation
                {
                    Id = "obs" + i,
                    Code = new CodeableConcept { Text = "lab" + i }
                };
                diagnosticReport.Contained.Add(observation);
                diagnosticReport.Result.Add(observation.GetContainerReference());
            }

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.AreEqual(groupsNumber, labTestReport.Groups.Count);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabGroupNameIsCopiedFromObservationCode()
        {
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
            };
            const string panelName = "lab panel";
            var observation = new Observation
            {
                Code = new CodeableConcept { Text = panelName }
            };
            diagnosticReport.Contained.Add(observation);
            diagnosticReport.Result.Add(observation.GetContainerReference());

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.AreEqual(panelName, labTestReport.Groups.First().GroupName.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabGroupLaboratoryIsCopiedFromPerformer()
        {
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
            };
            const string labName = "Fabrikam analysis";
            var performer = new FhirOrganization
            {
                Id = "perf",
                Name = labName
            };
            var observation = new Observation
            {
                Code = new CodeableConcept { Text = "lab panel" },
                Performer = new List<ResourceReference>
                {
                    performer.GetContainerReference()
                }
            };
            diagnosticReport.Contained.Add(performer);
            diagnosticReport.Contained.Add(observation);
            diagnosticReport.Result.Add(observation.GetContainerReference());

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.AreEqual(labName, labTestReport.Groups.First().LaboratoryName?.Name);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabGroupStatusIsCopied()
        {
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
            };
            var observation = new Observation
            {
                Code = new CodeableConcept { Text = "lab panel" },
                Status = ObservationStatus.Final
            };
            diagnosticReport.Contained.Add(observation);
            diagnosticReport.Result.Add(observation.GetContainerReference());

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.AreEqual(HealthVaultLabStatusCodes.CompleteCode, labTestReport.Groups.First().Status?.First()?.Value);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabSubGroupsAreAddedFromRelatedMemberObservations()
        {
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
            };
            const string subPanelName = "lab sub panel";
            var subObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = subPanelName }
            };
            var observation = new Observation
            {
                Code = new CodeableConcept { Text = "lab panel" },
                Related = new List<Observation.RelatedComponent>
                {
                    new Observation.RelatedComponent
                    {
                        Type = Observation.ObservationRelationshipType.HasMember,
                        Target = subObservation.GetContainerReference()
                    }
                }
            };
            diagnosticReport.Contained.Add(subObservation);
            diagnosticReport.Contained.Add(observation);
            diagnosticReport.Result.Add(observation.GetContainerReference());

            var labTestReport = diagnosticReport.ToHealthVault();

            Assert.AreEqual(subPanelName, labTestReport.Groups.First().SubGroups.FirstOrDefault()?.GroupName.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultDateTimeIsCopied()
        {
            var labtestDateTime = new LocalDateTime(2017, 12, 12, 12, 12, 0, 0);
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Effective = new FhirDateTime(labtestDateTime.ToDateTimeUnspecified())
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.IsTrue(labTestResultDetails?.When?.Equals(labtestDateTime) == true);
        }

        private ItemTypes.LabTestResultDetails ExtractFirstResult(DiagnosticReport diagnosticReport)
        {
            var labTestReport = diagnosticReport.ToHealthVault();
            return labTestReport.Groups.First().Results.FirstOrDefault();
        }

        private static DiagnosticReport GetSampleDiagnosticReport(Observation detailObservation)
        {
            var diagnosticReport = new DiagnosticReport
            {
                IssuedElement = Hl7.Fhir.Model.Instant.Now(),
            };
            var observation = new Observation
            {
                Code = new CodeableConcept { Text = "lab panel" },
                Related = new List<Observation.RelatedComponent>
                {
                    new Observation.RelatedComponent
                    {
                        Type = Observation.ObservationRelationshipType.DerivedFrom,
                        Target = detailObservation.GetContainerReference()
                    }
                }
            };
            diagnosticReport.Contained.Add(detailObservation);
            diagnosticReport.Contained.Add(observation);
            diagnosticReport.Result.Add(observation.GetContainerReference());
            return diagnosticReport;
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultNameIsCopied()
        {
            const string labResultName = "Lab Name";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Extension = new List<Extension>
                {
                   new Extension{
                       Url = HealthVaultExtensions.LabTestResultName,
                       Value = new FhirString(labResultName)
                    }
                }
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(labResultName, labTestResultDetails?.Name);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultSubstanceIsCopiedFromSpecimen()
        {
            const string substanceName = "Blood";
            var specimen = new Specimen
            {
                Id = "spec",
                Type = new CodeableConcept
                {
                    Text = substanceName
                }
            };
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Specimen = specimen.GetContainerReference()
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);
            diagnosticReport.Contained.Add(specimen);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(substanceName, labTestResultDetails?.Substance?.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultCollectionMethodIsCopiedFromSpecimen()
        {
            const string collectionMethod = "Smear procedure";
            var specimen = new Specimen
            {
                Id = "spec",
                Collection = new Specimen.CollectionComponent
                {
                    Method = new CodeableConcept
                    {
                        Text = collectionMethod
                    }
                }
            };
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Specimen = specimen.GetContainerReference()
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);
            diagnosticReport.Contained.Add(specimen);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(collectionMethod, labTestResultDetails?.CollectionMethod?.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultClinicalCodeIsCopied()
        {
            const string loincCode = "1 minute Apgar Color";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = loincCode },
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(loincCode, labTestResultDetails?.ClinicalCode?.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultValueIsCopied()
        {
            const string measure = "221 mg/dL";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Value = new FhirString(measure)
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(measure, labTestResultDetails?.Value?.Measurement.Display);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultValueDetailsAreCopiedAsExtension()
        {
            const string measure = "221 mg/dL";
            const int measureValue = 221;
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Value = new FhirString(measure)
                {
                    Extension = new List<Extension>
                    {
                        new Extension
                        {
                            Url=  HealthVaultExtensions.LabTestResultValueDetail,
                            Value = new Quantity
                            {
                                Value = measureValue,
                                Unit = "mg/dL",
                                Code = "mg-per-dl",
                                System = "http://unitsofmeasure.org"
                            }
                        }
                    }
                }
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(measureValue, labTestResultDetails?.Value?.Measurement.Structured.FirstOrDefault()?.Value);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultRangesAreCopied()
        {
            const string rangeTypeText = "Result Range";
            const string rangeTextText = "100-199";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Value = new FhirString("221 mg/dL"),
                ReferenceRange = new List<Observation.ReferenceRangeComponent>
                {
                    new Observation.ReferenceRangeComponent
                    {
                        Type = new CodeableConcept{Text=rangeTypeText},
                        Text = rangeTextText
                    }
                }
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(rangeTypeText, labTestResultDetails?.Value?.Ranges.FirstOrDefault()?.RangeType.Text);
            Assert.AreEqual(rangeTextText, labTestResultDetails?.Value?.Ranges.FirstOrDefault()?.Text.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultRangeTextIsCopiedFromExtension()
        {
            const string rangeTextText = "clear to yellow";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Value = new FhirString("clear"),
                ReferenceRange = new List<Observation.ReferenceRangeComponent>
                {
                    new Observation.ReferenceRangeComponent
                    {
                        Type = new CodeableConcept{Text="Result Range"},
                        Text = rangeTextText,
                        Extension= new List<Extension>
                        {
                            new Extension{
                                Url =HealthVaultExtensions.LabTestResultValueRangeText,
                                Value = new CodeableConcept{Text=rangeTextText}
                            }
                        }
                    }
                }
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(rangeTextText, labTestResultDetails?.Value?.Ranges.FirstOrDefault()?.Text.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultRangesValuesAreCopied()
        {
            const int minimumValue = 100;
            const int maximumValue = 199;
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Value = new FhirString("221 mg/dL"),
                ReferenceRange = new List<Observation.ReferenceRangeComponent>
                {
                    new Observation.ReferenceRangeComponent
                    {
                        Type = new CodeableConcept{Text="Result Range"},
                        Text = "100-199",
                        Low = new SimpleQuantity
                        {
                            Value=minimumValue
                        }
                    },
                    new Observation.ReferenceRangeComponent
                    {
                        Type = new CodeableConcept{Text="Result Range"},
                        Text = "100-199",
                        High = new SimpleQuantity
                        {
                            Value=maximumValue
                        }
                    }
                }
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(minimumValue, labTestResultDetails?.Value?.Ranges.FirstOrDefault()?.Value.Minimum);
            Assert.AreEqual(maximumValue, labTestResultDetails?.Value?.Ranges.ElementAtOrDefault(1)?.Value.Maximum);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultValueFlagsAreCopiedFromExtension()
        {
            const string flagText = "High";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Value = new FhirString("221 mg/dL"),
                Extension = new List<Extension>
                {
                    new Extension
                    {
                        Url = HealthVaultExtensions.LabTestResultValueFlag,
                        Value = new CodeableConcept{Text=flagText}
                    }
                }
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(flagText, labTestResultDetails?.Value?.Flag.FirstOrDefault()?.Text);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultStatusIsCopied()
        {
            const string measure = "221 mg/dL";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Status = ObservationStatus.Final
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(HealthVaultLabStatusCodes.CompleteCode, labTestResultDetails?.Status.First().Value);
        }

        [TestMethod]
        public void WhenDiagnosticReportTransformedToHealthVault_ThenLabResultNoteIsCopied()
        {
            const string comment = "Great Results";
            var detailObservation = new Observation
            {
                Id = "sub",
                Code = new CodeableConcept { Text = "detail lab" },
                Comment = comment
            };
            DiagnosticReport diagnosticReport = GetSampleDiagnosticReport(detailObservation);

            var labTestResultDetails = ExtractFirstResult(diagnosticReport);

            Assert.AreEqual(comment, labTestResultDetails?.Note);
        }
    }
}
