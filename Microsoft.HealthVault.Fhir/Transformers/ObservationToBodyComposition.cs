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
using UnitsNet;
using UnitsNet.Units;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToBodyComposition
    {
        internal static BodyComposition ToBodyComposition(this Observation observation)
        {
            var bodyComposition = observation.ToThingBase<ItemTypes.BodyComposition>();

            if (observation.Component != null)
            {
                var bodyCompositionValue = new BodyCompositionValue();

                foreach (var component in observation.Component)
                {
                    var componentValue = component.Value as Quantity;
                    if (componentValue?.Value == null)
                    {
                        continue;
                    }

                    switch (componentValue.Unit)
                    {
                        case UnitAbbreviations.Kilogram:
                            bodyCompositionValue.MassValue = new WeightValue((double)componentValue.Value.Value);
                            break;
                        case UnitAbbreviations.Percent:
                            bodyCompositionValue.PercentValue = (double?)componentValue.Value;
                            break;
                    }
                }

                bodyComposition.Value = bodyCompositionValue;
            }

            bodyComposition.When = ObservationToHealthVault.GetApproximateDateTimeFromEffectiveDate(observation.Effective);

            if (observation.Code?.Coding != null)
            {
                foreach (var code in observation.Code.Coding)
                {
                    if (string.Equals(code.System, VocabularyUris.HealthVaultVocabulariesUri, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = code.Code.Split(':');
                        var vocabName = value[0];
                        var vocabCode = value.Length == 2 ? value[1] : null;

                        switch (vocabName)
                        {
                            case HealthVaultVocabularies.BodyCompositionMeasurementMethods:
                                bodyComposition.SetMeasurementMethod(
                                    code.Display,
                                    vocabCode,
                                    vocabName,
                                    HealthVaultVocabularies.Wc, code.Version);
                                break;
                            case HealthVaultVocabularies.BodyCompositionSites:
                                bodyComposition.SetSite(
                                    code.Display,
                                    vocabCode,
                                    vocabName,
                                    HealthVaultVocabularies.Wc, code.Version);
                                break;
                            case HealthVaultVocabularies.BodyCompositionMeasurementNames:
                                bodyComposition.SetMeasurementName(
                                    code.Display,
                                    vocabCode,
                                    vocabName,
                                    HealthVaultVocabularies.Wc, code.Version);
                                break;
                        }
                    }
                }
            }

            return bodyComposition;
        }

        private static void SetMeasurementMethod(this BodyComposition bodyComposition, string display, string code, string vocabName, string family, string version)
        {
            if (bodyComposition.MeasurementMethod == null)
            {
                bodyComposition.MeasurementMethod = new CodableValue(display);
            }

            bodyComposition.MeasurementMethod.Add(new CodedValue(code, vocabName, family, version));
        }

        private static void SetSite(this BodyComposition bodyComposition, string display, string code, string vocabName, string family, string version)
        {
            if (bodyComposition.Site == null)
            {
                bodyComposition.Site = new CodableValue(display);
            }

            bodyComposition.Site.Add(new CodedValue(code, vocabName, family, version));
        }

        private static void SetMeasurementName(this BodyComposition bodyComposition, string display, string code, string vocabName, string family, string version)
        {
            if (bodyComposition.MeasurementName == null)
            {
                bodyComposition.MeasurementName = new CodableValue(display);
            }

            bodyComposition.MeasurementName.Add(new CodedValue(code, vocabName, family, version));
        }
    }
}
