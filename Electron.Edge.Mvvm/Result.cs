namespace Electron.Edge.Mvvm
{
    public class Result
    {
        // These properties can be accessed from JavaScript to check result
        public bool ok { get; set; }
        public object result { get; set; }

        public static object Ok(object result)
        {
            return new Result
            {
                ok = true,
                result = result
            };
        }

        public static object NotOk(object result)
        {
            return new Result
            {
                ok = false,
                result = result
            };
        }
    }
}