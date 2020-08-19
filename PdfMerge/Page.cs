using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfMerge
{
    public class Page
    {
        private SourcePdf _source;
        private int _pageNo;
        public Page(SourcePdf source, int pageNo = -1)
        {
            _source = source;
            _pageNo = pageNo;
        }

        public SourcePdf Source
        {
            get { return _source; }
        }
        public int PageNo
        {
            get { return _pageNo; }
        }
    }
}
