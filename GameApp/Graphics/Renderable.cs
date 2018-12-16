using System.Collections.Generic;
using System.Linq;
using GameApp.Graphics.Buffers;
using GameApp.Graphics.Textures;
using GameApp.Graphics.Utility;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;

#pragma warning disable 649

namespace GameApp.Graphics {
    internal sealed class Renderable : IRenderable {
        private readonly ShaderVertexAttributeResolver attributeResolver;
        private readonly ShaderUniformAssignmentHandler shaderUniformAssignmentHandler;

        private Shader shader;
        private Mesh mesh;

        private VertexBufferObject vbo;
        private IndexBufferObject ibo;

        private readonly Dictionary<ShaderVertexAttribute, VertexAttribute> resolvedMeshAttributes;

        private short isValid;

        private bool isMeshDirty;
        private bool isShaderDirty;
        private bool isDataDirty;

        private readonly ShaderUniformAssigner uniformAssigner;

        internal Renderable(ShaderVertexAttributeResolver attributeResolver, ShaderUniformAssignmentHandler shaderUniformAssignmentHandler, Shader shader, Mesh mesh) {
            if (attributeResolver == null) {
                Log.WriteLine($"Cannot create renderable, attributeResolver is null.");
                return;
            }

            if (shaderUniformAssignmentHandler == null) {
                Log.WriteLine($"Cannot create renderable, shaderUniformAssignmentHandler is null.");
                return;
            }

            this.resolvedMeshAttributes = new Dictionary<ShaderVertexAttribute, VertexAttribute>();

            this.uniformAssigner = new ShaderUniformAssigner();

            this.attributeResolver = attributeResolver;
            this.shaderUniformAssignmentHandler = shaderUniformAssignmentHandler;

            Shader = shader;
            Mesh = mesh;
        }

        ~Renderable() {
            GraphicsHandler.Instance.DisposeRenderable(this);
        }

        public void Render() {
            if (isValid < 0)
                CheckValidity();

            if (isValid > 0) {
                Log.WriteLine("Cannot render. Renderable is not valid.", LogType.Warning);
                return;
            }

            //if (shader.Uniforms.Count(uniform => uniform.Type == UniformType.Texture2D) != textures.Length) {
            //    Log.WriteLine("Cannot render. Texture count does not match uniform texture count in shader.");
            //    return;
            //}

            Clean();

            shader.Bind();
            shader.EnableVertexAttributes();

            this.uniformAssigner.AssignUniforms(this.shader, this.shaderUniformAssignmentHandler);

            vbo.Bind();
            shader.AssignVertexAttributePointers();
            vbo.Release();

            GLHandler.Instance.Render(ibo);

            foreach (Texture textureUniform in this.uniformAssigner.TextureUniforms)
                textureUniform.Release();

            shader.DisableVertexAttributes();
            shader.Release();
        }

        public Shader Shader {
            get => this.shader;
            set {
                if (value == null) {
                    Log.WriteLine($"Cannot set shader, shader is null.");
                    return;
                }

                if (!value.IsCompiled) {
                    Log.WriteLine($"Cannot set shader, shader is not compiled.");
                    return;
                }

                this.shader = value;
                this.isShaderDirty = true;
                this.isValid = -1;
            }
        }

        IShader IRenderable.Shader {
            get => Shader;
            set => Shader = (Shader)value;
        }

        public Mesh Mesh {
            get => this.mesh;
            set {
                if (value == null) {
                    Log.WriteLine($"Cannot set mesh, mesh is null.");
                    return;
                }

                if (this.mesh != null)
                    this.mesh.OnMeshVertexDataChanged -= OnMeshVertexDataChanged;

                this.mesh = value;
                this.mesh.OnMeshVertexDataChanged += OnMeshVertexDataChanged;
                this.isMeshDirty = true;
                this.isValid = -1;
            }
        }

        IMesh IRenderable.Mesh {
            get => Mesh;
            set => Mesh = (Mesh)value;
        }

        internal VertexBufferObject VertexBufferObject => this.vbo;

        internal IndexBufferObject IndexBufferObject => this.ibo;

        private void Clean() {
            if (isMeshDirty) {
                GLHandler.Instance.DeleteVBO(vbo);
                GLHandler.Instance.DeleteIBO(ibo);

                this.resolvedMeshAttributes.Clear();
                foreach (ShaderVertexAttribute shaderAttribute in shader.Attributes) {
                    VertexAttribute resolvedMeshAttribute = attributeResolver(shaderAttribute, mesh.VertexAttributes);

                    if (resolvedMeshAttribute != null)
                        resolvedMeshAttributes.Add(shaderAttribute, resolvedMeshAttribute);
                }

                vbo = GLHandler.Instance.CreateVBO(this.mesh.GetInterleavedVertexData(resolvedMeshAttributes.Values), BufferType.Dynamic);
                ibo = GLHandler.Instance.CreateIBO(mesh.Indices, BufferType.Static);

                this.isMeshDirty = false;
                this.isShaderDirty = false;
                this.isDataDirty = false;
            } else if (isShaderDirty) {
                this.resolvedMeshAttributes.Clear();
                foreach (ShaderVertexAttribute shaderAttribute in this.shader.Attributes) {
                    VertexAttribute resolvedMeshAttribute = this.attributeResolver(shaderAttribute, this.mesh.VertexAttributes);

                    if (resolvedMeshAttribute != null)
                        resolvedMeshAttributes.Add(shaderAttribute, resolvedMeshAttribute);
                }

                vbo.Data = this.mesh.GetInterleavedVertexData(resolvedMeshAttributes.Values);

                this.isShaderDirty = false;
                this.isDataDirty = false;
            } else if (isDataDirty) {
                vbo.Data = this.mesh.GetInterleavedVertexData(resolvedMeshAttributes.Values);

                this.isDataDirty = false;
            }
        }

        private void CheckValidity() {
            this.isValid = 1;

            if (shader == null) {
                Log.WriteLine("Renderable not valid. Shader missing.");
                return;
            }

            if (mesh == null) {
                Log.WriteLine("Renderable not valid. Mesh missing.");
                return;
            }

            this.isValid = 0;
        }


        private void OnMeshVertexDataChanged(Mesh mesh) {
            this.isDataDirty = true;
        }
    }
}