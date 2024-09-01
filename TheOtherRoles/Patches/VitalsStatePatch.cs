using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.Utilities;

namespace TheOtherRoles.Patches
{
    using FakeVitalsParam = (byte playerId, VitalsState state);
    public enum VitalsState
    {
        Disconnected,
        Alive,
        Dead,
    }
    public record FakeVitals(FakeVitalsParam[] Players);

    public class VitalsStatePatch
    {
        static public FakeVitals VitalsFromActuals
        {
            get
            {
                List<FakeVitalsParam> param = new();
                var deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>(); ;
                foreach (var p in GameData.Instance.AllPlayers.GetFastEnumerator())
                {
                    VitalsState state = VitalsState.Alive;
                    if (p.IsDead)
                        state = deadBodies.Any(d => d.ParentId == p.PlayerId) ? VitalsState.Dead : VitalsState.Disconnected;

                    param.Add(new(p.PlayerId, state));
                }
                return new(param.ToArray());
            }
        }
    }
}
