using System;
using System.Threading;

namespace SongService
{

    /// <summary>
    /// This is the service decorator that saves the whole cache into data store peroidically. This saving stratigy 
    /// is approperate for XML data store. For SQL data store, the dirty mark saving decorator works a lot better
    /// because it avoids unnessary database operations for data not changed.
    /// </summary>
    public class SnapshotSavingCacheMusicSvc : CacheMusicSvcDecorator
    {
        private Timer timer;

        /// <summary>
        /// Construct a preload cache music service decorator.
        /// </summary>
        /// <param name="svc">The decorated music service object.</param>
        /// <param name="dataStore">The data store object that represents the data store.</param>
        public SnapshotSavingCacheMusicSvc(IMusicSvc svc, IDataStore dataStore)
            : base(svc, dataStore)
        {
            // start timer to save dirty data periodically.
            // TODO: config timer
            this.timer = new Timer(this.SaveCache, this, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
        }

        /// <summary>
        /// Save the whole cache to the data store.
        /// </summary>
        private void SaveCache(object state)
        {
            dataStore.writeAllAlbums(this.svc.Albums);
        }
    }
}
