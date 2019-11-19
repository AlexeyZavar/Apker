// App.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;

#endregion

namespace Apker
{
  [Serializable]
  public class App
  {
    public App(string package, string name, string version, string numVersion, string size, string apkUrl,
               string obbUrl)
    {
      Package = package;
      Name = name;
      Version = version;
      NumVersion = numVersion;
      Size = size;
      ApkUrl = apkUrl;
      ObbUrl = obbUrl;
    }

    public string Package { get; }
    public string Name { get; }
    public string Version { get; }
    public string NumVersion { get; }
    public string Size { get; }
    public string ApkUrl { get; }
    public string ObbUrl { get; }
  }
}
