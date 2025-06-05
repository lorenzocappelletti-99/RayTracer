using System.Globalization;
using Xunit;

namespace Trace;


/// <summary>
///  A specific position in a source file
///  This class has the following fields:
/// - FileName: the name of the file, or the empty str (e.g., because the source code was provided)
/// - LineNum: number of the line (starting from 1)
/// - ColNum: number of the column (starting from 1)
/// </summary>
public struct SourceLocation
{
    public string FileName = "";
    public int LineNum = 0;
    public int ColNum = 0;

    // Parameterless constructor: initializes LineNum and ColNum to 1 by default.
    public SourceLocation()
    {
        FileName = "";
        LineNum = 1;
        ColNum = 1;
    }

    // Full‐parameter constructor as before
    public SourceLocation(string fileName, int lineNum, int colNum)
    {
        FileName = fileName;
        LineNum = lineNum;
        ColNum = colNum;
    }
}


/// <summary>
/// A lexical token, used when parsing a scene file
/// </summary>
public abstract class Token(SourceLocation location)
{
    public SourceLocation Location = location;
}

/// <summary>
/// A token signalling the end of a file
/// </summary>
public class StopToken(SourceLocation location) : Token(location);


/// <summary>
/// List of keywords
/// </summary>
public enum KeywordEnum
{
    New = 1,
    Material = 2,
    Plane = 3,
    Sphere = 4,
    Diffuse = 5,
    Specular = 6,
    Uniform = 7,
    Checkered = 8,
    Image = 9,
    Identity = 10,
    Translation = 11,
    RotationX = 12,
    RotationY = 13,
    RotationZ = 14,
    Scaling = 15,
    Camera = 16,
    Orthogonal = 17,
    Perspective = 18,
    Float = 19,
    PointLight = 20
}

/// <summary>
/// 
/// </summary>
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

public class KeywordToken(SourceLocation location, KeywordEnum keyword) : Token(location)
{
    public KeywordEnum Keyword = keyword;

    public override string ToString() => Keyword.ToString();
    
}

public class IdentifierToken(SourceLocation location, string identifier) : Token(location)
{
    public string Identifier = identifier;

    public override string ToString() => Identifier;
}

public class StringToken(SourceLocation location, string s) : Token(location)
{
    public readonly string S = s;

    public override string ToString() => S;
}

public class LiteralNumberToken(SourceLocation location, float value) : Token(location)
{
    public float Number = value;
    
    public override string ToString() => Number.ToString();
}

/// <summary>
/// Token containing a symbol. Equivalent status of a keyword token.
/// Separated for clarity’s sake.
/// </summary>
public class SymbolToken(SourceLocation location, char symbol) : Token(location)
{
    public char Symbol = symbol;
    
    public override string ToString() => Symbol.ToString();
}


/// <summary>
/// An error found by the lexer/parser while reading a scene file
/// The fields of this type are the following:
///
/// `file_name`: the name of the file, or the empty string if there is no real file
/// `line_num`: the line number where the error was discovered (starting from 1)
/// `col_num`: the column number where the error was discovered (starting from 1)
/// `message`: a user-frendly error message
/// 
/// </summary>
public class GrammarError(SourceLocation location, string message) : Exception
{
    public SourceLocation Location = location;
    public string Message = message;
}


/// <summary>
/// A high-level wrapper around a stream, used to parse scene files
/// 
/// This class implements a wrapper around a stream, with the following additional capabilities:
/// - It tracks the line number and column number;
/// - It permits to "un-read" characters and tokens.
/// </summary>
public class InputStream
{
    private const string Whitespace = " \t\n\r";
    private const string Symbols = "()<>[],*,=";
    
    public StreamReader Stream;
    public SourceLocation Location;

    public int Tabulations;
    public SourceLocation SavedLocation;
    public Token? SavedToken;
    public char SavedChar;
    

    public InputStream(StreamReader stream, string fileName, int tabulations = 8)
    {
        Stream = stream;
        
        Location = new SourceLocation(fileName: fileName, lineNum: 1, colNum: 1);

        SavedChar = '\0';
        SavedLocation = Location;
        Tabulations = tabulations;
        
        SavedToken = null;
    }
    
