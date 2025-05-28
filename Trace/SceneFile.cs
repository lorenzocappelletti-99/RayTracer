namespace Trace;

/*
WHITESPACE = " \t\n\r"
SYMBOLS = "()<>[],*"
*/

/// <summary>
/// A specific position in a source file
/// This class has the following fields:
/// - FileName: the name of the file, or the empty str (e.g., because the source code was provided)
/// - LineNum: number of the line (starting from 1)
/// - ColNum: number of the column (starting from 1)
/// </summary>
public class SourceLocation
{
    public string FileName = "";
    public int LineNum = 0;
    public int ColNum = 0;
}

/// <summary>
/// A lexical token, used when parsing a scene file
/// </summary>
public abstract class Token
{
    public SourceLocation Location = new SourceLocation();
}

/// <summary>
/// A token signalling the end of a file
/// </summary>
public class StopToken : Token
{
    public StopToken(SourceLocation location)
    {
        Location = location;
    }
}

public enum KeywordEnum
{
    New = 1,
    Material,
    Plane,
    Sphere,
    Diffuse,
    Specular,
    Uniform,
    Checkered,
    Image,
    Identity,
    Translation,
    RotationX,
    RotationY,
    RotationZ,
    Scaling,
    Camera,
    Orthogonal,
    Perspective,
    Float,
    PointLight
}

public static class KeywordMap
{
    public static readonly Dictionary<string, KeywordEnum> Keywords = new Dictionary<string, KeywordEnum>
    {
        { "new", KeywordEnum.New },
        { "material", KeywordEnum.Material },
        { "plane", KeywordEnum.Plane },
        { "sphere", KeywordEnum.Sphere },
        { "diffuse", KeywordEnum.Diffuse },
        { "specular", KeywordEnum.Specular },
        { "uniform", KeywordEnum.Uniform },
        { "checkered", KeywordEnum.Checkered },
        { "image", KeywordEnum.Image },
        { "identity", KeywordEnum.Identity },
        { "translation", KeywordEnum.Translation },
        { "rotation_x", KeywordEnum.RotationX },
        { "rotation_y", KeywordEnum.RotationY },
        { "rotation_z", KeywordEnum.RotationZ },
        { "scaling", KeywordEnum.Scaling },
        { "camera", KeywordEnum.Camera },
        { "orthogonal", KeywordEnum.Orthogonal },
        { "perspective", KeywordEnum.Perspective },
        { "float", KeywordEnum.Float },
        { "point_light", KeywordEnum.PointLight }
    };
}

public class KeywordToken : Token
{
    public KeywordEnum Keyword;

    public KeywordToken(SourceLocation location, KeywordEnum keyword)
    {
        Location = location;
        Keyword = keyword;
    }
}

public class IdentifierToken : Token
{
    public string Identifier;

    public IdentifierToken(SourceLocation location, string identifier)
    {
        Location = location;
        Identifier = identifier;
    }

    public string IdentifierText()
    {
        return Identifier;
    }
}

public class LiteralNumberToken : Token
{
    public float Number;
    
    public LiteralNumberToken(SourceLocation location, float value)
    {
        Location = location;
        Number = value;
    }
}

public class SymbolToken : Token
{
    public string Symbol;
    
    public SymbolToken(SourceLocation location, string symbol)
    {
        Location = location;
        Symbol = symbol;
    }
}


public class GrammarError
{
    public SourceLocation Location;
    public string Message;
}

public class InputStream
{
    private int Tabulations = 8;
}
