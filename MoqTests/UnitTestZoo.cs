using System;
using System.Text;
using System.Collections.Generic;
using Xunit;
using MoqTests.ZooDomain;
using Moq;

//Credits: 
//https://www.richard-banks.org/2010/07/mocking-comparison-part-1-basics.html

namespace MoqTests
{    
    public class UnitTestZoo
    {

        private string testName = "Spike";

        [Fact]
        public void Moq_arrange_act_assert()
        {
            var monkey = new Mock<IMonkey>();
            monkey.Setup(m => m.Name).Returns(testName);

            var actual = monkey.Object.Name;

            Assert.Equal(testName, actual);
        }

        [Fact]
        public void Moq_properties()
        {
            var monkey = new Mock<IMonkey>();
            monkey.SetupProperty(m => m.Name);

            monkey.Object.Name = "Spike";
            Assert.Equal("Spike", monkey.Object.Name);
        }

        [Fact]
        public void Moq_properties2()
        {
            var monkey = new Mock<IMonkey>();
            monkey.SetupAllProperties();

            monkey.Object.Name = "Spike";
            Assert.Equal("Spike", monkey.Object.Name);
        }


        [Fact]
        public void Moq_constraints()
        {
            var monkey = new Mock<IMonkey>();

            //monkey.Setup(m => m.TryAddFleas(It.IsAny<int>())).Returns(true);
            monkey.Setup(m => m.TryAddFleas(It.IsInRange(3, 10, Range.Exclusive))).Returns(true);

            Assert.Equal(true, monkey.Object.TryAddFleas(5));
            Assert.Equal(false, monkey.Object.TryAddFleas(-1));
            Assert.Equal(true, monkey.Object.TryAddFleas(9));
        }


        [Fact]
        public void Moq_repetitions()
        {
            var monkey = new Mock<IMonkey>();

            monkey.Object.TryAddFleas(5);
            monkey.Object.TryAddFleas(-1);
            monkey.Object.TryAddFleas(9);

            monkey.Verify(m => m.TryAddFleas(
              It.IsInRange(3, 10, Range.Exclusive)
             ),
             Times.Exactly(2));
            monkey.Verify(m => m.TryAddFleas(-1), Times.Once());
        }


        [Fact]
        public void Moq_multiple_calls()
        {
            var monkey = new Mock<IMonkey>();
            monkey.Setup(m => m.TryAddFleas(1)).Returns(true);

            Assert.True(monkey.Object.TryAddFleas(1));
            Assert.True(monkey.Object.TryAddFleas(1));
        }

        [Fact]
        public void Moq_throwing_exceptions()
        {
            var monkey = new Mock<IMonkey>();
            monkey.Setup(m => m.Name).Throws(new ApplicationException());
            Assert.Throws<ApplicationException>(() => monkey.Object.Name);
        }

        [Fact]
        public void Moq_recursive_mocks()
        {
            var monkey = new Mock<IMonkey>();
            monkey.Setup(m => m.Keeper().AssignedMonkey).Returns(monkey.Object);
            monkey.SetupProperty(m => m.Name);
            monkey.Object.Name = "Spike";
            Assert.NotNull(monkey.Object.Keeper());
            Assert.Equal("Spike", monkey.Object.Keeper().AssignedMonkey.Name);
        }

        [Fact]
        public void Moq_functions_and_callbacks()
        {
            var fleaCount = 25;
            var monkey = new Mock<IMonkey>();

            monkey.Setup(m => m.CurrentFleaCount()).Returns(() => fleaCount);
            monkey.Setup(m => m.TryAddFleas(It.IsAny<int>()))
             .Returns<int>(fleas =>
             {
                 fleaCount = fleas;
                 return true;
             });

            Assert.Equal(25, monkey.Object.CurrentFleaCount());

            monkey.Object.TryAddFleas(10);
            Assert.Equal(10, monkey.Object.CurrentFleaCount());
        }

        [Fact]
        public void Moq_event_subscriber()
        {
            var monkey = new Mock<IMonkey>();
            var keeper = new ZooKeeper();

            keeper.OnBananaReady += monkey.Object.BananaReady;

            keeper.FeedMonkeys();
            monkey.Verify(m => m.BananaReady(keeper,
             It.Is<BananaEventArgs>(b => b.IsRipe)));
        }

        [Fact]
        public void Moq_multiple_interfaces()
        {
            var monkey = new Mock<IMonkey>();
            monkey.As<IZooKeeper>();
            monkey.SetupProperty(m => m.Name);
            Mock.Get((IZooKeeper)monkey.Object).SetupProperty(k => k.AssignedMonkey);

            monkey.Object.Name = "Spike";
            ((IZooKeeper)monkey.Object).AssignedMonkey = monkey.Object;

            Assert.Equal("Spike", ((IZooKeeper)monkey.Object).AssignedMonkey.Name);

            Assert.IsAssignableFrom<IMonkey>(monkey.Object);
            Assert.IsAssignableFrom<IZooKeeper>(monkey.Object);
        }
    }
}
