using Leopotam.Ecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Module.Fight.Ecs
{
    public static class EcsWorldExtension
    {
        public static string GetName(this EcsWorld world)
        {
            return "EmployeeName";
        }

        public static EcsEntity CreateAirplane(this EcsWorld world)
        {
            EcsEntity entity = world.NewEntity();
            //            / Get() returns component on entity. If component not exists -it will be added.
            //ref Component1 c1 = ref entity.Get<Component1>();
            //            ref Component2 c2 = ref entity.Get<Component2>();
            //            // Del() removes component from entity.
            //            entity.Del<Component2>();
            //            // Component can be replaced with new instance of component. If component not exist - it will be added.
            //            var weapon = new WeaponComponent() { Ammo = 10, GunName = "Handgun" };
            //            entity.Replace(weapon);
            //            // With Replace() you can chain component's creation:
            //            var entity2 = world.NewEntity();
            //            entity2.Replace(new Component1 { Id = 10 }).Replace(new Component2 { Name = "Username" });
            //            // Any entity can be copied with all components:
            //            var entity2Copy = entity2.Copy();
            //            // Any entity can be merged / "moved" to another entity (source will be destroyed):
            //            var newEntity = world.NewEntity();
            //            entity2Copy.MoveTo(newEntity); // all components from entity2Copy moved to newEntity, entity2Copy destroyed.
            //                                           // any entity can be destroyed. 
            //            entity.Destroy();
            return entity;
        }
    }
}
