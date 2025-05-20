# Raytracer

Raytracer 0.2.0 is a program that allows you to generate simple images and convert PFM-RGB image files to JPG or PNG format.

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

For different options and help with the demo command, run:

```dotnet run demo help```

## Converting PFM to JPG

To convert a PFM image to JPG, use the following command:

```dotnet run -- <inputfile>.pfm <a-factor> <gamma-factor> <outputfile>.jpg```

Example

## Animation

You can also create a simple animation by running the following script:

```./make_animation.sh```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Maintainers

- [Pietro Puppi](https://github.com/sedna42)
- [Lorenzo Cappelletti](https://github.com/lorenzocappelletti-99)

## License

Licensed under the EUPL-1.2.
