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
            Console.ForegroundColor = ConsoleColor.Red;
            Stopwatch stopwatch = new Stopwatch(); stopwatch.Start();
            //Program p = new Program();
            //p.event_handler();
            string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - ";
            chessboard board = new chessboard(fen);
            Console.WriteLine("start position");
            board.printstatics();

            fen = board.get_fen_notation();
            int nodes = board.generator.Perft(5);
            Console.WriteLine(nodes+" leaf nodes");

            Console.WriteLine("end position");
            Console.WriteLine(board.ToStringfromlist());


            stopwatch.Stop(); Console.WriteLine("Elapsed Time is {0} seconds", (float)stopwatch.ElapsedMilliseconds/1000);
            if (!fen.Equals(board.get_fen_notation()))
            {
                Console.WriteLine(fen);
                Console.WriteLine(board.get_fen_notation());
                Console.WriteLine("something went wrong");
            }
            Console.ReadLine();
        }
        private void event_handler()
        {
            chessboard board = new chessboard();
            string input = Console.ReadLine();
            while (!input.Equals("done"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                switch (input)
                {
                    case "d":
                        Console.WriteLine(board.ToString());
                        break;
                    case string s when s.StartsWith("go perft"):
                        int num = Int32.Parse(input.Substring(9));
                        Console.WriteLine(board.generator.Perft(num));
                        break;
                    case string s when s.StartsWith("position fen"):
                        input.Remove(0, 13);
                        board = new chessboard(input);
                        break;
                    case "":
                        Console.WriteLine();
                        break;
                    default:
                        Console.WriteLine("unknown command: " + input);
                        break;
                }
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
            }
        }
    }
}
