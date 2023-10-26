using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
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
                volume.Size.x,
                mask
            );
        }
        #endregion

        #region IsInside
        public static bool Collides(this HighlightedZone zone, BarricadeDrop drop)
        {
            return zone.Collides(drop.model.GetComponent<Collider>());
        }

        public static bool Collides(this HighlightedZone zone, StructureDrop drop)
        {
            return zone.Collides(drop.model.GetComponent<Collider>());
        }

        private static bool Collides(this HighlightedZone zone, Collider dropCollider)
        {
            Vector3 closestPoint = dropCollider.ClosestPoint(zone.Volume.Center);

            return zone.Collider.ClosestPoint(closestPoint) == closestPoint;
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
                .Select(collider => collider.transform.ToBarricade());
        }

        private static IEnumerable<BarricadeDrop> GetBarricadesInSphere(this HighlightedZone zone)
        {
            return zone.Volume
                .CastSphere(RayMasks.BARRICADE)
                .Select(collider => collider.transform.ToBarricade());
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
                .Select(collider => collider.transform.ToStructure());
        }

        private static IEnumerable<StructureDrop> GetStructuresInSphere(this HighlightedZone zone)
        {
            return zone.Volume
                .CastSphere(RayMasks.STRUCTURE)
                .Select(collider => collider.transform.ToStructure());
        }
        #endregion
    }
}
