using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Graphics.Textures;
using GameEngine.Graphics.Utility;

namespace GameEngine.Graphics {
    public sealed class Renderable : IDisposable {
        private readonly Shader shader;
        private readonly Mesh mesh;

        private readonly Texture[] textures;

        private int vaoID;
        private readonly VertexBufferObject vbo;
        private readonly IndexBufferObject ibo;

        private readonly Action<Shader, Shader.ShaderUniform> uniformSetter;

        public Renderable(Shader shader,
                            Mesh mesh,
                            Texture[] textures,
                            Func<VertexAttribute, IEnumerable<VertexAttribute>, VertexAttribute> vertexAttributeResolver,
                            Action<Shader, Shader.ShaderUniform> uniformSetter) {

            this.shader = shader;
            this.mesh = mesh;
            this.textures = textures;

            this.uniformSetter = uniformSetter;

            Dictionary<Shader.ShaderVertexAttribute, VertexAttribute> resolvedAttributes = new Dictionary<Shader.ShaderVertexAttribute, VertexAttribute>();
            foreach (Shader.ShaderVertexAttribute sva in this.shader.Attributes) {
                VertexAttribute resolvedAttribute = vertexAttributeResolver(sva, this.mesh.VertexAttributes);
                resolvedAttributes.Add(sva, resolvedAttribute);
            }

            float[] vertexData = this.mesh.GetInterleavedVertexData(resolvedAttributes.Values);

            GLHandler.InitializeVAO(this);

            this.vbo = new VertexBufferObject(vertexData.Length, BufferType.Static);
            this.vbo.Data = vertexData;

            this.ibo = new IndexBufferObject(this.mesh.Indices.Count(), BufferType.Dynamic);
            this.ibo.Data = this.mesh.Indices;

            GLHandler.BindVAO(this);
            this.shader.Bind();
            this.shader.EnableVertexAttributes();
            this.vbo.Bind();
            this.shader.AssignVertexAttributePointers();
            this.vbo.Release();
            this.shader.DisableVertexAttributes();
            this.shader.Release();
            GLHandler.ReleaseVAO(this);
        }

        ~Renderable() {
            Dispose();
        }

        private void PrepareRender() {
            // sync index data
            if (!this.ibo.Data.SequenceEqual(this.mesh.Indices))
                this.ibo.Data = this.mesh.Indices;

            this.ibo.PrepareRender();
            this.vbo.PrepareRender();
        }

        internal void Render() {
            PrepareRender();

            this.shader.Bind();
            GLHandler.BindVAO(this);
            this.shader.EnableVertexAttributes();

            foreach (Texture texture in this.textures) {
                texture.Bind();
                texture.Activate();
            }

            foreach (Shader.ShaderUniform uniform in this.shader.Uniforms) {
                this.uniformSetter(this.shader, uniform);
            }

            GLHandler.Render(this.ibo);

            foreach (Texture texture in this.textures) {
                texture.Release();
            }

            this.shader.DisableVertexAttributes();
            GLHandler.ReleaseVAO(this);
            this.shader.Release();
        }

        public void Dispose() {
            GLHandler.DeleteVAO(this);
            this.vbo.Dispose();
            this.ibo.Dispose();
        }

        public bool IsBound => GLHandler.IsVAOBound(this);

        public bool IsDisposed => this.vaoID <= 0;
    }
}