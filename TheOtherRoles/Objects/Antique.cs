using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public class Antique
    {
        public static List<Antique> antiques = new();
        public GameObject gameObject;
        public SpriteRenderer spriteRenderer;
        public bool isBroken;
        public SystemTypes room;
        public bool isDetected;

        public Antique(Vector2 p, SystemTypes system)
        {
            gameObject = new GameObject("Antique") { layer = 11 };
            gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);

            Vector3 position = new(p.x, p.y, p.y / 1000f + 0.01f);
            gameObject.transform.position = position;
            gameObject.transform.localPosition = position;

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = getNormalSprite();
            spriteRenderer.color = spriteRenderer.color.SetAlpha(0.5f);

            room = system;
            isBroken = false;
            isDetected = false;
            gameObject.SetActive(false);
            antiques.Add(this);
        }

        public void revealAntique()
        {
            if (gameObject != null && spriteRenderer != null)
            {
                gameObject.SetActive(true);
                spriteRenderer.sprite = getBrokenSprite();
                spriteRenderer.color = spriteRenderer.color.SetAlpha(1f);
                return;
            }
        }

        public static void clearAllAntiques()
        {
            foreach (var antique in antiques) antique.gameObject.Destroy();
            antiques = new();
        }

        private static Sprite normalSprite;
        private static Sprite getNormalSprite()
        {
            if (normalSprite) return normalSprite;
            normalSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ArchaeologistAntique.png", 350f);
            return normalSprite;
        }
        private static Sprite brokenSprite;
        public static Sprite getBrokenSprite()
        {
            if (brokenSprite) return brokenSprite;
            brokenSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.ArchaeologistAntiqueBroken.png", 350f);
            return brokenSprite;
        }

        public static Dictionary<Vector3, SystemTypes> SkeldPos = new()
        {
            { new(2.5f, -16.0f), SystemTypes.Comms },
            { new(-3.5f, -16.0f), SystemTypes.Storage },
            { new(-19.5f, -6.75f), SystemTypes.Reactor },
            { new(-7.5f, -2.0f), SystemTypes.MedBay },
            { new(17.0f, -6.0f), SystemTypes.Nav },
            { new(-17.0f, -13.5f), SystemTypes.LowerEngine },
            { new(-18.0f, 2.5f), SystemTypes.UpperEngine }
        };

        public static Dictionary<Vector3, SystemTypes> MiraPos = new()
        {
            { new(22.0f, 17.5f), SystemTypes.Admin },
            { new(15f, 24.75f), SystemTypes.Greenhouse },
            { new(2.5f, 13.5f), SystemTypes.Reactor },
            { new(-6.5f, 3.0f), SystemTypes.Launchpad },
            { new(16.25f, 3.0f), SystemTypes.Comms },
            { new(13.75f, 17.25f), SystemTypes.Office },
            { new(11.5f, 10.5f), SystemTypes.Laboratory },
            { new(19.6f, -2.0f), SystemTypes.Balcony }
        };

        public static Dictionary<Vector3, SystemTypes> PolusPos = new()
        {
            { new(20f, -25f), SystemTypes.Admin },
            { new(12.75f, -24.75f), SystemTypes.Weapons },
            { new(12.75f, -17.25f), SystemTypes.Comms },
            { new(2.25f, -23.5f), SystemTypes.BoilerRoom },
            { new(33.0f, -10.0f), SystemTypes.Laboratory },
            { new(21.5f, -12.25f), SystemTypes.Storage },
            { new(25.0f, -7.0f), SystemTypes.Laboratory },
            { new(9.75f, -13.25f), SystemTypes.Electrical },
            { new(37.0f, -22f), SystemTypes.Specimens }
        };

        public static Dictionary<Vector3, SystemTypes> AirsihpPos = new()
        {
            { new(-12.0f, 11.5f), SystemTypes.VaultRoom },
            { new(17.25f, 15.25f), SystemTypes.MeetingRoom },
            { new(11.5f, 2.75f), SystemTypes.MainHall },
            { new(6.0f, 3.0f), SystemTypes.MainHall },
            { new(28.5f, -1.5f), SystemTypes.Ventilation },
            { new(31f, 7.25f), SystemTypes.Lounge },
            { new(5.5f, -14.5f), SystemTypes.Security },
            { new(1.5f, -2.5f), SystemTypes.Engine },
            { new(-12.0f, 3.0f), SystemTypes.Comms },
            { new(-20.75f, -3.0f), SystemTypes.Cockpit },
            { new(-13.75f, -7.75f), SystemTypes.Armory },
            { new(-16.75f, -12.5f), SystemTypes.ViewingDeck },
            { new(16.25f, -8.5f), SystemTypes.Electrical },
            { new(20.75f, 2.75f), SystemTypes.Showers }
        };

        public static Dictionary<Vector3, SystemTypes> FunglePos = new()
        {
            { new(1.5f, -1.6f),  SystemTypes.SleepingQuarters },
            { new(-10.0f, 13.0f), SystemTypes.Dropship },
            { new(-15.5f, -7.6f), SystemTypes.Kitchen },
            { new(9f, -11.5f), SystemTypes.Greenhouse },
            { new(-22.5f, -7.25f), SystemTypes.FishingDock },
            { new(20.6f, -8f), SystemTypes.Reactor },
            { new(12.25f, 10.0f), SystemTypes.MiningPit }
        };
    }
}
