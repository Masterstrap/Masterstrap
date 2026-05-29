
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Masterstrap
{
    public static class DependencyLoader
    {
        public static void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                string assemblyShortName = new AssemblyName(args.Name).Name ?? string.Empty;
                Assembly assembly = ((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies()).FirstOrDefault<Assembly>((Func<Assembly, bool>)(a => a.GetName().Name == assemblyShortName));
                if (assembly != null)
                    return assembly;
                string str1 = assemblyShortName + ".dll";
                string[] strArray = new string[3]
                {
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str1),
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libs", str1),
            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof (object).Assembly.Location) ?? string.Empty, str1)
                };
                foreach (string str2 in strArray)
                {
                    if (File.Exists(str2))
                        return Assembly.LoadFrom(str2);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
