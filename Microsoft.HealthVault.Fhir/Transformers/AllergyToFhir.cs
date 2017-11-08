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
        internal static AllergyIntolerance ToFhirInternal(Allergy allergy, AllergyIntolerance allergyIntolerance)
        {
            var allergyExtension = new Extension
            {
                Url = HealthVaultExtensions.Allergy
            };

            allergyIntolerance.Code = allergy.Name.ToFhir();

            if (allergy.FirstObserved != null)
            {
                allergyIntolerance.Onset = allergy.FirstObserved.ToFhir();
            }

            if (allergy.AllergenType != null)
            {
                allergyIntolerance.SetAllergyIntoleranceCategory(allergy.AllergenType, allergyExtension);
            }

            if (allergy.Reaction != null)
            {
                allergyIntolerance.Reaction = new List<AllergyIntolerance.ReactionComponent>
                {
                     new AllergyIntolerance.ReactionComponent {
                         Manifestation = new List<CodeableConcept>{ allergy.Reaction.ToFhir() },
                         Description = allergy.Reaction.Text
                     }
                };
            }

            if (allergy.TreatmentProvider != null)
            {
                allergyIntolerance.AddAsserter(allergy.TreatmentProvider.ToFhir());
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
                    allergyIntolerance.ClinicalStatus = AllergyIntoleranceClinicalStatus.Resolved;
                }
                else
                {
                    allergyIntolerance.ClinicalStatus = AllergyIntoleranceClinicalStatus.Active;
                }
            }

            if (allergy.CommonData != null && allergy.CommonData.Note != null)
            {
                var note = new Hl7.Fhir.Model.Annotation();
                note.Text = allergy.CommonData.Note;
                allergyIntolerance.Note = new List<Hl7.Fhir.Model.Annotation> { note };
            }

            allergyIntolerance.Type = AllergyIntoleranceType.Allergy;
            allergyIntolerance.Extension.Add(allergyExtension);
            return allergyIntolerance;
        }

        private static void SetAllergyIntoleranceCategory(this AllergyIntolerance allergyIntolerance, CodableValue allergenType, Extension allergyExtension)
        {
            List<AllergyIntoleranceCategory?> lstAllergyIntoleranceCategory = new List<AllergyIntoleranceCategory?>();
            string aValue = allergenType.FirstOrDefault().Value;
            AllergyIntoleranceCategory allergyIntoleranceCategory;

            if (aValue != null)
            {
                if (Enum.TryParse(aValue, true, out allergyIntoleranceCategory))
                {
                    lstAllergyIntoleranceCategory.Add(allergyIntoleranceCategory);
                }
                else
                {
                    allergyExtension.AddExtension(HealthVaultExtensions.AllergenType, new FhirString(aValue));
                }
            }

            if (lstAllergyIntoleranceCategory.Count > 0)
            {
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
