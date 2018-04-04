using GameEngine.Math;
using GameEngine.Utility.DataStructures;

namespace GameEngine.Graphics.Utility {
    public class MatrixTransformationHelper {
        private static ObjectPool<Matrix4> mPool4 = new ObjectPool<Matrix4>(Matrix4.CreateIdentity, m => m.MakeIdentity());

        #region ProjectionMatrices
        public static Matrix4 SetToOrthographicProjection(float left, float right, float bottom, float top, float nearVal, float farVal, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            m.MakeIdentity();

            float v00 = 2f / (right - left);
            float v11 = 2f / (top - bottom);
            float v22 = -2f / (farVal - nearVal);
            float v03 = -(right + left) / (right - left);
            float v13 = -(top + bottom) / (top - bottom);
            float v23 = -(farVal + nearVal) / (farVal - nearVal);

            m.MakeIdentity();
            m.SetColumnMajor(v00, 0, 0);
            m.SetColumnMajor(v11, 1, 1);
            m.SetColumnMajor(v22, 2, 2);
            m.SetColumnMajor(v03, 0, 3);
            m.SetColumnMajor(v13, 1, 3);
            m.SetColumnMajor(v23, 2, 3);

            return m;
        }

        public static Matrix4 SetToPerspectiveProjection(float fov, float aspectRatio, float nearVal, float farVal, bool fovInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            m.MakeIdentity();

            if (!fovInRad)
                fov *= Mathf.DEG2RAD;
            float tan2 = (float)System.Math.Tan(fov / 2.0);
            float v11 = 1f / tan2;
            float v00 = v11 / aspectRatio;
            float v22 = (-nearVal - farVal) / (nearVal - farVal);
            float v23 = (2 * farVal * nearVal) / (nearVal - farVal);

            m.SetColumnMajor(v00, 0, 0);
            m.SetColumnMajor(v11, 1, 1);
            m.SetColumnMajor(v22, 2, 2);
            m.SetColumnMajor(v23, 2, 3);
            m.SetColumnMajor(1, 3, 2);

            return m;
        }
        #endregion ProjectionMatrices

        #region 2DAffineTransformations
        public static Matrix3 SetTo2DTranslation(float x, float y, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            m.rc02 = x;
            m.rc12 = y;

            return m;
        }

        public static Matrix3 SetTo2DClockwiseRotation(float angleInRad, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);
            m.rc00 = c;
            m.rc01 = s;
            m.rc10 = -s;
            m.rc11 = c;

            return m;
        }

        public static Matrix3 SetTo2DCounterClockwiseRotation(float angleInRad, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);
            m.rc00 = c;
            m.rc01 = -s;
            m.rc10 = s;
            m.rc11 = c;

