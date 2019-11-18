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
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using static Apker.Logger;

#endregion

namespace Apker
{
  public static class Market
  {
    private static List<App> repo { get; set; }
    public static IBrowsingContext Context { get; set; }

    public static async Task<App> GetInformation(string packageName)
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

      //version = regex.Match( version ).ToString().Replace( "(", "" ).Replace( ")", "" );
      version = regex.Replace( version, "" ).Replace( " ", "" );

      return new App( packageName, name, version, size, apkUrl, obbUrl );
    }

    public static void Menu()
    {
      repo = new List<App>();
      again:
      Console.Clear();
      Log( "[c:03]Google play fetcher[c:08]:" );
      Log( "1. [c:0a]Download & add app" );
      Log( "2. [c:0c]Remove app" );
      Log( "3. [c:09]Update all apps" );
      Log( "[c:08]\nr. [c:04]Return" );
      var choose = Utils.Chooser();
      switch ( choose )
      {
        case '1':
          var package = Utils.GetInput( "Package name: " );
          if ( repo.Find( x => x.Package.Contains( package ) ) != null )
          {
            Log( "[c:0c]This app already exists in the repository!" );
            Utils.Wait();
            goto again;
          }

          var app = GetInformation( package ).Result;

          using ( var client = new WebClient() )
          {
            var apk = Context.OpenAsync( app.ApkUrl );
            var apkUrl = ((IHtmlAnchorElement) apk.Result.Body.QuerySelector(
                             "#download-result > div.has-text-centered > div > table > tbody > tr > td:nth-child(1) > p > a" )
                         ).Href;
            client.DownloadFile(
              apkUrl, Config.GetInstance().WorkingDir + "Repo/" + NameBuilder( app.Name, app.Version ) + ".apk" );

            if ( app.ObbUrl != null )
            {
              var obb = Context.OpenAsync( app.ObbUrl );
              var obbElement = (IHtmlAnchorElement) obb.Result.Body.QuerySelector(
                "#download-result > div.has-text-centered > div > table > tbody > tr > td:nth-child(1) > p > a" );
              var obbUrl = obbElement.Href;
              var obbName = obbElement.InnerHtml;
              Directory.CreateDirectory(Config.GetInstance().WorkingDir + "Repo/" + package);
              client.DownloadFile(
                obbUrl,
                Config.GetInstance().WorkingDir + "Repo/" + package + "/" + obbName +
                ".obb" );
            }
          }

          Log( "Downloaded" );
          Utils.WaitForPress();
          break;
      }

      goto again;
    }

    private static void SaveToFile()
    {
      var formatter = new BinaryFormatter();
      using var fs = new FileStream( "repo.dat", FileMode.OpenOrCreate );
      formatter.Serialize( fs, repo );
    }

    public static void LoadFromFile()
    {
      var formatter = new BinaryFormatter();
      using var fs = new FileStream( "repo.dat", FileMode.OpenOrCreate );
      var cfg = (List<App>) formatter.Deserialize( fs );
      repo = cfg;
    }

    private static string NameBuilder(string name, string version)
    {
      return name.ToUpper().Replace( "-", "." ).Replace( " ", "." ) + "." + version;
    }
  }
}
