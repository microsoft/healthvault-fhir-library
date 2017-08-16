using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HVCondition = Microsoft.HealthVault.ItemTypes.Condition;
namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ConditionToFhirTest
    {
        [TestMethod]
        public void WhenHealthVaultConditionTransformedToFhir_ThenValuesEquals()
        {      
            HVCondition hvCondition = new HVCondition(new ItemTypes.CodableValue("High blood pressure", new ItemTypes.CodedValue("1147", "MayoConditions", "Mayo", "2.0")));
            hvCondition.Name.Add(new ItemTypes.CodedValue("1148", "MayoConditions", "Mayo", "1.0"));
            hvCondition.OnsetDate= new ItemTypes.ApproximateDateTime(DateTime.Now.AddDays(-15));
            hvCondition.StopDate = new ItemTypes.ApproximateDateTime() { ApproximateDate = new ItemTypes.ApproximateDate(2013, 03, 11) };
            hvCondition.StopDate.ApproximateDate = new ItemTypes.ApproximateDate(2015, 02, 22);
            hvCondition.Status = new ItemTypes.CodableValue("Past: No longer has this", new ItemTypes.CodedValue("intermittent", "condition-occurrence", "wc", "1"));
            hvCondition.CommonData.Note = "condition critical";
            hvCondition.CommonData.Source = "patient via InstantPHR patient portal";
            hvCondition.StopReason = "In Control";            
            hvCondition.Key = new ThingKey(new Guid("1C855AC0-892A-4352-9A82-3DCBD22BF0BC"), new Guid("706CEAFA-D506-43A8-9758-441FD9C3D407"));

            var fhircondition = hvCondition.ToFhir() as Condition;      
            Assert.IsNotNull(fhircondition);
            Assert.IsNotNull(fhircondition.Code);
            Assert.IsNotNull(fhircondition.Code.Coding);
            Assert.AreEqual(2, fhircondition.Code.Coding.Count);
            Assert.AreEqual("2015-02-22", fhircondition.Abatement.ToString());
            Assert.AreEqual("High blood pressure", fhircondition.Code.Coding[0].Display);
         
        }

        [TestMethod]
        public void WhenHealthVaultConditionTransformedToFhir_ThenValuesEqualWithAbatementString()
        {
            HVCondition hvCondition = new HVCondition(new ItemTypes.CodableValue("High blood pressure", new ItemTypes.CodedValue("1147", "MayoConditions", "Mayo", "2.0")));
            hvCondition.OnsetDate = new ItemTypes.ApproximateDateTime(DateTime.Now.AddDays(-15));
            hvCondition.StopDate = new ItemTypes.ApproximateDateTime();
            hvCondition.StopDate.Description = "around december 9, 2013";
            
            var fhircondition = hvCondition.ToFhir() as Condition;
            Assert.IsNotNull(fhircondition);
            Assert.IsNotNull(fhircondition.Code);
            Assert.AreEqual("around december 9, 2013", fhircondition.Abatement.ToString());
            
        }
    }
}
