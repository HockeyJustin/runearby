using CoreLookup.AllCountries;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitAndIntegrationTests.CoreLookup.AllCountries
{
	public class GlobalVendorRadiusCheckTests
	{
		GlobalVendorRadiusCheck _cut;

		public GlobalVendorRadiusCheckTests()
		{
			List<LocationLatLng> lookupdata = Stubs.GetFakeVendors_WithRadiiTheyWillVisit();

			var iVendorDataMock = new Mock<IVendorData>();
			iVendorDataMock.Setup(_ =>
				_.GetVendors()
			).ReturnsAsync(lookupdata);

			_cut = new GlobalVendorRadiusCheck(iVendorDataMock.Object);
		}

		[Fact]
		public async void GetVendorsWhoCanVisitCustomer_FromHavant_Expect_PortsmouthEastSotonDave()
		{
			// Arrange
			// NOTE: No radius here, because the vendors are coming to the customer (vendors set their own radius)
			LocationLatLng customerToVisit = new LocationLatLng("Havant Customer", 50.866133, -1.012903);

			// Act
			var vendors = await _cut.GetVendorsWhoCanVisitCustomer(customerToVisit);
			var vendorsNames = vendors.Select(_ => _.Identifier).ToList();

			Assert.Equal(2, vendorsNames.Count);
			Assert.Contains("Portsmouth Vendor", vendorsNames);
			Assert.Contains("East Soton Vendor", vendorsNames);
		}



		[Fact]
		public async void GetVendorsCustomerCanVisit_FromHavantWith15kRadius_Expect_PortsmouthOnly()
		{
			// Arrange
			// NOTE: No radius here, because the vendors are coming to the customer (vendors set their own radius)
			LocationLatLng visitingCustomer = new LocationLatLng("Havant Customer", 50.866133, -1.012903, 15000);

			// Act
			var vendorsToVisit = await _cut.GetVendorsCustomerCanVisit(visitingCustomer);
			var vendorsNames = vendorsToVisit.Select(_ => _.Identifier).ToList();

			Assert.Equal(1, vendorsNames.Count);
			Assert.Contains("Portsmouth Vendor", vendorsNames);
		}





	}
}
