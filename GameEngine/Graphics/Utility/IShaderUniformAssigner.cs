using GameEngine.Math;
using GameEngine.Utility;

namespace GameEngine.Graphics.Utility {
    public interface IShaderUniformAssigner {
        void SetUniform(string uniformName, Color value);
        void SetUniform(string uniformName, float value);
        void SetUniform(string uniformName, float value1, float value2);
        void SetUniform(string uniformName, float value1, float value2, float value3);
        void SetUniform(string uniformName, float value1, float value2, float value3, float value4);
        //void SetUniform(string uniformName, int value);
        void SetUniform(string uniformName, Matrix2 value);
        void SetUniform(string uniformName, Matrix3 value);
        void SetUniform(string uniformName, Matrix4 value);
        void SetUniform(string uniformName, ITexture value);
        void SetUniform(string uniformName, Vector2 value);
        void SetUniform(string uniformName, Vector3 value);
        void SetUniform(string uniformName, Vector4 value);
    }
}