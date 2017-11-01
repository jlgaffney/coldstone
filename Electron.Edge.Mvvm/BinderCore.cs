using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Electron.Edge.Mvvm
{
    public class BinderCore
    {
        private const string DynamicLinkLibraryExtension = "dll";

        private ViewModelRepository viewModelRepo;

        public BinderCore()
        {
            AssemblyLoadContext.Default.Resolving += TryLoadFromSameFolder;
        }

        private static Assembly TryLoadFromSameFolder(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var assemblyPath = Path.Combine(folderPath, assemblyName.Name + "." + DynamicLinkLibraryExtension);

            return File.Exists(assemblyPath) ? context.LoadFromAssemblyPath(assemblyPath) : null;
        }

        public object Initialize(string viewModelAssemblyPath)
        {
            Assembly viewModelAssembly;
            try
            {
                viewModelAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(viewModelAssemblyPath);
            }
            catch
            {
                // Assembly may already be loaded
                viewModelAssembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(viewModelAssemblyPath)));
            }

            this.viewModelRepo = new ViewModelRepository(viewModelAssembly);

            return string.Empty;
        }

        public object CreateViewModel(string viewModelName)
        {
            var vm = viewModelRepo.Create(viewModelName);
            return vm.Id;
        }

        public object GetPropertyValue(string id, string property)
        {
            var vm = viewModelRepo.GetViewModel(id);
            return vm?.GetPropertyValue(property).ToString();
        }

        public object GetPropertyAsViewModel(string id, string property)
        {
            var vm = viewModelRepo.GetViewModel(id);
            var child = vm?.GetPropertyAsViewModel(property);

            if (child == null) return null;

            // Add child ViewModel to repository if required
            if (!viewModelRepo.Contains(child.Id))
            {
                viewModelRepo.Add(child);
            }

            return child.Id;
        }

        public object SetPropertyValue(string id, string property, string value)
        {            
            var vm = viewModelRepo.GetViewModel(id);
            vm?.SetPropertyValue(property, value);
            return null;
        }

        public object BindToProperty(string id, string property, Func<object, Task<object>> onChanged)
        {
            var vm = viewModelRepo.GetViewModel(id);
            vm?.Bind(property, onChanged);
            return null;
        }

        public object ExecuteCommand(string id, string command)
        {
            var vm = viewModelRepo.GetViewModel(id);
            vm?.ExecuteCommand(command);
            return null;
        }
    }
}