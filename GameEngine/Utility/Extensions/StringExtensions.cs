namespace GameEngine.Utility.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// Replaces the parameter targets in the text with the given parameters. The parameters have the form of '<prefix>X<postfix>' where x is an integer greater or equal to zero.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="postfix">The postfix.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static string ReplaceParameters(this string text, string prefix, string postfix, params string[] parameters) {

            if (prefix == null)
                prefix = "";

            if (postfix == null)
                postfix = "";

            for (int i = 0; i < parameters.Length; i++) {
                text = text.Replace($"{prefix}{i}{postfix}", parameters[i]);
            }

            return text;
        }

        public static string PadBoth(this string str, int totalLength) {
            int spaces = totalLength - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft).PadRight(totalLength);
        }
    }
}