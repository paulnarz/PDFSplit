using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFSplit
{
    public class FileSplitOptions
    {
        public string InputFilePath { get; set; }
        public string OutputFilePattern { get; set; }        
        public int StartingIndex { get; set; } = 1;
        public int IndexIncrement { get; set; } = 1;
    }

    class Program
    {
        static void Main(string[] args)
        {            
            var options = new List<FileSplitOptions>()
            {
                new FileSplitOptions() {
                    InputFilePath = @"C:\Dev\quadrophenia.pdf",
                    OutputFilePattern = @"C:\Dev\quad_{0}.pdf",
                },
            };

            try
            {
                foreach (var option in options)
                {
                    SplitDocs(option);
                    Console.WriteLine("done.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }           
        }

        public static void SplitDocs(FileSplitOptions options)
        {
            // Open the file
            using (PdfDocument inputDocument = PdfReader.Open(options.InputFilePath, PdfDocumentOpenMode.Import))
            {
                int index = options.StartingIndex;
                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    string outputFileName = string.Format(options.OutputFilePattern, index);
                    index += options.IndexIncrement;

                    // Create new document
                    using (PdfDocument outputDocument = new PdfDocument())
                    {
                        outputDocument.Version = inputDocument.Version;
                        outputDocument.Info.Title = outputFileName;
                        outputDocument.Info.Creator = inputDocument.Info.Creator;

                        // Add the page and save it
                        outputDocument.AddPage(inputDocument.Pages[i]);
                        outputDocument.Save(outputFileName);
                    }
                }
            }
        }
    }
}
