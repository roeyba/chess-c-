using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.types_of_peaces
{
    class Pawn : Peace
    {
        public Pawn(bool iswhitee, int position) : base(iswhitee, position)
        {
            this.iswhite = iswhitee;
            this.position = position;
            this.type = 0;
        }
    }
}
