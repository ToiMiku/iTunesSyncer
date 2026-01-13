using iTunesSyncer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace iTunesSyncer.PlaylistUtility
{
    class XMLPlaylistIO : IPlaylistIO
    {
        private readonly XNamespace XspfNameSpace = "http://xspf.org/ns/0/";

        public string ImportFilePattern => "*.xspf";

        public string ExportFileExtension => ".xspf";

        public XMLPlaylistIO()
        {
        }


        public Playlist Load(Stream stream)
        {
            var xml = XDocument.Load(stream);

            var playlist = LoadPlaylistElement(xml);

            return playlist;
        }

        private Playlist LoadPlaylistElement(XDocument xml)
        {
            var playlistElement = xml.GetElement(XspfNameSpace + "playlist");

            var location = playlistElement.GetElement(XspfNameSpace + "location").Value;
            location = location.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            location = PathExtension.UriDecode(location);

            var playlist = new Playlist()
            {
                Title = playlistElement.GetElement(XspfNameSpace + "title").Value,
                Location = location,
            };

            var trackListElement = playlistElement.GetElement(XspfNameSpace + "trackList");
            foreach (var trackElement in trackListElement.GetElements(XspfNameSpace + "track"))
            {
                var track = LoadTrackElement(trackElement);
                playlist.Tracks.Add(track);
            }

            return playlist;
        }

        private Track LoadTrackElement(XElement trackElement)
        {
            var location = trackElement.GetElement(XspfNameSpace + "location").Value;
            location = location.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            location = PathExtension.UriDecode(location);

            var track = new Track()
            {
                Title = trackElement.GetElement(XspfNameSpace + "title").Value,
                Location = location,
            };

            foreach (var extensionElement in trackElement.GetElements(XspfNameSpace + "extension"))
            {
                var applicationName = extensionElement.Attribute("application").Value;

                if (applicationName != MyTrackExtend.ApplicationName)
                    throw new Exception("TrackのApplicationの名前がおかしい");

                foreach (var trackOptionElement in extensionElement.GetElements())
                {
                    if (trackOptionElement.Name.Namespace != MyTrackExtend.NameSpace)
                        throw new Exception("TrackOptionのNameSpaceの名前がおかしい");

                    var trackOption = new TrackOption()
                    {
                        TagName = MyTrackExtend.TagName,
                        NameSpace = MyTrackExtend.NameSpace,
                        OptionName = trackOptionElement.Name.LocalName,
                        Value = trackOptionElement.Value,
                    };
                    track.Options.Add(trackOption);
                }
            }

            return track;
        }

        public void Save(Stream stream, Playlist playlist)
        {
            // ヘッダーを作成
            var xml = new XDocument
            {
                Declaration = new XDeclaration("1.0", "utf-8", "true")
            };

            // playlistを作成
            var playlistElement = CreatePlaylistElement(playlist);
            xml.Add(playlistElement);

            // 保存
            xml.Save(stream);
        }

        private XElement CreatePlaylistElement(Playlist playlist)
        {
            // playlistを作成
            var playlistElement = new XElement(XspfNameSpace + "playlist");
            SetPlaylistElementAttribute(playlistElement, playlist.Tracks);
            playlistElement.Add(new XElement(XspfNameSpace + "title") { Value = playlist.Title });
            playlistElement.Add(new XElement(XspfNameSpace + "location") { Value = PathExtension.UriEncode(playlist.Location) });

            // trackListを作成
            var trackListElement = new XElement(XspfNameSpace + "trackList");
            playlistElement.Add(trackListElement);

            // trackを作成
            foreach (var track in playlist.Tracks)
            {
                var trackElement = CreateTrackElement(track);
                trackListElement.Add(trackElement);
            }

            return playlistElement;
        }

        private void SetPlaylistElementAttribute(XElement playlistElement, IEnumerable<Track> tracks)
        {
            playlistElement.SetAttributeValue("version", 1);
            playlistElement.SetAttributeValue("xmlns", XspfNameSpace);

            playlistElement.SetAttributeValue(XNamespace.Xmlns + MyTrackExtend.TagName, MyTrackExtend.NameSpace);
        }

        private XElement CreateTrackElement(Track track)
        {
            // trackを作成
            var trackElement = new XElement(XspfNameSpace + "track");
            trackElement.Add(new XElement(XspfNameSpace + "title") { Value = track.Title });
            trackElement.Add(new XElement(XspfNameSpace + "location") { Value = PathExtension.UriEncode(track.Location) });

            // extensionElementを作成
            var extensionElement = new XElement(XspfNameSpace + "extension");
            extensionElement.SetAttributeValue("application", MyTrackExtend.ApplicationName);
            trackElement.Add(extensionElement);

            XNamespace myNameSpace = MyTrackExtend.NameSpace;
            foreach (var option in track.Options)
            {
                var optionElement = new XElement(myNameSpace + option.OptionName)
                {
                    Value = option.Value
                };
                extensionElement.Add(optionElement);
            }

            return trackElement;
        }
    }
}
