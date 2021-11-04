using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.types_of_peaces
{
    class King : Peace
    {
        public Boolean can_custle;

        public King(bool iswhitee, int position) : base(iswhitee, position)
        {
            this.can_custle = true;
            this.iswhite = iswhitee;
            this.type = 5;
            this.position = position;
        }
    }
}
