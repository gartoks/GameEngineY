using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameApp.Files;
using GameApp.Graphics.Textures;
using GameApp.Graphics.Utility;
using GameApp.Resources;
using GameEngine.Graphics;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Utility;
using GameEngine.Math;
using GameEngine.Resources.Loaders;
using Color = GameEngine.Utility.Color;

namespace GameApp.Graphics {
    internal sealed class GraphicsHandler : IGraphicsHandler {
        private static GraphicsHandler instance;
        internal static GraphicsHandler Instance {
            get => GraphicsHandler.instance;
            private set { if (GraphicsHandler.instance != null) throw new InvalidOperationException("Only one instance per manager type permitted."); else instance = value; }
        }

        public GraphicsHandler() {
            Instance = this;
        }

        internal void Install() { }

        internal bool VerifyInstallation() => true;

        internal void Initialize() {
            // load default shaders

            ResourceManager.Instance.LoadResource<IShader, ShaderLoadingParameters>(
                "internal_circleShader",
                new ShaderLoadingParameters(new [] {
                    Path.Combine(FileManager.Instance.ExecutablePath, "Graphics", "Shaders", "EllipseShader.vs"),
                    Path.Combine(FileManager.Instance.ExecutablePath, "Graphics", "Shaders", "EllipseShader.fs")
                }),
                0, true);
        }

        public Matrix4 CurrentTransformationMatrix => GLHandler.Instance.CurrentTransformationMatrix;

        internal Mesh CreateMesh(int vertexCount, VertexAttribute[] vertexAttributes, (uint idx0, uint idx1, uint idx2)[] clockwiseTriangles) {
            return new Mesh(vertexCount, vertexAttributes, clockwiseTriangles);
        }

        IMesh IGraphicsHandler.CreateMesh(int vertexCount, VertexAttribute[] vertexAttributes, (uint idx0, uint idx1, uint idx2)[] clockwiseTriangles) => CreateMesh(vertexCount, vertexAttributes, clockwiseTriangles);

        internal Mesh CreateDefaultMesh() {  // TODO maybe cache default mesh, but then one could modify its values
            VertexAttribute va_position = new VertexAttribute("in_position", VertexAttributeType.FloatVector3);
            VertexAttribute va_color = new VertexAttribute("in_color", VertexAttributeType.FloatVector3);
            VertexAttribute va_texCoords = new VertexAttribute("in_texCoords0", VertexAttributeType.FloatVector2);
            VertexAttribute[] vertexAtributes = { va_position, va_color, va_texCoords };

            (uint idx0, uint idx1, uint idx2)[] indices = {
                (0, 1, 2),
                (2, 1, 3)
            };

            float[][] textureCoordinates = Texture.GetDefaultTextureCoordinates();

            Mesh mesh = CreateMesh(4, vertexAtributes, indices);
            int i = 0;
            for (int y = 0; y < 2; y++) {
                for (int x = 0; x < 2; x++) {
                    VertexData va = mesh.GetVertexData(i);
                    va.SetAttributeData(va_position, -0.5f + x, -0.5f + y, 0);
                    va.SetAttributeData(va_color, Color.WHITE.ToArray(false));
                    va.SetAttributeData(va_texCoords, textureCoordinates[x + y * 2]);
                    i++;
                }
            }

            return mesh;
        }

        IMesh IGraphicsHandler.CreateDefaultMesh() => CreateDefaultMesh();

        internal Texture2D CreateTexture(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter) {
            Bitmap bmp = new Bitmap(bitmap);

            int textureID;
            if (!Thread.CurrentThread.Equals(Application.Application.Instance.RenderThread)) {
                Task<int> task = new Task<int>(() => {
                    GLHandler.Instance.InitializeTexture(bmp, wrapS, wrapT, minFiler, magFilter, out int texID);
                    return texID;
                });
                GLHandler.Instance.Queue(task);
                task.Wait();
                textureID = task.Result;
            } else {
                GLHandler.Instance.InitializeTexture(bmp, wrapS, wrapT, minFiler, magFilter, out int texID);
                textureID = texID;
            }


            return new Texture2D(bmp, wrapS, wrapT, minFiler, magFilter, textureID);
        }

        ITexture IGraphicsHandler.CreateTexture(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter) {
            return CreateTexture(bitmap, wrapS, wrapT, minFiler, magFilter);
        }

