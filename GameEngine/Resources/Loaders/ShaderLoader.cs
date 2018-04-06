using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine.Graphics;
using GameEngine.Logging;

namespace GameEngine.Resources.Loaders {
    public class ShaderLoaderParameters : ResourceLoadingParameters<Shader> {
        public ShaderLoaderParameters(IEnumerable<string> filePaths)
            : base(filePaths) {

            if (filePaths.Count() != 2) {
                Log.WriteLine("A shader resource must have exactly two file.", LogType.Error);
                return;
            }
        }
    }

    public class ShaderLoader : ResourceLoader<Shader, ShaderLoaderParameters> {
        public override Shader Load(IEnumerable<string> filePaths, ShaderLoaderParameters loadingParameters) {
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

            return new Shader(vertexShaderSource, fragmentShaderSource);
        }
    }
}