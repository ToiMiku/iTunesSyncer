
namespace iTunesSyncer.PlaylistUtility
{
    public class TrackOption
    {
        public string TagName { get; set; }
        public string NameSpace { get; set; }

        public string OptionName { get; set; }
        public string Value { get; set; }

        public TrackOption()
        {
            TagName = string.Empty;
            NameSpace = string.Empty;
            OptionName = string.Empty;
            Value = string.Empty;
        }

        public TrackOption(TrackOption originOption)
        {
            TagName = originOption.TagName;
            NameSpace = originOption.NameSpace;
            OptionName = originOption.OptionName;
            Value = originOption.Value;
        }
    }
}