        public Texture CreateDefaultTexture(int width, int height, Color color) {
            Bitmap bmp = new Bitmap(width, height);
            System.Drawing.Color c = color.ToWinColor;;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    bmp.SetPixel(x, y, c);
                }
            }

            return CreateTexture(bmp, TextureWrapMode.Repeat, TextureWrapMode.Repeat, TextureFilterMode.Nearest, TextureFilterMode.Nearest);
        }

        ITexture IGraphicsHandler.CreateDefaultTexture(int width, int height, Color color) => CreateDefaultTexture(width, height, color);

        internal void DisposeTexture(Texture2D texture) {
            Task task = new Task(() => {
                GLHandler.Instance.DisposeTexture(texture);
            });
            GLHandler.Instance.Queue(task);
            task.Wait();
        }

        internal TextureAtlas CreateTextureAtlas(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter, IEnumerable<(string regionName, int x, int y, int width, int height)> regionData) {
            Bitmap bmp = new Bitmap(bitmap);

            Task<int> task = new Task<int>(() => {
                GLHandler.Instance.InitializeTexture(bmp, wrapS, wrapT, minFiler, magFilter, out int texID);
                return texID;
            });
            GLHandler.Instance.Queue(task);
            task.Wait();

            int textureID = task.Result;

            TextureAtlas atlas = new TextureAtlas(bmp, wrapS, wrapT, minFiler, magFilter, textureID);
            foreach ((string regionName, int x, int y, int width, int height) regionDate in regionData) {
                atlas.SetRegion(regionDate.regionName, regionDate.x, regionDate.y, regionDate.width, regionDate.height);
            }

            return atlas;
        }

        ITextureAtlas IGraphicsHandler.CreateTextureAtlas(Bitmap bitmap, TextureWrapMode wrapS, TextureWrapMode wrapT, TextureFilterMode minFiler, TextureFilterMode magFilter, IEnumerable<(string regionName, int x, int y, int width, int height)> regionData) {
            return CreateTextureAtlas(bitmap, wrapS, wrapT, minFiler, magFilter, regionData);
        }

        internal Shader CreateShader(string vertexShaderSource, string fragmentShaderSource) {
            Task<Shader> task = new Task<Shader>(() => GLHandler.Instance.CreateShader(vertexShaderSource, fragmentShaderSource));
            GLHandler.Instance.Queue(task);
            task.Wait();

            return task.Result;
        }

        IShader IGraphicsHandler.CreateShader(string vertexShaderSource, string fragmentShaderSource) {
            return CreateShader(vertexShaderSource, fragmentShaderSource);
        }

        internal void DisposeShader(Shader shader) {
            Task task = new Task(() => {
                GLHandler.Instance.DeleteShader(shader);
            });
            GLHandler.Instance.Queue(task);
            task.Wait();
        }

        internal Renderable CreateRenderable(ShaderVertexAttributeResolver attributeResolver, ShaderUniformAssignmentHandler shaderUniformAssignmentHandler, Shader shader, Mesh mesh) {
            return new Renderable(attributeResolver, shaderUniformAssignmentHandler, shader, mesh);
        }

        IRenderable IGraphicsHandler.CreateRenderable(ShaderVertexAttributeResolver attributeResolver, ShaderUniformAssignmentHandler shaderUniformAssignmentHandler, IShader shader, IMesh mesh) {
            return CreateRenderable(attributeResolver, shaderUniformAssignmentHandler, (Shader)shader, (Mesh)mesh);
        }

        internal void DisposeRenderable(Renderable renderable) {
            Task task = new Task(() => {
                GLHandler.Instance.DeleteIBO(renderable.IndexBufferObject);
                GLHandler.Instance.DeleteVBO(renderable.VertexBufferObject);
            });
            GLHandler.Instance.Queue(task);
            task.Wait();
        }

        IShader IGraphicsHandler.CreateDefaultShader(int textureCount) => CreateDefaultShader(textureCount);

        Shader CreateDefaultShader(int textureCount) {
            Task<(string vS, string fS)> task = new Task<(string, string)>(() => {
                string vS, fS;
                if (textureCount == 0)
                    CreateUntexturedPassthroughShader(out vS, out fS);
                else
                    CreateTexturedPassthroughShader(textureCount, out vS, out fS);

                return (vS, fS);
            });
            GLHandler.Instance.Queue(task);
            task.Wait();

            return CreateShader(task.Result.vS, task.Result.fS);
        }

        public const string DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME = "u_viewProjectionMatrix";
        public const string DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME = "u_modelMatrix";

        private static void CreateUntexturedPassthroughShader(out string vertexShaderSource, out string fragmentShaderSource) {
            StringBuilder sb_vert = new StringBuilder();
            sb_vert.Append("#version 330 core\n\n");
            sb_vert.Append("uniform mat4 " + DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME + " = mat4(1.0);\n");
            sb_vert.Append("uniform mat4 " + DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME + " = mat4(1.0);\n\n");
            int layoutLocation = 0;
            sb_vert.Append($"layout(location = {layoutLocation++}) in vec3 in_position;\n");
            sb_vert.Append("void main() {\n");
            sb_vert.Append("\tvec4 pos = vec4(in_position, 1.0);\n");
            sb_vert.Append($"\tgl_Position = {DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME} * {DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME} * pos;\n");
            sb_vert.Append("}\n");

            layoutLocation = 0;
            StringBuilder sb_frag = new StringBuilder();
            sb_frag.Append("#version 330 core\n\n");
            sb_frag.Append($"layout(location = {layoutLocation++}) out vec4 out_color;\n\n");
            sb_frag.Append("void main() {\n");
            sb_frag.Append("\tout_color = vec4(1.0, 1.0, 1.0, 1.0);\n");
            sb_frag.Append("}\n");

            vertexShaderSource = sb_vert.ToString();
            fragmentShaderSource = sb_frag.ToString();
        }

        private static void CreateTexturedPassthroughShader(int textureCount, out string vertexShaderSource, out string fragmentShaderSource) {
            int maxTU = GLHandler.Instance.SupportedTextureUnits;
            if (textureCount <= 0 || textureCount > maxTU)
                throw new ArgumentException(textureCount + " Texture Units are not supported. " + maxTU + " are maximal supported.");

            StringBuilder sb_vert = new StringBuilder();
            sb_vert.Append("#version 330 core\n\n");
            sb_vert.Append($"uniform mat4 {DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME} = mat4(1.0);\n");
            sb_vert.Append($"uniform mat4 {DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME} = mat4(1.0);\n\n");
            int layoutLocation = 0;
            sb_vert.Append($"layout(location = {layoutLocation++}) in vec3 in_position;\n");
            sb_vert.Append($"layout(location = {layoutLocation++}) in vec4 in_color;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_vert.Append($"layout(location = {layoutLocation++}) in vec2 in_texCoords{i};\n");
            }
            sb_vert.Append("\n");
            sb_vert.Append("out vec4 v_worldPos;\n");
            sb_vert.Append("out vec4 v_color;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_vert.Append($"out vec2 v_texCoords{i};\n");
            }
            sb_vert.Append("\n");
            sb_vert.Append("void main() {\n");
            sb_vert.Append("\tvec4 pos = vec4(in_position, 1.0);\n");
            sb_vert.Append($"\tgl_Position = {DEFAULT_SHADER_VIEWPROJECTIONMATRIX_UNIFORM_NAME} * {DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME} * pos;\n");
            sb_vert.Append("\tv_color = in_color;\n");
            sb_vert.Append($"\tv_worldPos = {DEFAULT_SHADER_MODELMATRIX_UNIFORM_NAME} * pos;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_vert.Append($"\tv_texCoords{i} = in_texCoords{i};\n");
            }
            sb_vert.Append("}\n");

            layoutLocation = 0;
            StringBuilder sb_frag = new StringBuilder();
            sb_frag.Append("#version 330 core\n\n");
            for (int i = 0; i < textureCount; i++) {
                sb_frag.Append($"uniform sampler2D u_texture{i};\n");
            }
            sb_frag.Append("\n");
            sb_frag.Append("in vec4 v_worldPos;\n");
            sb_frag.Append("in vec4 v_color;\n");
            for (int i = 0; i < textureCount; i++) {
                sb_frag.Append($"in vec2 v_texCoords{i};\n");
            }
            sb_frag.Append("\n");
            sb_frag.Append($"layout(location = {layoutLocation++}) out vec4 out_color;\n\n");
            sb_frag.Append("void main() {\n");
            sb_frag.Append("\tout_color = v_color");
            for (int i = 0; i < textureCount; i++) {
                sb_frag.Append($" * texture(u_texture{i}, v_texCoords{i})");
            }
            sb_frag.Append(";\n");
            sb_frag.Append("}\n");

            vertexShaderSource = sb_vert.ToString();
            fragmentShaderSource = sb_frag.ToString();
        }
    }
}