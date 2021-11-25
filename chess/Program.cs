using chess.types_of_peaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chess
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();
            //Console.WriteLine(true +1);
            Console.ForegroundColor = ConsoleColor.Red;
            chessboard board = new chessboard();
            Console.WriteLine("start position");
            Console.WriteLine(board.ToString());

            List<Move> moves = board.generator.generate_moves();
            int count = 0;
            foreach (Move move in moves)
            {
                board.manualy_makemove(move);
                Console.WriteLine(board.ToString());
                List<Move> moves2 = board.generator.generate_moves();
                
                foreach (Move move2 in moves2)
                {
                    board.manualy_makemove(move2);
                    Console.WriteLine(board.ToString());
                    /*
                    List<Move> moves3 = m.generate_moves();

                    foreach (Move move3 in moves3)
                    {
                        c.manualy_makemove(move3);
                        Console.WriteLine(c.ToString());
                        count++;
                        c.unmakemove();
                    }*/

                    board.unmakemove();
                }
                board.unmakemove();
            }
            Console.WriteLine(count);

            /*
            Move one = new Move(48, 8);
            Move two = new Move(49, 9);
            c.manualy_makemove(one);
            c.manualy_makemove(two);
            Console.WriteLine(c.ToStringfromlist());
            
            c.unmakemove(); c.unmakemove();
            */
            Console.WriteLine("end position");
            Console.WriteLine(board.ToStringfromlist());

            stopwatch.Stop(); Console.WriteLine("Elapsed Time is {0} seconds", (float)stopwatch.ElapsedMilliseconds/1000);
            Console.ReadLine();
        }
    }
}
