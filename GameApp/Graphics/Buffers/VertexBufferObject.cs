using System;
using GameEngine.Logging;

#pragma warning disable 649

namespace GameApp.Graphics.Buffers {
    internal sealed class VertexBufferObject {
        internal const int MAX_SIZE = 60000;

        internal readonly BufferType Type;
        private readonly float[] data;

        private readonly int vboID;

        internal VertexBufferObject(int vboID, float[] data, BufferType bufferType) {
            if (data.Length > MAX_SIZE) {
                Log.WriteLine($"There can be at maximum {MAX_SIZE} vertex data.", LogType.Error);
                return;
            }

            this.vboID = vboID;

            Type = bufferType;

            this.data = new float[data.Length];
            Array.Copy(data, this.data, data.Length);
        }

        ~VertexBufferObject() {
            GLHandler.Instance.DeleteVBO(this);
        }

        internal void Bind() {
            GLHandler.Instance.BindVBO(this);
        }

        internal void Release() {
            GLHandler.Instance.ReleaseVBO(this);
        }

        internal void PrepareRender() { }

        internal float[] Data {
            get => this.data;
            set {
                if (value == null || value.Length != this.data.Length) {
                    Log.WriteLine("Cannot update vertex buffer object data. Data lengths do not match.", LogType.Error);
                    return;
                }

                Array.Copy(value, this.data, data.Length);
                GLHandler.Instance.UpdateVBOData(this);
            }
        }

        internal int Size => Data.Length;

        internal bool IsBound => GLHandler.Instance.IsVBOBound(this);

        internal bool IsDisposed => this.vboID <= 0;
    }
}