// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Extensions;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ExerciseToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultExerciseTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var swimmingCodableValue = new CodableValue("Swimming", "swimming", "exercise-activities", "wc", "1");
            var secondsCodableValue = new CodableValue("seconds", "s", "duration-units", "wc", "1");

            var exerciseSegment = new ExerciseSegment(swimmingCodableValue)
            {
                Duration = 180,
                Distance = new Length(31.5),
                Offset = 43.3,
                Title = "Segment 1"
            };
            exerciseSegment.Details.Add("segment 1 - lap 1", new ExerciseDetail(swimmingCodableValue[0], new StructuredMeasurement(46.2, secondsCodableValue)));
            exerciseSegment.Details.Add("segment 1 - lap 2", new ExerciseDetail(swimmingCodableValue[0], new StructuredMeasurement(21, secondsCodableValue)));

            var exercise = new Exercise(new ApproximateDateTime(SystemClock.Instance.InUtc().GetCurrentLocalDateTime()), swimmingCodableValue)
            {
                Distance = new Length(30),
                Duration = 10,
                Title = "New Swim"
            };
            exercise.Segments.Add(exerciseSegment);
            exercise.Details.Add("lap 1", new ExerciseDetail(swimmingCodableValue[0], new StructuredMeasurement(30, secondsCodableValue)));
            exercise.Details.Add("lap 2", new ExerciseDetail(swimmingCodableValue[0], new StructuredMeasurement(28, secondsCodableValue)));

            var observation = exercise.ToFhir();

            Assert.IsNotNull(observation);
            Assert.AreEqual(5 ,observation.Extension.Count);
            Assert.AreEqual(HealthVaultVocabularies.Exercise, observation.Code.Coding[0].Code);

            var detailExtensions = observation.Extension.Where(x => x.Url == HealthVaultExtensions.ExerciseDetail).ToList();
            Assert.AreEqual(2, detailExtensions.Count);
            Assert.AreEqual("lap 1", ((FhirString)detailExtensions[0].Extension[0].Value).Value);
            Assert.AreEqual("exercise-activities:swimming", ((CodeableConcept)detailExtensions[0].Extension[1].Value).Coding[0].Code);
            Assert.AreEqual(30, ((Quantity)detailExtensions[0].Extension[2].Value).Value);
            Assert.AreEqual("seconds", ((Quantity)detailExtensions[0].Extension[2].Value).Unit);

            var segmentExtensions = observation.Extension.Where(x => x.Url == HealthVaultExtensions.ExerciseSegment).ToList();
            Assert.AreEqual(1, segmentExtensions.Count);
            Assert.AreEqual("exercise-activities:swimming", ((CodeableConcept)segmentExtensions[0].Extension[0].Value).Coding[0].Code);
            Assert.AreEqual("Segment 1", ((FhirString)segmentExtensions[0].Extension[1].Value).Value);
            Assert.AreEqual(180, ((FhirDecimal)segmentExtensions[0].Extension[2].Value).Value);
            Assert.AreEqual(31.5m, ((Quantity)segmentExtensions[0].Extension[3].Value).Value);
            Assert.AreEqual(43.3m, ((FhirDecimal)segmentExtensions[0].Extension[4].Value).Value);
            Assert.AreEqual(3, segmentExtensions[0].Extension[5].Extension.Count);
            Assert.AreEqual("segment 1 - lap 1", ((FhirString)segmentExtensions[0].Extension[5].Extension[0].Value).Value);

            Assert.AreEqual($"{HealthVaultVocabularies.Exercise}:{HealthVaultVocabularies.ExerciseDistance}", observation.Component[0].Code.Coding[0].Code);
            Assert.AreEqual(30, ((Quantity)observation.Component[0].Value).Value);
            Assert.AreEqual("m", ((Quantity)observation.Component[0].Value).Unit);

            Assert.AreEqual($"{HealthVaultVocabularies.Exercise}:{HealthVaultVocabularies.ExerciseDuration}", observation.Component[1].Code.Coding[0].Code);
            Assert.AreEqual(10, ((Quantity)observation.Component[1].Value).Value);
            Assert.AreEqual("min", ((Quantity)observation.Component[1].Value).Unit);

            Assert.AreEqual($"{HealthVaultVocabularies.Exercise}:{HealthVaultVocabularies.ExerciseActivity}", observation.Component[2].Code.Coding[0].Code);
            Assert.AreEqual("exercise-activities:swimming", ((CodeableConcept)observation.Component[2].Value).Coding[0].Code);
        }
    }
}
