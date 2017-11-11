using System.Dynamic;
using Xunit;

namespace Electron.Edge.Mvvm.Tests
{
    public class BinderTests
    {
        // TODO Add test assembly for tests to try load
        private const string TestExampleAssemblyPath = "";
        
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