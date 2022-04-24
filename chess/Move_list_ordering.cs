using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    internal class Move_list_ordering
    {
        private int[] Scores;
        private Chessboard chessboard;

        const int squareControlledByOpponentPawnPenalty = 350; // penalty for squares controlled by opponent pawns
        const int capturedPieceValueMultiplier = 10; //10x the value of the captured piece

        public Move_list_ordering(Chessboard board)
        {
            this.Scores = new int[218];//this is the maximum posible legal moves in a single position.(most of the time there would be about 30 moves - for each one one score)
            this.chessboard = board;
        }

        //according to the web page https://www.chessprogramming.org/Move_Ordering#Standard_techniques - "Typical move ordering"
        //the sort is in the evaluations array, after that the list is sorted according to the array.
        public List<Move> OrdereMoves(List<Move> moves)
        {
            for (int i = 0; i < moves.Count; i++)
            {
                //ivaluate every move and put it in the array.
                this.Scores[i] = this.Score(moves[i]);
            }
            Sort_list(moves);
            return moves;
        }
        //this function evaluates the move and returns the score.
        private int Score(Move move)
        {
            //https://www.chessprogramming.org/Move_Ordering#Standard_techniques
            int score = 0;
            Peace movepeace = this.chessboard.board[move.startsquare].Peace;
            int movePieceType = movepeace.type;
            int edgecase = move.edgecase;

            if (move.capturedpeace != null)
            {
                // Order moves to try capturing the most valuable opponent piece with least valuable of own pieces first
                // The capturedPieceValueMultiplier is used to make even 'bad' captures like QxP rank above non-captures
                score = capturedPieceValueMultiplier * move.capturedpeace.GetPieceValue() - movepeace.GetPieceValue();
            }

            if (movePieceType == Peace.Pawn)
            {
                if (edgecase == Move.pawn_promote_to_knight)
                    score += Peace.Knight_value;
                else if (edgecase == Move.pawn_promote_to_bishop)
                    score += Peace.Bishop_value;
                else if (edgecase == Move.pawn_promote_to_rook)
                    score += Peace.Rook_value;
                else if (edgecase == Move.pawn_promote_to_queen)
                    score += Peace.Queen_value;
            }
            else
            {
                // Penalize moving piece to a square attacked by opponent pawn

            }
            return score;
        }

        // this function sorts the list according to the array.
        private void Sort_list(List<Move> moves)
        {
            for (int i = 0; i < moves.Count - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    int swapIndex = j - 1;
                    if (Scores[swapIndex] < Scores[j])
                    {
                        (moves[j], moves[swapIndex]) = (moves[swapIndex], moves[j]);
                        (Scores[j], Scores[swapIndex]) = (Scores[swapIndex], Scores[j]);
                    }
                }
            }
        }

        private Boolean Pos_in_check_at_diagonaly_up(int position, int direction)
        {
            int pos = position;
            for (int i = 0; i < Movegenerator.squers_to_edge[direction][position]; i++)
            {
                pos += Movegenerator.directions[direction];
                if (chessboard.board[pos].Isocupied())
                {
                    if (!chessboard.board[pos].Peace.color.Equals(chessboard.color_turn))
                    {
                        int pc_type = chessboard.board[pos].Peace.type;
                        if (pc_type == Peace.Bishop || pc_type == Peace.Queen)
                            return true;
                        if (i == 0)
                        {
                            if (pc_type == Peace.King)
                                return true;
                            if (chessboard.Is_white_turn() && pc_type == Peace.Pawn)
                                return true;
                        }
                    }
                    break;
                }
            }
            return false;
        }
        private Boolean Pos_in_check_at_diagonaly_down(int position, int direction)
        {
            int pos = position;
            for (int i = 0; i < Movegenerator.squers_to_edge[direction][position]; i++)
            {
                pos += Movegenerator.directions[direction];
                if (chessboard.board[pos].Isocupied())
                {
                    if (!chessboard.board[pos].Peace.color.Equals(chessboard.color_turn))
                    {
                        int pc_type = chessboard.board[pos].Peace.type;
                        if (pc_type == Peace.Bishop || pc_type == Peace.Queen)
                            return true;
                        if (i == 0)
                        {
                            if (pc_type == Peace.King)
                                return true;
                            if (chessboard.Is_black_turn() && pc_type == Peace.Pawn)
                                return true;
                        }
                    }
                    break;
                }
            }
            return false;
        }
        private Boolean Pos_in_check_at_verticle(int position, int direction)
        {
            int pos = position;
            for (int i = 0; i < Movegenerator.squers_to_edge[direction][position]; i++)
            {
                pos += Movegenerator.directions[direction];
                if (chessboard.board[pos].Isocupied())
                {
                    if (!chessboard.board[pos].Peace.color.Equals(chessboard.color_turn))
                    {
                        int pc_type = chessboard.board[pos].Peace.type;
                        if (pc_type == Peace.Rook || pc_type == Peace.Queen)
                            return true;
                        if (pc_type == Peace.King && i == 0)
                            return true;
                    }
                    break;
                }
            }
            return false;
        }
        public Boolean Pos_in_check(int position)
        {
            if (
                Pos_in_check_at_diagonaly_up(position, Movegenerator.right_up) ||
                Pos_in_check_at_diagonaly_down(position, Movegenerator.left_down) ||
                Pos_in_check_at_diagonaly_up(position, Movegenerator.left_up) ||
                Pos_in_check_at_diagonaly_down(position, Movegenerator.right_down) ||
                Pos_in_check_at_verticle(position, Movegenerator.right) ||
                Pos_in_check_at_verticle(position, Movegenerator.left) ||
                Pos_in_check_at_verticle(position, Movegenerator.down) ||
                Pos_in_check_at_verticle(position, Movegenerator.up)
                )
                return true;
            //generate all the moves right_up to the peace
            //generate all the moves left_down to the peace
            //generate all the moves left_up to the peace
            //generate all the moves right_down to the peace
            ////////////////////////////////////////////////////////////vertical movement
            //generate all the moves right to the peace
            //generate all the moves left to the peace
            //generate all the moves down the peace
            //generate all the moves on top of the peace
            int[] op_knight_positions = new int[chessboard.peaces[chessboard.Opponent_color()][Peace.Knight].Count];
            int t;
            for (t = 0; t < op_knight_positions.Length; t++)
                op_knight_positions[t] = chessboard.peaces[chessboard.Opponent_color()][Peace.Knight][t].position;
            int i = Chessboard.Get_i_pos(position);
            int j = Chessboard.Get_j_pos(position);
            for (t = 0; t < op_knight_positions.Length; t++)
            {
                if (i != Chessboard.Get_i_pos(op_knight_positions[t]) && j != Chessboard.Get_j_pos(op_knight_positions[t]) && (Math.Abs(i - Chessboard.Get_i_pos(op_knight_positions[t])) + Math.Abs(j - Chessboard.Get_j_pos(op_knight_positions[t])) == 3))
                    return true;
            }
            return false;
        }
    }
}
