using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GameEngine.Game.GameObjects;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Textures;
using GameEngine.Graphics.Utility;
using GameEngine.Math;
using GameEngine.Modding;
using GameEngine.Utility;

namespace GameEngine.Graphics {
    internal static class GLHandler {

        private static IGLHandler handler;

        internal static void Render(IndexBufferObject ibo) {
            Handler.Render(ibo);
        }

        #region VertexArrayObjects
        internal static void InitializeVAO(Renderable renderable) {
            Handler.InitializeVAO(renderable);
        }

        internal static void BindVAO(Renderable renderable) {
            Handler.BindVAO(renderable);
        }

        internal static void ReleaseVAO(Renderable renderable) {
            Handler.ReleaseVAO(renderable);
        }

        internal static void DeleteVAO(Renderable renderable) {
            Handler.DeleteVAO(renderable);
        }

        internal static bool IsVAOBound(Renderable renderable) {
            return Handler.IsVAOBound(renderable);
        }
        #endregion

        #region VertexBufferObjects
        internal static void InitializeVBO(VertexBufferObject vbo) {
            Handler.InitializeVBO(vbo);
        }

        internal static void BindVBO(VertexBufferObject vbo) {
            Handler.BindVBO(vbo);
        }

        internal static void ReleaseVBO(VertexBufferObject vbo) {
            Handler.ReleaseVBO(vbo);
        }

        internal static void MapVBOData(VertexBufferObject vbo) {
            Handler.MapVBOData(vbo);
        }

        internal static void DeleteVBO(VertexBufferObject vbo) {
            Handler.DeleteVBO(vbo);
        }

        internal static bool IsVBOBound(VertexBufferObject vbo) {
            return Handler.IsVBOBound(vbo);
        }
        #endregion

        #region IndexBufferObject
        internal static void InitializeIBO(IndexBufferObject ibo) {
            Handler.InitializeIBO(ibo);
        }

        internal static void BindIBO(IndexBufferObject ibo) {
            Handler.BindIBO(ibo);
        }

        internal static void ReleaseIBO(IndexBufferObject ibo) {
            Handler.ReleaseIBO(ibo);
        }

        internal static void MapIBOData(IndexBufferObject ibo) {
            Handler.MapIBOData(ibo);
        }

        internal static void DeleteIBO(IndexBufferObject ibo) {
            Handler.DeleteIBO(ibo);
        }

        internal static bool IsIBOBound(IndexBufferObject ibo) {
            return Handler.IsIBOBound(ibo);
        }
        #endregion

        #region Shaders
        internal static void CreateShader(string vertexShaderSource, string fragmentShaderSource, /*string geometryShaderSource,*/
                            out int programHandle, out int vertexShaderHandle, out int fragmentShaderHandle/*, out int geometryShaderHandle*/) {
            Handler.CreateShader(vertexShaderSource, fragmentShaderSource, out programHandle, out vertexShaderHandle, out fragmentShaderHandle);
        }

        internal static void DeleteShader(Shader shader) {
            Handler.DeleteShader(shader);
        }

        internal static void BindShader(Shader shader) {
            Handler.BindShader(shader);
        }

        internal static void ReleaseShader(Shader shader) {
            Handler.ReleaseShader(shader);
        }

        internal static void SetVertexAttributePointer(Shader.ShaderVertexAttribute vertexAttribute, /*Type type, bool v, */int stride) {
            Handler.SetVertexAttributePointer(vertexAttribute, stride);
        }

        internal static void EnableVertexAttributeArray(int attributeIndex) {
            Handler.EnableVertexAttributeArray(attributeIndex);
        }

        internal static void DisableVertexAttributeArray(int attributeIndex) {
            Handler.DisableVertexAttributeArray(attributeIndex);
        }

        internal static Dictionary<string, Shader.ShaderUniform> RetrieveShaderUniforms(Shader shader, out bool validUniforms) {
            return Handler.RetrieveShaderUniforms(shader, out validUniforms);
        }

        internal static Dictionary<string, Shader.ShaderVertexAttribute> RetrieveShaderAttributes(Shader shader, out bool validAttributes) {
            return Handler.RetrieveShaderAttributes(shader, out validAttributes);
        }

        internal static void SetShaderUniform(int uniformLocation, float v1) {
            Handler.SetShaderUniform(uniformLocation, v1);
        }

        internal static void SetShaderUniform(int uniformLocation, float v1, float v2) {
            Handler.SetShaderUniform(uniformLocation, v1, v2);
        }

        internal static void SetShaderUniform(int uniformLocation, float v1, float v2, float v3) {
            Handler.SetShaderUniform(uniformLocation, v1, v2, v3);
        }

        internal static void SetShaderUniform(int uniformLocation, float v1, float v2, float v3, float v4) {
            Handler.SetShaderUniform(uniformLocation, v1, v2, v3, v4);
        }

        internal static void SetShaderUniform(int uniformLocation, int v1) {
            Handler.SetShaderUniform(uniformLocation, v1);
        }

        //internal static void SetShaderUniform(int uniformLocation, int v1, int v2) {
        //    Handler.SetShaderUniform(uniformLocation, v1, v2);
        //}

