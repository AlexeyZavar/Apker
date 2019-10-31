// SerializeStatic.cs is a part of Apker
// 
// Created by AlexeyZavar

#region

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace Apker
{
  // https://habr.com/ru/post/137405
  public static class SerializeStatic
  {
    public static void Save(Type staticClass, string filename)
    {
      var fields = staticClass.GetProperties();


      var a = new object[fields.Length, 2];
      var i = 0;
      foreach ( var field in fields )
      {
        a[i, 0] = field.Name;
        a[i, 1] = field.GetValue( null );
        i++;
      }

      Stream f = File.Open( filename, FileMode.Create );
      var formatter = new BinaryFormatter();
      formatter.Serialize( f, a );
      f.Close();
    }

    public static void Load(Type staticClass, string filename)
    {
      var fields = staticClass.GetProperties();
      object[,] a;
      Stream f = File.Open( filename, FileMode.Open );
      var formatter = new BinaryFormatter();
      a = formatter.Deserialize( f ) as object[,];
      f.Close();
      if ( a.GetLength( 0 ) != fields.Length ) return;
      var i = 0;
      foreach ( var field in fields )
      {
        if ( field.Name == a[i, 0] as string )
          if ( a[i, 1] != null )
            field.SetValue( null, a[i, 1] );
        i++;
      }
    }
  }
}
