﻿using System.Collections;
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
        }

        protected override void OnUpdate() {
            foreach (var entity in GetEntities<Group>())
            {
                if (entity.PlayerCollision.collide == 1 && entity.PlayerCollision.newCollide == 0) {
                    entity.PlayerCollision.newCollide = 1;

                    //Victory?
                    if (entity.PlayerCollision.lastColldierType == gamemanager.instance.victoryCollider) {
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