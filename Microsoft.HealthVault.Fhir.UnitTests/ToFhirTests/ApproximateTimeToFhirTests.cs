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

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class ApproximateTimeToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultApproximateTimeTransformedToFhir_ThenValuesEqual()
        {
            var approximateTime = new ApproximateTime(1, 2, 3, 4);

            var fhirTime = approximateTime.ToFhir();

            Assert.IsNotNull(fhirTime);
            Assert.IsNotNull("01:02:03.004", fhirTime.Value);
        }

        [TestMethod]
        public void WhenHeathVaultApproximateTimePartialTransformedToFhir_ThenValuesEqual()
        {
            // Only hours, minutes
            var approximateTime1 = new ApproximateTime(23,59);
            var fhirTime1 = approximateTime1.ToFhir();
            Assert.IsNotNull(fhirTime1);
            Assert.IsNotNull("23:59:00.000", fhirTime1.Value);

            // Only hours, minutes, seconds
            var approximateTime2 = new ApproximateTime(23, 59, 59);
            var fhirTime2 = approximateTime2.ToFhir();
            Assert.IsNotNull(fhirTime2);
            Assert.IsNotNull("23:59:59.000", fhirTime2.Value);

            // Only hours, minutes, seconds, milliseconds
            var approximateTime3 = new ApproximateTime(23, 59, 59, 999);
            var fhirTime3 = approximateTime3.ToFhir();
            Assert.IsNotNull(fhirTime3);
            Assert.IsNotNull("23:59:59.999", fhirTime2.Value);
        }
    }
}
