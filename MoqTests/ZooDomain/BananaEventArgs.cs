using System;

namespace MoqTests.ZooDomain
{
    public class BananaEventArgs : EventArgs
    {
        public BananaEventArgs(bool isRipe)
        {
            IsRipe = isRipe;
        }

        public bool IsRipe { get; private set; }
    }
}
