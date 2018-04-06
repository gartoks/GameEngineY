using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameEngine.Logging;

namespace GameEngine.Resources.Loaders {
    public class TextLoadingParameters : ResourceLoadingParameters<string> {

        public readonly Encoding Encoding;

        public TextLoadingParameters(IEnumerable<string> filePaths, Encoding encoding)
            : base(filePaths) {

            if (filePaths.Count() != 1) {
                Log.WriteLine("A text resource must have exactly one file.", LogType.Error);
                return;
            }

            if (encoding == null) {
                Log.WriteLine("String encoding for a text resource cannot be null.", LogType.Error);
                return;
            }

            Encoding = encoding;
        }
    }

    public class TextLoader : ResourceLoader<string, TextLoadingParameters> {
        public override string Load(IEnumerable<string> filePaths, TextLoadingParameters loadingParameters) {
            string text;
            try {
                text = File.ReadAllText(filePaths.Single(), loadingParameters.Encoding);
            } catch (Exception) {
                return null;
            }

            return text;
        }
    }
}