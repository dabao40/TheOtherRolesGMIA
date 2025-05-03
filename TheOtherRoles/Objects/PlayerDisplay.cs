using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public class PlayerDisplay : MonoBehaviour
    {
        public CosmeticsLayer Cosmetics { get; private set; } = null!;
        public PlayerAnimations Animations { get; private set; } = null!;

        static PlayerDisplay()
        {
            ClassInjector.RegisterTypeInIl2Cpp<PlayerDisplay>();
        }

        public void Awake()
        {
            Cosmetics = gameObject.GetComponentInChildren<CosmeticsLayer>();
            Animations = gameObject.GetComponentInChildren<PlayerAnimations>();

            SetBodyType(PlayerBodyTypes.Normal);
            Cosmetics.ToggleName(false);

            Cosmetics.zIndexSpacing = 0.001f;
            Cosmetics.SetCosmeticZIndices();
        }

        public void UpdateFromDefault()
        {
            Cosmetics.SetMaskType(PlayerMaterial.MaskType.None);
            Cosmetics.SetBodyColor(0);

            Cosmetics.SetSkin(string.Empty, 0);
            Cosmetics.SetHatColor(Palette.White);
            Cosmetics.SetVisorAlpha(Palette.White.a);

            Cosmetics.SetHat(string.Empty, 0);
            Cosmetics.SetVisor(string.Empty, 0);
            Cosmetics.SetEnabledColorblind(false);
        }


        public void UpdateFromPlayerOutfit(PlayerControl player, bool isDead, bool includePet)
        {
            NetworkedPlayerInfo.PlayerOutfit outfit = player.Data.Outfits[player.CurrentOutfitType];
            UpdateFromPlayerOutfit(outfit, isDead, includePet);
        }

        public void UpdateFromPlayerOutfit(NetworkedPlayerInfo.PlayerOutfit outfit, bool isDead, bool includePet)
        {
            Cosmetics.SetMaskType(PlayerMaterial.MaskType.None);
            Cosmetics.SetBodyColor(outfit.ColorId);

            Cosmetics.SetSkin(isDead ? string.Empty : outfit.SkinId, outfit.ColorId);
            Color c = isDead ? Palette.HalfWhite : Palette.White;
            Cosmetics.SetHatColor(c);
            Cosmetics.SetVisorAlpha(c.a);

            Cosmetics.SetHat(outfit.HatId, outfit.ColorId);
            if (includePet) Cosmetics.SetPetIdle(DestroyableSingleton<HatManager>.Instance.GetPetById(outfit.PetId), outfit.ColorId, null);
            Cosmetics.SetVisor(outfit.VisorId, outfit.ColorId);
            Cosmetics.SetEnabledColorblind(false);
        }

        public void SetBodyType(PlayerBodyTypes bodyType)
        {
            Cosmetics.EnsureInitialized(bodyType);
            Animations.SetBodyType(bodyType, Cosmetics.currentBodySprite.normalCosmeticOffset, Cosmetics.currentBodySprite.flippedCosmeticOffset);
            if (bodyType == PlayerBodyTypes.Normal) Cosmetics.normalBodySprite.Visible = true;
        }

        public void SetLayer(int layer)
        {
            void SetLayerRecursively(Transform transform)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    child.gameObject.layer = layer;
                    SetLayerRecursively(child);
                }
            }

            SetLayerRecursively(transform);
        }
    }
}
