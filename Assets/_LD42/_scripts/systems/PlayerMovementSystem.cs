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

                //No moving if dead
                if (gamemanager.instance.dead || gamemanager.instance.lockMovement) {
                    return;
                }


                //Have we hit something if so stop!
                if (entity.PlayerCollision.collide != 1) {
                    
                    position.x += entity.Speed.Value * entity.PlayerInput.Horizontal * Time.deltaTime;
                    rotation.w = Mathf.Clamp(entity.PlayerInput.Horizontal, -0.5f, 0.5f);

                    position.y += entity.Speed.Value * entity.PlayerInput.Vertical * Time.deltaTime;
                    //rotation.w = Mathf.Clamp(entity.PlayerInput.Vertical, -0.5f, 0.5f);

                    entity.Transform.position = position;
                    entity.Transform.rotation = rotation;
                }
                //If we're hitting a trigger apply the inverse of what we were just doing
                else {
                    //Make the collision detection better later.

                    float knockbackForce = gamemanager.instance.knockbackforce;

                    position.x -= entity.Speed.Value * entity.PlayerInput.Horizontal * Time.deltaTime * knockbackForce;
                    rotation.w = Mathf.Clamp(entity.PlayerInput.Horizontal, -0.5f, 0.5f);

                    position.y -= entity.Speed.Value * entity.PlayerInput.Vertical * Time.deltaTime * knockbackForce;

                    entity.Transform.position = position;
                    entity.Transform.rotation = rotation;

                    //entity.PlayerInput.PlayerRigidbody2D.AddForce( new Vector2(-entity.PlayerInput.Horizontal, -entity.PlayerInput.Vertical), ForceMode2D.Impulse);


                    //Debug.Log("We hit something!");
                }
            }
        }
    }

}