// Copyright (c) Get Real Health.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;

namespace Microsoft.HealthVault.Fhir.FhirExtensions
{
    public static class FhirDateTimeExtensions
    {
        /// <summary>
        /// FhirDateTimePrecision enum will determine whether FhirDateTime contains only 
        /// year(should contains 4 Digits format{0:D4})
        /// month(should contains 2 Digits format{0:D2})
        /// day(should contains 2 Digits format{0:D2})
        /// hour(should contains 2 Digits format{0:D2})
        /// minute(should contains 2 Digits format{0:D2})
        /// seconds(should contains 2 Digits format{0:D2})
        /// </summary>
        public enum FhirDateTimePrecision
        {
            Year = 4,       //2017
            Month = 7,      //2017-02
            Day = 10,       //2017-02-21
            Minute = 16,    //2017-02-21T13:45
            Second = 19    //2017-02-21T13:45:21
        }

        public static FhirDateTimePrecision Precision(this FhirDateTime fdt)
        {
            return (FhirDateTimePrecision)fdt.Value.Length; 
        }
    }
}
