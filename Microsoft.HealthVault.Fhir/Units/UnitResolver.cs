// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Fhir.Units
{
    internal class UnitResolver
    {
        private static volatile UnitResolver s_instance;
        private static object _lockInstance = new object();

        public List<UnitConversion> UnitConversions { get; set; }

        private UnitResolver()
        {
            UnitConversions = JsonConvert.DeserializeObject<List<UnitConversion>>(File.ReadAllText(@"Data\UnitConversions.json"));
        }

        public static UnitResolver Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (_lockInstance)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new UnitResolver();
                        }
                    }
                }
                return s_instance;
            }
        }
    }
}
