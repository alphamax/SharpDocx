﻿//#define SUPPORT_MULTI_THREADING_AND_LARGE_DOCUMENTS_IN_NET35

using DocumentFormat.OpenXml.Packaging;
using System.IO;

namespace SharpDocx
{
    public abstract class DocumentStreamBase : DocumentBase
    {
        public MemoryStream Generate(Stream inputStream, object model = null)
        {
            inputStream.Seek(0, SeekOrigin.Begin);
            MemoryStream outputstream = new MemoryStream();
            inputStream.CopyTo(outputstream);

#if NET35 && SUPPORT_MULTI_THREADING_AND_LARGE_DOCUMENTS_IN_NET35
            // Due to a bug in System.IO.Packaging writing large uncompressed parts (>10MB) isn't thread safe in .NET 3.5.
            // Workaround: make writing in all threads and processes sequential.
            // Microsoft fixed this in .NET 4.5 (see https://maheshkumar.wordpress.com/2014/10/21/).
            PackageMutex.WaitOne(Timeout.Infinite, false);

            try
            {
#endif
            outputstream.Seek(0, SeekOrigin.Begin);
            using (Package = WordprocessingDocument.Open(outputstream, true))
            {
                GenerateInternal(model);
            }

#if NET35 && SUPPORT_MULTI_THREADING_AND_LARGE_DOCUMENTS_IN_NET35
            }
            finally
            {
                PackageMutex.ReleaseMutex();
            }
#endif
            // Reset position of stream.
            outputstream.Seek(0, SeekOrigin.Begin);
            return outputstream;
        }

        internal void Init(object model)
        {
            base.InitBase(model);
        }
    }
}