    /// <summary>
    /// Update `location` after having read `ch` from the stream
    /// </summary>
    /// <param name="ch"></param>
    public void UpdatePos(char ch)
    {
        switch (ch)
        {
            case '\0':
                return;
            case '\n':
                Location.LineNum++;
                Location.ColNum = 1;
                break;
            case '\t':
                Location.ColNum += Tabulations;
                break;
            default:
                Location.ColNum++;
                break;
        }
    }


    public char ReadChar()
    {
        char ch;
        
        if (SavedChar != '\0')
        {
            ch = SavedChar;
            SavedChar = '\0';
        }
        else
        {
            var read = Stream.Read();
            ch = (char)read;
        }
        
        SavedLocation = Location;
        UpdatePos(ch);
        
        return ch;
    }

    public void UnreadChar(char ch)
    {
        Assert.True(SavedChar == '\0');
        SavedChar = ch;
        Location = SavedLocation;
    }


    public void SkipWhitespacesAndComments()
    {
        var ch = ReadChar();
        
        while (Whitespace.Contains(ch) || ch == '#')
        {
            if (ch == '#')
            {
                char next;
                while ((next = ReadChar()) != '\0' && next != '\n' && next != '\r') { }
            }

            ch = ReadChar();
            
            if (ch == '\0')
                return;
        }
        
        UnreadChar(ch);
    }


    public StringToken ParseStringToken(SourceLocation tokenLocation)
    {
        var token = "";
        while (true)
        {
            var ch = ReadChar();
            
            if (ch == '"') break;
            if (ch == '\0')
                throw new GrammarError(tokenLocation, "unterminated string");
            
            token += ch;
        }
        return new StringToken(tokenLocation, token);
    }

    public LiteralNumberToken ParseFloatToken(char firstChar, SourceLocation tokenLocation)
    {
        // Inizializziamo la stringa del token col primo carattere già letto
        var token = firstChar.ToString();

        while (true)
        {
            char ch = ReadChar();

            // Se il carattere non è cifra, '.' o 'e'/'E', lo rimettiamo indietro e usciamo
            if (!(char.IsDigit(ch) || ch == '.' || ch == 'e' || ch == 'E'))
            {
                UnreadChar(ch);
                break;
            }

            token += ch;
        }

        float value;
        try
        {
            // Provo a convertire la stringa 'token' in float, usando la cultura Invariant
            value = float.Parse(token, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            // Se il parsing fallisce, rilancio l’errore di grammatica
            throw new GrammarError(tokenLocation, $"'{token}' is an invalid floating-point number");
        }

        return new LiteralNumberToken(tokenLocation, value);
    }
    
    public Token ParseKeywordOrIdentifierToken(char firstChar, SourceLocation tokenLocation)
    {
        // Accumuliamo il lexeme a partire dal primo carattere
        var token = firstChar.ToString();

        while (true)
        {
            var ch = ReadChar();

            // Se il carattere non è lettera, cifra o underscore, lo rimetto indietro e interrompo
            if (!(char.IsLetterOrDigit(ch) || ch == '_'))
            {
                UnreadChar(ch);
                break;
            }

            token += ch;
        }

        // Proviamo a vedere se 'token' è una keyword
        if (KeywordMap.Keywords.TryGetValue(token, out KeywordEnum kw))
        {
            return new KeywordToken(tokenLocation, kw);
        }
        else
        {
            return new IdentifierToken(tokenLocation, token);
        }
    }

    public Token ReadToken()
    {
        if (SavedToken != null)
        {
            var result = SavedToken;
            SavedToken = null;
            return result;
        }

        SkipWhitespacesAndComments();

        var ch = ReadChar();

        if (ch == '\0')
            return new StopToken(Location);
        
        var tokenLocation = Location;

        if (Symbols.Contains(ch))
            return new SymbolToken(tokenLocation, ch);

        if (ch == '"')
            return ParseStringToken(tokenLocation);

        if (char.IsDigit(ch) || ch == '+' || ch == '-' || ch == '.')
            return ParseFloatToken(ch, Location);

        if (char.IsLetter(ch) || ch == '_')
            return ParseKeywordOrIdentifierToken(ch, Location);

        // Carattere non riconosciuto
        throw new GrammarError(
            location: Location,
            $"Invalid character {ch}"
        );

    }
    public void UnreadToken(Token token)
    {
        Assert.True(SavedToken == null);
        SavedToken = token;
    }


}


public class Scene
{
    public World World = new();
    public Camera? Camera;
    public Dictionary<string, float>? FloatVariables;
    public HashSet<string> OverriddenVariables;
    public Dictionary<string, Material> Materials = new();
    
