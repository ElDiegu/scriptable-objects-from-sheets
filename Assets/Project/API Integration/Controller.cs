using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEngine;

namespace SOFromSheets.Controllers
{
	public class GetController
	{
		public static List<List<string>> GetRange(string sheetId, string range, SheetsService service) 
		{
			SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(sheetId, range);
			ValueRange response = request.Execute();

			List<List<string>> values = response.Values.Select(row => row.Select(cell => cell?.ToString() ?? string.Empty).ToList()).ToList();
			return values;
		}
	}

	public class UpdateController 
	{
	    public static void UpdateCell(string sheetId, string cell, string value, SheetsService service) 
	    {
	        ValueRange valueRange = new ValueRange();

			var obList = new List<object>() { value };
			valueRange.Values = new List<IList<object>>() { obList };

			SpreadsheetsResource.ValuesResource.UpdateRequest request = service.Spreadsheets.Values.Update(valueRange, sheetId, cell);
			request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
			var response = request.Execute();

			Debug.Log($"{response.UpdatedCells}");
	    }
	}
}
