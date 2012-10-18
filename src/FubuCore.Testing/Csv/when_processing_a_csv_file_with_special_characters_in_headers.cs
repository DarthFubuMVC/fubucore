namespace FubuCore.Testing.Csv
{
    public class when_processing_a_csv_file_with_a_comma_in_headers : SpecialCharacterHeaderHarness
    {
        protected override string getActualContent()
        {
            return "comma , handled";
        }
    }

    public class when_processing_a_csv_file_with_a_new_line_in_headers : SpecialCharacterHeaderHarness
    {
        protected override string getActualContent()
        {
            // On a Windows Environment this could return \r\n but will get evaluated as \n when read
            return @"new line
, handled";
        }

        protected override string getExpectedContent()
        {
            return "new line\n, handled";
        }
    }

    public class when_processing_a_csv_file_with_a_double_quote_in_headers : SpecialCharacterHeaderHarness
    {
        protected override string getActualContent()
        {
            return "double quote \"\" handled";
        }

        protected override string getExpectedContent()
        {
            return "double quote \" handled";
        }
    }
}