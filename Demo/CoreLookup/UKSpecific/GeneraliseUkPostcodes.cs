using CoreLookup.AllCountries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.UKSpecific
{

	public interface IGeneraliseUkPostcodes
	{
		/// <summary>
		/// This method will take the lat-long data for specific postcodes
		/// and make something more generalised.
		/// e.g. PO4 1AX, PO4 1BY -> PO4 with average lat/long coordinates.
		/// </summary>
		/// <returns>Outputs the data to a file</returns>
		Task ConvertSpecificPostcodesToGeneralised();
	}


	public class GeneraliseUkPostcodes : IGeneraliseUkPostcodes
	{
		ILocationLatLngMethods _postcodeZipLatLongMethods;

		public GeneraliseUkPostcodes(ILocationLatLngMethods postcodeZipLatLongMethods)
		{
			_postcodeZipLatLongMethods = postcodeZipLatLongMethods;
		}


		public async Task ConvertSpecificPostcodesToGeneralised()
		{

			var slnPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"Demo\bin\Debug\netcoreapp3.1", "");
			var processedDataFolder = Path.Combine(slnPath, @"CoreLookup\Data\UK\CodePointOpen2020\Data_Processed_LatLong\Merged");
			var startingFile = Path.Combine(processedDataFolder, "uk_postcodes_lat_long_full.csv");
			var finalFile = Path.Combine(processedDataFolder, "uk_postcodes_lat_long_area_district_only.csv");

			// 1. Read in the merged data
			List<LocationLatLng> fullPostcodesList = await _postcodeZipLatLongMethods.GetPostcodeZipLatLongFromFile(startingFile);

			// 2. Loop throug the data
			string currentEvalPostcode = "";
			List<LocationLatLng> currentEvalPostcodeLookup = new List<LocationLatLng>();
			List<LocationLatLng> finalShortPostcodes = new List<LocationLatLng>();

			for (int i = 0; i < fullPostcodesList.Count; i++)
			{
				var pCode = fullPostcodesList[i];
				var postcodeStart = "";
				if (pCode.Identifier.Contains(" "))
				{
					// e.g. PO4 8ZW
					postcodeStart = pCode.Identifier.Split(' ')[0];
				}
				else
				{
					// e.g. PO142PY
					postcodeStart = pCode.Identifier.Substring(0, 4);
				}

				if ((String.IsNullOrWhiteSpace(currentEvalPostcode) || postcodeStart == currentEvalPostcode)
					&& i != fullPostcodesList.Count - 1)
				{
					currentEvalPostcode = postcodeStart;
					currentEvalPostcodeLookup.Add(pCode);
				}
				else
				{
					// The list has moved on. So we need to process the last batch of data and start afresh.
					var previousSetCondensed = await CondensePostcodes(currentEvalPostcode, currentEvalPostcodeLookup);
					finalShortPostcodes.Add(previousSetCondensed);
					Console.WriteLine($"Processed Postcodes: {currentEvalPostcode}");

					// Set up the new data
					currentEvalPostcodeLookup = new List<LocationLatLng>();
					currentEvalPostcode = postcodeStart;
					currentEvalPostcodeLookup.Add(pCode); // This is the next postcode along. Make sure to include it :)
				}
			}


			await _postcodeZipLatLongMethods.SavePostcodeZipLatLongToFile(finalFile, finalShortPostcodes);

		}



		public async Task<LocationLatLng> CondensePostcodes(string shortPostcode, List<LocationLatLng> postcodes)
		{
			LocationLatLng returnValue = new LocationLatLng();
			returnValue.Identifier = shortPostcode;

			returnValue.Latitude = Math.Round(postcodes.Average(_ => _.Latitude), 6);
			returnValue.Longitude = Math.Round(postcodes.Average(_ => _.Longitude), 6);

			return returnValue;
		}




		





	}
}
