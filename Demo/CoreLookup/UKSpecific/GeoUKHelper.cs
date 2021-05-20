using System;
using System.Collections.Generic;
using System.Text;
using GeoUK;
using GeoUK.Projections;
using GeoUK.Coordinates;
using GeoUK.Ellipsoids;

namespace CoreLookup.UKSpecific
{

	public interface IGeoUKHelper
	{
		LatitudeLongitude ConvertEastNorthToLatLong_HigherAccuracySlow(double easting, double northing);

		LatitudeLongitude ConvertEastNorthToLatLong_LowerAccuracyFast(double easting, double northing);

	}


	/// <summary>
	/// // GeoUK - https://bitbucket.org/johnnewcombe/geouk/src/master/
	///	// NUGET -> GEOUK.OSTN (more accurate - also john newcombe)
	//	   https://www.codeproject.com/Articles/1007147/Converting-Latitude-and-Longitude-to-British-Natio
	/// </summary>
	public class GeoUKHelper : IGeoUKHelper
	{


		public LatitudeLongitude ConvertEastNorthToLatLong_HigherAccuracySlow(double easting, double northing)
		{
			Osgb36 accurate = new Osgb36(easting, northing);
			return GeoUK.OSTN.Transform.OsgbToEtrs89(accurate);
		}


		public LatitudeLongitude ConvertEastNorthToLatLong_LowerAccuracyFast(double easting, double northing)
		{
			// Convert to Cartesian
			var cartesian = GeoUK.Convert.ToCartesian(new Airy1830(),
							new BritishNationalGrid(),
							new EastingNorthing(easting, northing));

			//ETRS89 is effectively WGS84   
			var wgsCartesian = Transform.Osgb36ToEtrs89(cartesian);

			var wgsLatLong = GeoUK.Convert.ToLatitudeLongitude(new Wgs84(), wgsCartesian);

			return wgsLatLong;

		}
	}
}
