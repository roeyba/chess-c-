﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess.types_of_peaces
{
    class Rook : Peace
    {

        public Rook(bool iswhitee, int position) : base(iswhitee, position)
        {
            this.type = 3;
            
        }

        public List<Move> getmoves()
        {
            List<Move> moves = new List<Move>();
            this
            return moves;
        }

    }
}
