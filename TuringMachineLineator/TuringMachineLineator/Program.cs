using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineLineator
{
    class Program
    {
        static void Main(string[] args)
        {
            Lineator lineator = new Lineator();

            Console.WriteLine("Please enter the file you wish to open...");
            string filename = Console.ReadLine();
            lineator.Import(filename);
            lineator.Lineate(filename + "_Flat");
        }
    }
}
