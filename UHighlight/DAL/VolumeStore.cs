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
        private readonly ILiteCollection<ZoneGroup> _groups;

        public VolumeStore(IEnvironmentAdapter environmentAdapter)
        {
            _database = new LiteDatabase(Path.Combine(environmentAdapter.Directory, "zones.db"));
            _groups = _database.GetCollection<ZoneGroup>();

            _groups.EnsureIndex(group => group.Name);
        }

        public bool Exists(string groupName, string zoneName)
        {
            return _groups.Exists(group =>
                group.Name == groupName &&
                group.Zones.Any(volume => volume.Name == zoneName)
            );
        }

        public void Upsert(Volume volume)
        {
            if (Exists(volume.Group, volume.Name))
                throw new Exception("Confict");

            if (!_groups.Exists(group => group.Name == volume.Group))
            {
                _groups.Insert(new ZoneGroup() 
                { 
                    Name = volume.Group, 
                    Zones = new List<Volume> { volume } 
                });
            }
            else
            {
                ZoneGroup group = GetGroup(volume.Group);
                group.Zones.Add(volume);

                _groups.Update(group);
            }
        }

        public void CreateGroup(string groupName)
        {
            if (!_groups.Exists(group => group.Name == groupName))
                _groups.Insert(new ZoneGroup() { Name = groupName });
        }

        public IEnumerable<string> GetGroupNames()
        {
            return _groups
                .FindAll()
                .Select(group => group.Name);
        }

        public IEnumerable<ZoneGroup> GetGroups()
        {
            return _groups
                .FindAll();
        }

        public ZoneGroup GetGroup(string groupName)
        {
            return _groups.FindOne(group => group.Name == groupName);
        }

        public IEnumerable<Volume> GetVolumes(string groupName)
        {
            return GetGroup(groupName)
                .Zones;
        }

        public Volume GetVolume(string groupName, string zoneName)
        {
            return GetVolumes(groupName)
                .FirstOrDefault(zone => zone.Name == zoneName);
        }

        public void DeleteVolume(string groupName, string zoneName)
        {
            ZoneGroup group = GetGroup(groupName);

            Volume zone = group.Zones.Find(zone => zone.Name == zoneName);
            group.Zones.Remove(zone);

            _groups.Update(group);
        }

        public void DeleteGroup(string groupName)
        {
            _groups.DeleteMany(group => group.Name == groupName);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}