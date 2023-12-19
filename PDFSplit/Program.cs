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
        public List<int> Groups { get; set; }
        public List<int> PageNumbers { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var options = new List<FileSplitOptions>()
            {
                new FileSplitOptions()
                {
                    InputFilePath = @"C:\Users\Paul\Downloads\OnslaughtRulebookWeb (small).pdf",
                    OutputFilePattern = @"C:\Users\Paul\Downloads\OnslaughtRulebookWeb_{0}.pdf",         
                    StartingIndex = 25,
                    PageNumbers = new List<int>()
                    {
                        25,
                    }
                }                

                //new FileSplitOptions()
                //{
                //    InputFilePath = @"C:/Users/Paul/Documents/_pb/health/glasses_prescription_2020.pdf",
                //    OutputFilePattern = @"C:/Users/Paul/Documents/_pb/health/glasses_prescription_2020_p{0}.pdf",
                //},
                //new FileSplitOptions() {
                //    InputFilePath = @"C:\dev\games\earth reborn\EarthReborn.Rules.pdf",
                //    OutputFilePattern = @"C:\dev\games\earth reborn\rules\Rules.{0:d2}.pdf",
                //    Groups = new List<int>()
                //    {
                //        4, 3, 2, 4, 1, 2, 1, 4, 2, 2, 2, 2, 4, 1, 3, 2, 4
                //    }
                //},
                //new FileSplitOptions() {
                //    InputFilePath = @"C:\dev\games\earth reborn\EarthReborn.Scenarios.pdf",
                //    OutputFilePattern = @"C:\dev\games\earth reborn\rules\Scenarios.{0:d2}.pdf",
                //    Groups = new List<int>()
                //    {
                //        3, 3, 5, 3
                //    }
                //},
                //new FileSplitOptions() {
                //    InputFilePath = @"C:\dev\games\earth reborn\EarthReborn.Guide.pdf",
                //    OutputFilePattern = @"C:\dev\games\earth reborn\rules\Guide.{0:d2}.pdf",
                //    Groups = new List<int>()
                //    {
                //        3, 4, 1, 3
                //    }
                //},
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
                List<List<PdfPage>> pageGroups = new List<List<PdfPage>>();

                if (options.PageNumbers != null)
                {
                    foreach (var pageNumber in options.PageNumbers)
                    {
                        pageGroups.Add(new List<PdfPage>() { inputDocument.Pages[pageNumber - 1] });
                    }
                }
                else if (options.Groups == null)
                {
                    foreach (var page in inputDocument.Pages)
                    {
                        pageGroups.Add(new List<PdfPage>() { page });
                    }
                }
                else
                {
                    var i = 0;                    
                    foreach (var group in options.Groups)
                    {
                        var j = 0;
                        var pageGroup = new List<PdfPage>();
                        while (i < inputDocument.Pages.Count && j < group)
                        {
                            pageGroup.Add(inputDocument.Pages[i++]);
                            j++;
                        }
                        pageGroups.Add(pageGroup);
                    }
                }

                int index = options.StartingIndex;
                foreach (var pageGroup in pageGroups)
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
                        foreach (var page in pageGroup)
                        {
                            outputDocument.AddPage(page);
                        }
                        
                        outputDocument.Save(outputFileName);
                    }
                }
            }
        }
    }
}
