using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GameEngine.Graphics.Utility {
    public class IndexBufferObject : IDisposable {
        public const int MAX_SIZE = 60000;

        public readonly BufferType Type;
        public readonly int Size;

        private int iboID;

        private readonly ObservableCollection<uint> data;

        private bool isDirty;

        public IndexBufferObject(int size, BufferType bufferType) {
            if (size > MAX_SIZE)
                throw new ArgumentException($"There can be at maximum {MAX_SIZE} indices.");

            Type = bufferType;
            Size = size;

            this.data = new ObservableCollection<uint>();
            this.isDirty = true;

            this.data.CollectionChanged += OnDataChanged;

            GLHandler.InitializeIBO(this);
        }

        ~IndexBufferObject() {
            Dispose();
        }

        public void Dispose() {
            GLHandler.DeleteIBO(this);
        }

        public void Bind() {
            GLHandler.BindIBO(this);
        }

        public void Release() {
            GLHandler.ReleaseIBO(this);
        }

        // TODO bind for usage / active etc ... clean before

        private void OnDataChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.isDirty = true;
        }

        public void PrepareRender() {
            if (!this.isDirty)
                return;

            GLHandler.MapIBOData(this);

            this.isDirty = false;
        }



        public IEnumerable<uint> Data {
            get => this.data;
            set {
                if (value == null)
                    value = new uint[0];

                this.data.Clear();
                foreach (uint i in value) {
                    this.data.Add(i);
                }
            }
        }

        public bool IsBound => GLHandler.IsIBOBound(this);

        public bool IsDisposed => this.iboID <= 0;
    }
}