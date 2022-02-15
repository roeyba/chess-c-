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
        private chessboard chessboard;

        const int squareControlledByOpponentPawnPenalty = 350; // penalty for squares controlled by opponent pawns
        const int capturedPieceValueMultiplier = 10; //10x the value of the captured piece

        public Move_list_ordering(chessboard board)
        {
            this.Scores = new int[218];//this is the maximum posible legal moves in a single position.
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
            sort_list(moves);
            return moves;
        }
        //this function evaluates the move and returns the score.
        private int Score(Move move)
        {
            //https://www.chessprogramming.org/Move_Ordering#Standard_techniques
            int score = 0;
            Peace movepeace = this.chessboard.board[chessboard.get_i_pos(move.startsquare), chessboard.get_j_pos(move.startsquare)].Peace;
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
                // I need to check if the square is attacked by the opponent (using attack grid). implement that and than do this part

                /*
				if (BitBoardUtility.ContainsSquare (moveGenerator.opponentPawnAttackMap, moves[i].TargetSquare)) {
					score -= squareControlledByOpponentPawnPenalty;
				}
                */
            }
            return score;
        }

        // this function sorts the list according to the array.
        private void sort_list(List<Move> moves)
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
    }
}
