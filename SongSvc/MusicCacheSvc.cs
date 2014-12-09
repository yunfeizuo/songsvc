using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SongService
{
    /// <summary>
    /// This is the basic memory cached service handler. In order to handle large volume of requests,
    /// we use ConcurrentDictionary to build index for albums. 
    /// 
    /// This class is pure in memory solution for the song service. It doesn't know anything about the
    /// persistent data store. We will use decorator pattern to add different persitent policies 
    /// dynamically. 
    /// </summary>
    public class MusicCacheSvc : IMusicSvc
    {
        /// <summary>
        /// Thread safe album name index. The key is album name while the value is the album object.
        /// This is used to speed up the album lookup by name.
        /// </summary>
        ConcurrentDictionary<string, Album> albumIndex = new ConcurrentDictionary<string, Album>();

        public Album FindAlbum(string albumName)
        {
            if (albumName == null)
            {
                throw new ArgumentNullException("albumName");
            }

            Album album;
            return albumIndex.TryGetValue(albumName, out album) ? album : null;
        }

        public void AddSong(string albumName, Song song)
        {
            if (albumName == null)
            {
                throw new ArgumentNullException("albumName");
            }
            if (song == null)
            {
                throw new ArgumentNullException("song");
            }

            var album = FindAlbum(albumName);
            if (album == null)
            {
                throw new SongServiceException("Album not found.");
            }
            album.AddSong(song);
        }

        public void UpdateSong(string albumName, Song song)
        {
            if (albumName == null)
            {
                throw new ArgumentNullException("albumName");
            }
            if (song == null)
            {
                throw new ArgumentNullException("song");
            }

            var album = FindAlbum(albumName);
            if (album == null)
            {
                throw new SongServiceException("Album not found.");
            }
            album.UpdateSong(song);
        }

        public void AddAlbum(Album album)
        {
            if (album == null)
            {
                throw new ArgumentNullException("album");
            }
            if (album.Name == null)
            {
                throw new ArgumentNullException("Album requires a name.");
            }
                
            // add or replace the album in the index
            albumIndex.AddOrUpdate(album.Name, album, (k, v) => album);
        }

        public ICollection<Album> Albums
        {
            get {
                return albumIndex.Values;
            }
            
        }
    }

}
