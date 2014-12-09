using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace SongService
{
    /// <summary>
    /// Helper object used for XML searilization.
    /// </summary>
    public class Artist
    {
        [XmlAttribute("name")]
        public String Name
        {
            get;
            set;
        }

        [XmlElement("album")]
        public List<Album> Albums
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Helper object used for XML searilization.
    /// </summary>
    [XmlRoot("music")]
    public class Music
    {
        [XmlElement("artist")]
        public List<Artist> Artists
        {
            get;
            set;
        }
    }
}
