using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Logging;

namespace GameEngine.Resources {
    public interface IResourceLoadingParameters {
    }

    public abstract class ResourceLoadingParameters<T> : IResourceLoadingParameters {
        public readonly IEnumerable<string> FilePaths;

        protected ResourceLoadingParameters(IEnumerable<string> filePaths) {
            if (filePaths == null || !filePaths.Any() || filePaths.Any(string.IsNullOrEmpty)) {
                Log.WriteLine("Invalid resource file paths.", LogType.Error);
                return;
            }

            FilePaths = filePaths;
        }
    }
}