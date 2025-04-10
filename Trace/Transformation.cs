namespace Trace
{
    public struct Transformation
    {
        public static readonly float[,] IdentityMatr4X4 = new[,]
        {
            { 1.0f, 0.0f, 0.0f, 0.0f },
            { 0.0f, 1.0f, 0.0f, 0.0f },
            { 0.0f, 0.0f, 1.0f, 0.0f },
            { 0.0f, 0.0f, 0.0f, 1.0f }
        };

        public float[,] M;
        public float[,] Invm;

        // Constructor
        public Transformation(float[,]? m = null, float[,]? invm = null)
        {
            M = m ?? IdentityMatr4X4;
            Invm = invm ?? IdentityMatr4X4;
        }

        public static bool AreClose(Transformation v1, Transformation v2, float sigma = 1e-5f)
        {
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (Color.are_close(v1.M[i, j], v2.M[i, j], sigma)) return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Method that calculates the matrix multiplication (row by column) of two 4x4 matrices a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Transformation MatrProd(Transformation a, Transformation b)
        {
            var result = new Transformation();
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (result.M != null)
                            result.M[i, j] += a.M[i, k] * b.M[k, j];
                    }
                }
            }
            return result;
        }
        
        public static Transformation operator *(Transformation t1, Transformation t2)
        {
            // Multiply the forward matrices to get the resulting transformation matrix.
            var resultM = MatrProd(t1, t2);  
            // Multiply the inverse matrices in reverse order.
            // Wrap the inverse matrices (of type float[,]) into Transformation objects.
            var resultInvm = MatrProd(new Transformation(t2.Invm), new Transformation(t1.Invm));  
            // Return a new Transformation with the computed forward matrix and inverse matrix.
            return new Transformation(resultM.M, resultInvm.M);
        }

         
        /// <summary>
        /// Checks if the transformation is consistent (i.e. M * M^-1 equals the identity matrix).
        /// </summary>
        public bool IsConsistent()
        {
            // Compute the product M * Invm as a Transformation
            Transformation prod = MatrProd(this, new Transformation(Invm));

            // Compare each element of the resulting matrix with the identity matrix
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Math.Abs(prod.M[i, j] - IdentityMatr4X4[i, j]) > 1e-5f)
                    {
                        return false;
                    }
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

        public Point Apply(Point p)
        {
            // Decompose the matrix into rows
            float[] row0 = [M[0, 0], M[0, 1], M[0, 2], M[0, 3]];
            float[] row1 = [M[1, 0], M[1, 1], M[1, 2], M[1, 3]];
            float[] row2 = [M[2, 0], M[2, 1], M[2, 2], M[2, 3]];
            float[] row3 = [M[3, 0], M[3, 1], M[3, 2], M[3, 3]];

            // Multiply the matrix by the point
            float newX = p.X * row0[0] + p.Y * row0[1] + p.Z * row0[2] + row0[3];
            float newY = p.X * row1[0] + p.Y * row1[1] + p.Z * row1[2] + row1[3];
            float newZ = p.X * row2[0] + p.Y * row2[1] + p.Z * row2[2] + row2[3];
            float w = p.X * row3[0] + p.Y * row3[1] + p.Z * row3[2] + row3[3];

            // If w == 1, return the transformed point directly
            if (Color.are_close(w, 1f))
            {
                return new Point(newX, newY, newZ);
            }
            else
            {
                // Otherwise, normalize the point by dividing by w
                return new Point(newX / w, newY / w, newZ / w);
            }
        }
        
        public Vec Apply(Vec v)
        {
            // Decompose the matrix; use only the first 3 columns of the first 3 rows
            float[] row0 = [M[0, 0], M[0, 1], M[0, 2]];
            float[] row1 = [M[1, 0], M[1, 1], M[1, 2]];
            float[] row2 = [M[2, 0], M[2, 1], M[2, 2]];

            float newX = v.X * row0[0] + v.Y * row0[1] + v.Z * row0[2];
            float newY = v.X * row1[0] + v.Y * row1[1] + v.Z * row1[2];
            float newZ = v.X * row2[0] + v.Y * row2[1] + v.Z * row2[2];

            return new Vec(newX, newY, newZ);
        }
        
        /// <summary>
        /// Applies the inverse transformation to a normal. Since normals transform with
        /// the inverse-transpose of the transformation matrix, this method uses the inverse (Invm)
        /// and only the first three rows to transform the Normal.
        /// </summary>
        /// <param name="n">The normal to transform.</param>
        /// <returns>The transformed normal.</returns>
        public Normal Apply(Normal n)
        {
            // Decompose the inverse matrix (Invm) and use only the first three rows:
            float[] row0 = [Invm[0, 0], Invm[0, 1], Invm[0, 2]];
            float[] row1 = [Invm[1, 0], Invm[1, 1], Invm[1, 2]];
            float[] row2 = [Invm[2, 0], Invm[2, 1], Invm[2, 2]];
            
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
