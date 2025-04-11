using Xunit;
using Xunit.Abstractions;

namespace Trace.Tests;

public class TransformationsTest(ITestOutputHelper testOutputHelper)
{
        // Helper method to deep clone a 4x4 float matrix.
        private float[,] DeepClone(float[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            float[,] clone = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    clone[i, j] = matrix[i, j];
                }
            }

            return clone;
        }
        private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
        [Fact]
        public void TestIsClose()
        {
            // Arrange: create transformation m1 with given matrices.
            Transformation m1 = new Transformation(
                m: new[,]
                {
                    { 1.0f, 2.0f, 3.0f, 4.0f },
                    { 5.0f, 6.0f, 7.0f, 8.0f },
                    { 9.0f, 9.0f, 8.0f, 7.0f },
                    { 6.0f, 5.0f, 4.0f, 1.0f },
                },
                invm: new[,]
                {
                    { -3.75f,   2.75f, -1.0f,  0.0f },
                    { 4.375f, -3.875f,  2.0f, -0.5f },
                    { 0.5f,    0.5f,  -1.0f,  1.0f },
                    { -1.375f,  0.875f,  0.0f, -0.5f },
                }
            );

            // Assert that m1 is consistent.
            Assert.True(m1.IsConsistent(), "m1 should be consistent.");

            // Create m2 as a deep clone of m1.
            Transformation m2 = new Transformation(DeepClone(m1.M), DeepClone(m1.Invm));
            Assert.True(Transformation.AreClose(m1, m2), "m1 should be close to m2.");

            // Create m3 as a deep clone, then modify an element in m3.M,
            // so that m3 becomes "not close" to m1.
            Transformation m3 = new Transformation(DeepClone(m1.M), DeepClone(m1.Invm));
            m3.M[2, 2] += 1.0f;  // This modification should break consistency.
            Assert.False(Transformation.AreClose(m1, m3), "m1 should not be close to m3.");

            // Create m4 as a deep clone, then modify an element in m4.Invm.
            Transformation m4 = new Transformation(DeepClone(m1.M), DeepClone(m1.Invm));
            m4.Invm[2, 2] += 1.0f;  // This modification should also break closeness.
            Assert.False(Transformation.AreClose(m1, m4), "m1 should not be close to m4.");
        }
        
        [Fact]
        public void TestMultiplication()
        {
            var m1 = new Transformation(
                new[,]
                {
                    {1.0f, 2.0f, 3.0f, 4.0f},
                    {5.0f, 6.0f, 7.0f, 8.0f},
                    {9.0f, 9.0f, 8.0f, 7.0f},
                    {6.0f, 5.0f, 4.0f, 1.0f},
                },
                new[,]
                {
                    {-3.75f, 2.75f, -1.0f, 0.0f},
                    {4.375f, -3.875f, 2.0f, -0.5f},
                    {0.5f, 0.5f, -1.0f, 1.0f},
                    {-1.375f, 0.875f, 0.0f, -0.5f},
                }
            );
            Assert.True(m1.IsConsistent());

            var m2 = new Transformation(
                new[,]
                {
                    {3.0f, 5.0f, 2.0f, 4.0f},
                    {4.0f, 1.0f, 0.0f, 5.0f},
                    {6.0f, 3.0f, 2.0f, 0.0f},
                    {1.0f, 4.0f, 2.0f, 1.0f},
                },
                new[,]
                {
                    {0.4f, -0.2f, 0.2f, -0.6f},
                    {2.9f, -1.7f, 0.2f, -3.1f},
                    {-5.55f, 3.15f, -0.4f, 6.45f},
                    {-0.9f, 0.7f, -0.2f, 1.1f},
                }
            );
            Assert.True(m2.IsConsistent());

            var expected = new Transformation(
                new[,]
                {
                    {33.0f, 32.0f, 16.0f, 18.0f},
                    {89.0f, 84.0f, 40.0f, 58.0f},
                    {118.0f, 106.0f, 48.0f, 88.0f},
                    {63.0f, 51.0f, 22.0f, 50.0f},
                },
                new[,]
                {
                    {-1.45f, 1.45f, -1.0f, 0.6f},
                    {-13.95f, 11.95f, -6.5f, 2.6f},
                    {25.525f, -22.025f, 12.25f, -5.2f},
                    {4.825f, -4.325f, 2.5f, -1.1f},
                }
            );
            Assert.True(expected.IsConsistent());
            
            var prod = m1 * m2; 
            Assert.True(Transformation.AreClose(expected, prod));
        }
        
        [Fact]
        public void TestVecPointMultiplication()
        {
            var m = new Transformation(
                new[,]
                {
                    {1f, 2f, 3f, 4f},
                    {5f, 6f, 7f, 8f},
                    {9f, 9f, 8f, 7f},
                    {0f, 0f, 0f, 1f}
                },
                new[,]
                {
                    {-3.75f, 2.75f, -1f, 0f},
                    {5.75f, -4.75f, 2f, 1f},
                    {-2.25f, 2.25f, -1f, -2f},
                    {0f, 0f, 0f, 1f}
                }
            );

            Assert.True(m.IsConsistent());

            var expectedV = new Vec(14f, 38f, 51f);
            Assert.True(Vec.AreClose(expectedV, m * new Vec(1f, 2f, 3f)));

            var expectedP = new Point(18f, 46f, 58f);
            Assert.True(Point.AreClose(expectedP, m * new Point(1f, 2f, 3f)));

            var expectedN = new Normal(-8.75f, 7.75f, -3f);
            Assert.True(Normal.AreClose(expectedN, m * new Normal(3f, 2f, 4f)));
        }
        
        [Fact]
        public void TestInverse()
        {
            // Create a transformation m1 with matrix M and InvM
            Transformation m1 = new Transformation(
                new[,] {
                    { 1.0f, 2.0f, 3.0f, 4.0f },
                    { 5.0f, 6.0f, 7.0f, 8.0f },
                    { 9.0f, 9.0f, 8.0f, 7.0f },
                    { 6.0f, 5.0f, 4.0f, 1.0f }
                },
                new[,] {
                    { -3.75f, 2.75f, -1.0f, 0.0f },
                    { 4.375f, -3.875f, 2.0f, -0.5f },
                    { 0.5f, 0.5f, -1.0f, 1.0f },
                    { -1.375f, 0.875f, 0.0f, -0.5f }
                }
            );

            // Calculate the inverse
            Transformation m2 = m1.Inverse();
            
            //_testOutputHelper.WriteLine(m2.ToString());

            // Verify that the inverse is consistent.
            Assert.True(m2.IsConsistent(), "Inverse should be consistent.");

            // Calculate the product between m1 and its inverse.
            Transformation prod = m1 * m2;
            
            //_testOutputHelper.WriteLine(new Transformation().ToString());

            // Verify that the product equals the identity matrix.
            Assert.True(prod.IsConsistent(), "Product should be consistent.");
            Assert.True(Transformation.AreClose(prod, new Transformation()), "Product should equal the identity transformation.");
        }
        
        [Fact]
        public void TestTranslations()
        {
            var p = new Point(3f,4f,5f);
            var p0 = p.Translation(new Vec(1f, 2f, 3f));
            
            Assert.True(Point.AreClose(p0, new Point(4f,6f,8f)));
            
            // Create two translation transformations
            Transformation tr1 = Transformation.Translation(new Vec(1.0f, 2.0f, 3.0f));
            Assert.True(tr1.IsConsistent(), "tr1 should be consistent.");

            Transformation tr2 = Transformation.Translation(new Vec(4.0f, 6.0f, 8.0f));
            Assert.True(tr2.IsConsistent(), "tr2 should be consistent.");

            // Multiply the two transformations
            Transformation prod = tr1 * tr2;
            Assert.True(prod.IsConsistent(), "Product should be consistent.");

            // The sum of the translations (1,2,3)+(4,6,8) = (5,8,11)
            Transformation expected = Transformation.Translation(new Vec(5.0f, 8.0f, 11.0f));
            Assert.True(Transformation.AreClose(prod, expected), "Product should equal the expected translation.");
        }
        
        [Fact]
        public void TestRotations()
        {
            
            var vecX = new Vec(1f, 0f, 0f);
            var vecY = new Vec(0f, 1f, 0f);
            var vecZ = new Vec(0f, 0f, 1f);
            // Check the consistency of small rotations
            Assert.True(Transformation.RotationX(0.1f).IsConsistent(), "RotationX(0.1°) should be consistent.");
            Assert.True(Transformation.RotationY(0.1f).IsConsistent(), "RotationY(0.1°) should be consistent.");
            Assert.True(Transformation.RotationZ(0.1f).IsConsistent(), "RotationZ(0.1°) should be consistent.");

            // Verify 90° rotations
            // For example, rotating the vector Y by 90° about the X axis should yield the vector Z (or its negative, depending on the right-hand rule convention)
            // Assuming that rotation_x(90) transforms VEC_Y into VEC_Z:
           
            //Transformation rx = Transformation.RotationX(90f);
            //Vec resultX = rx * vecY;

            var resultX = vecY.RotationX(90f);
            
            Assert.True(Vec.AreClose(resultX, vecZ), "RotationX(90°) * VEC_Y should be close to VEC_Z.");

            // Similarly for Y and Z:
            //Transformation ry = Transformation.RotationY(90f);
            //Vec resultY = ry * vecZ;
            
            var resultY = vecZ.RotationY(90f);
            
            Assert.True(Vec.AreClose(resultY, vecX), "RotationY(90°) * VEC_Z should be close to VEC_X.");

            //Transformation rz = Transformation.RotationZ(90f);
            //Vec resultZ = rz * vecX;
            
            var resultZ = vecX.RotationZ(90f);
            
            Assert.True(Vec.AreClose(resultZ, vecY), "RotationZ(90°) * VEC_X should be close to VEC_Y.");
        }
}
