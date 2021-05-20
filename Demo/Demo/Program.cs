using CoreLookup;
using CoreLookup.AllCountries;
using CoreLookup.Helpers;
using CoreLookup.UKSpecific;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Demo
{
	class Program
	{
    /// <summary>
    /// DI set up in line with 
    /// https://medium.com/swlh/how-to-take-advantage-of-dependency-injection-in-net-core-2-2-console-applications-274e50a6c350
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static async Task Main(string[] args)
    {

      Console.WriteLine("Starting");

      // Create service collection and configure our services
      var services = ConfigureServices();    // Generate a provider
      var serviceProvider = services.BuildServiceProvider();

      // Kick off our actual code
      await serviceProvider.GetService<ConsoleApplication>().Run();

      Console.WriteLine("Ending");
    }

    public class ConsoleApplication
    {
      private readonly IMain _coreMain; 
      public ConsoleApplication(IMain coreMain)
      {
        _coreMain = coreMain;
      }

      // Application starting point
      public async Task Run()
      {
        await _coreMain.Run();
      }
    }


    private static IServiceCollection ConfigureServices()
    {
      IServiceCollection services = new ServiceCollection();
      services.AddTransient<IFileHelper, FileHelper>();
      services.AddTransient<ICsvParser, CsvParser>();

      services.AddTransient<IVendorData, VendorDummyData>();
      services.AddTransient<IGlobalVendorRadiusCheck, GlobalVendorRadiusCheck>();
      services.AddTransient<ILocationLatLngMethods, LocationLatLngMethods>();

      services.AddTransient<IGeoUKHelper, GeoUKHelper>();
      services.AddTransient<IConvertEastingsNorthingsToLatLongUK, ConvertENtoLatLongUk>();
      services.AddTransient<IConvertEastingsNorthingsToLatLongUK, ConvertENtoLatLongUk>();
      services.AddTransient<IGeneraliseUkPostcodes, GeneraliseUkPostcodes>();
      services.AddTransient<IOSMapConverter, OSMapConverter>();
      services.AddTransient<IProcessedUkData, ProcessedUkData>();
      services.AddSingleton<IUkRadiusCheck, UkRadiusCheck>(); 
      services.AddSingleton<IUkNearestPostcodeToCoordinates, UkNearestPostcodeToCoordinates>();

      
      services.AddTransient<IMain, CoreLookup.Main>();
      
      services.AddTransient<ConsoleApplication>(); // IMPORTANT! Register our application entry point
      return services;
    }




  }
}
