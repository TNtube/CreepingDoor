using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Project.Utils
{
    public static class StaticUpdateHelper
    {
        private class StaticUpdateID { }
        private static void OnStaticUpdate() => staticUpdate?.Invoke();

        private static event System.Action staticUpdate;
        public static event System.Action StaticUpdate {
            add { staticUpdate = (System.Action)System.Delegate.Combine(staticUpdate, value); }
            remove { staticUpdate = (System.Action)System.Delegate.Remove(staticUpdate, value); }
        }

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            PlayerLoopSystem defaultSystems = PlayerLoop.GetDefaultPlayerLoop();
            PlayerLoopSystem staticUpdateSystem = new PlayerLoopSystem { subSystemList = null, updateDelegate = OnStaticUpdate, type = typeof(StaticUpdateID) };
            PlayerLoopSystem systemsWithStaticUpdate = AddSystem<Update>(in defaultSystems, staticUpdateSystem);
            PlayerLoop.SetPlayerLoop(systemsWithStaticUpdate);
        }

        private static PlayerLoopSystem AddSystem<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem systemToAdd) where T : struct
        {
            PlayerLoopSystem newPlayerLoop = new PlayerLoopSystem {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };

            List<PlayerLoopSystem> newSubSystemList = new List<PlayerLoopSystem>();
            foreach (var subSystem in loopSystem.subSystemList) {
                newSubSystemList.Add(subSystem);
                if (subSystem.type == typeof(T)) {
                    newSubSystemList.Add(systemToAdd);
                }
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }
    }
}