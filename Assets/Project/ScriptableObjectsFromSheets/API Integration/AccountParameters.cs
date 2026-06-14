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

		public static string ServiceAccountID { 
			get 
			{
				try 
				{
				    StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, ServiceAccountJsonPath, "AccountID.txt"));
					return sr.ReadLine();
				}
				catch (Exception exception) 
				{
				    Debug.LogException(exception);
					return null;
				}
			}
		}

		public static string PrivateKey { 
			get 
			{
				try 
				{
				    StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, ServiceAccountJsonPath, "Key.txt"));
					return sr.ReadLine().Replace("\\n", "\n");
				}
				catch (Exception exception) 
				{
				    Debug.LogException(exception);
					return null;
				}
			}
		}
	}
}
