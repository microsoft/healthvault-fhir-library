using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    public static class HumanNameToHealthVault
    {
        public static Name ToHealthVault(this HumanName fhirName)
        {
            var name = new ItemTypes.Name()
            {
                Last = fhirName.Family,
                Suffix = fhirName.Suffix.Any() ? new ItemTypes.CodableValue(fhirName.Suffix.First()) : null,
                Title = fhirName.Prefix.Any() ? new ItemTypes.CodableValue(fhirName.Prefix.First()) : null,
                First = fhirName.Given.FirstOrDefault() ?? string.Empty,
                Middle = fhirName.Given.ElementAtOrDefault(1)                
            };
            if (!string.IsNullOrEmpty(fhirName.Text))
            {
                name.Full = fhirName.Text;
            }
            return name;
        }
    }
}
