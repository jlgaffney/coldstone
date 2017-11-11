using System.Dynamic;
using Xunit;

namespace Electron.Edge.Mvvm.Tests
{
    public class BinderTests
    {
        private const string TestExampleAssemblyPath = "C:\\Users\\jg\\dev\\src\\crypto\\neo\\personal_repos\\neo-gui-x\\electron\\lib\\Neo.UI.ViewModels.dll";
        
        [Fact(DisplayName = "Initialize_Successful")]
        [Trait("Binder", "Initialization")]
        public async void InitializeSuccessfulTest()
        {
            var binder = new Binder();

            dynamic obj = new ExpandoObject();
            obj.path = TestExampleAssemblyPath;

            var result = await binder.Initialize(obj) as Result;

            Assert.NotNull(result);
            Assert.True(result.ok);
        }

        [Fact(DisplayName = "Initialize_Unsuccessful_Invalid_Path")]
        [Trait("Binder", "Initialization")]
        public async void InitializeUnsuccessfulInvalidPathTest()
        {
            var binder = new Binder();

            dynamic obj = new ExpandoObject();
            obj.path = string.Empty;

            var result = await binder.Initialize(obj) as Result;

            Assert.NotNull(result);
            Assert.False(result.ok);
            Assert.True(result.result is string);
            Assert.False(string.IsNullOrEmpty((string)result.result));
        }
    }
}