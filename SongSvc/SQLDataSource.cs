using System;
using System.Collections.Generic;

namespace SongService
{


    /// <summary>
    /// This is the SQL data store which read and write data to SQL database. 
    ///
    /// I DIDN'T IMPLEMENT  THIS CLASS HERE BECAUSE IT REQUIRES A TEST DATABASE WHICH TAKES A LITTLE TIME 
    /// TO SETUP. :) 
    /// 
    /// TODO: add SQL implementation.
    ///
    /// </summary>
    public class SQLDataSource : IDataStore
    {

        public Album readOne(string albumName)
        {
            // TODO: CREATE STORE PROCEDURE AND CALL IT.
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
