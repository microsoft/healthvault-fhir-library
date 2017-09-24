// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.FhirExtensions.Helpers;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using FhirOrganization = Hl7.Fhir.Model.Organization;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    [TestCategory(nameof(LabTestResults))]
    public class LabTestResultsToFhirTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            Assert.IsTrue(true);
        }

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
                OrderedBy = new ItemTypes.Organization(name)
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
    }
}
