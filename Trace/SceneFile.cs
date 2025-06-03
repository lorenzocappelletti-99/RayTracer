using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

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
    public string FileName;
    public int LineNum;
    public int ColNum;

    public SourceLocation(string fileName, int lineNum, int colNum)
    {
        FileName = fileName;
        LineNum = lineNum;
        ColNum = colNum;
    }
    
    public SourceLocation() {}
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
    public string Symbol;
    
    public SymbolToken(SourceLocation location, string symbol)
    {
        Location = location;
        Symbol = symbol;
    }
}

/// <summary>
/// 
/// </summary>
public class GrammarError : Exception
{
    public SourceLocation Location;
    public string Message;

    public GrammarError(SourceLocation location, string message)
    {
        Location = location;
        Message = message;
    }
}

public class InputStream
{
    private const string Whitespace = " \t\n\r";
    private const string Symbols = "()<>[],*";
    
    public StreamReader Stream;
    public SourceLocation Location;

    public int Tabulations;
    public SourceLocation SavedLocation;
    public string Savedstring;
    
    public Token SavedToken;
    

    public InputStream(StreamReader stream, string fileName, Token token, int tabulations = 8)
    {
        Stream = stream;
        
        Location = new SourceLocation(fileName: fileName, lineNum: 1, colNum: 1);

        Savedstring = "";
        SavedLocation = Location;
        Tabulations = tabulations;
        SavedToken = token;
    }

    public void UpdatePos(string ch)
    {
        switch (ch)
        {
            case "":
                return;
            case "\n":
                Location.LineNum++;
                Location.ColNum=1;
                break;
            case "\t":
                Location.ColNum+=Tabulations;
                break;
            default:
                Location.ColNum ++;
                break;
        }
    }

    public string ReadChar()
    {
        string ch;
        
        if (Savedstring != "")
        {
            ch = Savedstring;
            Savedstring = "";
        }
        else
        {
            using var stream = Stream;
            ch = stream.Read().ToString();
        }
        
        SavedLocation = Location;
        UpdatePos(ch);
        
        return ch;
    }

    public void UnreadChar(string ch)
    {
        Assert.True(Savedstring == "");
        Savedstring = ch;
        Location = SavedLocation;
    }

    public void SkipWhitespacesAndComments()
    {
        var ch = ReadChar();
        while (Whitespace.Contains(ch) || ch == "#")
        {
            if (ch != "#") continue;
            while("\r\n".Contains(ch));
            ch = ReadChar();
            if (ch == "") return;
        }
        UnreadChar(ch);
    }

    public StringToken ParseStringToken(SourceLocation tokenLocation)
    {
        var token = "";
        while (true)
        {
            var ch = ReadChar();
            
            if (ch == "") break;
            // if(ch == "") 
            
            token += ch;
        }
        return new StringToken(tokenLocation, token);
    }

    public LiteralNumberToken ParseLiteralNumberToken(string firststring, SourceLocation tokenLocation)
    {
        var token = firststring;
        while (true)
        {
            var ch = ReadChar();
            if (!ch.All(char.IsDigit) || ch == "." || ch == "e" || ch == "E")
            {
                UnreadChar(ch);
                break;
            }
            token += ch;
        }
        try
        {
            var value = float.Parse(token); // or float.Parse(token) if you want single-precision
            return new LiteralNumberToken(tokenLocation, value);
        }
        catch(FormatException)
        {
            throw new GrammarError(tokenLocation, $"'{token}' is an invalid floating-point number");
        }
    }

    public Token ParseKeywordOrIdentifier(string firstChar, SourceLocation tokenLocation)
    {
        var token = firstChar;
        while (true)
        {
            var ch = ReadChar();
            if (ch.All(char.IsLetterOrDigit) || ch == "_")
            {
                UnreadChar(ch);
                break;
            }
            token += ch;
        }
        if (KeywordMap.Keywords.TryGetValue(token, out var keywordEnum))
        {
            return new KeywordToken(tokenLocation, keywordEnum);
        }
        return new IdentifierToken(tokenLocation, token);

    }

    public Token ReadToken()
    {
        SkipWhitespacesAndComments();
        var ch = ReadChar();
        if (ch == "")
        {
            return new StopToken(location: Location);
        }
        
        var tokenLocation = Location;

        if (Symbols.Contains(ch))
        {
            return new SymbolToken(tokenLocation, ch);
        }
        if(ch == "'" )
        {
           return ParseStringToken(tokenLocation); 
        }
    }
}
