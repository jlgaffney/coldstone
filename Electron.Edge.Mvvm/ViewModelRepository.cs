using System;
using System.Collections.Generic;
using System.Reflection;

namespace Electron.Edge.Mvvm
{
    public class ViewModelRepository
    {
        private readonly Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();

        private readonly Assembly assembly;

        public ViewModelRepository(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public ViewModel Create(string name)
        {
            var type = assembly.GetType(name);

            if (type == null)
            {
                throw new Exception("ViewModel type \"" + name + "\" is not in assembly!");
            }

            var instance = Activator.CreateInstance(type);
            var vm = new ViewModel(instance);
            viewModels[vm.GetHashCode().ToString()] = vm;
            return vm;
        }

        public bool Contains(string id)
        {
            return GetViewModel(id) != null;
        }

        public void Add(ViewModel vm)
        {
            viewModels[vm.GetHashCode().ToString()] = vm;
        }

        public ViewModel GetViewModel(string id)
        {
            if (!viewModels.ContainsKey(id)) return null;

            return viewModels[id];
        }
    }
}