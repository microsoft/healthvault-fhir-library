using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.Fhir.Transformers;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.Fhir.UnitTests.ToFhirTests
{
    [TestClass]
    public class ExerciseToFhirTests
    {
        [TestMethod]
        public void WhenHeathVaultExerciseTransformedToFhir_ThenCodeAndValuesEqual()
        {
            var swimmingCodableValue = new CodableValue("Swimming", "swimming", "exercise-activities", "wc", "1");
            var secondsCodableValue = new CodableValue("seconds", "s", "wc", "duration-units", "1");
            var exercise = new Exercise(new ApproximateDateTime(DateTime.Now), swimmingCodableValue);

            exercise.Distance = new Length(30);
            exercise.Duration = 10;
            exercise.Title = "New Swim";
            exercise.Segments.Add(new ExerciseSegment(swimmingCodableValue));
            exercise.Details.Add("lap 1", new ExerciseDetail(swimmingCodableValue[0], new StructuredMeasurement(30, secondsCodableValue)));
            exercise.Details.Add("lap 2", new ExerciseDetail(swimmingCodableValue[0], new StructuredMeasurement(28, secondsCodableValue)));

            var observation = exercise.ToFhir();

            var json = FhirSerializer.SerializeToJson(observation);

            Assert.IsNotNull(observation);
            Assert.AreEqual(5 ,observation.Extension.Count);
            Assert.AreEqual(HealthVaultVocabularies.Exercise, observation.Code.Coding[0].Code);

            var detailExtensions = observation.Extension.Where(x => x.Url == "http://healthvault.com/exercise/detail").ToList();
            Assert.AreEqual(2, detailExtensions.Count());
            Assert.AreEqual("lap 1", ((FhirString)detailExtensions[0].Extension[0].Value).Value);
            Assert.AreEqual("exercise-activities:swimming", ((CodeableConcept)detailExtensions[0].Extension[1].Value).Coding[0].Code);
            Assert.AreEqual(30, ((Quantity)detailExtensions[0].Extension[2].Value).Value);
            Assert.AreEqual("s", ((Quantity)detailExtensions[0].Extension[2].Value).Unit);

            var segmentExtensions = observation.Extension.Where(x => x.Url == "http://healthvault.com/exercise/segment").ToList();
            Assert.AreEqual(1, segmentExtensions.Count());
            Assert.AreEqual("exercise-activities:swimming", ((CodeableConcept)segmentExtensions[0].Extension[0].Value).Coding[0].Code);

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
