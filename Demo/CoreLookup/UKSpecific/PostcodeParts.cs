using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CoreLookup.UKSpecific
{
	public class PostcodeParts
	{
		// https://www.mrs.org.uk/pdf/postcodeformat.pdf
		// Outward code   Inward code
		// Area District  Sector Unit
		//  SW     1W       0     NY

		// AREA: PO the area.There are 124 postcode areas in the UK.
		// DISTRICT: There are approximately 20 Postcode districts in an area.
		// SECTOR: There are approximately 3000 addresses in a sector. 
		// UNIT: There are approximately 15 addresses per unit.

		// https://www.bph-postcodes.co.uk/guidetopc.cgi
		// Areas 						124				(approx 250K addresses)
		// District 				2,982			(approx 10K addresses)
		// Sectors 					11,204		(aprox 2700 addresses)
		// Postcodes 				1,765,422 (approx 15 addresses per unit)
		// Delivery Points 	30,688,800

		// FORMAT EXAMPLE
		// AN NAA				M1 1AA
		// ANN NAA			M60 1NW
		// AAN NAA			CR2 6XH
		// AANN NAA			DN55 1PT
		// ANA NAA			W1A 1HQ
		// AANA NAA			EC1A 1BB

		public PostcodeParts()
		{

		}

		/// <summary>
		/// WARNING: This relies on a format as provided by the OS Open Data.
		///					 There is no validation for poorly formatted postcodes
		/// </summary>
		/// <param name="postcode"></param>
		public PostcodeParts(string postcode, PostcodePartsType postcodePartsType)
		{
			if(postcodePartsType == PostcodePartsType.Full)
			{
				FillFromFullPostcode(postcode.ToUpper());
			}
			else if (postcodePartsType == PostcodePartsType.AreaAndDistrict)
			{
				FillAreaAndDistrict(postcode.ToUpper());
			}
			else
			{
				throw new NotImplementedException("Cannot handle this postcodeparttype.");
			}

			
		}

		public string Area { get; set; }

		public string District { get; set; }

		public string Sector { get; set; }

		public string Unit { get; set; }


		public void FillFromFullPostcode(string postcode)
		{
			if(String.IsNullOrWhiteSpace(postcode) || postcode.Length < 5)
			{
				throw new InvalidOperationException("Full postcode required for split to component parts");
			}

			string postcodeEnd = postcode.Substring(postcode.Length - 3, 3);
			string postcodeStart = postcode.Replace(postcodeEnd, "").Replace(" ", "");

			FillAreaAndDistrict(postcodeStart);
			FillSectorAndUnit(postcodeEnd);
		}


		public void FillAreaAndDistrict(string postcodeAreaAndDistrict)
		{
			string areaBuilder = "";
			foreach (char c in postcodeAreaAndDistrict)
			{
				if (Char.IsDigit(c))
				{
					break;
				}
				else
				{
					areaBuilder += c;
				}
			}

			Area = areaBuilder;
			District = postcodeAreaAndDistrict.Replace(Area, "");
		}


		public void FillSectorAndUnit(string postcodeSectorAndUnit)
		{
			string sectorBuilder = "";
			foreach (char c in postcodeSectorAndUnit)
			{
				if (!Char.IsDigit(c))
				{
					break;
				}
				else
				{
					sectorBuilder += c;
				}
			}

			Sector = sectorBuilder;
			Unit = postcodeSectorAndUnit.Replace(Sector, "");
		}


		/// <summary>
		/// The Area e.g. PO . This is around 250K addresses.
		/// </summary>
		/// <returns></returns>
		public string GetArea_250K()
		{
			return Area;
		}

		/// <summary>
		/// The Area and District e.g. PO4.  This is around 10,000 addresses.
		/// </summary>
		/// <returns></returns>
		public string GetAreaAndDistrict_10K()
		{
			return Area + District;
		}

		/// <summary>
		/// Area district and Sector e.g. PO4 8. This is around 2,700 addresses
		/// </summary>
		/// <returns></returns>
		public string GetAreaDistrictAndSector_2700()
		{
			return Area + District + Sector;
		}

		/// <summary>
		/// Get the full postcode e.g. PO4 8RA. Varies, but averages 15 addresses. 
		/// </summary>
		/// <returns></returns>
		public string GetFullPostcode_15()
		{
			if(District.Length > 1)
			{
				return Area + District + Sector + Unit;
			}
			else
			{
				return Area + District + " " + Sector + Unit;
			}
		}
	}
}
