using System;

namespace GameEngine.Math {
    public class Matrix2 {

        //public static Matrix2 IDENTITY { get { return new Matrix2(); } }

        private const int DIM = 2;
        private const int DIM2 = DIM * DIM;

        private readonly float[] data;

        public static Matrix2 CreateIdentity() {
            return new Matrix2(1, 0, 1, 0);
        }

        public static Matrix2 CreateDiagonal(float v00, float v11) {
            return Matrix2.CreateColumnMajor(v00, 0, 0, v11);
        }

        public static Matrix2 Create(Matrix2 m) {
            return new Matrix2(m.ColumnMajor);
        }

        public static Matrix2 CreateRowMajor(float cr00, float cr10, float cr01, float cr11) {
            return new Matrix2(cr00, cr01, cr10, cr11);
        }

        public static Matrix2 CreateRowMajor(params Vector2[] rows) {
            if (rows.Length != DIM)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix2(rows[0][0], rows[1][0], rows[0][1], rows[1][1]);
        }

        public static Matrix2 CreateRowMajor(float[,] cr) {
            return new Matrix2(cr[0, 0], cr[0, 1], cr[1, 0], cr[1, 1]);
        }

        public static Matrix2 CreateRowMajor(float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix2(data[0], data[2], data[1], data[3]);
        }

        public static Matrix2 CreateColumnMajor(float rc00, float rc10, float rc01, float rc11) {
            return new Matrix2(rc00, rc10, rc01, rc11);
        }

        public static Matrix2 CreateColumnMajor(params Vector2[] columns) {
            if (columns.Length != DIM)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix2(columns[0][0], columns[0][1], columns[1][0], columns[1][1]);
        }

        public static Matrix2 CreateColumnMajor(float[,] rc) {
            return new Matrix2(rc[0, 0], rc[1, 0], rc[0, 1], rc[1, 1]);
        }

        public static Matrix2 CreateColumnMajor(float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix2(data[0], data[1], data[2], data[3]);
        }

        private Matrix2(params float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException();

            this.data = new float[DIM2];
            Array.Copy(data, this.data, DIM2);
        }

        //private float this[int row,int column] {
        //    get { return Data[row + column * DIM]; }
        //    set { Data[row + column * DIM] = value; }
        //}

        //private float this[int index] {
        //    get { return Data[index]; }
        //    set { Data[index] = value; }
        //}

        public Matrix3 AsMatrix3() {
            return Matrix3.CreateRowMajor(rc00, rc01,  0,
                                          rc10, rc11,  0,
                                          0,    0,     1);
        }

        public Matrix4 AsMatrix4() {
            return Matrix4.CreateRowMajor(rc00, rc01, 0,   0,
                                          rc10, rc11, 0,   0,
                                          0,    0,    1,   0,
                                          0,    0,    0,   1);
        }

        public Matrix2 Apply(Func<int, int, float, float> function) {
            for (int column = 0; column < DIM; column++)
                for (int row = 0; row < DIM; row++)
                    SetColumnMajor(function.Invoke(row, column, GetColumnMajor(row, column)), row, column);

            return this;
        }

        public Matrix2 Add(Matrix2 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] += m.ColumnMajor[i];

            return this;
        }

        public Matrix2 Subtract(Matrix2 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] -= m.ColumnMajor[i];

