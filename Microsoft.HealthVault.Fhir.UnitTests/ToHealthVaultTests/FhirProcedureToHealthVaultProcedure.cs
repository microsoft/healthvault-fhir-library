using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class FhirProcedureToHealthVaultProcedure
    {
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
            //TODO: Assert code and system of hvProcedure.Name

            //primary-provider
            Assert.AreEqual("Dokter Bronsig", hvProcedure.PrimaryProvider.Name.Full);
            Assert.AreEqual("Medical oncologist", hvProcedure.PrimaryProvider.ProfessionalTraining);
        }
    }
}
