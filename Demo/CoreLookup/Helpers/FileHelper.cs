using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoreLookup.Helpers
{
	public interface IFileHelper
	{
		void MergeTextFiles(string directoryOfCsvs, string outputFileName);
	}

	public class FileHelper : IFileHelper
	{
		public void MergeTextFiles(string directoryOfCsvs, string outputFileName)
		{

			var filesToProcess = Directory.GetFiles(directoryOfCsvs); // NOT RECURSIVE

			StringBuilder sb = new StringBuilder();

			int counter = 0;
			foreach (var file in filesToProcess)
			{
				Console.WriteLine(++counter);
				var contents = File.ReadAllText(file);
				sb.Append(contents);
			}

			var outputDirectory = Path.Combine(directoryOfCsvs, "Merged");
			if (!Directory.Exists(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}

			var outputFileFullPath = Path.Combine(outputDirectory, outputFileName);

			File.WriteAllText(outputFileFullPath, sb.ToString());


		}

	}
}
