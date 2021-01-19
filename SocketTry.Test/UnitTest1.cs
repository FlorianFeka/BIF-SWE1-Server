using System;
using Xunit;
using FluentAssertions;
using SocketTry.Http;

namespace SocketTry.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = new HttpServer();
            // can reach internal methods
            //a.StartFirstAccept();
        }
    }
}
