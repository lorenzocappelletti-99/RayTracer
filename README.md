# Raytracer

<<<<<<< Updated upstream
Raytracer 0.2.0 is a program that allows you to generate simple images and convert PFM-RGB image files to JPG or PNG format.
=======
Raytracer 1.0.0 is a program that allows you to generate simple images and convert PFM-RGB image files to JPG or PNG format.

---
>>>>>>> Stashed changes

## Requirements

- [.NET](https://dotnet.microsoft.com/en-us/) (Version 9.0)
- Git (if cloning the repository)

---

## Installation

```bash
git clone [https://github.com/lorenzocappelletti-99/RayTracer](https://github.com/lorenzocappelletti-99/RayTracer)
````

If Git is not available, download the zipped directory directly from the repository page.

The project is compiled using .NET. If .NET is not available, download the precompiled executable from the repository.

---

## Build Instructions

If you want to build the project from source, navigate to the project directory and run:

<<<<<<< Updated upstream
```dotnet build```

=======
```bash
dotnet build
````
>>>>>>> Stashed changes
## Usage

### Render Command

Run the renderer using the following command:

```dotnet run render [sceneFile] [options]```

### Options

| Option                    | Description                                  | Example                 | Default    |
| ------------------------- | -------------------------------------------- | ----------------------- | ---------- |
| `-w`, `--width`           | Image width (in pixels)                      | `--width 800`           | 1366       |
| `-h`, `--height`          | Image height (in pixels)                     | `--height 600`          | 768        |
| `-o`, `--output`          | Output filename (PNG or JPG)                 | `--output demo.png`     | demo.png   |
| `-A`, `--AntiAliasing`    | Enable/disable antialiasing (`true`/`false`) | `--AntiAliasing true`   | false      |
| `-R`, `--NumOfRays`       | Rays per pixel                               | `--NumOfRays 10`        | 3          |
| `-x`, `--RussianRoulette` | Russian roulette path limit                  | `--RussianRoulette 5`   | 1          |
| `-D`, `--MaxDepth`        | Maximum depth for ray reflection             | `--MaxDepth 4`          | 2          |
| `-r`, `--renderer`        | Rendering mode (`PathTracer`, `PointLight`)  | `--renderer PathTracer` | PathTracer |

#### Example Output

![demo](https://github.com/lorenzocappelletti-99/RayTracer/blob/master/Myraytracer/output/trying.png)  
*The image above was rendered using -A true, -R 10, `example.txt`.*

####  Scene Definition

Scenes are defined using plain text files (e.g., `.txt`) placed in the `input/` directory. These files describe the objects, materials, lighting, and camera parameters using a custom syntax.

The file `example.txt` included in this release demonstrates the full range of currently supported scene features and syntax.


### Demo Command

To generate a simple image, use the following command:

<<<<<<< Updated upstream
```dotnet run demo```

For different options and help with the demo command, run:
=======
```bash
dotnet run demo
````

List of all functionalities:

| Option | Description | Example | Default |
| :---------- | :---------------------------------------- | :---------------------- | :--------- |
| `-c`, `--camera` | **Projection type** (`Perspective`/`Orthogonal`) | `--camera Perspective` | Perspective |
| `-a`, `--angle` | **Rotation angle Z** (degrees) | `--angle 45` | 0 |
| `-w`, `--width` | **Image width** | `--width 1920` | 1366 |
| `-h`, `--height` | **Image height** | `--height 1080` | 768 |
| `-o`, `--output` | **Output file name** (LDR) | `--output image.png` | demo.png |
| `-A`, `--AntiAliasing` | **Anti-aliasing enabled** | `--AntiAliasing true` | false |
| `-R`, `--RaysPerPixel` | **Rays per pixel** | `--RaysPerPixel 5` | 1 |
| `-r`, `--Renderer` | **Renderer type** | `--Renderer PointLight` | PathTracer |
>>>>>>> Stashed changes

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
