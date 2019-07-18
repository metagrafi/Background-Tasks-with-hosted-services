using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException() { }
        public IPServiceNotAvailableException(string msg) : base(string.Format("IPStack Service {0}",msg)) { }
    }
}
