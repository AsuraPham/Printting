using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PrintSilent
{
    public class Printing
    {
        private Font _printFont;
        private StreamReader _streamToPrint;

        private string _printerName;
        private readonly string _printingForm;

        public Printing(string printingForm)
        {
            _printingForm = printingForm;
        }

        private void Print(PrintDocument printDocument)
        {
            if (!printDocument.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer");
            }

            if (!string.IsNullOrEmpty(_printerName))
            {
                printDocument.PrinterSettings.PrinterName = _printerName;
            }

            printDocument.PrintPage += pd_PrintPage;
            printDocument.PrintController = new StandardPrintController();
            printDocument.Print();
        }

        public void ProcessPrint(string filePath)
        {
            var printDocument = new PrintDocument();

            LoadPrinterNameByForm(_printingForm);

            //Load Printing config from webapi

            // Set print setting
            SetPrintSetting(printDocument);

            try
            {
                // Load pdf file to stream
                _streamToPrint = new StreamReader(filePath);

                if (string.IsNullOrEmpty(_printerName))
                {
                    // show dialog
                    var printDialog = new PrintDialog { Document = printDocument };
                    var result = printDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        // Save
                        var printName = printDialog.PrinterSettings.PrinterName;
                        AppendToConfigFile(new PrinterByFormConfigObj
                        {
                            FormName = _printingForm,
                            PrinterName = printName
                        });

                        Print(printDocument);
                    }
                }
                else
                {
                    Print(printDocument);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _streamToPrint.Close();
            }
        }

        private void SetPrintSetting(PrintDocument printDocument)
        {
            printDocument.DefaultPageSettings.Landscape = true;
            // https://stackoverflow.com/questions/684801/setting-the-paper-size
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("210 x 297 mm", 800, 800);
        }

        private void LoadPrinterNameByForm(string formName)
        {
            // Read file
            var listPrinterForm = GetListPrintersFromConfigFile();

            if (listPrinterForm == null || !listPrinterForm.Any())
            {
                _printerName = string.Empty;
                return;
            }

            var printerByFormName = listPrinterForm.FirstOrDefault(x => x.FormName.ToLower().Equals(formName.ToLower()));
            if (printerByFormName == null)
            {
                _printerName = string.Empty;
                return;
            }

            _printerName = printerByFormName.PrinterName;
        }

        private List<PrinterByFormConfigObj> GetListPrintersFromConfigFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("PrinterConfiguration.txt"))
                {
                    // Read the stream to a string and write the string to the console
                    string line;
                    var listPrinters = new List<PrinterByFormConfigObj>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] words = line.Split(':');
                        listPrinters.Add(new PrinterByFormConfigObj
                        {
                            FormName = words[0],
                            PrinterName = words[1].TrimStart().TrimEnd()
                        });
                    }
                    sr.Close();

                    return listPrinters;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void AppendToConfigFile(PrinterByFormConfigObj obj)
        {
            File.AppendAllText(@"PrinterConfiguration.txt", $"{obj.FormName}: {obj.PrinterName}" + Environment.NewLine);
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
                           _printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
                   ((line = _streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                                    _printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, _printFont, Brushes.Black,
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