using System.Collections.Generic;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Janitor : RoleBase<Janitor>
    {
        public static Color color = Palette.ImpostorRed;

        public static bool impostorsCanSeeBody = true;
        public static bool canSabotage = true;

        public Janitor()
        {
            RoleId = roleId = RoleId.Janitor;
        }

        public override void OnKill(PlayerControl target)
        {
            if (!player.Data.IsDead)
            {
                DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
                for (int i = 0; i < array.Length; i++)
                    if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == target.PlayerId)
                        _ = new DeadBodyReporter(player, array[i]);
            }
        }

        public override void OnMeetingStart()
        {
            DeadBodyReporter.clearAndReload();
        }

        public static void clearAndReload()
        {
            impostorsCanSeeBody = CustomOptionHolder.janitorImpostorsCanSeeDeadBody.getBool();
            canSabotage = CustomOptionHolder.janitorCanSabotage.getBool();
            players = [];
            DeadBodyReporter.clearAndReload();
        }
    }

    public class DeadBodyReporter
    {
        public PlayerControl Killer;
        public PlayerControl Target;
        public DeadBody DeadBody;
        public GameObject GameObject = null;
        public bool Reported = false;
        public static float ReportDistance = 1.5f;
        public static int Id = 0;
        public static List<DeadBodyReporter> DeadBodyReporters = [];

        public static void clearAndReload()
        {
            foreach (var reporter in DeadBodyReporters)
            {
                reporter.Destroy();
            }
            DeadBodyReporters = [];
        }

        public void Destroy()
        {
            if (GameObject)
                Object.Destroy(GameObject);
            GameObject = null;
        }

        public void Update()
        {
            if (GameObject == null || Reported || MeetingHud.Instance) return;

            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            bool skipAutoReport = PlayerControl.LocalPlayer == Killer || (Janitor.impostorsCanSeeBody && PlayerControl.LocalPlayer.Data.Role.IsImpostor);

            DeadBody.bodyRenderers[0].color = skipAutoReport ? new(1, 1, 1, 0.3f) : Color.clear;
            if (skipAutoReport) return;

            var distance = Vector2.Distance(GameObject.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
            if (distance <= ReportDistance)
            {
                if (Target != null && Target.Data != null)
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(Target.Data);
                    Reported = true;
                }
            }
        }

        public DeadBodyReporter(PlayerControl player, DeadBody deadBody)
        {
            Killer = player;
            Target = Helpers.playerById(deadBody.ParentId);
            DeadBody = deadBody;
            GameObject = new GameObject("DeadBodyReporter " + Id);
            Id++;
            GameObject.transform.SetParent(DeadBody.transform);

            var pos = DeadBody.TruePosition;
            var vector = new Vector3(pos.x, pos.y, (pos.y / 1000f) + 0.001f);
            GameObject.transform.position = vector;
            GameObject.layer = 11;
            var localCanSee = PlayerControl.LocalPlayer == player || (PlayerControl.LocalPlayer.Data.Role.IsImpostor && Janitor.impostorsCanSeeBody);

            if (!localCanSee) DeadBody.myCollider.tag = "Untagged";

            deadBody.bodyRenderers[0].color = new(1, 1, 1, 0.3f);
            DeadBody.bloodSplatter.color = Color.clear;
            GameObject.SetActive(true);

            DeadBodyReporters.Add(this);
        }
    }
}
