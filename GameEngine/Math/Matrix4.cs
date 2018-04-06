using System;

namespace GameEngine.Math {
    public class Matrix4 {

        private const int DIM = 4;
        private const int DIM2 = DIM * DIM;

        private readonly float[] data;

        public static Matrix4 CreateIdentity() {
            return CreateDiagonal(1, 1, 1, 1);
        }

        public static Matrix4 CreateDiagonal(float v00, float v11, float v22, float v33) {
            return Matrix4.CreateColumnMajor(v00, 0,    0,   0,
                                              0,   v11,  0,   0,
                                              0,   0,    v22, 0,
                                              0,   0,    0,   v33);
        }

        public static Matrix4 Create(Matrix4 m) {
            return new Matrix4(m.ColumnMajor);
        }

        public static Matrix4 CreateRowMajor(float cr00, float cr10, float cr20, float cr30,
                                              float cr01, float cr11, float cr21, float cr31,
                                              float cr02, float cr12, float cr22, float cr32,
                                              float cr03, float cr13, float cr23, float cr33) {
            return new Matrix4(cr00, cr01, cr02, cr03, cr10, cr11, cr12, cr13, cr20, cr21, cr22, cr23, cr30, cr31, cr32, cr33);
        }

        public static Matrix4 CreateRowMajor(params Vector3[] rows) {
            if (rows.Length != DIM)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix4(rows[0][0], rows[1][0], rows[2][0], rows[3][0],
                                rows[0][1], rows[1][1], rows[2][1], rows[3][1],
                                rows[0][2], rows[1][2], rows[2][2], rows[3][2],
                                rows[0][3], rows[1][3], rows[2][3], rows[3][3]);
        }

        public static Matrix4 CreateRowMajor(float[,] cr) {
            return new Matrix4(cr[0, 0], cr[0, 1], cr[0, 2], cr[0, 3],
                                cr[1, 0], cr[1, 1], cr[1, 2], cr[1, 3],
                                cr[2, 0], cr[2, 1], cr[2, 2], cr[2, 3],
                                cr[3, 0], cr[3, 1], cr[3, 2], cr[3, 3]);
        }

        public static Matrix4 CreateRowMajor(float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix4(data[0], data[4], data[8],  data[12],
                                data[1], data[5], data[9],  data[13],
                                data[2], data[6], data[10], data[14],
                                data[3], data[7], data[11], data[15]);
        }

        public static Matrix4 CreateColumnMajor(float rc00, float rc10, float rc20, float rc30,
                                                 float rc01, float rc11, float rc21, float rc31,
                                                 float rc02, float rc12, float rc22, float rc32,
                                                 float rc03, float rc13, float rc23, float rc33) {
            return new Matrix4(rc00, rc10, rc20, rc30,
                                rc01, rc11, rc21, rc31,
                                rc02, rc12, rc22, rc32,
                                rc03, rc13, rc23, rc33);
        }

        public static Matrix4 CreateColumnMajor(params Vector3[] columns) {
            if (columns.Length != DIM)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix4(columns[0][0], columns[0][1], columns[0][2], columns[0][3],
                                columns[1][0], columns[1][1], columns[1][2], columns[1][3],
                                columns[2][0], columns[2][1], columns[2][2], columns[2][3],
                                columns[3][0], columns[3][1], columns[3][2], columns[3][3]);
        }

        public static Matrix4 CreateColumnMajor(float[,] rc) {
            return new Matrix4(rc[0, 0], rc[1, 0], rc[2, 0], rc[3, 0],
                                rc[0, 1], rc[1, 1], rc[2, 1], rc[3, 1],
                                rc[0, 2], rc[1, 2], rc[2, 2], rc[3, 2],
                                rc[0, 3], rc[1, 3], rc[2, 3], rc[3, 3]);
        }

        public static Matrix4 CreateColumnMajor(float[] data) {
            if (data.Length != DIM2)
                throw new ArgumentException("Invalid matrix data length.");

            return new Matrix4(data[0],  data[1],  data[2],  data[3],
                                data[4],  data[5],  data[6],  data[7],
                                data[8],  data[9],  data[10], data[11],
                                data[12], data[13], data[14], data[15]);
        }

