using System.Collections.Generic;
using GameApp.Graphics.Textures;
using GameApp.Graphics.Utility;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Utility;
using Log = GameApp.Logging.Log;

// ReSharper disable CollectionNeverUpdated.Local
// ReSharper disable NotAccessedField.Local
#pragma warning disable 169
#pragma warning disable 649

namespace GameApp.Graphics {
    internal class Shader : IShader {
        private readonly string vertexShaderSource;
        private readonly string fragmentShaderSource;
        //internal string geometryShaderSource;

        private readonly int vertexShaderHandle;
        private readonly int fragmentShaderHandle;
        //private int geometryShaderHandle;
        private readonly int programHandle;

        private readonly Dictionary<string, ShaderUniform> uniforms;
        private readonly Dictionary<string, ShaderVertexAttribute> attributes;

        private readonly int stride;

        internal Shader(string vertexShaderSource, string fragmentShaderSource, int vertexShaderHandle, int fragmentShaderHandle, int programHandle, Dictionary<string, ShaderUniform> uniforms, Dictionary<string, ShaderVertexAttribute> attributes, int stride) {
            this.vertexShaderSource = vertexShaderSource;
            this.fragmentShaderSource = fragmentShaderSource;
            this.vertexShaderHandle = vertexShaderHandle;
            this.fragmentShaderHandle = fragmentShaderHandle;
            this.programHandle = programHandle;
            this.uniforms = uniforms;
            this.attributes = attributes;
            this.stride = stride;
        }

        //public Shader(string vertexShaderSourceCode, string fragmentShaderSourceCode/*, string geometryShaderSourceCode = null*/) {
        //    vertexShaderSource = vertexShaderSourceCode;
        //    fragmentShaderSource = fragmentShaderSourceCode;
        //    //this.geometryShaderSource = geometryShaderSourceCode;

        //    GLHandler.InitializeShader(this);
        //}

        ~Shader() {
            GraphicsHandler.Instance.DisposeShader(this);
        }

        internal void Bind() {
            if (!IsCompiled) {
                Log.Instance.WriteLine("Cannot bind shader, shader is not compiled.", LogType.Warning);
                return;
            }

            GLHandler.Instance.BindShader(this);
        }

        internal void Release() {
            if (!IsCompiled)
                return;

            GLHandler.Instance.ReleaseShader(this);
        }

        internal void EnableVertexAttributes() {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot bind shader vertex attributes, shader is not bound.", LogType.Warning);
                return;
            }

            foreach (ShaderVertexAttribute sva in this.attributes.Values) {
                sva.Enable();
            }
        }

        internal void DisableVertexAttributes() {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot release shader vertex attributes, shader is not bound.", LogType.Warning);
                return;
            }

