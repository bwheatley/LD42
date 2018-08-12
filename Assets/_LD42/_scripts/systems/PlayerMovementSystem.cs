using System;
using Hybrid.Components;
using Unity.Entities;
using UnityEngine;

namespace Hybrid.Systems
{
    public class PlayerMovementSystem : ComponentSystem
    {

        private struct Group
        {
            public Transform Transform;
            public PlayerInput PlayerInput;
            public PlayerCollision PlayerCollision;
            public Speed Speed;
        }


        protected override void OnUpdate()
        {

            foreach (var entity in GetEntities<Group>())
            {
                var position = entity.Transform.position;
                var rotation = entity.Transform.rotation;
                float floatHeight = 1;
                float liftForce = 1 ;

                //If player is moving say so
                if ((entity.PlayerInput.Horizontal < -.2f  || entity.PlayerInput.Horizontal > .2f) || entity.PlayerInput.Vertical < -.2f || entity.PlayerInput.Vertical > .2f) {
                    gamemanager.instance.playeMoving = true;
                    gamemanager.instance.PlayWalk(true);
                }
                else {
                    gamemanager.instance.playeMoving = false;
                    gamemanager.instance.PlayWalk(false);
                }

                //No moving if dead
                if (gamemanager.instance.dead || gamemanager.instance.lockMovement) {
                    entity.PlayerInput.Horizontal = 0;
                    entity.PlayerInput.Vertical = 0;
                    return;
                }

                var pos = entity.Transform.position;
                var dir = new Vector2(entity.PlayerInput.Horizontal, entity.PlayerInput.Vertical);
                var hit = Physics2D.Raycast(pos, dir, 10, gamemanager.instance.hitLayers);

                if (hit.collider != null)
                {
                    //float distance = Mathf.Abs(hit.point.x - entity.Transform.position.x);
                    Debug.Log(string.Format("We hit something Distance: {0}!", hit.distance));

                    if (hit.distance <= gamemanager.instance.minimumMoveRange) {
                        entity.PlayerInput.Horizontal = 0;
                        entity.PlayerInput.Vertical = 0;
                        return;
                    }

                }

                position.x += entity.Speed.Value * entity.PlayerInput.Horizontal * Time.deltaTime;
                position.y += entity.Speed.Value * entity.PlayerInput.Vertical * Time.deltaTime;

                entity.Transform.position = position;
                entity.Transform.rotation = rotation;

                //Have we hit something if so stop!
                if (entity.PlayerCollision.collide != 1) {
                    
                    
                }

            }
        }
    }

}