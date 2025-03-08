using System;

namespace Assets._Scripts
{
    public class PlayerDeadEventArgs : EventArgs
    {
        public ParentPlayerScript player;
        public PlayerDeadEventArgs(ParentPlayerScript player)
        {
            this.player = player;
        }
    }

}
