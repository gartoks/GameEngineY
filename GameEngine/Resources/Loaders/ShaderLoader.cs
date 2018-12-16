using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine.Graphics;
using GameEngine.Logging;

namespace GameEngine.Resources.Loaders {
    public class ShaderLoadingParameters : ResourceLoadingParameters<IShader> {
        public ShaderLoadingParameters(IEnumerable<string> filePaths)
            : base(filePaths) {

            if (filePaths.Count() != 2) {
                Log.WriteLine("A shader resource must have exactly two file.", LogType.Error);
                return;
            }
        }
    }

    public class ShaderLoader : ResourceLoader<IShader, ShaderLoadingParameters> {
        public override IShader Load(IEnumerable<string> filePaths, ShaderLoadingParameters loadingParameters) {
            string vertexShaderFile = filePaths.ElementAt(0);
            string fragmentShaderFile = filePaths.ElementAt(1);

            if (!File.Exists(vertexShaderFile)) {
                Log.WriteLine($"Vertex shader file does not exist. {vertexShaderFile}", LogType.Error);
                return null;
            }

            if (!File.Exists(fragmentShaderFile)) {
                Log.WriteLine($"Fragment shader file does not exist. {fragmentShaderFile}", LogType.Error);
                return null;
            }

            string vertexShaderSource = File.ReadAllText(vertexShaderFile);
            string fragmentShaderSource = File.ReadAllText(fragmentShaderFile);

            return GraphicsHandler.CreateShader(vertexShaderSource, fragmentShaderSource);
        }
    }
}