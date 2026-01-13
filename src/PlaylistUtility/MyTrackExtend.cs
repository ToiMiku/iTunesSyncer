using iTunesSyncer.PlaylistUtility;
using System;
using System.Linq;

namespace iTunesSyncer.PlaylistUtility
{
    static class MyTrackExtend
    {
        public const string ApplicationName = "http://www.iTunesSyncer.co.jp/playlist/0/";
        public const string TagName = "its";
        public const string NameSpace = "http://www.iTunesSyncer.co.jp/playlist/ns/0/";

        public const string FileHashOptionName = "filehash";
        public const string RelativePathOptionName = "relativepath";

        public static void SetRelativePath(this Track track, string relativePath)
            => SetStringOption(track, RelativePathOptionName, relativePath);
        public static string GetRelativePath(this Track track)
            => GetStringOption(track, RelativePathOptionName);

        public static void SetFileHash(this Track track, string hash)
            => SetStringOption(track, FileHashOptionName, hash);
        public static string GetFileHash(this Track track)
            => GetStringOption(track, FileHashOptionName);

        private static void SetStringOption(Track track, string optionName, string hash)
        {
            var trackOption = FindTrackOption(track, optionName);
            if (trackOption is null)
            {
                trackOption = new TrackOption()
                {
                    TagName = TagName,
                    NameSpace = NameSpace,
                    OptionName = optionName,
                };
                track.Options.Add(trackOption);
            }

            trackOption.Value = hash;
        }

        private static string GetStringOption(Track track, string optionName)
        {
            var trackOption = FindTrackOption(track, optionName);
            if (trackOption is null)
                return String.Empty;

            return trackOption.Value;
        }

        private static TrackOption FindTrackOption(Track track, string optionName)
        {
            var trackOptions = track.Options.Where(x => x.OptionName == optionName);

            var trackOptionsCount = trackOptions.Count();
            if (trackOptionsCount == 0)
            {
                return null;
            }
            else if (trackOptionsCount == 1)
            {
                return trackOptions.First();
            }
            else
            {
                throw new Exception("2個以上あるのはおかしい");
            }
        }
    }
}
