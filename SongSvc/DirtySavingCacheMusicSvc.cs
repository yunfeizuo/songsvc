using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongService
{


    /// <summary>
    /// This service decorator add data saving behavior to the basic music service. It uses the producer/consumer
    /// paradigm to kickoff the asynchronous data store writing.
    /// We use 
    /// </summary>
    public class DirtySavingCacheMusicSvc : CacheMusicSvcDecorator
    {
        private BlockingCollection<KeyValuePair<string, Song>> dirtyQuque =
            new BlockingCollection<KeyValuePair<string,Song>>();

        public DirtySavingCacheMusicSvc(IMusicSvc svc, IDataStore dataStore)
            : base(svc, dataStore)
        {
            StartSavingThread();
        }

        /// <summary>
        /// Attache the dirty marking operation to the basic <c>addSong()</c> method.
        /// We push the dirty data into the dirty queue after we call the basic <c>AddSong()</c> so that
        /// the write worker thread can wirte it into the data store.
        /// </summary>
        /// <param name="albumName">album name</param>
        /// <param name="song">song</param>
        public override void AddSong(string albumName, Song song)
        {
            this.svc.AddSong(albumName, song);
            dirtyQuque.Add(new KeyValuePair<string, Song>(albumName, song));
        }

        /// <summary>
        /// Attache the dirty marking operation to the basic <c>addSong()</c> method.
        /// We push the dirty data into the dirty queue after we call the basic <c>AddSong()</c> so that
        /// the write worker thread can wirte it into the data store.
        /// </summary>
        /// <param name="albumName">album name</param>
        /// <param name="song">song</param>
        public override void UpdateSong(string albumName, Song song)
        {
            this.svc.UpdateSong(albumName, song);
            dirtyQuque.Add(new KeyValuePair<string, Song>(albumName, song));
        }

        /// <summary>
        /// Start a background worker thread to write dirty data to data stroe aynchronously.
        /// </summary>
        private void StartSavingThread()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // The write worker thread will be blocked here untill a dirty item is pushed into the queue.
                    KeyValuePair<string, Song> item = dirtyQuque.Take();
                    this.dataStore.writeOneSong(item.Key, item.Value);
                }
            });
        }
    }

}
