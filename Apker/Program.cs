﻿// Program.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using static Apker.Logger;

#endregion

namespace Apker
{
  internal static class Program
  {
    private static bool _exit;

    private static void Main(string[] args)
    {
      Loader.Load();

      if ( args.Length != 0 )
      {
        switch ( args[0] )
        {
          case "check":
            NameAndIntegrityCheck();
            break;
          case "installer":
            MakeInstaller();
            break;
          case "uninstaller":
            MakeUninstaller();
            break;
          default:
            goto menu;
        }

        Environment.Exit( 0 );
      }

      menu:
      while ( !_exit ) Menu();
    }

    private static void Menu()
    {
      Console.Clear();
      Log( "Main menu:" );
      Log( "1. [c:09]Check all apk files for naming & integrity" );
      Log( "2. [c:0b]Make [c:0a]installer" );
      Log( "3. [c:0b]Make [c:0c]uninstaller" );
      Log( "4. [c:0e]Settings" );
      Log( "e. [c:0c]Exit" );
      var choose = Utils.Chooser();
      Utils.ClearWorkspace();
      Console.Clear();
      switch ( choose )
      {
        case '1':
          NameAndIntegrityCheck();
          break;
        case '2':
          MakeInstaller();
          break;
        case '3':
          MakeUninstaller();
          break;
        case '4':
          Config.Menu();
          break;
        case 'e':
          _exit = true;
          break;
        default:
          return;
      }

      Utils.WaitForPress();
    }

    private static void MakeUninstaller()
    {
      var apkFiles = Utils.FindFiles( "apk" );
      var uninstallerDir = Config.WorkingDir + "Uninstaller";
      Utils.DuplicateDirectory( Config.WorkingDir + "UninstallerSrc", uninstallerDir );
      var source = File.ReadAllLines( uninstallerDir + "/install.sh" ).ToList();
      foreach ( var apk in apkFiles )
      {
        var name = Utils.GetPackageName( apk );
        Log( $"[c:0b]{name}[c:08] will be [c:0c]uninstalled" );
        source.Insert( source.IndexOf( "  # Uninstall" ) + 1, $"pm uninstall {name}" );
      }

      File.Delete( uninstallerDir + "/install.sh" );
      File.Create( uninstallerDir + "/install.sh" ).Close();
      File.WriteAllLines( uninstallerDir + "/install.sh", source.ToArray() );
      Log( "\n[c:0e]Creating archive, please wait..." );
      Utils.CreateZip( uninstallerDir, "Uninstaller.zip" );
      Log( "\n[c:0a]Done" );
    }

    private static void MakeInstaller()
    {
      var apkFiles = Utils.FindFiles( "apk" );
      var installerDir = Config.WorkingDir + "Installer";
      if ( Directory.Exists( installerDir ) )
        Directory.Delete( installerDir, true );
      Utils.DuplicateDirectory( Config.WorkingDir + "InstallerSrc", installerDir );
      foreach ( var apk in apkFiles )
      {
        var name = Path.GetFileName( apk );
        Log( $"[c:03]Copying {name}..." );
        File.Copy( apk, Config.WorkingDir + "Installer/apks/" + name );
      }

      Log( "\n[c:0e]Creating archive, please wait..." );
      Utils.CreateZip( installerDir, "Installer.zip" );
      Log( "\n[c:0a]Done" );
    }

    private static void NameAndIntegrityCheck()
    {
      Log( "[c:0e]Naming & integrity check started\n" );
      var apkFiles = Utils.FindFiles( "apk" );
      var verificationError = false;
      var namingError = false;
      var namingErrorList = new List<string>();
      foreach ( var apk in apkFiles )
      {
        var apkName = Path.GetFileName( apk );
        var name = Utils.CheckName( apkName );
        var nameResult = name ? "[c:0a]passed" : "[c:0c]failed";
        var zip = new ZipFile( apk );
        var verification = zip.TestArchive( true );
        zip.Close();
        var verificationResult = verification ? "[c:0a]passed" : "[c:0c]failed";
        Log( $"[c:0b]{apkName}[c:08] - naming {nameResult}[c:08], integrity {verificationResult}" );
        if ( !verification )
          verificationError = true;
        if ( name ) continue;
        namingError = true;
        namingErrorList.Add( apk );
      }

      if ( verificationError )
        Log(
          "\n[c:06]Looks like you have corrupted apk files\n**OR**\nIf you're using modified version of app - it cannot be installed without PM patch\n" );

      if ( namingError )
      {
        Log( "[c:06]Do you want to rename apk files automatically? (y/n)" );
        var choose = Utils.Chooser();
        if ( choose == 'y' )
          foreach ( var (apk, name, folder, newName) in from apk in namingErrorList
                                                        let name = Path.GetFileName( apk )
                                                        let folder = Path.GetDirectoryName( apk )
                                                        let newName = Utils.RemoveNamingErrors( name )
                                                        select (apk, name, folder, newName) )
          {
            Log( $"Renaming {name} to {newName}" );
            File.Move( apk, folder + "/" + newName );
            Utils.Wait();
          }
      }

      Log( "\n[c:02]Done" );
    }
  }
}
