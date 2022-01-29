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
        public Boolean canmove(Peace peace) // if psudolegaly allowed to move a peace to that tile.
        {
            if (isocupied())
            {
                if (peace.iswhite != this.Peace.iswhite)
                    return true;
                return false;
            }
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
        public Boolean[] can_castle = { true, true, true , true, true, true }; // white all, white left , white right, black all , black left , black right, ||before got eaten: white left, white right, black left , black right                               
        public Boolean whiteturn = true;// true is white and false is black
        public Movegenerator generator;

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
         *        
         *    
         *    (i)
         *      +---+---+---+---+---+---+---+---+
         *    8 | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 |
         *      +---+---+---+---+---+---+---+---+
         *    7 | 8 | 9 |10 |11 |12 |13 |14 |15 |
         *      +---+---+---+---+---+---+---+---+
         *    6 |16 |17 |18 |19 |20 |21 |22 |23 |
         *      +---+---+---+---+---+---+---+---+
         *    5 |24 |25 |26 |27 |28 |29 |30 |31 |
         *      +---+---+---+---+---+---+---+---+
         *    4 |32 |33 |34 |35 |36 |37 |38 |39 |
         *      +---+---+---+---+---+---+---+---+
         *    3 |40 |41 |42 |43 |44 |45 |46 |47 |
         *      +---+---+---+---+---+---+---+---+
         *    2 |48 |49 |50 |51 |52 |53 |54 |55 |
         *      +---+---+---+---+---+---+---+---+
         *    1 |56 |57 |58 |59 |60 |61 |62 |63 |
         *      +---+---+---+---+---+---+---+---+
         *        a   b   c   d   e   f   g   h  (j)
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

            //create the move generator
            generator = new Movegenerator(this);
        }

        //create an empty chess board and initialize it to beggining chess position 
        public chessboard()
        {
            initialize_objects();

            //create all of the pawns
            for (int j = 0; j < 8; j++) 
            {
                addpeacetoboardandlist(new Pawn(false, j + 8));
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
            addpeacetoboardandlist(new Queen(false, 3)); addpeacetoboardandlist(new Queen(true, 59));

            //create all of the kings
            addpeacetoboardandlist(new King(false, 4)); addpeacetoboardandlist(new King(true, 60));
            
        }

        public chessboard(string FEN_notation)
        {
            initialize_objects();
            int i = 0;
            int j = 0;
            int counter = 0;
            var dic = new Dictionary<char, int>()
                            {
                                {'p' , Peace.Pawn} ,
                                {'n' , Peace.Knight} ,
                                {'b' , Peace.Bishop} ,
                                {'r' , Peace.Rook} ,
                                {'q' , Peace.Queen} ,
                                {'k' , Peace.King}
                            };
            foreach (char character in FEN_notation)
            {
                int num= (int)Char.GetNumericValue(character);
                if(num != -1)//if represent a true number
                {
                    j += num;
                }
                else
                {
                    switch (character)
                    {
                        case '/':
                            j = 0;
                            i++;
                            break;
                        case ' ':
                            counter++;
                            goto LoopEnd;
                        default:
                            int peacetype = dic[Char.ToLower(character)];
                            if (Char.IsUpper(character))//if white peace
                                addpeacetoboardandlist(new Peace(true,i*8+j , peacetype));
                            else
                                addpeacetoboardandlist(new Peace(false, i * 8 + j, peacetype));
                            j++;
                            break;
                    }
                }
                counter++;
            }
            
            LoopEnd:
            if (Char.ToLower(FEN_notation[counter++]) == 'w')
                this.whiteturn = true;
            else
                this.whiteturn = false;
            counter = counter + 1;
            this.can_castle[1] = false;
            this.can_castle[2] = false;
            this.can_castle[4] = false;
            this.can_castle[5] = false;
            while (FEN_notation[counter] != ' ')
            {// white all, white left , white right, black all , black left , black right
                if (FEN_notation[counter] == 'K')
                    this.can_castle[2] = true;
                else if (FEN_notation[counter] == 'Q')
                    this.can_castle[1] = true;
                else if (FEN_notation[counter] == 'k')
                    this.can_castle[5] = true;
                else if (FEN_notation[counter] == 'q')
                    this.can_castle[4] = true;
                else if(FEN_notation[counter] == '-')//no one can castle
                {
                    this.can_castle[1] = false;
                    this.can_castle[2] = false;
                    this.can_castle[4] = false;
                    this.can_castle[5] = false;
                    break;
                }
                counter++;
                if (FEN_notation.Length == counter)
                    break;
            }
            //En passant target square ,Halfmove Clock and Fullmove counter are ignored;
        }

        public string get_fen_notation()
        {
            string fen = "";
            int counter = 0;
            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    if (board[i, j].isocupied())
                    {
                        if(counter != 0)
                            fen += counter.ToString();
                        counter = 0;
                        fen += board[i, j].Peace.get_type_char_rep();
                    }
                    else
                    {
                        counter++;
                        if(j == board_size-1)
                            fen += counter.ToString();
                    }
                }
                if(i!= board_size-1)
                    fen += "/";
                counter = 0;
            }
            if(this.whiteturn)
                fen += " w ";
            else
                fen += " b ";
            if(!this.can_castle[1] && !this.can_castle[2] && !this.can_castle[4] && !this.can_castle[5])
                fen += "- ";
            else
            {
                if (this.can_castle[0])
                {
                    if (this.can_castle[2]) fen += "K";
                    if (this.can_castle[1]) fen += "Q";
                }
                if (this.can_castle[3])
                {
                    if (this.can_castle[5]) fen += "k";
                    if (this.can_castle[4]) fen += "q";
                }
            }
            return fen;
        }
        //add peace in a unocupied Tile
        private void addpeacetoboardandlist(Peace peace) 
        {
            board[peace.get_i_pos(), peace.get_j_pos()].Peace = peace;
            addpeacetolist(peace);
        }
        private void removepeacefromlist(Peace peace)
        {
            if (peace.iswhite)
            {
                this.white_parts[peace.type].Remove(peace);
                return;
            }
            this.black_parts[peace.type].Remove(peace);
        }
        private void addpeacetolist(Peace peace)
        {
            if (peace.iswhite)
            {
                this.white_parts[peace.type].Add(peace);
                return;
            }
            this.black_parts[peace.type].Add(peace);
        }
        private List<Peace>[] get_current_player_list()
        {
            if (this.whiteturn)
                return this.white_parts;
            return this.black_parts;
        }

        //take a move - not matter if the move eats a peace of the same color of the eater
        //takes a move - no matter whose turn to play it is!!!!
        //no matter if there is a peace from the same color in it!!!!
        //not chacking if the peace than need to move exist!!!
        //not cheking if the move is legal
        public void manualy_makemove(Move move) //make sure if its last move of the pawn that it is correct
        {
            int istartpo = get_i_pos(move.startsquare);
            int jstartpo = get_j_pos(move.startsquare);
            //
            int iendpo = get_i_pos(move.endsquare);
            int jendpo = get_j_pos(move.endsquare);
            //
            if (this.board[iendpo, jendpo].isocupied()) //if there is a cptured peace in this move
            {
                move.capturedpeace = this.board[iendpo, jendpo].Peace; //save the captured peace
                removepeacefromlist(move.capturedpeace);

                if (move.capturedpeace.type == Peace.Rook)
                {// what happen if rook gets eaten - you have to change the castling rights
                    int offset = getoffset(move.capturedpeace.iswhite);
                    if (this.can_castle[offset])//if king hasnt moved
                    {
                        int row;
                        if (move.capturedpeace.iswhite)
                        {
                            row = 7;
                        }
                        else
                        {
                            row = 0;
                        }
                        if (iendpo == row)//if the eaten rook is the one that can castle
                        {
                            //debug issue in fen :r3k2r/Pppp1ppp/1b3nbN/nPP5/BB2P3/q4N2/P2P2PP/r2Q1RK1 w kq - 0 1
                            //this is why the row value got in, otherwise the board think white eat his left side rook and now he cant castle that side.
                            if (jendpo == 0 && this.can_castle[offset + 1])//eating the left rook that can castle
                            {
                                this.can_castle[offset + 1] = false; // the right rook moved (cant caslte anymore)
                                move.capturedpeace.position -= 64; //mark for the unmakemove func
                            }
                            else if (jendpo == 7 && this.can_castle[offset + 2]) //eating the right rook that can castle
                            { //right rook
                                this.can_castle[offset + 2] = false;// the left rook moved (cant caslte anymore)
                                move.capturedpeace.position -= 64;//mark for the unmakemove func
                            }
                        }
                    }
                }
            }
            else if (move.edgecase == Move.enpassant) // if a En passant movement
            {
                move.capturedpeace = this.board[istartpo, jendpo].Peace; //save the captured pawn
                removepeacefromlist(move.capturedpeace);
                this.board[istartpo, jendpo].Peace = null;//the captured peace squer isnt ocupied anymore
            }
            else if (move.edgecase == Move.castle) // if a castle movement
            { //the king moves more than one square in one move
                int row = 0;
                if (this.board[istartpo, jstartpo].Peace.iswhite)
                    row = 7;
                if(move.endsquare == move.startsquare +2)//right castle
                {//changing the position of the right rook
                    if (this.board[row, 7].Peace == null)
                    {
                        this.printstatics();
                    }
                    this.board[row, 5].Peace = this.board[row, 7].Peace;
                    this.board[row, 5].Peace.position = row * chessboard.board_size+5;
                    this.board[row, 7].Peace = null;
                }
                else if(move.endsquare == move.startsquare - 2)//left castle
                {//changing the position of the left rook
                    if (this.board[row, 0].Peace == null)
                    {
                        this.printstatics();
                    }
                    this.board[row, 3].Peace = this.board[row, 0].Peace;
                    this.board[row, 3].Peace.position = row * chessboard.board_size+3;
                    this.board[row, 0].Peace = null;
                }
            }
            //change board representation
            this.board[iendpo, jendpo].Peace = this.board[istartpo, jstartpo].Peace; // change the position of the mover in the board
            this.board[iendpo, jendpo].Peace.position = move.endsquare;//change the position of the mover in the list
            this.board[istartpo, jstartpo].Peace = null;//the current squer isnt ocupied
            
            change_castling_rights(this.board[iendpo, jendpo].Peace.iswhite,move.edgecase, jstartpo);
            
            //if a pawn does promotion
            if (move.edgecase > 0 && move.edgecase <= 4)
            {
                removepeacefromlist(this.board[iendpo, jendpo].Peace);
                this.board[iendpo, jendpo].Peace.type = move.edgecase;
                addpeacetolist(this.board[iendpo, jendpo].Peace);
            }
            moves.Push(move);
            switchplayerturn();
        }
        
        //changing the castling rights according to the peace than moved and its position and if you make move or unmaking it
        //start position refers to the place the peace was before you make the move!!!
        //return edgecase number for a move
        public void change_castling_rights(bool iswhite, int edgecase, int jstartpo)
        {
            if (kingdidntmoved(iswhite) && edgecase !=0)
            {
                int offset = getoffset(iswhite);
                if (edgecase == Move.rook_moving)
                {
                    if (jstartpo == 0)//left rook moved, cant castle at left side anymove
                    { //left rook
                        this.can_castle[offset + 1] = false;
                    }
                    else //(jstartpo == 7) //right rook moved, cant castle at right side anymove
                    { //right rook
                        this.can_castle[offset + 2] = false;
                    }
                }
                else if (edgecase == Move.king_moving || edgecase == Move.castle)//king moved, cant castle any more
                { //if king moved or castled:
                    this.can_castle[offset] = false;
                }
            }// white all, white left , white right, black all , black left , black right
        }
        
        public void unmake_castling_rights(int edgecase, Boolean iswhite, int jstartpo)
        {
            if (edgecase != 0)
            {
                int offset = getoffset(iswhite);
                if (edgecase == Move.king_moving || edgecase == Move.castle)//king can now castle
                {
                    this.can_castle[offset] = true;
                }
                else if (edgecase == Move.rook_moving)//left rook can now castle
                {
                    if (jstartpo == 0)//left rook moved, can castle at left side
                    { //left rook
                        this.can_castle[offset + 1] = true;
                    }
                    else //(jstartpo == 7) //right rook moved, ca castle at right side
                    { //right rook
                        this.can_castle[offset + 2] = true;
                    }
                }
            }
        }
        
        //return the chess board position back before the last move that has been made
        public void unmakelastmove()
        {
            Move move = this.moves.Pop();

            int istartpo = get_i_pos(move.startsquare);
            int jstartpo = get_j_pos(move.startsquare);
            //
            int iendpo = get_i_pos(move.endsquare);
            int jendpo = get_j_pos(move.endsquare);

            bool peace_got_captured = move.capturedpeace != null;
            if (peace_got_captured && move.capturedpeace.position <0)//rook got eaten and could castle before that
            {// if rook got eaten - you have to change the castling rights
                move.capturedpeace.position += 64;
                int offset = getoffset(move.capturedpeace.iswhite);
                if (jendpo == 0)//eating the left rook
                {
                    this.can_castle[offset + 1] = true; 
                }
                else if (jendpo == 7) //eating the right rook
                { //right rook
                    this.can_castle[offset + 2] = true;
                }
                
            }
            //
            unmake_castling_rights(move.edgecase, this.board[iendpo, jendpo].Peace.iswhite, jstartpo);
            //move the peace position value back
            this.board[iendpo, jendpo].Peace.position = move.startsquare;
            //move the peace position back in the board
            this.board[istartpo, jstartpo].Peace = this.board[iendpo, jendpo].Peace;//the start squer is now ocupied
            //if pawn promoted
            if (move.peacepromote())
            {
                removepeacefromlist(this.board[istartpo, jstartpo].Peace);
                this.board[istartpo, jstartpo].Peace.type = Peace.Pawn;
                addpeacetolist(this.board[istartpo, jstartpo].Peace);
            }
            //
            //
            if (peace_got_captured) //if a peace needs to come back
            {

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
                if (move.edgecase == Move.castle) // if a castle movement
                { //the king moves more than one square in one move
                    int row = 0;
                    if (this.board[istartpo, jstartpo].Peace.iswhite)
                        row = 7;
                    if (move.endsquare == move.startsquare +2)//right castle
                    {//changing the position of the right rook
                        this.board[row, 7].Peace = this.board[row, 5].Peace;
                        this.board[row, 7].Peace.position = row * chessboard.board_size+7;
                        this.board[row, 5].Peace = null;
                    }
                    if (move.endsquare == move.startsquare - 2)//left castle
                    {//changing the position of the left rook
                        this.board[row, 0].Peace = this.board[row, 3].Peace;
                        this.board[row, 0].Peace.position = row * chessboard.board_size;
                        this.board[row, 3].Peace = null;
                    }
                }
            }
            switchplayerturn();
        }
        
        public void manualy_makemove(string four_letters_position, int edgecase)
        {// example: "a1a2"
            //this function works acording to the real notation in a real chess game:
            /*
               +---+---+---+---+---+---+---+---+
               | r |   |   |   | k |   |   | r | 8
               +---+---+---+---+---+---+---+---+
               | p |   | p | p | q | p | b |   | 7
               +---+---+---+---+---+---+---+---+
               | b | n |   |   | p | n | p |   | 6
               +---+---+---+---+---+---+---+---+
               |   |   |   | P | N |   |   |   | 5
               +---+---+---+---+---+---+---+---+
               |   | p |   |   | P |   |   |   | 4
               +---+---+---+---+---+---+---+---+
               |   |   | N |   |   | Q |   | p | 3
               +---+---+---+---+---+---+---+---+
               | P | P | P | B | B | P | P | P | 2
               +---+---+---+---+---+---+---+---+
               | R |   |   |   | K |   |   | R | 1
               +---+---+---+---+---+---+---+---+
                 a   b   c   d   e   f   g   h 
            */
            int init_i = Math.Abs(four_letters_position[1] - '0' - 8);
            int init_j = char.ToUpper(four_letters_position[0]) - 65;
            int final_i = Math.Abs(four_letters_position[3] - '0' - 8);
            int final_j = char.ToUpper(four_letters_position[2]) - 65;

            List<Move> moves = this.generator.generate_legal_moves(this.board[init_i, init_j].Peace);
            moves.Any(move => move.startsquare == init_i * 8 + init_j & move.endsquare == final_i * 8 + final_j);
            foreach (Move move in moves)
            {
                if(move.startsquare == init_i * 8 + init_j & move.endsquare == final_i * 8 + final_j)
                {
                    manualy_makemove(new Move(init_i * 8 + init_j, final_i * 8 + final_j, move.edgecase));
                    return;
                }
            }
            Console.WriteLine("move is ilegal...");
        }

        //switch the boolean value indicates which player turn it is to play
        public void switchplayerturn()
        {
            this.whiteturn = !this.whiteturn;
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
                            tmp += "| . ";
                    }
                    tmp += "|\n";
                }
            }
            tmp += "     0   1   2   3   4   5   6   7  \n";
            if (!tmp.Equals(ToStringfromlist()))
            {
                Console.WriteLine("the list and board arent syncronised!");
                Console.WriteLine(tmp);
                Console.WriteLine(ToStringfromlist());
            }
            return tmp;
        }

        //return a string representation of the board from the list - used for testing internaly
        private string ToStringfromlist()
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
                        //Console.WriteLine(peace.ToString());
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
                        //Console.WriteLine(peace.ToString());
                        board2[get_i_pos(peace.position), get_j_pos(peace.position)] = peace.get_type_char_rep();
                        blackpeacecont++;
                    }
                    else
                        Console.WriteLine("second peace on the same position: " + peace.ToString());
                }
            }
            //Console.WriteLine("black peace count: "+ blackpeacecont);
            //Console.WriteLine("white peace count: " + whitepeacecont);
            string tmp = "";
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

        public string get_all_played_moves(bool with_new_lines =true)
        {
            string movesst = "";
            Stack<Move> tmpmoves = new Stack<Move>();
            while (this.moves.Count != 0)
            {
                tmpmoves.Push(this.moves.Pop());
            }
            while (tmpmoves.Count != 0)
            {
                Move move = tmpmoves.Pop();
                movesst += move.to_mininal_string();
                if (with_new_lines)
                    movesst += "\n";
                else
                    movesst += "-";
                this.moves.Push(move);
            }
            return movesst.Remove(movesst.Length - 1);
        }

        public void printstatics()
        {
            Console.WriteLine(this.ToString());
            Console.WriteLine(this.get_fen_notation());
            Console.WriteLine(this.get_all_played_moves());
        }
        
        //return if the current players turn king's is in check
        public Boolean current_player_king_in_check()
        {
            bool ans = false;
            int kingpos = this.get_current_player_list()[Peace.King][0].position;
            this.switchplayerturn(); //so i could find out if the opponent attack the player
            foreach (Move attackmove in generator.generate_attacking_moves()) //check if the king in check - therfore, checkmate/stalemate.
            {
                if (attackmove.endsquare == kingpos)
                {
                    ans= true; break;
                }
            }
            switchplayerturn();
            return ans;
        }

        //return if the king moved at least once sience the beggining of the game.
        public bool kingdidntmoved(bool iswhite)
        {// check if the king of that peace didnt move from there position once.
            if (iswhite)
                return this.can_castle[0];
            return this.can_castle[3];
        }

        public int getoffset(bool iswhite)
        {
            if (iswhite)
                return 0;
            return 3;
        }
        //gets i/j index for the board matrix representation from a one number position 
        public static int get_i_pos(int position)
        {//return the i value of the peace/move the board matrix
            return position / 8;
        }
        public static int get_j_pos(int position)
        {//return the j value of the peace/move the board matrix
            return position % 8;
        }
        public static char get_j_pos_as_letter(int position)
        {
            const string letters = "ABCDEFGH";
            return letters[get_j_pos(position)];
        }

        //*this evaltuation is relative to the player that makes the move:
        //cases:
        //Evaluate()==0  => the position is equal.
        //Evaluate()>0   => the side who's turn it is to move is doing better.
        //Evaluate()<0   => the other side is doing better.
        public int Evaluate()
        {//doesnt ivaluate checkmates and draws.
            int diff_count=0;
            diff_count += (this.white_parts[Peace.Pawn].Count   - this.black_parts[Peace.Pawn].Count)   * Peace.Pawn_value;
            diff_count += (this.white_parts[Peace.Knight].Count - this.black_parts[Peace.Knight].Count) * Peace.Knight_value;
            diff_count += (this.white_parts[Peace.Bishop].Count - this.black_parts[Peace.Bishop].Count) * Peace.Bishop_value;
            diff_count += (this.white_parts[Peace.Rook].Count   - this.black_parts[Peace.Rook].Count)   * Peace.Rook_value;
            diff_count += (this.white_parts[Peace.Queen].Count  - this.black_parts[Peace.Queen].Count)  * Peace.Queen_value;

            //int prespective= (this.whiteturn) ? 1 : -1; //1 if its white turn, -1 if it its black turn.
            //return diff_count * prespective;
            return diff_count;
        }
    }
}