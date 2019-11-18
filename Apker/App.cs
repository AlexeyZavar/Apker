// App.cs is a part of Apker
// 
// Created by AlexeyZavar

namespace Apker
{
  public class App
  {
    public App(string package, string name, string version, string size, string apkUrl, string obbUrl)
    {
      Package = package;
      Name = name;
      Version = version;
      Size = size;
      ApkUrl = apkUrl;
      ObbUrl = obbUrl;
    }

    public static string Package { private set; get; }
    public static string Name { private set; get; }
    public static string Version { private set; get; }
    public static string Size { private set; get; }
    public static string ApkUrl { private set; get; }
    public static string ObbUrl { private set; get; }
  }
}
