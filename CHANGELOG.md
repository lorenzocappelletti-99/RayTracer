# [unreleased]
- Added cone parsing (see `example.txt`).
- Added command-Line Option for PCG initial state and PCG sequence identifier.
- Add a few images (see `output` directory for results and see `input` directory for scene definition examples).

# Version 1.1.0

- Added Render Timer: A clock now tracks and displays the time taken by the core rendering process ( the `FireAllRays` function). [#pull_18](https://github.com/lorenzocappelletti-99/RayTracer/pull/18)
- Added Progress Bar: A real-time progress indicator provides visual feedback during rendering (increments percent at each rendered row of pixel). 
- Added command-Line Option for Luminosity Factor: You can now specify a custom tone mapping scale factor to enhance the luminosity of your images via the CLI (e.g.,` --factor 4`, default at 0.6).

# Version 1.0.0

-   The program can produce images using a path tracer that solves the rendering equation. [#code](https://github.com/lorenzocappelletti-99/RayTracer/blob/463710b53b0293a4c25608f583a08616282b0710/Trace/Renderer.cs#L99)

-   Introduced support for scene description via external text files (e.g., `example.txt`). [#pull_15](https://github.com/lorenzocappelletti-99/RayTracer/pull/15)
-   Implemented Constructive Solid Geometry (CSG) operations for advanced shape composition. [#pull_7](https://github.com/lorenzocappelletti-99/RayTracer/pull/7)  [#code](https://github.com/lorenzocappelletti-99/RayTracer/blob/8768616a491c358f634b72223ea151d62ab7c9a5/Trace/Shapes.cs#L1150)
-   Improved renderer interface with support for:
    - Path tracing with Russian Roulette and variable max depth. 
    - Point light rendering. [#pull_14](https://github.com/lorenzocappelletti-99/RayTracer/pull/14) [#code](https://github.com/lorenzocappelletti-99/RayTracer/blob/463710b53b0293a4c25608f583a08616282b0710/Trace/Renderer.cs#L148)
    - Adjustable resolution and sampling parameters.
    - Optional antialiasing toggle. [#pull_13](https://github.com/lorenzocappelletti-99/RayTracer/pull/13)
-   CLI expanded with user-friendly options for resolution, renderer type, output file, and rendering configuration.
-   Included an example scene file and rendering instructions.

# Version 0.3.0

- Third release of the code.
- The program is able to produce images with a PathTracer (On/Off Renderer). [#pull_11](https://github.com/lorenzocappelletti-99/RayTracer/pull/11) [#code](https://github.com/lorenzocappelletti-99/RayTracer/blob/463710b53b0293a4c25608f583a08616282b0710/Trace/Renderer.cs#L33)
- Scenes can only be made by actually learning a bit of C# and modifying command.cs .
- A user-friendly method to compose scenes will be released shortly.

# Version 0.2.0

- Second release of the code. Now it actually generates PFM files.
- Functionalities of 0.1.0 are available. 

# Version 0.1.0

-   First release of the code. [#pull_6](https://github.com/lorenzocappelletti-99/RayTracer/pull/6)

# 2025-04-30

-   Fix an issue with the vertical order of the images. [#pull_3](https://github.com/lorenzocappelletti-99/RayTracer/issues/3)
