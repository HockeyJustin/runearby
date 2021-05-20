using CoreLookup.AllCountries;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitAndIntegrationTests.CoreLookup.AllCountries
{
	public static class Stubs
	{
		public static List<LocationLatLng> GetFakeVendors_WithRadiiTheyWillVisit()
		{
			List<LocationLatLng> lookupdata = new List<LocationLatLng>();
			lookupdata.Add(new LocationLatLng("Aberdeen Vendor", 57.108239, -2.23697, 50000)); // Aberdeen
			lookupdata.Add(new LocationLatLng("Portsmouth Vendor", 50.790176, -1.062084, 48000)); // Portsmouth
			lookupdata.Add(new LocationLatLng("IoW Vendor", 50.691012, -1.313466, 12000)); // Isle of Wight
			lookupdata.Add(new LocationLatLng("East Soton Vendor", 50.907201, -1.397348, 35000)); // Southampton (east side of) - (edge case inside)
			lookupdata.Add(new LocationLatLng("West Soton Vendor", 51.116668, -1.51669, 29000)); // Southhampton (edge case outside)
			lookupdata.Add(new LocationLatLng("Brighton Vendor", 50.842346, -0.139502, 4000)); // Brighton City Center - Out.

			lookupdata.Add(new LocationLatLng("Bristol Vendor", 51.458457, -2.574165, 64000)); // Bristol (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("Gloucester Wales Vendor", 51.692217, -2.45966, 12000));  // Gloucester, Eng (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("Gloucester England Vendor", 51.760229, -2.472594, 9000)); // Gloucester, Wales (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("Cardiff Vendor", 51.479102, -3.178085, 64000)); // Cardiff (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("Newport Vendor", 51.609115, -2.849027, 600)); // Newport (for Bristol Channel test)
			return lookupdata;
		}

	}
}
