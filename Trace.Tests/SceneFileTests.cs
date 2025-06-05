/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

namespace Trace.Tests;

using Xunit.Abstractions;
using System.IO;
using System.Text;
using Xunit;

public class SceneFileTest(ITestOutputHelper testOutputHelper)
{

    public static void AssertIsKeyword(Token token, KeywordEnum expectedKeyword)
    {
        // Ensure that the token is exactly of type KeyWord
        Assert.IsType<KeywordToken>(token);

        // Ora possiamo fare il cast in sicurezza
        var keywordToken = (KeywordToken)token;

        // Verifichiamo che il valore dell’enum corrisponda a quello atteso
        Assert.Equal(expectedKeyword, keywordToken.Keyword);
    }

    public static void AssertIsIdentifier(Token token, string expectedIdentifier)
    {
        // Ensure that the token is exactly of type IdentifierToken
        Assert.IsType<IdentifierToken>(token);

        // Safe to cast now that we know the type is IdentifierToken
        var idToken = (IdentifierToken)token;

        // Compare the actual identifier text with the expected value
        Assert.Equal(expectedIdentifier, idToken.Identifier);
    }
    
    public static void AssertIsSymbol(Token token, string expectedSymbol)
    {
        // Ensure that the token is exactly of type SymbolToken
        Assert.IsType<SymbolToken>(token);

        // Safe to cast now that we know the type is SymbolToken
        var symToken = (SymbolToken)token;

        // The Symbol property is a char; convert it to string for comparison
        Assert.Equal(expectedSymbol, symToken.Symbol.ToString());
    }
    
    public static void AssertIsNumber(Token token, float expectedNumber)
    {
        // Ensure that the token is exactly of type LiteralNumberToken
        Assert.IsType<LiteralNumberToken>(token);

        // Safe to cast now that we know the type is LiteralNumberToken
        var numToken = (LiteralNumberToken)token;

        // Compare the numeric value with the expected value
        Assert.Equal(expectedNumber, numToken.Number);
    }
    
    public static void AssertIsString(Token token, string expectedString)
    {
        // Ensure that the token is exactly of type StringToken
        Assert.IsType<StringToken>(token);

        // Safe to cast now that we know the type is StringToken
        var strToken = (StringToken)token;

        // Compare the string content with the expected value
        Assert.Equal(expectedString, strToken.S);
    }

    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    [Fact]
    public void TestSceneFile()
    {
        var bytes = Encoding.UTF8.GetBytes("abc   \nd\nef");
        using var reader = new StreamReader(new MemoryStream(bytes));
        var stream = new InputStream(reader, fileName: "file");
        
        Assert.Equal(1, stream.Location.LineNum);
        Assert.Equal(1, stream.Location.ColNum);
        
        var ch = stream.ReadChar();
        Assert.Equal('a', ch);
        Assert.Equal(1, stream.Location.LineNum);
        Assert.Equal(2, stream.Location.ColNum);
        
        stream.UnreadChar('A');
        Assert.Equal(1, stream.Location.LineNum);
        Assert.Equal(1, stream.Location.ColNum);
        
        ch = stream.ReadChar();
        Assert.Equal('A', ch);
        Assert.Equal(1, stream.Location.LineNum);
        Assert.Equal(2, stream.Location.ColNum);
        
       // _testOutputHelper.WriteLine("1) so far so good!\n");
        
        
        ch = stream.ReadChar();
        Assert.Equal('b', ch);
        Assert.Equal(1, stream.Location.LineNum);
        Assert.Equal(3, stream.Location.ColNum);
        
       // _testOutputHelper.WriteLine("2) so far so good!\n");

        
        ch = stream.ReadChar();
        Assert.Equal('c', ch);
        Assert.Equal(1, stream.Location.LineNum);
        Assert.Equal(4, stream.Location.ColNum);
        
        //_testOutputHelper.WriteLine("3) so far so good!\n");
        
        stream.SkipWhitespacesAndComments();
        
        //_testOutputHelper.WriteLine("4) so far so good!\n");
        
        ch = stream.ReadChar();
        Assert.Equal('d', ch);
        Assert.Equal(2, stream.Location.LineNum);
        Assert.Equal(2, stream.Location.ColNum);
        
        ch = stream.ReadChar();
        Assert.Equal('\n', ch);
        Assert.Equal(3, stream.Location.LineNum);
        Assert.Equal(1, stream.Location.ColNum);
        
        ch = stream.ReadChar();
        Assert.Equal('e', ch);
        Assert.Equal(3, stream.Location.LineNum);
        Assert.Equal(2, stream.Location.ColNum);
        
        ch = stream.ReadChar();
        Assert.Equal('f', ch);
        Assert.Equal(3, stream.Location.LineNum);
        Assert.Equal(3, stream.Location.ColNum);
        
       // _testOutputHelper.WriteLine("4) so far so good!\n");
        
        ch = stream.ReadChar();
        Assert.Equal('\0', ch);
        
        //_testOutputHelper.WriteLine("5) so far so good!\n");
    }


