using System;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using FubuLocalization.Basic;

namespace localizer
{
    public class FolderInput
    {
        [Description("The folder where localization files will be found")]
        public string Folder { get; set; }

        public XmlDirectoryLocalizationStorage GetStorage()
        {
            return new XmlDirectoryLocalizationStorage(new[]{Folder});
        }
    }

    [CommandDescription("Merges any and all 'missing' localization data in the designated folder")]
    public class MergeCommand : FubuCommand<FolderInput>
    {
        public override bool Execute(FolderInput input)
        {
            var storage = input.GetStorage();

            Console.WriteLine("Reading missing localization key/values from " + storage.MissingLocaleFile);

            storage.MergeAllMissing();

            return true;
        }
    }

    [CommandDescription("Opens the 'missing' localization file in a text editor", Name = "open-missing")]
    public class OpenMissingCommand : FubuCommand<FolderInput>
    {
        public override bool Execute(FolderInput input)
        {
            var missingLocaleFile = input.GetStorage().MissingLocaleFile;

            if (File.Exists(missingLocaleFile))
            {
                Console.WriteLine("Opening " + missingLocaleFile);
                new FileSystem().LaunchEditor(missingLocaleFile);
            }
            else
            {
                Console.WriteLine(missingLocaleFile + " does not exist");
            }

            return true;
        }
    }

    [CommandDescription("Asserts that there is no unmerged localization data in the designated folder",
        Name = "assert-no-missing")]
    public class AssertNoMissingCommand : FubuCommand<FolderInput>
    {
        public override bool Execute(FolderInput input)
        {
            var storage = input.GetStorage();
            if (storage.HasMissingLocalizationKeys())
            {
                throw new ApplicationException("Detected unmerged localization keys at " + storage.MissingLocaleFile);
            }

            Console.WriteLine("No missing localization data");
            return true;
        }
    }
}