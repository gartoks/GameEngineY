using GameEngine.Math;

namespace GameEngine.Graphics.Utility {
    public static class MatrixTransformationHelper {
        #region Setter
        public static Vector4 MakePoint(this Vector2 v) => new Vector4(v.x, v.y, 0, 1);

        public static Vector4 MakeVector(this Vector2 v) => new Vector4(v.x, v.y, 0, 0);

        public static Vector4 MakePoint(this Vector3 v) => new Vector4(v.x, v.y, v.z, 1);

        public static Vector4 MakeVector(this Vector3 v) => new Vector4(v.x, v.y, v.z, 0);
        #endregion

        #region ProjectionMatrices
        public static Matrix4 MakeOrthographicProjection(this Matrix4 m, float left, float right, float bottom, float top, float near, float far) {
            m.MakeIdentity();

            float v00 = 2f / (right - left);
            float v11 = 2f / (top - bottom);
            float v22 = -2f / (far - near);
            float v03 = -(right + left) / (right - left);
            float v13 = -(top + bottom) / (top - bottom);
            float v23 = -(far + near) / (far - near);

            m.SetColumnMajor(v00, 0, 0);
            m.SetColumnMajor(v11, 1, 1);
            m.SetColumnMajor(v22, 2, 2);
            m.SetColumnMajor(v03, 0, 3);
            m.SetColumnMajor(v13, 1, 3);
            m.SetColumnMajor(v23, 2, 3);

            return m;
        }

        public static Matrix4 MakePerspectiveProjection(this Matrix4 m, float fovInRad, float aspectRatio, float near, float far) {
            m.MakeIdentity();

            float tan2 = (float)System.Math.Tan(fovInRad / 2.0);
            float v11 = 1f / tan2;
            float v00 = v11 / aspectRatio;
            float v22 = (-near - far) / (near - far);
            float v23 = (2 * far * near) / (near - far);

            m.SetColumnMajor(v00, 0, 0);
            m.SetColumnMajor(v11, 1, 1);
            m.SetColumnMajor(v22, 2, 2);
            m.SetColumnMajor(v23, 2, 3);
            m.SetColumnMajor(1, 3, 2);

            return m;
        }
        #endregion ProjectionMatrices

        #region Transformations
        #region Translation
        public static Matrix4 MakeTranslation(this Matrix4 m, Vector2 v) => MakeTranslation(m, v.x, v.y);
        public static Matrix4 MakeTranslation(this Matrix4 m, float x, float y) {
            m.MakeIdentity();

            m.rc03 = x;
            m.rc13 = y;
            m.rc23 = 0;

            return m;
        }

        public static Matrix4 MakeTranslation(this Matrix4 m, Vector3 v) => MakeTranslation(m, v.x, v.y, v.z);
        public static Matrix4 MakeTranslation(this Matrix4 m, float x, float y, float z) {
            m.MakeIdentity();

            m.rc03 = x;
            m.rc13 = y;
            m.rc23 = z;

            return m;
        }

