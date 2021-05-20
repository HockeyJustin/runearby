using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreLookup.Helpers
{
  public interface ICsvParser
  {
    string[] ReadCsvFromFile(string fileName);

    void OutputCsvResults(string headerRow, string[] contentRows, string outputFilePath);
  }

  public class CsvParser : ICsvParser
  {
    public string[] ReadCsvFromFile(string fileName)
    {
      string[] returnRows = new string[0];

      try
      {
        string fileContents = "";

        // Get the contents from the file
        using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
          using (var sr = new StreamReader(fs))
          {
            fileContents = sr.ReadToEnd();
          }
        }

        if (String.IsNullOrWhiteSpace(fileContents))
          return returnRows;

        string[] fileContentLines = fileContents.Split(System.Environment.NewLine.ToCharArray());

        if (fileContentLines == null || !fileContentLines.Any())
          return returnRows;

        return fileContentLines;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }

      return returnRows;
    }

    public void OutputCsvResults(string headerRow, string[] contentRows, string outputFilePath)
    {
      if (File.Exists(outputFilePath))
      {
        File.Delete(outputFilePath);
      }

      var csv = new StringBuilder();
      if (!String.IsNullOrWhiteSpace(headerRow))
      {
        csv.AppendLine(headerRow); // @"""Key"",""Result"""
      }

      foreach (var row in contentRows)
      {
        csv.AppendLine().Append(row);
      }

      File.WriteAllText(outputFilePath, csv.ToString());
    }

  }
}
