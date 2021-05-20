using CoreLookup.AllCountries;
using CoreLookup.UKSpecific;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitAndIntegrationTests.CoreLookup.UkSpecific
{
	public class UkNearestPostcodeToCoordinatesTests
	{
		IUkNearestPostcodeToCoordinates _cut;

		public UkNearestPostcodeToCoordinatesTests()
		{
		}

		public async Task InitializeTest()
		{
			List<LocationLatLng> lookupdata = Stubs.GetPostcodeLookupData_Full();

			var iProcessedUkDataMock = new Mock<IProcessedUkData>();
			iProcessedUkDataMock.Setup(_ =>
				_.GetData(It.IsAny<PostcodePartsType>())
			).ReturnsAsync(lookupdata);

			_cut = new UkNearestPostcodeToCoordinates(iProcessedUkDataMock.Object);

			await _cut.Initalize();
		}


		[Fact]
		public async void GetClosestPostcodeToCoordinate_NearP04_Expect_PO4()
		{
			// Arrange
			var expected = "PO4 1AA";
			await InitializeTest();

			// Act
			var result = await _cut.GetClosestPostcodeToCoordinate(50.796435, -1.0648767);

			// Assert
			Assert.Equal(expected, result.Identifier);
		}


	}
}
