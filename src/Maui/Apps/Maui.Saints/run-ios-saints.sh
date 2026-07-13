#!/bin/zsh

# Usage:
#   ./run-ios-saints.sh                   # iPhone 16 Pro (default)
#   ./run-ios-saints.sh --ipad            # iPad Pro 13-inch (M4)
#   ./run-ios-saints.sh --device "iPad mini (A17 Pro)"
#   ./run-ios-saints.sh --clean                        # clean before build

PROJECT="Maui.Saints.csproj"
APP_BUNDLE="bin/Debug/net9.0-ios18.0/iossimulator-arm64/Maui.Saints.app"
BUNDLE_ID="com.peterrexj.christiancatholicsaints"

SIMULATOR_NAME="iPhone 16 Pro"
CLEAN=false

for arg in "$@"; do
  case "$arg" in
    --ipad)   SIMULATOR_NAME="iPad Pro 13-inch (M4)" ;;
    --clean)  CLEAN=true ;;
    --device) ;;
  esac
done

for i in $(seq 1 $#); do
  if [ "${@[$i]}" = "--device" ] && [ $((i+1)) -le $# ]; then
    SIMULATOR_NAME="${@[$((i+1))]}"
  fi
done

cd "$(dirname "$0")"

echo "==> Finding simulator: $SIMULATOR_NAME..."
SIMULATOR_ID=$(xcrun simctl list devices available | grep "$SIMULATOR_NAME" | grep -v "unavailable" | head -1 | sed 's/.*(\([A-F0-9-]*\)).*/\1/')

if [ -z "$SIMULATOR_ID" ]; then
  echo "ERROR: No simulator found matching '$SIMULATOR_NAME'"
  echo "Available simulators:"
  xcrun simctl list devices available | grep -v "^==" | grep -v "^--" | grep -v "^$" | grep "iPhone\|iPad"
  exit 1
fi

echo "    Found: $SIMULATOR_ID"

echo "==> Booting simulator..."
STATUS=$(xcrun simctl list devices | grep "$SIMULATOR_ID" | grep -o "Booted")
if [ "$STATUS" != "Booted" ]; then
  xcrun simctl boot "$SIMULATOR_ID"
  open -a Simulator
else
  echo "    Simulator already booted"
fi

if [ "$CLEAN" = "true" ]; then
  echo "==> Cleaning..."
  dotnet clean "$PROJECT" -f net9.0-ios18.0 -c Debug
fi

echo "==> Building..."
dotnet build "$PROJECT" -f net9.0-ios18.0 -c Debug || exit 1

echo "==> Installing..."
xcrun simctl install "$SIMULATOR_ID" "$APP_BUNDLE" || exit 1

echo "==> Terminating existing instance..."
xcrun simctl terminate "$SIMULATOR_ID" "$BUNDLE_ID" 2>/dev/null || true

echo "==> Launching..."
xcrun simctl launch "$SIMULATOR_ID" "$BUNDLE_ID"

echo "==> Done"
