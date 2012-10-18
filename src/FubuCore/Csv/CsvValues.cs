using System;

namespace FubuCore.Csv
{
    public class CsvValues
    {
        public CsvValues(string values)
        {
            Values = Parse(values);
        }

        public string[] Values { get; set; } 

        public static string[] Parse(string input)
        {
            // TODO -- Support escape tokens
            return input .Split(new[] { "," }, StringSplitOptions.None);
        }
    }
}