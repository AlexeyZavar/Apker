// Market.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using static Apker.Logger;

#endregion

namespace Apker
{
  public static class Market
  {
    private static List<App> Repo { get; set; }
    public static IBrowsingContext Context { get; set; }
    public static string RepoDir { get; set; }

    private static async Task<App> GetInformation(string packageName)
    {
      var document = await Context.OpenAsync( $"https://apkcombo.com/en-en/{packageName}" );

      if ( document.StatusCode == HttpStatusCode.NotFound )
        return null;

      var apkBtn =
        document.Body.QuerySelector(
          "body > section > div > div > div.column.is-8 > div.abuttons > a.abutton.is-success.is-fullwidth" );
      var obbBtn =
        document.Body.QuerySelector(
          "body > section > div > div > div.column.is-8 > div.abuttons > a.abutton.is-rounded.is-fullwidth" );

      var apkUrl = ((IHtmlAnchorElement) apkBtn).Href;
      string obbUrl = null;
      if ( obbBtn != null )
        obbUrl = ((IHtmlAnchorElement) obbBtn).Href;


      var name = document.Body.QuerySelector(
                            "body > section > div > div > div.column.is-8 > article > div:nth-child(2) > h1 > a" )
                         .InnerHtml;
      var version = document.Body.QuerySelector(
                               "body > section > div > div > div.column.is-8 > table:nth-child(21) > tbody > tr:nth-child(2) > td:nth-child(2)" )
                            .InnerHtml;
      var size = document.Body.QuerySelector(
                            "body > section > div > div > div.column.is-8 > table:nth-child(21) > tbody > tr:nth-child(4) > td:nth-child(2)" )
                         .InnerHtml;

      var regex = new Regex( "\\([0-9]*\\)" );

      var numVersion = regex.Match( version ).ToString().Replace( "(", "" ).Replace( ")", "" );
      version = regex.Replace( version, "" ).Replace( " ", "" );

      return new App( packageName, name, version, numVersion, size, apkUrl, obbUrl );
    }

    public static void Menu()
    {
      again:
      Console.Clear();
      Log( "[c:03]Google Play fetcher[c:08]:" );
      Log( "1. [c:0a]Download & add app" );
      Log( "2. [c:0c]Remove app" );
      Log( "3. [c:09]Update all apps" );
      Log( "4. [c:0b]List all apps" );
      Log( "[c:08]\nr. [c:04]Return" );
      SaveRepo();
      var choose = Utils.Chooser();
      switch ( choose )
      {
        case '1':
          Download();
          break;
        case '2':
          Remove();
          break;
        case '3':
          Update();
          break;
        case '4':
          ListApps();
          break;
        case 'r':
          goto exit;
      }

      goto again;
      exit: ;
    }

    private static void ListApps()
    {
      Console.Clear();
      Log( "[c:03]Apps list:" );
      foreach ( var app in Repo ) Log( $"[c:0e]{app.Name}: [c:09]{app.Version}, {app.Size}" );
      Utils.WaitForPress();
    }

    private static void Update()
    {
      Console.Clear();
      var tempRepo = Repo;
      foreach ( var app in tempRepo )
      {
        Log( $"[c:0b]Checking [c:03]{app.Name}[c:0b] for updates" );
        var app2 = GetInformation( app.Package ).Result;
        if ( app2.NumVersion == app.NumVersion )
        {
          Log( $"[c:03]{app.Name}[c:0b] is up-to-date" );
          continue;
        }

        Log( $"[c:03]{app.Name}[c:0b]: {app.Version} != {app2.Version}" );
        RemoveApp( app );
        DownloadApp( app );
        Log( $"[c:0b]{app.Name}[c:0b] now is up-to-date\n" );
      }

      Utils.WaitForPress();
    }

    private static void Remove()
    {
      var package = Utils.GetInput( "Package name: " );
      var app = FindByPackageName( package );
      if ( FindByPackageName( package ) == null )
      {
        Log( "[c:0c]This app doesn't exists in the repository!" );
        Utils.Wait();
        return;
      }

      RemoveApp( app );
      Log( $"[c:0a]{app.Name} removed" );
      Utils.WaitForPress();
    }

    private static void RemoveApp(App app)
    {
      File.Delete( RepoDir + ApkNameBuilder( app.Name, app.Version ) + ".apk" );
      if ( app.ObbUrl != null )
        Directory.Delete( RepoDir + app.Package, true );
      Repo.Remove( app );
    }

    private static App FindByPackageName(string package)
    {
      return Repo.Find( x => x.Package.Contains( package ) );
    }

    private static void Download()
    {
      var package = Utils.GetInput( "Package name: " );
      if ( FindByPackageName( package ) != null )
      {
        Log( "[c:0c]This app already exists in the repository!" );
        Utils.Wait();
        return;
      }

      var app = GetInformation( package ).Result;

      DownloadApp( app );

      Repo.Add( app );

      Log( $"[c:0a]{app.Name} downloaded" );
      Utils.WaitForPress();
    }

    private static void DownloadApp(App app)
    {
      DownloadApk( app );

      if ( app.ObbUrl != null )
        DownloadObb( app );
    }

    private static void DownloadApk(App app)
    {
      var apkUrl = GetDownloadUrl( app.ApkUrl );

      Utils.Download(
        apkUrl, RepoDir + ApkNameBuilder( app.Name, app.Version ) + ".apk" );
    }

    private static void DownloadObb(App app)
    {
      var obbUrl = GetDownloadUrl( app.ObbUrl );

      Directory.CreateDirectory( RepoDir + app.Package );

      Utils.Download(
        obbUrl, RepoDir + app.Package + "/" + ObbNameBuilder( app.NumVersion, app.Package ) );
    }

    private static string ObbNameBuilder(string numVersion, string package)
    {
      return $"main.{numVersion}.{package}.obb";
    }

    private static void SaveRepo()
    {
      var formatter = new BinaryFormatter();
      using var fs = new FileStream( "repo.dat", FileMode.OpenOrCreate );
      formatter.Serialize( fs, Repo );
    }

    public static void LoadRepo()
    {
      if ( !File.Exists( "repo.dat" ) )
      {
        Repo = new List<App>();
        return;
      }

      var formatter = new BinaryFormatter();
      using var fs = new FileStream( "repo.dat", FileMode.OpenOrCreate );
      var cfg = (List<App>) formatter.Deserialize( fs );
      Repo = cfg;
    }

    private static string GetDownloadUrl(string url)
    {
      Context.OpenAsync( url ).Result.Dispose();

      Utils.Wait();

      var result = Context.OpenAsync( url ).Result;

      var loader = (IHtmlAnchorElement) result.Body.QuerySelector(
        "#download-result > div.has-text-centered > div > table > tbody > tr > td:nth-child(1) > p > a" );

      while ( loader == null )
      {
        Thread.Sleep(4500);
        result = Context.OpenAsync( url ).Result;
        loader = (IHtmlAnchorElement) result.Body.QuerySelector(
          "#download-result > div.has-text-centered > div > table > tbody > tr > td:nth-child(1) > p > a" );
        result.Dispose();
      }

      var apkUrl = ((IHtmlAnchorElement) result.Body.QuerySelector(
                       "#download-result > div.has-text-centered > div > table > tbody > tr > td:nth-child(1) > p > a" )
                   ).Href;
      return apkUrl;
    }

    private static string ApkNameBuilder(string name, string version)
    {
      return name.ToUpper().Replace( "-", "" ).Replace( "&AMP;", "." ).Replace( " ", "." ).Replace( "'", "" ) + "." +
             version.Replace( "_", "." );
    }
  }
}
