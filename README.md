
# Basic Design Theory

In order to handle the large volume of concurrent requests efficiently, we need to cache data in memory from the data source. 
As we have multiple types of data source and we need to switch between data sources transperantly to our clients, we need to 
model the data sources with a unified interface and implement the interface for both XML and SQL data sources. 

# Basic OOP obstraction

However, XML and SQL data store is working very differently. For example, we generally read and write the whole XML file when
we need to load or save changed data, we call it snapshot data sync. However, a snapshot data sync for SQL is very wasteful. 
In a case when we have huge dataset shared by a cluster of server, we want to minimize the amount of data transferred between
service and databases. We generally do a incremental cache sync. 

# Design Patterns

I applied the "Decorate Pattern" on the service model. A decorate pattern allows us to attache addtional behaviors to an simple
object with the same interface. Let's assume we do a pure memory service without a data source sync. We can put the data into 
a memory concurrent hash table, which serves user with the perfect performance. This pure memory service is coded as 
class '''MusicCacheSvc'''. This class alone can work independently. It doesn't have any external dependency to data sources or
cache loading policy (e.g. cache preload, or delay load). 

The cache loading polices are attached to the basic pure memory solution by following decorators:
1. ```PreloadCacheMusicSvc```: Preload all data from data source. 
2. ```LazyLoadCacheMusicSvc```: Delay load cache from data source. This decorator intercepts the find album service calls and 
foward to the basic service to check if the data is in cache, if it's a cache miss, tries to load data from a data source.
3. ```SnapshotSavingCacheMusicSvc```: Save all cached data into data source periodically. A timer kick off a background writing
process in a fixed schedule. 
4. ```DirtySavingCacheMusicSvc```: This decorator intercepts data update service calls and mark the changed data as dirty. A 
backgroud worker thread will write all dirty data into data source. I uses the PRODUCER/CONSUMER paradigm here. I put a dirty
data entry into a blocking queue while the worker thread tries to take dirty data out from that queue and write to data stores.
we can also extend this to write data synchronously when client want to confirm the writing success.

Because each decorator only override a limited subset of the base service object. I created an abstract decorator class 
```CacheMusicSvcDecorator```, which implements the default decorator behavior: foward the call to the decorated service object. 

This design results in a loosely couple system. All the core services doesn't know each other, policis are attached at runtime
by injecting one service into another. Changes to one service won't affect another. It make it very easy to configure the services cynamically.
For example, if we want speed up service restart, we can use ```PreloadCacheMusicSvc``` and ```XmlDataStore``` to load cache
from a local XML file, which is a old copy of data from database. And then we chain this up by a delay cache loader 
```LazyLoadCacheMusicSvc``` and a ```DirtySavingCacheMusicSvc``` decorator to get data data from database and save dirty cache
data to it. We can also easily extend our service. For example, if we have a cluster and we want to replicate changes to other 
server in the cluster, we can simply create another decorator ```ReplicationCacheMusicSvc```. It monitors the data changes and
broadcast change messages to the cluster. Meanwhile, it also listen to the broadcasting messages and applies changes to local
cache if there is any. All existing code doesn't need to be changed.

Following sample code shows how to use different configurations on the service. 

```
// To create a XML service with precache cache loading and full cache writing.
var xmlDataStore = new XmlDataStore(path);
var svcImpl = new PreloadCacheMusicSvc(new SnapshotSavingCacheMusicSvc(new MusicCacheSvc(), xmlDataStore), xmlDataStore);
var svc = new SongService(svcImpl);

// To switch the service with SQL data store and use incremental cache loading and saving policy.
var sqlDataStore = new SQLDataStore(connectionString);
var sqlSvcImpl = new LazyLoadCacheMusicSvc(new DirtySavingCacheMusicSvc(new MusicCacheSvc(), sqlDataStore), sqlDataStore);
svc.InternalService = sqlSvcImpl;

// We can even combine xml data store and SQL data store. Following example creates a service preload cache from local 
// XML file when the service starts and use SQL data store to handle cache miss and saves the data to SQL database. A local
// data source can speed up service starts for a large scale application.
var hybridSvcImpl = new new PreloadCacheMusicSvc(sqlSvcImpl, xmlDataStore); 
```

# Unit Tests

I didn't create unit tests because of the time limit. 

# Quick Start

Open the project with VS 2013. Hit F5 to run the service in debug model. Use the WCF test client to send testing request to 
the service. 

I made a simple change to the testing XML file Songs.xml. I changed some song ids so that it meets the requirements of unique song id.



