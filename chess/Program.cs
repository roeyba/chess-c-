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

            Console.ForegroundColor = ConsoleColor.Red;
            chessboard c = new chessboard();
            Console.WriteLine(c.ToString());

            Movegenerator m = new Movegenerator(c);
            List<Move> moves = m.generate_moves();
            
            foreach (Move move in moves)
            {
                c.manualy_makemove(move);
                Console.WriteLine(c.ToString());
                c.unmakemove();
            }

            /*
            Move one = new Move(48, 8);
            Move two = new Move(49, 9);
            c.manualy_makemove(one);
            c.manualy_makemove(two);
            Console.WriteLine(c.ToStringfromlist());
            
            c.unmakemove(); c.unmakemove();
            */
            Console.WriteLine(c.ToStringfromlist());

            stopwatch.Stop(); Console.WriteLine("Elapsed Time is {0} seconds", (float)stopwatch.ElapsedMilliseconds/1000);
            Console.ReadLine();
        }
    }
}
