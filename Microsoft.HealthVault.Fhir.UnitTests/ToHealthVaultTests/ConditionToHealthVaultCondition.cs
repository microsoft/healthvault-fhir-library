using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Fhir.UnitTests.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVItemTypes = Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class ConditionToHealthVaultCondition
    {
        [TestMethod]
        public void WhenConditionToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirCondition.json");
            var fhirParser = new FhirJsonParser();
            var fhircondition = fhirParser.Parse<Condition>(json);
            var cd = fhircondition.ToHealthVault();
            Assert.IsNotNull(cd);
            Assert.AreEqual("Acute renal insufficiency specified as due to procedure", cd.Name.Text);
            Assert.AreEqual(new HVItemTypes.ApproximateDateTime() {ApproximateDate = new HVItemTypes.ApproximateDate(2013, 03, 11)}, cd.OnsetDate);
            Assert.AreEqual("In Control", cd.StopReason);
            Assert.AreEqual("intermittent", cd.Status.Text);
            Assert.AreEqual("The patient is anuric.The patient state is critcal.",cd.CommonData.Note);
            Assert.AreEqual(new HVItemTypes.ApproximateDateTime() { ApproximateDate = new HVItemTypes.ApproximateDate(2015)}, cd.StopDate);
            Assert.AreEqual("patient via InstantPHR patient portal", cd.CommonData.Source);

        }
             
    }
}
