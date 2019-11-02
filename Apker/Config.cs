// Config.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.IO;
using static Apker.Logger;

#endregion

namespace Apker
{
  [Serializable]
  public class Config
  {
    public static string LogPath { get; private set; }
    public static bool ColorMode { get; private set; }
    public static string WorkingDir { get; private set; }

    public static void LoadDefaults()
    {
      LogPath = "log.txt";
      ColorMode = true;
      WorkingDir = "data/";
    }

    public static void LoadFromFile()
    {
      SerializeStatic.Load( typeof(Config), "apker.config" );
    }

    private static void SaveToFile()
    {
      SerializeStatic.Save( typeof(Config), "apker.config" );
    }


    public static void Menu()
    {
      again:
      Console.Clear();
      Log( "Current settings:" );
      Log( $"1. Path to log: [c:09]{LogPath}" );
      Log( $"2. Color mode: [c:09]{ColorMode}" );
      Log( $"3. Path to working dir: [c:09]{WorkingDir}" );
      Log( "\nr. Save & return to main menu" );
      var choose = Utils.Chooser();
      switch ( choose )
      {
        case '1':
          var logPath = Utils.GetInput( "Path to log: " );
          if ( logPath == "" )
            goto again;
          var path = Path.GetDirectoryName( logPath );
          if ( path != "" && Directory.Exists( Path.GetFullPath( logPath ) ) )
          {
            LogPath = logPath;
          }
          else if ( path == "" )
          {
            LogPath = logPath;
          }
          else
          {
            Log( "[c:0c]Invalid path" );
            Utils.Wait();
          }

          break;
        case '2':
          Console.Write( "Color mode (y/n): " );
          var mode = Utils.Chooser();
          switch ( mode )
          {
            case 'y':
              ColorMode = true;
              break;
            case 'n':
              ColorMode = false;
              break;
            default:
              Log( "[c:0c]Invalid mode" );
              Utils.Wait();
              goto again;
          }

          break;
        case '3':
          var workPath = Utils.GetInput( "Path to working dir: " );
          if ( Directory.Exists( workPath ) )
          {
            WorkingDir = workPath;
          }
          else
          {
            Log( "[c:0c]Invalid path" );
            Utils.Wait();
          }

          break;
        case 'r':
          goto exit;
      }

      goto again;

      exit:
      SaveToFile();
    }
  }
}
