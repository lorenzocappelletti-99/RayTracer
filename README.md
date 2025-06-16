# Raytracer

Raytracer 0.3.0 is a program that allows you to generate simple images and convert PFM-RGB image files to JPG or PNG format.

## Requirements

  - [.NET](https://dotnet.microsoft.com/en-us/) (Version 9.0)
  - Git (if cloning the repository)    

## Installation
```git clone https://github.com/lorenzocappelletti-99/RayTracer```

If Git is not available, download the zipped directory directly from the repository page.

The project is compiled using .NET. If .NET is not available, download the precompiled executable from the repository.

## Build Instructions

If you want to build the project from source, navigate to the project directory and run:


```dotnet build```

## Usage
Generating a Simple Image

To generate a simple image, use the following command:


```dotnet run demo```

for a more complex usage, run
```dotnet run demo -c Perspective -A true```

List of all functionalities:
```dotnet run demo [options]```

```-c```|```--camera``` Options: Perspective, Orthogonal

```-a```|```--angle``` Options. angle degres

```-w```|```--width```

```-h```|```--height```

```-o```|```--output``` Eg: demo.png, demo.jpg

```-A```|```--AntiAliasing``` Options: true, false

```-r```|```--renderer``` Options: PathTracer, PointLight


## Converting PFM to JPG

To convert a PFM image to JPG, use the following command:

```dotnet run -- <inputfile>.pfm <a-factor> <gamma-factor> <outputfile>.jpg```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Maintainers

- [Pietro Puppi](https://github.com/sedna42)
- [Lorenzo Cappelletti](https://github.com/lorenzocappelletti-99)

## License

Licensed under the EUPL-1.2.
