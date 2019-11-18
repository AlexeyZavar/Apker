// MarketApi.cs is a part of Apker
// 
// Created by AlexeyZavar

namespace Apker
{
  public class MarketApi
  {
    // https://play.google.com/store/apps/details?id=com.google.android.youtube&hl=en
    // https://apps.evozi.com/apk-downloader/?id=com.google.android.youtube
    public static App GetInformation(string packageName)
    {
      return new App( null, null, null, null );
    }
  }
}
