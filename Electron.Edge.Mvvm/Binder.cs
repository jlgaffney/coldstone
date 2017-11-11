using System;
using System.Threading.Tasks;

namespace Electron.Edge.Mvvm
{
    public class Binder
    {
        private static BinderCore binder;

        public Task<object> Initialize(dynamic obj)
        {
            binder = new BinderCore();

            return SyncTask(() => binder.Initialize(obj.path));
        }

        public Task<object> CreateViewModel(dynamic obj)
        {
            return SyncTask(() => binder.CreateViewModel(obj.name));
        }

        public Task<object> GetPropertyValue(dynamic obj)
        {
            return AsyncTask(() => binder.GetPropertyValue(obj.id, obj.property));
        }

        public Task<object> SetPropertyValue(dynamic obj)
        {
            return AsyncTask(() => binder.SetPropertyValue(obj.id, obj.property, obj.value));
        }

        public Task<object> GetPropertyAsViewModel(dynamic obj)
        {
            return SyncTask(() => binder.GetPropertyAsViewModel(obj.id, obj.property));
        }

        public Task<object> BindToProperty(dynamic obj)
        {
            return SyncTask(() => binder.BindToProperty(obj.id, obj.property, obj.onChanged));
        }

        public Task<object> ExecuteCommand(dynamic obj)
        {
            return AsyncTask(() => binder.ExecuteCommand(obj.id, obj.command));
        }

        private static Task<object> SyncTask(Func<object> function)
        {
            var task = AsyncTask(function);
            task.Wait();
            return task;
        }

        private static Task<object> AsyncTask(Func<object> function)
        {
            return Task.Factory.StartNew(() =>
            {
                if (function == null) return Result.NotOk("Function cannot be null!");

                try
                {
                    var result = function();
                    return Result.Ok(result);
                }
                catch (Exception ex)
                {
                    return Result.NotOk(ex.ToString());
                }
            });
        }
    }
}