/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/


namespace Trace;


public struct Transformation(float[,]? m = null, float[,]? invm = null)
{
    private static readonly float[,] IdentityMatr4X4 = new float[, ]
    {
        { 1.0f, 0.0f, 0.0f, 0.0f },
        { 0.0f, 1.0f, 0.0f, 0.0f },
        { 0.0f, 0.0f, 1.0f, 0.0f },
        { 0.0f, 0.0f, 0.0f, 1.0f }
    };

    public float[,] M = m ?? IdentityMatr4X4;
    public float[,] Invm = invm ?? IdentityMatr4X4;

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

    Transformation _matr_prod(Transformation a, Transformation b)
    {
        var result = new Transformation();
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    if (result.M != null) result.M[i, j] += a.M[i, k] * b.M[k, j];
                }
            }
        }
        return result;
    }
    
}

