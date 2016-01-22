using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.Data;
using System.Runtime.InteropServices;

namespace diNo.OmnisDB
{
  public class OmnisConnection
  {
    #region Dll Imports
    public const int HWND_BROADCAST = 0xFFFF;

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32")]
    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("user32")]
    public static extern int RegisterWindowMessage(string message);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

    [DllImport("user32.dll")]
    internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);
    private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

    // Delegate to filter which windows to include 
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    #endregion Dll Imports

    public OmnisConnection()
    {
      var afenster = FindWindowsWithText("Bayerische Schülerdatei").ToList();
      var kinder = GetAllChildHandles(afenster[0]);


      var alleFenster = FindWindows((hWhn, lparam) => true);

      List<string> fenstertexte = new List<string>();
      foreach (var fenster in kinder)
      {
        string fenstertest = GetWindowText(fenster);
        // Da sucht man nach dem Fenster "Zeugnisdaten - ..."
        if (!string.IsNullOrEmpty(fenstertest) && fenstertest.StartsWith("Zeugnisdaten -"))
        {
          // aktiviere Fenster
          SetFocus(fenster);
        }
      }

      //OdbcConnection conn = new OdbcConnection("DRIVER={OMNIS ODBC Driver};DataFilePath=C:/Projects/diNo/OmnisDB/sd.df1;UserID=hello;Password=world;");
      OdbcConnection conn = new OdbcConnection("DSN=sd");
      //OdbcConnection conn = new OdbcConnection("Driver={OMNIS ODBC Driver};DBQ=C:/Projects/diNo/OmnisDB/sd.df1");
      //OdbcConnection conn = new OdbcConnection("Driver=Omnis ODBC Driver; DataFilePath=C:\\Projects\\diNo\\OmnisDB\\sd.df1;Username = myuser; Password = mypassword");
      //OdbcConnection conn = new OdbcConnection("odbc:DRIVER=OMNIS;DataFilePath=C:\\Projects\\diNo\\OmnisDB\\sd.df1");
      conn.Open();

      var tableDT = conn.GetSchema("TABLES");
      List<object[]> tables = new List<object[]>();
      foreach (DataRow r in tableDT.Rows)
      {
        tables.Add(new object[3] { r[1], r[2], r[3] });
      }

      var command = conn.CreateCommand();
      command.CommandText = "SELECT K_Name FROM DKlassen";
      var cmdResult = command.ExecuteReader();
      foreach (var result in cmdResult)
      {
        result.ToString();
      }

      var insertCommand = conn.CreateCommand();
      insertCommand.CommandText = "INSERT INTO DKlassen (K_Name) Values ('Spass')";
      insertCommand.ExecuteNonQuery();
    }

    public List<IntPtr> GetAllChildHandles(IntPtr aHandle)
    {
      List<IntPtr> childHandles = new List<IntPtr>();

      GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
      IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

      try
      {
        EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
        EnumChildWindows(aHandle, childProc, pointerChildHandlesList);
      }
      finally
      {
        gcChildhandlesList.Free();
      }

      return childHandles;
    }

    private bool EnumWindow(IntPtr hWnd, IntPtr lParam)
    {
      GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

      if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
      {
        return false;
      }

      List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
      childHandles.Add(hWnd);

      return true;
    }

    /// <summary> Get the text for the window pointed to by hWnd </summary>
    public static string GetWindowText(IntPtr hWnd)
    {
      int size = GetWindowTextLength(hWnd);
      if (size > 0)
      {
        var builder = new StringBuilder(size + 1);
        GetWindowText(hWnd, builder, builder.Capacity);
        return builder.ToString();
      }

      return String.Empty;
    }

    /// <summary> Find all windows that match the given filter </summary>
    /// <param name="filter"> A delegate that returns true for windows
    ///    that should be returned and false for windows that should
    ///    not be returned </param>
    public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
    {
      IntPtr found = IntPtr.Zero;
      List<IntPtr> windows = new List<IntPtr>();

      EnumWindows(delegate (IntPtr wnd, IntPtr param)
      {
        if (filter(wnd, param))
        {
          // only add the windows that pass the filter
          windows.Add(wnd);
        }

        // but return true here so that we iterate all windows
        return true;
      }, IntPtr.Zero);

      return windows;
    }

    /// <summary> Find all windows that contain the given title text </summary>
    /// <param name="titleText"> The text that the window title must contain. </param>
    public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
    {
      return FindWindows(delegate (IntPtr wnd, IntPtr param)
      {
        return GetWindowText(wnd).Contains(titleText);
      });
    }
  }
}
