using System;
using System.Data.Common;

namespace Prolog
{
  // Code for handling the SQL predicates
  public partial class Engine
  {
    string sqlProvider;
    string sqlConnectstring;
    DbProviderFactory dbProviderFactory;
    DbConnection dbConnection; // in this version: a single connection only

    void DbConnectionInfoFromConfig (string providerKey, BaseTerm connectionArgs)
    {
      string connectInfo = ConfigSettings.GetConfigSetting (providerKey, null);

      if (connectInfo == null)
        IO.Error ("No SQL provider info found in config file for key '{0}'", providerKey);

      string [] s = connectInfo.Split ('|');

      if (s == null || s.Length != 2)
        IO.Error ("Ill-formatted connection string in config file:\r\n'{0}'", connectInfo);

      sqlProvider = s [0];
      sqlConnectstring = Utils.Format (s [1], connectionArgs);
    }

    void DbOpenConnection ()
    {
      try
      {
        if (sqlProvider == null)
          IO.Error ("No SQL Provider supplied. Use sql_connect/2 or sql_provider/1");

        if (sqlConnectstring == null)
          IO.Error ("No SQL Connectstring supplied Use sql_connect/2 or sql_provider/1");

        dbProviderFactory = DbProviderFactories.GetFactory (sqlProvider);
        dbConnection = dbProviderFactory.CreateConnection ();
        dbConnection.ConnectionString = sqlConnectstring;
        dbConnection.Open ();
      }
      catch (Exception e)
      {
        IO.Fatal (
@"Unable to open database connection.
Provider      : {0}
Connectstring : {1}
System message: {2}",
          sqlProvider, sqlConnectstring, e.Message);
      }
    }

    void DbCloseConnection ()
    {
      if (dbConnection != null)
      {
        dbConnection.Close ();
        dbConnection = null;
      }
    }
  }
}
