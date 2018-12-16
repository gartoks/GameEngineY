using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GameApp.Graphics.Buffers;
using GameApp.Graphics.Textures;
using GameApp.Graphics.Utility;
using GameEngine.Exceptions;
using GameEngine.Game.GameObjects;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Utility.DataStructures;
using OpenTK.Graphics.OpenGL;
using Color = GameEngine.Utility.Color;
using Log = GameApp.Logging.Log;
using TextureWrapMode = GameEngine.Graphics.RenderSettings.TextureWrapMode;

// ReSharper disable PossibleNullReferenceException

namespace GameApp.Graphics {
    internal sealed class GLHandler {   // TODO stencil
        private static GLHandler instance;
        internal static GLHandler Instance {
            get => GLHandler.instance;
            private set { if (GLHandler.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        private Thread renderThread;

        private Color clearColor;
        private ClearBufferMask clearBufferMask;

        internal bool IsRendering { get; private set; }
        //private Renderer renderer;

        private readonly Stack<Matrix4> transformStack;
        private readonly ObjectPool<Matrix4> matrixPool;

        private Texture[] assignedTextures;
        private Dictionary<Texture, int> assignedTextureUnits;

        private Shader boundShader;

        private Renderable boundVertexArrayObject;
        private VertexBufferObject boundVertexBufferObject;
        private IndexBufferObject boundIndexBufferObject;

        private int supportedTextureUnits;

        private (BlendFunction source, BlendFunction destination)? blendFunctions;
        private GameEngine.Graphics.RenderSettings.DepthFunction? depthFunction;
        private AntiAliasMode antiAliasMode;

        private Queue<Task> glTaskQueue;
        private Queue<Task> glTaskQueue_swap;

        internal GLHandler() {
            Instance = this;

            this.transformStack = new Stack<Matrix4>();
            this.matrixPool = new ObjectPool<Matrix4>(Matrix4.CreateIdentity, m => { m.MakeIdentity(); return m; });

            this.glTaskQueue = new Queue<Task>();
            this.glTaskQueue_swap = new Queue<Task>();

            IsRendering = false;
            //this.renderer = null;

            supportedTextureUnits = -1;
        }

        internal void Install() { }

        internal bool VerifyInstallation() => true;

        internal void Initialize() {
            EnableBlending = true;
            BlendMode = BlendMode.Default;

            EnableDepthTest = true;
            DepthFunction = GameEngine.Graphics.RenderSettings.DepthFunction.Less;

            //EnableColors(true, true, true, true);

            //EnableEdgeAntialiasing = true;
            //AntiAliasMode = AntiAliasMode.Fastest;

            EnableCulling = true;
            ClockwiseCulling = true;
            CullFaces(false, true);

            ClearColor = Color.BLACK;
            SetClearModes(true, true, false);

            this.assignedTextures = new Texture[SupportedTextureUnits];
            this.assignedTextureUnits = new Dictionary<Texture, int>();
        }

        internal void Queue(Task glTask) {
            lock (this.glTaskQueue) {
                this.glTaskQueue.Enqueue(glTask);
            }
        }

        internal void BeginRendering() {
            if (this.renderThread == null)
                this.renderThread = Thread.CurrentThread;

            if (this.supportedTextureUnits == -1)
                this.supportedTextureUnits = GL.GetInteger(GetPName.MaxTextureUnits);

            lock (this.glTaskQueue) {
                Queue<Task> tmp = glTaskQueue;
                glTaskQueue = glTaskQueue_swap;
                glTaskQueue_swap = tmp;
            }

            foreach (Task glTask in glTaskQueue_swap)
                glTask.RunSynchronously();
            glTaskQueue_swap.Clear();

            this.transformStack.Clear();

            //GL.ClearDepth(1f);  // TODO maybe allow different values
            GL.Clear(this.clearBufferMask);

            //EnableBlending = true;
            //BlendMode = BlendMode.Default;

            //EnableDepthTest = true;
            //DepthFunction = GameEngine.Graphics.RenderSettings.DepthFunction.Less;

            //Viewport viewport = Game.Instance.Viewport;
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(viewport.ViewProjectionMatrix.ColumnMajor);

            IsRendering = true;
        }

        internal void EndRendering() {
            IsRendering = false;

            GL.Flush();

            Window.Window.Instance.SwapBuffers();   // un-nice

            foreach (Matrix4 m in this.transformStack) {
                this.matrixPool.Put(m);
            }
        }

        internal void Render(IndexBufferObject ibo) {
            BindIBO(ibo);
            GL.DrawElements(PrimitiveType.Triangles, ibo.Size, DrawElementsType.UnsignedInt, 0);
            ReleaseIBO(ibo);
        }

        #region VertexArrayObjects
        //internal int CreateVAO() {
        //    return GL.GenVertexArray();
        //}

        //internal void BindVAO(Renderable vao) {
        //    if (IsVAOBound(vao))
        //        return;

        //    if (vao.IsDisposed) {
        //        Log.Instance.WriteLine("Cannot bind vertex array object. It is disposed.", LogType.Error);
        //        return;
        //    }

        //    int vaoID = GetVAOID(vao);

        //    GL.BindVertexArray(vaoID);
        //    this.boundVertexArrayObject = vao;
        //}

        //internal void ReleaseVAO(Renderable vao) {
        //    if (!IsVAOBound(vao))
        //        return;

        //    if (vao.IsDisposed) {
        //        Log.Instance.WriteLine("Cannot release vertex array object. It is disposed.", LogType.Error);
        //        return;
        //    }

        //    GL.BindVertexArray(0);
        //    this.boundVertexArrayObject = null;
        //}

        //internal void DeleteVAO(Renderable vao) {
        //    if (vao == null)
        //        return;

        //    if (vao.IsBound)
        //        ReleaseVAO(vao);

        //    FieldInfo vaoIDFI = vao.GetType().GetField("vaoID", BindingFlags.NonPublic | BindingFlags.Instance);

        //    int vaoID = (int)vaoIDFI.GetValue(vao);
        //    vaoIDFI.SetValue(vao, -1);

        //    GL.DeleteVertexArray(vaoID);
        //}

        //internal bool IsVAOBound(Renderable vao) => vao != null && vao.Equals(this.boundVertexArrayObject);

        //private static int GetVAOID(Renderable vao) {
        //    FieldInfo vaoIDFI = vao.GetType().GetField("vaoID", BindingFlags.NonPublic | BindingFlags.Instance);

        //    return (int)vaoIDFI.GetValue(vao);
        //}
        #endregion

        #region VertexBufferObjects
        internal VertexBufferObject CreateVBO(float[] data, BufferType type) {
            int vboID = GL.GenBuffer();

            VertexBufferObject vbo = new VertexBufferObject(vboID, data, type);

            VertexBufferObject previouslyBoundVBO = this.boundVertexBufferObject;

            BindVBO(vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vbo.Size * sizeof(float), (IntPtr)null, ToBufferUsageHint(vbo.Type));

            IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
            Marshal.Copy(vbo.Data, 0, mapBufferPtr, vbo.Size);
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);

            if (previouslyBoundVBO != null)
                BindVBO(previouslyBoundVBO);
            else
                ReleaseVBO(vbo);

            return vbo;
        }

        internal void BindVBO(VertexBufferObject vbo) {
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

        internal void ReleaseVBO(VertexBufferObject vbo) {
            if (!IsVBOBound(vbo))
                return;

            if (vbo.IsDisposed) {
                Log.Instance.WriteLine("Cannot release vertex buffer object. It is disposed.", LogType.Error);
                return;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            this.boundVertexBufferObject = null;
        }

        internal void DeleteVBO(VertexBufferObject vbo) {
            if (vbo == null || vbo.IsDisposed)
                return;

            if (vbo.IsBound)
                ReleaseVBO(vbo);

            FieldInfo vboIDFI = vbo.GetType().GetField("vboID", BindingFlags.NonPublic | BindingFlags.Instance);

            int vboID = (int)vboIDFI.GetValue(vbo);
            vboIDFI.SetValue(vbo, -1);

            GL.DeleteBuffer(vboID);
        }

        internal void UpdateVBOData(VertexBufferObject vbo) {
            VertexBufferObject previouslyBoundVBO = this.boundVertexBufferObject;

            BindVBO(vbo);

            //GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, (IntPtr)(vbo.Size * sizeof(float)), vbo.Data);

            IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
            Marshal.Copy(vbo.Data, 0, mapBufferPtr, vbo.Data.Length);
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);

            if (previouslyBoundVBO != null)
                BindVBO(previouslyBoundVBO);
            else
                ReleaseVBO(vbo);
        }

        internal bool IsVBOBound(VertexBufferObject vbo) => vbo != null && vbo.Equals(this.boundVertexBufferObject);

        private static int GetVBOID(VertexBufferObject vbo) {
            FieldInfo vboIDFI = vbo.GetType().GetField("vboID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (int)vboIDFI.GetValue(vbo);
        }
        #endregion

        #region IndexBufferObjects
        internal IndexBufferObject CreateIBO(int[] data, BufferType type) {
            int iboID = GL.GenBuffer();

            IndexBufferObject ibo = new IndexBufferObject(iboID, data, type);

            IndexBufferObject previouslyBoundIBO = this.boundIndexBufferObject;

            BindIBO(ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, ibo.Size * sizeof(int), (IntPtr)null, ToBufferUsageHint(ibo.Type));

            IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
            Marshal.Copy(ibo.Data, 0, mapBufferPtr, ibo.Size);
            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);

            if (previouslyBoundIBO != null)
                BindIBO(previouslyBoundIBO);
            else
                ReleaseIBO(ibo);

            return ibo;
        }

        internal void BindIBO(IndexBufferObject ibo) {
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

        internal void ReleaseIBO(IndexBufferObject ibo) {
            if (!IsIBOBound(ibo))
                return;

            if (ibo.IsDisposed) {
                Log.Instance.WriteLine("Cannot release index buffer object. It is disposed.", LogType.Error);
                return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            this.boundIndexBufferObject = null;
        }

        //private void MapIBOData(IndexBufferObject ibo) {
        //    if (ibo.IsDisposed) {
        //        Log.Instance.WriteLine("Cannot release index buffer object. It is disposed.", LogType.Error);
        //        return;
        //    }

        //    IndexBufferObject previouslyBoundIBO = this.boundIndexBufferObject;

        //    BindIBO(ibo);

        //    IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
        //    Marshal.Copy(ibo.Data, 0, mapBufferPtr, ibo.Size);
        //    GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);

        //    if (previouslyBoundIBO != null)
        //        BindIBO(previouslyBoundIBO);
        //    else
        //        ReleaseIBO(ibo);
        //}

        internal void DeleteIBO(IndexBufferObject ibo) {
            if (ibo == null || ibo.IsDisposed)
                return;

            if (ibo.IsBound)
                ReleaseIBO(ibo);

            FieldInfo iboIDFI = ibo.GetType().GetField("iboID", BindingFlags.NonPublic | BindingFlags.Instance);

            int iboID = (int)iboIDFI.GetValue(ibo);
            iboIDFI.SetValue(ibo, -1);

            try {
                GL.DeleteBuffer(iboID);
            } catch (AccessViolationException) { }
        }

        internal void UpdateIBOData(IndexBufferObject ibo) {
            IndexBufferObject previouslyBoundIBO = this.boundIndexBufferObject;

            BindIBO(ibo);

            IntPtr mapBufferPtr = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
            Marshal.Copy(ibo.Data, 0, mapBufferPtr, ibo.Size);
            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);

            if (previouslyBoundIBO != null)
                BindIBO(previouslyBoundIBO);
            else
                ReleaseIBO(ibo);
        }

        internal bool IsIBOBound(IndexBufferObject ibo) => ibo != null && ibo.Equals(this.boundIndexBufferObject);

        private static int GetIBOID(IndexBufferObject ibo) {
            FieldInfo iboIDFI = ibo.GetType().GetField("iboID", BindingFlags.NonPublic | BindingFlags.Instance);

            return (int)iboIDFI.GetValue(ibo);
        }
        #endregion

        #region Shaders
        internal Shader CreateShader(string vertexShaderSource, string fragmentShaderSource) {
            int programHandle = 0;
            int vertexShaderHandle = CompileShader(vertexShaderSource, ShaderType.VertexShader);
            int fragmentShaderHandle = CompileShader(fragmentShaderSource, ShaderType.FragmentShader);
            var uniforms = new Dictionary<string, ShaderUniform>();
            var attributes = new Dictionary<string, ShaderVertexAttribute>();
            int stride = 0;

            if (vertexShaderHandle == 0 || fragmentShaderHandle == 0) {
                Log.Instance.WriteLine("Could not create vertex- or fragment shader handle.", LogType.Error);
                return null;
            }

            programHandle = CreateShaderProrgam(vertexShaderHandle, fragmentShaderHandle);

            if (programHandle == 0) {
                Log.Instance.WriteLine("Could not link shader and create program handle.", LogType.Error);
                return null;
            }
            if (!TryRetrieveShaderUniforms(programHandle, out uniforms)) {
                Log.Instance.WriteLine("Could not retrieve shader uniforms.", LogType.Error);
                GL.DeleteProgram(programHandle);
                GL.DeleteShader(vertexShaderHandle);
                GL.DeleteShader(fragmentShaderHandle); return null;
            }

            if (!TryRetrieveShaderAttributes(programHandle, out attributes)) {
                Log.Instance.WriteLine("Could not retrieve shader attributes.", LogType.Error);
                GL.DeleteProgram(programHandle);
                GL.DeleteShader(vertexShaderHandle);
                GL.DeleteShader(fragmentShaderHandle); return null;
            }

            stride = attributes.Values.Sum(sva => sva.ComponentCount) * sizeof(float);

            return new Shader(vertexShaderSource, fragmentShaderSource, vertexShaderHandle, fragmentShaderHandle, programHandle, uniforms, attributes, stride);
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
            if (status == 0) {
                string infoLog = GL.GetShaderInfoLog(shader);
                Log.Instance.WriteLine($"{shaderType} compliation error: {infoLog}");
                return 0;
            }

            return shader;
        }

        internal void DeleteShader(Shader shader) {
            if (!shader.IsCompiled)
                return;

            if (IsShaderBound(shader))
                ReleaseShader(shader);

            FieldInfo phFI = typeof(Shader).GetField("programHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            int ph = (int)phFI.GetValue(shader);
            phFI.SetValue(shader, 0);

            FieldInfo vshFI = typeof(Shader).GetField("vertexShaderHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            int vsh = (int)vshFI.GetValue(shader);
            vshFI.SetValue(shader, 0);

            FieldInfo fshFI = typeof(Shader).GetField("fragmentShaderHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            int fsh = (int)fshFI.GetValue(shader);
            fshFI.SetValue(shader, 0);

            GL.DeleteProgram(ph);
            GL.DeleteShader(vsh);
            GL.DeleteShader(fsh);
        }

        internal void BindShader(Shader shader) {
            if (shader.Equals(this.boundShader))
                return;

            (int program, int vertexShader, int fragmentShader) handles = GetShaderHandles(shader);

            GL.UseProgram(handles.program);
            this.boundShader = shader;
        }

        internal void ReleaseShader(Shader shader) {
            if (!this.boundShader.Equals(shader))
                return;
            //asdasd // TODO set to default shader ?
            GL.UseProgram(0);
            this.boundShader = null;
        }

        internal void SetVertexAttributePointer(ShaderVertexAttribute vertexAttribute, /*Type type,bool normalized,  */int stride) {
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

        internal void EnableVertexAttributeArray(int attributeIndex) {
            GL.EnableVertexAttribArray(attributeIndex);
        }

        internal void DisableVertexAttributeArray(int attributeIndex) {
            GL.DisableVertexAttribArray(attributeIndex);
        }

        private static (int ph, int vsh, int fsh) GetShaderHandles(Shader shader) {
            FieldInfo programHandleFI = shader.GetType().GetField("programHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo vertexShaderHandleFI = shader.GetType().GetField("vertexShaderHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo fragmentShaderHandleFI = shader.GetType().GetField("fragmentShaderHandle", BindingFlags.NonPublic | BindingFlags.Instance);

            int programHandle = (int)programHandleFI.GetValue(shader);
            int vertexShaderHandle = (int)vertexShaderHandleFI.GetValue(shader);
            int fragmentShaderHandle = (int)fragmentShaderHandleFI.GetValue(shader);

            return (programHandle, vertexShaderHandle, fragmentShaderHandle);
        }

        private bool TryRetrieveShaderUniforms(int shaderProgramHandle, out Dictionary<string, ShaderUniform> uniforms) {
            uniforms = new Dictionary<string, ShaderUniform>();

            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            bool validUniforms = true;
            for (int i = 0; i < uniformCount; i++) {
                string uniformName = GL.GetActiveUniform(shaderProgramHandle, i, out int size, out ActiveUniformType type);
                int uniformLocation = GL.GetUniformLocation(shaderProgramHandle, uniformName);

                if (!IsValidShaderUniformType(type)) {
                    Log.Instance.WriteLine($"Invalid shader uniform type {type.ToString()} for uniform '{uniformName}'.", LogType.Error);
                    validUniforms = false;
                    break;
                }

                UniformType uniformType = ConvertUniformType(type);

                ShaderUniform uniform = new ShaderUniform(uniformName, uniformType, uniformLocation, size * ActiveUniformTypeToSize(type));
                uniforms.Add(uniformName, uniform);
            }

            if (!validUniforms)
                uniforms.Clear();

            return validUniforms;
        }

        private bool TryRetrieveShaderAttributes(int shaderProgramHandle, out Dictionary<string, ShaderVertexAttribute> attributes) {
            Dictionary<string, ShaderVertexAttribute> tmpAttributes = new Dictionary<string, ShaderVertexAttribute>();

            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveAttributes, out int attributeCount);

            bool validAttributes = true;
            for (int i = 0; i < attributeCount; i++) {
                String attributeName = GL.GetActiveAttrib(shaderProgramHandle, i, out int size, out ActiveAttribType type);
                int attributeLocation = GL.GetAttribLocation(shaderProgramHandle, attributeName);

                if (!IsValidShaderAttributeType(type)) {
                    Log.Instance.WriteLine($"Invalid shader attribute type {type.ToString()} for attribute '{attributeName}'.", LogType.Error);
                    validAttributes = false;
                    break;
                }

                VertexAttributeType attributeType = ConvertVertexAttributeType(type);

                ShaderVertexAttribute attribute = new ShaderVertexAttribute(attributeName, attributeLocation, attributeType);
                tmpAttributes.Add(attributeName, attribute);
            }

            IOrderedEnumerable<KeyValuePair<string, ShaderVertexAttribute>> keyValuePairs = tmpAttributes.OrderBy(pair => pair.Value.AttributeIndex);
            attributes = new Dictionary<string, ShaderVertexAttribute>();
            foreach (KeyValuePair<string, ShaderVertexAttribute> pair in keyValuePairs) {
                attributes[pair.Key] = pair.Value;
            }

            if (!validAttributes) {
                attributes.Clear();
                return false;
            }

            int currentOffset = 0;
            List<ShaderVertexAttribute> nonProcessedVAs = new List<ShaderVertexAttribute>(attributes.Values);
            while (nonProcessedVAs.Count > 0) {

                // get smallest index sva
                ShaderVertexAttribute currentSVA = null;
                foreach (ShaderVertexAttribute sva2 in nonProcessedVAs) {
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

            return true;
        }

        private UniformType ConvertUniformType(ActiveUniformType uniformType) {
            switch (uniformType) {
                //case ActiveUniformType.Int: return Shader.UniformType.Int;
                case ActiveUniformType.Float: return UniformType.Float;
                case ActiveUniformType.FloatVec2: return UniformType.FloatVector2;
                case ActiveUniformType.FloatVec3: return UniformType.FloatVector3;
                case ActiveUniformType.FloatVec4: return UniformType.FloatVector4;
                case ActiveUniformType.FloatMat2: return UniformType.Matrix2x2;
                case ActiveUniformType.FloatMat3: return UniformType.Matrix3x3;
                case ActiveUniformType.FloatMat4: return UniformType.Matrix4x4;
                case ActiveUniformType.Sampler2D: return UniformType.Texture2D;
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

        private VertexAttributeType ConvertVertexAttributeType(ActiveAttribType attributeType) {
            switch (attributeType) {
                //case ActiveUniformType.Int: return Shader.UniformType.Int;
                case ActiveAttribType.Float: return VertexAttributeType.Float;
                case ActiveAttribType.FloatVec2: return VertexAttributeType.FloatVector2;
                case ActiveAttribType.FloatVec3: return VertexAttributeType.FloatVector3;
                case ActiveAttribType.FloatVec4: return VertexAttributeType.FloatVector4;
            }

            throw new ArgumentException();
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

        internal void SetShaderUniform(int uniformLocation, float v1) {
            GL.Uniform1(uniformLocation, v1);
        }

        internal void SetShaderUniform(int uniformLocation, float v1, float v2) {
            GL.Uniform2(uniformLocation, v1, v2);
        }

        internal void SetShaderUniform(int uniformLocation, float v1, float v2, float v3) {
            GL.Uniform3(uniformLocation, v1, v2, v3);
        }

        internal void SetShaderUniform(int uniformLocation, float v1, float v2, float v3, float v4) {
            GL.Uniform4(uniformLocation, v1, v2, v3, v4);
        }

        internal void SetShaderUniform(int uniformLocation, int v1) {
            GL.Uniform1(uniformLocation, v1);
        }

        //internal void SetShaderUniform(int uniformLocation, int v1, int v2) {
        //    GL.Uniform2(uniformLocation, v1, v2);
        //}

        //internal void SetShaderUniform(int uniformLocation, int v1, int v2, int v3) {
        //    GL.Uniform3(uniformLocation, v1, v2, v3);
        //}

        //internal void SetShaderUniform(int uniformLocation, int v1, int v2, int v3, int v4) {
        //    GL.Uniform4(uniformLocation, v1, v2, v3, v4);
        //}

        internal void SetShaderUniform(int uniformLocation, Matrix2 v1) {
            GL.UniformMatrix2(uniformLocation, 1, false, v1.ColumnMajor);
        }

        internal void SetShaderUniform(int uniformLocation, Matrix3 v1) {
            GL.UniformMatrix3(uniformLocation, 1, false, v1.ColumnMajor);
        }

        internal void SetShaderUniform(int uniformLocation, Matrix4 v1) {
            GL.UniformMatrix4(uniformLocation, 1, false, v1.ColumnMajor);
        }

        internal bool IsShaderBound(Shader shader) {
            return shader.Equals(this.boundShader);
        }

        #endregion

        #region Textures
        internal void InitializeTexture(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFilter, TextureFilterMode magFilter, out int textureID) {
            textureID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmapData.Width, bitmapData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);

            bitmap.UnlockBits(bitmapData);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapModeUtils.ToWrapMode(wrapS));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapModeUtils.ToWrapMode(wrapT));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureFilterModeUtils.ToMinFilter(minFilter));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureFilterModeUtils.ToMinFilter(magFilter));

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        internal void UpdateTextureData(Texture texture, int offsetX, int offsetY, Bitmap bitmap) {
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, offsetX, offsetY, bitmapData.Width, bitmapData.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        internal void DisposeTexture(Texture2D texture) {
            if (texture.IsDisposed)
                return;

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            FieldInfo texIDFI = typeof(Texture2D).GetField("textureID", BindingFlags.Instance | BindingFlags.NonPublic);
            texIDFI.SetValue(texture, 0);

            GL.DeleteTexture(texture.TextureID);
        }

        internal void AssignTexture(Texture2D texture) {
            int unassignedTextureUnit = GetUnassignedTextureUnit();
            if (unassignedTextureUnit < 0) {
                Log.Instance.WriteLine("Cannot assign texture. No texture unit is available.", LogType.Warning);
                return;
            }

            AssignTexture(texture, unassignedTextureUnit, false);
        }

        internal void AssignTexture(Texture2D texture, int textureUnit, bool overrideBoundTexture = true) {
            if (textureUnit < 0 || textureUnit >= SupportedTextureUnits) {
                Log.Instance.WriteLine($"Invalid texture unit {textureUnit}. Must be between 0 and {SupportedTextureUnits} (exclusive).", LogType.Error);
                return;
            }

            if (IsTextureAssigned(texture))
                return;

            Texture currentlyAssignedTexture = AssignedTexture(textureUnit);
            if (currentlyAssignedTexture != null && !overrideBoundTexture) {
                Log.Instance.WriteLine($"Cannot assign texture {texture.TextureID} to texture unit {textureUnit}. Another texture is already assigned.", LogType.Warning);
                return;
            }

            GL.BindTextureUnit(textureUnit, texture.TextureID);

            this.assignedTextures[textureUnit] = texture;
            this.assignedTextureUnits[texture] = textureUnit;
        }

        internal void UnassignTexture(Texture texture) {
            int assignedTextureUnit = AssignedTextureUnit(texture);
            if (assignedTextureUnit < 0)
                return;

            this.assignedTextures[assignedTextureUnit] = null;
            this.assignedTextureUnits.Remove(texture);
            GL.BindTextureUnit(assignedTextureUnit, 0);
        }

        internal void UnassignTextureUnit(int textureUnit) {
            if (textureUnit < 0 || textureUnit >= SupportedTextureUnits) {
                Log.Instance.WriteLine($"Invalid texture unit {textureUnit}. Must be between 0 and {SupportedTextureUnits} (exclusive).", LogType.Error);
                return;
            }

            Texture currentlyAssignedTexture = AssignedTexture(textureUnit);
            this.assignedTextures[textureUnit] = null;
            this.assignedTextureUnits.Remove(currentlyAssignedTexture);
            GL.BindTextureUnit(textureUnit, 0);
        }

        internal bool IsTextureAssigned(Texture texture) => AssignedTextureUnit(texture) >= 0;

        internal int AssignedTextureUnit(Texture texture) {
            if (this.assignedTextureUnits.TryGetValue(texture, out int textureUnit))
                return textureUnit;
            else
                return -1;
        }

        internal Texture AssignedTexture(int textureUnit) {
            if (textureUnit < 0 || textureUnit >= SupportedTextureUnits) {
                Log.Instance.WriteLine($"Invalid texture unit {textureUnit}. Must be between 0 and {SupportedTextureUnits} (exclusive).", LogType.Error);
                return null;
            }

            return this.assignedTextures[textureUnit];
        }

        internal int GetUnassignedTextureUnit() {
            for (int i = 0; i < this.assignedTextures.Length; i++) {
                if (this.assignedTextures[i] == null)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Updates the texture's wrap mode.
        /// </summary>
        /// <param name="texture">The texture.</param>
        internal void UpdateTextureWrapMode(Texture texture) {
            if (texture.IsDisposed) {
                Log.Instance.WriteLine("Cannot update texture wrap mode. Texture is disposed.", LogType.Error);
                return;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapModeUtils.ToWrapMode(texture.WrapS));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapModeUtils.ToWrapMode(texture.WrapT));

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Updates the texture's filter mode.
        /// </summary>
        /// <param name="texture">The texture.</param>
        internal void UpdateTextureFilterMode(Texture texture) {
            if (texture.IsDisposed) {
                Log.Instance.WriteLine("Cannot update texture filter mode. Texture is disposed.", LogType.Error);
                return;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureFilterModeUtils.ToMinFilter(texture.MinFilter));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureFilterModeUtils.ToMinFilter(texture.MagFilter));

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Determines whether the specified texture is bound.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///   <c>true</c> if the specified texture is bound; otherwise, <c>false</c>.
        /// </returns>
        internal int SupportedTextureUnits {
            get {
                if (this.supportedTextureUnits == -1)
                    this.supportedTextureUnits = GL.GetInteger(GetPName.MaxTextureUnits);

                return this.supportedTextureUnits;
            }
        }

        #endregion Textures

        #region Transforms
        internal void ApplyTransformation(Transform transform) {
            if (!IsRendering)
                return;

            Matrix4 m = transform.LocalTransformationMatrix;
            if (this.transformStack.Any())
                m.MultiplyRight(this.transformStack.Peek());

            this.transformStack.Push(m);
        }

        //internal void ApplyTranslation(float dx, float dy) {
        //    if (!IsRendering)
        //        return;

        //    Matrix4 m = MatrixTransformationHelper.SetTo3DTranslation(dx, -dy, 0, this.matrixPool.Get());
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        //internal void ApplyRotation(float angle) {
        //    if (!IsRendering)
        //        return;

        //    Matrix4 m = MatrixTransformationHelper.SetTo3DZAxisClockwiseRotation(angle, this.matrixPool.Get());
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        //internal void ApplyRotation(float angle, float px, float py) {
        //    if (!IsRendering)
        //        return;

        //    Matrix m = this.matrixPool.Get();
        //    m.RotateAt(angle * Utils.RAD_TO_DEG, new PointF(px, py));
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        //internal void ApplyScaling(float sx, float sy) {
        //    if (!IsRendering)
        //        return;

        //    Matrix4 m = MatrixTransformationHelper.SetTo3DScaling(sx, sy, 1, this.matrixPool.Get());
        //    if (this.transformStack.Any())
        //        m.MultiplyRight(this.transformStack.Peek());

        //    this.transformStack.Push(m);
        //}

        internal void RevertTransform() {
            if (!IsRendering)
                return;

            if (this.transformStack.Count == 0)
                return;

            this.matrixPool.Put(this.transformStack.Pop());
        }

        internal Matrix4 CurrentTransformationMatrix => this.transformStack.Any() ? this.transformStack.Peek().Clone(): this.matrixPool.Get();

        #endregion

        #region RenderSettings
        internal BlendMode BlendMode {
            set => BlendFunctions = BlendingModeUtils.ModeToFunctions(value);
        }

        internal (BlendFunction source, BlendFunction destination)? BlendFunctions {
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

        internal GameEngine.Graphics.RenderSettings.DepthFunction? DepthFunction {
            get => this.depthFunction;
            set {
                this.depthFunction = value;

                EnableDepthTest = value != null;

                if (DepthFunction != null)
                    GL.DepthFunc(DepthFunctionUtils.ToDepthFunctions(DepthFunction.Value));
            }
        }

        internal AntiAliasMode AntiAliasMode {
            get => this.antiAliasMode;
            set {
                this.antiAliasMode = value;

                GL.Hint(HintTarget.PointSmoothHint, AntiAliasModeUtils.ToHint(value));
                GL.Hint(HintTarget.LineSmoothHint, AntiAliasModeUtils.ToHint(value));
                GL.Hint(HintTarget.PolygonSmoothHint, AntiAliasModeUtils.ToHint(value));
            }
        }

        internal Color ClearColor {
            get => this.clearColor;
            set {
                this.clearColor = value;
                GL.ClearColor(ClearColor.r, ClearColor.g, ClearColor.b, ClearColor.a);
            }
        }

        internal void SetClearModes(bool color, bool depth, bool stencil) {
            ClearBufferMask cc = color ? ClearBufferMask.ColorBufferBit : 0;
            ClearBufferMask cd = depth ? ClearBufferMask.DepthBufferBit : 0;
            ClearBufferMask cs = stencil ? ClearBufferMask.StencilBufferBit : 0;

            this.clearBufferMask = cc | cd | cs;
        }

        internal bool EnableCulling {
            set {
                if (value)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);
            }
        }

        internal bool ClockwiseCulling {
            set => GL.FrontFace(value ? FrontFaceDirection.Cw : FrontFaceDirection.Ccw);
        }

        internal void CullFaces(bool front, bool back) {
            if (front && back)
                GL.CullFace(CullFaceMode.FrontAndBack);
            else if (front)
                GL.CullFace(CullFaceMode.Front);
            else if (back)
                GL.CullFace(CullFaceMode.Back);
        }

        internal bool EnableBlending {
            set {
                if (value)
                    GL.Enable(EnableCap.Blend);
                else
                    GL.Disable(EnableCap.Blend);
            }
        }

        internal bool EnableDepthTest {
            set {
                if (value)
                    GL.Enable(EnableCap.DepthTest);
                else
                    GL.Disable(EnableCap.DepthTest);
            }
        }

        internal bool EnableEdgeAntialiasing {
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

        internal void EnableColors(bool red, bool green, bool blue, bool alpha) {
            GL.ColorMask(red, green, blue, alpha);
        }

        #endregion RenderSettings

        //internal void EnableLighting(bool enable) {
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

        private static int ActiveAttribTypeToSize(ActiveAttribType type) {
            string typeString = type.ToString();

            if (typeString.EndsWith("Vec2"))
                return 2;

            if (typeString.EndsWith("Vec3"))
                return 3;

            if (typeString.EndsWith("Vec4"))
                return 4;

            return 1;
        }

        private static int ActiveUniformTypeToSize(ActiveUniformType type) {
            string typeString = type.ToString();

            if (typeString.EndsWith("Vec2"))
                return 2;

            if (typeString.EndsWith("Vec3"))
                return 3;

            if (typeString.EndsWith("Vec4"))
                return 4;

            return 1;
        }

        //internal void SetVertexAttributePointer(int attributeIndex, int componentCount, bool v, int stride, int byteOffset) {
        //    throw new NotImplementedException();
        //}
        #endregion

    }
}