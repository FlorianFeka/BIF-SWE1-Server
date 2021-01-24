using System;

namespace SimpleServer.Attributes.Verbs
{
    public abstract class HttpVerbAttribute : Attribute
    {
        public string SufixRoute { get; set; }
    }
}
