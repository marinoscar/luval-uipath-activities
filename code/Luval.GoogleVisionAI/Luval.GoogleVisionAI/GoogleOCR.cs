using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoogleVisionAI
{
    public class GoogleOCR
    {
        private readonly string _apiKey;

        public GoogleOCR(string apiKey)
        {
            _apiKey = apiKey;
            GoogleCredential.FromAccessToken(_apiKey);

        }

        public string DoOcr(string pdfFile)
        {
            if (string.IsNullOrWhiteSpace(pdfFile)) throw new ArgumentNullException("PDF file argument cannot be null");
            var inputFileInfo = new FileInfo(pdfFile);
            if (!inputFileInfo.Exists) throw new FileNotFoundException("PDF file doest not exists");

            var session = Guid.NewGuid().ToString();
            var workingDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            if (!Directory.Exists(Path.Combine(workingDir.FullName, "luval-gs")))
                Directory.CreateDirectory(Path.Combine(workingDir.FullName, "luval-gs"));

            var fileDir = Directory.CreateDirectory(Path.Combine(workingDir.FullName, string.Format(@"luval-gs\{0}", session)));
            inputFileInfo = inputFileInfo.CopyTo(Path.Combine(fileDir.FullName, inputFileInfo.Name));

            var text = new StringWriter();
            foreach (var img in Pdf2Image.Convert(inputFileInfo, fileDir))
            {
                var content = GetOCR(img);
                text.WriteLine(content);
            }
            return text.ToString();
        }

        private string GetOCR(FileInfo imageFile)
        {
            
            if (!imageFile.Exists) throw new ArgumentException("Image file does not exist");
            //var credential = GoogleCredential.FromFile(jsonPath);
            //var storage = StorageClient.Create(credential);

            var client = ImageAnnotatorClient.Create();
            var image = Image.FromFile(imageFile.FullName);
            var response = client.DetectDocumentText(image);
            return response.Text;
        }


    }
}
