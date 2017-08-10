// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToExercise
    {
        internal static Exercise ToExercise(this Observation observation)
        {
            var exercise = observation.ToThingBase<Exercise>();

            if (!observation.Extension.IsNullOrEmpty())
            {
                var detailExtensions = observation.Extension.Where(x => x.Url == "http://healthvault.com/exercise/detail").ToList();
                foreach (var detail in detailExtensions)
                {
                    ExerciseDetail exerciseDetail;
                    var key = GetExerciseDetailKey(detail, out exerciseDetail);

                    if (!string.IsNullOrEmpty(key))
                    {
                        exercise.Details.Add(key, exerciseDetail);
                    }
                }

                var segmentExtensions = observation.Extension.Where(x => x.Url == "http://healthvault.com/exercise/segment").ToList();
                foreach (var segment in segmentExtensions)
                {
                    var exerciseSegment = new ExerciseSegment();
                    foreach (var extension in segment.Extension)
                    {
                        switch (extension.Url)
                        {
                            case "exercise-segment-activity":
                                var activityCoding = ((CodeableConcept)extension.Value).Coding[0];
                                var value = activityCoding.Code.Split(':');
                                var vocabName = value[0];
                                var vocabCode = value.Length == 2 ? value[1] : null;

                                exerciseSegment.Activity = new CodableValue(activityCoding.Display, vocabCode, vocabName, activityCoding.System, activityCoding.Version);
                                break;
                            case "exercise-segment-title":
                                exerciseSegment.Title = ((FhirString)extension.Value).Value;
                                break;
                            case "exercise-segment-duration":
                                exerciseSegment.Duration = (double?)((FhirDecimal)extension.Value).Value;
                                break;
                            case "exercise-segment-distance":
                                var valueQuantity = (Quantity)extension.Value;
                                if (valueQuantity != null)
                                {
                                    exerciseSegment.Distance = new Length((double)valueQuantity.Value.Value);
                                }
                                break;
                            case "exercise-segment-offset":
                                exerciseSegment.Offset = (double?)((FhirDecimal)extension.Value).Value;
                                break;
                            case "exercise/detail":
                                ExerciseDetail exerciseDetail;
                                var key = GetExerciseDetailKey(extension, out exerciseDetail);

                                if (!string.IsNullOrEmpty(key))
                                {
                                    exerciseSegment.Details.Add(key, exerciseDetail);
                                }
                                break;
                        }
                    }
                }
            }

            foreach (var component in observation.Component)
            {
                if (component.Code?.Coding == null || component.Code?.Coding.Count == 0)
                {
                    continue;
                }

                var code = component.Code.Coding[0];
                if (string.Equals(code.System, VocabularyUris.HealthVaultVocabulariesUri, StringComparison.OrdinalIgnoreCase))
                {
                    var value = code.Code.Split(':');
                    var vocabCode = value.Length == 2 ? value[1] : null;

                    switch (vocabCode)
                    {
                        case HealthVaultVocabularies.ExerciseDistance:
                            var distanceQuantity = (Quantity)component.Value;
                            if (distanceQuantity?.Value != null)
                            {
                                exercise.Distance = new Length((double)distanceQuantity.Value);
                            }
                            break;
                        case HealthVaultVocabularies.ExerciseDuration:
                            var durationQuantity = (Quantity)component.Value;
                            if (durationQuantity?.Value != null)
                            {
                                exercise.Duration = (double)durationQuantity.Value;
                            }
                            break;
                        case HealthVaultVocabularies.ExerciseActivity:
                            if (component.Value != null)
                            {
                                var coding = ((CodeableConcept)component.Value).Coding[0];

                                var activityValue = coding.Code.Split(':');
                                var activityValueVocabName = activityValue[0];
                                var activityValueCode = activityValue.Length == 2 ? activityValue[1] : null;

                                exercise.SetActivity(coding.Display, activityValueCode, activityValueVocabName, HealthVaultVocabularies.Wc, coding.Version);
                            }
                            break;
                    }
                }

            }

            return exercise;
        }

        private static void SetActivity(this Exercise exercise, string display, string code, string vocabName, string family, string version)
        {
            if (exercise.Activity == null)
            {
                exercise.Activity = new CodableValue(display);
            }

            exercise.Activity.Add(new CodedValue(code, vocabName, family, version));
        }

        private static string GetExerciseDetailKey(Extension detail, out ExerciseDetail exerciseDetail)
        {
            string key = "";
            exerciseDetail = new ExerciseDetail();
            foreach (var extension in detail.Extension)
            {
                switch (extension.Url)
                {
                    case "exercise-detail-name":
                        key = ((FhirString)extension.Value).Value;
                        break;
                    case "exercise-detail-type":
                        var code = ((CodeableConcept)extension.Value).Coding[0];
                        var value = code.Code.Split(':');
                        var vocabName = value[0];
                        var vocabCode = value.Length == 2 ? value[1] : null;

                        exerciseDetail.Name = new CodedValue(vocabCode, vocabName, HealthVaultVocabularies.Wc, code.Version);
                        break;
                    case "exercise-detail-value":
                        exerciseDetail.Value = new StructuredMeasurement((double)((Quantity)extension.Value).Value.Value, new CodableValue());
                        break;
                }
            }
            return key;
        }
    }
}