    private Scene(Dictionary<string, float>? floatVariables, HashSet<string> overriddenVariables)
    {
        FloatVariables = floatVariables;
        OverriddenVariables = overriddenVariables;
    }

    public static void ExpectSymbol(char symbol, InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if (token is not SymbolToken || token.GetType() != typeof(SymbolToken))
        {
            throw new GrammarError(token.Location, $"{nameof(ExpectSymbol)}: got '{token}' instead of '{symbol}'");
        }
    }

    public static KeywordEnum ExpectKeywords(List<KeywordEnum> keywords, InputStream inputFile)
    {
        var token = inputFile.ReadToken();

        if (token is not KeywordToken key)
            throw new GrammarError(token.Location, $"expected a keyword instead of '{token}'");
        if (!keywords.Contains(key.Keyword))
            throw new GrammarError(token.Location,
                $"expected one of the keywords {string.Join(",", Enum.GetNames(typeof(KeywordEnum)))} instead of '{token}'"
            );
        return key.Keyword;
    }
    
    public static float ExpectNumber(InputStream inputFile, Scene scene)
    {   
        var token = inputFile.ReadToken();
        switch (token)
        {
            case LiteralNumberToken number:
                return number.Number;
            case IdentifierToken identifier:
            {
                var variableName = identifier.Identifier;
                if(scene.FloatVariables.ContainsKey(variableName))
                    throw new GrammarError(token.Location, $"variable '{variableName}' is unknown");
                return scene.FloatVariables[variableName];
            }
            default:
                throw new GrammarError(token.Location, $"expected a number instead of '{token}'");
        }
    }

    public static string ExpectString(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if(token is not StringToken) 
            throw new GrammarError(token.Location, $"expected a string in {token}");
        return ((StringToken)token).S;
    }
    
    public static string ExpectIdentifier(InputStream inputFile)
    {
        var token = inputFile.ReadToken();
        if(token is not IdentifierToken)
            throw new GrammarError(token.Location, $"expected a identifier in {token}");
        return ((IdentifierToken)token).Identifier;
    }

