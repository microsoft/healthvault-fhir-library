using System;
using Hl7.Fhir.Model;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class FhirDateTimeExtensions
    {
        public enum FhirDateTimePrecision
        {
            Year = 4,       //2017
            Month = 7,      //2017-22
            Day = 10,       //2017-22-21
            Minute = 15,    //2017-22-21T13:45
            Second = 18    //2017-22-21T13:45:21
        }

        public static FhirDateTimePrecision Precision(this FhirDateTime fdt)
        {
            return (FhirDateTimePrecision)Math.Min(fdt.Value.Length, 18); 
        }
    }
}