        //internal static void SetShaderUniform(int uniformLocation, int v1, int v2, int v3) {
        //    Handler.SetShaderUniform(uniformLocation, v1, v2, v3);
        //}

        //internal static void SetShaderUniform(int uniformLocation, int v1, int v2, int v3, int v4) {
        //    Handler.SetShaderUniform(uniformLocation, v1, v2, v3, v4);
        //}

        internal static void SetShaderUniform(int uniformLocation, Matrix2 v1) {
            Handler.SetShaderUniform(uniformLocation, v1);
        }

        internal static void SetShaderUniform(int uniformLocation, Matrix3 v1) {
            Handler.SetShaderUniform(uniformLocation, v1);
        }

        internal static void SetShaderUniform(int uniformLocation, Matrix4 v1) {
            Handler.SetShaderUniform(uniformLocation, v1);
        }

        internal static bool IsShaderBound(Shader shader) => Handler.IsShaderBound(shader);
        #endregion

        #region Textures
        internal static void InitializeTexture(Texture2D texture) => Handler.InitializeTexture(texture);

        internal static int BindTexture(Texture texture) => Handler.BindTexture(texture);

        internal static void BindTexture(Texture texture, int textureUnit) => Handler.BindTexture(texture, textureUnit);

        internal static void ReleaseTexture(int textureUnit) => Handler.ReleaseTexture(textureUnit);

        internal static void DisposeTexture(Texture texture) => Handler.DisposeTexture(texture);

        internal static void ReleaseTexture(Texture texture) => Handler.ReleaseTexture(texture);

        internal static void ActivateTexture(Texture texture) => Handler.ActivateTexture(texture);

        internal static void ActivateTextureUnit(int textureUnit) => Handler.ActivateTextureUnit(textureUnit);

        internal static Texture BoundTexture(int textureUnit) => Handler.BoundTexture(textureUnit);

        internal static int BoundToTextureUnit(Texture texture) => Handler.BoundToTextureUnit(texture);

        internal static void UpdateTextureWrapMode(Texture texture) => Handler.UpdateTextureWrapMode(texture);

        internal static void UpdateTextureFilterMode(Texture texture) => Handler.UpdateTextureFilterMode(texture);

        internal static bool IsTextureBound(Texture texture) => Handler.IsTextureBound(texture);

        internal static bool HasActiveTexture => Handler.HasActiveTexture;

        internal static Texture ActiveTexture => Handler.ActiveTexture;

        internal static int SupportedTextureUnits => Handler.SupportedTextureUnits;
        #endregion Textures

        internal static void ApplyTransformation(Transform transform) {
            Handler.ApplyTransformation(transform);
        }

        //internal static void ApplyTranslation(float dx, float dy) {
        //    Handler.ApplyTranslation(dx, dy);
        //}

        //internal static void ApplyRotation(float angle) {
        //    Handler.ApplyRotation(angle);
        //}

        //void ApplyRotation(float angle, float px, float py);

        //internal static void ApplyScaling(float sx, float sy) {
        //    Handler.ApplyScaling(sx, sy);
        //}

        internal static void RevertTransform() {
            Handler.RevertTransform();
        }

        internal static Matrix4 CurrentTransformationMatrix => Handler.CurrentTransformationMatrix;

        #region RenderSettings
        internal static BlendMode BlendMode {
            set => Handler.BlendMode = value;
        }

        internal static (BlendFunction source, BlendFunction destination)? BlendFunctions {
            get => Handler.BlendFunctions;
            set => Handler.BlendFunctions = value;
        }

        internal static DepthFunction? DepthFunction {
            get => Handler.DepthFunction;
            set => Handler.DepthFunction = value;
        }

        internal static AntiAliasMode AntiAliasMode {
            get => Handler.AntiAliasMode;
            set => Handler.AntiAliasMode = value;
        }

        internal static Color ClearColor {
            get => Handler.ClearColor;
            set => Handler.ClearColor = value;
        }

        internal static void SetClearModes(bool color, bool depth, bool stencil) => Handler.SetClearModes(color, depth, stencil);

        internal static bool EnableCulling {
            set => Handler.EnableCulling = value;
        }

        internal static bool ClockwiseCulling {
            set => Handler.ClockwiseCulling = value;
        }
        internal static void CullFaces(bool front, bool back) => Handler.CullFaces(front, back);

        internal static bool EnableTextures {
            set => Handler.EnableTextures = value;
        }
        internal static bool EnableBlending {
            set => Handler.EnableBlending = value;
        }

        internal static bool EnableDepthTest {
            set => Handler.EnableDepthTest = value;
        }

        internal static bool EnableEdgeAntialiasing {
            set => Handler.EnableEdgeAntialiasing = value;
        }

        internal static void EnableColors(bool red, bool green, bool blue, bool alpha) => Handler.EnableColors(red, green, blue, alpha);
        #endregion RenderSettings

