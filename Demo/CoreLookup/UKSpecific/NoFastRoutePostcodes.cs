using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLookup.UKSpecific
{
	/// <summary>
	/// Ref for islands - https://en.wikipedia.org/wiki/List_of_islands_of_the_British_Isles. Have included major islands that don't have roads to them.
	/// </summary>
	public class NoFastRoutePostcodes
	{
		public string OneSideDescription { get; set; }
		public string[] OneSide { get; set; }

		public string OtherSideDescription { get; set; }
		public string[] OtherSide { get; set; }

		public bool IsSplitSamePostcodeArea { get; set; }



		public static List<string> GetIslands_WithOwnAreaPostcodes()
		{
			List<string> isles = new List<string>();
			isles.Add("IM"); // Isle of Man
			isles.Add("JE"); // Jersey
			isles.Add("GY"); // Guernsey
			isles.Add("HS"); // Hebrides (e.g. Lewis, Harris etc)
			isles.Add("ZE"); // Shetlands
			return isles;
		}


		/// <summary>
		/// These are postcode that will require a boat or plane.
		/// </summary>
		/// <returns></returns>
		public static List<string> GetIslands_WithAreaDistrictPostcodes()
		{
			var isleOfWight = new string[] { "PO30", "PO31", "PO32", "PO33", "PO34", "PO35", "PO36", "PO37", "PO38", "PO39", "PO40", "PO41" };
			var isleOfMan = new string[] { "IM1", "IM2", "IM3", "IM4", "IM5", "IM6", "IM7", "IM8", "IM9", "IM86", "IM87", "IM99", };
			var anglesey = new string[] { "LL58", "LL59", "LL60", "LL61", "LL62", "LL63", "LL64", "LL65", "LL66", "LL67", "LL68", "LL69", "LL70", "LL71", "LL72", "LL73", "LL74", "LL75", "LL76", "LL77", "LL77", "LL78" };
			var scilly = new string[] { "TR21", "TR22", "TR23", "TR24", "TR25" };
			var orkneys = new string[] { "KW15", "KW16", "KW17" };
			var rumEigg = new string[] { "PH41", "PH42", "PH43", "PH44" };
			var mullIslay = new string[] { "PA41", "PA42", "PA43", "PA44", "PA45", "PA46", "PA47", "PA48", "PA49", "PA60", "PA61", "PA62", "PA63", "PA64", "PA65", "PA66", "PA67", "PA68", "PA69", "PA70", "PA71", "PA72", "PA73", "PA74", "PA75", "PA76", "PA77", "PA78", "PA80" };
			//EX39 is split across water! Ignore for now.

			List<string> isles = new List<string>();
			isles.AddRange(isleOfWight);
			isles.AddRange(anglesey);
			isles.AddRange(isleOfMan);
			isles.AddRange(scilly);
			isles.AddRange(orkneys);
			isles.AddRange(rumEigg);
			isles.AddRange(mullIslay);
			return isles;
		}




		/// <summary>
		/// These are cross-land trips that can't take a 'crow flies' approach due
		/// to lack of bridges (e.g. across the bristol channel)
		/// </summary>
		/// <returns></returns>
		public static NoFastRoutePostcodes GetEngWalesPostcodes_Areas_SplitByWater()
		{
			// Bristol Channel / River severn
			NoFastRoutePostcodes bristolChannel = new NoFastRoutePostcodes();
			bristolChannel.OneSideDescription = "England";
			bristolChannel.OneSide = new string[] { "BS", "TA", "EX", "PL", "TR" }; // WARNING: GL is actually split into two, spanning both sides of the Severn, so not included here.
			bristolChannel.OtherSideDescription = "Wales";
			bristolChannel.OtherSide = new string[] { "NP", "CF", "SA", };

			return bristolChannel;
		}


		public static NoFastRoutePostcodes GetThamesEstuary_Areas_SplitByWater()
		{
			NoFastRoutePostcodes farEastThamesNoBridge = new NoFastRoutePostcodes();
			farEastThamesNoBridge.OneSideDescription = "North-Southend-on-Sea/Chelmsford";
			farEastThamesNoBridge.OneSide = new string[] { "SS", "CM" };
			farEastThamesNoBridge.OtherSideDescription = "South-Canterbury";
			farEastThamesNoBridge.OtherSide = new string[] { "CT" };
			return farEastThamesNoBridge;
		}


		public static NoFastRoutePostcodes GetGloucester_AreaDistrics_SplitByWater()
		{
			// Gloucester (split by )
			NoFastRoutePostcodes gloucester = new NoFastRoutePostcodes();
			gloucester.IsSplitSamePostcodeArea = true;
			gloucester.OneSideDescription = "England";
			gloucester.OneSide = new string[] { "GL1", "GL2", "GL3", "GL4", "GL5", "GL6", "GL7", "GL8", "GL9", "GL10", "GL11", "GL12", "GL13", "GL19", "GL20", "GL50", "GL51", "GL52", "GL53", "GL54", "GL55", "GL56" };
			gloucester.OtherSideDescription = "Wales";
			gloucester.OtherSide = new string[] { "GL15", "GL14", "GL16", "GL17", "GL18" };

			return gloucester;
		}

		


		// River Humber - Assume can navigate around the river Humber OK. Have checked journey times.

		// Isle Of Man - Assume far enough away it wouldn't end up in a radius check.

		// Scotland (e.g. Hebrides). Assume most are far enough away, they wouldn't end up in a radius check.

		// Thames estuary

	}

}
