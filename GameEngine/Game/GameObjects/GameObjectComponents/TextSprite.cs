using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GameEngine.Game.Utility.UserInterface;
using GameEngine.Graphics;
using GameEngine.Graphics.Utility;
using GameEngine.Input;
using GameEngine.Logging;
using GameEngine.Math;
using GameEngine.Resources;
using GameEngine.Resources.Loaders;
using Color = GameEngine.Utility.Color;
using Rectangle = GameEngine.Math.Shapes.Rectangle;

// ReSharper disable OperatorIsCanBeUsed
// ReSharper disable LocalVariableHidesMember

namespace GameEngine.Game.GameObjects.GameObjectComponents {
    public class TextSprite : GOC {
        private const string ATTRIBUTE_NAME_POSITION = "in_position";
        private const string ATTRIBUTE_NAME_COLOR = "in_color";
        private const string ATTRIBUTE_NAME_TEXTURE_COORDINATES = "in_texCoords0";
        private const string UNIFORM_NAME_TEXTURE = "u_texture0";
        private const string UNIFORM_NAME_VIEWPROJECTIONMATRIX = "u_viewProjectionMatrix";
        private const string UNIFORM_NAME_MODEL_MATRIX = "u_modelMatrix";

        private Color color;
        private string text;
        private int textureHeight;
        private FontFamily font;
        private GUIDock dock;
        private Rectangle bounds;


        private ITextureAtlas texture;

        private bool noText;

        public override void Initialize() {
            this.text = "<TEXT>";
            this.textureHeight = 128;
            this.font = new FontFamily("Consolas");
            this.color = Color.WHITE;
            this.bounds = new Rectangle(0, 0, 1, 1);
            this.bounds.OnChange += rectangle => RebuildMesh();
            this.dock = GUIDock.Centered;

            this.noText = false;

            LoadFontAtlas();

            CreateRenderable(ResolveAttributes, AssignUniforms, GraphicsHandler.CreateDefaultShader(1), BuildMesh());
        }

        //protected override void Update() {
        //    if (InputHandler.IsKeyReleased(Key.Keypad7))
        //        Dock = GUIDock.TopLeft;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad8))
        //        Dock = GUIDock.TopCenter;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad9))
        //        Dock = GUIDock.TopRight;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad4))
        //        Dock = GUIDock.LeftCenter;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad5))
        //        Dock = GUIDock.Centered;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad6))
        //        Dock = GUIDock.RightCenter;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad1))
        //        Dock = GUIDock.BottomLeft;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad2))
        //        Dock = GUIDock.BottomCenter;
        //    else if (InputHandler.IsKeyReleased(Key.Keypad3))
        //        Dock = GUIDock.BottomRight;
        //    else if (InputHandler.IsKeyReleased(Key.Space))
        //        Log.WriteLine("tS" + bounds.ToString(), LogType.Debug);
        //}

        public override void Initialize(object[] parameters) {
            if (parameters.Length >= 1 && parameters[0] is string txt)
                this.text = txt;
            else
                this.text = "<TEXT>";

            if (this.text.Length == 0) {
                this.text = " ";
                this.noText = true;
            }

            if (parameters.Length >= 2 && parameters[1] is ushort texHeight)
                this.textureHeight = texHeight;
            else
                this.textureHeight = 128;

            if (parameters.Length >= 3 && parameters[2] is FontFamily fnt)
                this.font = fnt;
            else
                this.font = new FontFamily("Consolas");

            if (parameters.Length >= 4 && parameters[3] is Color c)
                this.color = c;
            else
                this.color = Color.WHITE;

            if (parameters.Length >= 5 && parameters[4] is Rectangle r)
                this.bounds = new Rectangle(r);
            else
                this.bounds = new Rectangle(0, 0, 1, 1);
            this.bounds.OnChange += rectangle => RebuildMesh();

            IShader s;
            if (parameters.Length >= 6 && parameters[5] is IShader sh)
                s = sh;
            else
                s = GraphicsHandler.CreateDefaultShader(1);

            LoadFontAtlas();

            CreateRenderable(ResolveAttributes, AssignUniforms, s, BuildMesh());
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

                if (value.Attributes.Count(uniform => uniform.Type == VertexAttributeType.FloatVector4 && uniform.Name == ATTRIBUTE_NAME_COLOR) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a vec4 attribute named '{ATTRIBUTE_NAME_COLOR}'.");
                    return;
                }

                if (value.Attributes.Count(uniform => uniform.Type == VertexAttributeType.FloatVector2 && uniform.Name == ATTRIBUTE_NAME_COLOR) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a vec2 attribute named '{ATTRIBUTE_NAME_TEXTURE_COORDINATES}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Matrix4x4 && uniform.Name == UNIFORM_NAME_VIEWPROJECTIONMATRIX) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a mat4 attribute named '{UNIFORM_NAME_VIEWPROJECTIONMATRIX}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Matrix4x4 && uniform.Name == UNIFORM_NAME_MODEL_MATRIX) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a mat4 attribute named '{UNIFORM_NAME_MODEL_MATRIX}'.");
                    return;
                }

