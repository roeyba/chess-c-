using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    // create a data stract - Move that containe the start and end of a move
    // stract - a way of orginizing data
    public struct Move
    {
        public readonly int startsquare;
        public readonly int endsquare;
        public Peace capturedpeace;

        // the user create new move, when making the move its cheked if it is a ligal move and if so 'capturedpeace' is added if necesary
        public Move(int startsquare, int endsquare)
        {
            this.startsquare = startsquare;
            this.endsquare = endsquare;
            capturedpeace = null;
        }
    }

    class Movegenerator
    {
        public static List<Move> generate_moves(chessboard c)
        {
            List<Move> moves= new List<Move>();
            //check whose turn it is to play
            //go over all of the peaces of the curren player and for each peace add its moves to the "moves" list
            //go over all of the peaces and delete the moves where the curent player peaces are interfiting with the move
            //delete the moves that cause the curent player king to be in check



            return moves;
        }
    }
}
