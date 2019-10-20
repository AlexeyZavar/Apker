@echo off
title Installer maker by AlexeyZavar
set work_dir=%~dp0
rd /s /q %work_dir%\Installer > nul
del /s /f /q installer.zip > nul
mkdir Installer > nul
robocopy %work_dir%\InstallerSrc %work_dir%\Installer /s /e > nul

for /d %%B in (%work_dir%\*) do (
	for %%C in ("%%B\*.apk") do (
		echo Copying %%C
		xcopy %%C %work_dir%\Installer\apks\ > nul
	)
)

rd /s /q %work_dir%\Installer\Installer > nul
rd /s /q %work_dir%\Installer\InstallerSrc > nul
del /s /f /q  %work_dir%\Installer\apks\MAGISK*.apk

7z a -tzip -ssw -mx5 -r0 %work_dir%\installer.zip %work_dir%\Installer\*

rd /s /q %work_dir%\Installer > nul

echo Done!
pause