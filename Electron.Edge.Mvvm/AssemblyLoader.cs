using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Electron.Edge.Mvvm
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        private readonly string directoryPath;

        public AssemblyLoader(string directoryPath) => this.directoryPath = directoryPath;

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var dependencyContext = DependencyContext.Default;
            var compilationLibraries = dependencyContext
                .CompileLibraries
                .Where(x => x.Name.Contains(assemblyName.Name))
                .ToList();

            if (compilationLibraries.Count > 0)
            {
                return Assembly.Load(new AssemblyName(compilationLibraries.First().Name));
            }
            else
            {
                var file = new FileInfo($"{this.directoryPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");
                if (File.Exists(file.FullName))
                {
                    var asemblyLoader = new AssemblyLoader(file.DirectoryName);
                    return asemblyLoader.LoadFromAssemblyPath(file.FullName);
                }
            }

            return Assembly.Load(assemblyName);
        }
    }
}