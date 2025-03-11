using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEngine;

namespace SOFromSheets.Integration
{
	/// <summary>
	/// Provides the Sheets Service required to access all the Sheets API functionalities.
	/// </summary>
	public class SheetsServiceProvider
	{
		public SheetsService service;

		public SheetsServiceProvider() 
		{
			ServiceAccountCredential.Initializer initializer = new ServiceAccountCredential.Initializer(AccountParameters.ServiceAccountID);

			ServiceAccountCredential credential = new ServiceAccountCredential(initializer.FromPrivateKey(AccountParameters.PrivateKey));
			
			BaseClientService.Initializer baseServiceInitializer = new BaseClientService.Initializer() 
			{
				HttpClientInitializer = credential
			};
			
			service = new SheetsService(baseServiceInitializer);
		}
	}
	
	/// <summary>
	/// Stores the Account ID and Private Key of the Service Account
	/// </summary>
	public static class AccountParameters 
	{
		// Temporal setup for testing purposes, in the future, this path should be defined by the user
		// and key and account ID should be parsed from keys.json
		private readonly static string basePath = "Project/Credentials/";

		private static string _serviceAccountID;
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

		private static string _privateKey;
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
