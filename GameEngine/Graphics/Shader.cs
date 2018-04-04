using System;
using System.Collections.Generic;
using GameEngine.Graphics.Textures;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Utility;

namespace GameEngine.Graphics {
    public class Shader : IDisposable {
        public enum UniformType {
            Float,
            FloatVector2,
            FloatVector3,
            FloatVector4,
            //Int,
            Texture2D,
            Matrix2x2,
            Matrix3x3,
            Matrix4x4
        }

        private Dictionary<string, ShaderUniform> uniforms;
        private Dictionary<string, ShaderVertexAttribute> attributes;

        private int stride;

        private int vertexShaderHandle;
        private int fragmentShaderHandle;
        //private int geometryShaderHandle;
        private int programHandle;

        private readonly string vertexShaderSource;
        private readonly string fragmentShaderSource;
        //internal string geometryShaderSource;

        public bool IsCompiled { get; private set; }

        public Shader(string vertexShaderSourceCode, string fragmentShaderSourceCode/*, string geometryShaderSourceCode = null*/) {
            this.vertexShaderSource = vertexShaderSourceCode;
            this.fragmentShaderSource = fragmentShaderSourceCode;
            //this.geometryShaderSource = geometryShaderSourceCode;

            CompileShader();
        }

        ~Shader() {
            Dispose();
        }

        public void Dispose() {
            if (IsBound)
                Release();

            if (IsCompiled) {
                GLHandler.DeleteShader(this);
                IsCompiled = true;
            }
        }

        private void CompileShader() {
            bool succsess = CreateShader();

            if (succsess)
                succsess = RetrieveUniforms();
            if (succsess)
                succsess = RetrieveAttributes();
            if (succsess)
                CalculateStride();

            IsCompiled = succsess;
        }

        public void Bind() {
            if (!IsCompiled)
                CompileShader();

            GLHandler.BindShader(this);
        }

        public void Release() {
            if (!IsCompiled)
                return;

            GLHandler.ReleaseShader(this);
        }

        public void EnableVertexAttributes() {
            if (!IsBound) {
                Log.WriteLine("Cannot bind shader vertex attributes, shader is not bound.", LogType.Warning);
                return;
            }

            foreach (ShaderVertexAttribute sva in this.attributes.Values) {
                sva.Enable();
            }
        }

        public void DisableVertexAttributes() {
            if (!IsBound) {
                Log.WriteLine("Cannot release shader vertex attributes, shader is not bound.", LogType.Warning);
                return;
            }

            foreach (ShaderVertexAttribute sva in this.attributes.Values) {
                sva.Disable();
            }
        }

        public void AssignVertexAttributePointers() {
            if (!IsBound) {
                Log.WriteLine("Cannot assign shader vertex attributes pointers, shader is not bound.", LogType.Warning);
                return;
            }

            foreach (ShaderVertexAttribute sva in this.attributes.Values) {
                GLHandler.SetVertexAttributePointer(sva, this.stride);
            }
        }

        #region Uniforms
        public void SetUniform(string uniformName, int value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, int value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value);
        }

        //public void SetUniform(string uniformName, int value1, int value2) {
        //    if (!IsBound) {
        //        Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value1, value2);
        //}

        //public void SetUniform(int uniformLocation, int value1, int value2) {
        //    if (!IsBound) {
        //        Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value1, value2);
        //}

        //public void SetUniform(string uniformName, int value1, int value2, int value3) {
        //    if (!IsBound) {
        //        Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value1, value2, value3);
        //}

        //public void SetUniform(int uniformLocation, int value1, int value2, int value3) {
        //    if (!IsBound) {
        //        Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value1, value2,value3);
        //}

        //public void SetUniform(string uniformName, int value1, int value2, int value3, int value4) {
        //    if (!IsBound) {
        //        Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    int location;
        //    if ((location = GetUniformLocation(uniformName)) == -1) {
        //        Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
        //        return;
        //    }

        //    SetUniform(location, value1, value2, value3, value4);
        //}

        //public void SetUniform(int uniformLocation, int value1, int value2, int value3, int value4) {
        //    if (!IsBound) {
        //        Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
        //        return;
        //    }

        //    if (uniformLocation == -1) {
        //        Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
        //        return;
        //    }

        //    GLHandler.SetShaderUniform(uniformLocation, value1, value2, value3, value4);
        //}

