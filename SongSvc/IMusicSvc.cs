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
    public interface IMusicSvc : ISongService
    {
        /// <summary>
        /// Add an album.
        /// This is the internal API isn't exposed on the WCF service.
        /// </summary>
        /// <param name="album"></param>
        void AddAlbum(Album album);

        /// <summary>
        /// Get all albums.
        /// This is the internal API isn't exposed on the WCF service.
        /// </summary>
        System.Collections.Generic.ICollection<Album> Albums { get; }

    }
}
