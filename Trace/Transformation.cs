/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using System;
using System.Text;

namespace Trace
{
    public struct Transformation
    {
        public static readonly float[,] IdentityMatr4X4 = new float[4, 4]
        {
            { 1.0f, 0.0f, 0.0f, 0.0f },
            { 0.0f, 1.0f, 0.0f, 0.0f },
            { 0.0f, 0.0f, 1.0f, 0.0f },
            { 0.0f, 0.0f, 0.0f, 1.0f }
        };

        public float[,] M;
        public float[,] Invm;

        // Explicit parameterless constructor (C# 10 or later)
        public Transformation()
        {
            // Initializes M and Invm to the identity matrix
            M = IdentityMatr4X4;
            Invm = IdentityMatr4X4;
        }

        // Constructor with parameters as before
        public Transformation(float[,]? m, float[,]? invm) : this()
        {
            M = m ?? IdentityMatr4X4;
            Invm = invm ?? IdentityMatr4X4;
        }

        /// <summary>
        /// Compare 2 matrices (direct and inverse)
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="sigma=1e-5"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public static bool AreClose(Transformation t1, Transformation t2, float sigma = 1e-4f)
        {
            // Compare the forward matrices
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (!Color.are_close(t1.M[i, j], t2.M[i, j], sigma))
                        return false;
                }
            }

