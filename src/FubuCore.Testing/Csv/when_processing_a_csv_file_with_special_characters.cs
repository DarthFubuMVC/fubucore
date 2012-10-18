namespace FubuCore.Testing.Csv
{
    public class when_processing_a_csv_file_with_a_comma : SpecialCharacterHarness
    {
        protected override string getActualContent()
        {
            return "comma , handled";
        }
    }

    public class when_processing_a_csv_file_with_a_new_line : SpecialCharacterHarness
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

    public class when_processing_a_csv_file_with_a_double_quote : SpecialCharacterHarness
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

    // Not really sure on this scenario
    public class when_processing_a_csv_file_with_missing_matching_double_quote : SpecialCharacterHarness
    {
        protected override string getActualContent()
        {
            return " \"escape not finished";
        }

        protected override string getExpectedContent()
        {
            return " escape not finished";
        }
    }
}