using SharpDocx;
using System.IO;

namespace Tutorial
{
    internal class Program
    {
        private static readonly string BasePath =
            Path.GetDirectoryName(typeof(Program).Assembly.Location) + @"/../../../../..";

        private static void Main()
        {
            var viewPath = $"{BasePath}/Views/Tutorial.cs.docx";
            var documentPath = $"{BasePath}/Documents/Tutorial.docx";
            var imageDirectory = $"{BasePath}/Images";

#if DEBUG
            string documentViewer = null; // .NET Framework 3.5 or greater will automatically search for a Docx viewer.
            //var documentViewer = @"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE"; // NETCOREAPP3_1 and NET6_0 won't.

            Ide.Start(viewPath, documentPath, null, null, f => f.ImageDirectory = imageDirectory, documentViewer);
#else

            DocumentFileBase document = DocumentFactory.Create(viewPath);
            document.ImageDirectory = imageDirectory;
            document.Generate(documentPath);
            Console.WriteLine($"Succesfully generated {documentPath} using view {viewPath}.");

            var documentStreamPath = documentPath.Replace(".docx", ".stream.docx");
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(viewPath));
            var documentStream = DocumentFactory.Create((new FileInfo(viewPath)).Name, ms);
            documentStream.ImageDirectory = imageDirectory;
            var outputStream = documentStream.Generate(ms);
            File.WriteAllBytes(documentStreamPath, outputStream.ToArray());

#endif
        }
    }
}