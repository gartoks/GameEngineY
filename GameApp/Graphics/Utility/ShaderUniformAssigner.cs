using System.Collections.Generic;
using GameApp.Graphics.Textures;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Utility;

namespace GameApp.Graphics.Utility {
    internal class ShaderUniformAssigner : IShaderUniformAssigner {
        private Shader shader;
        private IUniform currentlyToBeAssignedUniform;

        private readonly List<Texture> textures = new List<Texture>();

        internal void AssignUniforms(Shader shader, ShaderUniformAssignmentHandler assignmentHandler) {
            this.textures.Clear();
            this.shader = shader;

            foreach (ShaderUniform uniform in this.shader.Uniforms) {
                this.currentlyToBeAssignedUniform = uniform;
                assignmentHandler(this, uniform);
            }

            this.currentlyToBeAssignedUniform = null;
            this.shader = null;
        }

        public void SetUniform(string uniformName, Color value) {
            SetUniform(uniformName, value.r, value.g, value.b, value.a);
        }

        public void SetUniform(string uniformName, float value) {
            if (currentlyToBeAssignedUniform.Type != UniformType.Float || currentlyToBeAssignedUniform.ComponentCount != 1) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value);
        }

        public void SetUniform(string uniformName, float value1, float value2) {
            if (currentlyToBeAssignedUniform.Type != UniformType.FloatVector2 || currentlyToBeAssignedUniform.ComponentCount != 2) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value1, value2);
        }

        public void SetUniform(string uniformName, float value1, float value2, float value3) {
            if (currentlyToBeAssignedUniform.Type != UniformType.FloatVector3 || currentlyToBeAssignedUniform.ComponentCount != 4) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value1, value2, value3);
        }

        public void SetUniform(string uniformName, float value1, float value2, float value3, float value4) {
            if (currentlyToBeAssignedUniform.Type != UniformType.FloatVector4 || currentlyToBeAssignedUniform.ComponentCount != 4) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value1, value2, value3, value4);
        }

        public void SetUniform(string uniformName, Matrix2 value) {
            if (currentlyToBeAssignedUniform.Type != UniformType.Matrix2x2 || currentlyToBeAssignedUniform.ComponentCount != 1) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value);
        }

        public void SetUniform(string uniformName, Matrix3 value) {
            if (currentlyToBeAssignedUniform.Type != UniformType.Matrix3x3 || currentlyToBeAssignedUniform.ComponentCount != 1) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value);
        }

        public void SetUniform(string uniformName, Matrix4 value) {
            if (currentlyToBeAssignedUniform.Type != UniformType.Matrix4x4 || currentlyToBeAssignedUniform.ComponentCount != 1) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            shader.SetUniform(uniformName, value);
        }

        internal void SetUniform(string uniformName, Texture value) {
            if (currentlyToBeAssignedUniform.Type != UniformType.Texture2D || currentlyToBeAssignedUniform.ComponentCount != 1) {
                Logging.Log.Instance.WriteLine($"Cannot assign uniform '{uniformName}' to uniform '{currentlyToBeAssignedUniform.Name}'. Invalid type or component count.", LogType.Error);
                return;
            }

            this.textures.Add(value);
            value.Bind();
            shader.SetUniform(uniformName, value);
        }

        void IShaderUniformAssigner.SetUniform(string uniformName, ITexture texture) => SetUniform(uniformName, (Texture)texture);

        public void SetUniform(string uniformName, Vector2 value) {
            SetUniform(uniformName, value.x, value.y);
        }

        public void SetUniform(string uniformName, Vector3 value) {
            SetUniform(uniformName, value.x, value.y, value.z);
        }

        public void SetUniform(string uniformName, Vector4 value) {
            SetUniform(uniformName, value.x, value.y, value.z, value.w);
        }

        public IEnumerable<Texture> TextureUniforms => this.textures;
    }
}