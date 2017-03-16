using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqTests.ZooDomain
{
    public class Tourist
    {
        IMonkey monkey;

        ~Tourist()
        {
            if (monkey != null)
                monkey.Dance -= Look_ADancingMonkey;
        }

        public int PhotosTaken { get; private set; }

        public void SeeAMonkey(IMonkey monkey)
        {
            if (this.monkey != monkey && this.monkey != null)
                this.monkey.Dance -= Look_ADancingMonkey;

            this.monkey = monkey;
            monkey.Dance += Look_ADancingMonkey;
        }

        public void Look_ADancingMonkey(object sender, EventArgs args)
        {
            if (((IMonkey)sender).CurrentFleaCount() <= 100)
            {
                TakeAPhoto((IMonkey)sender);
            }
            return;
        }

        private void TakeAPhoto(IMonkey monkey)
        {
            PhotosTaken++;
            Console.WriteLine(monkey.Name);
        }
    }
}
