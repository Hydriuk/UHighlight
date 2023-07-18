#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Adapters;
using LiteDB;
using System;
using System.Collections.Generic;
using UHighlight.API;
using UHighlight.Models;
using System.Linq;
using System.IO;
using System.Reflection;

namespace UHighlight.DAL
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    public class VolumeStore : IVolumeStore
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<Volume> _volumes;
        private readonly ILiteCollection<BsonDocument> _categories;

        public VolumeStore(IEnvironmentAdapter environmentAdapter)
        {
            _database = new LiteDatabase(Path.Combine(environmentAdapter.Directory, "volumes.db"));
            _volumes = _database.GetCollection<Volume>();
            _categories = _database.GetCollection<BsonDocument>("categories");

            _volumes.EnsureIndex(volume => volume.Category);
        }

        public bool Exists(string category, string name)
        {
            return _volumes.Exists(volume => 
                volume.Category == category && 
                volume.Name == name
            );
        }

        public void Upsert(Volume volume)
        {
            if (Exists(volume.Category, volume.Name))
                throw new Exception("Confict");

            if(!_categories.Exists(category => category["Name"] == volume.Category))
                _categories.Insert(new BsonDocument() { ["Name"] = volume.Category });

            _volumes.Upsert(volume);
        }

        public IEnumerable<string> GetCategories()
        {
            return _categories
                .FindAll()
                .Select(category => category["Name"].AsString);
        }

        public IEnumerable<Volume> GetVolumes(string category)
        {
            return _volumes.Find(volume => volume.Category == category);
        }

        public Volume GetVolume(string category, string name)
        {
            return _volumes.FindOne(volume => 
                volume.Category == category && 
                volume.Name == name
            );
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
