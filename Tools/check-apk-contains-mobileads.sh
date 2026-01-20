#!/usr/bin/env bash
# Usage: ./tools/check-apk-contains-mobileads.sh path/to/your.apk
set -euo pipefail

APK_PATH="${1:-}"
if [ -z "$APK_PATH" ]; then
  echo "Usage: $0 path/to/app.apk"
  exit 2
fi

if ! command -v unzip >/dev/null 2>&1; then
  echo "`unzip` required. Install it and retry."
  exit 2
fi

TMPDIR="$(mktemp -d)"
cleanup() { rm -rf "$TMPDIR"; }
trap cleanup EXIT

unzip -q "$APK_PATH" -d "$TMPDIR"

# Search all .dex files for the provider classname
FOUND=0
for dex in "$TMPDIR"/*.dex; do
  if [ -f "$dex" ]; then
    if strings "$dex" | grep -q "com/google/android/gms/ads/MobileAdsInitProvider"; then
      echo "Found MobileAdsInitProvider reference inside $(basename "$dex")"
      FOUND=1
    fi
  fi
done

if [ $FOUND -eq 0 ]; then
  echo "MobileAdsInitProvider NOT found in any classes.dex"
  exit 1
fi

exit 0