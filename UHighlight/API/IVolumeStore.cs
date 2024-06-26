﻿#if OPENMOD
using OpenMod.API.Ioc;
#endif
using System;
using System.Collections.Generic;
using UHighlight.Models;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IVolumeStore : IDisposable
    {
        bool Exists(string groupName);
        bool Exists(string groupName, string zoneName);
        void Upsert(Volume volume);
        void CreateGroup(string groupName);
        void UpdateGroup(ZoneGroup group);
        IEnumerable<string> GetGroupNames();
        IEnumerable<ZoneGroup> GetGroups();
        ZoneGroup GetGroup(string groupName);
        IEnumerable<Volume> GetVolumes(string groupName);
        Volume GetVolume(string groupName, string zoneName);
        void DeleteVolume(string groupName, string zoneName);
        void DeleteGroup(string groupName);
        List<ZoneProperty> GetProperties(string groupName);
        void CreateProperty(string groupName, ZoneProperty property);
        void DeleteProperty(string groupName, int index);
    }
}