        public void SetUniform(string uniformName, float value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, float value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value);
        }

        public void SetUniform(string uniformName, float value1, float value2) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value1, value2);
        }

        public void SetUniform(int uniformLocation, float value1, float value2) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value1, value2);
        }

        public void SetUniform(string uniformName, float value1, float value2, float value3) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value1, value2, value3);
        }

        public void SetUniform(int uniformLocation, float value1, float value2, float value3) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value1, value2, value3);
        }

        public void SetUniform(string uniformName, float value1, float value2, float value3, float value4) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value1, value2, value3, value4);
        }

        public void SetUniform(int uniformLocation, float value1, float value2, float value3, float value4) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value1, value2, value3, value4);
        }

        public void SetUniform(string uniformName, Texture value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Texture value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value.TextureID);
        }

        public void SetUniform(string uniformName, Vector2 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Vector2 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value.x, value.y);
        }

        public void SetUniform(string uniformName, Vector3 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Vector3 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value.x, value.y, value.z);
        }

        public void SetUniform(string uniformName, Vector4 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Vector4 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value.x, value.y, value.z, value.w);
        }

        public void SetUniform(string uniformName, Color value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Color value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value.r, value.g, value.b, value.a);
        }

        public void SetUniform(string uniformName, Matrix2 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Matrix2 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value);
        }

        public void SetUniform(string uniformName, Matrix3 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Matrix3 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value);
        }

        public void SetUniform(string uniformName, Matrix4 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            int location;
            if ((location = GetUniformLocation(uniformName)) == -1) {
                Log.WriteLine($"Could not find uniform of name '{uniformName}' location.", LogType.Warning);
                return;
            }

            SetUniform(location, value);
        }

        public void SetUniform(int uniformLocation, Matrix4 value) {
            if (!IsBound) {
                Log.WriteLine("Cannot set shader uniform, shader is not bound.", LogType.Warning);
                return;
            }

            if (uniformLocation == -1) {
                Log.WriteLine($"Could not find uniform location {uniformLocation}.", LogType.Warning);
                return;
            }

            GLHandler.SetShaderUniform(uniformLocation, value);
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

        public IEnumerable<ShaderVertexAttribute> Attributes => this.attributes.Values;

        public bool IsBound => IsCompiled && GLHandler.IsShaderBound(this);

        //public bool HasGeometryShader {
        //    get => this.geometryShaderSource != null && this.geometryShaderHandle > 0;
        //}

        public bool HasUniform(string uniformName) {
            return this.uniforms.ContainsKey(uniformName);
        }

        public bool HasAttribute(string attributeName) {
            return this.attributes.ContainsKey(attributeName);
        }

        public int GetUniformLocation(string uniformName) {
            if (this.uniforms.TryGetValue(uniformName, out ShaderUniform u))
                return u.UniformIndex;

            return -1;
        }

        public int GetAttributeLocation(string attributeName) {
            if (this.attributes.TryGetValue(attributeName, out ShaderVertexAttribute u))
                return u.AttributeIndex;

            return -1;
        }

        private bool CreateShader() {
            GLHandler.CreateShader(this.vertexShaderSource, this.fragmentShaderSource, /*this.geometryShaderSource, */out int ph, out int vsh, out int fsh/*, out gsh*//*, gsh*/);

            if (vsh == 0 || fsh == 0/*  || (this.geometryShaderSource != null&& gsh == 0)*/) {
                Log.WriteLine("Could not compile shader.", LogType.Error);
                return false;
            }

            this.vertexShaderHandle = vsh;
            this.fragmentShaderHandle = fsh;
            //this.geometryShaderHandle = gsh;

            if (ph == 0) {
                Log.WriteLine("Could not link shader.", LogType.Error);
                return false;
            }

            this.programHandle = ph;

            return true;
        }

        private void CalculateStride() {
            this.stride = 0;
            foreach (ShaderVertexAttribute sva in attributes.Values) {
                stride += sva.ComponentCount;
            }

            this.stride *= sizeof(float);
        }

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

        private bool RetrieveUniforms() {
            this.uniforms = GLHandler.RetrieveShaderUniforms(this, out bool valid);
            return valid;
        }

        private bool RetrieveAttributes() {
            this.attributes = GLHandler.RetrieveShaderAttributes(this, out bool valid);
            return valid;
        }

        public sealed class ShaderVertexAttribute : VertexAttribute {
            public readonly int AttributeIndex;
            public int ByteOffset;

            public ShaderVertexAttribute(string name, int attributeIndex, int componentCount)
                : base(name, componentCount) {

                AttributeIndex = attributeIndex;
                ByteOffset = -1;
            }

            public void Enable() {
                GLHandler.EnableVertexAttributeArray(AttributeIndex);
            }

            public void Disable() {
                GLHandler.DisableVertexAttributeArray(AttributeIndex);
            }
        }

        public sealed class ShaderUniform {
            public readonly string Name;
            public readonly UniformType Type;
            public readonly int UniformIndex;
            public readonly int ComponentCount;

            public ShaderUniform(string name, UniformType type, int uniformIndex, int componentCount) {
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Type = type;
                UniformIndex = uniformIndex;
                ComponentCount = componentCount;
            }

            public override bool Equals(object obj) {
                if (obj == null || GetType() != obj.GetType()) {
                    return false;
                }

                ShaderUniform other = obj as ShaderUniform;

                return ComponentCount == other.ComponentCount && Name.Equals(other.Name);
            }

            public override int GetHashCode() {
                return 31 * Name.GetHashCode() + ComponentCount.GetHashCode();
            }
        }
    }
}