        public static Matrix4 Translate(this Matrix4 m, Vector2 v) => Translate(m, v.x, v.y);
        public static Matrix4 Translate(this Matrix4 m, float x, float y) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeTranslation(x, y));
        }

        public static Matrix4 Translate(this Matrix4 m, Vector3 v) => Translate(m, v.x, v.y, v.z);
        public static Matrix4 Translate(this Matrix4 m, float x, float y, float z) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeTranslation(x, y, z));
        }
        #endregion Translation

        #region Rotation
        public static Matrix4 MakeRotationX(this Matrix4 m, float angleInRad, bool clockwise) {
            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            if (clockwise) {
                m.rc11 = c;
                m.rc12 = s;
                m.rc21 = -s;
                m.rc22 = c;
            } else {
                m.rc11 = c;
                m.rc12 = -s;
                m.rc21 = s;
                m.rc22 = c;
            }

            return m;
        }

        public static Matrix4 RotateX(this Matrix4 m, float angleInRad, bool clockwise) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeRotationX(angleInRad, clockwise));
        }

        public static Matrix4 MakeRotationAroundX(this Matrix4 m, Vector4 p, float angleInRad, bool clockwise) => MakeRotationAroundZ(m, p.y, p.z, angleInRad, clockwise);
        public static Matrix4 MakeRotationAroundX(this Matrix4 m, float y, float z, float angleInRad, bool clockwise) {
            m.MakeIdentity();

            m.Translate(0, -y, -z);
            m.RotateZ(angleInRad, clockwise);
            m.Translate(0, y, z);

            return m;
        }

        public static Matrix4 RotateAroundX(this Matrix4 m, Vector4 p, float angleInRad, bool clockwise) => RotateAroundX(m, p.y, p.z, angleInRad, clockwise);
        public static Matrix4 RotateAroundX(this Matrix4 m, float y, float z, float angleInRad, bool clockwise) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeRotationAroundX(y, z, angleInRad, clockwise));
        }

        public static Matrix4 MakeRotationY(this Matrix4 m, float angleInRad, bool clockwise) {
            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            if (clockwise) {
                m.rc00 = c;
                m.rc02 = -s;
                m.rc20 = s;
                m.rc22 = c;
            } else {
                m.rc00 = c;
                m.rc02 = s;
                m.rc20 = -s;
                m.rc22 = c;
            }

            return m;
        }

        public static Matrix4 RotateY(this Matrix4 m, float angleInRad, bool clockwise) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeRotationY(angleInRad, clockwise));
        }

        public static Matrix4 MakeRotationAroundY(this Matrix4 m, Vector4 p, float angleInRad, bool clockwise) => MakeRotationAroundZ(m, p.x, p.z, angleInRad, clockwise);
        public static Matrix4 MakeRotationAroundY(this Matrix4 m, float x, float z, float angleInRad, bool clockwise) {
            m.MakeIdentity();

            m.Translate(-x, 0, -z);
            m.RotateZ(angleInRad, clockwise);
            m.Translate(x, 0, z);

            return m;
        }

        public static Matrix4 RotateAroundY(this Matrix4 m, Vector4 p, float angleInRad, bool clockwise) => RotateAroundX(m, p.x, p.z, angleInRad, clockwise);
        public static Matrix4 RotateAroundY(this Matrix4 m, float x, float z, float angleInRad, bool clockwise) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeRotationAroundY(x, z, angleInRad, clockwise));
        }

        public static Matrix4 MakeRotationZ(this Matrix4 m, float angleInRad, bool clockwise) {
            float s = (float)System.Math.Sin(angleInRad);
            float c = (float)System.Math.Cos(angleInRad);

            m.MakeIdentity();

            if (clockwise) {
                m.rc00 = c;
                m.rc01 = s;
                m.rc10 = -s;
                m.rc11 = c;
            } else {
                m.rc00 = c;
                m.rc01 = -s;
                m.rc10 = s;
                m.rc11 = c;
            }

            return m;
        }

        public static Matrix4 RotateZ(this Matrix4 m, float angleInRad, bool clockwise) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeRotationZ(angleInRad, clockwise));
        }

        public static Matrix4 MakeRotationAroundZ(this Matrix4 m, Vector4 p, float angleInRad, bool clockwise) => MakeRotationAroundZ(m, p.x, p.y, angleInRad, clockwise);
        public static Matrix4 MakeRotationAroundZ(this Matrix4 m, float x, float y, float angleInRad, bool clockwise) {
            m.MakeIdentity();

            m.Translate(-x, -y, 0);
            m.RotateZ(angleInRad, clockwise);
            m.Translate(x, y, 0);

            return m;
        }

        public static Matrix4 RotateAroundZ(this Matrix4 m, Vector4 p, float angleInRad, bool clockwise) => RotateAroundX(m, p.x, p.y, angleInRad, clockwise);
        public static Matrix4 RotateAroundZ(this Matrix4 m, float x, float y, float angleInRad, bool clockwise) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeRotationAroundZ(x, y, angleInRad, clockwise));
        }
        #endregion Rotation

        #region Scale
        public static Matrix4 MakeScaling(this Matrix4 m, Vector2 v) => MakeTranslation(m, v.x, v.y);
        public static Matrix4 MakeScaling(this Matrix4 m, float x, float y) {
            m.MakeIdentity();

            m.rc00 = x;
            m.rc11 = y;
            m.rc22 = 1;

            return m;
        }

        public static Matrix4 MakeScaling(this Matrix4 m, Vector3 v) => MakeTranslation(m, v.x, v.y, v.z);
        public static Matrix4 MakeScaling(this Matrix4 m, float x, float y, float z) {
            m.MakeIdentity();

            m.rc00 = x;
            m.rc11 = y;
            m.rc22 = z;

            return m;
        }

        public static Matrix4 Scale(this Matrix4 m, Vector2 v) => Translate(m, v.x, v.y);
        public static Matrix4 Scale(this Matrix4 m, float x, float y) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeScaling(x, y));
        }

        public static Matrix4 Scale(this Matrix4 m, Vector3 v) => Translate(m, v.x, v.y, v.z);
        public static Matrix4 Scale(this Matrix4 m, float x, float y, float z) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeScaling(x, y, z));
        }
        #endregion Scale

        #region Combined
        public static Matrix4 MakeTransformation(this Matrix4 m, Vector2 t, float angleInRad, bool clockwise, Vector2 s) => MakeTransformation(m, t.x, t.y, angleInRad, clockwise, s.x, s.y);
        public static Matrix4 MakeTransformation(this Matrix4 m, float dx, float dy, float angleInRad, bool clockwise, float sx, float sy) {
            m.MakeIdentity();

            m.Scale(sx, sy);
            m.RotateZ(angleInRad, clockwise);
            m.Translate(dx, dy);

            return m;
        }

        public static Matrix4 Transform(this Matrix4 m, Vector2 t, float angleInRad, bool clockwise, Vector2 s) => Transform(m, t.x, t.y, angleInRad, clockwise, s.x, s.y);
        public static Matrix4 Transform(this Matrix4 m, float dx, float dy, float angleInRad, bool clockwise, float sx, float sy) {
            return m.MultiplyLeft(Matrix4.CreateIdentity().MakeTransformation(dx, dy, angleInRad, clockwise, sx, sy));
        }
        #endregion Combined
        #endregion Transformations
    }
}