using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Hazel;
using TheOtherRoles.MetaContext;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    class BombEffect
    {
        static IDividedSpriteLoader ExplosionSprite = DividedSpriteLoader.FromResource("TheOtherRoles.Resources.BombEffect.png", 200f, 4, 2);

        public BombEffect(PlayerControl player)
        {
            Vector3 position = new(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z - 0.001f); // just behind player
            TORGUIManager.Instance.StartCoroutine(CoAnim(position).WrapToIl2Cpp());
        }

        static IEnumerator CoAnim(Vector2 pos)
        {
            if (MeetingHud.Instance) yield break;
            var bombRenderer = Helpers.CreateObject<SpriteRenderer>("BombEffect", null, pos.AsVector3(-10f));
            bombRenderer.transform.localScale = Vector3.one * 1.8f;
            bombRenderer.transform.localEulerAngles = new(0f, 0f, System.Random.Shared.NextSingle() * 360f);
            for (int i = 0; i < 8; i++)
            {
                bombRenderer.sprite = ExplosionSprite.GetSprite(i);
                yield return Effects.Wait(0.12f);
            }
            UnityEngine.Object.Destroy(bombRenderer.gameObject);
        }
    }
}
