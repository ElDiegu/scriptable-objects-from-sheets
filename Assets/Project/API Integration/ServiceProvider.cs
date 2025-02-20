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
	
	public static class AccountParameters 
	{
		public static string ServiceAccountID { get => Resources.Load<TextAsset>("Credentials/AccountID").text; private set {} }
		public static string PrivateKey { get => Resources.Load<TextAsset>("Credentials/Key").text.Replace("\\n", "\n"); private set {} }
	}
}
