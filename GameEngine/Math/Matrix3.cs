using System;

namespace GameEngine.Math {
    public class Matrix3 {

        //public static Matrix3A IDENTITY { get { return new Matrix3A(); } }

        private const int DIM = 3;
        private const int DIM2 = DIM * DIM;

        private float[] data;

        public static Matrix3 CreateIdentity() {
            return CreateDiagonal(1, 1, 1);
        }

        public static Matrix3 CreateDiagonal(float v00, float v11, float v22) {
            return Matrix3.CreateColumnMajor(v00, 0, 0, 0, v11, 0, 0, 0, v22);
        }

        public static Matrix3 Create(Matrix3 m) {
            return new Matrix3(m.ColumnMajor);
        }

        public static Matrix3 CreateRowMajor(  float cr00, float cr10, float cr20,
                                                float cr01, float cr11, float cr21,
                                                float cr02, float cr12, float cr22) {
            return new Matrix3(cr00, cr01, cr02, cr10, cr11, cr12, cr20, cr21, cr22);
        }

        public static Matrix3 CreateRowMajor(params Vector3[] rows) {
            if (rows.Length != DIM)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix3(rows[0][0], rows[1][0], rows[2][0], rows[0][1], rows[1][1], rows[2][1], rows[0][2], rows[1][2], rows[2][2]);
        }

        public static Matrix3 CreateRowMajor(float[,] cr) {
            return new Matrix3(cr[0, 0], cr[0, 1], cr[0, 2], cr[1, 0], cr[1, 1], cr[1, 2], cr[2, 0], cr[2, 1], cr[2, 2]);
        }

        public static Matrix3 CreateRowMajor(float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix3(data[0], data[3], data[6], data[1], data[4], data[7], data[2], data[5], data[8]);
        }

        public static Matrix3 CreateColumnMajor(float rc00, float rc10, float rc20, float rc01, float rc11, float rc21, float rc02, float rc12, float rc22) {
            return new Matrix3(rc00, rc10, rc20, rc01, rc11, rc21, rc02, rc12, rc22);
        }

        public static Matrix3 CreateColumnMajor(params Vector3[] columns) {
            if (columns.Length != DIM)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix3(columns[0][0], columns[0][1], columns[0][2], columns[1][0], columns[1][1], columns[1][2], columns[2][0], columns[2][1], columns[2][2]);
        }

        public static Matrix3 CreateColumnMajor(float[,] rc) {
            return new Matrix3(rc[0, 0], rc[1, 0], rc[2, 0], rc[0, 1], rc[1, 1], rc[2, 1], rc[0, 2], rc[1, 2], rc[2, 2]);
        }

        public static Matrix3 CreateColumnMajor(float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix3(data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8]);
        }

        private Matrix3(params float[] data) {
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

        public Matrix4 AsMatrix4() {
            return Matrix4.CreateRowMajor(   rc00, rc01, rc02, 0,
                                             rc10, rc11, rc12, 0,
                                             rc20, rc21, rc22, 0,
                                             0,    0,    0,    1);
        }

        public Matrix3 Apply(Func<int, int, float, float> function) {
            for (int column = 0; column < DIM; column++)
                for (int row = 0; row < DIM; row++)
                    SetColumnMajor(function.Invoke(row, column, GetColumnMajor(row, column)), row, column);

            return this;
        }

        public Matrix3 Add(Matrix3 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] += m.ColumnMajor[i];

            return this;
        }

        public Matrix3 Subtract(Matrix3 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] -= m.ColumnMajor[i];

