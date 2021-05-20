using CoreLookup.AllCountries;
using Geolocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.UKSpecific
{
	public interface IUkRadiusCheck
	{
		Task Initalize(PostcodePartsType postcodePartsType = PostcodePartsType.AreaAndDistrict);
		Task<List<string>> FindPostcodesInRadiusMiles(string startingPostcode, PostcodePartsType startingPostcodeType, double radiusInMiles, bool excludeBoatRides, bool avoidClippingOtherSideBristolChannel);
		Task<List<string>> FindPostcodesInRadiusKm(string startingPostcode, PostcodePartsType startingPostcodeType, double radiusInKm, bool excludeBoatRides, bool avoidClippingOtherSideBristolChannel);
	}

	/// <summary>
	/// Uses https://www.nuget.org/packages/Geolocation/
	/// </summary>
	public class UkRadiusCheck : IUkRadiusCheck
	{
		IProcessedUkData _processedUkData;
		List<LocationLatLng> _lookupData = new List<LocationLatLng>();

		public UkRadiusCheck(IProcessedUkData processedUkData)
		{
			_processedUkData = processedUkData;
		}


		public async Task Initalize(PostcodePartsType postcodePartsType = PostcodePartsType.AreaAndDistrict)
		{
			if(_lookupData != null || !_lookupData.Any())
			{
				_lookupData.Clear();
				_lookupData = new List<LocationLatLng>();
			}

			_lookupData = await _processedUkData.GetData(postcodePartsType);
		}


		public async Task<List<string>> FindPostcodesInRadiusMiles(string startingPostcode, PostcodePartsType startingPostcodeType, double radiusInMiles, bool excludeBoatRides, bool avoidClippingOtherSideBristolChannel)
		{
			var radiusInKm = (double)radiusInMiles * ((double)8 / (double)5);
			return await FindPostcodesInRadiusKm(startingPostcode, startingPostcodeType, radiusInKm, excludeBoatRides, avoidClippingOtherSideBristolChannel);
		}


		public async Task<List<string>> FindPostcodesInRadiusKm(string startingPostcode, PostcodePartsType startingPostcodeType, double radiusInKm, bool excludeBoatRides, bool avoidClippingOtherSideBristolChannel)
		{
			if(_lookupData == null || !_lookupData.Any())
			{
				throw new InvalidOperationException("Must initialize the data first");
			}

			List<string> postcodesInRadius = new List<string>();

			// 1. Break the postcode down to the bit we need.
			PostcodeParts postcode = new PostcodeParts(startingPostcode, startingPostcodeType);
			var areaAndDistrict = postcode.GetAreaAndDistrict_10K(); // E.g. PO4


			// 2. Get lat long for the postcode entered.
			var startingLatLong = _lookupData.FirstOrDefault(_ => _.Identifier == areaAndDistrict);

			if (startingLatLong == null)
			{
				throw new InvalidOperationException($"Could not find location for postcode {startingPostcode} -> {areaAndDistrict}");
			}

			var startingLocation = new Coordinate(startingLatLong.Latitude, startingLatLong.Longitude);

			// 3. Loop through all the postcodes to find ones in the radius.
			foreach (var loc in _lookupData)
			{
				var compareLocation = new Coordinate(loc.Latitude, loc.Longitude);
				var distanceInKm = GeoCalculator.GetDistance(startingLocation, compareLocation, 2, DistanceUnit.Kilometers);

				if (distanceInKm < radiusInKm)
				{
					postcodesInRadius.Add(loc.Identifier);
				}
			}


			if(excludeBoatRides)
			{
				var islandAreaPostcodes = NoFastRoutePostcodes.GetIslands_WithOwnAreaPostcodes();
				var islandAreaDistrictPostcodes = NoFastRoutePostcodes.GetIslands_WithAreaDistrictPostcodes();

				bool isStartPointAnIsland = false;

				var startingPostcodeParts = new PostcodeParts(startingPostcode, startingPostcodeType);
				var startingPostcodeArea = startingPostcodeParts.GetArea_250K();
				var startingPostcodeAreaDistrict = startingPostcodeParts.GetAreaAndDistrict_10K();

				var isIslandAreaMatch = islandAreaPostcodes.FirstOrDefault(_ => _ == startingPostcodeArea) != null;
				var isIslandAreaDistrictmatch = islandAreaDistrictPostcodes.FirstOrDefault(_ => _ == startingPostcodeAreaDistrict) != null;

				bool isStartingPointAnIsland = isIslandAreaMatch || isIslandAreaDistrictmatch;

				// Exclude Water Crossings - Kept Simple
				if(isStartingPointAnIsland)
				{
					// Only Keep postcodes in same Area code (cross island e.g. Jersey (JE)/Guernsey (GY))
					postcodesInRadius = postcodesInRadius.Where(_ => _.StartsWith(startingPostcodeArea)).ToList();
					// Then remove anything that isn't in the island postcode area/district list (.e.g. IoW -> PO33 OK, PO5 (pompey) not.
					postcodesInRadius = postcodesInRadius.Where(_ => islandAreaDistrictPostcodes.Contains(_)).ToList();
				}
				else
				{
					// Starting on the mainland - Remove any Island postcode area (e.g. starting at IV [inverness], excluding HS [hebrides])
					List<string> goodPostcodes = new List<string>();
					foreach (var row in postcodesInRadius)
					{
						var rowParts = new PostcodeParts(row, PostcodePartsType.AreaAndDistrict);
						var area = rowParts.GetArea_250K();
						if (!islandAreaPostcodes.Contains(area))
						{
							goodPostcodes.Add(row);
						}
					}
					postcodesInRadius = goodPostcodes;


					// Remove anything that starts with islandAreaPostcodes or is in islandAreaDistrictPostcodes (can simplify as district in lookup data)
					postcodesInRadius = postcodesInRadius.Where(_ => !islandAreaDistrictPostcodes.Contains(_)).ToList();
				}
			}


			if(avoidClippingOtherSideBristolChannel)
			{

				var startingPostcodeParts = new PostcodeParts(startingPostcode, startingPostcodeType);
				var startingPostcodeArea = startingPostcodeParts.GetArea_250K();

				var englandSWalesPostcodeAreas = NoFastRoutePostcodes.GetEngWalesPostcodes_Areas_SplitByWater();
				var gloucester = NoFastRoutePostcodes.GetGloucester_AreaDistrics_SplitByWater();


				var isStartSWEngland = false;
				foreach (var row in englandSWalesPostcodeAreas.OneSide) // ONESIDE = ENG
				{
					if (row == startingPostcodeArea)
					{
						isStartSWEngland = true;
						break;
					}
				}

				if(isStartSWEngland)
				{
					// If we're starting in England, Remove any postcodes that have welsh starters (e.g. CF, NP)
					List<string> goodPostcodes = new List<string>();
					foreach(var row in postcodesInRadius)
					{
						var rowParts = new PostcodeParts(row, PostcodePartsType.AreaAndDistrict);
						var area = rowParts.GetArea_250K();
						if(!englandSWalesPostcodeAreas.OtherSide.Contains(area))
						{
							goodPostcodes.Add(row);
						}
					}
					postcodesInRadius = goodPostcodes;


					// We're in England, so remove the Welsh Gloucester Postcodes (Area+District, so easy to handle).
					postcodesInRadius = postcodesInRadius.Where(_ => !gloucester.OtherSide.Contains(_)).ToList();

				}



				var isStartSouthWales = false;
				foreach(var row in englandSWalesPostcodeAreas.OtherSide) // OtherSide = WALES
				{
					if(row == startingPostcodeArea)
					{
						isStartSouthWales = true;
						break;
					}
				}

				if(isStartSouthWales)
				{
					// If we're starting in Wales, Remove any postcodes that have English starters (e.g. BS, TR)
					List<string> goodPostcodes = new List<string>();
					foreach (var row in postcodesInRadius)
					{
						var rowParts = new PostcodeParts(row, PostcodePartsType.AreaAndDistrict);
						var area = rowParts.GetArea_250K();
						if (!englandSWalesPostcodeAreas.OneSide.Contains(area))
						{
							goodPostcodes.Add(row);
						}
					}
					postcodesInRadius = goodPostcodes;

					// We're in Wales, so remove the English Gloucester Postcodes (Area+District, so easy to handle).
					postcodesInRadius = postcodesInRadius.Where(_ => !gloucester.OneSide.Contains(_)).ToList();

				}

				







				//var southWalesAreaPostcodes = NoFastRoutePostcodes.GetWales_ClippableAreaPostcodes();
				//var southWalesAreaDistrictPostcodes = NoFastRoutePostcodes.GetWales_ClippableAreaDistricts();


				//var startingPostcodeParts = new PostcodeParts(startingPostcode, startingPostcodeType);
				//var startingPostcodeArea = startingPostcodeParts.GetArea_250K();
				//var startingPostcodeAreaDistrict = startingPostcodeParts.GetAreaAndDistrict_10K();

				//var isSWalesAreaMatch = southWalesAreaPostcodes.FirstOrDefault(_ => _ == startingPostcodeArea) != null;
				//var isSWalesAreaDistrictmatch = southWalesAreaDistrictPostcodes.FirstOrDefault(_ => _ == startingPostcodeAreaDistrict) != null;
				//bool isStartingPointInWales = isSWalesAreaMatch || isSWalesAreaDistrictmatch;

				//// Avoid River Severn/Bristol Channel Crossings
				//if (isStartingPointInWales)
				//{
				//	// Only Keep postcodes in same Area code (cross island e.g. Jersey (JE)/Guernsey (GY))
				//	postcodesInRadius = postcodesInRadius.Where(_ => _.StartsWith(startingPostcodeArea)).ToList();
				//	// Then remove anything that isn't in the island postcode area/district list (.e.g. IoW -> PO33 OK, PO5 (pompey) not.
				//	postcodesInRadius = postcodesInRadius.Where(_ => southWalesAreaDistrictPostcodes.Contains(_)).ToList();
				//}
				//else
				//{
				//	// Starting on the mainland
				//	postcodesInRadius = postcodesInRadius.Where(_ => !islandAreaPostcodes.Contains(startingPostcodeArea)).ToList();
				//	// Remove anything that starts with islandAreaPostcodes or is in islandAreaDistrictPostcodes
				//	postcodesInRadius = postcodesInRadius.Where(_ => !islandAreaDistrictPostcodes.Contains(_)).ToList();
				//}

			}



			// 4. Return list
			return postcodesInRadius;
		}



	}
}
