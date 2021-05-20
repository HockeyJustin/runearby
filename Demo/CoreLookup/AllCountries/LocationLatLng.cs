using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLookup.AllCountries
{
	public class LocationLatLng
	{
		public LocationLatLng()
		{

		}

		public LocationLatLng(string postcodeCsvRow)
		{
			var row = postcodeCsvRow.Split(',');
			Identifier = row[0].ToString().Replace("\"", "");
			Latitude = double.Parse(row[1]);
			Longitude = double.Parse(row[2]);
		}

		public LocationLatLng(string postcodeOrIdentifier, double lat, double lon)
		{
			Identifier = postcodeOrIdentifier.Replace("\"", "");
			Latitude = lat;
			Longitude = lon;
		}

		public LocationLatLng(string identifier, double lat, double lon, int radiusInMeters)
			:this(identifier, lat, lon)
		{
			RadiusInMeters = radiusInMeters;
		}


		/// <summary>
		/// This could be a postcode e.g. PO4 8RA, OR a name e.g. "Dave"
		/// </summary>
		public string Identifier { get; set; }

		/// <summary>
		/// Latitude
		/// </summary>
		public double Latitude { get; set; }

		/// <summary>
		/// Longitude
		/// </summary>
		public double Longitude { get; set; }

		/// <summary>
		/// A radius from the lat/long in meters.
		/// </summary>
		public int RadiusInMeters { get; set; }


		public string ToPostcodeCsv()
		{
			var latLongTxt = $"\"{Identifier}\",{Latitude},{Longitude}";
			return latLongTxt;
		}



	}
}
