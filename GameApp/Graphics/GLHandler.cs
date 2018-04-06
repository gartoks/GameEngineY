using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GameApp.Graphics.Utility;
using GameEngine.Exceptions;
using GameEngine.Game.GameObjects;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Textures;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Utility.DataStructures;
using OpenTK.Graphics.OpenGL;
using Color = GameEngine.Utility.Color;
using Log = GameApp.Logging.Log;

namespace GameApp.Graphics {
    internal class GLHandler : IGLHandler {   // TODO stencil

        private static GLHandler instance;
        internal static GLHandler Instance {
            get => GLHandler.instance;
            private set { if (GLHandler.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private Color clearColor;
        private ClearBufferMask clearBufferMask;

        public bool IsRendering { get; private set; }
        //private Renderer renderer;

        private readonly Stack<Matrix4> transformStack;
        private readonly ObjectPool<Matrix4> matrixPool;


        public int ActiveTextureUnit { get; private set; }
        private Texture[] boundTextures;
        private int availableTextureUnits;

        private Shader boundShader;

        private Renderable boundVertexArrayObject;
        private VertexBufferObject boundVertexBufferObject;
        private IndexBufferObject boundIndexBufferObject;

        private int supportedTextureUnits;

        private (BlendFunction source, BlendFunction destination)? blendFunctions;
        private GameEngine.Graphics.RenderSettings.DepthFunction? depthFunction;
        private AntiAliasMode antiAliasMode;

        public GLHandler() {
            Instance = this;

            this.transformStack = new Stack<Matrix4>();
            this.matrixPool = new ObjectPool<Matrix4>(Matrix4.CreateIdentity, m => { m.MakeIdentity(); return m; });

            IsRendering = false;
            //this.renderer = null;

            ActiveTextureUnit = -1;
            boundTextures = null;
            availableTextureUnits = -1;
            supportedTextureUnits = -1;
        }

        internal void Install() { }

        internal bool VerifyInstallation() => true;

        internal void Initialize() {
            EnableTextures = true;

            EnableBlending = true;
            BlendMode = BlendMode.Default;

            EnableDepthTest = true;
            DepthFunction = GameEngine.Graphics.RenderSettings.DepthFunction.Less;

            EnableColors(true, true, true, true);

            EnableEdgeAntialiasing = true;
            AntiAliasMode = AntiAliasMode.Fastest;

            EnableCulling = true;
            ClockwiseCulling = false;
            CullFaces(false, true);

            ClearColor = Color.BLACK;
            SetClearModes(true, true, false);
        }

        internal void BeginRendering() {
            this.transformStack.Clear();

            GL.ClearDepth(1f);  // TODO maybe allow different values
            GL.Clear(this.clearBufferMask);

            //Viewport viewport = Game.Instance.Viewport;
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(viewport.ViewProjectionMatrix.ColumnMajor);

            IsRendering = true;
        }

        internal void EndRendering() {
            IsRendering = false;

            //GL.Flush();

            Window.Window.Instance.SwapBuffers();   // un-nice

            foreach (Matrix4 m in this.transformStack) {
                this.matrixPool.Put(m);
            }
        }

        public void Render(IndexBufferObject ibo) {
            BindIBO(ibo);
            GL.DrawElements(PrimitiveType.Triangles, ibo.Data.Count() / 3, DrawElementsType.UnsignedInt, 0);
            ReleaseIBO(ibo);
        }

        #region VertexArrayObjects
        public void InitializeVAO(Renderable vao) {
            FieldInfo vaoIDFI = vao.GetType().GetField("vaoID", BindingFlags.NonPublic | BindingFlags.Instance);

            int vaoID = GL.GenVertexArray();

            vaoIDFI.SetValue(vao, vaoID);
        }

        public void BindVAO(Renderable vao) {
            if (IsVAOBound(vao))
                return;

            if (vao.IsDisposed) {
                Log.Instance.WriteLine("Cannot bind vertex array object. It is disposed.", LogType.Error);
                return;
            }

            int vaoID = GetVAOID(vao);

            GL.BindVertexArray(vaoID);
            this.boundVertexArrayObject = vao;
        }

        public void ReleaseVAO(Renderable vao) {
            if (!IsVAOBound(vao))
                return;

            if (vao.IsDisposed) {
                Log.Instance.WriteLine("Cannot release vertex array object. It is disposed.", LogType.Error);
                return;
            }

            GL.BindVertexArray(0);
            this.boundVertexArrayObject = null;
        }

        public void DeleteVAO(Renderable vao) {
            if (vao.IsDisposed)
                return;

            if (vao.IsBound)
                ReleaseVAO(vao);

            FieldInfo vaoIDFI = vao.GetType().GetField("vaoID", BindingFlags.NonPublic | BindingFlags.Instance);

            int vaoID = (int)vaoIDFI.GetValue(vao);
            vaoIDFI.SetValue(vao, -1);

            GL.DeleteVertexArray(vaoID);
        }

        public bool IsVAOBound(Renderable vao) => vao != null && vao.Equals(this.boundVertexArrayObject);

        private static int GetVAOID(Renderable vao) {
            FieldInfo vaoIDFI = vao.GetType().GetField("vaoID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (int)vaoIDFI.GetValue(vao);
        }
        #endregion

        #region VertexBufferObjects
        public void InitializeVBO(VertexBufferObject vbo) {
            FieldInfo vboIDFI = vbo.GetType().GetField("vboID", BindingFlags.NonPublic | BindingFlags.Instance);

            int vboID = GL.GenBuffer();

            vboIDFI.SetValue(vbo, vboID);

            VertexBufferObject previouslyBoundVBO = this.boundVertexBufferObject;

            BindVBO(vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vbo.Size * sizeof(float), (IntPtr)null, ToBufferUsageHint(vbo.Type));

            if (previouslyBoundVBO != null)
                BindVBO(previouslyBoundVBO);
            else
                ReleaseVBO(vbo);
        }

        public void BindVBO(VertexBufferObject vbo) {
            if (IsVBOBound(vbo))
                return;

            if (vbo.IsDisposed) {
                Log.Instance.WriteLine("Cannot bind vertex buffer object. It is disposed.", LogType.Error);
                return;
            }

            int vboID = GetVBOID(vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            this.boundVertexBufferObject = vbo;
        }

        public void ReleaseVBO(VertexBufferObject vbo) {
            if (!IsVBOBound(vbo))
                return;

            if (vbo.IsDisposed) {
                Log.Instance.WriteLine("Cannot release vertex buffer object. It is disposed.", LogType.Error);
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            this.boundVertexBufferObject = null;
        }

        public void MapVBOData(VertexBufferObject vbo) {
            if (vbo.IsDisposed) {
                Log.Instance.WriteLine("Cannot release vertex buffer object. It is disposed.", LogType.Error);
                return;
            }

            VertexBufferObject previouslyBoundVBO = this.boundVertexBufferObject;

            BindVBO(vbo);

            IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
            Marshal.Copy(vbo.Data.ToArray(), 0, mapBufferPtr, vbo.Data.Count());
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);

            if (previouslyBoundVBO != null)
                BindVBO(previouslyBoundVBO);
            else
                ReleaseVBO(vbo);
        }

        public void DeleteVBO(VertexBufferObject vbo) {
            if (vbo.IsDisposed)
                return;

            if (vbo.IsBound)
                ReleaseVBO(vbo);

            FieldInfo vboIDFI = vbo.GetType().GetField("vboID", BindingFlags.NonPublic | BindingFlags.Instance);

            int vboID = (int)vboIDFI.GetValue(vbo);
            vboIDFI.SetValue(vbo, -1);

            GL.DeleteBuffer(vboID);
        }

        public bool IsVBOBound(VertexBufferObject vbo) => vbo != null && vbo.Equals(this.boundVertexBufferObject);

        private static int GetVBOID(VertexBufferObject vbo) {
            FieldInfo vboIDFI = vbo.GetType().GetField("vboID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (int)vboIDFI.GetValue(vbo);
        }
        #endregion

        #region IndexBufferObjects
        public void InitializeIBO(IndexBufferObject ibo) {
            FieldInfo iboIDFI = ibo.GetType().GetField("iboID", BindingFlags.NonPublic | BindingFlags.Instance);

            int iboID = GL.GenBuffer();

            iboIDFI.SetValue(ibo, iboID);

            IndexBufferObject previouslyBoundIBO = this.boundIndexBufferObject;

            BindIBO(ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ibo.Size * sizeof(int), (IntPtr)null, ToBufferUsageHint(ibo.Type));

            if (previouslyBoundIBO != null)
                BindIBO(previouslyBoundIBO);
            else
                ReleaseIBO(ibo);
        }

        public void BindIBO(IndexBufferObject ibo) {
            if (IsIBOBound(ibo))
                return;

            if (ibo.IsDisposed) {
                Log.Instance.WriteLine("Cannot bind index buffer object. It is disposed.", LogType.Error);
                return;
            }

            int iboID = GetIBOID(ibo);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, iboID);
            this.boundIndexBufferObject = ibo;
        }

        public void ReleaseIBO(IndexBufferObject ibo) {
            if (!IsIBOBound(ibo))
                return;

            if (ibo.IsDisposed) {
                Log.Instance.WriteLine("Cannot release index buffer object. It is disposed.", LogType.Error);
                return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            this.boundIndexBufferObject = null;
        }

        public void MapIBOData(IndexBufferObject ibo) {
            if (ibo.IsDisposed) {
                Log.Instance.WriteLine("Cannot release index buffer object. It is disposed.", LogType.Error);
                return;
            }

            IndexBufferObject previouslyBoundIBO = this.boundIndexBufferObject;

            BindIBO(ibo);

            IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
            Marshal.Copy((int[])(object)ibo.Data.ToArray(), 0, mapBufferPtr, ibo.Data.Count());
            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);

            if (previouslyBoundIBO != null)
                BindIBO(previouslyBoundIBO);
            else
                ReleaseIBO(ibo);
        }

        public void DeleteIBO(IndexBufferObject ibo) {
            if (ibo.IsDisposed)
                return;

            if (ibo.IsBound)
                ReleaseIBO(ibo);

            FieldInfo iboIDFI = ibo.GetType().GetField("iboID", BindingFlags.NonPublic | BindingFlags.Instance);

            int iboID = (int)iboIDFI.GetValue(ibo);
            iboIDFI.SetValue(ibo, -1);

            GL.DeleteBuffer(iboID);
        }

        public bool IsIBOBound(IndexBufferObject ibo) => ibo != null && ibo.Equals(this.boundIndexBufferObject);

        private static int GetIBOID(IndexBufferObject ibo) {
            FieldInfo iboIDFI = ibo.GetType().GetField("iboID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (int)iboIDFI.GetValue(ibo);
        }
        #endregion

        #region Shaders
        public void CreateShader(string vertexShaderSource, string fragmentShaderSource, /*string geometryShaderSource,*/
                                            out int programHandle, out int vertexShaderHandle, out int fragmentShaderHandle/*, out int geometryShaderHandle*/) {

            vertexShaderHandle = CompileShader(vertexShaderSource, ShaderType.VertexShader);
            fragmentShaderHandle = CompileShader(fragmentShaderSource, ShaderType.FragmentShader);
            //vertexShaderHandle = CompileShader(geometryShaderSource, ShaderType.GeometryShader);

            if (vertexShaderHandle == 0 || fragmentShaderHandle == 0) {
                vertexShaderHandle = 0;
                fragmentShaderHandle = 0;
                programHandle = 0;
                return;
            }

            programHandle = CreateShaderProrgam(vertexShaderHandle, fragmentShaderHandle);
        }

        private int CreateShaderProrgam(int vertexShaderHandle, int fragmentShaderHandle/*, int geometryShaderHandle*/) {
            int shaderProgram = GL.CreateProgram();

            if (shaderProgram == 0)
                return 0;

            GL.AttachShader(shaderProgram, vertexShaderHandle);
            GL.AttachShader(shaderProgram, fragmentShaderHandle);
            //if (geometryShaderHandle > 0)
            //    GL.AttachShader(shaderProgram, geometryShaderHandle);

            GL.LinkProgram(shaderProgram);
            GL.ValidateProgram(shaderProgram);

            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
                return 0;

            return shaderProgram;
        }

        private int CompileShader(string shaderSource, ShaderType shaderType) {
            if (shaderSource == null)
                return 0;

            int shader = GL.CreateShader(shaderType);

            if (shader == 0)
                return 0;

            GL.ShaderSource(shader, shaderSource);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
                return 0;

            return shader;
        }

        public void DeleteShader(Shader shader) {
            if (IsShaderBound(shader))
                ReleaseShader(shader);

            (int program, int vertexShader, int fragmentShader) handles = GetShaderHandles(shader);

            GL.DeleteProgram(handles.program);
            GL.DeleteShader(handles.vertexShader);
            GL.DeleteShader(handles.fragmentShader);
        }

        public void BindShader(Shader shader) {
            if (shader.Equals(this.boundShader))
                return;

            (int program, int vertexShader, int fragmentShader) handles = GetShaderHandles(shader);

            GL.UseProgram(handles.program);
            this.boundShader = shader;
        }

        public void ReleaseShader(Shader shader) {
            if (!this.boundShader.Equals(shader))
                return;
            //asdasd // TODO set to default shader ?
            GL.UseProgram(0);
            this.boundShader = null;
        }

        public void SetVertexAttributePointer(Shader.ShaderVertexAttribute vertexAttribute, /*Type type,bool normalized,  */int stride) {
            GL.VertexAttribPointer(vertexAttribute.AttributeIndex, vertexAttribute.ComponentCount, /*ToVertexAttribPointerType(type)*/ VertexAttribPointerType.Float, false, stride, vertexAttribute.ByteOffset);
        }

        //private VertexAttribPointerType ToVertexAttribPointerType(Type type) {
        //    if (type.Equals(typeof(float)))
        //        return VertexAttribPointerType.Float;

        //    if (type.Equals(typeof(int)))
        //        return VertexAttribPointerType.Int;

        //    if (type.Equals(typeof(uint)))
        //        return VertexAttribPointerType.UnsignedInt;

        //    if (type.Equals(typeof(short)))
        //        return VertexAttribPointerType.Short;

        //    if (type.Equals(typeof(ushort)))
        //        return VertexAttribPointerType.UnsignedShort;

        //    if (type.Equals(typeof(byte)))
        //        return VertexAttribPointerType.UnsignedByte;

        //    if (type.Equals(typeof(double)))
        //        return VertexAttribPointerType.Double;

        //    throw new ArgumentException();
        //}

        public void EnableVertexAttributeArray(int attributeIndex) {
            GL.EnableVertexAttribArray(attributeIndex);
        }

        public void DisableVertexAttributeArray(int attributeIndex) {
            GL.DisableVertexAttribArray(attributeIndex);
        }

        private static (int ph, int vsh, int fsh) GetShaderHandles(Shader shader) {
            FieldInfo programHandleFI = shader.GetType().GetField("programHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo vertexShaderHandleFI = shader.GetType().GetField("vertexShaderHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fragmentShaderHandleFI = shader.GetType().GetField("fragmentShaderHandle", BindingFlags.NonPublic | BindingFlags.Instance);

            int programHandle = (int)programHandleFI.GetValue(shader);
            int vertexShaderHandle = (int)programHandleFI.GetValue(shader);
            int fragmentShaderHandle = (int)programHandleFI.GetValue(shader);

            return (programHandle, vertexShaderHandle, fragmentShaderHandle);
        }

        public Dictionary<string, Shader.ShaderUniform> RetrieveShaderUniforms(Shader shader, out bool validUniforms) {
            Dictionary<string, Shader.ShaderUniform> uniforms = new Dictionary<string, Shader.ShaderUniform>();

            (int program, int vertexShader, int fragmentShader) handles = GetShaderHandles(shader);

            GL.GetProgram(handles.program, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            validUniforms = true;
            for (int i = 0; i < uniformCount; i++) {
                string uniformName = GL.GetActiveUniform(handles.program, i, out int size, out ActiveUniformType type);
                int uniformLocation = GL.GetUniformLocation(handles.program, uniformName);

                if (!IsValidShaderUniformType(type)) {
                    Log.Instance.WriteLine($"Invalid shader uniform type {type.ToString()} for uniform '{uniformName}'.", LogType.Error);
                    validUniforms = false;
                    break;
                }

                Shader.UniformType uniformType = ConvertUniformType(type);

                Shader.ShaderUniform uniform = new Shader.ShaderUniform(uniformName, uniformType, uniformLocation, size);
                uniforms.Add(uniformName, uniform);
            }

            if (!validUniforms)
                uniforms.Clear();

            return uniforms;
        }

        public Dictionary<string, Shader.ShaderVertexAttribute> RetrieveShaderAttributes(Shader shader, out bool validAttributes) {
            Dictionary<string, Shader.ShaderVertexAttribute> attributes = new Dictionary<string, Shader.ShaderVertexAttribute>();

            (int program, int vertexShader, int fragmentShader) handles = GetShaderHandles(shader);

            GL.GetProgram(handles.program, GetProgramParameterName.ActiveAttributes, out int attributeCount);

            validAttributes = true;
            for (int i = 0; i < attributeCount; i++) {
                String attributeName = GL.GetActiveAttrib(handles.program, i, out int size, out ActiveAttribType type);
                int attributeLocation = GL.GetAttribLocation(handles.program, attributeName);

                if (!IsValidShaderAttributeType(type)) {
                    Log.Instance.WriteLine($"Invalid shader attribute type {type.ToString()} for attribute '{attributeName}'.", LogType.Error);
                    validAttributes = false;
                    break;
                }

                Shader.ShaderVertexAttribute attribute = new Shader.ShaderVertexAttribute(attributeName, attributeLocation, size);
                attributes.Add(attributeName, attribute);
            }

            if (!validAttributes) {
                attributes.Clear();
                return attributes;
            }

            int currentOffset = 0;
            HashSet<Shader.ShaderVertexAttribute> nonProcessedVAs = new HashSet<Shader.ShaderVertexAttribute>(attributes.Values);
            while (nonProcessedVAs.Count > 0) {

                // get smallest index sva
                Shader.ShaderVertexAttribute currentSVA = null;
                foreach (Shader.ShaderVertexAttribute sva2 in nonProcessedVAs) {
                    if (currentSVA == null || sva2.AttributeIndex < currentSVA.AttributeIndex)
                        currentSVA = sva2;
                }

                // set offset
                currentSVA.ByteOffset = currentOffset;

                // update offset
                currentOffset += currentSVA.ComponentCount * sizeof(float);

                // remove from non processed
                nonProcessedVAs.Remove(currentSVA);
            }

            return attributes;
        }

        private Shader.UniformType ConvertUniformType(ActiveUniformType uniformType) {
            switch (uniformType) {
                //case ActiveUniformType.Int: return Shader.UniformType.Int;
                case ActiveUniformType.Float: return Shader.UniformType.Float;
                case ActiveUniformType.FloatVec2: return Shader.UniformType.FloatVector2;
                case ActiveUniformType.FloatVec3: return Shader.UniformType.FloatVector3;
                case ActiveUniformType.FloatVec4: return Shader.UniformType.FloatVector4;
                case ActiveUniformType.FloatMat2: return Shader.UniformType.Matrix2x2;
                case ActiveUniformType.FloatMat3: return Shader.UniformType.Matrix3x3;
                case ActiveUniformType.FloatMat4: return Shader.UniformType.Matrix4x4;
                case ActiveUniformType.Sampler2D: return Shader.UniformType.Texture2D;
            }

            throw new ArgumentException();
        }

        private bool IsValidShaderUniformType(ActiveUniformType uniformType) {
            return ActiveUniformType.Sampler2D.Equals(uniformType) ||
                    ActiveUniformType.Float.Equals(uniformType) ||
                    ActiveUniformType.FloatVec2.Equals(uniformType) ||
                    ActiveUniformType.FloatVec3.Equals(uniformType) ||
                    ActiveUniformType.FloatVec4.Equals(uniformType) ||
                    ActiveUniformType.FloatMat2.Equals(uniformType) ||
                    ActiveUniformType.FloatMat3.Equals(uniformType) ||
                    ActiveUniformType.FloatMat4.Equals(uniformType);
                    //ActiveUniformType.Int.Equals(uniformType);
                    //ActiveUniformType.IntVec2.Equals(uniformType) ||
                    //ActiveUniformType.IntVec3.Equals(uniformType) ||
                    //ActiveUniformType.IntVec4.Equals(uniformType);
        }

        private bool IsValidShaderAttributeType(ActiveAttribType attributeType) {
            return ActiveAttribType.Float.Equals(attributeType) ||
                    ActiveAttribType.FloatVec2.Equals(attributeType) ||
                    ActiveAttribType.FloatVec3.Equals(attributeType) ||
                    ActiveAttribType.FloatVec4.Equals(attributeType) ||
                    ActiveAttribType.FloatMat2.Equals(attributeType) ||
                    ActiveAttribType.FloatMat3.Equals(attributeType) ||
                    ActiveAttribType.FloatMat4.Equals(attributeType);// ||
                    //ActiveAttribType.Int.Equals(attributeType) ||
                    //ActiveAttribType.IntVec2.Equals(attributeType) ||
                    //ActiveAttribType.IntVec3.Equals(attributeType) ||
                    //ActiveAttribType.IntVec4.Equals(attributeType);
        }

        public void SetShaderUniform(int uniformLocation, float v1) {
            GL.Uniform1(uniformLocation, v1);
        }

        public void SetShaderUniform(int uniformLocation, float v1, float v2) {
            GL.Uniform2(uniformLocation, v1, v2);
        }

        public void SetShaderUniform(int uniformLocation, float v1, float v2, float v3) {
            GL.Uniform3(uniformLocation, v1, v2, v3);
        }

        public void SetShaderUniform(int uniformLocation, float v1, float v2, float v3, float v4) {
            GL.Uniform4(uniformLocation, v1, v2, v3, v4);
        }

        public void SetShaderUniform(int uniformLocation, int v1) {
            GL.Uniform1(uniformLocation, v1);
        }

        //public void SetShaderUniform(int uniformLocation, int v1, int v2) {
        //    GL.Uniform2(uniformLocation, v1, v2);
        //}

        //public void SetShaderUniform(int uniformLocation, int v1, int v2, int v3) {
        //    GL.Uniform3(uniformLocation, v1, v2, v3);
        //}

        //public void SetShaderUniform(int uniformLocation, int v1, int v2, int v3, int v4) {
        //    GL.Uniform4(uniformLocation, v1, v2, v3, v4);
        //}

        public void SetShaderUniform(int uniformLocation, Matrix2 v1) {
            GL.UniformMatrix2(uniformLocation, 1, false, v1.ColumnMajor);
        }

        public void SetShaderUniform(int uniformLocation, Matrix3 v1) {
            GL.UniformMatrix3(uniformLocation, 1, false, v1.ColumnMajor);
        }

        public void SetShaderUniform(int uniformLocation, Matrix4 v1) {
            GL.UniformMatrix4(uniformLocation, 1, false, v1.ColumnMajor);
        }

        public bool IsShaderBound(Shader shader) {
            return shader.Equals(this.boundShader);
        }

        #endregion

        #region Textures
        public void InitializeTexture(Texture2D texture) {
            int texID = GL.GenTexture();

            FieldInfo texIDFI = texture.GetType().GetField("texID", BindingFlags.NonPublic | BindingFlags.Instance);
            texIDFI.SetValue(texture, texID);

            FieldInfo bmpFI = texture.GetType().GetField("bitmap", BindingFlags.NonPublic | BindingFlags.Instance);
            Bitmap bmp = (Bitmap)bmpFI.GetValue(texture);
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bmp.UnlockBits(bitmapData);
        }

        /// <summary>
        /// Binds the texture to an available texture unit.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Returns the texture unit the texture was bound to. If all texture units are in use -1 is returned. </returns>
        public int BindTexture(Texture texture) {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            int textureUnit = BoundToTextureUnit(texture);
            if (textureUnit > 0)
                return textureUnit;

            if (AvailableTextureUnits == 0)
                return -1;

            textureUnit = NextAvailableTextureUnit;

            BindTexture(texture, textureUnit);

            return textureUnit;
        }


        /// <summary>
        /// Binds the texture to the given texture unit. if the texture is already bound to another texture unit this texture unit will be cleared.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="textureUnit">The texture unit.</param>
        public void BindTexture(Texture texture, int textureUnit) {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture));

            int previouslyBoundToTextureUnit = BoundToTextureUnit(texture);
            if (previouslyBoundToTextureUnit >= 0)
                ReleaseTexture(previouslyBoundToTextureUnit);

            ActivateTextureUnit(textureUnit);

            AvailableTextureUnits--;
            GL.BindTexture(TextureTarget.Texture2D, GetTextureID(texture));
            this.boundTextures[textureUnit] = texture;
        }

        /// <summary>
        /// Releases the texture of the given texture unit.
        /// </summary>
        /// <param name="textureUnit">The texture unit.</param>
        /// <returns>Returns the bound texture. If no texture was bound to the given texture unit null is returned.</returns>
        public void ReleaseTexture(int textureUnit) {
            Texture texture = BoundTexture(textureUnit);
            if (texture == null)
                return;

            if (ActiveTextureUnit != textureUnit)
                ActivateTextureUnit(textureUnit);

            this.boundTextures[textureUnit] = null;
            GL.BindTexture(TextureTarget.Texture2D, 0);
            ActiveTextureUnit = -1;

            AvailableTextureUnits++;
        }

        /// <summary>
        /// Releases the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Returns the texture unit the texture was bound to. If it was not bound -1 is returned.</returns>
        public void ReleaseTexture(Texture texture) {
            int textureUnit = BoundToTextureUnit(texture);

            if (textureUnit < 0)
                return;

            ReleaseTexture(textureUnit);
        }

        public void DisposeTexture(Texture texture) {
            ReleaseTexture(texture);
            GL.DeleteTexture(GetTextureID(texture));
        }

        public void ActivateTexture(Texture texture) {
            int textureUnit = BoundToTextureUnit(texture);

            if (textureUnit < 0)
                return;

            ActivateTextureUnit(textureUnit);
        }

        public void ActivateTextureUnit(int textureUnit) {
            if (BoundTexture(textureUnit) == null)
                return;

            if (ActiveTextureUnit == textureUnit)
                return;

            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            ActiveTextureUnit = textureUnit;
        }

        public Texture BoundTexture(int textureUnit) {
            if (textureUnit < 0 || textureUnit >= SupportedTextureUnits)
                return null;

            return BoundTextures[textureUnit];
        }

        /// <summary>
        /// Finds the texture unit the texture is bound to.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Returns the texture unit the texture is bound to. If it is not bound to any texture unit -1 is returned.</returns>
        public int BoundToTextureUnit(Texture texture) {
            if (texture == null)
                return -1;

            for (int i = 0; i < BoundTextures.Length; i++) {
                if (texture.Equals(BoundTextures[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Updates the texture's wrap mode.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public void UpdateTextureWrapMode(Texture texture) {
            ActivateTexture(texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapModeUtils.ToWrapMode(texture.WrapS));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapModeUtils.ToWrapMode(texture.WrapT));
        }

        /// <summary>
        /// Updates the texture's filter mode.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public void UpdateTextureFilterMode(Texture texture) {
            ActivateTexture(texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureFilterModeUtils.ToMinFilter(texture.MinFilter));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureFilterModeUtils.ToMinFilter(texture.MagFilter));
        }

        /// <summary>
        /// Determines whether the specified texture is bound.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///   <c>true</c> if the specified texture is bound; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTextureBound(Texture texture) {
            return BoundToTextureUnit(texture) != -1;
        }

        private int NextAvailableTextureUnit {
            get {
                for (int i = 0; i < BoundTextures.Length; i++) {
                    if (BoundTextures[i] == null)
                        return i;
                }

                return -1;
            }
        }

        private int GetTextureID(Texture texture) {
            PropertyInfo prop = texture.GetType().GetProperty("TextureID", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)prop.GetValue(texture);
            //MethodInfo getter = prop.GetGetMethod(true);
            //return (int)getter.Invoke(texture, null);
        }

        public bool HasActiveTexture => ActiveTextureUnit >= 0;

        public Texture ActiveTexture => BoundTexture(ActiveTextureUnit);

        private Texture[] BoundTextures {
            get {
                if (this.boundTextures == null) {
                    this.boundTextures = new Texture[SupportedTextureUnits];
                    this.availableTextureUnits = SupportedTextureUnits;
                }

                return this.boundTextures;
            }
        }

        private int AvailableTextureUnits {
            get {
                if (this.availableTextureUnits < 0) {
                    this.boundTextures = new Texture[SupportedTextureUnits];
                    this.availableTextureUnits = SupportedTextureUnits;
                }

                return this.availableTextureUnits;
            }
            set {
                this.availableTextureUnits = value;

                if (this.availableTextureUnits < 0 || this.availableTextureUnits > SupportedTextureUnits)
                    throw new RenderException("Invalid number of available texture units.");
            }
        }

        public int SupportedTextureUnits {
            get {
                if (this.supportedTextureUnits == -1)
                    this.supportedTextureUnits = GL.GetInteger(GetPName.MaxTextureUnits);

                return this.supportedTextureUnits;
            }
        }
        #endregion Textures

        #region Transforms
        public void ApplyTransformation(Transform transform) {
            if (!IsRendering)
                return;

            Matrix4 m = transform.LocalTransformationMatrix;
            if (this.transformStack.Any())
                m.MultiplyRight(this.transformStack.Peek());

            this.transformStack.Push(m);
        }

        //public void ApplyTranslation(float dx, float dy) {
        //    if (!IsRendering)
        //        return;

        //    Matrix4 m = MatrixTransformationHelper.SetTo3DTranslation(dx, -dy, 0, this.matrixPool.Get());
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        //public void ApplyRotation(float angle) {
        //    if (!IsRendering)
        //        return;

        //    Matrix4 m = MatrixTransformationHelper.SetTo3DZAxisClockwiseRotation(angle, this.matrixPool.Get());
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        //public void ApplyRotation(float angle, float px, float py) {
        //    if (!IsRendering)
        //        return;

        //    Matrix m = this.matrixPool.Get();
        //    m.RotateAt(angle * Utils.RAD_TO_DEG, new PointF(px, py));
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        //public void ApplyScaling(float sx, float sy) {
        //    if (!IsRendering)
        //        return;

        //    Matrix4 m = MatrixTransformationHelper.SetTo3DScaling(sx, sy, 1, this.matrixPool.Get());
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        public void RevertTransform() {
            if (!IsRendering)
                return;

            if (this.transformStack.Count == 0)
                return;

            this.matrixPool.Put(this.transformStack.Pop());
        }

        public Matrix4 CurrentTransformationMatrix => this.transformStack.Any() ? this.transformStack.Peek().Clone(): this.matrixPool.Get();

        #endregion

        #region RenderSettings
        public BlendMode BlendMode {
            set => BlendFunctions = BlendingModeUtils.ModeToFunctions(value);
        }

        public (BlendFunction source, BlendFunction destination)? BlendFunctions {
            get {
                if (this.blendFunctions == null)
                    return null;

                BlendFunction s = this.blendFunctions.Value.source;
                BlendFunction d = this.blendFunctions.Value.destination;

                return (s, d);
            }
            set {
                this.blendFunctions = value;

                EnableBlending = value != null;

                if (BlendFunctions != null) {
                    (BlendingFactorSrc source, BlendingFactorDest destination) fs = BlendingModeUtils.ToBlendFunctions(BlendFunctions.Value);
                    GL.BlendFunc(fs.source, fs.destination);
                }
            }
        }

        public GameEngine.Graphics.RenderSettings.DepthFunction? DepthFunction {
            get => this.depthFunction;
            set {
                this.depthFunction = value;

                EnableDepthTest = value != null;

                if (DepthFunction != null)
                    GL.DepthFunc(DepthFunctionUtils.ToDepthFunctions(DepthFunction.Value));
            }
        }

        public AntiAliasMode AntiAliasMode {
            get => this.antiAliasMode;
            set {
                this.antiAliasMode = value;

                GL.Hint(HintTarget.PointSmoothHint, AntiAliasModeUtils.ToHint(value));
                GL.Hint(HintTarget.LineSmoothHint, AntiAliasModeUtils.ToHint(value));
                GL.Hint(HintTarget.PolygonSmoothHint, AntiAliasModeUtils.ToHint(value));
            }
        }

        public GameEngine.Utility.Color ClearColor {
            get => this.clearColor;
            set {
                this.clearColor = value;
                GL.ClearColor(ClearColor.r, ClearColor.g, ClearColor.b, ClearColor.a);
            }
        }

        public void SetClearModes(bool color, bool depth, bool stencil) {
            ClearBufferMask cc = color ? ClearBufferMask.ColorBufferBit : 0;
            ClearBufferMask cd = color ? ClearBufferMask.DepthBufferBit : 0;
            ClearBufferMask cs = color ? ClearBufferMask.StencilBufferBit : 0;

            this.clearBufferMask = cc | cd | cs;
        }

        public bool EnableCulling {
            set {
                if (value)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);
            }
        }

        public bool ClockwiseCulling {
            set => GL.FrontFace(value ? FrontFaceDirection.Cw : FrontFaceDirection.Ccw);
        }

        public void CullFaces(bool front, bool back) {
            if (front && back)
                GL.CullFace(CullFaceMode.FrontAndBack);
            else if (front)
                GL.CullFace(CullFaceMode.Front);
            else if (back)
                GL.CullFace(CullFaceMode.Back);
        }

        public bool EnableTextures {
            set {
                if (value)
                    GL.Enable(EnableCap.Texture2D);
                else
                    GL.Disable(EnableCap.Texture2D);
            }
        }

        public bool EnableBlending {
            set {
                if (value)
                    GL.Enable(EnableCap.Blend);
                else
                    GL.Disable(EnableCap.Blend);
            }
        }

        public bool EnableDepthTest {
            set {
                if (value)
                    GL.Enable(EnableCap.DepthTest);
                else
                    GL.Disable(EnableCap.DepthTest);
            }
        }

        public bool EnableEdgeAntialiasing {
            set {
                if (value) {
                    GL.Enable(EnableCap.PointSmooth);
                    GL.Enable(EnableCap.LineSmooth);
                    GL.Enable(EnableCap.PolygonSmooth);
                } else {
                    GL.Disable(EnableCap.PointSmooth);
                    GL.Disable(EnableCap.LineSmooth);
                    GL.Disable(EnableCap.PolygonSmooth);
                }
            }
        }

        public void EnableColors(bool red, bool green, bool blue, bool alpha) {
            GL.ColorMask(red, green, blue, alpha);
        }

        #endregion RenderSettings

        //public void EnableLighting(bool enable) {
        //    if (enable)
        //        GL.Enable(GL_LIGHTING);
        //    else
        //        GL.Disable(GL_LIGHTING);
        //}

        #region Misc
        private static BufferUsageHint ToBufferUsageHint(BufferType bufferType) {
            switch (bufferType) {
                case BufferType.Dynamic:
                    return BufferUsageHint.DynamicDraw;
                case BufferType.Static:
                    return BufferUsageHint.StaticDraw;
            }

            throw new ArgumentException();
        }

        //public void SetVertexAttributePointer(int attributeIndex, int componentCount, bool v, int stride, int byteOffset) {
        //    throw new NotImplementedException();
        //}
        #endregion

    }
}