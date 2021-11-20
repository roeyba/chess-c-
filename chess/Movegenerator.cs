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

    public class Movegenerator
    {
        public chessboard c;

        public Movegenerator(chessboard chess)
        {
            c = chess;
        }

        //generates all the legal moves that exist in this chess board position
        // all the generated moves return ass a list that contain move objects
        public List<Move> generate_moves()
        {
            List<Move> moves = new List<Move>();
            List<Peace>[] parts;
            if (this.c.whitetomove)
                parts = c.white_parts;
            else
                parts = c.black_parts;

            foreach (Peace peace in parts[Peace.Knight])
            {
                /*
                int i = peace.get_i_pos();
                int j = peace.get_j_pos();
                if(!this.c.board[i, t].isocupied())
                moves.Add(new Move());
                */
                //not done yet
            }



            if (this.c.whitetomove)
            {
                foreach (Peace peace in parts[Peace.Pawn])//Peace.Knight
                {
                    getmoves_w_pawn_pc(peace,moves);
                }
            }
            else
            {
                foreach (Peace peace in parts[Peace.Pawn])
                {
                    getmoves_b_pawn_pc(peace, moves);
                }
            }

            foreach (Peace peace in parts[Peace.Rook])
            {
                getmoves_sliding_pc(peace, moves);
            }

            foreach (Peace peace in parts[Peace.Bishop])
            {
                getmoves_diagonal_pc(peace, moves);
            }

            foreach (Peace peace in parts[Peace.Queen])
            {
                getmoves_sliding_pc(peace, moves);
                getmoves_diagonal_pc(peace, moves);
            }


            //check whose turn it is to play
            //go over all of the peaces of the curren player and for each peace add its moves to the "moves" list
            //go over all of the peaces and delete the moves where the curent player peaces are interfiting with the move
            //delete the moves that cause the curent player king to be in check



            return moves;
        }
        public List<Move> getmoves_sliding_pc(Peace peace, List<Move> moves)
        {
            int i = peace.get_i_pos();
            int j = peace.get_j_pos();
            
            //generate all the moves right to the peace
            for(int t = j+1; t < chessboard.board_size; t++)
            {
                if (!this.c.board[i, t].isocupied())
                {
                    moves.Add(new Move(peace.position,i*8+t));
                }
                else
                {
                    if(peace.iswhite != c.board[i, t].Peace.iswhite)
                        moves.Add(new Move(peace.position, i * 8 + t));
                    break;
                }
                    
            }

            //generate all the moves left to the peace
            for (int t = j -1; t >= 0; t--)
            {
                if (!this.c.board[i, t].isocupied())
                {
                    moves.Add(new Move(peace.position, i * 8 + t));
                }
                else
                {
                    if (peace.iswhite != c.board[i, t].Peace.iswhite)
                        moves.Add(new Move(peace.position, i * 8 + t));
                    break;
                }
            }

            //generate all the moves down the peace
            for (int t = i+1; t< chessboard.board_size; t++)
            {
                if (!this.c.board[t, j].isocupied())
                {
                    moves.Add(new Move(peace.position, t * 8 + j));
                }
                else
                {
                    if (peace.iswhite != c.board[t, j].Peace.iswhite)
                        moves.Add(new Move(peace.position, t * 8 + j));
                    break;
                }
            }

            //generate all the moves on top of the peace
            for (int t = i -1; t >= 0; t--)
            {
                if (!this.c.board[t, j].isocupied())
                {
                    moves.Add(new Move(peace.position, t * 8 + j));
                }
                else
                {
                    if (peace.iswhite != c.board[t, j].Peace.iswhite)
                        moves.Add(new Move(peace.position, t * 8 + j));
                    break;
                }
            }

            return moves;
        }
        
        public List<Move> getmoves_diagonal_pc(Peace peace, List<Move> moves)
        {
            int i = peace.get_i_pos();
            int j = peace.get_j_pos();
            int t;
            //generate all the moves right_up to the peace
            for (t = 1; t + j < chessboard.board_size && i - t >= 0; t++)
            {
                if (!this.c.board[i - t, t + j].isocupied())
                {
                    moves.Add(new Move(peace.position, (i - t) * 8 + t + j));
                }
                else
                {
                    if (peace.iswhite != c.board[i - t, t + j].Peace.iswhite)
                        moves.Add(new Move(peace.position, (i - t) * 8 + t + j));
                    break;
                }
            }

            //generate all the moves left_down to the peace
            for (t = 1; j - t >= 0 && t + i < chessboard.board_size; t++)
            {
                if (!this.c.board[t + i, j - t].isocupied())
                {
                    moves.Add(new Move(peace.position, (t + i) * 8 + j - t));
                }
                else
                {
                    if (peace.iswhite != c.board[t + i, j - t].Peace.iswhite)
                        moves.Add(new Move(peace.position, (t + i) * 8 + j - t));
                    break;
                }
            }

            //generate all the moves left_up to the peace
            for (t = 1; j-t>= 0 && i-t >= 0; t++)
            {
                if (!this.c.board[i - t, j - t].isocupied())
                {
                    moves.Add(new Move(peace.position, (i - t) * 8 + j - t));
                }
                else
                {
                    if (peace.iswhite != c.board[i - t, j - t].Peace.iswhite)
                        moves.Add(new Move(peace.position, (i - t) * 8 + j - t));
                    break;
                }
            }

            //generate all the moves right_down to the peace
            for (t = 1; t + j< chessboard.board_size && t + i < chessboard.board_size; t++)
            {
                if (!this.c.board[i+t, j+t].isocupied())
                {
                    moves.Add(new Move(peace.position, (i + t) * 8 + j + t));
                }
                else
                {
                    if (peace.iswhite != c.board[i + t, j + t].Peace.iswhite)
                        moves.Add(new Move(peace.position, (i + t) * 8 + j + t));
                    break;
                }
            }
            return moves;
        }

        public List<Move> getmoves_w_pawn_pc(Peace peace, List<Move> moves)
        {
            int i = peace.get_i_pos();
            int j = peace.get_j_pos();
            if(!c.board[i - 1, j].isocupied()) //one step forwords
            {
                moves.Add(new Move(peace.position, (i - 1) * 8 + j));
                if (i == 6 && !c.board[i - 2, j].isocupied())//two steps forwords
                    moves.Add(new Move(peace.position, (i - 2) * 8 + j));
            }
            if (this.c.moves.Count != 0)
            {
                Move lastmove = this.c.moves.Pop();
                int jfinal = chessboard.get_j_pos(lastmove.endsquare);
                int ifinal = chessboard.get_i_pos(lastmove.endsquare);
                int istart = chessboard.get_i_pos(lastmove.startsquare);
                Peace lastmovepeace;
                if (j != chessboard.board_size - 1 && c.board[i, j + 1].isocupied())
                {
                    lastmovepeace = c.board[i, j + 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && false == lastmovepeace.iswhite)// En passant right
                    {
                        if (jfinal == j + 1 && ifinal == i && istart == i - 2)
                            moves.Add(new Move(peace.position, (i - 1) * 8 + j + 1));
                    }
                }
                if (j != 0 && c.board[i, j - 1].isocupied())
                {
                    lastmovepeace = c.board[i, j - 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && false == lastmovepeace.iswhite)// En passant left
                    {
                        if (jfinal == j - 1 && ifinal == i && istart == i - 2)
                            moves.Add(new Move(peace.position, (i - 1) * 8 + j - 1));
                    }
                }
                this.c.moves.Push(lastmove);
            }
            return moves;
        }

        public List<Move> getmoves_b_pawn_pc(Peace peace, List<Move> moves) //this one is done - the second one shoud be more ifishent
        {
            int i = peace.get_i_pos();
            int j = peace.get_j_pos();
            if (!c.board[i + 1, j].isocupied()) //one step forwords
            {
                moves.Add(new Move(peace.position, (i + 1) * 8 + j));
                if (i == 1 && !c.board[i + 2, j].isocupied())//two steps forwords
                    moves.Add(new Move(peace.position, (i + 2) * 8 + j));
            }
            if (this.c.moves.Count != 0)
            {
                Move lastmove = this.c.moves.Pop();
                int jfinal = chessboard.get_j_pos(lastmove.endsquare);
                int ifinal = chessboard.get_i_pos(lastmove.endsquare);
                int istart = chessboard.get_i_pos(lastmove.startsquare);
                Peace lastmovepeace;
                if (j != chessboard.board_size-1 && c.board[i, j + 1].isocupied())
                {
                    lastmovepeace = c.board[i, j + 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && true == lastmovepeace.iswhite)// En passant right
                    {
                        if (jfinal == j + 1 && ifinal == i && istart == i - 2)
                            moves.Add(new Move(peace.position, (i - 1) * 8 + j + 1));
                    }
                }
                if (j != 0 && c.board[i, j - 1].isocupied())
                {
                    lastmovepeace = c.board[i, j - 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && true == lastmovepeace.iswhite)// En passant left
                    {
                        if (jfinal == j - 1 && ifinal == i && istart == i - 2)
                            moves.Add(new Move(peace.position, (i - 1) * 8 + j - 1));
                    }
                }
                this.c.moves.Push(lastmove);
            }
            return moves;
        }
    }
}
