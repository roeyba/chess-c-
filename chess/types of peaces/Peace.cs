﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    public class Peace
    {
        //const is unchangeble variable and it's static
        //all of the values are in centipawns https://www.chessprogramming.org/Centipawns
        public const int Pawn   = 0; public const int Pawn_value = 100;
        public const int Knight = 1; public const int Knight_value = 320;
        public const int Bishop = 2; public const int Bishop_value = 330;
        public const int Rook   = 3; public const int Rook_value = 500;
        public const int Queen  = 4; public const int Queen_value = 900;
        public const int King   = 5; //public const int King_value = 99999;
        public int type;
        public bool iswhite;
        public int position; // 0-63 number represent location, the count starts from left to right for each row from down to up
        //if a rook got eaten and needs to come back with the posibility to castle than his position = -1*position;
        public Peace(bool iswhite, int position)
        {
            this.iswhite = iswhite;
            this.position = position;
        }

        public Peace(bool iswhite, int position, int type)
        {
            this.type = type;
            this.iswhite = iswhite;
            this.position = position;
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
        public int GetPieceValue()
        {
            switch (this.type)
            {
                case 0:
                    return Pawn_value;
                case 1:
                    return Knight_value;
                case 2:
                    return Bishop_value;
                case 3:
                    return Rook_value;
                case 4:
                    return Queen_value;
            }
            return int.MaxValue;
        }

    }
}
