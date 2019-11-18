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

    public string Package { get; }
    public string Name { get; }
    public string Version { get; }
    public string Size { get; }
    public string ApkUrl { get; }
    public string ObbUrl { get; }
  }
}
