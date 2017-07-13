﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Fhir.Vocabularies
{
    internal class VocabToHealthVaultDictionaries
    {
        private static VocabToHealthVaultDictionaries s_instance;
        private static object _lockInstance = new object();

        internal Dictionary<string, string> Snomed { private set; get; }
        internal Dictionary<string, string> Loinc { private set; get; }

        private VocabToHealthVaultDictionaries()
        {
            Snomed = JsonConvert.DeserializeObject<Dictionary<string, string>>(GetMapping(@"snomed.json"));
            Loinc = JsonConvert.DeserializeObject<Dictionary<string, string>>(GetMapping(@"loinc.json"));
        }

        public static VocabToHealthVaultDictionaries Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (_lockInstance)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new VocabToHealthVaultDictionaries();
                        }
                    }
                }

                return s_instance;
            }
        }

        public static string GetMapping(string mappingFileName)
        {            
            string resourceName = $"Microsoft.HealthVault.Fhir.Data.{mappingFileName}";            
            using (Stream stream = typeof(VocabToHealthVaultDictionaries).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}