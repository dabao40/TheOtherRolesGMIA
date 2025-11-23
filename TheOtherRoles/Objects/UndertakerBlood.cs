using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public class UndertakerBlood
    {
        public static List<UndertakerBlood> undertakerBloods = new();

        public GameObject undertakerBlood;
        private GameObject background;

        private static Sprite undertakerBloodSprite;
        public static Sprite getUndertakerBloodSprite()
        {
            if (undertakerBloodSprite) return undertakerBloodSprite;
            undertakerBloodSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CrimeScene.png", 300f);
            return undertakerBloodSprite;
        }

        public UndertakerBlood(Vector2 p)
        {
            undertakerBlood = new GameObject("UndertakerBlood") { layer = 11 };
            undertakerBlood.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
            background = new GameObject("Background") { layer = 11 };
            background.transform.SetParent(undertakerBlood.transform);
            Vector3 position = new(p.x, p.y, p.y / 1000 + 0.001f); // just behind player
            undertakerBlood.transform.position = position;
            background.transform.localPosition = new Vector3(0, 0, -1f); // before player

            var undertakerBloodRenderer = undertakerBlood.AddComponent<SpriteRenderer>();
            undertakerBloodRenderer.sprite = getUndertakerBloodSprite();

            undertakerBlood.SetActive(true);
            undertakerBloods.Add(this);
        }

        public static void clearUndertakerBloods()
        {
            undertakerBloods = new List<UndertakerBlood>();
        }
    }
}
