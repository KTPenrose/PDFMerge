using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PdfMerge
{
    public class PdfMerge
    {
        private const string PDFMERGE = "pdfmerge";

        private const string FILE = "file";
        private const string PATH = "path";
        private const string NICKNAME = "nickname";

        private const string OUTPUT = "output";
        private const string ALLOWOVERWRITE = "allowoverwrite";

        private const string PAGE = "page";
        private const string SOURCE = "source";
        private const string PAGENO = "pageno";

        private IXmlLineInfo _xmlInfo;



        public void Merge(string[] args)
        {

            if (args.Length != 2 || (args.Length > 1 && args[0] != "-f"))
            {
                Console.WriteLine("-f [xml filename]");
                Console.WriteLine();
                Console.WriteLine("XML File looks like:");
                Console.WriteLine("<pdfmerge>");
                Console.WriteLine("   <file path='filename1' nickname='nickname1'/>");
                Console.WriteLine("   <file path='filename2' nickname='nickname2'/>");
                Console.WriteLine("   <output path='filename' allowoverwrite='true or false'/>");
                Console.WriteLine("   <page source='nickname1' pageno='page #'/>");
                Console.WriteLine("   <page source='nickname2' pageno='page #'/>");
                Console.WriteLine("</pdfmerge>");
            }
            else
            {
                //LoadPdfSharpDll();
                string filename = args[1];
                if (!File.Exists(filename))
                {
                    Console.WriteLine("XML file not found: " + filename);
                }
                else
                {
                    List<SourcePdf> sources = new List<SourcePdf>();
                    List<Page> pages = new List<Page>();
                    string outputPath = null;
                    bool allowoverwrite = false;
                    try
                    {
                        using (FileStream s = new FileStream(filename, FileMode.Open))
                        {
                            using (XmlReader reader = XmlReader.Create(s))
                            {
                                _xmlInfo = (IXmlLineInfo)reader;
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        if (reader.Name == FILE)
                                        {
                                            string path = ReadPath(reader, true);
                                            string nickname = ReadNickname(reader);
                                            sources.Add(new SourcePdf(path, nickname));
                                        }
                                        else if (reader.Name == OUTPUT)
                                        {
                                            string path = ReadPath(reader, false);
                                            string allowoverwriteS = reader.GetAttribute(ALLOWOVERWRITE);
                                            allowoverwrite = (allowoverwriteS != null && "true".Equals(allowoverwriteS.ToLower()));
                                            if (!allowoverwrite && File.Exists(path))
                                            {
                                                Console.WriteLine("Output file already exists: " + path + " on line " + _xmlInfo.LineNumber);
                                                return;
                                            }
                                            outputPath = path;
                                        }
                                        else if (reader.Name == PAGE)
                                        {
                                            SourcePdf source = ReadSource(reader, sources);
                                            int pageNo = ReadPageNo(reader);
                                            pages.Add(new Page(source, pageNo));
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        if (reader.Name == PDFMERGE)
                                        {
                                            if (sources.Count == 0)
                                            {
                                                throw new Exception("No file (source) elements were specified.");
                                            }
                                            if (outputPath == null)
                                            {
                                                throw new Exception("No output element was specified.");
                                            }
                                            if (pages.Count == 0)
                                            {
                                                throw new Exception("No page elements were specified.");
                                            }

                                            using (PdfDocument outPdf = new PdfDocument())
                                            {
                                                foreach (Page p in pages)
                                                {
                                                    CopyPages(p, outPdf);
                                                }
                                                if (allowoverwrite && File.Exists(PATH))
                                                {
                                                    FileInfo fi = new FileInfo(outputPath);
                                                    fi.Delete();
                                                    fi.Refresh();
                                                    int ct = 0;
                                                    while (fi.Exists && ct++ < 100)
                                                    {
                                                        System.Threading.Thread.Sleep(100);
                                                        fi.Refresh();
                                                    }
                                                    if (fi.Exists)
                                                    {
                                                        throw new Exception("Could not overwrite file '" + outputPath + "'.");
                                                    }
                                                }
                                                outPdf.Save(outputPath);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        private static void CopyPages(Page p, PdfDocument outPdf)
        {
            if (p.PageNo == -1)
            {
                for (int i = 0; i < p.Source.PdfDocument.PageCount; i++)
                {
                    outPdf.AddPage(p.Source.PdfDocument.Pages[i]);
                }
            }
            else if (p.PageNo > 0 && p.PageNo <= p.Source.PdfDocument.PageCount)
            {
                Console.WriteLine("Writing page " + p.PageNo + " of " + p.Source.Path);
                outPdf.AddPage(p.Source.PdfDocument.Pages[p.PageNo - 1]);
            }
        }

        private string ReadPath(XmlReader reader, bool shouldExist)
        {
            string path = reader.GetAttribute(PATH);
            if (path == null)
            {
                throw new Exception("No path attribute specified for element on line " + _xmlInfo.LineNumber);
            }
            System.IO.FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(path);
            }
            catch (ArgumentException) { }
            catch (System.IO.PathTooLongException) { }
            catch (NotSupportedException) { }
            if (ReferenceEquals(fi, null))
            {
                throw new Exception("'" + path + "' is not a valid filename.");
            }
            if (shouldExist && !File.Exists(path))
            {
                Console.WriteLine("File not found: '" + path + "' on line " + _xmlInfo.LineNumber);
            }
            return path;
        }

        private string ReadNickname(XmlReader reader)
        {
            string nickname = reader.GetAttribute(NICKNAME);
            if (nickname == null)
            {
                throw new Exception("No nickname attribute specified for element on line " + _xmlInfo.LineNumber);
            }
            if (String.IsNullOrWhiteSpace(nickname))
            {
                throw new Exception("Nickname attribute cannot be empty for element on line " + _xmlInfo.LineNumber);
            }
            return nickname;
        }

        private int ReadPageNo(XmlReader reader)
        {
            string pageS = reader.GetAttribute(PAGENO);
            if (String.IsNullOrWhiteSpace(pageS))
            {
                return -1;
            }
            try
            {
                return Int32.Parse(pageS);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " on line " + _xmlInfo.LineNumber);
            }
        }

        private SourcePdf ReadSource(XmlReader reader, List<SourcePdf> sources)
        {
            string nickname = reader.GetAttribute(SOURCE);
            if (nickname == null)
            {
                throw new Exception("No source attribute specified for element on line " + _xmlInfo.LineNumber);
            }
            if (String.IsNullOrWhiteSpace(nickname))
            {
                throw new Exception("Nickname attribute cannot be empty for element on line " + _xmlInfo.LineNumber);
            }
            foreach (SourcePdf s in sources)
            {
                if (s.Nickname.Equals(nickname, StringComparison.CurrentCultureIgnoreCase))
                {
                    return s;
                }
            }
            throw new Exception("Unknown nickname '+nickname+' for element on line " + _xmlInfo.LineNumber);
        }

    }
}
