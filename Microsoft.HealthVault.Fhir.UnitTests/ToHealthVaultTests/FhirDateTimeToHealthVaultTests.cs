// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    [TestCategory("Fhir to ItemTypes")]
    [TestCategory(nameof(FhirDateTime))]
    public class FhirDateTimeToHealthVaultTests
    {
        [TestMethod]
        public void WhenFhirDateTransformedToApproximateDateTime_ThenStringIsCopiedToDescription()
        {
            const string datetime = "A long time ago";
            Element element = new FhirString(datetime);

            var approximateDate = element.ToAproximateDateTime();

            Assert.AreEqual(datetime, approximateDate.Description);
        }

        [TestMethod]
        [DynamicData(nameof(ApproximateDateTest.GetDatePairs), typeof(ApproximateDateTest), DynamicDataSourceType.Method)]
        public void WhenFhirDateTransformedToApproximateDateTime_ThenOnlyYearIsAccepted(string date,
            ApproximateDateTime approximateDateTime)
        {
            ApproximateDateTime actual = new FhirDateTime(date).ToAproximateDateTime();
            Assert.AreEqual(approximateDateTime, actual);
        }

        private class ApproximateDateTest
        {
            internal static IEnumerable<object[]> GetDatePairs()
            {
                yield return new object[] { "2014", new ApproximateDateTime { ApproximateDate = new ApproximateDate(2014) } };
                yield return new object[] { "2015-12", new ApproximateDateTime { ApproximateDate = new ApproximateDate(2015, 12) } };
                yield return new object[] { "2016-12-19", new ApproximateDateTime { ApproximateDate = new ApproximateDate(2016, 12, 19) } };
                yield return new object[] { "2017-12-19T10:10:00", new ApproximateDateTime {
                    ApproximateDate = new ApproximateDate(2017, 12,19),
                    ApproximateTime =new ApproximateTime(10,10,0)
                } };
                yield return new object[] { "2017-12-19T10:10:00.012", new ApproximateDateTime {
                    ApproximateDate = new ApproximateDate(2017, 12,19),
                    ApproximateTime =new ApproximateTime(10,10,0,12)
                } };

            }
        }
    }
}
