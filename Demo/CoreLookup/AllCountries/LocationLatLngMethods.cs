using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.AllCountries
{
	public interface ILocationLatLngMethods
	{
		Task<List<LocationLatLng>> GetPostcodeZipLatLongFromFile(string fileName);
		Task SavePostcodeZipLatLongToFile(string fileName, List<LocationLatLng> postcodeList);
	}

	public class LocationLatLngMethods : ILocationLatLngMethods
	{
		public async Task<List<LocationLatLng>> GetPostcodeZipLatLongFromFile(string fileName)
		{
			List<LocationLatLng> fullPostcodesList = new List<LocationLatLng>();

			var allProcessedData = await File.ReadAllLinesAsync(fileName);

			foreach (var line in allProcessedData)
			{
				if (String.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				LocationLatLng p = new LocationLatLng(line);
				fullPostcodesList.Add(p);
			}

			return fullPostcodesList;
		}


		public async Task SavePostcodeZipLatLongToFile(string fileName, List<LocationLatLng> postcodeList)
		{
			if(postcodeList == null || !postcodeList.Any())
			{
				return;
			}

			StringBuilder sb = new StringBuilder();
			foreach(var postcode in postcodeList)
			{
				sb.AppendLine(postcode.ToPostcodeCsv());
			}

			await File.WriteAllTextAsync(fileName, sb.ToString());
		}


		
	}
}
