namespace Electron.Edge.Mvvm
{
    public class EdgeResult
    {
        // These properties can be accessed from JavaScript to check result
        public bool ok { get; set; }
        public object result { get; set; }

        public static object Ok(object result)
        {
            return new EdgeResult
            {
                ok = true,
                result = result
            };
        }

        public static object NotOk(object result)
        {
            return new EdgeResult
            {
                ok = false,
                result = result
            };
        }
    }
}