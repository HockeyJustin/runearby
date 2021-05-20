using CoreLookup.AllCountries;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitAndIntegrationTests.CoreLookup.UkSpecific
{
	public static class Stubs
	{
		public static List<LocationLatLng> GetPostcodeLookupData_AreaDistrict()
		{
			List<LocationLatLng> lookupdata = new List<LocationLatLng>();
			lookupdata.Add(new LocationLatLng("AB13", 57.108239, -2.23697)); // Aberdeen
			lookupdata.Add(new LocationLatLng("PO4", 50.790176, -1.062084)); // Portsmouth
			lookupdata.Add(new LocationLatLng("PO30", 50.691012, -1.313466)); // Isle of Wight
			lookupdata.Add(new LocationLatLng("SO14", 50.907201, -1.397348)); // Southampton (east side of) - (edge case inside)
			lookupdata.Add(new LocationLatLng("SO20", 51.116668, -1.51669)); // Southhampton (edge case outside)
			lookupdata.Add(new LocationLatLng("BN1", 50.842346, -0.139502)); // Brighton City Center - Out.

			lookupdata.Add(new LocationLatLng("BS1", 51.458457, -2.574165)); // Bristol (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("GL13", 51.692217, -2.45966));  // Gloucester, Eng (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("GL15", 51.760229, -2.472594)); // Gloucester, Wales (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("CF10", 51.479102, -3.178085)); // Cardiff (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("NP26", 51.609115, -2.849027)); // Newport (for Bristol Channel test)
			

			return lookupdata;
		}


		public static List<LocationLatLng> GetPostcodeLookupData_Full()
		{
																																								// *NP = Nonsense postcode. Not real data!
			List<LocationLatLng> lookupdata = new List<LocationLatLng>();
			lookupdata.Add(new LocationLatLng("AB13 1AA", 57.108239, -2.23697));  // *NP - Aberdeen
			lookupdata.Add(new LocationLatLng("PO4 1AA", 50.790176, -1.062084));  // *NP - Portsmouth
			lookupdata.Add(new LocationLatLng("PO30 1AA", 50.691012, -1.313466)); // *NP - Isle of Wight
			lookupdata.Add(new LocationLatLng("SO14 1AA", 50.907201, -1.397348)); // *NP - Southampton (east side of) - (edge case inside)
			lookupdata.Add(new LocationLatLng("SO20 1AA", 51.116668, -1.51669));  // *NP - Southhampton (edge case outside)
			lookupdata.Add(new LocationLatLng("BN1 1AA", 50.842346, -0.139502));  // *NP - Brighton City Center - Out.

			lookupdata.Add(new LocationLatLng("BS1 1AD", 51.458457, -2.574165)); // Bristol (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("GL139AA", 51.692217, -2.45966));  // Gloucester, Eng (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("GL154AA", 51.760229, -2.472594)); // Gloucester, Wales (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("CF101AA", 51.479102, -3.178085)); // Cardiff (for Bristol Channel test)
			lookupdata.Add(new LocationLatLng("NP263AA", 51.609115, -2.849027)); // Newport (for Bristol Channel test)

			return lookupdata;
		}



	}
}
