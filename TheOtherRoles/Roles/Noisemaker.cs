using TheOtherRoles.Modules;
using UnityEngine;
using static TheOtherRoles.Patches.PlayerControlFixedUpdatePatch;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Roles
{
    public class Noisemaker : RoleBase<Noisemaker>
    {
        public static Color32 color = new(160, 131, 187, byte.MaxValue);
        public static PlayerControl noisemaker;
        public PlayerControl currentTarget = null;
        public PlayerControl target = null;

        public AchievementToken<int> acTokenChallenge = null;

        public Noisemaker()
        {
            RoleId = roleId = RoleId.Noisemaker;
            currentTarget = null;
            target = null;
            acTokenChallenge = null;
            numSound = Mathf.RoundToInt(CustomOptionHolder.noisemakerSoundNumber.getFloat());
        }

        public override void FixedUpdate()
        {
            if (PlayerControl.LocalPlayer != player) return;
            currentTarget = setTarget();
            if (target == null && numSound > 0)
                setPlayerOutline(currentTarget, color);
        }

        public override void PostInit()
        {
            if (PlayerControl.LocalPlayer != player) return;
            acTokenChallenge ??= new("noisemaker.challenge", 0, (val, _) => val >= 3);
        }

        public enum SoundTarget
        {
            Noisemaker,
            Crewmates,
            Everyone
        }

        public static float cooldown;
        public static float duration;
        public int numSound = 0;
        public static SoundTarget soundTarget;

        private static Sprite buttonSprite;
        public static Sprite getButtonSprite()
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NoisemakerButton.png", 115f);
            return buttonSprite;
        }

        public static void clearAndReload()
        {
            cooldown = CustomOptionHolder.noisemakerCooldown.getFloat();
            duration = CustomOptionHolder.noisemakerSoundDuration.getFloat();
            soundTarget = (SoundTarget)CustomOptionHolder.noisemakerSoundTarget.getSelection();
            players = [];
        }
    }
}
