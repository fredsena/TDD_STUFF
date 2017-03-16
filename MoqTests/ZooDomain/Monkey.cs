using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqTests.ZooDomain
{
    public class Monkey : IMonkey
    {

        private string name;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int NumberOfFleas { get; set; }

        public event EventHandler<EventArgs> Dance;


        public void BananaReady(object sender, BananaEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
            return;
        }

        public int CurrentFleaCount()
        {
            return NumberOfFleas;
        }

        public bool IsAwake(DateTime timeOfDay)
        {
            return DateTime.Now > timeOfDay;
        }

        public IZooKeeper Keeper()
        {
            return null;
        }

        public bool TryAddFleas(int numberOfFleas)
        {
            NumberOfFleas+= numberOfFleas;

            return NumberOfFleas < 10;
        }
    }
}
