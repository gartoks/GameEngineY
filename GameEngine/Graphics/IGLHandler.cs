using System.Collections.Generic;
using GameEngine.Game.GameObjects;
using GameEngine.Graphics.RenderSettings;
using GameEngine.Graphics.Textures;
using GameEngine.Graphics.Utility;
using GameEngine.Math;
using GameEngine.Utility;

namespace GameEngine.Graphics {
    public interface IGLHandler {   // TODO stencil

        void Render(IndexBufferObject ibo);

        #region VertexArrayObjects
        void InitializeVAO(Renderable vao);

        void BindVAO(Renderable vao);

        void ReleaseVAO(Renderable vao);

        void DeleteVAO(Renderable vao);

        bool IsVAOBound(Renderable vao);
        #endregion

        #region VertexBufferObjects
        void InitializeVBO(VertexBufferObject vbo);

        void BindVBO(VertexBufferObject vbo);

        void ReleaseVBO(VertexBufferObject vbo);

        void MapVBOData(VertexBufferObject vbo);

        void DeleteVBO(VertexBufferObject vbo);

        bool IsVBOBound(VertexBufferObject vbo);
        #endregion

        #region IndexBufferObjects
        void InitializeIBO(IndexBufferObject ibo);

        void BindIBO(IndexBufferObject ibo);

        void ReleaseIBO(IndexBufferObject ibo);

        void MapIBOData(IndexBufferObject ibo);

        void DeleteIBO(IndexBufferObject ibo);

        bool IsIBOBound(IndexBufferObject ibo);
        #endregion

        #region Shaders
        void CreateShader(string vertexShaderSource, string fragmentShaderSource, /*string geometryShaderSource,*/
                            out int programHandle, out int vertexShaderHandle, out int fragmentShaderHandle/*, out int geometryShaderHandle*/);

        void DeleteShader(Shader shader);

        void BindShader(Shader shader);

        void ReleaseShader(Shader shader);

        void SetVertexAttributePointer(Shader.ShaderVertexAttribute vertexAttribute, /*Type type, bool v, */int stride);

        //private static VertexAttribPointerType ToVertexAttribPointerType(Type type) {
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

        void EnableVertexAttributeArray(int attributeIndex);

        void DisableVertexAttributeArray(int attributeIndex);

        Dictionary<string, Shader.ShaderUniform> RetrieveShaderUniforms(Shader shader, out bool validUniforms);

        Dictionary<string, Shader.ShaderVertexAttribute> RetrieveShaderAttributes(Shader shader, out bool validAttributes);

        void SetShaderUniform(int uniformLocation, float v1);

        void SetShaderUniform(int uniformLocation, float v1, float v2);

        void SetShaderUniform(int uniformLocation, float v1, float v2, float v3);

        void SetShaderUniform(int uniformLocation, float v1, float v2, float v3, float v4);

        void SetShaderUniform(int uniformLocation, int v1);

        //void SetShaderUniform(int uniformLocation, int v1, int v2);

        //void SetShaderUniform(int uniformLocation, int v1, int v2, int v3);

        //void SetShaderUniform(int uniformLocation, int v1, int v2, int v3, int v4);

        void SetShaderUniform(int uniformLocation, Matrix2 v1);

        void SetShaderUniform(int uniformLocation, Matrix3 v1);

        void SetShaderUniform(int uniformLocation, Matrix4 v1);

        bool IsShaderBound(Shader shader);
        #endregion

        #region Textures
        void InitializeTexture(Texture2D texture);

        /// <summary>
        /// Binds the texture to an available texture unit.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Returns the texture unit the texture was bound to. If all texture units are in use -1 is returned. </returns>
        int BindTexture(Texture texture);

        /// <summary>
        /// Binds the texture to the given texture unit. if the texture is already bound to another texture unit this texture unit will be cleared.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="textureUnit">The texture unit.</param>
        /// <returns>Returns the texture previously bound to the given texture unit.</returns>
        void BindTexture(Texture texture, int textureUnit);

        /// <summary>
        /// Releases the texture of the given texture unit.
        /// </summary>
        /// <param name="textureUnit">The texture unit.</param>
        /// <returns>Returns the bound texture. If no texture was bound to the given texture unit null is returned.</returns>
        void ReleaseTexture(int textureUnit);

        void DisposeTexture(Texture texture);

        /// <summary>
        /// Releases the texture.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Returns the texture unit the texture was bound to. If it was not bound -1 is returned.</returns>
        void ReleaseTexture(Texture texture);

        void ActivateTexture(Texture texture);

        void ActivateTextureUnit(int textureUnit);

        Texture BoundTexture(int textureUnit);

        /// <summary>
        /// Finds the texture unit the texture is bound to.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>Returns the texture unit the texture is bound to. If it is not bound to any texture unit -1 is returned.</returns>
        int BoundToTextureUnit(Texture texture);

        /// <summary>
        /// Updates the texture's wrap mode.
        /// </summary>
        /// <param name="texture">The texture.</param>
        void UpdateTextureWrapMode(Texture texture);

        /// <summary>
        /// Updates the texture's filter mode.
        /// </summary>
        /// <param name="texture">The texture.</param>
        void UpdateTextureFilterMode(Texture texture);

        /// <summary>
        /// Determines whether the specified texture is bound.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <returns>
        ///   <c>true</c> if the specified texture is bound; otherwise, <c>false</c>.
        /// </returns>
        bool IsTextureBound(Texture texture);

        bool HasActiveTexture { get; }

        Texture ActiveTexture { get; }

        int SupportedTextureUnits { get; }
        #endregion Textures

        #region Transforms

        void ApplyTransformation(Transform transform);

        //void ApplyTranslation(float dx, float dy);

        //void ApplyRotation(float angle);

        //void ApplyRotation(float angle, float px, float py);

        //void ApplyScaling(float sx, float sy);

        void RevertTransform();

        Matrix4 CurrentTransformationMatrix { get; }
        #endregion

        #region RenderSettings
        BlendMode BlendMode { set; }

        (BlendFunction source, BlendFunction destination)? BlendFunctions { get; set; }

        DepthFunction? DepthFunction { get; set; }

        AntiAliasMode AntiAliasMode { get; set; }

        Color ClearColor { get; set; }

        void SetClearModes(bool color, bool depth, bool stencil);

        bool EnableCulling { set; }

        bool ClockwiseCulling { set; }

        void CullFaces(bool front, bool back);

        bool EnableTextures { set; }

        bool EnableBlending { set; }

        bool EnableDepthTest { set; }

        bool EnableEdgeAntialiasing { set; }

        void EnableColors(bool red, bool green, bool blue, bool alpha);
        #endregion RenderSettings

    }
}