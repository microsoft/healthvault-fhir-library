using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.HealthVault.Fhir.Units
{
    public class UnitConversion
    {
        public string Code { get; set; }
        public string UnitsNetType { get; set; }
        public string UnitsNetUnitEnum { get; set; }
        public string UnitsNetSource { get; set; }
        public string UnitsNetDestination { get; set; }
    }
}
