using CoreLookup.AllCountries;
using CoreLookup.Helpers;
using CoreLookup.UKSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoreLookup
{
	public interface IMain
	{
		Task Run();
	}


	public class Main : IMain
	{
		IOSMapConverter _oSMapConverter;
		IUkRadiusCheck _ukRadiusCheck;
		IUkNearestPostcodeToCoordinates _ukNearestPostcodeToCoordinates;
		IGlobalVendorRadiusCheck _globalVendorRadiusCheck;



		public Main(IOSMapConverter oSMapConverter,
			IUkRadiusCheck radiusCheck,
			IUkNearestPostcodeToCoordinates ukNearestPostcodeToCoordinates,
			IGlobalVendorRadiusCheck globalVendorRadiusCheck)
		{
			_oSMapConverter = oSMapConverter;
			_ukRadiusCheck = radiusCheck;
			_ukNearestPostcodeToCoordinates = ukNearestPostcodeToCoordinates;
			_globalVendorRadiusCheck = globalVendorRadiusCheck;
		}

		public async Task Run()
		{
			Console.WriteLine("RUN start");

			// GLOBALLY WORKABLE LAT/LONG + RADIUS DATA
			await GetGlobalVendorsForCustomer();



			// UK POSTCODE HANDLING
			// Convert OS data into useable lat/long data.
			// await _oSMapConverter.Convert_OS_CodePointOpen_Data_To_Usable_LatLong_Data();

			await RadiusCheckTest();
			await FindPostcodeFromCoordinates();

			await Task.Delay(3000);
			Console.WriteLine("RUN end");
		}


		public async Task GetGlobalVendorsForCustomer()
		{
			// NOTE: No radius here, because the vendors are coming to PETE (vendors set their own radius)
			LocationLatLng customerToVisit = new LocationLatLng("Hungry Pete (@B&Q Havant)", 50.866133, -1.012903);

			var vendors = await _globalVendorRadiusCheck.GetVendorsWhoCanVisitCustomer(customerToVisit);
			var vendorsNames = vendors.Select(_ => _.Identifier).ToList();
			var vendorNamesString = String.Join(",", vendorsNames);
			Console.WriteLine($"Vendors who can visit customer: {vendorNamesString}");

			// Pete wants to find vendors in an 'x' meter radius (he goes to them, so he sets the distance)
			LocationLatLng visitingCustomer = new LocationLatLng("Hungry Pete (@B&Q Havant)", 50.866133, -1.012903, 15000);
			var vendorsToVisit = await _globalVendorRadiusCheck.GetVendorsCustomerCanVisit(visitingCustomer);
			var vendorsNamesV = vendorsToVisit.Select(_ => _.Identifier).ToList();
			var vendorNamesVString = String.Join(",", vendorsNamesV);
			Console.WriteLine($"Vendors customer can visit: {vendorNamesVString}");

		}



		private async Task RadiusCheckTest()
		{
			await _ukRadiusCheck.Initalize();
			Console.WriteLine("Radius Check");

			var postcodesEngAvoidClippingWales = await _ukRadiusCheck.FindPostcodesInRadiusMiles("BS1 1AD", PostcodePartsType.Full, 15, false, true);
			Console.WriteLine("BS1 1AD (Bristol): Nearby postcodes (excluding Wales - NP) = " + String.Join(",", postcodesEngAvoidClippingWales));

			var postcodesWalesAvoidClippingEng= await _ukRadiusCheck.FindPostcodesInRadiusMiles("CF101AA", PostcodePartsType.Full, 20, false, true);
			Console.WriteLine("CF101AA (Cardiff): Nearby postcodes (excluding England - BS) = " + String.Join(",", postcodesWalesAvoidClippingEng));

			var sotnPostcodesInRadius = await _ukRadiusCheck.FindPostcodesInRadiusKm("SO140AA", PostcodePartsType.Full, 40, true, false);
			Console.WriteLine("SO140AA (Southampton): Nearby postcodes (excluding boat trips) = " + String.Join(",", sotnPostcodesInRadius));
			
			var postcodesInRadius = await _ukRadiusCheck.FindPostcodesInRadiusKm("PO4 8RA", PostcodePartsType.Full, 35, true, false);
			Console.WriteLine("PO4 8RA (Portsmouth): Nearby postcodes (excluding boat trips) = " + String.Join(",", postcodesInRadius));
			
			var iowPostcodesInRadius = await _ukRadiusCheck.FindPostcodesInRadiusKm("PO30 1UD", PostcodePartsType.Full, 35, true, false);
			Console.WriteLine("PO30 1UD (IoW): Nearby postcodes (excluding boat trips) = " + String.Join(",", iowPostcodesInRadius));

			Console.WriteLine("END Radius Check");
		}



		private async Task FindPostcodeFromCoordinates()
		{
			Console.WriteLine("Find postcode from coords");
			await _ukNearestPostcodeToCoordinates.Initalize();
			var closestPostcode = await _ukNearestPostcodeToCoordinates.GetClosestPostcodeToCoordinate(50.796435, -1.0648767);
			PostcodeParts postcodeParts = new PostcodeParts(closestPostcode.Identifier, PostcodePartsType.Full);
			Console.WriteLine($"Neaest postcode (expect PO4 8RA) = {closestPostcode.Identifier} ({postcodeParts.GetAreaAndDistrict_10K()})");
			Console.WriteLine("END Postcde match");
		}











		private void LittleLatLongTest()
		{
			// UK Parliament
			const double easting = 530284;
			const double northing = 179713;

			IGeoUKHelper geoUKHelper = new GeoUKHelper();

			var result = geoUKHelper.ConvertEastNorthToLatLong_HigherAccuracySlow(easting, northing);

			// NOTE: 17z is the zoom. Choose 19 or 20z for super close up
			var gmaps = string.Format("https://www.google.co.uk/maps/@{0},{1},17z", Math.Round(result.Latitude, 6), Math.Round(result.Longitude, 6));

		}

	}
}
