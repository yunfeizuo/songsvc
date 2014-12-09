using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SongService
{

    /// <summary>
    /// XmlDataStore use XML file to as the service data source, it implements the IDataStore. Ideally, we should use 
    /// <c>XMLReader</c> and <c>XMLWriter</c> instead of <c>XMLSerializer</c> to handle large XML files. However, I am 
    /// just using <c>XMLSerializer</c> here for simplicity.
    /// </summary>
    public class XmlDataStore : IDataStore
    {
        private String path;
        private XmlSerializer serializer = new XmlSerializer(typeof(Music));

        public XmlDataStore(String path) 
        {
            this.path = path;
        }
        
        /// <summary>
        /// Read one doesn't make sense to XML data source. This is kind of anti-pattern. However, using 
        /// <c>NotImplementedException</c> to prevent incorrect usage of this interface is a simple way to help us 
        /// in this lightweight service. In a real world application, we might want to separate the interfaces.
        /// </summary>
        /// <param name="albumName"></param>
        /// <returns></returns>
        public Album readOne(string albumName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read all albums from XML file and call action for each of them to add the albums to the cache.
        /// </summary>
        /// <param name="action"></param>
        public void readAllAlbums(Action<Album> action)
        {
            Music doc = null;
            using (var fs = new FileStream(this.path, FileMode.Open))
            {
                doc = (Music)this.serializer.Deserialize(fs);
            }
            
            if (doc.Artists == null)
            {
                return;
            }
           
            foreach (var artist in doc.Artists)
            {
                if (artist.Albums != null)
                {
                    foreach (var album in artist.Albums)
                    {
                        action.Invoke(album);
                    }
                }
            }
        }

        /// <summary>
        /// Write one song doesn't make sense to XML data store.
        /// </summary>
        /// <param name="albumName"></param>
        /// <param name="song"></param>
        public void writeOneSong(string albumName, Song song)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the albums to XML file. Each artist will have all his/her albums under the artist node.
        /// </summary>
        /// <param name="albums"></param>
        public void writeAllAlbums(ICollection<Album> albums)
        {
            var artists = new List<Artist>();

            // use linq to group albums by artist name.
            var groups = from album in albums
                    group album by album.Name into artist
                    select new { Name = artist.Key, Artist = artist };

            foreach (var g in groups)
            {
                var ams = new List<Album>();
                foreach (var album in g.Artist) {
                    ams.Add(album);
                }

                var artist = new Artist();
                artist.Name = g.Name;
                artist.Albums = ams;
                artists.Add(artist);
            }

            var doc = new Music();
            doc.Artists = artists;
            using (var fs = new StreamWriter(this.path))
            {
                serializer.Serialize(fs, doc);
            }
        }
    }
}
