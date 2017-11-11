using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Electron.Edge.Mvvm
{
    public class BinderCore
    {
        private ViewModelRepository viewModelRepo;

        public object Initialize(string viewModelAssemblyPath)
        {
            Assembly viewModelAssembly;
            try
            {
                viewModelAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(viewModelAssemblyPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Initialization failed! ViewModel assembly could not be loaded at " + viewModelAssemblyPath + "!", ex);
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