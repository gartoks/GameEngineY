using GameEngine.Logging;

#pragma warning disable 649

namespace GameApp.Graphics.Buffers {
    internal class IndexBufferObject {
        public const int MAX_SIZE = 60000;

        internal readonly BufferType Type;
        private readonly int[] data;

        private readonly int iboID;

        internal IndexBufferObject(int iboID, int[] data, BufferType bufferType) {
            if (data.Length > MAX_SIZE) {
                Log.WriteLine($"There can be at maximum {MAX_SIZE} indices.", LogType.Error);
                return;
            }

            this.iboID = iboID;

            Type = bufferType;

            this.data = data;
        }

        ~IndexBufferObject() {
            GLHandler.Instance.DeleteIBO(this);
        }

        internal void Bind() {
            GLHandler.Instance.BindIBO(this);
        }

        internal void Release() {
            GLHandler.Instance.ReleaseIBO(this);
        }

        internal void PrepareRender() { }

        internal int[] Data => this.data;

        internal int Size => Data.Length;

        internal bool IsBound => GLHandler.Instance.IsIBOBound(this);

        internal bool IsDisposed => this.iboID <= 0;
    }
}