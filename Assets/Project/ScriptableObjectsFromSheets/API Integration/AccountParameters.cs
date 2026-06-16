using System;
using System.IO;
using UnityEngine;

namespace ScriptableObjectsFromSheets.APIIntegration
{	
	/// <summary>
	/// Stores the Account ID and Private Key of the Service Account
	/// </summary>
	public static class AccountParameters
	{
		public const string PlayerPrefsKey = "ServiceAccountJsonPath";
		public static string ServiceAccountJsonPath = PlayerPrefs.GetString(PlayerPrefsKey, "");

		public static void UpdateServiceAccountJsonPath(string path)
		{
			PlayerPrefs.SetString(PlayerPrefsKey, path);
			ServiceAccountJsonPath = PlayerPrefs.GetString(PlayerPrefsKey, "");
		}
	}
}
