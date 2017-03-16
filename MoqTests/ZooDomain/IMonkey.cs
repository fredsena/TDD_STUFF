using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqTests.ZooDomain
{
    public interface IMonkey
    {
        string Name { get; set; }
        bool TryAddFleas(int numberOfFleas);
        int CurrentFleaCount();
        void Clean();
        bool IsAwake(DateTime timeOfDay);
        void BananaReady(object sender, BananaEventArgs e);

        event EventHandler<EventArgs> Dance;
        IZooKeeper Keeper();
    }
}
