using chess.types_of_peaces;
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

        public string Tostring()
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
    }
    /*
     -Check en passant:

    // En passant captures are a tricky special case. Because they are rather
    // uncommon, we do it simply by testing whether the king is attacked after
    // the move is made.

    -Do you want move your king?

    // If the moving piece is a king, check whether the destination
    // square is attacked by the opponent. Castling moves are checked
    // for legality during move generation.

    -You can make the move if it's not pinned. Or it's pinned, but it still protects the king.

    // A non-king move is legal if and only if it is not pinned or it
    // is moving along the ray towards or away from the king.
    */
    public class Movegenerator
    {
        private chessboard c;
        //public long WA = 0L, WP = 0L, BA = 0L, BP = 0L; //white attacking squares, white pinned peaces  //the rest is the same for black

        public Movegenerator(chessboard chess)
        {
            this.c = chess;
        }

        //generates all the legal moves that exist in this chess board position
        // all the generated moves return ass a list that contain move objects
        public List<Move> generate_moves()
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

        public List<Move> generate_attacking_moves()
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

        public List<Move> generate_moves(Peace peace)
        {
            if (peace.type == Peace.Pawn)
            {
                if (peace.iswhite)
                    return getmoves_w_pawn_pc(peace);
                return getmoves_b_pawn_pc(peace);
            }
            else
            {
                List<Move> moves = new List<Move>();
                switch (peace.type)
                {
                    case Peace.Knight:
                        getmoves_knight_pc(peace, moves);
                        return moves;
                    case Peace.Bishop:
                        getmoves_diagonal_pc(peace, moves);
                        return moves;
                    case Peace.Rook:
                        getmoves_verticle_pc(peace, moves);
                        return moves;
                    case Peace.Queen:
                        getmoves_verticle_pc(peace, moves);
                        getmoves_diagonal_pc(peace, moves);
                        return moves;
                    default: //case Peace.King:
                        Getmoves_king_pc(peace, moves);
                        return moves;
                }
            }
        }

        public void getmoves_verticle_pc(Peace peace, List<Move> moves)
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

        public void getmoves_diagonal_pc(Peace peace, List<Move> moves)
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

        public List<Move> getmoves_w_pawn_pc(Peace wpawn)
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

        public List<Move> getmoves_b_pawn_pc(Peace bpawn) //this one is done - the second one shoud be more ifishent
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

        public void getmoves_knight_pc(Peace knight, List<Move> moves)
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

        public void Getmoves_king_pc(Peace king, List<Move> moves) 
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

        public int Perft(int depth)
        {
            return Perfit(depth, depth);
        }

        public int Perfit(int depth, int headnode)
        {
            if (depth == 0) // if got to a leaf node
                return 1;
            List<Move> moves = Generatelegalmovesfrompseudolegal();
            int leafnodes = 0;
            foreach (Move move in moves)
            {
                c.manualy_makemove(move);
                int num = Perfit(depth - 1, headnode);
                leafnodes += num;
                if (depth == headnode)
                {
                    Console.WriteLine(move.Tostring() + num);
                    //Console.WriteLine(c.get_fen_notation());
                }
                c.unmakelastmove();
            }
            return leafnodes;
        }

        public List<Move> Generatelegalmovesfrompseudolegal()
        {
            List<Move> pseudolegalmoves = generate_moves();
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

    }

}
