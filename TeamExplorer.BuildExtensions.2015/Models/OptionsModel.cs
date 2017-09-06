using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.Win32;

namespace BuildTree.Models
{
	public class OptionsModel
	{
		private const string SettingsCollectionPath = @"BuildTreePackage";
		private const string SplitCharacterString = "SplitCharacter";
		private IServiceProvider _provider;

		public void Initialize(IServiceProvider provider)
		{
			_provider = provider;
			LoadSettings();
		}

		public void LoadSettings()
		{
			if (_provider != null)
			{
				var settingsManager = new ShellSettingsManager(_provider);
				var store = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
				SplitCharacter = Convert.ToChar(store.GetUInt32(SettingsCollectionPath, SplitCharacterString, '.'));
			}
			else
			{
				SplitCharacter = '.';
			}
		}

		public void SaveSettings()
		{
			try
			{
				var settingsManager = new ShellSettingsManager(_provider);
				var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
				if (!store.CollectionExists(SettingsCollectionPath))
				{
					store.CreateCollection(SettingsCollectionPath);
				}
				store.SetUInt32(SettingsCollectionPath, SplitCharacterString, Convert.ToUInt32(SplitCharacter));
			}
			catch
			{
				// do nothing
			}
		}

		public char? SplitCharacter { get; set; }
	}
}