            foreach (ShaderVertexAttribute sva in this.attributes.Values) {
                sva.Disable();
            }
        }

        internal void AssignVertexAttributePointers() {
            //if (!IsBound) {
            //    Log.Instance.WriteLine("Cannot assign shader vertex attributes pointers, shader is not bound.", LogType.Warning);
            //    return;
            //}

            foreach (ShaderVertexAttribute sva in this.attributes.Values) {
                GLHandler.Instance.SetVertexAttributePointer(sva, this.stride);
            }
        }

        #region Uniforms
        //internal void SetUniform(string uniformName, int value) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value);
        //}

        //internal void SetUniform(int uniformLocation, int value) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value);
        //}

        //public void SetUniform(string uniformName, int value1, int value2) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value1, value2);
        //}

        //public void SetUniform(int uniformLocation, int value1, int value2) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value1, value2);
        //}

        //public void SetUniform(string uniformName, int value1, int value2, int value3) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value1, value2, value3);
        //}

        //public void SetUniform(int uniformLocation, int value1, int value2, int value3) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value1, value2,value3);
        //}

        //public void SetUniform(string uniformName, int value1, int value2, int value3, int value4) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value1, value2, value3, value4);
        //}

        //public void SetUniform(int uniformLocation, int value1, int value2, int value3, int value4) {
        //    if (!IsBound) {
        //        Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value1, value2, value3, value4);
        //}

        internal void SetUniform(string uniformName, float value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, float value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value);
        }

        internal void SetUniform(string uniformName, float value1, float value2) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value1, value2);
        }

        internal void SetUniform(int uniformLocation, float value1, float value2) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value1, value2);
        }

        internal void SetUniform(string uniformName, float value1, float value2, float value3) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value1, value2, value3);
        }

        internal void SetUniform(int uniformLocation, float value1, float value2, float value3) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value1, value2, value3);
        }

        internal void SetUniform(string uniformName, float value1, float value2, float value3, float value4) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value1, value2, value3, value4);
        }

        internal void SetUniform(int uniformLocation, float value1, float value2, float value3, float value4) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value1, value2, value3, value4);
        }

        internal void SetUniform(string uniformName, ITexture value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, (Texture)value);
        }

        internal void SetUniform(int uniformLocation, Texture value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            if (!value.IsBound) {
                Log.Instance.WriteLine("Cannot assign texture shader uniform. Texture is not assigned to a texture unit.", LogType.Error);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, GLHandler.Instance.AssignedTextureUnit(value));
        }

        internal void SetUniform(string uniformName, Vector2 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Vector2 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value.x, value.y);
        }

        internal void SetUniform(string uniformName, Vector3 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Vector3 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value.x, value.y, value.z);
        }

        internal void SetUniform(string uniformName, Vector4 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Vector4 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value.x, value.y, value.z, value.w);
        }

        internal void SetUniform(string uniformName, Color value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Color value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value.r, value.g, value.b, value.a);
        }

        internal void SetUniform(string uniformName, Matrix2 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Matrix2 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value);
        }

        internal void SetUniform(string uniformName, Matrix3 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Matrix3 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value);
        }

        internal void SetUniform(string uniformName, Matrix4 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.Instance.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        internal void SetUniform(int uniformLocation, Matrix4 value) {
            if (!IsBound) {
                Log.Instance.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.Instance.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.Instance.SetShaderUniform(uniformLocation, value);
        }
        #endregion

        //public void setVertexAttribute(String attributeName, VertexAttribute attribute, int vertexByteSize, int offset) {   // FIXME (re)move
        //    setVertexAttribute(getAttributeLocation(attributeName), attribute, vertexByteSize, offset);
        //}
        // TOOD
        //public void setVertexAttribute(int attributeLocation, VertexAttribute attribute, int vertexByteSize, int offset) {  // FIXME (re)move
        //    if (attributeLocation < 0 || attribute == null || !isBound())
        //        return;

        //    glVertexAttribPointer(attributeLocation, attribute.numComponents, GL_DATA_TYPE.FLOAT, false, vertexByteSize, offset);
        //}

        public IEnumerable<ShaderUniform> Uniforms => this.uniforms.Values;

        IEnumerable<IUniform> IShader.Uniforms => Uniforms;

        public IEnumerable<ShaderVertexAttribute> Attributes => this.attributes.Values;

        IEnumerable<VertexAttribute> IShader.Attributes => Attributes;

        internal bool IsBound => IsCompiled && GLHandler.Instance.IsShaderBound(this);

        //public bool HasGeometryShader {
        //    get => this.geometryShaderSource != null && this.geometryShaderHandle > 0;
        //}

        public bool HasUniform(string uniformName) {
            return this.uniforms.ContainsKey(uniformName);
        }

        public bool HasAttribute(string attributeName) {
            return this.attributes.ContainsKey(attributeName);
        }

        internal int GetUniformLocation(string uniformName) {
            if (this.uniforms.TryGetValue(uniformName, out ShaderUniform u))
                return u.UniformIndex;

            return -1;
        }

        internal int GetAttributeLocation(string attributeName) {
            if (this.attributes.TryGetValue(attributeName, out ShaderVertexAttribute u))
                return u.AttributeIndex;

            return -1;
        }

        public bool IsCompiled => this.programHandle > 0;

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            Shader other = obj as Shader;
            return this.programHandle == other.programHandle && IsCompiled == other.IsCompiled;
        }

        public override int GetHashCode() {
            return 31 * this.programHandle.GetHashCode() + IsCompiled.GetHashCode();
        }
    }
}