            return this;
        }

        public Matrix2 ElementwiseMultiplication(Matrix2 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] *= m.ColumnMajor[i];

            return this;
        }

        public Matrix2 Scale(float s) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] *= s;

            return this;
        }

        public float Determinant => rc00 * rc11 - rc01 * rc10;

        public Matrix2 Inverse() {
            float det = Determinant;
            if (det == 0)
                return null;

            return Matrix2.CreateColumnMajor(rc11, -rc10, -rc01, rc00).Scale(1f / det);
        }

        public Matrix2 MakeIdentity() {
            rc00 = 1;
            rc10 = 0;
            rc01 = 0;
            rc11 = 1;

            return this;
        }

        public float rc00 {
            get => GetColumnMajor(0, 0);
            set => SetColumnMajor(value, 0, 0);
        }

        public float rc10 {
            get => GetColumnMajor(1, 0);
            set => SetColumnMajor(value, 1, 0);
        }

        public float rc01 {
            get => GetColumnMajor(0, 1);
            set => SetColumnMajor(value, 0, 1);
        }

        public float rc11 {
            get => GetColumnMajor(1, 1);
            set => SetColumnMajor(value, 1, 1);
        }

        public float GetColumnMajor(int row, int column) {
            return ColumnMajor[row + column * DIM];
        }

        public void SetColumnMajor(float data, int row, int column) {
            ColumnMajor[row + column * DIM] = data;
        }

        public float[] ColumnMajor {
            get => this.data;
            set {
                if (value.Length != DIM2)
                    throw new ArgumentException("Invalid data length.", "data");

                Array.Copy(value, this.data, DIM2);
            }
        }

        public float cr00 {
            get => GetRowMajor(0, 0);
            set => SetRowMajor(value, 0, 0);
        }

        public float cr10 {
            get => GetRowMajor(1, 0);
            set => SetRowMajor(value, 1, 0);
        }

        public float cr01 {
            get => GetRowMajor(0, 1);
            set => SetRowMajor(value, 0, 1);
        }

        public float cr11 {
            get => GetRowMajor(1, 1);
            set => SetRowMajor(value, 1, 1);
        }

        public float GetRowMajor(int column, int row) {
            return ColumnMajor[row + column * DIM];
        }

        public void SetRowMajor(float data, int column, int row) {
            ColumnMajor[row + column * DIM] = data;
        }

        public float[] RowMajor {
            get {
                float[] rm = new float[DIM2];
                for (int row = 0; row < DIM; row++)
                    for (int column = 0; column < DIM; column++)
                        rm[column + row * DIM] = GetRowMajor(column, row);
                return rm;
            }
            set {
                if (value.Length != DIM2)
                    throw new ArgumentException("Invalid data length.", "data");

                for (int row = 0; row < DIM; row++)
                    for (int column = 0; column < DIM; column++)
                        SetRowMajor(value[column + row * DIM], column, row);
            }
        }

        public void Set(Matrix2 m) {
            for (int i = 0; i < DIM2; i++) {
                this.data[i] = m.data[i];
            }
        }

        public Matrix2 Clone() => Matrix2.Create(this);

        public override bool Equals(System.Object obj) {
            if (!(obj is Matrix2 m))
                return false;

            for (int i = 0; i < DIM2; i++) {
                if (ColumnMajor[i] != m.ColumnMajor[i])
                    return false;
            }

            return true;
        }

        public override int GetHashCode() {
            return ColumnMajor.GetHashCode();
        }

        public override string ToString() {
            return $"[{cr00} {cr10}]\n[{cr01} {cr11}]";
        }

        public static Matrix2 Transpose(Matrix2 m0) {
            return CreateRowMajor(m0.ColumnMajor);
        }

        public static Matrix2 operator +(Matrix2 m0, Matrix2 m1) {
            return Create(m0).Add(m1);
        }

        public static Matrix2 operator -(Matrix2 m0, Matrix2 m1) {
            return Create(m0).Subtract(m1);
        }

        public static Matrix2 operator *(Matrix2 m0, Matrix2 m1) {
            float[] cm = new float[DIM2];

            for (int i = 0; i < DIM; i++) {
                for (int j = 0; j < DIM; j++) {
                    for (int k = 0; k < DIM; k++)
                        cm[j * DIM + i] += m0.ColumnMajor[k * DIM + i] * m1.ColumnMajor[j * DIM + k];
                }
            }

            return Matrix2.CreateColumnMajor(cm);
        }

        public static Matrix2 operator *(Matrix2 m0, float s) {
            return Create(m0).Scale(s);
        }

        public static Matrix2 operator *(float s, Matrix2 m0) {
            return m0 * s;
        }
    }
}
