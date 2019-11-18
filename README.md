# Apker
Apker - tool for people who loves to change firmware. Just flash module &amp; get all apps that you need!

Tested on **Windows 10 x64** and **Arch Linux**. **Mac OS** supported but not tested

***DotNet 3.0+ is required***

***If you want to multithreaded download on \*Unix - install python3 and 2 libraries: tqdm + multithread***

# Instructions
1. Create subdirectory in 'data': ex. "*Utilities*"
2. Drop apk files in it
3. Run **Apker.exe** or 'dotnet Apker.dll'
4. Chech apks (**1**)
5. Make installer & uninstaller (**2,3**)

If you don't need apps anymore - flash **Uninstaller.zip**

# Command line arguments
**"check"** - *checks apk files*

**"installer"** - *makes installer*

**"uninstaller"** - *makes uninstaller*

# Credits

[**Me**](https://github.com/AlexeyZavar)

[**SharpZipLib**](https://github.com/icsharpcode/SharpZipLib)

[**aapt**](https://developer.android.com/studio/command-line/aapt2)
