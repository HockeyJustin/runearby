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
	public interface IUkNearestPostcodeToCoordinates
	{
		Task Initalize(PostcodePartsType postcodePartsType = PostcodePartsType.Full);
		Task<LocationLatLng> GetClosestPostcodeToCoordinate(double lat, double lon);
	}


	public class UkNearestPostcodeToCoordinates : IUkNearestPostcodeToCoordinates
	{
		IProcessedUkData _processedUkData;
		List<LocationLatLng> _lookupData = new List<LocationLatLng>();

		public UkNearestPostcodeToCoordinates(IProcessedUkData processedUkData)
		{
			_processedUkData = processedUkData;
		}

		public async Task Initalize(PostcodePartsType postcodePartsType = PostcodePartsType.Full)
		{
			if (_lookupData != null || !_lookupData.Any())
			{
				_lookupData.Clear();
				_lookupData = new List<LocationLatLng>();
			}

			_lookupData = await _processedUkData.GetData(postcodePartsType);
		}


		/// <summary>
		/// Returns the closest full postcode to a set of coordinates.
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		/// <returns></returns>
		public async Task<LocationLatLng> GetClosestPostcodeToCoordinate(double lat, double lon)
		{
			if(_lookupData == null || !_lookupData.Any())
			{
				throw new InvalidOperationException("Must initialize the data first");
			}

			LocationLatLng closest = null;

			double closestTotalDifference = 1000000000;

			foreach (var row in _lookupData)
			{
				var latDifference = Math.Abs(lat - row.Latitude);
				var lonDifference = Math.Abs(lon - row.Longitude);
				var totalDifference = latDifference + lonDifference;

				if (totalDifference < closestTotalDifference)
				{
					closestTotalDifference = totalDifference;
					closest = row;

					if (closestTotalDifference == 0)
					{
						break; // We can't get any closer, so stop checking.
					}
				}
			}

			return closest;

		}


	}





}
