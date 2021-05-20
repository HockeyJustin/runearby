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
	public class UkRadiusCheckTests
	{

		UkRadiusCheck _cut;

		public UkRadiusCheckTests()
		{
		}

		public async Task InitializeTest()
		{
			List<LocationLatLng> lookupdata = Stubs.GetPostcodeLookupData_AreaDistrict();

			var iProcessedUkDataMock = new Mock<IProcessedUkData>();
			iProcessedUkDataMock.Setup(_ =>
				_.GetData(It.IsAny<PostcodePartsType>())
			).ReturnsAsync(lookupdata);

			_cut = new UkRadiusCheck(iProcessedUkDataMock.Object);

			await _cut.Initalize();
		}



		[Fact]
		public async void FindPostcodesInRadiusMiles_WithFullPostcode_ExcBoatTrip_Expect_Portsmouth_Southampton()
		{
			// Arrrange
			var startingPostcode = "PO4 8RA"; // Portsmouth
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 30, true, false);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains("PO4", result);
			Assert.Contains("SO14", result);
		}


		[Fact]
		public async void FindPostcodesInRadiusMiles_WithPartPostcode_ExcBoatTrip_Expect_Portsmouth_Southampton()
		{
			// Arrrange
			var startingPostcode = "PO4"; // Portsmouth
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.AreaAndDistrict, 30, true, false);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains("PO4", result);
			Assert.Contains("SO14", result);
		}


		[Fact]
		public async void FindPostcodesInRadiusMiles_WithFullPostcode_IncludingBoatTrip_Expect_IoWIncluded()
		{
			// Arrrange
			var startingPostcode = "PO4 8RA"; // Portsmouth
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 30, false, false);

			// Assert
			Assert.Equal(3, result.Count);
			Assert.Contains("PO30", result);
		}



		[Fact]
		public async void FindPostcodesInRadiusMiles_WithIoWStart_ExcBoatTrip_Expect_IoWOnly()
		{
			// Arrrange
			var startingPostcode = "PO30 1UD"; // IoW
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 30, true, false);

			// Assert
			Assert.Equal(1, result.Count);
			Assert.Contains("PO30", result);
		}



		[Fact]
		public async void FindPostcodesInRadiusMiles_WithIoWStart_IncBoatTrip_Expect_IoW_Portsmouth()
		{
			// Arrrange
			var startingPostcode = "PO30 1UD"; // IoW
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 15, false, false);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Contains("PO30", result);
			Assert.Contains("PO4", result);
		}



		[Fact]
		public async void FindPostcodesInRadiusMiles_FromBristol_IncBristolChannel_Expect_BristolAndCardiff()
		{
			// Arrrange
			var startingPostcode = "BS1 1AD"; // Bristol
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 40, false, false);

			// Assert
			Assert.Contains("BS1", result);
			Assert.Contains("GL13", result);
			Assert.Contains("GL15", result);
			Assert.Contains("CF10", result);
			Assert.Contains("NP26", result);
		}


		[Fact]
		public async void FindPostcodesInRadiusMiles_FromCardiff_IncBristolChannel_Expect_BristolAndCardiff()
		{
			// Arrrange
			var startingPostcode = "CF101AA"; // Cardiff
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 40, false, false);

			// Assert
			Assert.Contains("BS1", result);
			Assert.Contains("GL13", result);
			Assert.Contains("GL15", result);
			Assert.Contains("CF10", result);
			Assert.Contains("NP26", result);
		}



		[Fact]
		public async void FindPostcodesInRadiusMiles_FromBristol_ExBristolChannel_Expect_NotCardiffNewPort()
		{
			// Arrrange
			var startingPostcode = "BS1 1AD"; // Bristol
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 40, false, true);

			// Assert
			Assert.Contains("BS1", result);
			Assert.Contains("GL13", result);
			Assert.DoesNotContain("GL15", result);
			Assert.DoesNotContain("CF10", result);
			Assert.DoesNotContain("NP26", result);
		}

		[Fact]
		public async void FindPostcodesInRadiusMiles_FromCardiff_ExBristolChannel_Expect_NotBristolAndGlouc()
		{
			// Arrrange
			var startingPostcode = "CF101AA"; // Cardiff
			await InitializeTest();

			// Act
			var result = await _cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 40, false, true);

			// Assert
			Assert.DoesNotContain("BS1", result);
			Assert.DoesNotContain("GL13", result);
			Assert.Contains("GL15", result);
			Assert.Contains("CF10", result);
			Assert.Contains("NP26", result);
		}












		[Fact]
		public async void FindPostcodesInRadiusMiles_WithUnknownPostcode_ExpectError()
		{
			// Arrrange
			var startingPostcode = "PO99 9AA";
			await InitializeTest();

			// Act
			var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => 
			_cut.FindPostcodesInRadiusMiles(startingPostcode, PostcodePartsType.Full, 100, false, false)
			);
			Assert.Equal("Could not find location for postcode PO99 9AA -> PO99", ex.Message);
		}




	}
}