        private Matrix4(params float[] data) {
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

        public Matrix4 Apply(Func<int, int, float, float> function) {
            for (int column = 0; column < DIM; column++)
                for (int row = 0; row < DIM; row++)
                    SetColumnMajor(function.Invoke(row, column, GetColumnMajor(row, column)), row, column);

            return this;
        }

        public Matrix4 Add(Matrix4 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] += m.ColumnMajor[i];

            return this;
        }

        public Matrix4 Subtract(Matrix4 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] -= m.ColumnMajor[i];

            return this;
        }

        public Matrix4 ElementwiseMultiplication(Matrix4 m) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] *= m.ColumnMajor[i];

            return this;
        }

        public Matrix4 Scale(float s) {
            for (int i = 0; i < DIM2; i++)
                ColumnMajor[i] *= s;

            return this;
        }

        public float Determinant => rc00 * rc11 * rc22 * rc33 + rc00 * rc12 * rc23 * rc31 + rc00 * rc13 * rc21 * rc32
                                    + rc01 * rc10 * rc23 * rc32 + rc01 * rc12 * rc20 * rc33 + rc01 * rc13 * rc22 * rc30
                                    + rc02 * rc10 * rc21 * rc33 + rc02 * rc11 * rc23 * rc30 + rc02 * rc13 * rc20 * rc31
                                    + rc03 * rc10 * rc22 * rc31 + rc03 * rc11 * rc20 * rc32 + rc03 * rc12 * rc21 * rc30
                                    - rc00 * rc11 * rc23 * rc32 - rc00 * rc12 * rc21 * rc33 - rc00 * rc13 * rc22 * rc31
                                    - rc01 * rc10 * rc22 * rc33 - rc01 * rc12 * rc23 * rc30 - rc01 * rc13 * rc20 * rc32
                                    - rc02 * rc10 * rc23 * rc31 - rc02 * rc11 * rc20 * rc33 - rc02 * rc13 * rc21 * rc30
                                    - rc03 * rc10 * rc21 * rc32 - rc03 * rc11 * rc22 * rc30 - rc03 * rc12 * rc20 * rc31;

        public Matrix4 Inverse() {
            float det = Determinant;

            if (det == 0)
                return null;

            float b00 = rc11 * rc22 * rc33 + rc12 * rc23 * rc31 + rc13 * rc21 * rc32 - rc11 * rc23 * rc32 - rc12 * rc21 * rc33 - rc13 * rc22 * rc31;
            float b01 = rc01 * rc23 * rc32 + rc02 * rc21 * rc33 + rc03 * rc22 * rc31 - rc01 * rc22 * rc33 - rc02 * rc23 * rc31 - rc03 * rc21 * rc32;
            float b02 = rc01 * rc12 * rc33 + rc02 * rc13 * rc31 + rc03 * rc11 * rc32 - rc01 * rc13 * rc32 - rc02 * rc11 * rc33 - rc03 * rc12 * rc31;
            float b03 = rc01 * rc13 * rc22 + rc02 * rc11 * rc23 + rc03 * rc12 * rc21 - rc01 * rc12 * rc23 - rc02 * rc13 * rc21 - rc03 * rc11 * rc22;

            float b10 = rc10 * rc23 * rc32 + rc12 * rc20 * rc33 + rc13 * rc22 * rc30 - rc10 * rc22 * rc33 - rc12 * rc23 * rc30 - rc13 * rc20 * rc32;
            float b11 = rc00 * rc22 * rc33 + rc02 * rc23 * rc30 + rc03 * rc20 * rc32 - rc00 * rc23 * rc32 - rc02 * rc20 * rc33 - rc03 * rc22 * rc30;
            float b12 = rc00 * rc13 * rc32 + rc02 * rc10 * rc33 + rc03 * rc12 * rc30 - rc00 * rc12 * rc33 - rc02 * rc13 * rc30 - rc03 * rc10 * rc32;
            float b13 = rc00 * rc12 * rc23 + rc02 * rc13 * rc20 + rc03 * rc10 * rc22 - rc00 * rc13 * rc22 - rc02 * rc10 * rc23 - rc03 * rc12 * rc20;

            float b20 = rc10 * rc21 * rc33 + rc11 * rc23 * rc30 + rc13 * rc20 * rc31 - rc10 * rc23 * rc31 - rc11 * rc20 * rc33 - rc13 * rc21 * rc30;
            float b21 = rc00 * rc23 * rc31 + rc01 * rc20 * rc33 + rc03 * rc21 * rc30 - rc00 * rc21 * rc33 - rc01 * rc23 * rc30 - rc03 * rc20 * rc31;
            float b22 = rc00 * rc11 * rc33 + rc01 * rc13 * rc30 + rc03 * rc10 * rc31 - rc00 * rc13 * rc31 - rc01 * rc10 * rc33 - rc03 * rc11 * rc30;
            float b23 = rc00 * rc13 * rc21 + rc01 * rc10 * rc23 + rc03 * rc11 * rc20 - rc00 * rc11 * rc23 - rc01 * rc13 * rc20 - rc03 * rc10 * rc21;

            float b30 = rc10 * rc22 * rc31 + rc11 * rc20 * rc32 + rc12 * rc21 * rc30 - rc10 * rc21 * rc32 - rc11 * rc22 * rc30 - rc12 * rc20 * rc31;
            float b31 = rc00 * rc21 * rc32 + rc01 * rc22 * rc30 + rc02 * rc20 * rc31 - rc00 * rc22 * rc31 - rc01 * rc20 * rc32 - rc02 * rc21 * rc30;
            float b32 = rc00 * rc12 * rc31 + rc01 * rc10 * rc32 + rc02 * rc11 * rc30 - rc00 * rc11 * rc32 - rc01 * rc12 * rc30 - rc02 * rc10 * rc31;
            float b33 = rc00 * rc11 * rc22 + rc01 * rc12 * rc20 + rc02 * rc10 * rc21 - rc00 * rc12 * rc21 - rc01 * rc10 * rc22 - rc02 * rc11 * rc20;

            SetColumnMajor(b00, 0, 0);
            SetColumnMajor(b01, 0, 1);
            SetColumnMajor(b02, 0, 2);
            SetColumnMajor(b03, 0, 3);

            SetColumnMajor(b10, 1, 0);
            SetColumnMajor(b11, 1, 1);  // TODO make sure is set column major
            SetColumnMajor(b12, 1, 2);
            SetColumnMajor(b13, 1, 3);

            SetColumnMajor(b20, 2, 0);
            SetColumnMajor(b21, 2, 1);
            SetColumnMajor(b22, 2, 2);
            SetColumnMajor(b23, 2, 3);

            SetColumnMajor(b30, 3, 0);
            SetColumnMajor(b31, 3, 1);
            SetColumnMajor(b32, 3, 2);
            SetColumnMajor(b33, 3, 3);

            return this;

            //return Matrix4.CreateRowMajor(
            //    b00, b01, b02, b03,
            //    b10, b11, b12, b13,
            //    b20, b21, b22, b23,
            //    b30, b31, b32, b33
            //    ).Scale(1f / det);
        }

        public Matrix4 GetInverse() => Matrix4.Create(this).Inverse();

        public Matrix4 MakeIdentity() {
            rc00 = 1;
            rc10 = 0;
            rc20 = 0;
            rc30 = 0;

            rc01 = 0;
            rc11 = 1;
            rc21 = 0;
            rc31 = 0;

            rc02 = 0;
            rc12 = 0;
            rc22 = 1;
            rc32 = 0;

            rc03 = 0;
            rc13 = 0;
            rc23 = 0;
            rc33 = 1;
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

        public float rc30 {
            get => GetColumnMajor(3, 0);
            set => SetColumnMajor(value, 3, 0);
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

        public float rc31 {
            get => GetColumnMajor(3, 1);
            set => SetColumnMajor(value, 3, 1);
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

        public float rc32 {
            get => GetColumnMajor(3, 2);
            set => SetColumnMajor(value, 3, 2);
        }

        public float rc03 {
            get => GetColumnMajor(0, 3);
            set => SetColumnMajor(value, 0, 3);
        }

        public float rc13 {
            get => GetColumnMajor(1, 3);
            set => SetColumnMajor(value, 1, 3);
        }

        public float rc23 {
            get => GetColumnMajor(2, 3);
            set => SetColumnMajor(value, 2, 3);
        }

        public float rc33 {
            get => GetColumnMajor(3, 3);
            set => SetColumnMajor(value, 3, 3);
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

        public float cr30 {
            get => GetRowMajor(3, 0);
            set => SetRowMajor(value, 3, 0);
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

        public float cr31 {
            get => GetRowMajor(3, 1);
            set => SetRowMajor(value, 3, 1);
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

        public float cr32 {
            get => GetRowMajor(3, 2);
            set => SetRowMajor(value, 3, 2);
        }

        public float cr03 {
            get => GetRowMajor(0, 3);
            set => SetRowMajor(value, 0, 3);
        }

        public float cr13 {
            get => GetRowMajor(1, 3);
            set => SetRowMajor(value, 1, 3);
        }

        public float cr23 {
            get => GetRowMajor(2, 3);
            set => SetRowMajor(value, 2, 3);
        }

        public float cr33 {
            get => GetRowMajor(3, 3);
            set => SetRowMajor(value, 3, 3);
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

        public void Set(Matrix4 m) {
            for (int i = 0; i < DIM2; i++) {
                this.data[i] = m.data[i];
            }
        }

        public Matrix4 Clone() => Matrix4.Create(this);

        public override bool Equals(Object obj) {
            if (!(obj is Matrix4 m))
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
            return $"[{cr00} {cr10} {cr20} {cr30}]\n[{cr01} {cr11} {cr21} {cr31}]\n[{cr02} {cr12} {cr22} {cr32}]\n[{cr03} {cr13} {cr23} {cr33}]";
        }

        public static Matrix4 Transpose(Matrix4 m0) {
            return CreateRowMajor(m0.ColumnMajor);
        }

        public static Matrix4 operator +(Matrix4 m0, Matrix4 m1) {
            return Create(m0).Add(m1);
        }

        public static Matrix4 operator -(Matrix4 m0, Matrix4 m1) {
            return Create(m0).Subtract(m1);
        }

        public Matrix4 MultiplyRight(Matrix4 m) {
            float[] cm = new float[DIM2];

            for (int i = 0; i < DIM; i++) {
                for (int j = 0; j < DIM; j++) {
                    for (int k = 0; k < DIM; k++)
                        cm[j * DIM + i] += this.ColumnMajor[k * DIM + i] * m.ColumnMajor[j * DIM + k];
                }
            }

            this.ColumnMajor = cm;

            return this;
        }

        public Matrix4 MultiplyLeft(Matrix4 m) {
            float[] cm = new float[DIM2];

            for (int i = 0; i < DIM; i++) {
                for (int j = 0; j < DIM; j++) {
                    for (int k = 0; k < DIM; k++)
                        cm[j * DIM + i] += m.ColumnMajor[k * DIM + i] * this.ColumnMajor[j * DIM + k];
                }
            }

            this.ColumnMajor = cm;

            return this;
        }

        public Vector4 Multiply(Vector4 v) {
            float[] cm = new float[DIM];

            for (int i = 0; i < DIM; i++) {
                for (int k = 0; k < DIM; k++) {
                    cm[i] += this.ColumnMajor[k * DIM + i] * v[k];
                }
            }

            v.Set(cm[0], cm[1], cm[2], cm[3]);

            return v;
        }

        public static Matrix4 operator *(Matrix4 m0, Matrix4 m1) {
            return Create(m0).MultiplyRight(m1);
        }

        public static Matrix4 operator *(Matrix4 m0, float s) {
            return Create(m0).Scale(s);
        }

        public static Matrix4 operator *(float s, Matrix4 m0) {
            return m0 * s;
        }

        public static Vector4 operator *(Matrix4 m, Vector4 v) {
            return m.Multiply(new Vector4(v));
        }
    }
}
