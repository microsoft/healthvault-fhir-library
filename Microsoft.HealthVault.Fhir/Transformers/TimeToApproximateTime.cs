// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Hl7.Fhir.Model;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Fhir.Transformers
{
    internal static class TimeToApproximateTime
    {
        internal static ApproximateTime ToAppoximateTime(this Time time)
        {
            // Accepted formats for time are found here: https://www.hl7.org/fhir/datatypes.html#time
            var approximateTime = new ApproximateTime();
            var timePortions = time.ToString().Split(':');

            approximateTime.Hour = int.Parse(timePortions[0]);
            approximateTime.Minute = int.Parse(timePortions[1]);
            approximateTime.Second = int.Parse(timePortions[2].Substring(0, 2));

            if (timePortions[2].Contains("."))
            {
                // milliseconds will always start at position 3, but we aren't guaranteed to have 3 digits after. 
                approximateTime.Millisecond = int.Parse(timePortions[2].Substring(3, Math.Min(timePortions[2].Length - 3, 3)).PadRight(3, '0'));
            }

            return approximateTime;
        }
    }
}