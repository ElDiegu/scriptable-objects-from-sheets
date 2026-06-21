using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using ScriptableObjectsFromSheets.Core;
using UnityEngine;

namespace ScriptableObjectsFromSheets.APIIntegration
{
	/// <summary>
	/// Class where all Sheets API operations are handled.
	/// </summary>
	public class GoogleSheetsService
	{
		private static SheetsService _service;
		
		static GoogleSheetsService()
		{
			InitializeService();
		}

		/// <summary>
		/// Initializes the GoogleSheets Service with a provided credentials JSON
		/// </summary>
		private static void InitializeService()
		{
			if (string.IsNullOrEmpty(AccountParameters.PlayerPrefsKey))
			{
				Debug.LogWarning("No credentials provided for ScriptableObjects From Sheets. Google Sheets Service cannot be initialized." +
				                 "\nProvide a valid credentials file on the authentication window.");
			}

			try
			{
				ServiceAccountCredential credential;
				using (var stream =
				       new FileStream(AccountParameters.ServiceAccountJsonPath, FileMode.Open, FileAccess.Read))
				{
					credential = (ServiceAccountCredential)GoogleCredential.FromStream(stream)
						.CreateScoped(SheetsService.Scope.Spreadsheets)
						.UnderlyingCredential; 
				}
			
				BaseClientService.Initializer baseServiceInitializer = new BaseClientService.Initializer() 
				{
					HttpClientInitializer = credential,
					ApplicationName = "ScriptableObjectsFromSheets"
				};
			
				_service = new SheetsService(baseServiceInitializer);
				Debug.Log($"Sheets Service successfully initialized. You can now use ScriptableObjects from Sheets.");
			}
			catch (Exception e)
			{
				Debug.LogError($"Error {e.Message} while initializing Google Sheets service.");
			}
		}

		/// <summary>
		/// Tries to reinitialize Google Sheets service
		/// </summary>
		/// <returns>True if the sheets could be initilized, false otherwise</returns>
		public static bool ResetService()
		{
			try
			{
				InitializeService();
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		/// <summary>
		/// Gets a range from Google Sheets asynchronously
		/// </summary>
		/// <param name="sheetId">ID of the Sheet where to make the query</param>
		/// <param name="range">Range of the query on A1 notation</param>
		/// <returns>A 2x2 Matrix with the values from Google Sheets API as string</returns>
		/// <exception cref="Exception">Returns an Exception if Google Sheets API query returns null</exception>
		public static async Task<List<List<string>>> GetRangeAsync(string sheetId, string range) 
		{
			try
			{
				SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(sheetId, range);
				ValueRange response = await request.ExecuteAsync();
				
				if (response == null) throw new Exception($"No values returned from query range {range} in sheet {sheetId}");
				
				return ConvertValuesToString(response.Values);
			}
			catch (Exception e)
			{
				Debug.LogError($"Error while loading Sheet data: {e.Message}");
				return null;
			}
		}

		/// <summary>
		/// Gets a range from Google Sheets asynchronously
		/// </summary>
		/// <param name="query">Query object with a valid Sheet ID and Range</param>
		/// <returns>A 2x2 Matrix with the values from Google Sheets API as string</returns>
		public static async Task<List<List<string>>> GetRangeAsync(SheetQuery query)
		{
			return await GetRangeAsync(query.SheetId, query.Range);
		}

		/// <summary>
		/// Gets a range from Google Sheets
		/// </summary>
		/// <param name="sheetId">ID of the Sheet where to make the query</param>
		/// <param name="range">Range of the query on A1 notation</param>
		/// <returns>A 2x2 Matrix with the values from Google Sheets API as string</returns>
		public static List<List<string>> GetRange(string sheetId, string range)
		{
			return GetRangeAsync(sheetId, range).Result;
		}

		/// <summary>
		/// Gets a range from Google Sheets
		/// </summary>
		/// <param name="query">Query object with a valid Sheet ID and Range</param>
		/// <returns>A 2x2 Matrix with the values from Google Sheets API as string</returns>
		public static List<List<string>> GetRange(SheetQuery query)
		{
			return GetRangeAsync(query.SheetId, query.Range).Result;
		}
		
		public static void UpdateCell(string sheetId, string cell, string value) 
	    {
	        ValueRange valueRange = new ValueRange();

			var obList = new List<object>() { value };
			valueRange.Values = new List<IList<object>>() { obList };

			SpreadsheetsResource.ValuesResource.UpdateRequest request = _service.Spreadsheets.Values.Update(valueRange, sheetId, cell);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

			var response = request.Execute();

			Debug.Log($"{response.UpdatedCells}");
	    }
	    
	    public static async Task UpdateRangeAsync(string sheetId, string range, List<List<object>> values) 
	    {
	        ValueRange valueRange = new ValueRange();
	        valueRange.Values = (IList<IList<object>>)values;

	        SpreadsheetsResource.ValuesResource.UpdateRequest request = _service.Spreadsheets.Values.Update(valueRange, sheetId, range);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

			var response = await request.ExecuteAsync();

			Debug.Log($"{response.UpdatedCells}");
	    }

	    /// <summary>
	    /// Transforms the values obtained from Google Sheets API into a 2x2 Matrix of strings for processing
	    /// by the rest of the application
	    /// </summary>
	    /// <param name="values">Values obtained from Google Sheets API</param>
	    /// <returns>A 2x2 Matrix with the values from Google Sheets API as string</returns>
	    private static List<List<string>> ConvertValuesToString(IList<IList<object>> values) => 
		    values.Select(row => row.Select(cell => cell?.ToString() ?? string.Empty).ToList()).ToList();
	}
}
