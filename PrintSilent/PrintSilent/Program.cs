using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;


namespace PrintSilent
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var myPrint = new Printing();
            myPrint.Print("E:\\MyFile.txt");
        }
    }
}