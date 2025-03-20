using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using SOFromSheets.Integration;
using UnityEngine;

namespace SOFromSheets.Controllers
{
	/// <summary>
	/// Class where all Sheets API operations are handled.
	/// </summary>
	public class GoogleSheetsService
	{
		private static readonly SheetsService _service;
		
		static GoogleSheetsService() 
		{
		    ServiceAccountCredential.Initializer initializer = new ServiceAccountCredential.Initializer(AccountParameters.ServiceAccountID);

			ServiceAccountCredential credential = new ServiceAccountCredential(initializer.FromPrivateKey(AccountParameters.PrivateKey));
			
			BaseClientService.Initializer baseServiceInitializer = new BaseClientService.Initializer() 
			{
				HttpClientInitializer = credential
			};
			
			_service = new SheetsService(baseServiceInitializer);
		}
		
		public static List<List<string>> GetRange(string sheetId, string range) 
		{
			SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(sheetId, range);
			ValueRange response = request.Execute();

			List<List<string>> values = response.Values.Select(row => row.Select(cell => cell?.ToString() ?? string.Empty).ToList()).ToList();
			return values;
		}
		
		public static async Task<List<List<string>>> GetRangeAsync(string sheetId, string range) 
		{
		    SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(sheetId, range);
			ValueRange response = await request.ExecuteAsync();

			List<List<string>> values = response.Values.Select(row => row.Select(cell => cell?.ToString() ?? string.Empty).ToList()).ToList();
			
			return values;
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
	}
}
