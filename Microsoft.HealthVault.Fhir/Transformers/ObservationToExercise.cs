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
using Microsoft.HealthVault.Fhir.Codings;
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
                var detailExtensions = observation.Extension.Where(x => x.Url == HealthVaultExtensions.ExerciseDetail).ToList();
                foreach (var detail in detailExtensions)
                {
                    ExerciseDetail exerciseDetail;
                    var key = GetExerciseDetail(detail, out exerciseDetail);

                    if (!string.IsNullOrEmpty(key))
                    {
                        exercise.Details.Add(key, exerciseDetail);
                    }
                }

                var segmentExtensions = observation.Extension.Where(x => x.Url == HealthVaultExtensions.ExerciseSegment).ToList();
                foreach (var segment in segmentExtensions)
                {
                    exercise.Segments.Add(CreateExerciseSegment(segment));
                }
            }

            foreach (var component in observation.Component)
            {
                if (string.IsNullOrEmpty(component.Code?.Text))
                {
                    continue;
                }

                switch (component.Code.Text)
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
                        exercise.Activity = ((CodeableConcept)component.Value).ToCodableValue();

                        break;
                }

            }

            return exercise;
        }

        private static ExerciseSegment CreateExerciseSegment(Extension segment)
        {
            var exerciseSegment = new ExerciseSegment();
            foreach (var extension in segment.Extension)
            {
                switch (extension.Url)
                {
                    case HealthVaultExtensions.ExerciseSegmentActivity:
                        exerciseSegment.Activity = ((CodeableConcept) extension.Value).ToCodableValue();

                        break;
                    case HealthVaultExtensions.ExerciseSegmentTitle:
                        exerciseSegment.Title = ((FhirString) extension.Value).Value;

                        break;
                    case HealthVaultExtensions.ExerciseSegmentDuration:
                        exerciseSegment.Duration = (double?) ((FhirDecimal) extension.Value).Value;

                        break;
                    case HealthVaultExtensions.ExerciseSegmentDistance:
                        var valueQuantity = (Quantity) extension.Value;
                        if (valueQuantity?.Value != null)
                        {
                            exerciseSegment.Distance = new Length((double) valueQuantity.Value);
                        }

                        break;
                    case HealthVaultExtensions.ExerciseSegmentOffset:
                        exerciseSegment.Offset = (double?) ((FhirDecimal) extension.Value).Value;

                        break;
                    case HealthVaultExtensions.ExerciseDetail:
                        ExerciseDetail exerciseDetail;
                        var key = GetExerciseDetail(extension, out exerciseDetail);

                        if (!string.IsNullOrEmpty(key))
                        {
                            exerciseSegment.Details.Add(key, exerciseDetail);
                        }

                        break;
                }
            }
            return exerciseSegment;
        }

        private static string GetExerciseDetail(Extension detail, out ExerciseDetail exerciseDetail)
        {
            var key = "";
            exerciseDetail = new ExerciseDetail();

            foreach (var extension in detail.Extension)
            {
                if (extension.Value == null)
                {
                    continue;
                }

                switch (extension.Url)
                {
                    case HealthVaultExtensions.ExerciseDetailName:
                        key = ((FhirString)extension.Value).Value;

                        break;
                    case HealthVaultExtensions.ExerciseDetailType:
                        exerciseDetail.Name = ((Coding)extension.Value).ToCodedValue();

                        break;
                    case HealthVaultExtensions.ExerciseDetailValue:
                        var detailQuantity = (Quantity)extension.Value;

                        if(detailQuantity?.Value != null)
                        { 
                            exerciseDetail.Value = new StructuredMeasurement(
                                (double)detailQuantity.Value, 
                                CodeToHealthVaultHelper.CreateCodableValueFromQuantityValues(detailQuantity.System, detailQuantity.Code, detailQuantity.Unit)
                                );
                        }

                        break;
                }
            }

            return key;
        }
    }
}
