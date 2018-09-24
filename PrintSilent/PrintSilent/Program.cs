using System;
using System.ComponentModel;

namespace PrintSilent
{
    public class Program
    {
        private static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
            var myPrint = new Printing("Formh");
            myPrint.ProcessPrint("E:\\MyFile.txt");
        }
    }
}