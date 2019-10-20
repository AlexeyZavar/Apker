@echo off
aapt dump badging %1 | findstr -i "package: name=''" | StringConcater.exe