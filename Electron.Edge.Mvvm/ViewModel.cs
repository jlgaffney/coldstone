using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Electron.Edge.Mvvm
{
    public class ViewModel
    {
        private readonly object instance;

        private readonly Type instanceType;

        private bool capturePropertyChangedEvents;

        private readonly Dictionary<string, Func<object, Task<object>>> bindings;

        public ViewModel(object instance)
        {
            this.instance = instance;
            this.instanceType = instance.GetType();
            this.capturePropertyChangedEvents = false;
            this.bindings = new Dictionary<string, Func<object, Task<object>>>();
        }

        public string Id => this.GetHashCode().ToString();

        public object GetPropertyValue(string propertyName)
        {
            var propInfo = instanceType.GetProperty(propertyName);

            return propInfo?.GetValue(instance);
        }

        public void SetPropertyValue(string propertyName, string value)
        {
            var propInfo = instanceType.GetProperty(propertyName);

            if (propInfo == null) return;

            object convertedValue = value;

            if (propInfo.PropertyType == typeof(int))
            {
                // Try parse string to integer
                if (!int.TryParse(value, out var val))
                {
                    val = 0;
                }
                convertedValue = val;
            }

            propInfo.SetValue(instance, convertedValue);
        }

        public void ExecuteCommand(string commandName)
        {
            var propInfo = instanceType.GetProperty(commandName);

            if (propInfo == null) return;

            var command = propInfo.GetValue(instance);
            var commandType = command.GetType();
            var methodInfo = commandType.GetMethod("Execute");
            methodInfo?.Invoke(command, new object[] { null });
        }

        public ViewModel GetPropertyAsViewModel(string propertyName)
        {
            var propInfo = instanceType.GetProperty(propertyName);

            return propInfo == null ? null : new ViewModel(propInfo.GetValue(instance));
        }

        public void Bind(string propertName, Func<object, Task<object>> callback)
        {
            if (!capturePropertyChangedEvents)
            {
                var notify = instance as INotifyPropertyChanged;

                if (notify == null) return;

                notify.PropertyChanged += NotifyPropertyChanged;
                capturePropertyChangedEvents = true;
            }

            bindings[propertName] = callback;
        }

        private async void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!bindings.ContainsKey(e.PropertyName)) return;

            var propInfo = instanceType.GetProperty(e.PropertyName);

            if (propInfo == null) return;

            var callback = bindings[e.PropertyName];

            await callback(propInfo.GetValue(instance).ToString());                       
        }
    }
}