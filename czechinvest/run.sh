#!/bin/sh

if ! [ -x "$(command -v dotnet)" ]; then
  echo 'Dotnet is not installed. You need to run install_sdk.sh.' 
  exit 1.
fi


dotnet publish -o bin/app
dotnet bin/app/CzechInvest.dll
