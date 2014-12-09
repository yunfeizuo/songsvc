using System;
using System.Collections.Generic;

namespace SongService
{


    /// <summary>
    /// This is the SQL data store which read and write data to SQL database. I won't implement it here because 
    /// it requires a test database which takes long to setup. :) 
    /// 
    /// TODO: add SQL implementation.
    /// </summary>
    public class SQLDataSource : IDataStore
    {

        public Album readOne(string albumName)
        {
            throw new NotImplementedException();
        }

        public void readAllAlbums(Action<Album> action)
        {
            throw new NotImplementedException();
        }

        public void writeOneSong(string albumName, Song song)
        {
            throw new NotImplementedException();
        }

        public void writeAllAlbums(ICollection<Album> albums)
        {
            throw new NotImplementedException();
        }
    }
}
