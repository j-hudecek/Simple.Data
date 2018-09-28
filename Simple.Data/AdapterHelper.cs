using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Simple.Data
{
    class AdapterHelper
    {
        /*private static IAdapter ComposeAdapter(string contractName)
        {
            using (var container = CreateContainer())
            {
                var export = container.GetExport<IAdapter>(contractName);
                if (export == null) throw new ArgumentException("Unrecognised file.");
                return export.Value;
            }
        }*/

        private static CompositionHost CreateContainer()
        {
            var path = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
            path = Path.GetDirectoryName(path);
            if (path == null) throw new ArgumentException("Unrecognised file.");

            var assemblies = Directory.GetFiles(path, "Simple.Data.*.dll")
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies);
            return configuration.CreateContainer();
        }
    }
}
