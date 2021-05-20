using CoreLookup.AllCountries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.UKSpecific
{
	public interface IProcessedUkData
	{
		Task<List<LocationLatLng>> GetData(PostcodePartsType processedUkDataType);
	}

	public class ProcessedUkData : IProcessedUkData
	{
		ILocationLatLngMethods _postcodeZipLatLongMethods;

		public ProcessedUkData(ILocationLatLngMethods postcodeZipLatLongMethods)
		{
			_postcodeZipLatLongMethods = postcodeZipLatLongMethods;
		}

		public async Task<List<LocationLatLng>> GetData(PostcodePartsType processedUkDataType)
		{
			var slnPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"Demo\bin\Debug\netcoreapp3.1", "");
			var processedDataFolder = Path.Combine(slnPath, @"CoreLookup\Data\UK\CodePointOpen2020\Data_Processed_LatLong\Merged");

			string csvPath = "";
			if(processedUkDataType == PostcodePartsType.Full)
			{
				csvPath = "uk_postcodes_lat_long_full.csv";
			}
			else if(processedUkDataType == PostcodePartsType.AreaAndDistrict)
			{
				csvPath = "uk_postcodes_lat_long_area_district_only.csv";
			}
			else
			{
				throw new NotImplementedException("Need to add the csv's to make this happen.");
			}

			var postcodeDataPath = Path.Combine(processedDataFolder, csvPath);

			var lookupData = await _postcodeZipLatLongMethods.GetPostcodeZipLatLongFromFile(postcodeDataPath);

			return lookupData;
		}

	}


	




}
