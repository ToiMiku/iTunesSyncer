using System;
using System.Collections.Generic;

namespace iTunesSyncer.PlaylistUtility
{
    public class Track
    {
        public string Title { get; set; }
        public string Location { get; set; }

        public List<TrackOption> Options { get; set; }

        public Track()
        {
            Title = string.Empty;
            Location = string.Empty;
            Options = new List<TrackOption>();
        }

        public Track(Track originTrack)
        {
            Title = originTrack.Title;
            Location = originTrack.Location;
            Options = new List<TrackOption>(originTrack.Options.Count);

            foreach (var option in originTrack.Options)
            {
                Options.Add(new TrackOption(option));
            }
        }
    }
}
