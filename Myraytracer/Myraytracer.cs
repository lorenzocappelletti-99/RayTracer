// See https://aka.ms/new-console-template for more information

using Trace;


Console.WriteLine("Hello, World!");

var img = new HdrImage(3, 2);
        
img.SetPixel(0, 0, new Color(1.0e1f, 2.0e1f, 3.0e1f));
img.SetPixel(1, 0, new Color(4.0e1f, 5.0e1f, 6.0e1f));
img.SetPixel(2, 0, new Color(7.0e1f, 8.0e1f, 9.0e1f));
img.SetPixel(0, 1, new Color(1.0e2f, 2.0e2f, 3.0e2f));
img.SetPixel(1, 1, new Color(4.0e2f, 5.0e2f, 6.0e2f));
img.SetPixel(2, 1, new Color(7.0e2f, 8.0e2f, 9.0e2f));


var filePath = Path.Combine(Directory.GetCurrentDirectory(), "output.pfm");

using FileStream fileStream = File.Create(filePath);
img.WritePfm(fileStream, false);