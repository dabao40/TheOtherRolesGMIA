using System.Collections;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using TheOtherRoles.MetaContext;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Lighter : RoleBase<Lighter>
    {
        public static Color color = new Color32(238, 229, 190, byte.MaxValue);

        public static float lighterModeLightsOnVision = 2f;
        public static float lighterModeLightsOffVision = 0.75f;
        public static bool canSeeInvisible = true;
        public static float cooldown = 30f;
        public static float duration = 15f;
        public bool lightActive = false;

        public override void ResetRole(bool isShifted)
        {
            lightActive = false;
        }

        public static bool isLightActive(PlayerControl player)
        {
            if (player != null && isRole(player) && !player.Data.IsDead)
            {
                var r = getRole(player);
                return r.lightActive;
            }
            return false;
        }

        public void light()
        {
            var light = Helpers.CreateCustomLight(PlayerControl.LocalPlayer.transform.position, ShipStatus.Instance.CalculateLightRadius(PlayerControl.LocalPlayer.Data) * 6f, true);
            IEnumerator CoLight()
            {
                while (lightActive)
                {
                    light.transform.position = PlayerControl.LocalPlayer.transform.position;
                    yield return null;
                }

                Object.Destroy(light.gameObject);
            }

            TORGUIManager.Instance.StartCoroutine(CoLight());
        }

        public Lighter()
        {
            RoleId = roleId = RoleId.Lighter;
            lightActive = false;
        }

        public override void OnMeetingEnd(PlayerControl exiled = null)
        {
            lightActive = false;
        }

        public static void clearAndReload()
        {
            lighterModeLightsOnVision = CustomOptionHolder.lighterModeLightsOnVision.getFloat();
            lighterModeLightsOffVision = CustomOptionHolder.lighterModeLightsOffVision.getFloat();
            canSeeInvisible = CustomOptionHolder.lighterCanSeeInvisible.getBool();
            cooldown = CustomOptionHolder.lighterCooldown.getFloat();
            duration = CustomOptionHolder.lighterDuration.getFloat();
            players = [];
        }
    }
}
