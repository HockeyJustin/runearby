using CoreLookup.UKSpecific;
using Geolocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreLookup.AllCountries
{
	public interface IGlobalVendorRadiusCheck
	{
		/// <summary>
		/// Here, vendors have stated the distance they are willing to travel
		/// This will show a customer all vendors who are willing to travel to them.
		/// </summary>
		/// <param name="customer">A person with location data</param>
		/// <returns></returns>
		Task<List<LocationLatLng>> GetVendorsWhoCanVisitCustomer(LocationLatLng customer);

		/// <summary>
		/// A customer has stated how far they are willing to travel. 
		/// Given that, which vendors could they visit.
		/// </summary>
		/// <param name="customerWithRadius">A person with location AND RADIUS data</param>
		/// <returns></returns>
		Task<List<LocationLatLng>> GetVendorsCustomerCanVisit(LocationLatLng customerWithRadius);

		void Test();
	}



	public class GlobalVendorRadiusCheck : IGlobalVendorRadiusCheck
	{
		IVendorData _vendorData;

		public GlobalVendorRadiusCheck(IVendorData vendorData)
		{
			_vendorData = vendorData;
		}


		public async Task<List<LocationLatLng>> GetVendorsWhoCanVisitCustomer(LocationLatLng customer)
		{
			var customerCoordinates = new Coordinate(customer.Latitude, customer.Longitude);

			List<LocationLatLng> vendorsWhoMatch = new List<LocationLatLng>();

			var vendors = await _vendorData.GetVendors();

			foreach (var vendor in vendors)
			{
				var vendorLocation = new Coordinate(vendor.Latitude, vendor.Longitude);

				double distance = GeoCalculator.GetDistance(customerCoordinates, vendorLocation, 0, DistanceUnit.Meters);

				if (distance < vendor.RadiusInMeters)
				{
					vendorsWhoMatch.Add(vendor);
				}
			}

			return vendorsWhoMatch;
		}



		public async Task<List<LocationLatLng>> GetVendorsCustomerCanVisit(LocationLatLng customerWithRadius)
		{
			var customerCoordinates = new Coordinate(customerWithRadius.Latitude, customerWithRadius.Longitude);

			List<LocationLatLng> vendorsWhoMatch = new List<LocationLatLng>();

			var vendors = await _vendorData.GetVendors();

			foreach (var vendor in vendors)
			{
				var vendorLocation = new Coordinate(vendor.Latitude, vendor.Longitude);

				double distance = GeoCalculator.GetDistance(customerCoordinates, vendorLocation, 0, DistanceUnit.Meters);

				if (distance < customerWithRadius.RadiusInMeters)
				{
					vendorsWhoMatch.Add(vendor);
				}
			}

			return vendorsWhoMatch;
		}







		/// <summary>
		/// Uses https://www.nuget.org/packages/Geolocation/
		/// </summary>
		public void Test()
		{

			var landsEndLocation = new Coordinate(50.0775475, -5.6352355);
			var johnOGroatsLocation = new Coordinate(58.6366688, -3.0827024);

			double distance = GeoCalculator.GetDistance(landsEndLocation, johnOGroatsLocation, 1, DistanceUnit.Kilometers);

		}






	}

}
