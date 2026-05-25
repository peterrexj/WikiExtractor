#!/bin/zsh

SIMULATOR_NAME="iPhone 16 Pro"
PROJECT="Maui.Saints.csproj"
APP_BUNDLE="bin/Debug/net9.0-ios18.0/iossimulator-arm64/Maui.Saints.app"
BUNDLE_ID="com.peterrexj.christiancatholicsaints"

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

echo "==> Building..."
dotnet build "$PROJECT" -f net9.0-ios18.0 -c Debug || exit 1

echo "==> Installing..."
xcrun simctl install "$SIMULATOR_ID" "$APP_BUNDLE" || exit 1

echo "==> Terminating existing instance..."
xcrun simctl terminate "$SIMULATOR_ID" "$BUNDLE_ID" 2>/dev/null || true

echo "==> Launching..."
xcrun simctl launch "$SIMULATOR_ID" "$BUNDLE_ID"

echo "==> Done"
