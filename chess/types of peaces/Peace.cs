﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    public class Peace
    {
        //const is unchangeble variable and it is static
        public const int Pawn   = 0;
        public const int Knight = 1;
        public const int Bishop = 2;
        public const int Rook   = 3;
        public const int Queen  = 4;
        public const int King   = 5;
        public int type;
        public bool iswhite;
        public int position; // 0-63 number represent location, the count starts from left to right for each row from down to up

        public Peace(bool iswhite, int position)
        {
            this.iswhite = iswhite;
            this.position = position;
        }

        public Peace(Peace p)
        {
            this.type = p.type;
            this.iswhite = p.iswhite;
            this.position = p.position;
        }

        //return string representation of all of the info about the peace
        override public string ToString()
        {
            if(iswhite)
                return this.get_type_char_rep() + ", white, position:[" + this.get_i_pos() + "," + this.get_j_pos() + "]";
            else
                return this.get_type_char_rep() + ", black, position:["+ this.get_i_pos() + ","+ this.get_j_pos() + "]";
        }
        
        public char get_type_char_rep()
        {
            switch (this.type)
            {
                case 0:
                    if(iswhite)
                        return 'P';
                    return 'p';
                case 1:
                    if (iswhite)
                        return 'N';
                    return 'n';
                //
                case 2:
                    if (iswhite)
                        return 'B';
                    return 'b';
                case 3:
                    if (iswhite)
                        return 'R';
                    return 'r';
                //
                case 4:
                    if (iswhite)
                        return 'Q';
                    return 'q';
                //
                case 5:
                    if (iswhite)
                        return 'K';
                    return 'k';
                    //
            }
            return '#';
        }

        public int[] get_xy_pos()
        {
            return new int[] { this.position / 8, this.position % 8 };
        }
        public int get_i_pos()
        {//return the i value of the peace the board matrix
            return this.position / 8;
        }
        public int get_j_pos()
        {//return the i value of the peace the board matrix
            return this.position % 8;
        }

        public List<Move> getmoves_sliding_pc()
        {
            List<Move> moves = new List<Move>();

            return moves;
        }

    }
}
