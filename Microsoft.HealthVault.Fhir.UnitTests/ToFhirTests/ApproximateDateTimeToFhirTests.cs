// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class ApproximateDateTimeToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultApproximateDateTimeTransformedToFhir_ThenValuesEqual()
        {
            var testDateTime = new LocalDateTime(2017, 8, 3, 10, 17, 16, 115);
            var approximateDateTime = new ApproximateDateTime(testDateTime);

            var fhirDateTime = approximateDateTime.ToFhir();
            Assert.IsNotNull(fhirDateTime);
            Assert.AreEqual(testDateTime.ToDateTimeUnspecified(), fhirDateTime.ToDateTime().Value);
        }

        [TestMethod]
        public void WhenHeathVaultApproximateDateTimePartialTransformedToFhir_ThenDefaultValuesEqual()
        {
            // Year only
            var approximateDateTime1 = new ApproximateDateTime(new ApproximateDate(2017));
            var expectedDateTime1 = new DateTime(2017,1,1);
            var fhirDateTime1 = approximateDateTime1.ToFhir();
            Assert.IsNotNull(fhirDateTime1);
            Assert.AreEqual(expectedDateTime1, fhirDateTime1.ToDateTime().Value);

            // Year, month only
            var approximateDateTime2 = new ApproximateDateTime(new ApproximateDate(2017, 8));
            var expectedDateTime2 = new DateTime(2017, 8, 1);
            var fhirDateTime2 = approximateDateTime2.ToFhir();
            Assert.IsNotNull(fhirDateTime2);
            Assert.AreEqual(expectedDateTime2, fhirDateTime2.ToDateTime().Value);

            // Year, month, day only
            var approximateDateTime3 = new ApproximateDateTime(new ApproximateDate(2017, 8, 3));
            var expectedDateTime3 = new DateTime(2017, 8, 3);
            var fhirDateTime3 = approximateDateTime3.ToFhir();
            Assert.IsNotNull(fhirDateTime3);
            Assert.AreEqual(expectedDateTime3, fhirDateTime3.ToDateTime().Value);

            // Year, month, day, hour, minute only
            var approximateDateTime4 = new ApproximateDateTime(new ApproximateDate(2017, 8, 3), new ApproximateTime(10, 15));
            var expectedDateTime4 = new DateTime(2017, 8, 3, 10, 15, 0);
            var fhirDateTime4 = approximateDateTime4.ToFhir();
            Assert.IsNotNull(fhirDateTime4);
            Assert.AreEqual(expectedDateTime4, fhirDateTime4.ToDateTime().Value);

            // Year, month, day, hour, minute, second only
            var approximateDateTime5 = new ApproximateDateTime(new ApproximateDate(2017, 8, 3), new ApproximateTime(10, 15, 30));
            var expectedDateTime5 = new DateTime(2017, 8, 3, 10, 15, 30);
            var fhirDateTime5 = approximateDateTime5.ToFhir();
            Assert.IsNotNull(fhirDateTime5);
            Assert.AreEqual(expectedDateTime5, fhirDateTime5.ToDateTime().Value);

            // Year, month, day, hour, minute, second, millisecond only
            var approximateDateTime6 = new ApproximateDateTime(new ApproximateDate(2017, 8, 3), new ApproximateTime(10, 15, 30, 115));
            var expectedDateTime6 = new DateTime(2017, 8, 3, 10, 15, 30, 115);
            var fhirDateTime6 = approximateDateTime6.ToFhir();
            Assert.IsNotNull(fhirDateTime6);
            Assert.AreEqual(expectedDateTime6, fhirDateTime6.ToDateTime().Value);
        }
    }
}
