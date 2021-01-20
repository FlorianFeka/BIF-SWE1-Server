using System;

namespace SocketTry.Attributes.Verbs
{
    public abstract class HttpVerbAttribute : Attribute
    {
        public string SufixRoute { get; set; }
    }
}
