// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using Microsoft.HealthVault.Fhir.Constants;
using Microsoft.HealthVault.ItemTypes;
using static Hl7.Fhir.Model.AllergyIntolerance;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class AllergyIntoleranceToHealthVault
    {
        public static Allergy ToHealthVault(this AllergyIntolerance allergyIntolerance)
        {
            if (allergyIntolerance.Code.IsNullOrEmpty())
            {
                throw new System.ArgumentException($"Can not transform a {typeof(AllergyIntolerance)} with no code into {typeof(Allergy)}");
            }

            var allergy = allergyIntolerance.ToThingBase<Allergy>();

            string allergenType = string.Empty;
            var allergyExtension = allergyIntolerance.GetExtension(HealthVaultExtensions.Allergy);
            if (allergyExtension != null)
            {
                allergenType = allergyExtension.GetStringExtension(HealthVaultExtensions.AllergenType);
                allergy.AllergenCode = allergyExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.AllergenCode)?.ToCodableValue();
                allergy.Treatment = allergyExtension.GetExtensionValue<CodeableConcept>(HealthVaultExtensions.AllergyTreatement)?.ToCodableValue();
            }

            var coding = allergyIntolerance.Code.Coding.FirstOrDefault();

            if (HealthVaultVocabularies.SystemContainsHealthVaultUrl(coding.System))
            {
                allergy.Name = allergyIntolerance.Code.ToCodableValue();
            }
            else
            {
                allergy.SetAllergyName(
                    coding.Display,
                    coding.Code,
                    HealthVaultVocabularies.Fhir,
                    coding.System,
                    coding.Version);
            }

            if (allergyIntolerance.Reaction != null && allergyIntolerance.Reaction.Count > 0)
            {
                var code = allergyIntolerance.Reaction.FirstOrDefault().Manifestation.FirstOrDefault().Coding.First();

                if (HealthVaultVocabularies.SystemContainsHealthVaultUrl(code.System))
                {
                    allergy.Reaction = allergyIntolerance.Reaction.FirstOrDefault().Manifestation.FirstOrDefault().ToCodableValue();
                }
                else
                {
                    allergy.SetAllergyReaction(
                        code.Display,
                        code.Code,
                        HealthVaultVocabularies.Fhir,
                        code.System,
                        code.Version);
                }
            }

            if (allergyIntolerance.Onset != null)
            {
                allergy.FirstObserved = allergyIntolerance.Onset.ToAproximateDateTime();
            }

            if (!string.IsNullOrWhiteSpace(allergenType))
            {
                allergy.AllergenType = new CodableValue(allergenType)
                {
                    new CodedValue(allergenType, HealthVaultVocabularies.AllergenType, HealthVaultVocabularies.Wc, "1")
                };
            }
            else if (allergyIntolerance.Category != null && allergyIntolerance.Category.Count() > 0)
            {
                allergy.AllergenType = new CodableValue(allergyIntolerance.Category.FirstOrDefault().Value.ToString())
                {
                    new CodedValue(allergyIntolerance.Category.FirstOrDefault().Value.ToString(), FhirCategories.HL7Allergy, HealthVaultVocabularies.Fhir, "")
                };
            }

            if (allergyIntolerance.ClinicalStatus.HasValue)
            {
                if ((allergyIntolerance.ClinicalStatus.Value == (AllergyIntoleranceClinicalStatus.Resolved)) || (allergyIntolerance.ClinicalStatus.Value == (AllergyIntoleranceClinicalStatus.Inactive)))
                {
                    allergy.IsNegated = true;
                }
                else
                {
                    allergy.IsNegated = false;
                }
            }

            if (allergyIntolerance.Asserter != null)
            {
                allergy.TreatmentProvider = GetProvider(allergyIntolerance);
            }

            return allergy;
        }

        private static PersonItem GetProvider(this AllergyIntolerance allergyIntolerance)
        {
            if (allergyIntolerance.Asserter.IsContainedReference)
            {
                var containedReference = allergyIntolerance.Contained.SingleOrDefault(resouce =>
                    resouce.Id.Equals(allergyIntolerance.Asserter.Reference) && resouce.GetType().Equals(typeof(Practitioner)));

                if (containedReference == null)
                    return null;

                return (containedReference as Practitioner).ToHealthVault();
            }

            if (string.IsNullOrEmpty(allergyIntolerance.Asserter.Display))
                return null;

            return new PersonItem()
            {
                Name = new Name(allergyIntolerance.Asserter.Display)
            };
        }

        private static void SetAllergyName(this Allergy allergy, string display, string code, string vocabName, string family, string version)
        {
            if (allergy.Name == null || allergy.Name.Text == null)
            {
                allergy.Name = new CodableValue(display);
            }

            allergy.Name.Add(new CodedValue(code, vocabName, family, version));
        }

        private static void SetAllergyReaction(this Allergy allergy, string display, string code, string vocabName, string family, string version)
        {
            if (allergy.Reaction == null || allergy.Reaction.Text == null)
            {
                allergy.Reaction = new CodableValue(display);
            }

            allergy.Reaction.Add(new CodedValue(code, vocabName, family, version));
        }
    }
}
