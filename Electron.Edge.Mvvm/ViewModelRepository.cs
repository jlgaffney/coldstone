using System;
using System.Collections.Generic;
using System.Reflection;

namespace Electron.Edge.Mvvm
{
    public class ViewModelRepository
    {
        private static readonly Dictionary<string, ViewModel> ViewModels = new Dictionary<string, ViewModel>();

        private readonly Assembly assembly;

        public ViewModelRepository(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public ViewModel Create(string name)
        {
            var type = assembly.GetType(name);
            var instance = Activator.CreateInstance(type);
            var vm = new ViewModel(instance);
            ViewModels[vm.GetHashCode().ToString()] = vm;
            return vm;
        }

        public bool Contains(string id)
        {
            return GetViewModel(id) != null;
        }

        public void Add(ViewModel vm)
        {
            ViewModels[vm.GetHashCode().ToString()] = vm;
        }

        public ViewModel GetViewModel(string id)
        {
            if (!ViewModels.ContainsKey(id)) return null;

            return ViewModels[id];
        }
    }
}
