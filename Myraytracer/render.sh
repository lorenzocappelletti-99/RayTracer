#!/bin/bash

for angle in $(seq 0 1 359); do
    angleNNN=$(printf "%03d" $angle)  # For filename: 000, 001, ..., 359
    angleFloat=$(printf "%.1f" $angle)  # Force dot as decimal separator (e.g., 30.0)

    # Pass float as string with . (dot) decimal separator
    dotnet run demo PerspectiveProjection angleFloat $angle --output=img$angleNNN.png
done

ffmpeg -r 25 -f image2 -s 640x480 -i img%03d.png \
    -vcodec libx264 -pix_fmt yuv420p \
    spheres-perspective.mp4
