using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Fhir.Units
{
    public class UnitResolver
    {
        private static UnitResolver _instance;

        public List<UnitConversion> Units { get; set; }

        private UnitResolver()
        {
            Units= JsonConvert.DeserializeObject<List<UnitConversion>>(File.ReadAllText(@"Data\UnitConversions.json"));
        }

        public static UnitResolver Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UnitResolver();
                }
                return _instance;
            }
        }
    }
}
