// Copyright (c) Microsoft Corporation.  All rights reserved.
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
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    [TestClass]
    public class ObservationToHealthVaultSleepJournalAMTests
    {
        [TestMethod]
        public void WhenFhirSleepJournalAMTransformedToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirSleepJournalAM.json");

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);

            var sleepJournalAm = observation.ToHealthVault() as SleepJournalAM;
            Assert.IsNotNull(sleepJournalAm);

            Assert.AreEqual(new ApproximateTime(22, 30, 0, 900), sleepJournalAm.Bedtime);
            Assert.AreEqual(new ApproximateTime(6, 28, 59, 182), sleepJournalAm.WakeTime);
            Assert.AreEqual(100, sleepJournalAm.SleepMinutes);
            Assert.AreEqual(110, sleepJournalAm.SettlingMinutes);
            Assert.AreEqual(WakeState.Tired, sleepJournalAm.WakeState);
            Assert.AreEqual(1, sleepJournalAm.Medications.Count);
            Assert.AreEqual("ccabbac8-58f0-4e88-a1eb-538e21e7524d", sleepJournalAm.Medications[0].Value);
            Assert.AreEqual("Mayo", sleepJournalAm.Medications[0].VocabularyName);
            Assert.AreEqual(2, sleepJournalAm.Awakenings.Count);
            Assert.AreEqual(new ApproximateTime(23, 30, 0, 0), sleepJournalAm.Awakenings[0].When);
            Assert.AreEqual(40, sleepJournalAm.Awakenings[0].Minutes);
            Assert.AreEqual(new ApproximateTime(0, 30, 0, 0), sleepJournalAm.Awakenings[1].When);
            Assert.AreEqual(10, sleepJournalAm.Awakenings[1].Minutes);
        }
    }
}