// Utils.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;

#endregion

namespace Apker
{
  public static class Utils
  {
    private const string NamingPattern = "^[A-Za-z0-9.()]*.apk";

    public static bool CheckName(string name)
    {
      // To avoid problems i'm recommending to name apk files like "TELEGRAM.5.11.apk"
      return Regex.IsMatch( name, NamingPattern );
    }

    public static IEnumerable<string> FindFiles(string ext)
    {
      return (from d in Directory.GetDirectories( Config.GetInstance().WorkingDir )
              from f in Directory.GetFiles( d, $"*.{ext}" )
              select f).ToList();
    }

    public static void Wait()
    {
      Thread.Sleep( 500 );
    }

    public static void WaitForPress()
    {
      Console.ReadKey();
    }

    public static string RemoveNamingErrors(string str)
    {
      return Regex.Replace( str, "[@!#$%^&*\";:?]", "" );
    }

    public static char Chooser(string text = "\nEnter option: ")
    {
      Console.Write( text );
      var c = Console.ReadKey().KeyChar;
      Console.WriteLine();
      return c;
    }

    public static string GetInput(string text = "")
    {
      Console.Write( text );
      var t = Console.ReadLine();
      Console.WriteLine();
      return t;
    }

    public static void DuplicateDirectory(string source, string target)
    {
      var diSource = new DirectoryInfo( source );
      var diTarget = new DirectoryInfo( target );

      CopyAll( diSource, diTarget );
    }

    private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
      Directory.CreateDirectory( target.FullName );

      foreach ( var fi in source.GetFiles() ) fi.CopyTo( Path.Combine( target.FullName, fi.Name ), true );
      foreach ( var (diSourceSubDir, nextTargetSubDir) in from diSourceSubDir in source.GetDirectories()
                                                          let nextTargetSubDir =
                                                            target.CreateSubdirectory( diSourceSubDir.Name )
                                                          select (diSourceSubDir, nextTargetSubDir) )
        CopyAll( diSourceSubDir, nextTargetSubDir );
    }

    public static string GetPackageName(string apk)
    {
      var system = Environment.OSVersion.Platform;
      var proc = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          Arguments = $"dump badging \"{NormalizePath( apk )}\"",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          CreateNoWindow = true
        }
      };
      proc.StartInfo.FileName = system switch
      {
        PlatformID.Win32NT => "aapt/aapt.exe",
        PlatformID.Unix => "aapt/aapt.linux",
        PlatformID.MacOSX => "aapt/aapt.mac",
        _ => "aapt/aapt.exe",
      };
      proc.Start();
      Thread.Sleep( 400 );
      var dumped = proc.StandardOutput.ReadToEnd();
      proc.Dispose();
      var text = dumped.Replace( "package: name='", "" );
      var num = text.LastIndexOf( "' versionCode", StringComparison.Ordinal );
      return text.Substring( 0, num );
    }

    private static string NormalizePath(string path)
    {
      return path.Replace( "\\\\", "/" );
    }

    public static void CreateZip(string folder, string name)
    {
      var zip = new FastZip();
      if ( File.Exists( name ) )
        File.Delete( name );
      zip.CreateZip( name, folder, true, "" );
    }

    public static void ClearWorkspace()
    {
      var dirs = new List<string> { "Installer", "Uninstaller" };
      var files = new List<string> { "Installer.zip", "Uninstaller.zip" };
      foreach ( var dir in from dir in dirs
                           where Directory.Exists( dir )
                           select dir )
        Directory.Delete( Config.GetInstance().WorkingDir + dir, true );
      foreach ( var file in from file in files
                            where File.Exists( file )
                            select file )
        File.Delete( file );
    }

    public static void FixExecPermissions()
    {
      var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          RedirectStandardOutput = true,
          UseShellExecute = false,
          CreateNoWindow = true,
          WindowStyle = ProcessWindowStyle.Hidden,
          FileName = "/bin/bash",
          Arguments = "-c \"chmod 755 aapt/aapt.linux\""
        }
      };
      process.Start();
    }

    public static void Download(string url, string path)
    {
      if ( Config.GetInstance().MultithreadDownload )
        MultithreadDownload( url, path );
      else
        SingleDownload( url, path );
    }

    private static void MultithreadDownload(string url, string path)
    {
      var system = Environment.OSVersion.Platform;
      var proc = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          Arguments = $"{url} {path} 6",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          CreateNoWindow = true
        }
      };
      proc.StartInfo.FileName = system switch
      {
        PlatformID.Win32NT => $"{Config.GetInstance().WorkingDir}Downloader/downloader.exe",
        PlatformID.Unix => $"python3 {Config.GetInstance().WorkingDir}Downloader/source.py",
        PlatformID.MacOSX => $"python3 {Config.GetInstance().WorkingDir}Downloader/source.py",
        _ => $"{Config.GetInstance().WorkingDir}Downloader/downloader.exe",
      };
      proc.Start();
      Wait();
    }

    private static void SingleDownload(string url, string path)
    {
      using var client = new WebClient();
      client.DownloadFile( url, path );
    }
  }
}
