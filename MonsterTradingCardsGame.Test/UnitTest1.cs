using MonsterTradingCardsGame.Services;
using Xunit;
using FluentAssertions;

namespace MonsterTradingCardsGame.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var a = new UserService();
            a.Test().Should().BeTrue();
        }
    }
}
