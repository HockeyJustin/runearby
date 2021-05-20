using CoreLookup.AllCountries;
using CoreLookup.Helpers;
using GeoUK.Coordinates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.UKSpecific
{
	public interface IConvertEastingsNorthingsToLatLongUK
	{
		/// <summary>
		/// Convert OS Eastings/Northings data to corresponding files with Lat/Long data instead
		/// </summary>
		Task ConvertOSEastingsNorthingsDataToLatLongUK();

		/// <summary>
		/// Merge all the postcode files to one big file
		/// </summary>
		void MergeCsvFiles();
	}

	public class ConvertENtoLatLongUk : IConvertEastingsNorthingsToLatLongUK
	{
		ICsvParser _csvParser;
		IFileHelper _fileHelper;
		IGeoUKHelper _geoUKHelper;
		ILocationLatLngMethods _postcodeZipLatLongMethods;

		string slnPath = "";
		string ukRawDataFolder = "";
		string ukLatLongDataFolder = "";


		public ConvertENtoLatLongUk(ICsvParser csvParser,
			IFileHelper fileHelper,
		IGeoUKHelper geoUKHelper,
		ILocationLatLngMethods postcodeZipLatLongMethods)
		{
			_csvParser = csvParser;
			_fileHelper = fileHelper;
			_geoUKHelper = geoUKHelper;
			_postcodeZipLatLongMethods = postcodeZipLatLongMethods;
			

			slnPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"Demo\bin\Debug\netcoreapp3.1", "");
			ukRawDataFolder = Path.Combine(slnPath, @"CoreLookup\Data\UK\CodePointOpen2020\Data\CSV");
			ukLatLongDataFolder = Path.Combine(slnPath, @"CoreLookup\Data\UK\CodePointOpen2020\Data_Processed_LatLong");
		}


		public async Task ConvertOSEastingsNorthingsDataToLatLongUK()
		{
			
			var filesToProcess = Directory.GetFiles(ukRawDataFolder);
			Stopwatch st = new Stopwatch();
			st.Start();

			foreach (var file in filesToProcess)
			{
				try
				{
					var contentRows = _csvParser.ReadCsvFromFile(file);

					List<LocationLatLng> latLongRows = new List<LocationLatLng>();

					foreach (var row in contentRows)
					{
						if (String.IsNullOrWhiteSpace(row))
						{
							continue;
						}

						var split = row.Split(',');

						var postcode = split[0];
						Console.WriteLine(st.ElapsedMilliseconds + ":" + postcode);
						var easting = double.Parse(split[2]); // No Try here. We want it to bomb out if there's an error.
						var northing = double.Parse(split[3]);

						// A postcode covers multiple houses.
						// As it covers a fairly wide area, we don't need to go ultra-fine in the conversion.
						LatitudeLongitude latLong = _geoUKHelper.ConvertEastNorthToLatLong_LowerAccuracyFast(easting, northing);

						var gmaps = string.Format("https://www.google.co.uk/maps/@{0},{1},19z", Math.Round(latLong.Latitude, 6), Math.Round(latLong.Longitude, 6));

						// We don't need more than 6 decimal places (11.1cm accuracy)
						// https://gis.stackexchange.com/questions/8650/measuring-accuracy-of-latitude-and-longitude?newreg=5f6bf4ba81534a7ea34606ceceb00b35
						var latRounded = Math.Round(latLong.Latitude, 6);
						var longRounded = Math.Round(latLong.Longitude, 6);

						LocationLatLng postcodeZipLatLong = new LocationLatLng(postcode, latRounded, longRounded);
						latLongRows.Add(postcodeZipLatLong);
					}

					FileInfo fi = new FileInfo(file);
					var newFilePath = Path.Combine(ukLatLongDataFolder, fi.Name);

					await _postcodeZipLatLongMethods.SavePostcodeZipLatLongToFile(newFilePath, latLongRows);

					// Clear the data for quicker garbage disposal.
					latLongRows.Clear();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
					Console.ReadLine();
				}
			}

		}


		public void MergeCsvFiles()
		{
			_fileHelper.MergeTextFiles(ukLatLongDataFolder, "uk_postcodes_lat_long_full.csv");
		}

	}
}