            return m;
        }

        public static Matrix3 SetTo2DClockwiseRotationAround(float angleInRad, float x, float y, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);
            m.rc00 = c;
            m.rc01 = s;
            m.rc10 = -s;
            m.rc11 = c;
            m.rc02 = x - x * m.rc00 - y * m.rc01;
            m.rc12 = y - x * m.rc10 - y * m.rc11;

            return m;
        }

        public static Matrix3 SetTo2DCounterClockwiseRotationAround(float angleInRad, float x, float y, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);
            m.rc00 = c;
            m.rc01 = -s;
            m.rc10 = s;
            m.rc11 = c;
            m.rc02 = x - x * m.rc00 - y * m.rc01;
            m.rc12 = y - x * m.rc10 - y * m.rc11;

            return m;
        }

        public static Matrix3 SetTo2DScaling(float x, float y, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            m.rc00 = x;
            m.rc11 = y;

            return m;
        }

        public static Matrix3 SetTo2DXShearing(float s, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            m.rc01 = s;

            return m;
        }

        public static Matrix3 SetTo2DYShearing(float s, Matrix3 m = null) {
            if (m == null)
                m = Matrix3.CreateIdentity();

            m.MakeIdentity();

            m.rc10 = s;

            return m;
        }

        #endregion 2DAffineTransformations

        #region 3DAffineTransformations
        public static Matrix4 SetTo3DLookAt(Vector3 eye, Vector3 direction, Vector3 up, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            Vector3 target = eye + direction;

            Vector3 z = (eye - target).Normalize();
            Vector3 x = Vector3.Cross(up, z).Normalize();
            Vector3 y = Vector3.Cross(z, x).Normalize();

            m.rc00 = x.x;
            m.rc01 = y.x;
            m.rc02 = z.x;
            m.rc03 = 0;

            m.rc10 = x.y;
            m.rc11 = y.y;
            m.rc12 = z.y;
            m.rc13 = 0;

            m.rc20 = x.z;
            m.rc21 = y.z;
            m.rc22 = z.z;
            m.rc23 = 0;

            m.rc30 = -((x.x * eye.x) + (x.y * eye.y) + (x.z * eye.z));
            m.rc31 = -((y.x * eye.x) + (y.y * eye.y) + (y.z * eye.z));
            m.rc32 = -((z.x * eye.x) + (z.y * eye.y) + (z.z * eye.z));
            m.rc33 = 1;

            return m;
        }

        public static Matrix4 LookAt3D(Matrix4 m, Vector3 eye, Vector3 direction, Vector3 up) {
            Matrix4 m2 = SetTo3DLookAt(eye, direction, up, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DTranslation(float x, float y, float z, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            m.MakeIdentity();

            m.rc03 = x;
            m.rc13 = y;
            m.rc23 = z;

            return m;
        }

        public static Matrix4 Translate3D(Matrix4 m, float x, float y, float z) {
            Matrix4 m2 = SetTo3DTranslation(x, y, z, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DXAxisClockwiseRotation(float angleInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            m.rc11 = c;
            m.rc12 = s;
            m.rc21 = -s;
            m.rc22 = c;

            return m;
        }


        public static Matrix4 RotateClockwise3DXAxis(Matrix4 m, float angleInRad) {
            Matrix4 m2 = SetTo3DXAxisClockwiseRotation(angleInRad, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DYAxisClockwiseRotation(float angleInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            m.rc00 = c;
            m.rc02 = -s;
            m.rc20 = s;
            m.rc22 = c;

            return m;
        }

        public static Matrix4 RotateClockwise3DYAxis(Matrix4 m, float angleInRad) {
            Matrix4 m2 = SetTo3DYAxisClockwiseRotation(angleInRad, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DZAxisClockwiseRotation(float angleInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            m.rc00 = c;
            m.rc01 = s;
            m.rc10 = -s;
            m.rc11 = c;

            return m;
        }

        public static Matrix4 RotateClockwise3DZAxis(Matrix4 m, float angleInRad) {
            Matrix4 m2 = SetTo3DZAxisClockwiseRotation(angleInRad, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DXAxisCounterClockwiseRotation(float angleInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            m.rc11 = c;
            m.rc12 = -s;
            m.rc21 = s;
            m.rc22 = c;

            return m;
        }

        public static Matrix4 RotateCounterClockwise3DXAxis(Matrix4 m, float angleInRad) {
            Matrix4 m2 = SetTo3DXAxisCounterClockwiseRotation(angleInRad, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DYAxisCounterClockwiseRotation(float angleInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            m.rc00 = c;
            m.rc02 = s;
            m.rc20 = -s;
            m.rc22 = c;

            return m;
        }

        public static Matrix4 RotateCounterClockwise3DYAxis(Matrix4 m, float angleInRad) {
            Matrix4 m2 = SetTo3DYAxisCounterClockwiseRotation(angleInRad, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DZAxisCounterClockwiseRotation(float angleInRad, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            m.rc00 = c;
            m.rc01 = -s;
            m.rc10 = s;
            m.rc11 = c;

            return m;
        }

        public static Matrix4 RotateCounterClockwise3DZAxis(Matrix4 m, float angleInRad) {
            Matrix4 m2 = SetTo3DZAxisCounterClockwiseRotation(angleInRad, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }

        public static Matrix4 SetTo3DScaling(float x, float y, float z, Matrix4 m = null) {
            if (m == null)
                m = Matrix4.CreateIdentity();

            m.MakeIdentity();

            m.rc00 = x;
            m.rc11 = y;
            m.rc22 = z;

            return m;
        }

        public static Matrix4 Scale3D(Matrix4 m, float x, float y, float z) {
            Matrix4 m2 = SetTo3DScaling(x, y, z, mPool4.Get());

            m = m.MultiplyLeft(m2);

            mPool4.Put(m2);

            return m;
        }
        #endregion 3DAffineTransformations
    }
}