using InnerNet;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Objects
{
    public class FootprintHolder : MonoBehaviour
    {
        static FootprintHolder() => ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();

        public FootprintHolder(IntPtr ptr) : base(ptr) { }

        private static FootprintHolder _instance;
        public static FootprintHolder Instance
        {
            get => _instance ? _instance : _instance = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
            set => _instance = value;

        }

        private static Sprite _footprintSprite;
        private static Sprite FootprintSprite => _footprintSprite ??= Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Footprint.png", 600f);

        private static bool AnonymousFootprints => TheOtherRoles.Detective.anonymousFootprints;
        private static float FootprintDuration => TheOtherRoles.Detective.footprintDuration;

        private static int FootPrintsPerPlayer => (int)(1 / TheOtherRoles.Detective.footprintIntervall * TheOtherRoles.Detective.footprintDuration);

        private static int nextFootStep = 0;

        private static List<List<Footprint>> footPrintObjectList2D = new();

        public static void clearAndReload()
        {
            footPrintObjectList2D.ForEach(x => x.ForEach(y => y.GameObject.Destroy()));
            footPrintObjectList2D.Clear();
            foreach (var player in CachedPlayer.AllPlayers)
            {
                List<Footprint> fpList = new();
                for (int i = 0; i < FootPrintsPerPlayer; i++)
                {
                    fpList.Add(new Footprint());
                }
                footPrintObjectList2D.Add(fpList);
            }
            nextFootStep = 0;
        }

        public static void updateNextFootstep()
        {
            nextFootStep = (nextFootStep + 1) % FootPrintsPerPlayer;
        }

        private class Footprint
        {
            public GameObject GameObject;
            public Transform Transform;
            public SpriteRenderer Renderer;
            public PlayerControl Owner;
            public GameData.PlayerInfo Data;
            public float Lifetime;

            public Footprint()
            {
                GameObject = new("Footprint") { layer = 8 };
                Transform = GameObject.transform;
                Renderer = GameObject.AddComponent<SpriteRenderer>();
                Renderer.sprite = FootprintSprite;
                Renderer.color = Color.clear;
                GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                GameObject.SetActive(false);
            }
        }


        [HideFromIl2Cpp]
        public void MakeFootprint(PlayerControl player)
        {
            int playerN = CachedPlayer.AllPlayers.IndexOf(CachedPlayer.AllPlayers.First(x => x.PlayerId == player.PlayerId));
            Footprint print = footPrintObjectList2D[player.PlayerId][nextFootStep];

            print.Lifetime = FootprintDuration;

            var pos = player.transform.position;
            pos.z = pos.y / 1000f + 0.001f;
            print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, UnityEngine.Random.Range(0.0f, 360.0f)));
            print.GameObject.SetActive(true);
            print.Owner = player;
            print.Data = player.Data;
        }


        private static float updateDt = 0.10f;

        private void Start()
        {
            InvokeRepeating(nameof(FootprintUpdate), updateDt, updateDt);
        }
        private void FootprintUpdate()
        {
            if (Detective.detective == null || Detective.detective != CachedPlayer.LocalPlayer.PlayerControl)
                return;
            for (int playerN = 0; playerN < CachedPlayer.AllPlayers.Count; playerN++)
            {
                foreach (var activeFootprint in footPrintObjectList2D[playerN])
                {
                    var p = activeFootprint.Lifetime / FootprintDuration;

                    if (activeFootprint.Lifetime <= 0)
                    {
                        activeFootprint.GameObject.SetActive(false);
                        continue;
                    }

                    Color color;
                    if (AnonymousFootprints || Camouflager.camouflageTimer > 0 || Helpers.MushroomSabotageActive())
                    {
                        color = Palette.PlayerColors[6];
                    }
                    else if (activeFootprint.Owner == Morphling.morphling && Morphling.morphTimer > 0 && Morphling.morphTarget && Morphling.morphTarget.Data != null)
                    {
                        color = Palette.PlayerColors[Morphling.morphTarget.Data.DefaultOutfit.ColorId];
                    }
                    else
                    {
                        color = Palette.PlayerColors[activeFootprint.Data.DefaultOutfit.ColorId];
                    }
                    color.a = Math.Clamp(p, 0f, 1f);
                    activeFootprint.Renderer.color = color;

                    activeFootprint.Lifetime -= updateDt;
                }
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}