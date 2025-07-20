using TheOtherRoles.Roles;
using UnityEngine;

namespace TheOtherRoles.Objects {
    public class Arrow {
        public float perc = 0.925f;
        public SpriteRenderer image;
        public GameObject arrow;
        private Vector3 oldTarget;
        private ArrowBehaviour arrowBehaviour;

        private static Sprite sprite;
        public static Sprite getSprite() {
            if (sprite) return sprite;
            sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Arrow.png", 200f);
            return sprite;
        }


        public Arrow(Color color) {
            arrow = new GameObject("Arrow")
            {
                layer = 5
            };
            image = arrow.AddComponent<SpriteRenderer>();
            image.sprite = getSprite();
            image.color = color;
            arrowBehaviour = arrow.AddComponent<ArrowBehaviour>();
            arrowBehaviour.image = image;
        }

        public void Update() {
            Vector3 target = oldTarget;
            Update(target);
        }

        public void Update(Vector3 target, Color? color = null)
        {
            if (arrow == null) return;
            oldTarget = target;

            if (color.HasValue) image.color = color.Value;

            arrowBehaviour.target = target;
            arrowBehaviour.Update();
        }

        public static void UpdateProximity(Vector3 position)
        {
            if (!GameManager.Instance.GameHasStarted) return;

            if (Tracker.local.DangerMeterParent == null)
            {
                Tracker.local.DangerMeterParent = GameObject.Instantiate(GameObject.Find("ImpostorDetector"), HudManager.Instance.transform);
                Tracker.local.Meter = Tracker.local.DangerMeterParent.transform.GetChild(0).GetComponent<DangerMeter>();
                Tracker.local.DangerMeterParent.transform.localPosition = new(3.7f, -1.6f, 0);
                var backgroundrend = Tracker.local.DangerMeterParent.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
                backgroundrend.color = backgroundrend.color.SetAlpha(0.5f);
            }
            Tracker.local.DangerMeterParent.SetActive(MeetingHud.Instance == null && LobbyBehaviour.Instance == null && !PlayerControl.LocalPlayer.Data.IsDead && Tracker.local.tracked != null);
            Tracker.local.Meter.gameObject.SetActive(MeetingHud.Instance == null && LobbyBehaviour.Instance == null && !PlayerControl.LocalPlayer.Data.IsDead && Tracker.local.tracked != null);
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (Tracker.local.tracked == null)
            {
                Tracker.local.Meter.SetDangerValue(0, 0);
                return;
            }
            if (Tracker.local.DangerMeterParent.transform.localPosition.x != 3.7f) Tracker.local.DangerMeterParent.transform.localPosition = new(3.7f, -1.6f, 0);
            float num = float.MaxValue;
            float dangerLevel1;
            float dangerLevel2;

            float sqrMagnitude = (position - Tracker.local.player.transform.position).sqrMagnitude;
            if (sqrMagnitude < (55 * GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod) && num > sqrMagnitude)
            {
                num = sqrMagnitude;
            }

            dangerLevel1 = Mathf.Clamp01((55 - num) / (55 - 15 * GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod));
            dangerLevel2 = Mathf.Clamp01((15 - num) / (15 * GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod));

            Tracker.local.Meter.SetDangerValue(dangerLevel1, dangerLevel2);
        }
    }
}
