#/usr/bin/env bash

rm -rf ./out

dotnet publish OpenTabletDriver.Workarounds.csproj -c Release -f net6.0 -o ./out
rm ./out/*.deps.json
rm ./out/OpenTabletDriver.Plugin.dll
rm ./out/OpenTabletDriver.Plugin.pdb > /dev/null 2>&1

pushd ./out
zip -r ../TabletFixes.zip ./*
mv ../TabletFixes.zip ./TabletFixes.zip

sha256sum ./TabletFixes.zip > ./zip_hashes.sha256
popd