# HEAD

-   Fix an issue with the vertical order of the images [#3](https://github.com/lorenzocappelletti-99/RayTracer/issues/3)

# Version 0.1.0

-   First release of the code

# Version 0.2.0

- Second release of the code. Now it actually generates PFM files
- Functionalities of 0.1.0 are available

# Version 0.3.0

- Third release of the code.
- The program is able to produce images with a PathTracer or a PointLight algorithm 
- Scenes can only be made by actually learning a bit of C# and modifying command.cs .
- A user-friendly method to compose scenes will be released shortly.

# Version 1.0.0

-   Introduced support for scene description via external text files (e.g., `example.txt`)
-   Implemented Constructive Solid Geometry (CSG) operations for advanced shape composition
-   Improved renderer interface with support for:
    - Path tracing with Russian Roulette and variable max depth
    - Point light rendering
    - Adjustable resolution and sampling parameters
    - Optional antialiasing toggle
-   CLI expanded with user-friendly options for resolution, renderer type, output file, and rendering configuration
-   Included an example scene file and rendering instructions
