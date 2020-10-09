﻿/**
 * Kopernicus Planetary System Modifier
 * ------------------------------------------------------------- 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright of TakeTwo Interactive. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Kopernicus.Components.ModularComponentSystem;
using UnityEngine;

namespace Kopernicus.Components.ModularScatter
{
    /// <summary>
    /// A Scatter Component that can add colliders to a scatter object
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ScatterCollidersComponent : IComponent<ModularScatter>
    {
        /// <summary>
        /// How many scatters were active the last time we added colliders.
        /// This is used to avoid checking for the colliders every frame.
        /// </summary>
        private Int32 scatterCount = 0;

        /// <summary>
        /// The mesh that is used for the collider
        /// </summary>
        public Mesh CollisionMesh;

        /// <summary>
        /// Gets executed every frame and checks if a Kerbal is within the range of the scatter object
        /// </summary>
        void IComponent<ModularScatter>.Update(ModularScatter system)
        {
            float width = 0;
            float height = 0;
            float length = 0;
            float max = 0;
            float distance = 1000;
            Boolean inFlight = false;
            if (HighLogic.LoadedSceneIsFlight)
            {
                Vector3 activeVesselSize = FlightGlobals.ActiveVessel.vesselSize;
                width = activeVesselSize.x;
                height = activeVesselSize.y;
                length = activeVesselSize.z;
                max = Mathf.Max(width, height, length);
                inFlight = true;
            }

            PQSMod_LandClassScatterQuad[] quads = system.GetComponentsInChildren<PQSMod_LandClassScatterQuad>(true);
            for (Int32 i = 0; i < quads.Length; i++)
            {
                var surfaceObjects = quads[i].obj.GetComponentsInChildren<KopernicusSurfaceObject>(true);
                scatterCount = surfaceObjects.Count();
                if (surfaceObjects.Count() == scatterCount)
                {
                    return;
                }
                for (Int32 i2 = 0; i < surfaceObjects.Length; i++)
                {
                    KopernicusSurfaceObject scatter = surfaceObjects[i2];
                    if (inFlight)
                    {
                        distance = Vector3.Distance(FlightGlobals.ActiveVessel.transform.position, scatter.transform.position);
                    }

                    MeshCollider collider = scatter.GetComponent<MeshCollider>();
                    if (distance > max)
                    {
                        if (collider)
                        {
                            GameObject.Destroy(collider);
                        }
                    }
                    else
                    {
                        if (collider)
                        {
                            continue;
                        }

                        MeshFilter filter = scatter.gameObject.GetComponent<MeshFilter>();
                        collider = scatter.gameObject.AddComponent<MeshCollider>();
                        collider.sharedMesh = CollisionMesh ? CollisionMesh : filter.sharedMesh;
                        collider.enabled = true;
                    }
                }
            }
        }

        void IComponent<ModularScatter>.Apply(ModularScatter system)
        {
            // We don't use this
        }

        void IComponent<ModularScatter>.PostApply(ModularScatter system)
        {
            // We don't use this
        }
    }
}
