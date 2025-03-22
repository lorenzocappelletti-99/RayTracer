// See https://aka.ms/new-console-template for more information

using Trace;


Console.WriteLine("Hello, World!");

var img = new HdrImage(3,10);

img.WritePfm(img);
