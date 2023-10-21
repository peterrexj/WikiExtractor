#!/usr/bin/env bash
SLN_PATH="$APPCENTER_SOURCE_DIRECTORY/filename.sln"
UWP_PATH="$APPCENTER_SOURCE_DIRECTORY/path to UWP project file/UWP project filename.csproj"
dotnet sln $SLN_PATH remove $UWP_PATH