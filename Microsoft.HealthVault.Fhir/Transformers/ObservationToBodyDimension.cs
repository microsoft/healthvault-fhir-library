// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToBodyDimension
    {
        internal static BodyDimension ToBodyDimension(this Observation observation)
        {
            var bodyDimension = observation.ToThingBase<BodyDimension>();

            bodyDimension.Value = ObservationToHealthVault.GetThingValueFromQuantity<Length>(observation.Value as Quantity);
            bodyDimension.When = ObservationToHealthVault.GetApproximateDateTimeFromEffectiveDate(observation.Effective);

            if (observation.Code?.Coding != null)
            {
                foreach (var code in observation.Code.Coding)
                {
                    if (string.Equals(code.System, VocabularyUris.HealthVaultVocabulariesUri, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = code.Code.Split(':');
                        var vocabName = value[0];
                        var vocabCode = value.Length == 2 ? value[1] : null;

                        bodyDimension.SetMeasurementName(
                            code.Display,
                            vocabCode,
                            vocabName,
                            HealthVaultVocabularies.Wc, code.Version);
                    }
                    else
                    {
                        bodyDimension.SetMeasurementName(
                            code.Display,
                            code.Code,
                            HealthVaultVocabularies.Fhir,
                            code.System,
                            code.Version);
                    }
                }
            }

            return bodyDimension;
        }

        private static void SetMeasurementName(this BodyDimension bodyDimension, string display, string code, string vocabName, string family, string version)
        {
            if (bodyDimension.MeasurementName == null)
            {
                bodyDimension.MeasurementName = new CodableValue(display);
            }

            bodyDimension.MeasurementName.Add(new CodedValue(code, vocabName, family, version));
        }
    }
}