            // Compare the inverse matrices as well
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (!Color.are_close(t1.Invm[i, j], t2.Invm[i, j], sigma))
                        return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Matrix M:");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    sb.AppendFormat("{0,8:F3} ", M[i, j]);
                sb.AppendLine();
            }

            sb.AppendLine("Matrix Invm:");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    sb.AppendFormat("{0,8:F3} ", Invm[i, j]);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public Transformation Inverse()
        {
            return new Transformation(this.Invm, this.M);
        }

        /// <summary>
        /// Method that calculates the matrix multiplication (row by column) of two 4x4 matrices a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Transformation MatrProd(Transformation a, Transformation b)
        {
            float[,] m = new float[4, 4];
            float[,] invm = new float[4, 4];

            // Calculates m = a.M * b.M
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    m[i, j] = 0f;
                    for (int k = 0; k < 4; k++)
                        m[i, j] += a.M[i, k] * b.M[k, j];
                }
            }

            // Calculates invm = b.Invm * a.Invm (inverted order!)
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    invm[i, j] = 0f;
                    for (int k = 0; k < 4; k++)
                        invm[i, j] += b.Invm[i, k] * a.Invm[k, j];
                }
            }

            return new Transformation(m, invm);
        }


        /// <summary>
        /// Overload of the * operator for the row-column product of 2 matrices
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static Transformation operator *(Transformation t1, Transformation t2)
        {
            // Multiply the forward matrices to get the resulting transformation matrix.
            var result = MatrProd(t1, t2);
            // Multiply the inverse matrices in reverse order.
            return new Transformation(result.M, result.Invm);
        }

        /// <summary>
        /// Checks if the transformation is consistent (i.e. M * M^-1 equals the identity matrix).
        /// </summary>
        public bool IsConsistent()
        {
            // Wrap the Invm matrix in a new Transformation (so that both M and Invm are set to Invm).
            Transformation invTransform = new Transformation(Invm, Invm);
    
            // Compute the product: this.M * this.InvM (using our defined MatrProd).
            Transformation prod = this * invTransform;

            // Compare each element of the resulting matrix with the identity.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Math.Abs(prod.M[i, j] - IdentityMatr4X4[i, j]) > 1e-4f)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Method that performs a translation by vector v.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Transformation Translation(Vec v)
        {
            float[,] m = new[,]
            {
                { 1f, 0f, 0f, v.X },
                { 0f, 1f, 0f, v.Y },
                { 0f, 0f, 1f, v.Z },
                { 0f, 0f, 0f, 1f }
            };

            float[,] invM = new[,]
            {
                { 1f, 0f, 0f, -v.X },
                { 0f, 1f, 0f, -v.Y },
                { 0f, 0f, 1f, -v.Z },
                { 0f, 0f, 0f, 1f }
            };

            return new Transformation(m, invM);
        }
        
        /// <summary>
        /// Method that performs a scaling transformation using vector v.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Transformation Scaling(Vec v)
        {
            float[,] m = new[,]
            {
                { v.X, 0f,     0f,     0f },
                { 0f,    v.Y,  0f,     0f },
                { 0f,    0f,     v.Z,  0f },
                { 0f,    0f,     0f,     1f }
            };

            float[,] invm = new[,]
            {
                { 1f / v.X, 0f,         0f,         0f },
                { 0f,         1f / v.Y, 0f,         0f },
                { 0f,         0f,         1f / v.Z, 0f },
                { 0f,         0f,         0f,         1f }
            };

            return new Transformation(m, invm);
        }
        
        /// <summary>
        /// Method that performs a rotation (in degrees) around the X axis.
        /// </summary>
        /// <param name="angleDeg"></param>
        /// <returns></returns>
        public static Transformation RotationX(float angleDeg)
        {
            float radians = angleDeg * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float[,] m = new[,]
            {
                { 1f,  0f,   0f,  0f },
                { 0f,  cos, -sin, 0f },
                { 0f,  sin,  cos, 0f },
                { 0f,  0f,   0f,  1f }
            };

            float[,] invm = new[,]
            {
                { 1f,  0f,   0f,  0f },
                { 0f,  cos,  sin, 0f },
                { 0f, -sin,  cos, 0f },
                { 0f,  0f,   0f,  1f }
            };

            return new Transformation(m, invm);
        }
        
        /// <summary>
        /// Method that performs a rotation (in degrees) around the Y axis.
        /// </summary>
        /// <param name="angleDeg"></param>
        /// <returns></returns>
        public static Transformation RotationY(float angleDeg)
        {
            float radians = angleDeg * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float[,] m = new[,]
            {
                { cos,  0f,   sin, 0f },
                { 0f,   1f,   0f,  0f },
                { -sin, 0f,   cos, 0f },
                { 0f,   0f,   0f, 1f }
            };

            float[,] invm = new[,]
            {
                { cos,  0f,   -sin, 0f },
                { 0f,   1f,   0f,  0f },
                { sin,  0f,   cos,  0f },
                { 0f,   0f,   0f, 1f }
            };

            return new Transformation(m, invm);
        }
        
        /// <summary>
        /// Method that performs a rotation (in degrees) around the Z axis.
        /// </summary>
        /// <param name="angleDeg"></param>
        /// <returns></returns>
        public static Transformation RotationZ(float angleDeg)
        {
            float radians = angleDeg * (float)Math.PI / 180f;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float[,] m = new[,]
            {
                { cos,  -sin, 0f,  0f },
                { sin,  cos,  0f,  0f },
                { 0f,   0f,   1f,  0f },
                { 0f,   0f,   0f,  1f }
            };

            float[,] invm = new[,]
            {
                { cos,  sin, 0f,  0f },
                { -sin, cos, 0f,  0f },
                { 0f,   0f,  1f,  0f },
                { 0f,   0f,  0f,  1f }
            };

            return new Transformation(m, invm);
        }

        public static Point operator *(Transformation t, Point p)
        {
            // Decompose the matrix t.M into rows
            float[] row0 = [t.M[0, 0], t.M[0, 1], t.M[0, 2], t.M[0, 3]];
            float[] row1 = [t.M[1, 0], t.M[1, 1], t.M[1, 2], t.M[1, 3]];
            float[] row2 = [t.M[2, 0], t.M[2, 1], t.M[2, 2], t.M[2, 3]];
            float[] row3 = [t.M[3, 0], t.M[3, 1], t.M[3, 2], t.M[3, 3]];

            // Multiply the matrix by the point
            float newX = p.X * row0[0] + p.Y * row0[1] + p.Z * row0[2] + row0[3];
            float newY = p.X * row1[0] + p.Y * row1[1] + p.Z * row1[2] + row1[3];
            float newZ = p.X * row2[0] + p.Y * row2[1] + p.Z * row2[2] + row2[3];
            float w = p.X * row3[0] + p.Y * row3[1] + p.Z * row3[2] + row3[3];

            // If w is close to 1, return the transformed point directly
            if (Math.Abs(w - 1f) < 1e-5f)
            {
                return new Point(newX, newY, newZ);
            }
            else
            {
                // Otherwise, normalize the point by dividing by w
                return new Point(newX / w, newY / w, newZ / w);
            }
        }

        /// <summary>
        /// Product of a transformation (matrix) and a vector
        /// </summary>
        /// <param name="t"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vec operator *(Transformation t, Vec v)
        {
            // Decompose the matrix t.M (only the first 3 elements of the 3x3 matrix)
            float newX = v.X * t.M[0, 0] + v.Y * t.M[0, 1] + v.Z * t.M[0, 2];
            float newY = v.X * t.M[1, 0] + v.Y * t.M[1, 1] + v.Z * t.M[1, 2];
            float newZ = v.X * t.M[2, 0] + v.Y * t.M[2, 1] + v.Z * t.M[2, 2];

            // Return the new transformed vector
            return new Vec(newX, newY, newZ);
        }

        /// <summary>
        /// Applies the inverse transformation to a normal. Since normals transform with
        /// the inverse-transpose of the transformation matrix, this method uses the inverse (Invm)
        /// and only the first three rows to transform the Normal.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="n">The normal to transform.</param>
        /// <returns>The transformed normal.</returns>
        public static Normal operator *(Transformation t, Normal n)
        {
            // Decompose the inverse matrix t.Invm into rows
            float[] row0 = [t.Invm[0, 0], t.Invm[0, 1], t.Invm[0, 2]];
            float[] row1 = [t.Invm[1, 0], t.Invm[1, 1], t.Invm[1, 2]];
            float[] row2 = [t.Invm[2, 0], t.Invm[2, 1], t.Invm[2, 2]];

            // Multiply the inverse matrix by the normal
            float newX = n.X * row0[0] + n.Y * row1[0] + n.Z * row2[0];
            float newY = n.X * row0[1] + n.Y * row1[1] + n.Z * row2[1];
            float newZ = n.X * row0[2] + n.Y * row1[2] + n.Z * row2[2];

            return new Normal(newX, newY, newZ);
        }

        /// <summary>
        /// Applies (composes) another transformation with this one.
        /// The result is equivalent to applying this transformation followed by the given one.
        /// </summary>
        /// <param name="other">The transformation to apply after this one.</param>
        /// <returns>The resulting composed transformation.</returns>
        public Transformation Apply(Transformation other)
        {
            float[,] resultM = new float[4, 4];
            float[,] resultInvM = new float[4, 4];

            // Compute resultM = this.M * other.M
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    resultM[i, j] = 0f;
                    for (int k = 0; k < 4; k++)
                    {
                        resultM[i, j] += this.M[i, k] * other.M[k, j];
                    }
                }
            }

            // Compute resultInvM = other.Invm * this.Invm (note the reverse order)
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    resultInvM[i, j] = 0f;
                    for (int k = 0; k < 4; k++)
                    {
                        resultInvM[i, j] += other.Invm[i, k] * this.Invm[k, j];
                    }
                }
            }

            return new Transformation(resultM, resultInvM);
        }
        
    }
}
