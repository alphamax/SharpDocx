﻿using System;
using System.Diagnostics;
using System.IO;

namespace SharpDocx
{
    public static class Ide
    {
        public static void Start(
            string viewPath,
            string documentPath,
            object model = null,
            Type baseClassType = null,
            Action<DocumentBase> initializeDocument = null,
            string documentViewer = null)
        {
            Console.WriteLine("Initializing SharpDocx IDE...");

            viewPath = Path.GetFullPath(viewPath);
            documentPath = Path.GetFullPath(documentPath);
            ConsoleKeyInfo keyInfo;

            do
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Compiling '{viewPath}'.");
                    var document = DocumentFactory.Create(viewPath, model, baseClassType, true);
                    initializeDocument?.Invoke(document);
                    document.Generate(documentPath);
                    Console.WriteLine($"Succesfully generated '{documentPath}'.");

                    var documentStreamPath = documentPath.Replace(".docx", ".stream.docx");
                    using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(viewPath)))
                    {
                        var documentStream = DocumentFactory.Create((new FileInfo(viewPath)).Name, ms, model, baseClassType, true);
                        initializeDocument?.Invoke(documentStream);
                        using (MemoryStream outputStream = documentStream.Generate(ms))
                        {
                            File.WriteAllBytes(documentStreamPath, outputStream.ToArray());
                        }
                    }

                    try
                    {
                        // Show the generated document.
                        if (documentViewer != null)
                        {
                            Process.Start(documentViewer, documentPath);
                            Process.Start(documentViewer, documentStreamPath);
                        }
#if NET35_OR_GREATER
                        else
                        {
                            Process.Start(documentPath);
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        // Ignored.
                    }
                }
                catch (SharpDocxCompilationException e)
                {
                    Console.WriteLine(e.SourceCode);
                    Console.WriteLine(e.Errors);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                GC.Collect();
                Console.WriteLine("Press Esc to exit, any other key to retry . . .");
                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
}