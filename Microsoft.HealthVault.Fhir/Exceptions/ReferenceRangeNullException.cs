using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.HealthVault.Fhir.Exceptions
{
    public class ReferenceRangeNullException : Exception
    {
        public ReferenceRangeNullException() { }

        public ReferenceRangeNullException(string message)
            : base(message) { }

        public ReferenceRangeNullException(string message, Exception inner)
            :base(message, inner) { }
    }
}
