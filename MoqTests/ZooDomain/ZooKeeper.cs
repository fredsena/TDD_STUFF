using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqTests.ZooDomain
{
    public class ZooKeeper : IZooKeeper
    {
        public event EventHandler<BananaEventArgs> OnBananaReady;

        public IMonkey AssignedMonkey { get; set; }

        public void CleanMonkey()
        {
            if (AssignedMonkey == null) return;
            if (AssignedMonkey.CurrentFleaCount() < 10) return;
            if (AssignedMonkey.IsAwake(DateTime.Now))
                AssignedMonkey.Clean();
        }

        public void FeedMonkeys()
        {
            if (OnBananaReady != null)
            {
                OnBananaReady(this, new BananaEventArgs(true));
            }
        }
    }

    
}
