using System.Collections.Generic;

namespace SongService
{
    /// <summary>
    /// Base helper class that defines the default decorator behavior: forward all calls to the 
    /// decorated music service. The concrete decorator classes will override some of these methods
    /// to add behaviors to the decorated service object to do cache load and sync.
    /// </summary>
    public abstract class CacheMusicSvcDecorator : IMusicSvc
    {
        protected IMusicSvc svc;
        protected IDataStore dataStore;

        public CacheMusicSvcDecorator(IMusicSvc svc, IDataStore dataStore)
        {
            this.svc = svc;
            this.dataStore = dataStore;
        }

        public virtual Album FindAlbum(string albumName)
        {
            return this.svc.FindAlbum(albumName);
        }

        public virtual void AddSong(string albumName, Song song)
        {
            this.svc.AddSong(albumName, song);
        }

        public virtual void AddAlbum(Album album)
        {
            this.svc.AddAlbum(album);
        }

        public virtual void UpdateSong(string albumNam, Song song)
        {
            this.svc.UpdateSong(albumNam, song);
        }

        public virtual ICollection<Album> Albums
        {
            get { return this.svc.Albums; }
        }
    }
}
