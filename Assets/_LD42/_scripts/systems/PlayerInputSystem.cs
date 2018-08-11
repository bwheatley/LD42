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

            foreach (var entity in GetEntities<Group>())
            {

                //if (entity.PlayerCollision.newCollide == 1) {
                //    entity.PlayerInput.Horizontal = 0;
                //    entity.PlayerInput.Vertical = 0;
                //    return;
                //}

                entity.PlayerInput.Horizontal = Input.GetAxis("Horizontal");
                entity.PlayerInput.Vertical = Input.GetAxis("Vertical");
            }

        }
    }
}
