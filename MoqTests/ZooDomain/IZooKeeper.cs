using System;

namespace MoqTests.ZooDomain
{
    public interface IZooKeeper
    {
        event EventHandler<BananaEventArgs> OnBananaReady;
        IMonkey AssignedMonkey { get; set; }
        void CleanMonkey();
        void FeedMonkeys();
    }
}
