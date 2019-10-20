@echo off
SetLocal EnableExtensions EnableDelayedExpansion
title Uninstaller maker by AlexeyZavar

set work_dir=%~dp0

rd /s /q %work_dir%\Uninstaller > nul
del /s /f /q uninstaller.zip > nul
mkdir Uninstaller > nul

robocopy %work_dir%\UninstallerSrc %work_dir%\Uninstaller /s /e > nul

for /d %%B in (%work_dir%\*) do (
	for %%C in ("%%B\*.apk") do (
		For /F "Delims=" %%I In ('getPackageName.cmd %%C') Do Set package=%%~I
		echo Apk file: %%C [!package!] will be deleted after flashing module
		echo pm uninstall !package! >> %work_dir%\Uninstaller\install.sh.2
	)
)

echo. > %wokr_dir%Uninstaller\install.sh
copy /b %wokr_dir%Uninstaller\install.sh.1+%wokr_dir%Uninstaller\install.sh.2+%wokr_dir%Uninstaller\install.sh.3  %wokr_dir%Uninstaller\install.sh

rem del /s /f /q %wokr_dir%Uninstaller\install.sh.* deletes install.sh (???)
del /s /f /q %wokr_dir%Uninstaller\install.sh.1
del /s /f /q %wokr_dir%Uninstaller\install.sh.2
del /s /f /q %wokr_dir%Uninstaller\install.sh.3

7z a -tzip -ssw -mx5 -r0 %work_dir%\uninstaller.zip %work_dir%\Uninstaller\*

rd /s /q %work_dir%\Uninstaller > nul

echo Done!
pause