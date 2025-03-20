using System;
using System.IO;
using UnityEngine;

namespace SOFromSheets.Integration
{	
	/// <summary>
	/// Stores the Account ID and Private Key of the Service Account
	/// </summary>
	public static class AccountParameters 
	{
		// Temporal setup for testing purposes, in the future, this path should be defined by the user
		// and key and account ID should be parsed from keys.json
		private readonly static string basePath = "Project/Credentials/";

		public static string ServiceAccountID { 
			get 
			{
				try 
				{
				    StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, basePath, "AccountID.txt"));
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
				    StreamReader sr = new StreamReader(Path.Combine(Application.dataPath, basePath, "Key.txt"));
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