    public static Vec ParseVector(InputStream inputFile, Scene scene)
    {
        ExpectSymbol('[', inputFile);
        var x = ExpectNumber(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var y = ExpectNumber(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var z = ExpectNumber(inputFile, scene);
        ExpectSymbol(']', inputFile);
        return new Vec(x, y, z);
    }

    public static Color ParseColor(InputStream inputFile, Scene scene)
    {
        ExpectSymbol('<', inputFile);
        var r = ExpectNumber(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var g = ExpectNumber(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var b = ExpectNumber(inputFile, scene);
        ExpectSymbol('>', inputFile);
        return new Color(r, g, b);
    }

    public static Pigment ParsePigment(InputStream inputFile, Scene scene)
    {
        var keyword = ExpectKeywords([KeywordEnum.Checkered, KeywordEnum.Uniform, KeywordEnum.Image], inputFile);
        
        ExpectSymbol('(', inputFile);
 
        switch (keyword)
        {
            case KeywordEnum.Uniform:
            {
                var color = ParseColor(inputFile, scene);
                return new UniformPigment(color);
            }
            case KeywordEnum.Checkered:
            {
                var color1 = ParseColor(inputFile, scene);
                ExpectSymbol(',', inputFile);
                var color2 = ParseColor(inputFile, scene);
                try
                {
                    ExpectSymbol(',', inputFile);
                }
                catch
                {
                    var numOfSteps = (int)ExpectNumber(inputFile, scene);
                    return new CheckeredPigment(color1, color2, numOfSteps);
                }
                return new CheckeredPigment(color1, color2);
            }
            case KeywordEnum.Image:
            {
                var fileName = ExpectString(inputFile);
                using var stream = new FileStream(fileName, FileMode.Open);
                var image = HdrImage.ReadPfm(stream);
                return new ImagePigment(image);
            }
        }
        ExpectSymbol(')', inputFile);
        Assert.True(false, "this line should be unreachable");
        return null;
    }
    
    public static Brdf ParseBrdf(InputStream inputFile, Scene scene)
    {
        var brdfKeyword = ExpectKeywords([KeywordEnum.Diffuse, KeywordEnum.Specular], inputFile);
        ExpectSymbol('(', inputFile);
        var pigment = ParsePigment(inputFile, scene);
        switch (brdfKeyword)
        {
            case KeywordEnum.Diffuse:
                return new DiffusiveBrdf(pigment);
            case KeywordEnum.Specular:
                return new SpecularBrdf(pigment);
            default:
                Assert.True(false, "this line should be unreachable");
                return null;
        }
    }

    public static Tuple<string, Material> ParseMaterial(InputStream inputFile, Scene scene)
    {
        var name = ExpectIdentifier(inputFile);
        ExpectSymbol('(', inputFile);
        var brdf = ParseBrdf(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var emittedRadiance = ParsePigment(inputFile, scene);
        ExpectSymbol(')', inputFile);
        
        return new Tuple<string, Material>(name, new Material{Brdf = brdf, EmittedRadiance = emittedRadiance});
    }

    public static Transformation ParseTransformation(InputStream inputFile, Scene scene)
    {
        var result = new Transformation();
        while (true)
        {
            var transformationKw = ExpectKeywords(
            [   KeywordEnum.Identity,
                KeywordEnum.Translation,
                KeywordEnum.RotationX,
                KeywordEnum.RotationY,
                KeywordEnum.RotationZ,
                KeywordEnum.Scaling], inputFile);

            switch (transformationKw)
            {
                case KeywordEnum.Identity:
                    break;
                case KeywordEnum.Translation:
                    ExpectSymbol('(', inputFile);
                    result *= Transformation.Translation(ParseVector(inputFile, scene));
                    ExpectSymbol(')', inputFile);
                    break;
                case KeywordEnum.RotationX:
                    ExpectSymbol('(', inputFile);
                    result *= Transformation.RotationX(ExpectNumber(inputFile, scene));
                    ExpectSymbol(')', inputFile);
                    break;
                case KeywordEnum.RotationY:
                    ExpectSymbol('(', inputFile);
                    result *= Transformation.RotationY(ExpectNumber(inputFile, scene));
                    ExpectSymbol(')', inputFile);
                    break;
                case KeywordEnum.RotationZ:
                    ExpectSymbol('(', inputFile);
                    result *= Transformation.RotationZ(ExpectNumber(inputFile, scene));
                    ExpectSymbol(')', inputFile);
                    break;
                case KeywordEnum.Scaling:
                    ExpectSymbol('(', inputFile);
                    result *= Transformation.Scaling(ParseVector(inputFile, scene));
                    ExpectSymbol(')', inputFile);
                    break;
            }

            var nextKw = inputFile.ReadToken();
            if (nextKw is not SymbolToken token)
            {
                inputFile.UnreadToken(nextKw);
                break;
            }
            if (token.Symbol == '*') continue;
            inputFile.UnreadToken(token);
            break;
        }
        return result;
    }

    public static Sphere ParseSphere(InputStream inputFile, Scene scene)
    {
        ExpectSymbol('(', inputFile);
        
        var materialName = ExpectIdentifier(inputFile);
        if(!scene.Materials.ContainsKey(materialName)) throw new GrammarError(inputFile.Location, $"unknown material {materialName}");
        
        ExpectSymbol(',', inputFile);
        var transformation = ParseTransformation(inputFile, scene);
        ExpectSymbol(')', inputFile);
        
        return new Sphere(transformation: transformation, material: scene.Materials[materialName]);
    }

    public static Plane ParsePlane(InputStream inputFile, Scene scene)
    {
        ExpectSymbol('(', inputFile);
        
        var materialName = ExpectIdentifier(inputFile);
        if(!scene.Materials.ContainsKey(materialName)) throw new GrammarError(inputFile.Location, $"unknown material {materialName}");

        ExpectSymbol(',', inputFile);
        var transformation = ParseTransformation(inputFile, scene);
        ExpectSymbol(')', inputFile);
        
        return new Plane(transformation: transformation, material: scene.Materials[materialName]);
    }

    public static Camera ParseCamera(InputStream inputFile, Scene scene)
    {
        ExpectSymbol('(', inputFile);
        var typeKw = ExpectKeywords([KeywordEnum.Perspective, KeywordEnum.Orthogonal], inputFile);
        ExpectSymbol(',', inputFile);
        var transformation = ParseTransformation(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var aspectRatio = ExpectNumber(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var distance = ExpectNumber(inputFile, scene);
        ExpectSymbol(')', inputFile);

        switch (typeKw)
        {
            case KeywordEnum.Perspective:
                return new PerspectiveProjection(aspectRatio, distance, transformation);
            case KeywordEnum.Orthogonal:
                return new OrthogonalProjection(aspectRatio, transformation);
            // Are we sure?? must type in distance even if Orthogonal doesn't use it??
            default:
                Assert.True(false, "this line should be unreachable");
                return null;
        }
    }

    public static PointLight ParsePointLight(InputStream inputFile, Scene scene)
    {
        ExpectSymbol('(', inputFile);
        var position = ParseVector(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var color = ParseColor(inputFile, scene);
        ExpectSymbol(',', inputFile);
        var radius = ExpectNumber(inputFile, scene);
        ExpectSymbol(')', inputFile);
            
        return new PointLight(new Point(position.X, position.Y, position.Z), color, radius);
            
    }

    public static Scene ParseScene(InputStream inputFile, Dictionary<string, float>? variables = null)
    {
        variables ??= new Dictionary<string, float>();
        var scene = new Scene(variables, [..variables.Keys]);
        
        while (true)
        {
            var what = inputFile.ReadToken();
            if(what is StopToken) break;
            if(what is not KeywordToken whatToken) throw new GrammarError(inputFile.Location, $"expected a keyword instead of {what}");
            if(whatToken.Keyword == KeywordEnum.Float)
            {
                var variableName = ExpectIdentifier(inputFile);
                var variableLoc = inputFile.Location;
            
                ExpectSymbol('(', inputFile);
                var variableValue = ExpectNumber(inputFile, scene);
                ExpectSymbol(')', inputFile);
                if(scene.FloatVariables != null &&
                   scene.FloatVariables.ContainsKey(variableName) && 
                   scene.OverriddenVariables.Contains(variableName)) 
                    throw new GrammarError(variableLoc, $"variable {variableName} cannot be redefined");
                if(!scene.OverriddenVariables.Contains(variableName))
                    if (scene.FloatVariables != null)
                        scene.FloatVariables[variableName] = variableValue;
            }
            else if(whatToken.Keyword == KeywordEnum.Sphere)
                scene.World.AddShape(ParseSphere(inputFile, scene));
            else if(whatToken.Keyword == KeywordEnum.Plane)
                scene.World.AddShape(ParsePlane(inputFile, scene));
            else if(whatToken.Keyword ==  KeywordEnum.Camera && scene.Camera != null)
                throw new GrammarError(inputFile.Location, $"camera has already been specified");
            else if(whatToken.Keyword ==  KeywordEnum.Camera)
                scene.Camera = ParseCamera(inputFile, scene);
            else if(whatToken.Keyword ==  KeywordEnum.Material)
            {
                var materialName = ParseMaterial(inputFile, scene);
                scene.Materials[materialName.Item1] = materialName.Item2;
            }
            
            else if(whatToken.Keyword ==  KeywordEnum.PointLight)
            {
                var pointLight = ParsePointLight(inputFile, scene);
                scene.World.AddLight(pointLight);
            }
            else
                throw new GrammarError(what.Location, $"unexpected token {what}");
        }
        return scene;
    }
}


