using chess.types_of_peaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    //structer for holding a move
    public struct Move
    {
        public readonly int startsquare;
        public readonly int endsquare;
        public Peace capturedpeace;
        public int edgecase;
        /*edge case to number:
         *1 - pawn promote to knight
         *2 - pawn promote to bishop
         *3 - pawn promote to rook
         *4 - pawn promote to queen
         *5 - castle
         *7 - king moved, cant castle any more
         *8 - rook moved, cant castle at that side anymove
        */

        // the user create new move, when making the move its cheked if it is a ligal move and if so 'capturedpeace' is added if necesary
        public Move(int startsquare, int endsquare, int edgecase_num = 0)
        {
            this.startsquare = startsquare;
            this.endsquare = endsquare;
            capturedpeace = null;
            this.edgecase = edgecase_num;
        }
        public const int pawn_promote_to_knight = 1;
        public const int pawn_promote_to_bishop = 2;
        public const int pawn_promote_to_rook = 3;
        public const int pawn_promote_to_queen = 4;
        public const int castle = 5;
        public const int king_moving = 7; //the king hasnt moved once since the beggining of the game
        public const int rook_moving = 8; //the rook hasnt moved once since the beggining of the game
        public const int enpassant = 9;

        public string print_in_notation()
        {
            string promotion_peace = "";
            if(this.edgecase !=0)
            {
                switch (this.edgecase)
                {
                    case pawn_promote_to_knight:
                        promotion_peace = "k";
                        break;
                    case pawn_promote_to_bishop:
                        promotion_peace = "b";
                        break;
                    case pawn_promote_to_rook:
                        promotion_peace = "r";
                        break;
                    case pawn_promote_to_queen:
                        promotion_peace = "q";
                        break;
                }
            }
            return "[" + chessboard.get_j_pos_as_letter(startsquare) + "," + Math.Abs(chessboard.get_i_pos(startsquare) - 8) + "]" + "[" + chessboard.get_j_pos_as_letter(endsquare) + "," + Math.Abs(chessboard.get_i_pos(endsquare) - 8) + "]"+ promotion_peace+": ";
        }
        public bool peacepromote()
        {
            if (this.edgecase == Move.pawn_promote_to_queen || this.edgecase == Move.pawn_promote_to_rook || this.edgecase == Move.pawn_promote_to_bishop || this.edgecase == Move.pawn_promote_to_knight)
                return true;
            return false;
        }
        public string to_mininal_string()
        {
            return (startsquare.ToString() +","+ endsquare.ToString());
        }
    }
    
    public class Movegenerator
    {
        private chessboard c;
        private Move_list_ordering mo;
        //constractor.
        public Movegenerator(chessboard chess)
        {
            this.c = chess;
            this.mo = new Move_list_ordering(chess);
        }

        //generate all of the legal moves for one peace
        public List<Move> generate_legal_moves(Peace peace)
        {
            if (peace.type == Peace.Pawn)
            {
                if (peace.iswhite)
                    return Generatelegalmovesfrompseudolegal(getmoves_w_pawn_pc(peace));
                return Generatelegalmovesfrompseudolegal(getmoves_b_pawn_pc(peace));
            }
            else
            {
                List<Move> moves = new List<Move>();
                switch (peace.type)
                {
                    case Peace.Knight:
                        getmoves_knight_pc(peace, moves);
                        return Generatelegalmovesfrompseudolegal(moves);
                    case Peace.Bishop:
                        getmoves_diagonal_pc(peace, moves);
                        return Generatelegalmovesfrompseudolegal(moves);
                    case Peace.Rook:
                        getmoves_verticle_pc(peace, moves);
                        return Generatelegalmovesfrompseudolegal(moves);
                    case Peace.Queen:
                        getmoves_verticle_pc(peace, moves);
                        getmoves_diagonal_pc(peace, moves);
                        return Generatelegalmovesfrompseudolegal(moves);
                    case Peace.King:
                        Getmoves_king_pc(peace, moves);
                        return Generatelegalmovesfrompseudolegal(moves);
                    default:
                        return null;
                }
            }
        }
        
        //generate all of the ligal moves.
        public List<Move> generate_all_legal_moves()
        {
            return Generatelegalmovesfrompseudolegal(generate_psudo_legal_moves());
        }

        //generates all the psudo-legal moves that exist in this chess board position
        // all the generated moves return ass a list that contain move objects
        private List<Move> generate_psudo_legal_moves()
        {
            List<Move> moves = new List<Move>();
            List<Peace>[] parts;

            //go over all of the peaces of the curren player and for each peace add its moves to the "moves" list
            if (this.c.whiteturn)
            {
                parts = this.c.white_parts;
                foreach (Peace peace in parts[Peace.Pawn])//Peace.Knight
                {
                    moves.AddRange(getmoves_w_pawn_pc(peace));
                }
            }
            else
            {
                parts = this.c.black_parts;
                foreach (Peace peace in parts[Peace.Pawn])
                {
                    moves.AddRange(getmoves_b_pawn_pc(peace));
                }
            }

            foreach (Peace peace in parts[Peace.Knight])
            {
                getmoves_knight_pc(peace, moves);
            }

            foreach (Peace peace in parts[Peace.Bishop])
            {
                getmoves_diagonal_pc(peace, moves);
            }

            foreach (Peace peace in parts[Peace.Rook])
            {
                getmoves_verticle_pc(peace, moves);
            }
            foreach (Peace peace in parts[Peace.Queen])
            {
                getmoves_verticle_pc(peace, moves);
                getmoves_diagonal_pc(peace, moves);
            }
            foreach (Peace peace in parts[Peace.King])
            {
                Getmoves_king_pc(peace, moves);
            }

            return moves;
        }

        //these functions return or add to a input list the psudo-legal moves that a peace can do.
        //all of this fucntions are united in "generate_psudo_legal_moves()", in that function all of the posible psudo-legal moves in a board are assembled.
        private void getmoves_verticle_pc(Peace peace, List<Move> moves)
        {
            int i = peace.get_i_pos();
            int j = peace.get_j_pos();
            int edgecase = 0;
            int offset = this.c.getoffset(peace.iswhite);
            if(this.c.can_castle[offset] && peace.type == Peace.Rook)
            {
                int row;
                if (peace.iswhite)
                    row = 7;
                else
                    row = 0;
                if (i == row)//if the rook in the start game row position of the rooks
                {
                    //if the left rook dindt move and this is the rook, the same for the right rook:
                    if((this.c.can_castle[offset + 1] && j==0)|| (this.c.can_castle[offset + 2] && j == 7))
                        edgecase = Move.rook_moving;
                }
            }

            int t; //iterator
            //generate all the moves right to the peace
            for (t = j + 1; t < chessboard.board_size; t++)
            {
                if (!this.c.board[i, t].isocupied())
                {
                    moves.Add(new Move(peace.position, i * 8 + t, edgecase));
                }
                else
                {
                    if (peace.iswhite != c.board[i, t].Peace.iswhite)
                        moves.Add(new Move(peace.position, i * 8 + t, edgecase));
                    break;
                }

            }

            //generate all the moves left to the peace
            for (t = j - 1; t >= 0; t--)
            {
                if (!this.c.board[i, t].isocupied())
                {
                    moves.Add(new Move(peace.position, i * 8 + t, edgecase));
                }
                else
                {
                    if (peace.iswhite != c.board[i, t].Peace.iswhite)
                        moves.Add(new Move(peace.position, i * 8 + t, edgecase));
                    break;
                }
            }

            //generate all the moves down the peace
            for (t = i + 1; t < chessboard.board_size; t++)
            {
                if (!this.c.board[t, j].isocupied())
                {
                    moves.Add(new Move(peace.position, t * 8 + j, edgecase));
                }
                else
                {
                    if (peace.iswhite != c.board[t, j].Peace.iswhite)
                        moves.Add(new Move(peace.position, t * 8 + j, edgecase));
                    break;
                }
            }

            //generate all the moves on top of the peace
            for (t = i - 1; t >= 0; t--)
            {
                if (!this.c.board[t, j].isocupied())
                {
                    moves.Add(new Move(peace.position, t * 8 + j, edgecase));
                }
                else
                {
                    if (peace.iswhite != c.board[t, j].Peace.iswhite)
                        moves.Add(new Move(peace.position, t * 8 + j, edgecase));
                    break;
                }
            }
        }
        private void getmoves_diagonal_pc(Peace peace, List<Move> moves)
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
            for (t = 1; j - t >= 0 && i - t >= 0; t++)
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
            for (t = 1; t + j < chessboard.board_size && t + i < chessboard.board_size; t++)
            {
                if (!this.c.board[i + t, j + t].isocupied())
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
        }
        private List<Move> getmoves_w_pawn_pc(Peace wpawn)
        {
            List<Move> moves = new List<Move>();
            int i = wpawn.get_i_pos();
            int j = wpawn.get_j_pos();
            int jfinal = 15;
            int ifinal = 15;
            int istart = 15;
            Peace lastmovepeace;
            if (this.c.moves.Count != 0)
            {// En passant movement data
                Move lastmove = this.c.moves.Peek();
                jfinal = chessboard.get_j_pos(lastmove.endsquare);
                ifinal = chessboard.get_i_pos(lastmove.endsquare);
                istart = chessboard.get_i_pos(lastmove.startsquare);
            }
            if (!c.board[i - 1, j].isocupied()) //one step forwords
            {
                moves.Add(new Move(wpawn.position, wpawn.position -8));
                if (i == 6 && !c.board[i - 2, j].isocupied())         //two steps forwords
                    moves.Add(new Move(wpawn.position, wpawn.position -16));
            }
            if (j != chessboard.board_size - 1)//not at the edge right of the board
            {
                if (c.board[i - 1, j + 1].isocupied() && !c.board[i - 1, j + 1].Peace.iswhite)//normal capture right
                {
                    moves.Add(new Move(wpawn.position, wpawn.position -7));
                }
                else if (jfinal != 15 && c.board[i, j + 1].isocupied())
                {
                    lastmovepeace = c.board[i, j + 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && !lastmovepeace.iswhite)// En passant right
                    {
                        if (jfinal == j + 1 && ifinal == i && istart == i - 2)
                            moves.Add(new Move(wpawn.position, wpawn.position -7,Move.enpassant));
                    }
                }
            }
            if (j != 0)//not at the edge left of the board
            {
                if (c.board[i - 1, j - 1].isocupied() && !c.board[i - 1, j - 1].Peace.iswhite)//normal capture left
                {
                    moves.Add(new Move(wpawn.position, wpawn.position -9));
                }
                else if (jfinal != 15 && c.board[i, j - 1].isocupied())
                {
                    lastmovepeace = c.board[i, j - 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && !lastmovepeace.iswhite)// En passant left
                    {
                        if (jfinal == j - 1 && ifinal == i && istart == i - 2)
                            moves.Add(new Move(wpawn.position, wpawn.position -9, Move.enpassant));
                    }
                }
            }
            if (i == 1)
            {
                List<Move> final_moves = new List<Move>(4);
                foreach (Move move in moves)
                {
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_knight)); //knight
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_bishop)); //bishop
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_rook)); //rook
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_queen)); //queen
                }
                return final_moves;
            }
            else
                return moves;
        }
        private List<Move> getmoves_b_pawn_pc(Peace bpawn)
        {
            List<Move> moves = new List<Move>();
            int i = bpawn.get_i_pos();
            int j = bpawn.get_j_pos();
            int jfinal = 15;
            int ifinal = 15;
            int istart = 15;
            Peace lastmovepeace;
            if (this.c.moves.Count != 0)
            {// En passant movement data
                Move lastmove = this.c.moves.Peek();
                jfinal = chessboard.get_j_pos(lastmove.endsquare);
                ifinal = chessboard.get_i_pos(lastmove.endsquare);
                istart = chessboard.get_i_pos(lastmove.startsquare);
            }
            if (!c.board[i + 1, j].isocupied()) //one step forwords
            {
                moves.Add(new Move(bpawn.position, bpawn.position +8));
                if (i == 1 && !c.board[i + 2, j].isocupied())//two steps forwords
                    moves.Add(new Move(bpawn.position, bpawn.position +16));
            }
            if (j != 0)//not at the edge left of the board
            {
                if (c.board[i + 1, j - 1].isocupied() && c.board[i + 1, j - 1].Peace.iswhite)//normal capture left
                {
                    moves.Add(new Move(bpawn.position, bpawn.position +7));
                }
                else if (jfinal != 15 && c.board[i, j - 1].isocupied())
                {
                    lastmovepeace = c.board[i, j - 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && true == lastmovepeace.iswhite)// En passant left
                    {
                        if (jfinal == j - 1 && ifinal == i && istart == i + 2)
                            moves.Add(new Move(bpawn.position, bpawn.position + 7, Move.enpassant));
                    }
                }
            }
            if (j != chessboard.board_size - 1) //not at the edge right of the board
            {
                if (c.board[i + 1, j + 1].isocupied() && c.board[i + 1, j + 1].Peace.iswhite)//normal capture right
                {
                    moves.Add(new Move(bpawn.position, bpawn.position + 9));
                }
                else if (jfinal != 15 && c.board[i, j + 1].isocupied())
                {
                    lastmovepeace = c.board[i, j + 1].Peace;
                    if (Peace.Pawn == lastmovepeace.type && true == lastmovepeace.iswhite)// En passant right
                    {
                        if (jfinal == j + 1 && ifinal == i && istart == i + 2)
                            moves.Add(new Move(bpawn.position, bpawn.position + 9, Move.enpassant));
                    }
                }
            }
            if (i == 6)
            {
                List<Move> final_moves = new List<Move>(4);
                foreach (Move move in moves)
                {
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_knight)); //knight
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_bishop)); //bishop
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_rook)); //rook
                    final_moves.Add(new Move(move.startsquare, move.endsquare, Move.pawn_promote_to_queen)); //queen
                }
                return final_moves;
            }
            else
                return moves;
        }
        private void getmoves_knight_pc(Peace knight, List<Move> moves)
        {
            int i = knight.get_i_pos();
            int j = knight.get_j_pos();
            Boolean rightest = (j == 7);
            int first = 2;
            int second = 1;
            for (int numplace = 0; numplace < 2; numplace++)
            {
                for (int firstnumsighn = 0; firstnumsighn < 2; firstnumsighn++)
                {
                    for (int secondnumsighn = 0; secondnumsighn < 2; secondnumsighn++)
                    {
                        int tmpi = i + first;
                        int tmpj = j + second;
                        if (tmpi < 0 || tmpi > chessboard.board_size - 1 || tmpj < 0 || tmpj > chessboard.board_size - 1 || (c.board[tmpi, tmpj].isocupied() && c.board[tmpi, tmpj].Peace.iswhite == knight.iswhite))
                        {//the knight moves out of the board boundaries or land on a peace of its own color
                            second *= -1;
                            continue;
                        }
                        moves.Add(new Move(knight.position, (tmpi) * 8 + tmpj));
                        second *= -1;
                    }
                    first *= -1;
                }
                first = 1;
                second = 2;
            }
        }
        private void Getmoves_king_pc(Peace king, List<Move> moves) 
        {
            int i = king.get_i_pos();
            int j = king.get_j_pos();
            int edgecase = 0;
            if (this.c.kingdidntmoved(king.iswhite))
                edgecase = Move.king_moving;
            int row = this.c.getoffset(king.iswhite);
            if (c.can_castle[row])//castle both sides
            {
                if (c.can_castle[row + 1])//left
                {
                    if (!c.board[i, 1].isocupied() && !c.board[i, 2].isocupied() && !c.board[i, 3].isocupied())
                        moves.Add(new Move(king.position, king.position-2, Move.castle));
                }
                if (c.can_castle[row + 2])//right
                {
                    if (!c.board[i, 5].isocupied() && !c.board[i, 6].isocupied())
                    {
                        //if(king.position == 4)
                            //this.c.printstatics();
                        moves.Add(new Move(king.position, king.position + 2, Move.castle));
                    }
                }
            }
            //normal movement from here:

            Boolean[] borders = { j != 0, i != 7, j != 7, i != 0 };// 0left, 1down, 2right, 3up

            //same raw as starting posiotion
            if (borders[2] && c.board[i, j + 1].canmove(king))
                moves.Add(new Move(king.position, king.position + 1, edgecase));
            if (borders[0] && c.board[i, j - 1].canmove(king))
                moves.Add(new Move(king.position, king.position - 1, edgecase));
            //

            //raw down to the starting posiotion
            i++;
            if (borders[1])
            {
                if (borders[0] && c.board[i, j - 1].canmove(king))
                    moves.Add(new Move(king.position, king.position +7, edgecase));
                if (c.board[i, j].canmove(king))
                    moves.Add(new Move(king.position, king.position +8, edgecase));
                if (borders[2] && c.board[i, j + 1].canmove(king))
                    moves.Add(new Move(king.position, king.position +9, edgecase));
            }
            //

            //raw up to the starting posiotion
            i -= 2;
            if (borders[3])
            {
                if (borders[0] && c.board[i, j - 1].canmove(king))
                    moves.Add(new Move(king.position, king.position -9, edgecase));
                if (c.board[i, j].canmove(king))
                    moves.Add(new Move(king.position, king.position -8, edgecase));
                if (borders[2] && c.board[i, j + 1].canmove(king))
                    moves.Add(new Move(king.position, king.position -7, edgecase));
            }
            //
        }


        //Wrapping Operation - only for outside usege!
        public int Perft(int depth)
        {
            return Perfit(depth, depth);
        }
        //internal Operation - only for inside usege!
        private int Perfit(int depth, int headnode)
        {
            if (depth == 0) // if got to a leaf node
                return 1;
            List<Move> moves = generate_all_legal_moves();
            int leafnodes = 0;
            foreach (Move move in moves)
            {
                c.manualy_makemove(move);
                int num = Perfit(depth - 1, headnode);
                leafnodes += num;
                if (depth == headnode)
                {
                    Console.WriteLine(move.print_in_notation() + num);
                    //Console.WriteLine(c.get_fen_notation());
                }
                c.unmakelastmove();
            }
            return leafnodes;
        }

        //gets a list of pseudo-legal moves and return a list with all of the legal moves in this list
        private List<Move> Generatelegalmovesfrompseudolegal(List<Move> pseudolegalmoves)
        {
            List<Move> legalmoves = new List<Move>();
            foreach (Move tmpmove in pseudolegalmoves)
            {
                c.manualy_makemove(tmpmove);
                int king_pos;
                if (this.c.whiteturn)
                    king_pos = c.black_parts[Peace.King][0].position;
                else
                    king_pos = c.white_parts[Peace.King][0].position;

                List<Move> nextmoves = generate_attacking_moves();
                if (!nextmoves.Any(move => move.endsquare == king_pos))
                {
                    if(tmpmove.edgecase == Move.castle) //if the movement is a castle, making sure the move is legal
                    {
                        int kingbeforecastling; //where the king is before moving at all
                        if (this.c.whiteturn)
                            kingbeforecastling = 4;
                        else
                            kingbeforecastling = 60;
                        foreach (Move move in nextmoves)
                        {//    if in check and want to castle        //castle right                                          //castle left
                            if (move.endsquare == kingbeforecastling ||(king_pos % 8 == 6 && move.endsquare == king_pos - 1) || (king_pos % 8 == 2 && move.endsquare == king_pos + 1))
                                goto LoopEnd;
                        }
                    }
                    legalmoves.Add(tmpmove);
                }
                LoopEnd:
                this.c.unmakelastmove();
            }
            return legalmoves;
        }


        //generate all of the moves where the player can attack at(not matter if there is a peace you can eat in those squers)
        internal List<Move> generate_attacking_moves()
        {//only used to make sure the gemerator makes only ligal moves.

            List<Move> moves = new List<Move>();
            List<Peace>[] parts;

            //go over all of the peaces of the curren player and for each peace add its moves to the "moves" list
            if (this.c.whiteturn)
            {
                parts = this.c.white_parts;
                foreach (Peace peace in parts[Peace.Pawn])//Peace.Knight
                {
                    moves.AddRange(getattackingmoves_w_pawn_pc(peace));
                }
            }
            else
            {
                parts = this.c.black_parts;
                foreach (Peace peace in parts[Peace.Pawn])
                {
                    moves.AddRange(getattackingmoves_b_pawn_pc(peace));
                }
            }

            foreach (Peace peace in parts[Peace.Knight])
            {
                getmoves_knight_pc(peace, moves);
            }

            foreach (Peace peace in parts[Peace.Bishop])
            {
                getmoves_diagonal_pc(peace, moves);
            }

            foreach (Peace peace in parts[Peace.Rook])
            {
                getmoves_verticle_pc(peace, moves);
            }
            foreach (Peace peace in parts[Peace.Queen])
            {
                getmoves_verticle_pc(peace, moves);
                getmoves_diagonal_pc(peace, moves);
            }
            foreach (Peace peace in parts[Peace.King])
            {
                Getmoves_king_pc(peace, moves);
            }

            return moves;
        }
        //generate the black/white pawn attacking squers.
        public List<Move> getattackingmoves_w_pawn_pc(Peace wpawn)
        {
            List<Move> moves = new List<Move>();
            int j = wpawn.get_j_pos();

            if (j != chessboard.board_size - 1)//not at the edge right of the board
            {
                moves.Add(new Move(wpawn.position, wpawn.position - 7));//normal capture right
            }
            if (j != 0)//not at the edge left of the board
            {
                moves.Add(new Move(wpawn.position, wpawn.position - 9, Move.enpassant));//normal capture left
            }
            return moves;
        }
        public List<Move> getattackingmoves_b_pawn_pc(Peace bpawn)
        {
            List<Move> moves = new List<Move>();
            int j = bpawn.get_j_pos();
            if (j != 0)//not at the edge left of the board
            {
                moves.Add(new Move(bpawn.position, bpawn.position + 7));//normal capture left
            }
            if (j != chessboard.board_size - 1) //not at the edge right of the board
            {
                moves.Add(new Move(bpawn.position, bpawn.position + 9));//normal capture right
            }
            return moves;
        }

        //return the move the AI wants to play.
        public Move choose_move(int depth, bool iswhite) //the assamption is that the game isnt over yet
        {
            //aplpha: the worst posible score for white - negative infinity
            //beta: the worst posible score for black - positive infinity
            if (iswhite) { return alphaBetaMax_getmove(alpha: int.MinValue, beta: int.MaxValue, depth); }
            //
            //
            return alphaBetaMin_getmove(alpha: int.MinValue, beta: int.MaxValue, depth);
        }
        
        // a rapt functions for minimx, return the optimal Move obj.
        //minmax and alpha beta pruning.
        public Move alphaBetaMax_getmove(int alpha, int beta, int depth)
        {
            //assuming deph>0 and there are at least one move to be made at that position
            List<Move> child_nodes = this.mo.OrdereMoves(this.c.generator.generate_all_legal_moves());
            Move best_move =new Move();

            for (int i = 0; i < child_nodes.Count; i++)
            {
                Console.WriteLine(child_nodes[i].print_in_notation());
                this.c.manualy_makemove(child_nodes[i]);
                int score = alphaBetaMin(alpha, beta, depth - 1);
                Console.WriteLine("Score: " + score);
                this.c.unmakelastmove();
                if (score >= beta)
                    return child_nodes[i];   // fail hard beta-cutoff
                if (score > alpha)
                {
                    alpha = score; // alpha acts like max in MiniMax
                    best_move = child_nodes[i];
                }
            }
            return best_move;
        }
        public Move alphaBetaMin_getmove(int alpha, int beta, int depth)
        {
            //assuming deph>0 and there are at least one move to be made at that position
            List<Move> child_nodes = this.mo.OrdereMoves(this.c.generator.generate_all_legal_moves());
            Move best_move = new Move();

            for (int i = 0; i < child_nodes.Count; i++)
            {
                if (child_nodes[i].startsquare == 2 && child_nodes[i].endsquare == 26)
                    Console.WriteLine(child_nodes[i].to_mininal_string());
                //Console.WriteLine(child_nodes[i].print_in_notation());
                this.c.manualy_makemove(child_nodes[i]);
                int score = alphaBetaMax(alpha, beta, depth - 1);
                Console.WriteLine("Score: " + score);
                this.c.unmakelastmove();
                if (score <= alpha)
                    return child_nodes[i];   // fail hard beta-cutoff
                if (score < beta)
                {
                    beta = score; // alpha acts like max in MiniMax
                    best_move = child_nodes[i];
                }
            }
            return best_move;
        }

        public int alphaBetaMax(int alpha, int beta, int depthleft)
        {
            if (depthleft == 0) return this.c.Evaluate();

            List<Move> child_nodes = this.mo.OrdereMoves(this.c.generator.generate_all_legal_moves());
            // leaf node is reached
            if (child_nodes.Count == 0)// draw or someone won
            {
                if (this.c.current_player_king_in_check()) //if checkmate
                    return Int32.MinValue;
                return 0;//draw
            }

            for (int i = 0; i < child_nodes.Count; i++)
            {
                this.c.manualy_makemove(child_nodes[i]);
                int score = alphaBetaMin(alpha, beta, depthleft - 1);
                this.c.unmakelastmove();
                if (score >= beta)
                    return beta;   // fail hard beta-cutoff
                if (score > alpha)
                    alpha = score; // alpha acts like max in MiniMax
            }
            return alpha;
        }
        public int alphaBetaMin(int alpha, int beta, int depthleft)
        {
            if (depthleft == 0) return this.c.Evaluate();

            List<Move> child_nodes = this.mo.OrdereMoves(this.c.generator.generate_all_legal_moves());
            // leaf node is reached
            if (child_nodes.Count == 0)// draw or someone won
            {
                if (this.c.current_player_king_in_check()) //if checkmate
                    return Int32.MaxValue;
                return 0;//draw
            }

            for (int i = 0; i < child_nodes.Count; i++)
            {
                Console.WriteLine(child_nodes[i].print_in_notation());
                this.c.manualy_makemove(child_nodes[i]);
                int score = alphaBetaMax(alpha, beta, depthleft - 1);
                Console.WriteLine("Score: " + score);
                this.c.unmakelastmove();
                if (score <= alpha)
                    return alpha; // fail hard alpha-cutoff
                if (score < beta)
                    beta = score; // beta acts like min in MiniMax
            }
            return beta;
        }

        //generate captured legal moves
        private List<Move> getcapturesmoves()
        {
            return Generatelegalmovesfrompseudolegal(generate_psudo_legal_moves().FindAll(move => move.capturedpeace != null));
        }
        //minimax for captured moves
        public int alphaBetaMaxcapture(int alpha, int beta)
        {
            int eval = this.c.Evaluate();
            if (eval >= beta)
                return beta;   // fail hard beta-cutoff
            if (eval > alpha)
                alpha = eval; // alpha acts like max in MiniMax
            /////////////////

            List<Move> child_nodes = this.mo.OrdereMoves(getcapturesmoves());

            for (int i = 0; i < child_nodes.Count; i++)
            {
                this.c.manualy_makemove(child_nodes[i]);
                int score = alphaBetaMincapture(alpha, beta);
                this.c.unmakelastmove();
                if (score >= beta)
                    return beta;   // fail hard beta-cutoff
                if (score > alpha)
                    alpha = score; // alpha acts like max in MiniMax
            }
            return alpha;
        }
        public int alphaBetaMincapture(int alpha, int beta)
        {
            int eval = -this.c.Evaluate();
            if (eval <= alpha)
                return alpha; // fail hard alpha-cutoff
            if (eval < beta)
                beta = eval; // beta acts like min in MiniMax
            //////////////////

            List<Move> child_nodes = this.mo.OrdereMoves(getcapturesmoves());

            for (int i = 0; i < child_nodes.Count; i++)
            {
                this.c.manualy_makemove(child_nodes[i]);
                int score = alphaBetaMaxcapture(alpha, beta);
                this.c.unmakelastmove();
                if (score <= alpha)
                    return alpha; // fail hard alpha-cutoff
                if (score < beta)
                    beta = score; // beta acts like min in MiniMax
            }
            return beta;
        }

    }

}
