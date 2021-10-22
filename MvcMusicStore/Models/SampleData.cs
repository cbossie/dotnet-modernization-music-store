using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MvcMusicStore.Models
{
    public class SampleData : DropCreateDatabaseIfModelChanges<MusicStoreEntities>
    {
    }
}