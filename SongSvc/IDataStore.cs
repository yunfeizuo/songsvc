using System;
using System.Collections.Generic;

namespace SongService
{


    /// <summary>
    /// Generic interface for both XML and SQL data store.
    /// </summary>
    public interface IDataStore 
    {
        /// <summary>
        /// Read one album from data source.
        /// </summary>
        /// <param name="albumName">album name</param>
        /// <returns>The album with the specified name.</returns>
        Album readOne(string albumName);

        /// <summary>
        /// Read all albums from XML file and call action for each of them to add the albums to the cache.
        /// </summary>
        /// <param name="action">The callback which acts on each album we read from data source.</param>
        void readAllAlbums(Action<Album> action);

        /// <summary>
        /// Write one song to data store.
        /// </summary>
        /// <param name="albumName">album name</param>
        /// <param name="song">Song information</param>
        void writeOneSong(string albumName, Song song);

        /// <summary>
        /// Write all albums to data store.
        /// </summary>
        /// <param name="albums">All cached albums.</param>
        void writeAllAlbums(ICollection<Album> albums);
    }

}
