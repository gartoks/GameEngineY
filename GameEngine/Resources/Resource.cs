using System.Collections.Generic;
using GameEngine.Logging;

namespace GameEngine.Resources {
    public interface IResource { }

    public sealed class Resource<T> : IResource {

        public readonly string ID;

        public readonly IEnumerable<string> FilePaths;

        public T Data { get; }

        internal Resource(string id, IEnumerable<string> filePaths, T data) {
            if (string.IsNullOrWhiteSpace(id)) {
                Log.WriteLine("A resource identifier cannot be null.", LogType.Error);
                return;
            }

            ID = id;
            FilePaths = filePaths;
            this.Data = data;
        }
    }
}