using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
