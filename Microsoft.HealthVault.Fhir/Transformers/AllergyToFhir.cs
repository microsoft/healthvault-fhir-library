// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using static Hl7.Fhir.Model.AllergyIntolerance;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static partial class ThingBaseToFhir
    {
        public static AllergyIntolerance ToFhir(this Allergy allergy)
        {
            return AllergyToFhir.ToFhirInternal(allergy, ToFhirInternal<AllergyIntolerance>(allergy));
        }
    }
    internal static class AllergyToFhir
    {
        internal static AllergyIntolerance ToFhirInternal(Allergy allergy, AllergyIntolerance allergyInTolerance)
        {
            var allergyExtension = new Extension
            {
                Url = HealthVaultExtensions.Allergy
            };

            allergyInTolerance.Code = allergy.Name.ToFhir();
            
            if (allergy.FirstObserved != null)
            {
                allergyInTolerance.Onset = allergy.FirstObserved.ToFhir();
            }

            if (allergy.AllergenType != null)
            {
                allergyInTolerance.SetAllergyIntoleranceCategory(allergy.AllergenType, allergyExtension);
            }

            if (allergy.Reaction != null)
            {
                allergyInTolerance.Reaction = new List<AllergyIntolerance.ReactionComponent>
                {
                     new AllergyIntolerance.ReactionComponent {
                         Manifestation = new List<CodeableConcept>{ allergy.Reaction.ToFhir() },
                         Description = allergy.Reaction.Text
                     }
                };
            }

            if (allergy.TreatmentProvider != null)
            {
                allergyInTolerance.AddAsserter(allergy.TreatmentProvider.ToFhir());
            }

            if (allergy.Treatment != null)
            {
                allergyExtension.AddExtension(HealthVaultExtensions.AllergyTreatement, allergy.Treatment.ToFhir());
            }

            if (allergy.AllergenCode != null)
            {
                allergyExtension.AddExtension(HealthVaultExtensions.AllergenCode, allergy.AllergenCode.ToFhir());
            }

            if (allergy.IsNegated != null)
            {
                if (allergy.IsNegated.Value)
                {
                    allergyInTolerance.ClinicalStatus = AllergyIntoleranceClinicalStatus.Resolved;
                }
                else
                {
                    allergyInTolerance.ClinicalStatus = AllergyIntoleranceClinicalStatus.Active;
                }
            }

            if (allergy.CommonData != null)
            {
                var note = new Hl7.Fhir.Model.Annotation();
                note.Text = allergy.CommonData.Note;
                allergyInTolerance.Note = new List<Hl7.Fhir.Model.Annotation> { note };
            }

            allergyInTolerance.Type = AllergyIntoleranceType.Allergy;
            allergyInTolerance.Extension.Add(allergyExtension);

            return allergyInTolerance;

        }

        private static void SetAllergyIntoleranceCategory(this AllergyIntolerance allergyIntolerance, CodableValue allergenType, Extension allergyExtension)
        {
            foreach (CodedValue aValue in allergenType)
            {
                List<AllergyIntoleranceCategory?> lstAllergyIntoleranceCategory = new List<AllergyIntoleranceCategory?>();
                AllergyIntoleranceCategory allergyIntoleranceCategory;
                if (aValue.Value != null)
                {
                    if (Enum.TryParse(aValue.Value, true, out allergyIntoleranceCategory))
                    {
                        lstAllergyIntoleranceCategory.Add(allergyIntoleranceCategory);
                    }
                    else
                    {
                        allergyExtension.AddExtension(HealthVaultExtensions.AllergenType, new FhirString(aValue.Value));
                    }
                }
                if (lstAllergyIntoleranceCategory.Count > 0)
                    allergyIntolerance.Category = lstAllergyIntoleranceCategory;
            }
        }

        private static void AddAsserter(this AllergyIntolerance allergyIntolerance, Practitioner practitioner)
        {
            if (practitioner != null)
            {
                practitioner.Id = $"#practitioner-{Guid.NewGuid()}";
                allergyIntolerance.Contained.Add(practitioner);
                allergyIntolerance.Asserter = new ResourceReference
                {                    
                  Reference = practitioner.Id
                };
            }
        }
    }
}
