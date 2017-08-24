// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ApproximateTimeToFhirTimeTests
    {
        [TestMethod]
        public void WhenHeathVaultApproximateTimeTransformedToFhir_ThenValuesEqual()
        {
            var testTime = new LocalTime(10, 17, 16, 115);
            var approximateTime = new ApproximateTime(testTime);

            var fhirTime = approximateTime.ToFhir();
            Assert.IsNotNull(fhirTime);
            Assert.AreEqual(testTime.ToString(@"hh\:mm\:ss\.fff", null), fhirTime.Value);
        }

        [TestMethod]
        public void WhenHealthVaultApproximateTimePartialTransformedToFhir_ThenValuesEqual()
        {
            var testTime0 = new ApproximateTime();
            var fhirTime0 = testTime0.ToFhir();
            Assert.AreEqual("00:00:00.000", fhirTime0.Value);

            var testTime1 = new ApproximateTime(1, 1);
            var fhirTime1 = testTime1.ToFhir();
            Assert.AreEqual("01:01:00.000", fhirTime1.Value);

            var testTime2 = new ApproximateTime(2,2,2);
            var fhirTime2 = testTime2.ToFhir();
            Assert.AreEqual("02:02:02.000", fhirTime2.Value);

            var testTime3 = new ApproximateTime(3,3,3,3);
            var fhirTime3 = testTime3.ToFhir();
            Assert.AreEqual("03:03:03.003", fhirTime3.Value);
        }
    }
}
