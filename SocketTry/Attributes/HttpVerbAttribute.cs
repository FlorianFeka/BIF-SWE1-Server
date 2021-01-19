using System;

namespace SocketTry.Attributes
{
    public abstract class HttpVerbAttribute : Attribute
    {
        public string SufixRoute { get; set; }
    }
}
