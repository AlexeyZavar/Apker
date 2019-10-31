// Logger.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Apker.Config;

#endregion

namespace Apker
{
  public static class Logger
  {
    public delegate void LoggerHandler(string message);

    private static StreamWriter _sw;
    private static event LoggerHandler _log;

    public static event LoggerHandler LogHandlers
    {
      add
      {
        _log += value;
        Debug.WriteLine( $"Log handler {value.Method.Name} added" );
      }
      remove
      {
        _log -= value;
        Debug.WriteLine( $"Log handler {value.Method.Name} deleted" );
      }
    }

    public static void Log(string message)
    {
      _log?.Invoke( message );
    }

    public static void LogInConsole(string message)
    {
      if ( !message.Contains( "[c:" ) )
      {
        Console.WriteLine( message );
        return;
      }

      if ( !ColorMode )
      {
        Console.WriteLine( RemoveColorCodes( message ) );
        return;
      }

      /*
       * Ehh...
       *
       *
       * Coloring usage:
       * "[c:01]Hi, blue! [c:0c]Hi, red!"
       *
       * Supported all colors from Windows Console
       *
       */
      var pairs = new Dictionary<int, KeyValuePair<string, ConsoleColor>>();

      // Colors parser
      var parsed = message.Split( "[c:" );

      for ( var i = 0; i < parsed.Length; i++ )
      {
        var s = parsed[i];
        if ( string.IsNullOrEmpty( s ) ) continue;
        if ( s == "\n" )
        {
          Console.WriteLine();
          continue;
        }

        // don't kill me please :d
        switch ( s[1] )
        {
          case '0':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "00]", "" ), ConsoleColor.Black ) );
            break;
          case '1':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "01]", "" ), ConsoleColor.DarkBlue ) );
            break;
          case '2':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "02]", "" ), ConsoleColor.DarkGreen ) );
            break;
          case '3':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "03]", "" ), ConsoleColor.DarkCyan ) );
            break;
          case '4':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "04]", "" ), ConsoleColor.DarkRed ) );
            break;
          case '5':
            pairs.Add(
              i, new KeyValuePair<string, ConsoleColor>( s.Replace( "05]", "" ), ConsoleColor.DarkMagenta ) );
            break;
          case '6':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "06]", "" ), ConsoleColor.DarkYellow ) );
            break;
          case '7':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "07]", "" ), ConsoleColor.DarkGray ) );
            break;
          case '8':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "08]", "" ), ConsoleColor.Gray ) );
            break;
          case '9':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "09]", "" ), ConsoleColor.Blue ) );
            break;
          case 'a':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "0a]", "" ), ConsoleColor.Green ) );
            break;
          case 'b':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "0b]", "" ), ConsoleColor.Cyan ) );
            break;
          case 'c':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "0c]", "" ), ConsoleColor.Red ) );
            break;
          case 'd':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "0d]", "" ), ConsoleColor.Magenta ) );
            break;
          case 'e':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "0e]", "" ), ConsoleColor.Yellow ) );
            break;
          case 'f':
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s.Replace( "0f]", "" ), ConsoleColor.White ) );
            break;
          default:
            pairs.Add( i, new KeyValuePair<string, ConsoleColor>( s, ConsoleColor.Gray ) );
            break;
        }
      }

      foreach ( var (color, msg) in from keyItem in pairs.Keys
                                    let color = pairs[keyItem].Value
                                    let msg = pairs[keyItem].Key
                                    select (color, msg) )
      {
        Console.ForegroundColor = color;
        Console.Write( msg );
      }

      Console.WriteLine();
      Console.ResetColor();
    }

    public static void LogInFile(string message)
    {
      if ( _sw == null )
        _sw = new StreamWriter( LogPath, true, Encoding.Default );
      _sw?.WriteLine( RemoveColorCodes( message ) );
      _sw.Flush();
    }

    private static string RemoveColorCodes(string message)
    {
      var pattern = @"(\[c:)[0-9][0-9a-f]\]";
      var clean = Regex.Replace( message, pattern, "" );
      return clean;
    }
  }
}
