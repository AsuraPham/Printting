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
            var myPrint = new Printing("FormB");
            // myPrint.ReadFile();
            //PrinterByFormConfigObj ab=new PrinterByFormConfigObj("ahha","hehe");
             //myPrint.AppendToConfigFile(ab);
            myPrint.ProcessPrint("E:\\MyFile.txt");
        }
    }
}