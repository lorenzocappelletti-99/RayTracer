using Xunit;

namespace Trace.Tests;

public class TransformationsTest
{
        //Helper method to deep clone a 4x4 float matrix.
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

        [Fact]
        public void TestIsClose()
        {
            // Arrange: create transformation m1 with given matrices.
            Transformation m1 = new Transformation(
                m: new float[,]
                {
                    { 1.0f, 2.0f, 3.0f, 4.0f },
                    { 5.0f, 6.0f, 7.0f, 8.0f },
                    { 9.0f, 9.0f, 8.0f, 7.0f },
                    { 6.0f, 5.0f, 4.0f, 1.0f },
                },
                invm: new float[,]
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
            Assert.True(Transformation.AreClose(m1,m2), "m1 should be close to m2.");

            // Create m3 as a deep clone, then modify an element in m3.M,
            // so that m3 becomes "not close" to m1.
            Transformation m3 = new Transformation(DeepClone(m1.M), DeepClone(m1.Invm));
            m3.M[2, 2] += 1.0f;  // This modification should break consistency.
            Assert.False(Transformation.AreClose(m1,m3), "m1 should not be close to m3.");

            // Create m4 as a deep clone, then modify an element in m4.Invm.
            Transformation m4 = new Transformation(DeepClone(m1.M), DeepClone(m1.Invm));
            m4.Invm[2, 2] += 1.0f;  // This modification should also break closeness.
            Assert.False(Transformation.AreClose(m1,m4), "m1 should not be close to m4.");
        }
        
        [Fact]
        public void TestMultiplication()
        {
            var m1 = new Transformation(
                new float[,]
                {
                    {1.0f, 2.0f, 3.0f, 4.0f},
                    {5.0f, 6.0f, 7.0f, 8.0f},
                    {9.0f, 9.0f, 8.0f, 7.0f},
                    {6.0f, 5.0f, 4.0f, 1.0f},
                },
                new float[,]
                {
                    {-3.75f, 2.75f, -1f, 0f},
                    {4.375f, -3.875f, 2.0f, -0.5f},
                    {0.5f, 0.5f, -1.0f, 1.0f},
                    {-1.375f, 0.875f, 0.0f, -0.5f},
                }
            );
            Assert.True(m1.IsConsistent());

            var m2 = new Transformation(
                new float[,]
                {
                    {3.0f, 5.0f, 2.0f, 4.0f},
                    {4.0f, 1.0f, 0.0f, 5.0f},
                    {6.0f, 3.0f, 2.0f, 0.0f},
                    {1.0f, 4.0f, 2.0f, 1.0f},
                },
                new float[,]
                {
                    {0.4f, -0.2f, 0.2f, -0.6f},
                    {2.9f, -1.7f, 0.2f, -3.1f},
                    {-5.55f, 3.15f, -0.4f, 6.45f},
                    {-0.9f, 0.7f, -0.2f, 1.1f},
                }
            );
            Assert.True(m2.IsConsistent());

            var expected = new Transformation(
                new float[,]
                {
                    {33.0f, 32.0f, 16.0f, 18.0f},
                    {89.0f, 84.0f, 40.0f, 58.0f},
                    {118.0f, 106.0f, 48.0f, 88.0f},
                    {63.0f, 51.0f, 22.0f, 50.0f},
                },
                new float[,]
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
                new float[,]
                {
                    {1f, 2f, 3f, 4f},
                    {5f, 6f, 7f, 8f},
                    {9f, 9f, 8f, 7f},
                    {0f, 0f, 0f, 1f}
                },
                new float[,]
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
            // Crea una trasformazione m1 con matrice M e InvM
            Transformation m1 = new Transformation(
                new float[4, 4] {
                    { 1.0f, 2.0f, 3.0f, 4.0f },
                    { 5.0f, 6.0f, 7.0f, 8.0f },
                    { 9.0f, 9.0f, 8.0f, 7.0f },
                    { 6.0f, 5.0f, 4.0f, 1.0f }
                },
                new float[4, 4] {
                    { -3.75f, 2.75f, -1.0f, 0.0f },
                    { 4.375f, -3.875f, 2.0f, -0.5f },
                    { 0.5f, 0.5f, -1.0f, 1.0f },
                    { -1.375f, 0.875f, 0.0f, -0.5f }
                }
            );

            // Calcola l'inverso
            Transformation m2 = m1.Inverse();

            // Verifica che l'inverso sia consistente
            Assert.True(m2.IsConsistent(), "Inverso non è consistente.");

            // Calcola il prodotto tra m1 e il suo inverso
            Transformation prod = m1 * m2;

            // Verifica che il prodotto dia la matrice identità
            Assert.True(prod.IsConsistent(), "Prodotto non è consistente.");
            Assert.True(Transformation.AreClose(prod, new Transformation()), "Prodotto non è uguale alla matrice identità.");
        }


}