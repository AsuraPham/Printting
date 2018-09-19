using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintSilent
{
    class Printing
    {
        private Font printFont;
        private StreamReader streamToPrint;

        public void Print(string filePath)
        {
            try
            {
                streamToPrint = new StreamReader(filePath);
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                {
                    var lsPrinter = new List<string>
                    {
                        printer
                    };
                }
                try
                {
                    printFont = new Font("Arial", 10);
                    PrintDocument pd = new PrintDocument();
                    if (!pd.PrinterSettings.IsValid)
                    {
                        throw new Exception("Error: cannot find the default printer");
                    }

                    PrintDialog printDialog = new PrintDialog {Document = pd};
                    DialogResult result = printDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        pd.PrintPage += new PrintPageEventHandler
                            (this.pd_PrintPage);
                        pd.PrintController = new StandardPrintController();
                        pd.Print();
                    }
                    
                }
                finally
                {
                    streamToPrint.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
                           printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
                   ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                                    printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                    leftMargin, yPos, new StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }
    }
}
