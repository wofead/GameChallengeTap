using Assets.Scripts.Module.Fight.Ecs.Components;
using Leopotam.Ecs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : IEcsPreInitSystem, IEcsInitSystem, IEcsDestroySystem, IEcsPostDestroySystem
{
    EcsWorld _world = null;
    // We wants to get entities with "WeaponComponent" and without "HealthComponent".
    EcsFilter<PlayerComponent> _playernFilter = null;
    //EcsFilter<WeaponComponent>.Exclude<HealthComponent> _filter = null;
    public void PreInit()
    {
        // Will be called once during EcsSystems.Init() call and before IEcsInitSystem.Init.
    }

    public void Init()
    {
        // Will be called once during EcsSystems.Init() call.
    }

    public void Run()
    {
        // Will be called on each EcsSystems.Run() call.
    }

    public void Destroy()
    {
        // Will be called once during EcsSystems.Destroy() call.
    }

    public void PostDestroy()
    {
        // Will be called once during EcsSystems.Destroy() call and after IEcsDestroySystem.Destroy.
    }
}