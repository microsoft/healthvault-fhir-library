// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Fhir.UnitTests.ToHealthVaultTests
{
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Microsoft.HealthVault.ItemTypes;
    using Microsoft.HealthVault.Fhir.Transformers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ObservationToHealthVaultWeightTests
    {
        [TestMethod]
        public void WeightToHealthVault_Successful()
        {
            var json = @"{
                  'resourceType': 'Observation',
                  'id': 'example',
                  'text': {
                    'status': 'generated'                   
                  },
                  'status': 'final',
                  'category': [
                    {
                      'coding': [
                        {
                          'system': 'http://hl7.org/fhir/observation-category',
                          'code': 'vital-signs',
                          'display': 'Vital Signs'
                        }
                      ]
                    }
                  ],
                  'code': {
                    'coding': [
                      {
                        'system': 'http://loinc.org',
                        'code': '29463-7',
                        'display': 'Body Weight'
                      },
                      {
                        'system': 'http://loinc.org',
                        'code': '3141-9',
                        'display': 'Body weight Measured'
                      },
                      {
                        'system': 'http://snomed.info/sct',
                        'code': '27113001',
                        'display': 'Body weight'
                      },
                      {
                        'system': 'http://acme.org/devices/clinical-codes',
                        'code': 'body-weight',
                        'display': 'Body Weight'
                      },
                      { 
                        'system': 'http://healthvault.com/data-types/vital-statistics',
                        'code': 'Wgt',
                        'display': 'Body Weight'
                      }   
                    ]
                  },
                  'subject': {
                    'reference': 'Patient/example'
                  },
                  'context': {
                    'reference': 'Encounter/example'
                  },
                  'effectiveDateTime': '2016-03-28',
                  'valueQuantity': {
                    'value': 67,
                    'unit': 'kg',
                    'system': 'http://unitsofmeasure.org',
                    'code': 'kg'
                  }
                }";

            var fhirParser = new FhirJsonParser();
            var observation = fhirParser.Parse<Observation>(json);

            var weight = observation.ToHealthVault<Weight>();
            Assert.IsNotNull(weight);
            Assert.AreEqual(67, weight.Value.Kilograms);
            Assert.AreEqual("kg", weight.Value.DisplayValue.Units);
            Assert.AreEqual("kg", weight.Value.DisplayValue.UnitsCode);
        }
    }
}
