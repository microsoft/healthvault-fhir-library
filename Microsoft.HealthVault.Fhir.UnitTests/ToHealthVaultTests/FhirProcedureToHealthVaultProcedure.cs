// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class FhirProcedureToHealthVaultProcedure
    {
        [TestCategory(nameof(Procedure))]
        [TestMethod]
        public void WhenFhirProcedureToHealthVaultProcedure_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirProcedure.json");

            var fhirParser = new FhirJsonParser();
            var fhirProcedure = fhirParser.Parse<Procedure>(json);
            
            var hvProcedure = fhirProcedure.ToHealthVault() as ItemTypes.Procedure;
            Assert.IsNotNull(hvProcedure);
            //when
            Assert.AreEqual(2013,hvProcedure.When.ApproximateDate.Year);
            Assert.AreEqual(1, hvProcedure.When.ApproximateDate.Month);
            Assert.AreEqual(28, hvProcedure.When.ApproximateDate.Day);
            Assert.AreEqual(13, hvProcedure.When.ApproximateTime.Hour);
            Assert.AreEqual(31, hvProcedure.When.ApproximateTime.Minute);
            Assert.AreEqual(00, hvProcedure.When.ApproximateTime.Second);

            //name
            Assert.AreEqual("Chemotherapy", hvProcedure.Name.Text);            

            //primary-provider
            Assert.AreEqual("Adam", hvProcedure.PrimaryProvider.Name.First);
            Assert.AreEqual("Careful", hvProcedure.PrimaryProvider.Name.Last);
            Assert.AreEqual("Certified Medical Assistant", hvProcedure.PrimaryProvider.ProfessionalTraining);

            //secondary-provider
            Assert.AreEqual("Dokter Bronsig", hvProcedure.SecondaryProvider.Name.Full);

            //body-site
            Assert.AreEqual("Sphenoid bone", hvProcedure.AnatomicLocation.Text);
        }
    }
}
