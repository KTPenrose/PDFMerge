PDF Merge
=========

This software and corresponding source code are free to use for any purpose, commercial or otherwise.   It is a simple command-line front end to the open source [PDFsharp library] (https://sourceforge.net/projects/pdfsharp).

Use this command line utility to merge two or more PDF files. See the 'examples' directory for working copies of the provided examples.  To run them, open a command prompt, navigate to the directory, and execute the batch file (*.bat) in the corresponding directory.

The utility is driven from an XML file.  The XML file specifies one or more inputs (file node), one output (output node), and then one or more pages (page node).  It is obvious from the examples below.


Merge
-----

Create a file 'merge.xml'

&lt;pdfmerge&gt;0\
   &lt;file path='Pages1and3.pdf' nickname='doc1'/&gt;\
   &lt;file path='Pages2and4.pdf' nickname='doc2'/&gt;\
   &lt;output path='output.pdf' allowoverwrite='true'/&gt;\
   &lt;page source='doc1'/&gt;\
   &lt;page source='doc2'/&gt;\
&lt;/pdfmerge&gt;

run the following at the command prompt:

pdfmerge.exe -f merge.xml

Note that the <page> node does not contain a page number, so all pages from the specified source will be output.


Collate
-----

Create a file 'collate.xml'

&lt;pdfmerge&gt;\
   &lt;file path='Pages1and3.pdf' nickname='doc1'/&gt;\
   &lt;file path='Pages2and4.pdf' nickname='doc2'/&gt;\
   &lt;output path='output.pdf' allowoverwrite='true'/&gt;\
   &lt;page source='doc1' pageno='1'/&gt;\
   &lt;page source='doc2' pageno='1'/&gt;\
   &lt;page source='doc1' pageno='2'/&gt;\
   &lt;page source='doc2' pageno='2'/&gt;\
&lt;/pdfmerge&gt;\

run the following at the command prompt:

pdfmerge.exe -f collate.xml

Note: the output will collate the two documents, placing the pages in the following order: doc1 page 1, doc2 page 1, doc1 page 2, doc2 page 2.



