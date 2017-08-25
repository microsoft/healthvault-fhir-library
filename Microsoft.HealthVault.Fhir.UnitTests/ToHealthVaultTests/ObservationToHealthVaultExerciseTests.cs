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
    public class ObservationToHealthVaultExerciseTests
    {
        [TestMethod]
        public void WhenFhirExerciseTransformedToHealthVault_ThenValuesEqual()
        {
            var json = SampleUtil.GetSampleContent("FhirExercise.json");

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);

            var exercise = observation.ToHealthVault() as Exercise;

            Assert.IsNotNull(exercise);
            Assert.AreEqual("Swimming", exercise.Activity.Text);
            Assert.AreEqual(2, exercise.Details.Count);
            Assert.IsNotNull(exercise.Details["lap 1"]);
            Assert.AreEqual("swimming", exercise.Details["lap 1"].Name.Value);
            Assert.AreEqual(30, exercise.Details["lap 1"].Value.Value);
            Assert.AreEqual("seconds", exercise.Details["lap 1"].Value.Units.Text);
            Assert.AreEqual(1, exercise.Segments.Count);
            Assert.AreEqual("Segment 1", exercise.Segments[0].Title);
            Assert.AreEqual(180, exercise.Segments[0].Duration);
            Assert.AreEqual(31.5, exercise.Segments[0].Distance.Meters);
            Assert.AreEqual(43.3, exercise.Segments[0].Offset);
            Assert.AreEqual(2, exercise.Segments[0].Details.Count);
            Assert.IsNotNull(exercise.Segments[0].Details["segment 1 - lap 1"]);
            Assert.AreEqual(46.2, exercise.Segments[0].Details["segment 1 - lap 1"].Value.Value);
            Assert.AreEqual(10, exercise.Duration);
            Assert.AreEqual(30, exercise.Distance.Meters);
        }
    }
}