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
    /// This is the main WCF service object which serves request directly. This class is only a 
    /// light weight wrapper class that forwards the request to the concrete music service implementations.
    /// 
    /// This service take advantage of dependency injection so that it is easier to test and switching configurations
    /// at runtime. The decorator pattern make it is very flexible to change the service behavior dynamically without
    /// dependencies among different modules.
    /// 
    /// To create a <c>SongService</c>, we configure the cached music service with decorator objects dynamically.
    /// We can also switch the underline service data by setting <c>InternalService</c> property when the
    /// service is running (no thread-safty issue because object reference access is atomic).
    /// 
    /// Example: 
    /// <code>
    ///     
    ///     // To create a XML service with precache cache loading and full cache writing.
    ///     var xmlDataStore = new XmlDataStore(path);
    ///     var svcImpl = new PreloadCacheMusicSvc(new SnapshotSavingCacheMusicSvc(new MusicCacheSvc(), xmlDataStore), xmlDataStore);
    ///     var svc = new SongService(svcImpl);
    ///     
    ///     // To switch the service with SQL data store and use incremental cache loading and saving policy.
    ///     var sqlDataStore = new SQLDataStore(connectionString);
    ///     var sqlSvcImpl = new LazyLoadCacheMusicSvc(new DirtySavingCacheMusicSvc(new MusicCacheSvc(), sqlDataStore), sqlDataStore);
    ///     svc.InternalService = sqlSvcImpl;
    ///     
    ///     // We can even combine xml data store and SQL data store. Following example creates a service preload cache from local 
    ///     // XML file when the service starts and use SQL data store to handle cache miss and saves the data to SQL database. A local
    ///     // data source can speed up service starts for a large scale application.
    ///     var hybridSvcImpl = new new PreloadCacheMusicSvc(sqlSvcImpl, xmlDataStore); 
    /// 
    /// </code>
    /// 
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class SongService : ISongService
    {
        private IMusicSvc impl;
        public static readonly String DEFAULT_XML_FILE_PATH = "Songs.xml";

        /// <summary>
        /// Default service constructor use an XML file song.xml in the current directory as data store. It use cache preload and
        /// periodically full cache saving policy.
        /// </summary>
        public SongService()
        {
            var xmlDataStore = new XmlDataStore(DEFAULT_XML_FILE_PATH);
            impl = new PreloadCacheMusicSvc(new SnapshotSavingCacheMusicSvc(new MusicCacheSvc(), xmlDataStore), xmlDataStore);
        }

        /// <summary>
        /// Construct a song service with service implementation object, which is dynamically configured by
        /// decorator pattern.
        /// </summary>
        /// <param name="impl"></param>
        public SongService(IMusicSvc impl)
        {
            this.impl = impl;
        }

        /// <summary>
        /// Switch underline service data source and cache loading policy.
        /// </summary>
        public IMusicSvc InternalService
        {
            get;
            set;
        }

        /// <summary>
        /// Find an album by name.
        /// </summary>
        /// <param name="albumName">album name</param>
        /// <returns>Returns the album with the specified name if it exists. Otherwise, return null.</returns>
        public Album FindAlbum(string albumName)
        {
            return impl.FindAlbum(albumName);
        }

        /// <summary>
        /// Add a song to album
        /// </summary>
        /// <param name="albumNam">album name</param>
        /// <param name="song">song to add</param>
        public void AddSong(string albumName, Song song)
        {
            impl.AddSong(albumName, song);
        }

        /// <summary>
        /// Update song information.
        /// </summary>
        /// <param name="albumNam">albumn name</param>
        /// <param name="song">new song information</param>
        public void UpdateSong(string albumName, Song song)
        {
            impl.UpdateSong(albumName, song);
        }
    }

}
