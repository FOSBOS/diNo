namespace System.Runtime.CompilerServices
{
  // Polyfill: .NET Framework 4.8 bringt diesen Marker-Typ nicht mit, der C#-Compiler
  // braucht ihn aber, um "init"-Accessoren zuzulassen (u. a. vom CommunityToolkit.Mvvm-
  // Quellgenerator verwendet).
  internal static class IsExternalInit { }
}