    [Fact]
    public void TestReadToken()
    {
        var input = """
                    # This is a comment
                    # This is another comment
                    new material sky_material(
                        diffuse(image("my file.pfm")),
                        <5.0, 500.0, 300.0>
                    ) # Comment at the end of the line
                    """;
        
        var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var inputStream = new InputStream(reader, fileName: "file");
        
        AssertIsKeyword(inputStream.ReadToken(), KeywordEnum.New);
        AssertIsKeyword(inputStream.ReadToken(), KeywordEnum.Material);
        AssertIsIdentifier(inputStream.ReadToken(), "sky_material");
        AssertIsSymbol(inputStream.ReadToken(), "(");
        AssertIsKeyword(inputStream.ReadToken(), KeywordEnum.Diffuse);
        AssertIsSymbol(inputStream.ReadToken(), "(");
        AssertIsKeyword(inputStream.ReadToken(), KeywordEnum.Image);
        AssertIsSymbol(inputStream.ReadToken(), "(");
        AssertIsString(inputStream.ReadToken(), "my file.pfm");
        AssertIsSymbol(inputStream.ReadToken(), ")");
    }

    [Fact]
    public void TestParsePigment()
    {
        var input = """
                    material sky_material(
                        diffuse(uniform(<0, 0, 0>)),
                        checkered(<0.7, 0.5, 1>, <0,.2,.4>)
                    )
                    """;
        var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var inputStream = new InputStream(reader, fileName: "file");
        var scene = Scene.ParseScene(inputStream);

    }
    
    [Fact]
    public void TestParser()
    {
        var input = """
        float clock(150)
    
        material sky_material(
            diffuse(uniform(<0, 0, 0>)),
            checkered(<0.7, 0.5, 1>, <0,.2,.4>)
        )
    
        # Here is a comment
    
        material ground_material(
            diffuse(checkered(<0.3, 0.5, 0.1>,
                              <0.1, 0.2, 0.5>, 4)),
            uniform(<0, 0, 0>)
        )
    
        material sphere_material(
            specular(uniform(<0.5, 0.5, 0.5>)),
            uniform(<0, 0, 0>)
        )
    
        plane (sky_material, translation([0, 0, 100]) * rotation_y(clock))
        plane (ground_material, identity)
    
        sphere(sphere_material, translation([0, 0, 1]))
    
        camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)
        """;
        
        var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)));
        var inputStream = new InputStream(reader, fileName: "file");
        var scene = Scene.ParseScene(inputStream);

        // You would typically work with the 'scene' object here
     
        
        // Check that float variables are ok
        Assert.True(scene.FloatVariables is { Count: 1 });
        //Assert.Contains("clock", scene.FloatVariables.Keys);
        //Assert.True(Math.Abs(scene.FloatVariables["clock"] - 150.0) < 1e-5);
        
        // Check that materials are ok
        //Assert.True(scene.Materials.Count == 3);
        //Assert.Contains("sky_material", scene.Materials.Keys);
        //Assert.Contains("ground_material", scene.Materials.Keys);
        //Assert.Contains("sphere_material", scene.Materials.Keys);

        var sphereMaterial = scene.Materials["sphere_material"];
        var skyMaterial = scene.Materials["sky_material"];
        var groundMaterial = scene.Materials["ground_material"];
        
        //Assert.True(skyMaterial.Brdf is DiffusiveBrdf);
        //Assert.True(skyMaterial.Brdf.Pigment is UniformPigment);
        //if(skyMaterial.Brdf.Pigment is UniformPigment uniformPigment)
        //    Assert.True(uniformPigment.Color.IsClose(new Color(0.0f,0.0f,0.0f)));
    }
    /*
        assert isinstance(ground_material.brdf, DiffuseBRDF)
        assert isinstance(ground_material.brdf.pigment, CheckeredPigment)
        assert ground_material.brdf.pigment.color1.is_close(Color(0.3, 0.5, 0.1))
        assert ground_material.brdf.pigment.color2.is_close(Color(0.1, 0.2, 0.5))
        assert ground_material.brdf.pigment.num_of_steps == 4

        assert isinstance(sphere_material.brdf, SpecularBRDF)
        assert isinstance(sphere_material.brdf.pigment, UniformPigment)
        assert sphere_material.brdf.pigment.color.is_close(Color(0.5, 0.5, 0.5))

        assert isinstance(sky_material.emitted_radiance, UniformPigment)
        assert sky_material.emitted_radiance.color.is_close(Color(0.7, 0.5, 1.0))
        assert isinstance(ground_material.emitted_radiance, UniformPigment)
        assert ground_material.emitted_radiance.color.is_close(Color(0, 0, 0))
        assert isinstance(sphere_material.emitted_radiance, UniformPigment)
        assert sphere_material.emitted_radiance.color.is_close(Color(0, 0, 0))

        # Check that the shapes are ok

        assert len(scene.world.shapes) == 3
        assert isinstance(scene.world.shapes[0], Plane)
        assert scene.world.shapes[0].transformation.is_close(translation(Vec(0, 0, 100)) * rotation_y(150.0))
        assert isinstance(scene.world.shapes[1], Plane)
        assert scene.world.shapes[1].transformation.is_close(Transformation())
        assert isinstance(scene.world.shapes[2], Sphere)
        assert scene.world.shapes[2].transformation.is_close(translation(Vec(0, 0, 1)))

        # Check that the camera is ok

        assert isinstance(scene.camera, PerspectiveCamera)
        assert scene.camera.transformation.is_close(rotation_z(30) * translation(Vec(-4, 0, 1)))
        assert pytest.approx(1.0) == scene.camera.aspect_ratio
        assert pytest.approx(2.0) == scene.camera.screen_distance*/
}

