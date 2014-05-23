using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Prolog
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main (string [] args)
    {
      if (args.Length > 0) // batch processing assumed if arguments supplied
      {
        Prolog.MainForm.BatIO batIO = null;

        try
        {
          Engine e = new Engine (batIO = new Prolog.MainForm.BatIO ());
          Engine.ProcessArgs (e, args, true);
          Application.Exit ();

          return;
        }
        finally
        {
          if (batIO != null) batIO.Close ();
        }
      }

      Application.EnableVisualStyles ();
      Application.SetCompatibleTextRenderingDefault (false);
      Application.Run (new MainForm ());
    }
  }
}