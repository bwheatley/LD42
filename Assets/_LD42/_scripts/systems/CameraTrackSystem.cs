using System.Collections;
using System.Collections.Generic;
using Hybrid.Components;
using UnityEngine;
using Unity.Entities;

namespace Hybrid.Systems
{
    public class CameraTrackSystem : ComponentSystem
    {
        private struct Group
        {
            public CameraTarget CameraTarget;
        }


        protected override void OnUpdate()
        {
            // Entities.ForEach();
            // https://docs.unity3d.com/Packages/com.unity.entities@0.7/manual/ecs_entities_foreach.html
            // TODO this needs to work with the new version
            // foreach (var entity in GetEntities<Group>())
            // {
            //     entity.CameraTarget.CameraTransform.position =
            //         entity.CameraTarget.PlayerTransform.position + entity.CameraTarget.offset;
            // }
        }
    }

}
