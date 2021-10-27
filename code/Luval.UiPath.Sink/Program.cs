using Luval.GoogleVisionAI;
using System;

namespace Luval.UiPath.Sink
{
    class Program
    {
        static void Main(string[] args)
        {
            var ocr = new GoogleOCR("");
            var result = ocr.DoOcr(@"C:\Users\CH489GT\Downloads\54418385.pdf");

        }
    }
}
