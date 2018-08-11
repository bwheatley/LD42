using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Hybrid.Components;


namespace Hybrid.Systems
{
    public class PlayerCollisionSystem : ComponentSystem
    {

        private struct Group {
            public PlayerCollision PlayerCollision;
            public PlayerInput PlayerInput;
        }

        protected override void OnUpdate() {
            foreach (var entity in GetEntities<Group>())
            {
                if (entity.PlayerCollision.collide == 1 && entity.PlayerCollision.newCollide == 0) {
                    entity.PlayerCollision.newCollide = 1;

                    //Victory?
                    if (entity.PlayerCollision.lastColldierType == gamemanager.instance.victoryCollider 
                        && gamemanager.instance.GetNextLevel() == false) {

                        gamemanager.instance.SetNextLevel();
                        gamemanager.instance.SetLockMovement(true);

                        //Reset colliders
                        entity.PlayerCollision.collide = 0;
                        entity.PlayerCollision.newCollide = 0;

                        gamemanager.instance.NextLevel();
                        return;
                    }

                    //We Dead?
                    if (entity.PlayerCollision.lastColldierType == gamemanager.instance.deathCollider) {
                        gamemanager.instance.Death();
                    }

                }

            }
        }
    }

}