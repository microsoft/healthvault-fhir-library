// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Codes.HealthVault;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class ObservationToVitalSigns
    {
        internal static VitalSigns ToVitalSigns(this Observation observation)
        {
            var vitalSigns = observation.ToThingBase<ItemTypes.VitalSigns>();

            var observationValue = observation.Value as Quantity;
            var vitalSignResult = new VitalSignsResultType();
            vitalSignResult.Value = (double?) observationValue.Value;
            vitalSignResult.Unit = new CodableValue("celcius", new CodedValue("Cel", "lab-results-units", "wc", "1"));
            CodableValue value = new CodableValue("Temperature", new CodedValue("Tmp", "vital-statistics", "wc", "1"));
            vitalSignResult.Title = value;

            vitalSigns.VitalSignsResults.Add(vitalSignResult);
            vitalSigns.When = ObservationToHealthVault.GetHealthVaultTimeFromEffectiveDate(observation.Effective);

            return vitalSigns;
        }
    }
}