            return this;
        }

        public Matrix3 ElementwiseMultiplication(Matrix3 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] *= m.ColumnMajor[i];

            return this;
        }

        public Matrix3 Scale(float s) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] *= s;

            return this;
        }

        public float Determinant => rc00 * (rc11 * rc22 - rc12 * rc21) - rc01 * (rc10 * rc22 - rc12 * rc20) + rc02 * (rc10 * rc21 - rc11 * rc20);

        public Matrix3 Inverse() {
            float det = Determinant;

            if (det == 0)
                return null;

            return Matrix3.CreateRowMajor(
                rc11*rc22 - rc12*rc21, rc02*rc21 - rc01*rc22, rc01*rc12 - rc02*rc11,
                rc12*rc20 - rc10*rc22, rc00*rc22 - rc02*rc20, rc02*rc10 - rc00*rc12,
                rc10*rc21 - rc11*rc20, rc01*rc20 - rc00*rc21, rc00*rc11 - rc01*rc10
                ).Scale(1f / det);
        }

        public Matrix3 MakeIdentity() {
            rc00 = 1;
            rc10 = 0;
            rc20 = 0;
            rc01 = 0;
            rc11 = 1;
            rc21 = 0;
            rc02 = 0;
            rc12 = 0;
            rc22 = 1;

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

        public float rc20 {
            get => GetColumnMajor(2, 0);
            set => SetColumnMajor(value, 2, 0);
        }

        public float rc01 {
            get => GetColumnMajor(0, 1);
            set => SetColumnMajor(value, 0, 1);
        }

        public float rc11 {
            get => GetColumnMajor(1, 1);
            set => SetColumnMajor(value, 1, 1);
        }

        public float rc21 {
            get => GetColumnMajor(2, 1);
            set => SetColumnMajor(value, 2, 1);
        }

        public float rc02 {
            get => GetColumnMajor(0, 2);
            set => SetColumnMajor(value, 0, 2);
        }

        public float rc12 {
            get => GetColumnMajor(1, 2);
            set => SetColumnMajor(value, 1, 2);
        }

        public float rc22 {
            get => GetColumnMajor(2, 2);
            set => SetColumnMajor(value, 2, 2);
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

        public float cr20 {
            get => GetRowMajor(2, 0);
            set => SetRowMajor(value, 2, 0);
        }

        public float cr01 {
            get => GetRowMajor(0, 1);
            set => SetRowMajor(value, 0, 1);
        }

        public float cr11 {
            get => GetRowMajor(1, 1);
            set => SetRowMajor(value, 1, 1);
        }

        public float cr21 {
            get => GetRowMajor(2, 1);
            set => SetRowMajor(value, 2, 1);
        }

        public float cr02 {
            get => GetRowMajor(0, 2);
            set => SetRowMajor(value, 0, 2);
        }

        public float cr12 {
            get => GetRowMajor(1, 2);
            set => SetRowMajor(value, 1, 2);
        }

        public float cr22 {
            get => GetRowMajor(2, 2);
            set => SetRowMajor(value, 2, 2);
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

        public Matrix3 Clone() => Matrix3.Create(this);

        public override bool Equals(Object obj) {
            if (!(obj is Matrix3 m))
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
            return $"[{cr00} {cr10} {cr20}]\n[{cr01} {cr11} {cr21}]\n[{cr02} {cr12} {cr22}]";
        }

        public static Matrix3 Transpose(Matrix3 m0) {
            return CreateRowMajor(m0.ColumnMajor);
        }

        public static Matrix3 operator +(Matrix3 m0, Matrix3 m1) {
            return Create(m0).Add(m1);
        }

        public static Matrix3 operator -(Matrix3 m0, Matrix3 m1) {
            return Create(m0).Subtract(m1);
        }

        public static Matrix3 operator *(Matrix3 m0, Matrix3 m1) {
            float[] cm = new float[DIM2];

            for (int i = 0; i < DIM; i++) {
                for (int j = 0; j < DIM; j++) {
                    for (int k = 0; k < DIM; k++)
                        cm[j * DIM + i] += m0.ColumnMajor[k * DIM + i] * m1.ColumnMajor[j * DIM + k];
                }
            }

            return Matrix3.CreateColumnMajor(cm);
        }

        public static Matrix3 operator *(Matrix3 m0, float s) {
            return Create(m0).Scale(s);
        }

        public static Matrix3 operator *(float s, Matrix3 m0) {
            return m0 * s;
        }
    }
}
