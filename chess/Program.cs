using chess.types_of_peaces;
using System;
using System.Collections.Generic;
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
            chessboard c = new chessboard();

            Console.WriteLine(c.whitetomove);
            Move one = new Move(48, 8);
            Move two = new Move(49, 9);
            c.manualy_makemove(one);
            c.manualy_makemove(two);
            Console.WriteLine(c.ToStringfromlist());
            //until here its fine
            


            c.unmakemove(); c.unmakemove();
            Console.WriteLine(c.ToStringfromlist());

            Console.WriteLine(c.whitetomove);
            Console.ReadLine();
        }
    }
}
