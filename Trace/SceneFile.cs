using System.Globalization;
using Xunit;

namespace Trace;

/*
WHITESPACE = " \t\n\r"
SYMBOLS = "()<>[],*,="
*/

/// <summary>
/// A specific position in a source file
/// This class has the following fields:
/// - FileName: the name of the file, or the empty str (e.g., because the source code was provided)
/// - LineNum: number of the line (starting from 1)
/// - ColNum: number of the column (starting from 1)
/// </summary>
public struct SourceLocation
{
    public string FileName;
    public int LineNum;
    public int ColNum;

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

public class StringToken : Token
{
    public readonly string Value;

    public StringToken(SourceLocation location, string value)
    {
        Location = location;
        Value = value;
    }

    public string ValueText()
    {
        return Value;
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
    public char Symbol;
    
    public SymbolToken(SourceLocation location, char symbol)
    {
        Location = location;
        Symbol = symbol;
    }
}


public class GrammarError : Exception
{
    public SourceLocation Location { get; }

    public GrammarError(SourceLocation location, string message)
        : base(message)
    {
        Location = location;
    }
}

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
        /////////////////////////////////////
    }

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
            int read = Stream.Read();
            ch = read == -1 ? '\0' : (char)read;
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
        char ch = ReadChar();
        
        while (Whitespace.Contains(ch) || ch == '#')
        {
            if (ch == '#')
            {
                do
                {
                    ch = ReadChar();
                }
                while (ch != '\0' && ch != '\n' && ch != '\r');
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
            // if(ch == '\0') 
            
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
            char ch = ReadChar();

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

        if (Symbols.Contains(ch))
            return new SymbolToken(Location, ch);

        else if (ch == '"')
            return ParseStringToken(Location);

        else if (char.IsDigit(ch) || ch == '+' || ch == '-' || ch == '.')
            return ParseFloatToken(ch, Location);
        
        else if (char.IsLetter(ch) || ch == '_')
            return ParseKeywordOrIdentifierToken(ch, Location);

        else
        {
            // Carattere non riconosciuto
            throw new GrammarError(
                location: Location,
                $"Invalid character {ch}"
            );
        }

    }
}
