
namespace SongService
{
    /// <summary>
    /// Preload cache by reading all data from data store so that there is no cache miss after the service 
    /// starts.
    /// </summary>
    public class PreloadCacheMusicSvc : CacheMusicSvcDecorator
    {
        /// <summary>
        /// Construct a preload cache music service decorator.
        /// </summary>
        /// <param name="svc">The decorated music service object.</param>
        /// <param name="dataStore">The data store object that represents the data store.</param>
        public PreloadCacheMusicSvc(IMusicSvc svc, IDataStore dataStore)
            : base(svc, dataStore)
        {
            refreshCache();

            // TODO: start background timer to refresh cache periodically.
        }

        /// <summary>
        /// Read all albums from data store and call <c>addAlbum()</c> on the decorated cache music service 
        /// object to add or update the album. 
        /// PLEASE NOTE THAT THIS WON'T DELETE ALBUMS. IF WE NEED TO HANDLE DELETING ALBUMS, WE WILL HAVE 
        /// TO MAKE A COPY OF THE ALBUM INDEX DICTIONARY AND REPLACE THE OLD ONE WHEN WE REFRESH THE CACHE.
        /// </summary>
        void refreshCache()
        {
            dataStore.readAllAlbums(svc.AddAlbum);
        }
    }
}
