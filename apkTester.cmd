@echo off

title Apk Tester by AlexeyZavar

mkdir TESTER > nul
set work_dir=%~dp0

for /d %%B in (%work_dir%\*) do (
	for %%C in ("%%B\*.apk") do (
		7z e %%C -o%work_dir%\TESTER AndroidManifest.xml -r -aoa > nul
	)
)


rd /s /q %work_dir%\TESTER > nul


pause