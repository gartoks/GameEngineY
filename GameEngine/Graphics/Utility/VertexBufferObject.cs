using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GameEngine.Graphics.Utility {
    public sealed class VertexBufferObject : IDisposable {
        public const int MAX_SIZE = 60000;

        public readonly BufferType Type;
        public readonly int Size;

        private int vboID;

        private readonly ObservableCollection<float> data;

        private bool isDirty;

        public VertexBufferObject(int size, BufferType bufferType) {
            if (size > MAX_SIZE)
                throw new ArgumentException($"There can be at maximum {MAX_SIZE} vertex data.");

            Type = bufferType;
            Size = size;

            this.data = new ObservableCollection<float>();
            this.isDirty = true;

            this.data.CollectionChanged += OnDataChanged;

            GLHandler.InitializeVBO(this);
        }

        ~VertexBufferObject() {
            Dispose();
        }

        public void Dispose() {
            GLHandler.DeleteVBO(this);
        }

        public void Bind() {
            GLHandler.BindVBO(this);
        }

        public void Release() {
            GLHandler.ReleaseVBO(this);
        }

        // TODO bind for usage / active etc ... clean before

        private void OnDataChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.isDirty = true;
        }

        public void PrepareRender() {
            if (!this.isDirty)
                return;

            GLHandler.MapVBOData(this);

            this.isDirty = false;
        }

        public IEnumerable<float> Data {
            get => this.data;
            set {
                if (value == null)
                    value = new float[0];

                this.data.Clear();
                foreach (float d in value) {
                    this.data.Add(d);
                }
            }
        }

        public bool IsBound => GLHandler.IsVBOBound(this);

        public bool IsDisposed => this.vboID <= 0;
    }
}