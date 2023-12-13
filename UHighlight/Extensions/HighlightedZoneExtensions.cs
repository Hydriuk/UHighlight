using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UHighlight.Components;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.Extensions
{
    public static class HighlightedZoneExtensions
    {
        #region Utilities
        private static Collider[] CastCube(this Volume volume, int mask)
        {
            return Physics.OverlapBox(
                volume.Center,
                volume.Size / 2,
                Quaternion.FromToRotation(Vector3.forward, volume.Rotation),
                mask
            );
        }

        private static Collider[] CastSphere(this Volume volume, int mask)
        {
            return Physics.OverlapSphere(
                volume.Center,
                volume.Size.x / 2,
                mask
            );
        }
        #endregion

        #region IsInside
        public static bool Collides(this HighlightedZone zone, BarricadeDrop drop)
        {
            return zone.Collides(drop.model.position);
        }

        public static bool Collides(this HighlightedZone zone, StructureDrop drop)
        {
            return zone.Collides(drop.model.position);
        }

        public static bool Collides(this HighlightedZone zone, Vector3 center)
        {
            return zone.Collider.ClosestPoint(center) == center;
        }
        #endregion

        #region GetBarricades
        public static IEnumerable<BarricadeDrop> GetBarricades(this HighlightedZone zone) 
        {
            return zone.Volume.Shape switch
            {
                EVolumeShape.Cube => zone.GetBarricadesInCube(),
                EVolumeShape.Sphere => zone.GetBarricadesInSphere(),
                _ => throw new NotImplementedException()
            };
        }

        private static IEnumerable<BarricadeDrop> GetBarricadesInCube(this HighlightedZone zone)
        {
            return zone.Volume
                .CastCube(RayMasks.BARRICADE)
                .Select(collider => collider?.transform?.ToBarricade())
                .Where(collider => collider != null)
                .Cast<BarricadeDrop>();
        }

        private static IEnumerable<BarricadeDrop> GetBarricadesInSphere(this HighlightedZone zone)
        {
            return zone.Volume
                .CastSphere(RayMasks.BARRICADE)
                .Select(collider => collider.transform.ToBarricade())
                .Where(collider => collider != null)
                .Cast<BarricadeDrop>();
        }
        #endregion

        #region GetStructures
        public static IEnumerable<StructureDrop> GetStructures(this HighlightedZone zone)
        {
            return zone.Volume.Shape switch
            {
                EVolumeShape.Cube => zone.GetStructuresInCube(),
                EVolumeShape.Sphere => zone.GetStructuresInSphere(),
                _ => throw new NotImplementedException()
            };
        }

        private static IEnumerable<StructureDrop> GetStructuresInCube(this HighlightedZone zone)
        {
            return zone.Volume
                .CastCube(RayMasks.STRUCTURE)
                .Select(collider => collider.transform.ToStructure())
                .Where(collider => collider != null)
                .Cast<StructureDrop>();
        }

        private static IEnumerable<StructureDrop> GetStructuresInSphere(this HighlightedZone zone)
        {
            return zone.Volume
                .CastSphere(RayMasks.STRUCTURE)
                .Select(collider => collider.transform.ToStructure())
                .Where(collider => collider != null)
                .Cast<StructureDrop>();
        }
        #endregion
    }
}
