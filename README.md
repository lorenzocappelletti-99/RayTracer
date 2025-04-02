# Raytracer

Raytracer 0.1.0 is a program that converts PFM - RGB image format files to JPG format. 

Ongoing developments are present in the form of a ```geometry``` library. It will be used to handle future raytracing features.

## Requirements
- [.NET](https://dotnet.microsoft.com/en-us/) (Version 9.0)
- `git` (if cloning the repository)

## Installation
Use the package [git](https://git-scm.com/docs) clone to download the repository.

```bash
git clone https://github.com/lorenzocappelletti-99/RayTracer
```
If git is not available, download the zipped directory directly from the repository page.

The project is compiled using [.NET](https://dotnet.microsoft.com/en-us/). If .NET is not available, download the precompiled executable from the repository.

## Build Instructions
If you want to build the project from source, navigate to the project directory and run:

```powershell
dotnet build
```

## Usage
To convert a PFM image to JPG, use the following command:

```powershell
dotnet run -- <inputfile>.pfm <a-factor> <gamma-factor> <outputfile>.jpg
```

### Example
```powershell
dotnet run -- example.pfm 2.2 1.0 output.jpg
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Maintainers
- [Pietro Puppi](https://github.com/sedna42)
- [Lorenzo Cappelletti](https://github.com/lorenzocappelletti-99)

## License

Licensed under the [EUPL-1.2](https://license.md/wp-content/uploads/2022/06/eupl-1.2.txt).

## Badges

![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)
![License](https://img.shields.io/badge/License-EUPL--1.2-green)

