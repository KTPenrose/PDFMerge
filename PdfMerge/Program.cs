using System;
using System.Linq;
using System.Reflection;

namespace PdfMerge
{
    class Program
    {

        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.AssemblyResolve += (sender, bargs) =>
            {
                String dllName = new AssemblyName(bargs.Name).Name + ".dll";
                var assem = Assembly.GetExecutingAssembly();
                String resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));
                if (resourceName == null) return null; // Not found, maybe another handler will find it
                using (var stream = assem.GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            PdfMerge merge = new PdfMerge();
            merge.Merge(args);
            
        }

    }
}
