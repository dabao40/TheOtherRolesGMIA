using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using TheOtherRoles.Roles;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public class Silhouette
    {
        public GameObject gameObject;
        public float timeRemaining;
        public bool permanent = false;
        private bool visibleForEveryOne = false;
        public PlayerControl placePlayer;
        private SpriteRenderer renderer;

        public static List<Silhouette> silhouettes = new();

        private static Sprite SilhouetteSprite;
        public static Sprite getSilhouetteSprite()
        {
            if (SilhouetteSprite) return SilhouetteSprite;
            SilhouetteSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Silhouette.png", 225f);
            return SilhouetteSprite;
        }

        public Silhouette(Vector3 p, PlayerControl placePlayer, float duration = 1f, bool visibleForEveryOne = true)
        {
            if (duration <= 0f)
            {
                TheOtherRolesPlugin.Logger.LogMessage("silhouette: permanent!");
                permanent = true;
            }
            var yoyo = Yoyo.getRole(placePlayer);
            if (yoyo == null) return;

            this.visibleForEveryOne = visibleForEveryOne;
            gameObject = new GameObject("Silhouette");
            gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            //Vector3 position = new Vector3(p.x, p.y, PlayerControl.LocalPlayer.transform.localPosition.z + 0.001f); // just behind player
            Vector3 position = new(p.x, p.y, p.y / 1000f + 0.01f);
            gameObject.transform.position = position;
            gameObject.transform.localPosition = position;

            renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = getSilhouetteSprite();

            timeRemaining = duration;
            this.placePlayer = placePlayer;
            renderer.color = renderer.color.SetAlpha(yoyo.SilhouetteVisibility);

            bool visible = visibleForEveryOne || PlayerControl.LocalPlayer == placePlayer || PlayerControl.LocalPlayer.Data.IsDead;

            gameObject.SetActive(visible);
            silhouettes.Add(this);
        }

        public static void clearSilhouettes(PlayerControl player = null)
        {
            foreach (var sil in new List<Silhouette>(silhouettes)) {
                if (player != null && sil.placePlayer != player) continue;
                sil.gameObject.Destroy();
                silhouettes.Remove(sil);
            }
        }

        public static void UpdateAll()
        {
            foreach (Silhouette current in new List<Silhouette>(silhouettes))
            {
                if (current == null || current.gameObject == null) continue;
                var yoyo = Yoyo.getRole(current.placePlayer);
                current.timeRemaining -= Time.fixedDeltaTime;
                bool visible = current.visibleForEveryOne || PlayerControl.LocalPlayer == current.placePlayer || PlayerControl.LocalPlayer.Data.IsDead;
                current.gameObject.SetActive(visible);

                if (visible && current.timeRemaining > 0 && current.timeRemaining < 0.5)
                {
                    var alphaRatio = current.timeRemaining / 0.5f;
                    current.renderer.color = current.renderer.color.SetAlpha((yoyo != null ? yoyo.SilhouetteVisibility : 0) * alphaRatio);
                }

                if (current.timeRemaining < 0 && !current.permanent)
                {
                    TheOtherRolesPlugin.Logger.LogMessage($"update: permanent: {current.permanent}, time: {current.timeRemaining}");
                    current.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(current.gameObject);
                    silhouettes.Remove(current);
                }
            }
        }
    }
}
