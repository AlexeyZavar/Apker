// MarketApi.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using static Apker.Logger;

#endregion

namespace Apker
{
  public static class MarketApi
  {
    public static async Task<App> GetInformation(string packageName)
    {
      var config = Configuration.Default
                                .WithDefaultLoader();

      var context = BrowsingContext.New( config );
      var document = await context.OpenAsync( $"https://apkcombo.com/en-en/{packageName}" );

      if ( document.StatusCode == HttpStatusCode.NotFound )
        return null;

      var apkBtn =
        document.Body.QuerySelector(
          "body > section > div > div > div.column.is-8 > div.abuttons > a.abutton.is-success.is-fullwidth" );
      var obbBtn =
        document.Body.QuerySelector(
          "body > section > div > div > div.column.is-8 > div.abuttons > a.abutton.is-rounded.is-fullwidth" );

      var apkUrl = ((IHtmlAnchorElement) apkBtn).Href;
      var obbUrl = "";
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

      version = regex.Match( version ).ToString().Replace( "(", "" ).Replace( ")", "" );

      return new App( packageName, name, version, size, apkUrl, obbUrl );
    }
  }
}