        private static IGLHandler Handler {
            get {
                if (GLHandler.handler == null) {
                    FieldInfo glHandlerFI = ModBase.Instance.GetType().GetField("glHandler", BindingFlags.NonPublic | BindingFlags.Instance);
                    GLHandler.handler = (IGLHandler)glHandlerFI.GetValue(ModBase.Instance);
                }

                return GLHandler.handler;
            }
        }

        // TODO: cache created default shaders by texture count

        public const string DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME = "u_viewProjectionMatrix";
        public const string DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME = "u_modelMatrix";

        public static Shader CreateUntexturedPassthroughShader(bool createMatrixUniforms) {
            StringBuilder sb_vert = new StringBuilder();
            sb_vert.Append("#version 330 core\n\n");
            if (createMatrixUniforms) {
                sb_vert.Append("uniform mat4 " + DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME + " = mat4(1.0);\n");
                sb_vert.Append("uniform mat4 " + DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " = mat4(1.0);\n\n");
            }
            sb_vert.Append("layout(location = 0) in vec4 in_position;\n");
            sb_vert.Append("layout(location = 1) in vec4 in_color;\n\n");
            sb_vert.Append("out vec4 v_worldPos;\n");
            sb_vert.Append("out vec4 v_color;\n\n");
            sb_vert.Append("void main() {\n");
            sb_vert.Append("\tgl_Position = ");
            if (createMatrixUniforms) {
                sb_vert.Append(DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME + " * " + DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " * ");
            }
            sb_vert.Append("in_position;\n");
            sb_vert.Append("\tv_color = in_color;\n");
            sb_vert.Append("\tv_worldPos = ");
            if (createMatrixUniforms) {
                sb_vert.Append(DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " * ");
            }
            sb_vert.Append("in_position;\n");
            sb_vert.Append("}\n");

            StringBuilder sb_frag = new StringBuilder();
            sb_frag.Append("#version 330 core\n\n");
            sb_frag.Append("in vec4 v_worldPos;\n");
            sb_frag.Append("in vec4 v_color;\n\n");
            sb_frag.Append("layout(location = 0) out vec4 out_color;\n\n");
            sb_frag.Append("void main() {\n");
            sb_frag.Append("\tout_color = v_color;\n");
            sb_frag.Append("}\n");

            return new Shader(sb_vert.ToString(), sb_frag.ToString());
        }

        public static Shader CreateTexturedPassthroughShader(bool createMatrixUniforms, int textureCount) {
            int maxTU = GLHandler.SupportedTextureUnits;
            if (textureCount < 0 || textureCount > maxTU)
                throw new ArgumentException(textureCount + " Texture Units are not supported. " + maxTU + " are maximal supported.");

            if (textureCount == 0)
                return CreateUntexturedPassthroughShader(createMatrixUniforms);

            StringBuilder sb_vert = new StringBuilder();
            sb_vert.Append("#version 330 core\n\n");
            if (createMatrixUniforms) {
                sb_vert.Append("uniform mat4 " + DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME + " = mat4(1.0);\n");
                sb_vert.Append("uniform mat4 " + DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " = mat4(1.0);\n\n");
            }
            sb_vert.Append("layout(location = 0) in vec4 in_position;\n");
            sb_vert.Append("layout(location = 1) in vec4 in_color;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_vert.Append("layout(location = ").Append(2 + i).Append(") in vec2 in_texCoords").Append(i).Append(";\n");
            }
            sb_vert.Append("\n");
            sb_vert.Append("out vec4 v_worldPos;\n");
            sb_vert.Append("out vec4 v_color;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_vert.Append("out vec2 v_texCoords").Append(i).Append(";\n");
            }
            sb_vert.Append("\n");
            sb_vert.Append("void main() {\n");
            sb_vert.Append("\tgl_Position = ");
            if (createMatrixUniforms) {
                sb_vert.Append(DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME + " * " + DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " * ");
            }
            sb_vert.Append("in_position;\n");
            sb_vert.Append("\tv_color = in_color;\n");
            sb_vert.Append("\tv_worldPos = ");
            if (createMatrixUniforms) {
                sb_vert.Append(DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " * ");
            }
            sb_vert.Append("in_position;\n");
            sb_vert.Append("}\n");

            StringBuilder sb_frag = new StringBuilder();
            sb_frag.Append("#version 330 core\n\n");
            for (int i = 0; i < textureCount; i++) {
                sb_frag.Append("uniform sampler2D u_texture").Append(i).Append(";\n");
            }
            sb_frag.Append("\n");
            sb_frag.Append("in vec4 v_worldPos;\n");
            sb_frag.Append("in vec4 v_color;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_frag.Append("in vec2 in_texCoords").Append(i).Append(";\n");
            }
            sb_frag.Append("\n");
            sb_frag.Append("layout(location = 0) out vec4 out_color;\n\n");
            sb_frag.Append("void main() {\n");
            sb_frag.Append("\tout_color = v_color");
            for (int i = 0; i < textureCount; i++) {
                sb_frag.Append("\n\t\t* texture2D(u_texture").Append(i).Append(", v_texCoords").Append(i).Append(")");
            }
            sb_frag.Append(";\n");
            sb_frag.Append("}\n");

            return new Shader(sb_vert.ToString(), sb_frag.ToString());
        }
    }
}