// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Extensions;
using Period = Hl7.Fhir.Model.Period;

namespace Microsoft.HealthVault.Fhir.ToFhirTests.UnitTests
{
    [TestClass]
    public class SleepJournalAMToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultSleepJournalAMTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var sleepJournalAm = new SleepJournalAM(
                new HealthServiceDateTime(SystemClock.Instance.InUtc().GetCurrentLocalDateTime()),
                new ApproximateTime(22, 30),
                new ApproximateTime(6,28),
                100,
                110,
                WakeState.Tired
                );

            sleepJournalAm.Awakenings.Add(new Occurrence(new ApproximateTime(23, 30), 40));
            sleepJournalAm.Awakenings.Add(new Occurrence(new ApproximateTime(0, 30), 10));

            sleepJournalAm.Medications = new CodableValue("Benzaclin", new CodedValue("ccabbac8-58f0-4e88-a1eb-538e21e7524d", "Mayo", "RxNorm", "2" ));

            var observation = sleepJournalAm.ToFhir();

            Assert.IsNotNull(observation);
            Assert.AreEqual("22:30:00.000", observation.Component[0].Value.ToString());
            Assert.AreEqual("06:28:00.000", observation.Component[1].Value.ToString());
            Assert.AreEqual(100, ((Quantity)observation.Component[2].Value).Value);
            Assert.AreEqual(UnitAbbreviations.Minute, ((Quantity)observation.Component[2].Value).Unit);
            Assert.AreEqual(110, ((Quantity)observation.Component[3].Value).Value);
            Assert.AreEqual(UnitAbbreviations.Minute, ((Quantity)observation.Component[3].Value).Unit);

            Assert.AreEqual(new FhirDateTime(1900, 01, 01, 23, 30), ((Period)observation.Component[4].Value).StartElement);
            Assert.AreEqual(new FhirDateTime(1900, 01, 02, 0, 10), ((Period)observation.Component[4].Value).EndElement);

            Assert.AreEqual(new FhirDateTime(1900, 01, 01, 0, 30), ((Period)observation.Component[5].Value).StartElement);
            Assert.AreEqual(new FhirDateTime(1900, 01, 01, 0, 40), ((Period)observation.Component[5].Value).EndElement);

            Assert.AreEqual("Tired", ((CodeableConcept)observation.Component[6].Value).Coding[0].Code);

            Assert.AreEqual("Mayo:ccabbac8-58f0-4e88-a1eb-538e21e7524d", ((CodeableConcept)observation.Component[7].Value).Coding[0].Code);
        }
    }
}
