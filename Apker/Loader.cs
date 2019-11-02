// Loader.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.IO;
using System.Threading;
using static Apker.Logger;

#endregion

namespace Apker
{
  public static class Loader
  {
    private static void Logo()
    {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine(
        "   _         _             \r\n  /_\\  _ __ | | _____ _ __ \r\n //_\\\\| '_ \\| |/ / _ \\ '__|\r\n/  _  \\ |_) |   <  __/ |   \r\n\\_/ \\_/ .__/|_|\\_\\___|_|   \r\n      |_|                  " );
      Console.ResetColor();
    }

    public static void Load()
    {
      Logo();

      LogEvent += LogInConsole;
      LogEvent += LogInFile;

      if ( !File.Exists( "apker.config" ) )
        Config.LoadDefaults();
      else
        Config.LoadFromFile();

      Directory.CreateDirectory( Config.WorkingDir );

      Utils.ClearWorkspace();

      if ( Environment.OSVersion.Platform.Equals( PlatformID.Unix ) )
        UnixPermissions();

      Utils.Wait();
    }

    private static void UnixPermissions()
    {
      Log( "\n[c:0e]Looks like you're on *unix system. Make sure you're running Apker with sudo" );
      Thread.Sleep( 2000 );
      Utils.FixExecPermissions();
    }
  }
}
