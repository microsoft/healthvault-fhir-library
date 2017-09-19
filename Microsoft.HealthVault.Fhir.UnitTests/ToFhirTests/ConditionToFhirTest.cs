// Copyright(c) Get Real Health.All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Extensions;
using HVCondition = Microsoft.HealthVault.ItemTypes.Condition;
using FhirCondition = Hl7.Fhir.Model.Condition;
using Microsoft.HealthVault.Fhir.Constants;

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
            hvCondition.OnsetDate = new ItemTypes.ApproximateDateTime(SystemClock.Instance.InBclSystemDefaultZone().GetCurrentLocalDateTime().Minus(NodaTime.Period.FromDays(15)));
            hvCondition.StopDate = new ItemTypes.ApproximateDateTime() { ApproximateDate = new ItemTypes.ApproximateDate(2013, 03, 11) };
            hvCondition.StopDate.ApproximateDate = new ItemTypes.ApproximateDate(2015);
            hvCondition.Status = new ItemTypes.CodableValue("Past: No longer has this", new ItemTypes.CodedValue("intermittent", "condition-occurrence", "wc", "1"));
            hvCondition.CommonData.Note = "condition critical";
            hvCondition.StopReason = "In Control";
            hvCondition.Key = new ThingKey(new Guid("1C855AC0-892A-4352-9A82-3DCBD22BF0BC"), new Guid("706CEAFA-D506-43A8-9758-441FD9C3D407"));

            var fhirCondition = hvCondition.ToFhir() as FhirCondition;
            var conditionExtension = fhirCondition.GetExtension(HealthVaultExtensions.Condition);
            Assert.IsNotNull(fhirCondition);
            Assert.IsNotNull(fhirCondition.Code);
            Assert.IsNotNull(fhirCondition.Code.Coding);
            Assert.AreEqual(2, fhirCondition.Code.Coding.Count);
            Assert.AreEqual("2015", fhirCondition.Abatement.ToString());
            Assert.AreEqual("High blood pressure", fhirCondition.Code.Text);
            Assert.AreEqual("In Control", conditionExtension.GetStringExtension(HealthVaultExtensions.ConditionStopReason));
            Assert.AreEqual("intermittent", conditionExtension.GetStringExtension(HealthVaultExtensions.ConditionOccurrence));
            Assert.AreEqual("1c855ac0-892a-4352-9a82-3dcbd22bf0bc", fhirCondition.Id);
            Assert.AreEqual("706ceafa-d506-43a8-9758-441fd9c3d407", fhirCondition.VersionId);
            Assert.AreEqual("1147", fhirCondition.Code.Coding[0].Code);
            Assert.AreEqual("1148", fhirCondition.Code.Coding[1].Code);        
        }

        [TestMethod]
        public void WhenHealthVaultConditionTransformedToFhir_ThenValuesEqualWithAbatementString()
        {
            HVCondition hvCondition = new HVCondition(new ItemTypes.CodableValue("High blood pressure", new ItemTypes.CodedValue("1147", "MayoConditions", "Mayo", "2.0")));
            hvCondition.OnsetDate = new ItemTypes.ApproximateDateTime(SystemClock.Instance.InBclSystemDefaultZone().GetCurrentLocalDateTime().Minus(NodaTime.Period.FromDays(15)));
            hvCondition.StopDate = new ItemTypes.ApproximateDateTime();
            hvCondition.StopDate.Description = "around december 9, 2013";

            var fhirCondition = hvCondition.ToFhir() as FhirCondition;
            Assert.IsNotNull(fhirCondition);
            Assert.IsNotNull(fhirCondition.Code);
            Assert.AreEqual("around december 9, 2013", fhirCondition.Abatement.ToString());
        }
    }
}
