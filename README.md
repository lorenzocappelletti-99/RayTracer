# Raytracer

Raytracer 1.1.0 is a program that allows you to generate simple images and convert PFM-RGB image files to JPG or PNG format.

---

## Requirements

- [.NET](https://dotnet.microsoft.com/en-us/) (Version 9.0) (if you want to build the solution)
- Git (if cloning the repository)

---

## Installation

```bash
git clone [https://github.com/lorenzocappelletti-99/RayTracer](https://github.com/lorenzocappelletti-99/RayTracer)
````

If Git is not available, download the zipped directory directly from the repository page.

The project is compiled using .NET. If .NET is not available, download the precompiled executable from the repository and use the one compatible with your OS.

---

## Build Instructions

If you want to build the project from source, navigate to the project directory and run:

```bash
dotnet build Myraytracer/Myraytracer.csproj
````

If you want to build the entire solution run:
```bash
dotnet build Myraytracer.sln
````

If you do not want to build it in [release 1.1.0](https://github.com/lorenzocappelletti-99/RayTracer/releases/tag/v1.1.0) there is a zipped file with widows and linux compatible prebuilt executables.

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
| `-f`, `--factor`          | Tone mapping scale factor Default: "0.6"     | `--factor 0.18`         | 0.6        |
| `-s`, `--statePCG`        | PCG initial state ulong                      | `--statePCG 34`         | 42         |
| `-S`, `--sequencePCG`     | PCG sequence identifier ulong                | `--sequencePCG 23`      | 52         |

#### Example Output

<table>
  <tr>
    <td>
      <img src="https://github.com/lorenzocappelletti-99/RayTracer/blob/master/Myraytracer/output/trying.png" width="600"/><br/>
      <sub><b>Figure 1:</b> The image above was rendered using -A true, -R 10 </sub>
    </td>
    <td>
      <img src="https://github.com/lorenzocappelletti-99/RayTracer/blob/master/Myraytracer/output/PT-R6-D5-x2-Atrue_exemple.png" width="600"/><br/>
      <sub><b>Figure 2:</b> The image above was rendered using -A true, -R 6, -D 5, -x 2 scene description in `example.txt`. </sub>
    </td>
  </tr>
</table>

<table>
  <tr>
    <td>
      <img src="https://github.com/lorenzocappelletti-99/RayTracer/blob/master/Myraytracer/output/PT-R6-D3-x1.png" width="600"/><br/>
      <sub><b>Figure 3:</b> The image above was rendered using -A false, -R 6, -D 3, -x 1, scene description in cornell.txt </sub>
    </td>
    <td>
      <img src="https://github.com/lorenzocappelletti-99/RayTracer/blob/master/Myraytracer/output/PT-R6-D3-x2-Atrue.png" width="600"/><br/>
      <sub><b>Figure 4:</b> The image above was rendered using -A true, -R 6, -D 3, -x 2, scene description in cornell.txt </sub>
    </td>
  </tr>
</table>


####  Scene Definition

Scenes are defined using plain text files (e.g., `.txt`) placed in the `input/` directory. These files describe the objects, materials, lighting, and camera parameters using a custom syntax.

The file `example.txt` included in this release demonstrates the full range of currently supported scene features and syntax.


### Demo Command

To generate a simple image, use the following command:

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
| `-s`, `--statePCG`        | PCG initial state ulong                      | `--statePCG 34`         | 42         |
| `-S`, `--sequencePCG`     | PCG sequence identifier ulong                | `--sequencePCG 23`      | 52         |


## Converting PFM to JPG

To convert a PFM image to JPG, use the following command:

```dotnet run -- <inputfile>.pfm <a-factor> <gamma-factor> <outputfile>.jpg```

| Option | Description | Example | Default |
| :---------- | :---------------------------------------- | :---------------------- | :--------- |
| `-f`, `--factor` | **Tone mapping scale factor** | `--camera Perspective` | .6 |
| `-g`, `--gamma` | **Gamma correction value to screen nonlinearity** | `--gamma 2.2` | 1 |

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Maintainers

- [Pietro Puppi](https://github.com/sedna42)
- [Lorenzo Cappelletti](https://github.com/lorenzocappelletti-99)

## License

Licensed under the EUPL-1.2.
