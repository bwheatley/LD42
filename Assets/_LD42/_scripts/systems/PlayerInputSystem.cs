using System.Collections;
using System.Collections.Generic;
using Hybrid.Components;
using Unity.Entities;
using UnityEngine;

namespace Hybrid.Systems
{
    public class PlayerInputSystem : ComponentSystem
    {
        private struct Group
        {
            public PlayerInput PlayerInput;
            public PlayerCollision PlayerCollision;
        }

        protected override void OnUpdate()
        {

            // Entities.ForEach();
            // https://docs.unity3d.com/Packages/com.unity.entities@0.7/manual/ecs_entities_foreach.html
            // TODO fix for new version of entities
            // foreach (var entity in GetEntities<Group>())
            // {
            //
            //     //if (entity.PlayerCollision.newCollide == 1) {
            //     //    entity.PlayerInput.Horizontal = 0;
            //     //    entity.PlayerInput.Vertical = 0;
            //     //    return;
            //     //}
            //
            //     entity.PlayerInput.Horizontal = Input.GetAxis("Horizontal");
            //     entity.PlayerInput.Vertical = Input.GetAxis("Vertical");
            // }

        }
    }
}
