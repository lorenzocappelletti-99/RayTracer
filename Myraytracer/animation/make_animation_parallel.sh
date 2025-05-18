#!/usr/bin/env bash
set -euo pipefail

# Configurazione
CAMERA="PerspectiveProjection"
WIDTH=1366
HEIGHT=768
OUT_DIR="frames"

mkdir -p "${OUT_DIR}"

generate_frame() {
  angle="$1"
  idx=$(printf "%03d" "$angle")
  outfile="${OUT_DIR}/image${idx}.jpg"

  # Salta se il file esiste già
  if [[ -f "$outfile" ]]; then
    echo " • angle ${angle}° → ${outfile} already exists, skipping"
    return
  fi

  echo " • angle ${angle}° → ${outfile}..."
  dotnet run demo "$CAMERA" "$angle" "$WIDTH" "$HEIGHT" "$outfile" > /dev/null && \
  echo "   ✔ done angle $angle"
}

export -f generate_frame
export CAMERA WIDTH HEIGHT OUT_DIR

echo "Generating frames 0°–359° in parallel..."

seq 0 359 | parallel -j "$(sysctl -n hw.ncpu)" generate_frame

echo "✅ All frames in ./${OUT_DIR}/"
