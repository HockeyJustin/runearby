using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.UKSpecific
{
	/// <summary>
	/// This is the main entry point for converting 
	/// the Open Source Ordinance Survey Map 'CodePointOpen' data
	/// (https://osdatahub.os.uk/downloads/open/CodePointOpen)
	/// from fine grained Eastings Northings data 
	/// to fine grained Lat Long data (e.g. PO4 8RA,{E},{N} -> PO4 8RA,{lat},{long})
	/// then to more general Lat Long data (e.g. PO4 8RA, PO4 8SL -> PO4,{lat},{long})
	/// </summary>
	public interface IOSMapConverter
	{
		Task Convert_OS_CodePointOpen_Data_To_Usable_LatLong_Data();
	}


	public class OSMapConverter : IOSMapConverter
	{
		IConvertEastingsNorthingsToLatLongUK _ukConverter;
		IGeneraliseUkPostcodes _generaliseUkPostcodes;

		public OSMapConverter(IConvertEastingsNorthingsToLatLongUK ukConverter,
			IGeneraliseUkPostcodes generaliseUkPostcodes)
		{
			_ukConverter = ukConverter;
			_generaliseUkPostcodes = generaliseUkPostcodes;
		}


		public async Task Convert_OS_CodePointOpen_Data_To_Usable_LatLong_Data()
		{
			/// OS data for the UK details Eastings and Northings,
			/// which is fine for cartographers, but doesn't help the normal world.
			/// This converts all the postcodes to one "Postcode,Lat,Long" CSV
			Console.WriteLine("Running Convert_UK_OS_Data_To_One_LatLong_File");
			await Task.Delay(2000);
			await _ukConverter.ConvertOSEastingsNorthingsDataToLatLongUK();

			// Make One Big CSV of all the data to save loading in multipe files in future
			Console.WriteLine("Merging CSV Files");
			await Task.Delay(2000);
			_ukConverter.MergeCsvFiles();

			Console.WriteLine("Generalising Postcodes");
			await Task.Delay(2000);
			await _generaliseUkPostcodes.ConvertSpecificPostcodesToGeneralised();
		}



	}
}
