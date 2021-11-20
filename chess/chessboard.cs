using chess.types_of_peaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    public struct Tile
    {
        //public Boolean isocupied;
        public Peace Peace;

        public Tile(Peace peace = null)
        {
            this.Peace = peace;
        }
        
        public Boolean isocupied()
        {
            if (this.Peace == null)
                return false;
            return true;
        }
    }

    public class chessboard
    {
        public const int board_size = 8;
        public const int peaces_types_amount = 6;
        public List<Peace>[] white_parts = new List<Peace>[peaces_types_amount]; // | 0pawns | 1knights | 2bishops | 3rooks | 4queens | 5king
        public List<Peace>[] black_parts = new List<Peace>[peaces_types_amount]; // | 0pawns | 1knights | 2bishops | 3rooks | 4queens | 5king
        public Tile[,] board = new Tile[board_size, board_size]; // 8x8 board that represent the chess board
        public Stack<Move> moves = new Stack<Move>(40); // in an average chess game there are 40 moves

        public Boolean whitetomove = false;// true is white and false is black

        /*   (i)matrix representations
         *      +---+---+---+---+---+---+---+---+
         *    0 |0,0|0,1|0,2|0,3|0,4|0,5|0,6|0,7|
         *      +---+---+---+---+---+---+---+---+
         *    1 |1,0|1,1|1,2|1,3|1,4|1,5|1,6|1,7|
         *      +---+---+---+---+---+---+---+---+
         *    2 |2,0|2,1|2,2|2,3|2,4|2,5|2,6|2,7|
         *      +---+---+---+---+---+---+---+---+
         *    3 |3,0|3,1|3,2|3,3|3,4|3,5|3,6|3,7|
         *      +---+---+---+---+---+---+---+---+
         *    4 |4,0|4,1|4,2|4,3|4,4|4,5|4,6|4,7|
         *      +---+---+---+---+---+---+---+---+
         *    5 |5,0|5,1|5,2|5,3|5,4|5,5|5,6|5,7|
         *      +---+---+---+---+---+---+---+---+
         *    6 |6,0|6,1|6,2|6,3|6,4|6,5|6,6|6,7|
         *      +---+---+---+---+---+---+---+---+
         *    7 |7,0|7,1|7,2|7,3|7,4|7,5|7,6|7,7|
         *      +---+---+---+---+---+---+---+---+
         *        0   1   2   3   4   5   6   7  (j)
         *        
         *        
         *        
         *   (i)
         *      +---+---+---+---+---+---+---+---+
         *    0 | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
         *      +---+---+---+---+---+---+---+---+
         *    1 | 8 | 9 |10 |11 |12 |13 |14 |15 |
         *      +---+---+---+---+---+---+---+---+
         *    2 |16 |17 |18 |19 |20 |21 |22 |23 |
         *      +---+---+---+---+---+---+---+---+
         *    3 |24 |25 |26 |27 |28 |29 |30 |31 |
         *      +---+---+---+---+---+---+---+---+
         *    4 |32 |33 |34 |35 |36 |37 |38 |39 |
         *      +---+---+---+---+---+---+---+---+
         *    5 |40 |41 |42 |43 |44 |45 |46 |47 |
         *      +---+---+---+---+---+---+---+---+
         *    6 |48 |49 |50 |51 |52 |53 |54 |55 |
         *      +---+---+---+---+---+---+---+---+
         *    7 |56 |57 |58 |59 |60 |61 |62 |63 |
         *      +---+---+---+---+---+---+---+---+
         *        0   1   2   3   4   5   6   7  (j)
        */

        //initialize all of the objects to null
        private void initialize_objects() 
        {
            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    this.board[i, j] = new Tile(); //makes a blank board and initialize all tiles
                }
            }
            for (int i = 0; i < peaces_types_amount; i++)
            {
                this.white_parts[i] = new List<Peace>();
                this.black_parts[i] = new List<Peace>();
            }
            // in a chess game every side can only have 1 king
            this.white_parts[5].Capacity = 1;
            this.black_parts[5].Capacity = 1;

            // in a chess game every side can only have 8 pawns
            this.white_parts[0].Capacity = 8;
            this.black_parts[0].Capacity = 8;
        }

        //create an empty chess board and initialize it to beggining chess position 
        public chessboard()
        {
            initialize_objects();

            //create all of the pawns
            for (int j = 0; j < 8; j++) 
            {
                addpeacetoboardandlist(new Pawn(false, j + 8));
                if (j == 3)
                {
                    addpeacetoboardandlist(new Pawn(true, j + 24));
                }
                else
                    addpeacetoboardandlist(new Pawn(true, j + 48));
            }
            //create all of the knights
            addpeacetoboardandlist(new Knight(false, 1)); addpeacetoboardandlist(new Knight(false, 6));
            addpeacetoboardandlist(new Knight(true, 57)); addpeacetoboardandlist(new Knight(true, 62));

            //create all of the bishops
            addpeacetoboardandlist(new Bishop(false, 2)); addpeacetoboardandlist(new Bishop(false, 5));
            addpeacetoboardandlist(new Bishop(true, 58)); addpeacetoboardandlist(new Bishop(true, 61));

            //create all of the rooks
            addpeacetoboardandlist(new Rook(false, 0)); addpeacetoboardandlist(new Rook(false, 7));
            addpeacetoboardandlist(new Rook(true, 56)); addpeacetoboardandlist(new Rook(true, 63));

            //create all of the queens
            addpeacetoboardandlist(new Queen(false, 4)); addpeacetoboardandlist(new Queen(true, 60));

            //create all of the kings
            addpeacetoboardandlist(new King(false, 3)); addpeacetoboardandlist(new King(true, 59));
            
        }

        //add peace in a unocupied Tile
        private void addpeacetoboardandlist(Peace peace) 
        {
            board[peace.get_i_pos(), peace.get_j_pos()].Peace = peace;
            if (peace.iswhite)
                this.white_parts[peace.type].Add(peace);
            else
                this.black_parts[peace.type].Add(peace);
        }
        private void removepeacefromlist(Peace peace)
        {
            if (peace.iswhite)
                this.white_parts[peace.type].Remove(peace);
            else
                this.black_parts[peace.type].Remove(peace);
        }

        //takes a move - no matter whose turn to play it is!!!!
        //no matter if there is a peace from the same color in it!!!!
        //not chacking if the peace than need to move exist!!!
        //not cheking if the move is legal
        public void manualy_makemove(Move move)
        {
            int istartpo = get_i_pos(move.startsquare);
            int jstartpo = get_j_pos(move.startsquare);
            //
            int iendpo = get_i_pos(move.endsquare);
            int jendpo = get_j_pos(move.endsquare);
            //
            
            if (this.board[iendpo, jendpo].isocupied())
            {
                move.capturedpeace = this.board[iendpo, jendpo].Peace; //save the captured peace
                removepeacefromlist(move.capturedpeace);
            }
            else if (this.board[istartpo, jstartpo].Peace.type == Peace.Pawn && jstartpo != jendpo) // if a pawn moves diagonaly    // En passant movement!
            {
                move.capturedpeace = this.board[istartpo, jendpo].Peace; //save the captured pawn
                removepeacefromlist(move.capturedpeace);
                this.board[istartpo, jendpo].Peace = null;//the captured peace squer isnt ocupied anymore
            }

            //change board representation
            this.board[iendpo, jendpo].Peace = this.board[istartpo, jstartpo].Peace; // change the position of the mover in the board
            this.board[iendpo, jendpo].Peace.position = move.endsquare;//change the position of the mover in the list
            this.board[istartpo, jstartpo].Peace = null;//the current squer isnt ocupied
            
            moves.Push(move);
            switchplayerturn();
        }


        public void humam_makemove(Move move)
        {
            /*
            for (int i = 0; i < chessboard.peaces_types_amount; i++)
            {
                foreach (Peace peace in ch.getplayerlistpointer(false)[i])
                {
                    if (peace.position == endsquare)
                    {
                        capturedpeace = peace;
                        break;
                    }
                }
                if (this.capturedpeace != null)
                    break;
            }
            */
            //only if its your turn than does the move
            // put a peace in a new place, if a there is a peace there than save it in the move
            //if the peace is from the same color, dont make the move - it is ilugal
            //make a pawn special move possible
            moves.Push(move);
            switchplayerturn();
        }

        //return the chess board position back before the last move that has been made
        public void unmakemove()
        {
            Move move = this.moves.Pop();
            int istartpo = get_i_pos(move.startsquare);
            int jstartpo = get_j_pos(move.startsquare);
            //
            int iendpo = get_i_pos(move.endsquare);
            int jendpo = get_j_pos(move.endsquare);
            //

            
            //move the peace position value back
            this.board[iendpo, jendpo].Peace.position = move.startsquare;
            //move the peace position back in the board
            this.board[istartpo, jstartpo].Peace = this.board[iendpo, jendpo].Peace;//the start squer is now ocupied
            
            //
            //
            if(move.capturedpeace != null) //if a peace needs to come back
            {
                //Console.WriteLine(ToStringfromlist());
                //Console.WriteLine(ToString());
                //Console.WriteLine(move.capturedpeace.ToString()+"\n");

                //add the captured peace in the board
                this.board[move.capturedpeace.get_i_pos(), move.capturedpeace.get_j_pos()].Peace = move.capturedpeace;

                if(move.endsquare != move.capturedpeace.position) // if its a En passant move
                    this.board[iendpo, jendpo].Peace = null;//the end squer isnt ocupied anymore

                //add the captured peace in the list
                if (move.capturedpeace.iswhite)
                    this.white_parts[move.capturedpeace.type].Add(move.capturedpeace);
                else
                    this.black_parts[move.capturedpeace.type].Add(move.capturedpeace);
            }
            else
            {
                this.board[iendpo, jendpo].Peace = null;//the end squer isnt ocupied
            }
            switchplayerturn();  
        }

        //switch the boolean value indicates which player turn it is to play
        public void switchplayerturn()
        {
            this.whitetomove = !this.whitetomove;
        }

        //return a string representation of the board
        override public string ToString()
        {
            string tmp = "";
            for (int i = 0; i < board_size * 2 + 1; i++)
            {
                if (i % 2 == 0)
                    tmp += "   +---+---+---+---+---+---+---+---+\n";
                else
                {
                    tmp += " " + i / 2 + " ";
                    for (int j = 0; j < board_size; j++)
                    {
                        if(board[i / 2, j].isocupied())
                            tmp += "| " + board[i / 2, j].Peace.get_type_char_rep() + " ";
                        else
                            tmp += "|   ";
                    }
                    tmp += "|\n";
                }
            }
            tmp += "     0   1   2   3   4   5   6   7  \n";
            return tmp;
        }

        //return a string representation of the board from the list - used for testing
        public string ToStringfromlist()
        { //return a string representation of the board
            char[,] board2 = new char[board_size, board_size];
            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    board2[i, j] = '.';
                }
            }
            int blackpeacecont = 0;
            int whitepeacecont = 0;
            for (int i = 0; i < chessboard.peaces_types_amount; i++)
            {
                foreach (Peace peace in this.white_parts[i])
                {
                    if (board2[get_i_pos(peace.position), get_j_pos(peace.position)] == '.')
                    {
                        Console.WriteLine(peace.ToString());
                        board2[get_i_pos(peace.position), get_j_pos(peace.position)] = peace.get_type_char_rep();
                        whitepeacecont++;
                    }
                    else
                        Console.WriteLine("second peace on the same position: " + peace.ToString());
                }
                foreach (Peace peace in this.black_parts[i])
                {
                    if (board2[get_i_pos(peace.position), get_j_pos(peace.position)] == '.')
                    {
                        Console.WriteLine(peace.ToString());
                        board2[get_i_pos(peace.position), get_j_pos(peace.position)] = peace.get_type_char_rep();
                        blackpeacecont++;
                    }
                    else
                        Console.WriteLine("second peace on the same position: " + peace.ToString());
                }
            }
            Console.WriteLine("black peace count: "+ blackpeacecont);
            Console.WriteLine("white peace count: " + whitepeacecont);
            string tmp = "string representation from list representation:\n";
            for (int i = 0; i < board_size * 2+1; i++)
            {
                if (i % 2 == 0)
                    tmp += "   +---+---+---+---+---+---+---+---+\n";
                else 
                {
                    tmp += " "+i / 2 +" ";
                    for (int j = 0; j < board_size; j++)
                    {
                        tmp += "| "+ board2[i/2, j].ToString() +" ";
                        //tmp += "| " + "X" + " ";
                    }
                    tmp += "|\n";
                }
            }
            tmp += "     0   1   2   3   4   5   6   7  \n";
            return tmp;
        }

        
        //gets i/j index for the board matrix representation from a one number position
        public static int get_i_pos(int position)
        {//return the i value of the peace/move the board matrix
            return position / 8;
        }
        public static int get_j_pos(int position)
        {//return the i value of the peace/move the board matrix
            return position % 8;
        }

    }
}
//gets a chessboard object and coping it to new chessboard object - not finished!!
/*public chessboard(chessboard cb)
{
    initialize_matrix_and_lists();

    for (int i = 0; i < peaces_types_amount; i++)
    {
        for (int j = 0; j < cb.white_parts[i].Count; j++) //white peaces
        {
            this.white_parts[i].Add(new Peace(cb.white_parts[i][j]));
        }
        for (int j = 0; j < cb.black_parts[i].Count; j++) //black peaces
        {
            this.black_parts[i].Add(new Peace(cb.black_parts[i][j]));
        }
    }
}*/


//gets the list of the current players turn
/*
 * public List<Peace>[] getplayerlistpointer(bool currentplayerlist)
        {
            if (this.whitetomove && currentplayerlist || !this.whitetomove && !currentplayerlist)
                return white_parts;
            else
                return this.black_parts;
        }
*/