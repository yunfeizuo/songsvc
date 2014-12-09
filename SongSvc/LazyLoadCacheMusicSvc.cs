
namespace SongService
{

    /// <summary>
    /// This the decorator add lazy cache loading to a cached music service, it tries to load cache
    /// from data store when there is a cache miss when <c>findAlbum()</c> is called. It fowards
    /// any other called directly to the decorated object.
    /// Lazy cache loading works better when a service has huge data set but only a small subset of
    /// them are used at a given time frame. In a real world application, we will also have an eviction
    /// policy that decides when a cached item should be removed from cache. 
    /// </summary>
    public class LazyLoadCacheMusicSvc : CacheMusicSvcDecorator
    {
        /// <summary>
        /// Construct a LazyLoadCacheMusicSvc decorator object.
        /// </summary>
        /// <param name="svc">The decorated music service object.</param>
        /// <param name="dataStore">The data store object that represents the data store.</param>
        LazyLoadCacheMusicSvc(IMusicSvc svc, IDataStore dataStore) 
            : base(svc, dataStore)
        {
        }

        /// <summary>
        /// Override the default forwarding behavior. We will try to use the d
        /// </summary>
        /// <param name="albumName"></param>
        /// <returns></returns>
        public override Album FindAlbum(string albumName)
        {
            var album = svc.FindAlbum(albumName);
            if (album == null)
            {
                // cache miss, try to load from data source
                album = dataStore.readOne(albumName);
                svc.AddAlbum(album);
            }
            return album;
        }
    }
}
