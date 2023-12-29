#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Adapters;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UHighlight.API;
using UHighlight.Models;

namespace UHighlight.DAL
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class VolumeStore : IVolumeStore
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<Volume> _volumes;
        private readonly ILiteCollection<BsonDocument> _groups;

        public VolumeStore(IEnvironmentAdapter environmentAdapter)
        {
            _database = new LiteDatabase(Path.Combine(environmentAdapter.Directory, "volumes.db"));
            _volumes = _database.GetCollection<Volume>();
            _groups = _database.GetCollection<BsonDocument>("groups");

            _volumes.EnsureIndex(volume => volume.Group);
        }

        public bool Exists(string group, string name)
        {
            return _volumes.Exists(volume =>
                volume.Group == group &&
                volume.Name == name
            );
        }

        public void Upsert(Volume volume)
        {
            if (Exists(volume.Group, volume.Name))
                throw new Exception("Confict");

            if (!_groups.Exists(group => group["Name"] == volume.Group))
                _groups.Insert(new BsonDocument() { ["Name"] = volume.Group });

            _volumes.Upsert(volume);
        }

        public void CreateGroup(string group)
        {
            if (!_groups.Exists(g => g["Name"] == group))
                _groups.Insert(new BsonDocument() { ["Name"] = group });
        }

        public IEnumerable<string> GetGroups()
        {
            return _groups
                .FindAll()
                .Select(group => group["Name"].AsString);
        }

        public IEnumerable<Volume> GetVolumes(string group)
        {
            return _volumes.Find(volume => volume.Group == group);
        }

        public Volume GetVolume(string group, string name)
        {
            return _volumes.FindOne(volume =>
                volume.Group == group &&
                volume.Name == name
            );
        }

        public void DeleteVolume(string group, string name)
        {
            _volumes.DeleteMany(volume =>
                volume.Group == group &&
                volume.Name == name
            );
        }

        public void DeleteGroup(string group)
        {
            _volumes.DeleteMany(volume => volume.Group == group);
            _groups.DeleteMany(g => g["Name"] == group);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}