using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Concurrent;

namespace SongService
{
    /// <summary>
    /// This is the main WCF service interface
    /// </summary>
    [ServiceContract]
    public interface ISongService
    {
        /// <summary>
        /// Find an album by name.
        /// </summary>
        /// <param name="albumNam">album name</param>
        /// <returns>Returns the album with the specified name if it exists. Otherwise, return null.</returns>
        [OperationContract]
        Album FindAlbum(string albumNam);

        /// <summary>
        /// Add a song to album
        /// </summary>
        /// <param name="albumNam">album name</param>
        /// <param name="song">song to add</param>
        [OperationContract]
        void AddSong(string albumNam, Song song);

        /// <summary>
        /// Update song information.
        /// </summary>
        /// <param name="albumNam">albumn name</param>
        /// <param name="song">new song information</param>
        [OperationContract]
        void UpdateSong(string albumNam, Song song);
    }

    /// <summary>
    /// This the the data model for album. We may update an album concurrently. A ConcurrentDictionary
    /// is used here to ensure thread safty and speed up song lookups.
    /// </summary>
    [DataContract]
    public class Album
    {
        private ConcurrentDictionary<int, Song> songIndex = new ConcurrentDictionary<int, Song>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Album()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Album id</param>
        /// <param name="name">Album name</param>
        /// <param name="songs">Songs of the album</param>
        public Album(int id, string name, Song[] songs)
        {
            this.Id = id;
            this.Name = name;
            this.Songs = songs;
        }

        [DataMember]
        [XmlAttribute]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        [XmlElement("song")]
        public Song[] Songs
        {
            get
            {
                return songIndex.Values.ToArray();
            }
            set
            {
                songIndex.Clear();
                foreach (var s in value)
                {
                    AddSong(s);
                }
            }
        }

        [XmlAttribute("title")]
        public string Name 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Add a song to the album.
        /// </summary>
        /// <param name="song">Song to add.</param>
        public void AddSong(Song song)
        {
            if (songIndex.ContainsKey(song.Id))
            {
                throw new SongServiceException(String.Format("Song {0}", song.Id));
            }
            UpdateSong(song);
        }

        /// <summary>
        /// Update a song in the album. If the song doesn't exist, add it.
        /// </summary>
        /// <param name="song">Song to update.</param>
        public void UpdateSong(Song song)
        {
            songIndex.AddOrUpdate(song.Id, song, (k, v) => song);
        }
    }

    /// <summary>
    /// Represents a song inside albums. This is pure data object.
    /// </summary>
    [DataContract]
    public class Song
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Song()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">song id</param>
        /// <param name="title">song title</param>
        /// <param name="length">song legth</param>
        public Song(int id, string title, string length)
        {
            this.Id = id;
            this.Title = title;
            this.Length = length;
        }

        [DataMember]
        [XmlAttribute("SongId")]
        public int Id
        {
            get;
            set;
        }

        [DataMember]
        [XmlAttribute("title")]
        public string Title
        {
            get;
            set;
        }

        [DataMember]
        [XmlAttribute("length")]
        public string Length
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Exception type used to report service errors. If we need to handle some of the errors explicitly, we may 
    /// want to define multiple exception types so that client code can catch them by exception type and handle
    /// errors gracefully.
    /// </summary>
    public class SongServiceException : Exception
    {
        public SongServiceException(string message)
            : base(message)
        {
        }

        public SongServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
