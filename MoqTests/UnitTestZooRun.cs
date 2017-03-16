using System;
using Xunit;
using Moq;
using MoqTests.ZooDomain;

namespace MoqTests
{
    
    public class UnitTestZooRun
    {
        [Fact]
        public void TestMethod1()
        {            
            IZooKeeper zoo = new ZooKeeper();

            IMonkey monkey = new Monkey();

            zoo.AssignedMonkey = monkey;

            zoo.CleanMonkey();

        }
    }
}
