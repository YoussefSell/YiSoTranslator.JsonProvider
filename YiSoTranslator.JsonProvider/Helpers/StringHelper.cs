namespace YiSoTranslator.JsonProvider
{
    [System.Diagnostics.DebuggerStepThrough]
    internal static class StringHelper
    {
        /// <summary>
        /// check if the string is Null or Empty or WhiteSpace
        /// </summary>
        /// <param name="content"></param>
        /// <returns>true if null or empty or WhiteSpace, false if not</returns>
        public static bool IsNull(this string content)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
                return true;

            return false;
        }
    }
}
