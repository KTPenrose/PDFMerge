using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfMerge
{
    public class SourcePdf
    {
        private string _path;
        private string _nickname;

        private PdfDocument _pdfDocument;
        public SourcePdf(string path, string nickname)
        {
            _path = path;
            _nickname = nickname;
        }

        public string Nickname
        {
            get { return _nickname; }
        }

        public string Path
        {
            get { return _path; }
        }

        public PdfDocument PdfDocument 
        {
            get
            {
                if (_pdfDocument==null)
                {
                    _pdfDocument = PdfReader.Open(_path, PdfDocumentOpenMode.Import);
                }
                return _pdfDocument;
            }
        }
    }
}
