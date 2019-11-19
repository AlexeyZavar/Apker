// Config.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static Apker.Logger;

#endregion

namespace Apker
{
  [Serializable]
  public class Config
  {
    private static Config _instance;
    public string LogPath { get; private set; }
    public bool ColorMode { get; private set; }
    public string WorkingDir { get; private set; }
    public bool MultithreadDownload { get; private set; }

    public static Config GetInstance()
    {
      return _instance ??= new Config();
    }

    private static void SetInstance(Config cfg)
    {
      _instance = cfg;
    }

    public static void LoadDefaults()
    {
      var ins = GetInstance();

      ins.LogPath = "log.txt";
      ins.ColorMode = true;
      ins.WorkingDir = "data/";
      ins.MultithreadDownload = false;
    }

    public static void UpgradeConfig()
    {
      var ins = GetInstance();

      ins.LogPath ??= "log.txt";
      ins.WorkingDir ??= "data/";
    }

    private static void SaveToFile()
    {
      var formatter = new BinaryFormatter();
      using var fs = new FileStream( "config.dat", FileMode.OpenOrCreate );
      formatter.Serialize( fs, GetInstance() );
    }

    public static void LoadFromFile()
    {
      var formatter = new BinaryFormatter();
      using var fs = new FileStream( "config.dat", FileMode.OpenOrCreate );
      var cfg = (Config) formatter.Deserialize( fs );
      SetInstance( cfg );
    }

    public void Menu()
    {
      again:
      Console.Clear();
      Log( "[c:03]Current settings[c:08]:" );
      Log( $"1. [c:0d]Path to log[c:08]: [c:09]{LogPath}" );
      Log( $"2. [c:0d]Color mode[c:08]: [c:09]{ColorMode}" );
      Log( $"3. [c:0d]Path to working dir[c:08]: [c:09]{WorkingDir}" );
      Log( $"4. [c:0d]Multi-thread downloader[c:08]: [c:09]{MultithreadDownload}" );
      Log( "\nr. [c:0a]Save & return to main menu" );
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
          var colorMode = Utils.Chooser( "Color mode (y/n): " );
          switch ( colorMode )
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
              break;
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

        case '4':
          var downloaderMode = Utils.Chooser( "Multi-thread mode (y/n): " );
          switch ( downloaderMode )
          {
            case 'y':
              MultithreadDownload = true;
              break;
            case 'n':
              MultithreadDownload = false;
              break;
            default:
              Log( "[c:0c]Invalid mode" );
              Utils.Wait();
              break;
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