                if (value.Uniforms.Count(uniform => uniform.Type == UniformType.Texture2D && uniform.Name == UNIFORM_NAME_TEXTURE) != 1) {
                    Log.WriteLine($"Cannot set sprite shader. Must have a sampler2D attribute named '{UNIFORM_NAME_TEXTURE}'.");
                    return;
                }

                Renderable.Shader = value;
            }
        }

        public string Text {
            get => noText ? "" : this.text;
            set {
                if (value == null)
                    value = "";

                if (value.Equals(text))
                    return;

                noText = false;

                if (value.Length == 0) {
                    value = " ";
                    noText = true;
                }

                this.text = value;
                RebuildMesh();
            }
        }

        public FontFamily Font {
            get => this.font;
            set {
                if (value == null)
                    value = new FontFamily("Consolas");

                this.font = value;

                LoadFontAtlas();

                UpdateTextureCoordinates();
            }
        }

        public ushort TextureHeight {
            get => (ushort)this.textureHeight;
            set {
                if (textureHeight <= FontLoadingParameters.MIN_FONT_TEXTURE_HEIGHT) {
                    Log.WriteLine($"A font texture height must be at least {FontLoadingParameters.MIN_FONT_TEXTURE_HEIGHT}.");
                }

                this.textureHeight = value;

                LoadFontAtlas();

                UpdateTextureCoordinates();
            }
        }

        public Color Color {
            get => color;
            set {
                color = value;

                UpdateColor();
            }
        }

        public Vector2 Offset {
            get => this.bounds.Position;
            set => this.bounds.Position = value;
        }

        public Vector2 Size {
            get => this.bounds.Size;
            set => this.bounds.Size = value;
        }

        public GUIDock Dock {
            get => this.dock;
            set {
                this.dock = value;
                RebuildMesh();
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
                uniformAssigner.SetUniform("u_texture0", texture);
            else if (shaderUniform.Name.Equals("u_viewProjectionMatrix"))
                uniformAssigner.SetUniform("u_viewProjectionMatrix", Scene.MainViewport.ViewProjectionMatrix);
            else if (shaderUniform.Name.Equals("u_modelMatrix"))
                uniformAssigner.SetUniform("u_modelMatrix", GraphicsHandler.CurrentTransformationMatrix);
        }

        private void LoadFontAtlas() {
            if (!ResourceManager.TryGetResource($"font_{font.Name}_{textureHeight}", out texture, false)) {
                ResourceManager.LoadResource<ITextureAtlas, FontLoadingParameters>($"font_{font.Name}_{textureHeight}", new FontLoadingParameters(font, textureHeight), 0, true);
                ResourceManager.TryGetResource($"font_{font.Name}_{textureHeight}", out texture);
            }
        }

        private void UpdateColor() {
            IMesh mesh = Renderable.Mesh;
            for (int i = 0; i < this.text.Length; i++) {
                mesh.GetVertexData(0 + i * 4).SetAttributeData("in_color", Color.ToArray(true));      // top-left
                mesh.GetVertexData(1 + i * 4).SetAttributeData("in_color", Color.ToArray(true));       // top-right
                mesh.GetVertexData(2 + i * 4).SetAttributeData("in_color", Color.ToArray(true));      // bottom-right
                mesh.GetVertexData(3 + i * 4).SetAttributeData("in_color", Color.ToArray(true));     // bottom-left
            }
        }

        private void UpdateTextureCoordinates() {
            IMesh mesh = Renderable.Mesh;

            for (int i = 0; i < this.text.Length; i++) {
                char glyph = this.text[i];
                ITexture glyphTexture = texture.GetSubTexture(glyph.ToString());

                float[][] texCoords = glyphTexture.TextureCoordinates;
                for (int j = 0; j < 4; j++)
                    mesh.GetVertexData(j + i * 4).SetAttributeData("in_texCoords0", texCoords[j]);
            }
        }

        private void RebuildMesh() {
            Renderable.Mesh = BuildMesh();
        }

        private IMesh BuildMesh() {
            VertexAttribute[] vertexAttributes = {
                new VertexAttribute("in_position", VertexAttributeType.FloatVector3),
                new VertexAttribute("in_color", VertexAttributeType.FloatVector4),
                new VertexAttribute("in_texCoords0", VertexAttributeType.FloatVector2)
            };

            int vertexCount = this.text.Length * 4;
            int triangleCount = 2 * this.text.Length;

            (uint idx0, uint idx1, uint idx2)[] triangles = new(uint idx0, uint idx1, uint idx2)[triangleCount];
            for (uint i = 0; i < triangleCount; i += 2) {
                triangles[i] = (0 + i * 2, 1 + i * 2, 2 + i * 2);
                triangles[i + 1] = (0 + i * 2, 2 + i * 2, 3 + i * 2);
            }

            IMesh mesh = GraphicsHandler.CreateMesh(vertexCount, vertexAttributes, triangles);

            int totalTextureWidth = 0;
            int textureHeight = 0;
            for (int i = 0; i < this.text.Length; i++) {
                char glyph = this.text[i];

                ITexture glyphTexture = texture.GetSubTexture(glyph.ToString());

                totalTextureWidth += glyphTexture.Width;
                textureHeight = System.Math.Max(textureHeight, glyphTexture.Height);
            }

            float scale = bounds.Width / totalTextureWidth;
            bool widthDominated = true;
            if (scale * textureHeight > bounds.Height) {
                scale = bounds.Height / textureHeight;
                widthDominated = false;
            }

            float totalVertexWidth = widthDominated ? bounds.Width : scale * totalTextureWidth;
            float currentVertexXOffset = -totalVertexWidth / 2f;
            if (dock == GUIDock.LeftCenter || dock == GUIDock.BottomLeft || dock == GUIDock.TopLeft)
                currentVertexXOffset = -bounds.Width / 2f;
            else if (dock == GUIDock.RightCenter || dock == GUIDock.BottomRight || dock == GUIDock.TopRight)
                currentVertexXOffset = bounds.Width / 2f - totalVertexWidth;

            //float minVertexY = -bounds.Height / 2f + (!widthDominated ? 0 : 0.5f * (bounds.Height - vertexHeight));

            float vertexHeight = textureHeight * scale;
            float minVertexY = -vertexHeight / 2f;
            if (dock == GUIDock.BottomLeft || dock == GUIDock.BottomCenter || dock == GUIDock.BottomRight) {
                minVertexY = -bounds.Height / 2f;
            } else if (dock == GUIDock.TopLeft || dock == GUIDock.TopCenter || dock == GUIDock.TopRight) {
                minVertexY = bounds.Height / 2f - vertexHeight;
            }
            float maxVertexY = minVertexY + vertexHeight;


            for (int i = 0; i < this.text.Length; i++) {
                char glyph = this.text[i];
                ITexture glyphTexture = texture.GetSubTexture(glyph.ToString());

                float vertexWidth = scale * glyphTexture.Width;
                float minVertexX = currentVertexXOffset;
                float maxVertexX = minVertexX + vertexWidth;

                mesh.GetVertexData(0 + i * 4).SetAttributeData("in_position", minVertexX + bounds.X, maxVertexY + bounds.Y, 0);      // top-left
                mesh.GetVertexData(1 + i * 4).SetAttributeData("in_position", maxVertexX + bounds.X, maxVertexY + bounds.Y, 0);       // top-right
                mesh.GetVertexData(2 + i * 4).SetAttributeData("in_position", maxVertexX + bounds.X, minVertexY + bounds.Y, 0);      // bottom-right
                mesh.GetVertexData(3 + i * 4).SetAttributeData("in_position", minVertexX + bounds.X, minVertexY + bounds.Y, 0);     // bottom-left

                float[] colorData = noText ? Color.CLEAR.ToArray(true) : Color.ToArray(true);

                mesh.GetVertexData(0 + i * 4).SetAttributeData("in_color", Color.ToArray(true));      // top-left
                mesh.GetVertexData(1 + i * 4).SetAttributeData("in_color", Color.ToArray(true));       // top-right
                mesh.GetVertexData(2 + i * 4).SetAttributeData("in_color", Color.ToArray(true));      // bottom-right
                mesh.GetVertexData(3 + i * 4).SetAttributeData("in_color", Color.ToArray(true));     // bottom-left

                float[][] texCoords = glyphTexture.TextureCoordinates;
                for (int j = 0; j < 4; j++) {
                    mesh.GetVertexData(j + i * 4).SetAttributeData("in_texCoords0", texCoords[j]);
                }

                currentVertexXOffset += vertexWidth;
            }

            return mesh;
        }

    }
}