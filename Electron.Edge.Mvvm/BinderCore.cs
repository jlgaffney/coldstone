using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Electron.Edge.Mvvm
{
    public class BinderCore
    {
        private const string DynamicLinkLibraryExtension = "dll";

        private ViewModelRepository viewModelRepo;

        public BinderCore()
        {
            AppDomain.CurrentDomain.AssemblyResolve += LoadFromSameFolder;
        }

        private static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (string.IsNullOrEmpty(folderPath)) return null;

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + "." + DynamicLinkLibraryExtension);

            if (!File.Exists(assemblyPath)) return null;

            return Assembly.LoadFrom(assemblyPath);
        }

        public object Initialize(string assemblyPath)
        {
            var assembly = Assembly.LoadFile(assemblyPath);

            viewModelRepo = new ViewModelRepository(assembly);

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