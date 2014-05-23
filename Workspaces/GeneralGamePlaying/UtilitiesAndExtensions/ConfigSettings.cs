using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

public class ConfigSettings
{
	private ConfigSettings() { }

	public static string ReadSetting(string key)
	{
		// Get the configuration file.
		Configuration config =
			ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

		// Add an entry to appSettings.
		foreach (string existingKey in config.AppSettings.Settings.AllKeys)
		{
			if (existingKey == key)
			{
				return config.AppSettings.Settings[key].Value;
			}
		}
		return null;
	}

	public static void WriteSetting(string newKey, string newValue, bool refresh)
	{
		// Get the configuration file.
		Configuration config =
			ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

		// Add an entry to appSettings.
		var existingSetting = ReadSetting(newKey);
		if (existingSetting == null)
		{
			config.AppSettings.Settings.Add(newKey, newValue);
		}
		else
		{

			var foo = config.AppSettings.Settings[newKey];
			foo.Value = newValue;
		}

	
		// Save the configuration file.
		config.Save(ConfigurationSaveMode.Modified);

#if OLD_WAY
		// load config document for current assembly
		XmlDocument doc = loadConfigDocument();

		// retrieve appSettings node
		XmlNode node = doc.SelectSingleNode("//appSettings");

		if (node == null)
			throw new InvalidOperationException("appSettings section not found in config file.");

		try
		{
			// select the 'add' element that contains the key
			XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", key));

			if (elem != null)
			{
				// add value for key
				elem.SetAttribute("value", value);
			}
			else
			{
				// key was not found so create the 'add' element 
				// and set it's key/value attributes 
				elem = doc.CreateElement("add");
				elem.SetAttribute("key", key);
				elem.SetAttribute("value", value);
				node.AppendChild(elem);
			}
			doc.Save(getConfigFilePath());
		}
		catch
		{
			throw;
		}
#endif
	}

#if NEVER
	public static void RemoveSetting(string key)
	{
		// load config document for current assembly
		XmlDocument doc = loadConfigDocument();

		// retrieve appSettings node
		XmlNode node = doc.SelectSingleNode("//appSettings");

		try
		{
			if (node == null)
				throw new InvalidOperationException("appSettings section not found in config file.");
			else
			{
				// remove 'add' element with coresponding key
				node.RemoveChild(node.SelectSingleNode(string.Format("//add[@key='{0}']", key)));
				doc.Save(getConfigFilePath());
			}
		}
		catch (NullReferenceException e)
		{
			throw new Exception(string.Format("The key {0} does not exist.", key), e);
		}
	}

	private static XmlDocument loadConfigDocument()
	{
		XmlDocument doc = null;
		try
		{
			doc = new XmlDocument();
			doc.Load(getConfigFilePath());
			return doc;
		}
		catch (System.IO.FileNotFoundException e)
		{
			throw new Exception("No configuration file found.", e);
		}
	}
#endif
	private static string getConfigFilePath(bool includeExtension)
	{
		if (includeExtension)
		{
			return Assembly.GetExecutingAssembly().Location + ".config";			
		}
		else
		{
			return Assembly.GetExecutingAssembly().Location;
		}
	}
}