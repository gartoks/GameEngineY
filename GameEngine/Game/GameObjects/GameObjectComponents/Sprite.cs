using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;
using GameEngine.Utility;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public class Sprite : GOC {
        private const string ATTRIBUTE_NAME_POSITION = "in_position";
        private const string ATTRIBUTE_NAME_COLOR = "in_color";
        private const string ATTRIBUTE_NAME_TEXTURE_COORDINATES = "in_texCoords0";
        private const string UNIFORM_NAME_TEXTURE = "u_texture0";
        private const string UNIFORM_NAME_VIEWPROJECTIONMATRIX = "u_viewProjectionMatrix";
        private const string UNIFORM_NAME_MODEL_MATRIX = "u_modelMatrix";

        private Color color;
        private ITexture texture;

        public override void Initialize() {
            this.Initialize(new object[0]);
        }

        public override void Initialize(object[] parameters) {
            Color color = Color.WHITE;
            if (parameters.Length >= 1 && parameters[0] is Color c)
                color = c;

            ITexture tex;
            if (parameters.Length >= 2 && parameters[1] is ITexture t)
                tex = t;
            else
                tex = GraphicsHandler.CreateDefaultTexture(1, 1, Color.MAGENTA);

            IShader sh;
            if (parameters.Length >= 3 && parameters[2] is IShader s)
                sh = s;
            else
                sh = GraphicsHandler.CreateDefaultShader(1);

            CreateRenderable(ResolveAttributes, AssignUniforms, sh, BuildMesh());

            SetTexture(tex);
            Color = color;
        }

        public IShader Shader {
            get => Renderable.Shader;
            set {
                if (value == null)
                    return;

                if (value.Attributes.Count(attribute => attribute.Type == VertexAttributeType.FloatVector3 && attribute.Name == ATTRIBUTE_NAME_POSITION) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a vec3 attribute named '{ATTRIBUTE_NAME_POSITION}'.");
                    return;
                }

                if (value.Attributes.Count(attribute => attribute.Type == VertexAttributeType.FloatVector4 && attribute.Name == ATTRIBUTE_NAME_COLOR) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a vec4 attribute named '{ATTRIBUTE_NAME_COLOR}'.");
                    return;
                }

                if (value.Attributes.Count(attribute => attribute.Type == VertexAttributeType.FloatVector2 && attribute.Name == ATTRIBUTE_NAME_TEXTURE_COORDINATES) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a vec2 attribute named '{ATTRIBUTE_NAME_TEXTURE_COORDINATES}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Matrix4x4 && uniform.Name == UNIFORM_NAME_VIEWPROJECTIONMATRIX) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a mat4 uniform named '{UNIFORM_NAME_VIEWPROJECTIONMATRIX}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Matrix4x4 && uniform.Name == UNIFORM_NAME_MODEL_MATRIX) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a mat4 uniform named '{UNIFORM_NAME_MODEL_MATRIX}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Texture2D && uniform.Name == UNIFORM_NAME_TEXTURE) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a sampler2D uniform named '{UNIFORM_NAME_TEXTURE}'.");
                    return;
                }

                Renderable.Shader = value;
            }
        }

        public IMesh Mesh => Renderable.Mesh;

        public ITexture Texture {
            get {
                if (texture == null)
                    texture = GraphicsHandler.CreateDefaultTexture(1, 1, Color.MAGENTA);

                return texture;
            }
        }

        public void SetTexture(ITexture value, bool changeTextureCoordinates = true) {
            if (value == null)
                return;

            texture = value;

            if (changeTextureCoordinates) {
                IMesh mesh = Renderable.Mesh;
                int i = 0;
                for (int y = 0; y < 2; y++) {
                    for (int x = 0; x < 2; x++) {
                        IVertexData va = mesh.GetVertexData(i);
                        va.SetAttributeData("in_texCoords0", value.TextureCoordinates[x + y * 2]);
                        i++;
                    }
                }
            }
        }

        public Color Color {
            get => color;
            set {
                color = value;

                IMesh mesh = Renderable.Mesh;
                int i = 0;
                for (int y = 0; y < 2; y++) {
                    for (int x = 0; x < 2; x++) {
                        IVertexData va = mesh.GetVertexData(i);
                        va.SetAttributeData("in_color", value.ToArray(true));
                        i++;
                    }
                }
            }
        }


        private VertexAttribute ResolveAttributes(VertexAttribute shaderAttribute, IEnumerable<VertexAttribute> meshAttributes) {
            if (shaderAttribute.Name.Equals("in_position"))
                return meshAttributes.Single(n => n.Name.Equals("in_position"));
            if (shaderAttribute.Name.Equals("in_color"))
                return meshAttributes.Single(n => n.Name.Equals("in_color"));
            if (shaderAttribute.Name.Equals("in_texCoords0"))
                return meshAttributes.Single(n => n.Name.Equals("in_texCoords0"));

            throw new ArgumentException();
        }

        private void AssignUniforms(IShaderUniformAssigner uniformAssigner, IUniform shaderUniform) {
            if (shaderUniform.Name.Equals("u_texture0"))
                uniformAssigner.SetUniform("u_texture0", Texture);
            else if (shaderUniform.Name.Equals("u_viewProjectionMatrix"))
                uniformAssigner.SetUniform("u_viewProjectionMatrix", Scene.MainViewport.ViewProjectionMatrix);
            else if (shaderUniform.Name.Equals("u_modelMatrix"))
                uniformAssigner.SetUniform("u_modelMatrix", GraphicsHandler.CurrentTransformationMatrix);
        }

        private static IMesh BuildMesh() {
            VertexAttribute[] vertexAttributes = {
                new VertexAttribute("in_position", VertexAttributeType.FloatVector3),
                new VertexAttribute("in_color", VertexAttributeType.FloatVector4),
                new VertexAttribute("in_texCoords0", VertexAttributeType.FloatVector2)
            };

            (uint idx0, uint idx1, uint idx2)[] indices = {
                (0, 1, 2),
                (0, 2, 3)
            };

            IMesh mesh = GraphicsHandler.CreateMesh(4, vertexAttributes, indices);

            mesh.GetVertexData(0).SetAttributeData("in_position", -0.5f, 0.5f, 0);      // top-left
            mesh.GetVertexData(1).SetAttributeData("in_position", 0.5f, 0.5f, 0);       // top-right
            mesh.GetVertexData(2).SetAttributeData("in_position", 0.5f, -0.5f, 0);      // bottom-right
            mesh.GetVertexData(3).SetAttributeData("in_position", -0.5f, -0.5f, 0);     // bottom-left

            return mesh;
        }
    }
}