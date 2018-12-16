using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Logging;

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public class CustomSprite : GOC {
        private const string ATTRIBUTE_NAME_POSITION = "in_position";
        private const string UNIFORM_NAME_VIEWPROJECTIONMATRIX = "u_viewProjectionMatrix";
        private const string UNIFORM_NAME_MODEL_MATRIX = "u_modelMatrix";

        private readonly List<(VertexAttribute vertexAttribute, float[][] data)> additionalVertexData = new List<(VertexAttribute vertexAttribute, float[][] data)>();

        private Action<IShaderUniformAssigner, IUniform> additionalUniformAssigner;

        public override void Initialize() {
            this.Initialize(new object[0]);
        }

        public override void Initialize(object[] parameters) {
            IShader sh;
            if (parameters.Length >= 1 && parameters[0] is IShader s)
                sh = s;
            else
                sh = GraphicsHandler.CreateDefaultShader();

            if (parameters.Length >= 2 && parameters[1] is IEnumerable<(VertexAttribute vertexAttribute, float[][] data)> aVD && aVD.All(tuple => tuple.data.Length == 4))
                this.additionalVertexData.AddRange(aVD);

            AdditionalUniformAssigner = null;
            if (parameters.Length >= 3 && parameters[2] is Action<IShaderUniformAssigner, IUniform> aua)
                AdditionalUniformAssigner = aua;

            CreateRenderable(ResolveAttributes, AssignUniforms, sh, BuildMesh());
            Shader = sh;
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

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Matrix4x4 && uniform.Name == UNIFORM_NAME_VIEWPROJECTIONMATRIX) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a mat4 uniform named '{UNIFORM_NAME_VIEWPROJECTIONMATRIX}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Matrix4x4 && uniform.Name == UNIFORM_NAME_MODEL_MATRIX) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a mat4 uniform named '{UNIFORM_NAME_MODEL_MATRIX}'.");
                    return;
                }

                Renderable.Shader = value;
            }
        }

        public IMesh Mesh => Renderable.Mesh;

        public Action<IShaderUniformAssigner, IUniform> AdditionalUniformAssigner {
            private get => this.additionalUniformAssigner;
            set {
                if (value == null)
                    value = (assigner, uniform) => { };

                this.additionalUniformAssigner = value;
            }
        }

        public IEnumerable<(VertexAttribute vertexAttribute, float[][] data)> AdditionalVertexData {
            set {
                if (value == null) {
                    Log.WriteLine("Additional vertex data for CustomSprite is null.", LogType.Error);
                    return;
                }

                if (value.Any(tuple => tuple.data.Length != 4)) {
                    Log.WriteLine("Additional vertex data for CustomSprite must have 4 entries (one for each vertex).", LogType.Error);
                    return;
                }

                this.additionalVertexData.Clear();
                this.additionalVertexData.AddRange(value);

                Renderable.Mesh = BuildMesh();
            }
        }

        private VertexAttribute ResolveAttributes(VertexAttribute shaderAttribute, IEnumerable<VertexAttribute> meshAttributes) {
            return meshAttributes.FirstOrDefault(n => n.Name.Equals(shaderAttribute.Name));
        }

        private void AssignUniforms(IShaderUniformAssigner uniformAssigner, IUniform shaderUniform) {
            if (shaderUniform.Name.Equals("u_viewProjectionMatrix"))
                uniformAssigner.SetUniform(shaderUniform.Name, Scene.MainViewport.ViewProjectionMatrix);
            else if (shaderUniform.Name.Equals("u_modelMatrix"))
                uniformAssigner.SetUniform(shaderUniform.Name, GraphicsHandler.CurrentTransformationMatrix);
            else
                AdditionalUniformAssigner(uniformAssigner, shaderUniform);

        }

        private IMesh BuildMesh() {
            List<VertexAttribute> vertexAttributes = new List<VertexAttribute>();
            vertexAttributes.Add(new VertexAttribute("in_position", VertexAttributeType.FloatVector3));
            vertexAttributes.AddRange(additionalVertexData.Select(tuple => tuple.vertexAttribute));

            (uint idx0, uint idx1, uint idx2)[] indices = {
                (0, 1, 2),
                (0, 2, 3)
            };

            IMesh mesh = GraphicsHandler.CreateMesh(4, vertexAttributes.ToArray(), indices);

            mesh.GetVertexData(0).SetAttributeData("in_position", -0.5f, 0.5f, 0);      // top-left
            mesh.GetVertexData(1).SetAttributeData("in_position", 0.5f, 0.5f, 0);       // top-right
            mesh.GetVertexData(2).SetAttributeData("in_position", 0.5f, -0.5f, 0);      // bottom-right
            mesh.GetVertexData(3).SetAttributeData("in_position", -0.5f, -0.5f, 0);     // bottom-left

            for (int i = 0; i < 4; i++) {
                IVertexData vertexData = mesh.GetVertexData(i);

                foreach ((VertexAttribute vertexAttribute, float[][] data) vertexAttributeData in additionalVertexData) {
                    vertexData.SetAttributeData(vertexAttributeData.vertexAttribute, vertexAttributeData.data[i]);
                }
            }

            return mesh;
        }
    }
}