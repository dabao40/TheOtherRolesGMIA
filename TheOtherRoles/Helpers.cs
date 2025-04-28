using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using static TheOtherRoles.TheOtherRoles;
using TheOtherRoles.Modules;
using HarmonyLib;
using Hazel;
using TheOtherRoles.Utilities;
using System.Threading.Tasks;
using TheOtherRoles.CustomGameModes;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using System.Globalization;
using TheOtherRoles.Patches;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Runtime.InteropServices;
using TheOtherRoles.MetaContext;
using System.Text;
using BepInEx.Unity.IL2CPP.Utils;
namespace TheOtherRoles
{

    public enum MurderAttemptResult {
        PerformKill,
        SuppressKill,
        BlankKill,
        ReverseKill,
        DelayVampireKill
    }

    public enum CustomGamemodes {
        Classic,
        Guesser,
        HideNSeek,
        FreePlay
    }

    public static class Direction
    {
        public static Vector2 up = Vector2.up;
        public static Vector2 down = Vector2.down;
        public static Vector2 left = Vector2.left;
        public static Vector2 right = Vector2.right;
        public static Vector2 upleft = new(-0.70710677f, 0.70710677f);
        public static Vector2 upright = new(0.70710677f, 0.70710677f);
        public static Vector2 downleft = new(-0.70710677f, -0.70710677f);
        public static Vector2 downright = new(0.70710677f, -0.70710677f);
    }

    public static class Helpers
    {

        public static string previousEndGameSummary = "";
        public static Dictionary<string, Sprite> CachedSprites = new();

        public static Sprite loadSpriteFromResources(string path, float pixelsPerUnit) {
            try
            {
                if (CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
                Texture2D texture = loadTextureFromResources(path);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                return CachedSprites[path + pixelsPerUnit] = sprite;
            } catch {
                System.Console.WriteLine("Error loading sprite from path: " + path);
            }
            return null;
        }

        public static Sprite loadSpriteFromResource(Texture2D texture, float pixelsPerUnit, Rect textureRect, Vector2 pivot)
        {
            return Sprite.Create(texture, textureRect, pivot, pixelsPerUnit);
        }

        public static unsafe Texture2D loadTextureFromResources(string path) {
            try {
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                if (path.Contains("HorseHats")) {
                    byteTexture = new Il2CppStructArray<byte>(byteTexture.Reverse().ToArray());
                }
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            } catch {
                System.Console.WriteLine("Error loading texture from resources: " + path);
            }
            return null;
        }

        public static Texture2D loadTextureFromDisk(string path) {
            try {
                if (File.Exists(path)) {
                    Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                    var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                    ImageConversion.LoadImage(texture, byteTexture, false);
                    return texture;
                }
            } catch {
                TheOtherRolesPlugin.Logger.LogError("Error loading texture from disk: " + path);
            }
            return null;
        }

        /* This function has been removed from TOR because we switched to assetbundles for compressed audio. leaving it here for reference - Gendelo
        public static AudioClip loadAudioClipFromResources(string path, string clipName = "UNNAMED_TOR_AUDIO_CLIP") {
            // must be "raw (headerless) 2-channel signed 32 bit pcm (le)" (can e.g. use Audacity?to export)
            try {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                float[] samples = new float[byteAudio.Length / 4]; // 4 bytes per sample
                int offset;
                for (int i = 0; i < samples.Length; i++) {
                    offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / Int32.MaxValue;
                }
                int channels = 2;
                int sampleRate = 48000;
                AudioClip audioClip = AudioClip.Create(clipName, samples.Length / 2, channels, sampleRate, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            } catch {
                System.Console.WriteLine("Error loading AudioClip from resources: " + path);
            }
            return null;

            // Usage example:
            //AudioClip exampleClip = Helpers.loadAudioClipFromResources("TheOtherRoles.Resources.exampleClip.raw");
            //if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(exampleClip, false, 0.8f);
            
        }*/
        public static PlayerControl playerById(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;
            return null;
        }

        public static Dictionary<byte, PlayerControl> allPlayersById()
        {
            Dictionary<byte, PlayerControl> res = new();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                res.Add(player.PlayerId, player);
            return res;
        }

        // Intersteing Color Gradient Feature :)
        public static string GradientColorText(string startColorHex, string endColorHex, string text)
        {
            if (startColorHex.Length != 6 || endColorHex.Length != 6)
            {
                TheOtherRolesPlugin.Logger.LogError("GradientColorText : Invalid Color Hex Code, Hex code should be 6 characters long (without #) (e.g., FFFFFF).");
                return text;
            }

            Color startColor = HexToColor(startColorHex);
            Color endColor = HexToColor(endColorHex);

            int textLength = text.Length;
            float stepR = (endColor.r - startColor.r) / (float)textLength;
            float stepG = (endColor.g - startColor.g) / (float)textLength;
            float stepB = (endColor.b - startColor.b) / (float)textLength;
            float stepA = (endColor.a - startColor.a) / (float)textLength;

            string gradientText = "";

            for (int i = 0; i < textLength; i++)
            {
                float r = startColor.r + (stepR * i);
                float g = startColor.g + (stepG * i);
                float b = startColor.b + (stepB * i);
                float a = startColor.a + (stepA * i);


                string colorhex = ColorToHex(new Color(r, g, b, a));
                gradientText += $"<color=#{colorhex}>{text[i]}</color>";

            }

            return gradientText;

        }

        public static Color HexToColor(string hex)
        {
            Color color = new();
            _ = ColorUtility.TryParseHtmlString("#" + hex, out color);
            return color;
        }

        private static string ColorToHex(Color color)
        {
            Color32 color32 = (Color32)color;
            return $"{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
        }

        public static void handleVampireBiteOnBodyReport() {
            // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
            checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten, true, false);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
            writer.Write(byte.MaxValue);
            writer.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
        }

        public static void handleTrapperTrapOnBodyReport()
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TrapperMeetingFlag, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.trapperMeetingFlag();
        }

        public static IEnumerator BlackmailShhh()
        {
            yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
            var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
            var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
            HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
                HudManager.Instance.shhhEmblem.transform.localPosition.x,
                HudManager.Instance.shhhEmblem.transform.localPosition.y,
                HudManager.Instance.FullScreen.transform.position.z + 1f);
            HudManager.Instance.shhhEmblem.TextImage.text = ModTranslation.getString("blackmailerBlackmailText");
            HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
            yield return HudManager.Instance.ShowEmblem(true);
            HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
            HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
            yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
            yield return null;
        }

        public static void enableCursor(bool initalSetCursor)
        {
            if (initalSetCursor)
            {
                Sprite sprite = loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
                Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
                return;
            }
            if (TheOtherRolesPlugin.ToggleCursor.Value)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else
            {
                Sprite sprite = loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
                Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
            }
        }

        public static void refreshRoleDescription(PlayerControl player) {
            List<RoleInfo> infos = RoleInfo.getRoleInfoForPlayer(player);
            List<string> taskTexts = new(infos.Count);

            foreach (var roleInfo in infos)
            {
                taskTexts.Add(getRoleString(roleInfo, Husk.husk.Any(x => x.PlayerId == player.PlayerId)));
            }

            var toRemove = new List<PlayerTask>();
            foreach (PlayerTask t in player.myTasks.GetFastEnumerator())
            {
                var textTask = t.TryCast<ImportantTextTask>();
                if (textTask == null) continue;

                var currentText = textTask.Text;

                if (taskTexts.Contains(currentText)) taskTexts.Remove(currentText); // TextTask for this RoleInfo does not have to be added, as it already exists
                else toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
            }

            foreach (PlayerTask t in toRemove) {
                t.OnRemove();
                player.myTasks.Remove(t);
                UnityEngine.Object.Destroy(t.gameObject);
            }

            // Add TextTask for remaining RoleInfos
            foreach (string title in taskTexts) {
                var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = title;
                player.myTasks.Insert(0, task);
            }

            if (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) || player == CreatedMadmate.createdMadmate)
            {
                var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                task.transform.SetParent(player.transform, false);
                task.Text = cs(Madmate.color, $"{Madmate.fullName}: " + ModTranslation.getString("madmateShortDesc"));
                player.myTasks.Insert(0, task);
            }
        }

        internal static string getRoleString(RoleInfo roleInfo, bool isHusk)
        {
            bool addHusk = isHusk && !roleInfo.isModifier;
            if (roleInfo.roleId == RoleId.Jackal)
            {
                var getSidekickText = Jackal.canCreateSidekick && !addHusk ? ModTranslation.getString("jackalWithSidekick") : ModTranslation.getString("jackalShortDesc");
                return cs(roleInfo.color, $"{roleInfo.name}{(addHusk ? $" ({ModTranslation.getString("husk")})" : "")}: {getSidekickText}");
            }

            if (roleInfo.roleId == RoleId.Invert)
            {
                return cs(roleInfo.color, $"{roleInfo.name}: {roleInfo.shortDescription} ({Invert.meetings})");
            }

            return cs(roleInfo.color, $"{roleInfo.name}{(addHusk ? $" ({ModTranslation.getString("husk")})" : "")}: {roleInfo.shortDescription}");
        }

        public static bool isLighterColor(int colorId) {
            return CustomColors.lighterColors.Contains(colorId);
        }

        public static bool isCustomServer() {
            if (FastDestroyableSingleton<ServerManager>.Instance == null) return false;
            StringNames n = FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
            return n is not StringNames.ServerNA and not StringNames.ServerEU and not StringNames.ServerAS;
        }

        public static int[] MinPlayers = new int[7] {
            4, 4, 7, 9, 13, 15, 18
            };

        public static int[] RecommendedKillCooldown = new int[25] {
            0,
            0, 0, 0, 45, 30,
            15, 35, 30, 25, 20,
            20, 20, 20, 20, 20,
            20, 20, 20, 20, 20,
            20, 20, 20, 20
            };

        public static int[] RecommendedImpostors = new int[25] {
            0,
            0, 0, 0, 1, 1,
            1, 2, 2, 2, 2,
            2, 3, 3, 3, 3,
            3, 4, 4, 4, 4,
            5, 5, 5, 5
            };

        public static int[] MaxImpostors = new int[25] {
            0,
            0, 0, 0, 1, 1,
            1, 2, 2, 3, 3,
            3, 3, 4, 4, 5,
            5, 5, 6, 6, 6,
            6, 6, 6, 6
            };

        static public int[] Sequential(int length)
        {
            var array = new int[length];
            for (int i = 0; i < length; i++) array[i] = i;
            return array;
        }

        static public IEnumerator Smooth(this Transform transform, Vector3 goalLocalPosition, float duration)
        {
            float p = 0f;
            var origin = transform.localPosition;
            while (p < 1f)
            {
                p += Time.deltaTime / duration;

                float pp = (1f - p) * (1f - p);
                pp *= pp * pp;
                transform.localPosition = origin * pp + goalLocalPosition * (1f - pp);
                yield return null;
            }

            transform.localPosition = goalLocalPosition;
        }

        public static Vector3 convertPos(int index, int arrangeType, (int x, int y)[] arrangement, Vector3 origin, Vector3[] originOffset, Vector3 contentsOffset, float[] scale, (float x, float y)[] contentAreaMultiplier)
        {
            int num1 = index % arrangement[arrangeType].x;
            int num2 = index / arrangement[arrangeType].x;
            return origin + originOffset[arrangeType] + new Vector3(contentsOffset.x * scale[arrangeType] * contentAreaMultiplier[arrangeType].x * num1, contentsOffset.y * scale[arrangeType] * contentAreaMultiplier[arrangeType].y * num2, (float)(-(double)num2 * 0.0099999997764825821));
        }

        public static int GetDisplayType(int players)
        {
            if (players <= 15)
                return 0;
            return players > 18 ? 2 : 1;
        }

        public static bool hasFakeTasks(this PlayerControl player) {
            return player == Jester.jester || player == Jackal.jackal || player == Sidekick.sidekick || player == Arsonist.arsonist || player == Opportunist.opportunist || player == Vulture.vulture || Jackal.formerJackals.Any(x => x == player) || player == Moriarty.moriarty || player == Moriarty.formerMoriarty
                || (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) && !Madmate.hasTasks) ||
                (player == CreatedMadmate.createdMadmate && !CreatedMadmate.hasTasks) || player == Akujo.akujo || player == Kataomoi.kataomoi || player == PlagueDoctor.plagueDoctor || player == JekyllAndHyde.formerJekyllAndHyde || player == Cupid.cupid || (player == SchrodingersCat.schrodingersCat && !SchrodingersCat.hideRole)
                || player == Immoralist.immoralist || player == Doomsayer.doomsayer;
        }

        public static bool canBeErased(this PlayerControl player) {
            return player != Jackal.jackal && player != Sidekick.sidekick && !Jackal.formerJackals.Any(x => x == player);
        }

        public static bool shouldShowGhostInfo() {
            return (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.IsDead && !(PlayerControl.LocalPlayer == Busker.busker && Busker.pseudocideFlag) && TORMapOptions.ghostsSeeInformation) || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended;
        }

        public static void clearAllTasks(this PlayerControl player) {
            if (player == null) return;
            foreach (var playerTask in player.myTasks.GetFastEnumerator())
            {
                playerTask.OnRemove();
                UnityEngine.Object.Destroy(playerTask.gameObject);
            }
            player.myTasks.Clear();

            if (player.Data != null && player.Data.Tasks != null)
                player.Data.Tasks.Clear();
        }

        public static bool isMira()
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 1;
        }

        public static bool isAirship()
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 4;
        }
        public static bool isSkeld()
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 0;
        }
        public static bool isPolus()
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 2;
        }

        public static bool isFungle()
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId == 5;
        }

        public static bool MushroomSabotageActive()
        {
            return PlayerControl.LocalPlayer.myTasks.ToArray().Any((x) => x.TaskType == TaskTypes.MushroomMixupSabotage);
        }

        public static bool ShowButtons
        {
            get
            {
                return !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                !MeetingHud.Instance &&
                !ExileController.Instance;
            }
        }

        public static List<string> removeLineFeed(string line)
        {
            var lines = line.Split("\n", StringSplitOptions.None);
            return lines.ToList();
        }

        static public TitleShower GetTitleShower(this PlayerControl player)
        {
            if (player.TryGetComponent<TitleShower>(out var result))
                return result;
            else
            {
                return player?.gameObject.AddComponent<TitleShower>();
            }
        }

        public static string HeadLower(this string text) => char.ToLower(text[0]) + text[1..];
        public static string HeadUpper(this string text) => char.ToUpper(text[0]) + text[1..];

        public static IEnumerator CoGush(this PlayerControl player)
        {

            player.MyPhysics.body.velocity = Vector2.zero;
            if (player.AmOwner) player.MyPhysics.inputHandler.enabled = true;
            player.moveable = false;
            player.MyPhysics.myPlayer.Visible = true;
            player.cosmetics.AnimateSkinExitVent();

            yield return player.MyPhysics.Animations.CoPlayExitVentAnimation();

            player.cosmetics.AnimateSkinIdle();
            player.MyPhysics.Animations.PlayIdleAnimation();
            player.moveable = true;
            player.currentRoleAnimations.ForEach((Action<RoleEffectAnimation>)((an) => an.ToggleRenderer(true)));
            if (player.AmOwner) player.MyPhysics.inputHandler.enabled = false;
        }

        public static void generateNormalTasks(this PlayerControl player)
        {
            if (player == null) return;
            var tasks = new Il2CppSystem.Collections.Generic.List<byte>();
            var hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();

            List<byte> taskTypeIds = new();
            var commonTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in ShipStatus.Instance.CommonTasks) {
                if (ShipStatusPatch.commonTasks.Any(x => x == task.TaskType)) {
                    commonTasks.Add(task);
                    TheOtherRolesPlugin.Logger.LogMessage($"Generated {task.TaskType.ToString()} for player {player.Data.PlayerName}");
                }
            }

            int start = 0;
            MapUtilities.CachedShipStatus.AddTasksFromList(ref start, ShipStatusPatch.commonTasks.Count, tasks, hashSet, commonTasks);

            taskTypeIds.AddRange(tasks.ToArray().ToList());
            var option = GameOptionsManager.Instance.currentNormalGameOptions;
            taskTypeIds.AddRange(generateTasks(0, option.NumShortTasks, option.NumLongTasks));

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedSetTasks, Hazel.SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            writer.WriteBytesAndSize(taskTypeIds.ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.uncheckedSetTasks(player.PlayerId, taskTypeIds.ToArray());
        }

        public static void HandleRoleFlashOnDeath(PlayerControl target)
        {
            if (Noisemaker.noisemaker != null && Noisemaker.target != null && Noisemaker.target == target)
            {
                if ((Noisemaker.soundTarget == Noisemaker.SoundTarget.Noisemaker && PlayerControl.LocalPlayer == Noisemaker.noisemaker) ||
                    (Noisemaker.soundTarget == Noisemaker.SoundTarget.Crewmates && !isNeutral(PlayerControl.LocalPlayer) && !PlayerControl.LocalPlayer.Data.Role.IsImpostor) ||
                    (Noisemaker.soundTarget == Noisemaker.SoundTarget.Everyone))
                { InstantiateNoisemakerArrow(target.transform.localPosition, true).arrow.SetDuration(Noisemaker.duration); }
                Noisemaker.target = null;
                if (PlayerControl.LocalPlayer == Noisemaker.noisemaker)
                {
                    _ = new StaticAchievementToken("noisemaker.common1");
                    Noisemaker.acTokenChallenge.Value++;
                }
            }
            if (Immoralist.immoralist != null && PlayerControl.LocalPlayer == Immoralist.immoralist && !PlayerControl.LocalPlayer.Data.IsDead)
                showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));

            // Seer show flash and add dead player position
            if (Seer.seer != null && (PlayerControl.LocalPlayer == Seer.seer || shouldShowGhostInfo()) && !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1)
            {
                showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: ModTranslation.getString("seerInfo"));
                if (PlayerControl.LocalPlayer == Seer.seer)
                {
                    _ = new StaticAchievementToken("seer.common1");
                    Seer.acTokenChallenge.Value.flash++;
                }
            }
            if (Seer.deadBodyPositions != null) Seer.deadBodyPositions.Add(target.transform.position);

            // Tracker store body positions
            if (Tracker.deadBodyPositions != null) Tracker.deadBodyPositions.Add(target.transform.position);

            // VIP Modifier
            if (Vip.vip.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
            {
                Color color = Color.yellow;
                if (Vip.showColor)
                {
                    color = Color.white;
                    if (target.Data.Role.IsImpostor) color = Color.red;
                    else if (isNeutral(target)) color = Color.blue;
                }
                showFlash(color, 1.5f);
            }

            if (PlagueDoctor.plagueDoctor != null && (PlagueDoctor.canWinDead || !PlagueDoctor.plagueDoctor.Data.IsDead)) PlagueDoctor.checkWinStatus();
        }

        public static void HandleRolesOnErase(PlayerControl player)
        {
            // Specify shifting onto Lawyer/Akujo
            if (player == Lawyer.lawyer && Lawyer.target != null)
            {
                Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo != null) playerInfo.text = "";
            }
            else if (player == Akujo.akujo)
            {
                if (Akujo.honmei != null)
                {
                    Transform playerInfoTransform = Akujo.honmei.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                    if (playerInfo != null) playerInfo.text = "";
                }
                if (Akujo.keeps != null)
                {
                    foreach (PlayerControl playerControl in Akujo.keeps)
                    {
                        Transform playerInfoTransform = playerControl.cosmetics.nameText.transform.parent.FindChild("Info");
                        TMPro.TextMeshPro playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                        if (playerInfo != null) playerInfo.text = "";
                    }
                }
            }
        }

        public static Camera FindCamera(int cameraLayer) => Camera.allCameras.FirstOrDefault(c => (c.cullingMask & (1 << cameraLayer)) != 0);

        public static Vector3 WorldToScreenPoint(Vector3 worldPos, int cameraLayer)
        {
            return FindCamera(cameraLayer)?.WorldToScreenPoint(worldPos) ?? Vector3.zero;
        }

        public static Vector3 ScreenToWorldPoint(Vector3 screenPos, int cameraLayer)
        {
            return FindCamera(cameraLayer)?.ScreenToWorldPoint(screenPos) ?? Vector3.zero;
        }

        public static string GetClipboardString()
        {
            uint type = 0;
            if (ClipboardHelper.IsClipboardFormatAvailable(1U)) { type = 1U; Debug.Log("ASCII"); }
            if (ClipboardHelper.IsClipboardFormatAvailable(13U)) { type = 13U; Debug.Log("UNICODE"); }
            if (type == 0) return "";

            string result;
            try
            {
                if (!ClipboardHelper.OpenClipboard(IntPtr.Zero))
                {
                    result = "";
                }
                else
                {

                    IntPtr clipboardData = ClipboardHelper.GetClipboardData(type);
                    if (clipboardData == IntPtr.Zero)
                        result = "";
                    else
                    {
                        IntPtr intPtr = IntPtr.Zero;
                        try
                        {
                            intPtr = ClipboardHelper.GlobalLock(clipboardData);
                            int len = ClipboardHelper.GlobalSize(clipboardData);

                            if (type == 1U)
                                result = Marshal.PtrToStringAnsi(clipboardData, len);
                            else
                            {
                                result = Marshal.PtrToStringUni(clipboardData) ?? "";
                            }
                        }
                        finally
                        {
                            if (intPtr != IntPtr.Zero) ClipboardHelper.GlobalUnlock(intPtr);
                        }
                    }
                }
            }
            finally
            {
                ClipboardHelper.CloseClipboard();
            }
            return result;
        }

        static public SpriteRenderer CreateSharpBackground(Vector2 size, Color color, Transform transform)
        {
            var renderer = CreateObject<SpriteRenderer>("Background", transform, new Vector3(0, 0, 0.25f));
            return CreateSharpBackground(renderer, color, size);
        }

        static public SpriteRenderer CreateSharpBackground(SpriteRenderer renderer, Color color, Vector2 size)
        {
            renderer.sprite = loadSpriteFromResources("TheOtherRoles.Resources.StatisticsBackground.png", 100f);
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.tileMode = SpriteTileMode.Continuous;
            renderer.color = color;
            renderer.size = size;
            return renderer;
        }

        static public float GetKillCooldown(this PlayerControl player)
        {
            if (player == SerialKiller.serialKiller) return SerialKiller.killCooldown;
            if (player == SchrodingersCat.schrodingersCat) return SchrodingersCat.killCooldown;
            return GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        }

        static public IEnumerator Sequence(params IEnumerator[] enumerator)
        {
            foreach (var e in enumerator) yield return e;
        }

        public static int ComputeConstantHash(this string str)
        {
            const long MulPrime = 467;
            const int SurPrime = 9670057;

            long val = 0;
            foreach (char c in str)
            {
                val *= MulPrime;
                val += c;
                val %= SurPrime;
            }
            return (int)(val % SurPrime);
        }

        public static string ComputeConstantHashAsString(this string str)
        {
            var val = str.ComputeConstantHash();
            StringBuilder builder = new();
            while (val > 0)
            {
                builder.Append((char)('a' + (val % 26)));
                val /= 26;
            }
            return builder.ToString();
        }

        public static void ModShow(this HideAndSeekDeathPopup __instance, PlayerControl player, int deathIndex)
        {
            __instance.nameplate.SetPlayer(player);
            Vector3 localPosition = __instance.transform.localPosition;
            localPosition.z -= deathIndex;
            __instance.transform.localPosition = localPosition;
            __instance.StartCoroutine(AnimateCoroutine(__instance));
        }

        private static IEnumerator AnimateCoroutine(HideAndSeekDeathPopup __instance)
        {
            HideAndSeekDeathPopup andSeekDeathPopup = __instance;
            DateTime startTime = DateTime.UtcNow;
            bool doNeedReduceSpeed = true;
            bool doNeedIncrease = true;
            while (true)
            {
                AnimatorStateInfo animatorStateInfo = andSeekDeathPopup.animator.GetCurrentAnimatorStateInfo(0);
                if (animatorStateInfo.IsName("Show"))
                {
                    if (doNeedReduceSpeed && DateTime.UtcNow.Subtract(startTime).TotalSeconds >= 0.5)
                    {
                        __instance.animator.speed *= 0.15f;
                        doNeedReduceSpeed = false;
                    }
                    if (doNeedIncrease && DateTime.UtcNow.Subtract(startTime).TotalSeconds >= 6.8f)
                    {
                        __instance.animator.speed *= 6.66f;
                        doNeedIncrease = false;
                    }
                    yield return null;
                }
                else
                    break;
            }
            UnityEngine.Object.Destroy(andSeekDeathPopup.gameObject);
        }

        static public IEnumerator Action(Action action)
        {
            action.Invoke();
            yield break;
        }

        public static PassiveButton SetUpButton(this GameObject gameObject, bool withSound = false, SpriteRenderer buttonRenderer = null, Color? defaultColor = null, Color? selectedColor = null)
        {
            var button = gameObject.AddComponent<PassiveButton>();
            button.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            button.OnMouseOut = new UnityEngine.Events.UnityEvent();
            button.OnMouseOver = new UnityEngine.Events.UnityEvent();

            if (withSound)
            {
                button.OnClick.AddListener((Action)(() => SoundManager.Instance.PlaySound(VanillaAsset.SelectClip, false, 0.8f)));
                button.OnMouseOver.AddListener((Action)(() => SoundManager.Instance.PlaySound(VanillaAsset.HoverClip, false, 0.8f)));
            }
            if (buttonRenderer != null)
            {
                button.OnMouseOut.AddListener((Action)(() => buttonRenderer!.color = defaultColor ?? Color.white));
                button.OnMouseOver.AddListener((Action)(() => buttonRenderer!.color = selectedColor ?? Color.green));
            }

            if (buttonRenderer != null) buttonRenderer.color = defaultColor ?? Color.white;

            return button;
        }

        static public bool AnyNonTriggersBetween(Vector2 pos1, Vector2 pos2, out Vector2 vector, int? layerMask = null)
        {
            layerMask ??= Constants.ShipAndAllObjectsMask;
            vector = pos2 - pos1;
            return PhysicsHelpers.AnyNonTriggersBetween(pos1, vector.normalized, vector.magnitude, layerMask!.Value);
        }

        public static bool destroyGameObjects(GameObject root, DestroyInfo[] excludeInfos = null, GameObject obj = null)
        {
            if (obj == null)
                obj = root;

            DestroyInfo findInfo = null;
            if (excludeInfos != null)
                findInfo = Array.Find(excludeInfos, (info) => info.searchName == obj.name);
            if (obj != root && findInfo == null)
            {
                UnityEngine.Object.DestroyImmediate(obj);
                return true;
            }

            if (findInfo == null || findInfo.isDestroyChild)
            {
                for (int i = 0; i < obj.transform.GetChildCount(); ++i)
                {
                    if (destroyGameObjects(root, excludeInfos, obj.transform.GetChild(i).gameObject))
                        --i;
                }
            }
            return false;
        }

        public class DestroyInfo
        {
            public DestroyInfo(string searchName, bool isFindChild = true)
            {
                this.searchName = searchName;
                this.isDestroyChild = isFindChild;
            }

            public string searchName = "";
            public bool isDestroyChild = true;
        }

        public static bool hideGameObjects(GameObject root, DestroyInfo[] excludeInfos = null, GameObject obj = null)
        {
            if (obj == null)
                obj = root;

            DestroyInfo findInfo = null;
            if (excludeInfos != null)
                findInfo = Array.Find(excludeInfos, (info) => info.searchName == obj.name);
            if (obj != root && findInfo == null)
            {
                obj.SetActive(false);
                return true;
            }

            if (findInfo == null || findInfo.isDestroyChild)
            {
                for (int i = 0; i < obj.transform.GetChildCount(); ++i)
                {
                    if (destroyGameObjects(root, excludeInfos, obj.transform.GetChild(i).gameObject))
                        --i;
                }
            }
            return false;
        }

        public static PlayerControl getPlayerById(byte playerId)
        {
            return PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(p => p.PlayerId == playerId).FirstOrDefault();
        }

        public static MeshFilter CreateRectMesh(this MeshFilter filter, Vector2 size, Vector3? center = null)
        {
            center ??= Vector3.zero;

            var mesh = filter.mesh;

            float x = size.x * 0.5f;
            float y = size.y * 0.5f;
            mesh.SetVertices(new Vector3[] {
                new Vector3(-x, -y) + center.Value,
                new Vector3(x, -y) + center.Value,
                new Vector3(-x, y) + center.Value,
                new Vector3(x, y) + center.Value});
            mesh.SetTriangles(new int[] { 0, 2, 1, 2, 3, 1 }, 0);
            mesh.SetUVs(0, new Vector2[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) });
            var color = new Color32(255, 255, 255, 255);
            mesh.SetColors(new Color32[] { color, color, color, color });

            return filter;
        }

        public static RenderTexture SetCameraRenderTexture(this Camera camera, int textureX, int textureY)
        {
            if (camera.targetTexture) GameObject.Destroy(camera.targetTexture);
            camera.targetTexture = new RenderTexture(textureX, textureY, 32, RenderTextureFormat.ARGB32);

            return camera.targetTexture;
        }

        public static (MeshRenderer renderer, MeshFilter filter) CreateMeshRenderer(string objName, Transform parent, Vector3 localPosition, int? layer, Color? color = null)
        {
            var meshFilter = CreateObject<MeshFilter>("mesh", parent, localPosition, layer);
            var meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find(color.HasValue ? "Unlit/Color" : "Unlit/Texture"));
            if (color.HasValue) meshRenderer.sharedMaterial.color = color.Value;
            meshFilter.mesh = new Mesh();

            return (meshRenderer, meshFilter);
        }

        public static void setSemiTransparent(this PoolablePlayer player, bool value) {
            float alpha = value ? 0.25f : 1f;
            foreach (SpriteRenderer r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
                r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
            player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, alpha);
        }

        public static string GetString(this TranslationController t, StringNames key, params Il2CppSystem.Object[] parts) {
            return t.GetString(key, parts);
        }

        public static string cs(Color c, string s) {
            return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
        }

        public static int lineCount(string text) {
            return text.Count(c => c == '\n');
        }

        private static byte ToByte(float f) {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public struct TextFeatures
        {
            public float fontSizeMultiplier;
            public float lineSpacingOffset;
            public float heightMultiplier;
            public float topMarginMultiplier;
            public float scrollSpeedMultiplier;
        }

        public static TextFeatures AnalyzeTextFeatures(string text)
        {
            TextFeatures features = new()
            {
                fontSizeMultiplier = 1f,
                lineSpacingOffset = 0f,
                heightMultiplier = 1f,
                topMarginMultiplier = 1f,
                scrollSpeedMultiplier = 0.08f
            };

            bool hasCJK = System.Text.RegularExpressions.Regex.IsMatch(
                text, @"[\p{IsHiragana}\p{IsKatakana}\p{IsCJKUnifiedIdeographs}]");

            bool hasComplexScript = System.Text.RegularExpressions.Regex.IsMatch(
                text, @"[\p{IsArabic}\p{IsThai}]");

            if (hasCJK)
            {
                features.fontSizeMultiplier = 0.85f;
                features.lineSpacingOffset = -20f;
                features.heightMultiplier = 1.15f;
                features.scrollSpeedMultiplier = 0.05f;

                bool hasKana = text.Any(c => c >= '\u3040' && c <= '\u30FF');
                if (hasKana)
                {
                    features.topMarginMultiplier = 0.8f;
                    features.heightMultiplier = 1.25f;
                }
            }
            else if (hasComplexScript)
            {
                features.fontSizeMultiplier = 0.9f;
                features.lineSpacingOffset = -10f;
                features.heightMultiplier = 1.2f;
                features.scrollSpeedMultiplier = 0.06f;
            }

            float avgLineLength = (float)text.Split('\n').Average(line => line.Length);
            if (avgLineLength > 60)
            {
                features.fontSizeMultiplier *= 0.95f;
                features.lineSpacingOffset -= 5f;
            }

            return features;
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie) {
            tie = true;
            KeyValuePair<byte, int> result = new(byte.MaxValue, int.MinValue);
            foreach (KeyValuePair<byte, int> keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }
            }
            return result;
        }

        public static TMPro.TextMeshPro getFirst(this TMPro.TextMeshPro[] text)
        {
            if (text == null) return null;
            foreach (var self in text)
                if (self.text == "") return self;
            return text[0];
        }

        public static int totalCounts(this TMPro.TextMeshPro[] text)
        {
            if (text == null) return 0;
            int count = 0;
            foreach (var self in text)
                if (self.text != "") count++;
            return count;
        }

        public static bool sabotageActive()
        {
            var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
            return sabSystem.AnyActive;
        }

        public static float sabotageTimer()
        {
            var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
            return sabSystem.Timer;
        }
        public static bool canUseSabotage()
        {
            var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
            IActivatable doors = null;
            if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out ISystemType systemType))
            {
                doors = systemType.CastFast<IActivatable>();
            }
            return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
        }

        public static List<byte> generateTasks(int numCommon, int numShort, int numLong)
        {
            if (numCommon + numShort + numLong <= 0)
            {
                numShort = 1;
            }

            var tasks = new Il2CppSystem.Collections.Generic.List<byte>();
            var hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();

            var commonTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in MapUtilities.CachedShipStatus.CommonTasks.OrderBy(x => rnd.Next())) commonTasks.Add(task);

            var shortTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in MapUtilities.CachedShipStatus.ShortTasks.OrderBy(x => rnd.Next())) shortTasks.Add(task);

            var longTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
            foreach (var task in MapUtilities.CachedShipStatus.LongTasks.OrderBy(x => rnd.Next())) longTasks.Add(task);

            int start = 0;
            MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasks);

            start = 0;
            MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasks);

            start = 0;
            MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasks);

            return tasks.ToArray().ToList();
        }

        public static void generateAndAssignTasks(this PlayerControl player, int numCommon, int numShort, int numLong)
        {
            if (player == null) return;

            List<byte> taskTypeIds = generateTasks(numCommon, numShort, numLong);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedSetTasks, Hazel.SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            writer.WriteBytesAndSize(taskTypeIds.ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.uncheckedSetTasks(player.PlayerId, taskTypeIds.ToArray());
        }

        public static bool isChinese()
        {
            try
            {
                var name = CultureInfo.CurrentUICulture.Name;
                if (name.StartsWith("zh")) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static T FindAsset<T>(string name) where T : Il2CppObjectBase
        {
            foreach (var asset in UnityEngine.Object.FindObjectsOfTypeIncludingAssets(Il2CppType.Of<T>()))
            {
                if (asset.name == name) return asset.Cast<T>();
            }
            return null;
        }

        static public float Distance(this Vector3 myVec, Vector3 vector)
        {
            var vec = myVec - vector;
            vec.z = 0;
            return vec.magnitude;
        }

        public static LineRenderer SetUpLineRenderer(string objName, Transform parent, Vector3 localPosition, int? layer = null, float width = 0.2f)
        {
            var line = CreateObject<LineRenderer>(objName, parent, localPosition, layer);
            line.material.shader = Shader.Find("Sprites/Default");
            line.SetColors(Color.clear, Color.clear);
            line.positionCount = 2;
            line.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            line.useWorldSpace = false;
            line.SetWidth(width, width);
            return line;
        }

        static public float Distance(this Vector2 myVec, Vector2 vector) => (myVec - vector).magnitude;

        public static bool hidePlayerName(PlayerControl source, PlayerControl target) {
            if (Camouflager.camouflageTimer > 0f || MushroomSabotageActive()) return true; // No names are visible
            if (!source.Data.Role.IsImpostor && Ninja.isStealthed(target) && Ninja.ninja == target) return true; // Hide Ninja nametags from non-impostors
            if (Sprinter.sprinting && Sprinter.sprinter == target && source != Sprinter.sprinter) return true; // Hide Sprinter nametags
            if (Fox.stealthed && Fox.fox == target && source != Fox.fox) return true; // Hide Fox nametags
            if (source != target && Kataomoi.isStalking(target)) return true; // Hide Kataomoi nametags
            if (Patches.SurveillanceMinigamePatch.nightVisionIsActive) return true;
            //else if (Assassin.isInvisble && Assassin.assassin == target) return true;
            else if (!TORMapOptions.hidePlayerNames) return false; // All names are visible
            else if (source == null || target == null) return true;
            else if (source == target) return false; // Player sees his own name
            else if (source.Data.Role.IsImpostor && (target.Data.Role.IsImpostor || target == Spy.spy || (target == Sidekick.sidekick && Sidekick.wasTeamRed) || (target == Jackal.jackal && Jackal.wasTeamRed))) return false; // Members of team Impostors see the names of Impostors/Spies
            else if ((source == Lovers.lover1 || source == Lovers.lover2) && (target == Lovers.lover1 || target == Lovers.lover2)) return false; // Members of team Lovers see the names of each other
            else if (Cupid.lovers1 != null && Cupid.lovers2 != null && (source == Cupid.lovers1 || source == Cupid.lovers2) && (target == Cupid.lovers1 || target == Cupid.lovers2)) return false; // Members of team Cupid Lovers see the names of each other
            else if ((source == Jackal.jackal || source == Sidekick.sidekick) && (target == Jackal.jackal || target == Sidekick.sidekick || target == Jackal.fakeSidekick)) return false; // Members of team Jackal see the names of each other
            else if (Deputy.knowsSheriff && (source == Sheriff.sheriff || source == Deputy.deputy) && (target == Sheriff.sheriff || target == Deputy.deputy)) return false; // Sheriff & Deputy see the names of each other
            else if ((source == Fox.fox || source == Immoralist.immoralist) && (target == Fox.fox || target == Immoralist.immoralist)) return false; // Members of team Fox see the names of each other
            return true;
        }

        public static void setDefaultLook(this PlayerControl target, bool enforceNightVisionUpdate = true) {
            if (MushroomSabotageActive())
            {
                var instance = ShipStatus.Instance.CastFast<FungleShipStatus>().specialSabotage;
                MushroomMixupSabotageSystem.CondensedOutfit condensedOutfit = instance.currentMixups[target.PlayerId];
                NetworkedPlayerInfo.PlayerOutfit playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
                target.MixUpOutfit(playerOutfit);
            }
            else
                target.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId, enforceNightVisionUpdate);
        }

        public static void setLook(this PlayerControl target, String playerName, int colorId, string hatId, string visorId, string skinId, string petId, bool enforceNightVisionUpdate = true) {
            target.RawSetColor(colorId);
            target.RawSetVisor(visorId, colorId);
            target.RawSetHat(hatId, colorId);
            target.RawSetName(hidePlayerName(PlayerControl.LocalPlayer, target) ? "" : playerName);


            SkinViewData nextSkin = null;
            try { nextSkin = ShipStatus.Instance.CosmeticsCache.GetSkin(skinId); } catch { return; };

            PlayerPhysics playerPhysics = target.MyPhysics;
            AnimationClip clip = null;
            var spriteAnim = playerPhysics.myPlayer.cosmetics.skin.animator;
            var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();


            if (currentPhysicsAnim == playerPhysics.Animations.group.RunAnim) clip = nextSkin.RunAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.SpawnAnim) clip = nextSkin.SpawnAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.EnterVentAnim) clip = nextSkin.EnterVentAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.ExitVentAnim) clip = nextSkin.ExitVentAnim;
            else if (currentPhysicsAnim == playerPhysics.Animations.group.IdleAnim) clip = nextSkin.IdleAnim;
            else clip = nextSkin.IdleAnim;
            float progress = playerPhysics.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            playerPhysics.myPlayer.cosmetics.skin.skin = nextSkin;
            playerPhysics.myPlayer.cosmetics.skin.UpdateMaterial();

            spriteAnim.Play(clip, 1f);
            spriteAnim.m_animator.Play("a", 0, progress % 1);
            spriteAnim.m_animator.Update(0f);

            target.RawSetPet(petId, colorId);

            if (enforceNightVisionUpdate) Patches.SurveillanceMinigamePatch.enforceNightVision(target);
            Chameleon.update();  // so that morphling and camo wont make the chameleons visible
        }

        public static void showFlash(Color color, float duration = 1f, string message = "") {
            if (FastDestroyableSingleton<HudManager>.Instance == null || FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
            var renderer = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.FullScreen, HudManager.Instance.transform);
            renderer.gameObject.SetActive(true);
            renderer.enabled = true;

            // Message Text
            TMPro.TextMeshPro messageText = GameObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
            messageText.text = message;
            messageText.enableWordWrapping = false;
            messageText.transform.localScale = Vector3.one * 0.5f;
            messageText.transform.localPosition += new Vector3(0f, 2f, -69f);
            messageText.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) => {

                if (p < 0.5) {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * 0.75f));
                } else {
                    if (renderer != null)
                        renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * 0.75f));
                }
                if (p == 1f && renderer != null) renderer.enabled = false;
                if (p == 1f) messageText.gameObject.Destroy();
            })));
        }

        public static void ModRevive(this PlayerControl player, bool resetRoleIfGhost = true)
        {
            player.Data.IsDead = false;
            player.gameObject.layer = LayerMask.NameToLayer("Players");
            player.MyPhysics.ResetMoveState();
            player.cosmetics.SetPetSource(player);
            player.cosmetics.SetNameMask(true);
            if (player.AmOwner)
            {
                DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(true);
                DestroyableSingleton<HudManager>.Instance.KillButton.ToggleVisible(player.Data.Role.IsImpostor);
                DestroyableSingleton<HudManager>.Instance.AdminButton.ToggleVisible(player.Data.Role.IsImpostor);
                DestroyableSingleton<HudManager>.Instance.SabotageButton.ToggleVisible(player.Data.Role.IsImpostor);
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.ToggleVisible(player.Data.Role.IsImpostor);
                DestroyableSingleton<HudManager>.Instance.Chat.ForceClosed();
                DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(false);
            }
            if (!resetRoleIfGhost || !AmongUsClient.Instance.AmHost || !RoleManager.IsGhostRole(player.Data.Role.Role))
                return;
            player.RpcSetRole(RoleTypes.Crewmate);
        }

        public static GameObject CreateObject(string objName, Transform parent, Vector3 localPosition, int? layer = null)
        {
            var obj = new GameObject(objName);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPosition;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            if (layer.HasValue) obj.layer = layer.Value;
            else if (parent != null) obj.layer = parent.gameObject.layer;
            return obj;
        }

        public static T CreateObject<T>(string objName, Transform parent, Vector3 localPosition, int? layer = null) where T : Component
        {
            return CreateObject(objName, parent, localPosition, layer).AddComponent<T>();
        }

        public static void SetModText(this TextTranslatorTMP text, string translationKey)
        {
            text.TargetText = (StringNames)short.MaxValue;
            text.defaultStr = translationKey;
        }

        public static R GetRolePrefab<R>() where R : RoleBehaviour
        {
            foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
            {
                R r = role.TryCast<R>();
                if (r != null)
                {
                    return r;
                }
            }
            return null;
        }

        public static (GameObject obj, NoisemakerArrow arrow) InstantiateNoisemakerArrow(Vector2 targetPos, bool withSound = false)
        {
            var noisemaker = GetRolePrefab<NoisemakerRole>();
            if (noisemaker != null)
            {
                if (withSound && Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlayDynamicSound("NoisemakerAlert", noisemaker.deathSound, false, (DynamicSound.GetDynamicsFunction)((source, dt) =>
                    {
                        if (!PlayerControl.LocalPlayer)
                        {
                            source.volume = 0f;
                            return;
                        }
                        source.volume = 1f;
                        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                        source.volume = SoundManager.GetSoundVolume(targetPos, truePosition, 7f, 50f, 0.5f);
                    }), SoundManager.Instance.SfxChannel);
                    VibrationManager.Vibrate(1f, PlayerControl.LocalPlayer.GetTruePosition(), 7f, 1.2f, VibrationManager.VibrationFalloff.None, null, false);
                }
                GameObject gameObject = GameObject.Instantiate<GameObject>(noisemaker.deathArrowPrefab, Vector3.zero, Quaternion.identity);
                var deathArrow = gameObject.GetComponent<NoisemakerArrow>();
                deathArrow.SetDuration(3f);
                deathArrow.gameObject.SetActive(true);
                deathArrow.target = targetPos;

                return (gameObject, deathArrow);
            }

            return (null, null)!;
        }

        public static void DoTransitionFade(this TransitionFade transitionFade, GameObject transitionFrom, GameObject transitionTo, Action onTransition, Action callback)
        {
            if (transitionTo) transitionTo!.SetActive(false);

            IEnumerator Coroutine()
            {
                yield return Effects.ColorFade(transitionFade.overlay, Color.clear, Color.black, 0.1f);
                if (transitionFrom && transitionFrom!.gameObject) transitionFrom.gameObject.SetActive(false);
                if (transitionTo && transitionTo!.gameObject) if (transitionTo != null) transitionTo.gameObject.SetActive(true);
                onTransition.Invoke();
                yield return null;
                yield return Effects.ColorFade(transitionFade.overlay, Color.black, Color.clear, 0.1f);
                callback.Invoke();
                yield break;
            }

            transitionFade.StartCoroutine(Coroutine().WrapToIl2Cpp());
        }

        public static void TryAdd<T>(this List<T> list, T param) where T : class
        {
            if (list.Contains(param) || param == null) return;
            list.Add(param);
        }

        public static List<PlayerControl> GetAllRelatedPlayers(this PlayerControl player)
        {
            List<PlayerControl> relatedPlayers = new();
            if (player.getPartner() != null && Lovers.bothDie) relatedPlayers.TryAdd(player.getPartner());
            if (player.getCupidLover() != null) {
                relatedPlayers.TryAdd(player.getCupidLover());
                if (Cupid.cupid != null) relatedPlayers.TryAdd(Cupid.cupid);
            }
            if ((player == Akujo.akujo && Akujo.honmei != null) || (player == Akujo.honmei && Akujo.akujo != null)) relatedPlayers.TryAdd(player == Akujo.akujo ?
                Akujo.honmei : Akujo.akujo);
            if ((player == BomberA.bomberA || player == BomberB.bomberB) && BomberA.ifOneDiesBothDie) relatedPlayers.TryAdd(player == BomberA.bomberA ? BomberB.bomberB : BomberA.bomberA);
            if ((player == MimicK.mimicK || player == MimicA.mimicA) && MimicK.ifOneDiesBothDie) relatedPlayers.TryAdd(player == MimicK.mimicK ? MimicA.mimicA : MimicK.mimicK);
            if (player == Fox.fox && Immoralist.immoralist != null) relatedPlayers.TryAdd(Immoralist.immoralist);
            if (player == Kataomoi.target && Kataomoi.kataomoi != null) relatedPlayers.TryAdd(Kataomoi.kataomoi);
            relatedPlayers.RemoveAll(x => x == player);
            return relatedPlayers;
        }

        public static void setInvisible(PlayerControl player, Color color, float alpha)
        {
            if (player.cosmetics.currentBodySprite.BodySprite != null)
                player.cosmetics.currentBodySprite.BodySprite.color = color;

            if (player.cosmetics.skin?.layer != null)
                player.cosmetics.skin.layer.color = color;

            if (player.cosmetics.hat != null)
            {
                player.cosmetics.hat.FrontLayer.color = color;
                player.cosmetics.hat.BackLayer.color = color;
            }

            if (player.cosmetics.currentPet != null)
                player.cosmetics.currentPet.SetAlpha(alpha);

            if (player.cosmetics.visor != null)
                player.cosmetics.visor.Image.color = color;

            if (player.cosmetics.colorBlindText != null)
                player.cosmetics.colorBlindText.color = color;

            if (player.cosmetics.PettingHand != null)
                player.cosmetics.PettingHand.SetAlpha(alpha);
        }

        public static PlainShipRoom getPlainShipRoom(PlayerControl p)
        {
            PlainShipRoom[] array = null;
            Il2CppReferenceArray<Collider2D> buffer = new Collider2D[10];
            ContactFilter2D filter = default;
            filter.layerMask = Constants.PlayersOnlyMask;
            filter.useLayerMask = true;
            filter.useTriggers = false;
            array = MapUtilities.CachedShipStatus?.AllRooms;
            if (array == null) return null;
            foreach (PlainShipRoom plainShipRoom in array)
            {
                if (plainShipRoom.roomArea)
                {
                    int hitCount = plainShipRoom.roomArea.OverlapCollider(filter, buffer);
                    if (hitCount == 0) continue;
                    for (int i = 0; i < hitCount; i++)
                    {
                        if (buffer[i]?.gameObject == p.gameObject)
                        {
                            return plainShipRoom;
                        }
                    }
                }
            }
            return null;
        }

        public static Vector3 AsVector3(this Vector2 vec, float z)
        {
            Vector3 result = vec;
            result.z = z;
            return result;
        }

        public static bool roleCanUseSabotage(this PlayerControl player) {
            bool roleCouldUse = false;
            if (Madmate.madmate.Any(x => x.PlayerId == player?.PlayerId) && Madmate.canSabotage)
                roleCouldUse = true;
            else if (CreatedMadmate.createdMadmate != null && player == CreatedMadmate.createdMadmate && CreatedMadmate.canSabotage)
                roleCouldUse = true;
            else if (Janitor.janitor != null && Janitor.janitor == player)
                roleCouldUse = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == player && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                roleCouldUse = false;
            else if (player?.Data?.Role?.IsImpostor == true)
                roleCouldUse = true;
            return roleCouldUse;
        }

        public static bool roleCanUseVents(this PlayerControl player) {
            bool roleCouldUse = false;            
            if (Engineer.engineer != null && Engineer.engineer == player)
                roleCouldUse = true;
            else if (Jackal.canUseVents && Jackal.jackal != null && Jackal.jackal == player)
                roleCouldUse = true;
            else if (Sidekick.canUseVents && Sidekick.sidekick != null && Sidekick.sidekick == player)
                roleCouldUse = true;
            else if (Spy.canEnterVents && Spy.spy != null && Spy.spy == player)
                roleCouldUse = true;            
            else if (Vulture.canUseVents && Vulture.vulture != null && Vulture.vulture == player)
                roleCouldUse = true;
            else if (Madmate.canVent && Madmate.madmate.Any(x => x.PlayerId == player.PlayerId))
                roleCouldUse = true;
            else if (CreatedMadmate.canEnterVents && CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate == player)
                roleCouldUse = true;
            else if (Moriarty.moriarty != null && Moriarty.moriarty == player)
                roleCouldUse = true;
            else if (JekyllAndHyde.jekyllAndHyde != null && !JekyllAndHyde.isJekyll() && JekyllAndHyde.jekyllAndHyde == player)
                roleCouldUse = true;
            else if (Jester.jester != null && Jester.canUseVents && Jester.jester == player)
                roleCouldUse = true;
            else if (Thief.canUseVents && Thief.thief != null && Thief.thief == player)
                roleCouldUse = true;
            else if (SchrodingersCat.schrodingersCat != null && SchrodingersCat.schrodingersCat == player && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate)
                roleCouldUse = true;
            else if (player.Data?.Role != null && player.Data.Role.CanVent)  {
                if (Janitor.janitor != null && Janitor.janitor == player)
                    roleCouldUse = false;
                else if (Mafioso.mafioso != null && Mafioso.mafioso == player && Godfather.godfather != null && !Godfather.godfather.Data.IsDead)
                    roleCouldUse = false;
                else if (Ninja.ninja != null && Ninja.ninja == player && Ninja.canUseVents == false)
                    roleCouldUse = false;
                else if (Undertaker.undertaker != null && Undertaker.undertaker == player && Undertaker.DraggedBody != null && Undertaker.disableVent)
                    roleCouldUse = false;
                else
                    roleCouldUse = true;
            }
            return roleCouldUse;
        }
        public static bool checkArmored(PlayerControl target, bool breakShield, bool showShield, bool additionalCondition = true)
        {
            if (target != null && Armored.armored != null && Armored.armored == target && !Armored.isBrokenArmor && additionalCondition)
            {
                if (breakShield)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BreakArmor, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.breakArmor();
                }
                if (showShield)
                {
                    target.ShowFailedMurder();
                }
                return true;
            }
            return false;
        }

        public static MurderAttemptResult checkMuderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false) {
            var targetRole = RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault();

            // Modified vanilla checks
            if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
            if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
            if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return MurderAttemptResult.PerformKill;

            // Handle first kill attempt
            if (TORMapOptions.shieldFirstKill && TORMapOptions.firstKillPlayer == target) return MurderAttemptResult.SuppressKill;

            // Handle blank shot
            if (!ignoreBlank && Pursuer.blankedList.Any(x => x.PlayerId == killer.PlayerId)) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBlanked, Hazel.SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                writer.Write((byte)0);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.setBlanked(killer.PlayerId, 0);

                return MurderAttemptResult.BlankKill;
            }

            // Block impostor shielded kill
            if (Medic.shielded != null && Medic.shielded == target) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shieldedMurderAttempt();
                SoundEffectsManager.play("fail");

                MessageWriter acWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UnlockMedicAcChallenge, SendOption.Reliable, -1);
                writer.Write(killer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(acWriter);
                RPCProcedure.unlockMedicAcChallenge(killer.PlayerId);
                return MurderAttemptResult.SuppressKill;
            }

            // Block impostor not fully grown mini kill
            else if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp()) {
                return MurderAttemptResult.SuppressKill;
            }

            // Block Time Master with time shield kill
            else if (TimeMaster.shieldActive && TimeMaster.timeMaster != null && TimeMaster.timeMaster == target) {
                if (!blockRewind) { // Only rewind the attempt was not called because a meeting startet 
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.timeMasterRewindTime();
                }
                return MurderAttemptResult.SuppressKill;
            }

            else if (Cupid.cupid != null && Cupid.shielded == target && !Cupid.cupid.Data.IsDead)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CupidSuicide, Hazel.SendOption.Reliable, -1);
                writer.Write(Cupid.cupid.PlayerId);
                writer.Write(true);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.cupidSuicide(Cupid.cupid.PlayerId, true);
                return MurderAttemptResult.BlankKill;
            }

            // Kill the killer if Veteran is on alert
            else if (Veteran.veteran != null && Veteran.alertActive && Veteran.veteran == target)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.UnlockVeteranAcChallenge, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.unlockVeteranAcChallenge();
                return MurderAttemptResult.ReverseKill;
            }

            // Thief if hit crew only kill if setting says so, but also kill the thief.
            else if (Thief.isFailedThiefKill(target, killer, targetRole)) {
                if (!checkArmored(killer, true, true))
                    Thief.suicideFlag = true;
                return MurderAttemptResult.SuppressKill;
            }

            // Block Armored with armor kill
            else if (checkArmored(target, true, killer == PlayerControl.LocalPlayer, Sheriff.sheriff == null || killer.PlayerId != Sheriff.sheriff.PlayerId || (isEvil(target) && Sheriff.canKillNeutrals) || isKiller(target)))
            {
                return MurderAttemptResult.BlankKill;
            }

            // Block hunted with time shield kill
            else if (Hunted.timeshieldActive.Contains(target.PlayerId)) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.HuntedRewindTime, Hazel.SendOption.Reliable, -1);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.huntedRewindTime(target.PlayerId);

                return MurderAttemptResult.SuppressKill;
            }

            if (TransportationToolPatches.isUsingTransportation(target) && !blockRewind && killer == Vampire.vampire)
            {
                return MurderAttemptResult.DelayVampireKill;
            }
            else if (TransportationToolPatches.isUsingTransportation(target))
                return MurderAttemptResult.SuppressKill;

            return MurderAttemptResult.PerformKill;
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedMurderPlayer, Hazel.SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(showAnimation ? Byte.MaxValue : 0);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.uncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? Byte.MaxValue : (byte)0);
        }

        public static MurderAttemptResult checkMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)  {
            // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
            // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible

            MurderAttemptResult murder = checkMuderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);
            if (murder == MurderAttemptResult.PerformKill) {
                MurderPlayer(killer, target, showAnimation);
            }
            else if (murder == MurderAttemptResult.DelayVampireKill)
            {
                HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) => {
                    if (!TransportationToolPatches.isUsingTransportation(target) && Vampire.bitten != null)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                        writer.Write(byte.MaxValue);
                        writer.Write(byte.MaxValue);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                        MurderPlayer(killer, target, showAnimation);
                    }
                })));
            }

            if (murder == MurderAttemptResult.ReverseKill)
            {
                checkMurderAttemptAndKill(target, killer, isMeetingStart);
            }
            return murder;            
        }
    
        public static void shareGameVersion() {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable, -1);
            writer.Write((byte)TheOtherRolesPlugin.Version.Major);
            writer.Write((byte)TheOtherRolesPlugin.Version.Minor);
            writer.Write((byte)TheOtherRolesPlugin.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? Patches.GameStartManagerPatch.timer : -1f);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TheOtherRolesPlugin.Version.Revision < 0 ? 0xFF : TheOtherRolesPlugin.Version.Revision));
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.versionHandshake(TheOtherRolesPlugin.Version.Major, TheOtherRolesPlugin.Version.Minor, TheOtherRolesPlugin.Version.Build, TheOtherRolesPlugin.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static List<PlayerControl> getKillerTeamMembers(PlayerControl player) {
            List<PlayerControl> team = new();
            foreach(PlayerControl p in PlayerControl.AllPlayerControls) {
                if (player.Data.Role.IsImpostor && p.Data.Role.IsImpostor && player.PlayerId != p.PlayerId && team.All(x => x.PlayerId != p.PlayerId)) team.Add(p);
                else if (player == Jackal.jackal && p == Sidekick.sidekick) team.Add(p); 
                else if (player == Sidekick.sidekick && p == Jackal.jackal) team.Add(p);
            }
            
            return team;
        }

        /// <summary>
        /// Check whether the action should be done. (i.e. Veteran alert)
        /// </summary>
        /// <param name="player">The action player</param>
        /// <param name="target">The target of the action player</param>
        /// <returns></returns>
        public static bool checkSuspendAction(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null) return false;
            if (Veteran.veteran != null && target == Veteran.veteran && Veteran.alertActive)
            {
                if (isEvil(player))
                {
                    _ = checkMuderAttempt(player, target);  // Gives the Veteran the achievement
                    checkMurderAttemptAndKill(target, player);
                    return true;
                }
            }
            return false;
        }

        public static bool isNeutral(PlayerControl player) {
            RoleInfo roleInfo = RoleInfo.getRoleInfoForPlayer(player, false, true).FirstOrDefault();
            if (roleInfo != null)
                return roleInfo.isNeutral;
            return false;
        }

        public static bool isKiller(PlayerControl player) {
            return player.Data.Role.IsImpostor || 
                (isNeutral(player) && 
                player != Jester.jester && 
                player != Arsonist.arsonist && 
                player != Vulture.vulture && 
                player != Lawyer.lawyer && 
                player != Pursuer.pursuer &&
                player != Opportunist.opportunist &&
                player != Akujo.akujo &&
                player != PlagueDoctor.plagueDoctor &&
                player != Cupid.cupid &&
                !(player == SchrodingersCat.schrodingersCat && !SchrodingersCat.hasTeam()));

        }

        public static bool isEvil(PlayerControl player) {
            return player.Data.Role.IsImpostor || isNeutral(player);
        }

        public static bool zoomOutStatus = false;
        public static void toggleZoom(bool reset=false) {
            float orthographicSize = reset || zoomOutStatus ? 3f : 12f;

            zoomOutStatus = !zoomOutStatus && !reset;
            Camera.main.orthographicSize = orthographicSize;
            foreach (var cam in Camera.allCameras) {
                if (cam != null && cam.gameObject.name == "UI Camera") cam.orthographicSize = orthographicSize;  // The UI is scaled too, else we cant click the buttons. Downside: map is super small.
            }

            var tzGO = GameObject.Find("TOGGLEZOOMBUTTON");
            if (tzGO != null)
            {
                var rend = tzGO.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                var rendActive = tzGO.transform.Find("Active").GetComponent<SpriteRenderer>();
                rend.sprite = zoomOutStatus ? loadSpriteFromResources("TheOtherRoles.Resources.Plus_Button.png", 100f) : loadSpriteFromResources("TheOtherRoles.Resources.Minus_Button.png", 100f);
                rendActive.sprite = zoomOutStatus ? loadSpriteFromResources("TheOtherRoles.Resources.Plus_ButtonActive.png", 100f) : loadSpriteFromResources("TheOtherRoles.Resources.Minus_ButtonActive.png", 100f);
                tzGO.transform.localScale = new Vector3(1.2f, 1.2f, 1f) * (zoomOutStatus ? 4 : 1);
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen); // This will move button positions to the correct position.
        }

        public static void AddModSettingsChangeMessage(this NotificationPopper popper, StringNames key, string value, string option, bool playSound = true)
        {
            string str = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.LobbyChangeSettingNotification, "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + option + "</font>", "<font=\"Barlow-Black SDF\" material=\"Barlow-Black Outline\">" + value + "</font>");
            popper.SettingsChangeMessageLogic(key, str, playSound);
        }

        private static long GetBuiltInTicks()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var builtin = assembly.GetType("Builtin");
            if (builtin == null) return 0;
            var field = builtin.GetField("CompileTime");
            if (field == null) return 0;
            var value = field.GetValue(null);
            if (value == null) return 0;
            return (long)value;
        }

        public static async Task checkBeta() {
            if (TheOtherRolesPlugin.betaDays > 0) {
                TheOtherRolesPlugin.Logger.LogMessage($"Beta check");
                var ticks = GetBuiltInTicks();
                var compileTime = new DateTime(ticks, DateTimeKind.Utc);  // This may show as an error, but it is not, compilation will work!
                TheOtherRolesPlugin.Logger.LogMessage($"Compiled at {compileTime.ToString(CultureInfo.InvariantCulture)}");
                DateTime? now;
                // Get time from the internet, so no-one can cheat it (so easily).
                try {
                    var client = new System.Net.Http.HttpClient();
                    using var response = await client.GetAsync("http://www.google.com/");
                    if (response.IsSuccessStatusCode)
                        now = response.Headers.Date?.UtcDateTime;
                    else {
                        TheOtherRolesPlugin.Logger.LogMessage($"Could not get time from server: {response.StatusCode}");
                        now = DateTime.UtcNow; //In case something goes wrong. 
                    }
                } catch (System.Net.Http.HttpRequestException) {
                    now = DateTime.UtcNow;
                }
                if ((now - compileTime)?.TotalDays > TheOtherRolesPlugin.betaDays) {
                    TheOtherRolesPlugin.Logger.LogMessage($"Beta expired!");
                    BepInExUpdater.MessageBoxTimeout(BepInExUpdater.GetForegroundWindow(), "BETA is expired. You cannot play this version anymore.", "The Other Roles Beta", 0,0, 10000);
                    Application.Quit();

                } else TheOtherRolesPlugin.Logger.LogMessage($"Beta will remain runnable for {TheOtherRolesPlugin.betaDays - (now - compileTime)?.TotalDays} days!");
            }
        }

        public static bool hasImpVision(NetworkedPlayerInfo player) {
            return player.Role.IsImpostor
                || (((Jackal.jackal != null && Jackal.jackal.PlayerId == player.PlayerId) || Jackal.formerJackals.Any(x => x.PlayerId == player.PlayerId)) && Jackal.hasImpostorVision)
                || (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == player.PlayerId && Sidekick.hasImpostorVision)
                || (Spy.spy != null && Spy.spy.PlayerId == player.PlayerId && Spy.hasImpostorVision)
                || (Jester.jester != null && Jester.jester.PlayerId == player.PlayerId && Jester.hasImpostorVision)
                || (Thief.thief != null && Thief.thief.PlayerId == player.PlayerId && Thief.hasImpostorVision)
                || (Madmate.madmate.Any(x => x.PlayerId == player.PlayerId) && Madmate.hasImpostorVision)
                || (CreatedMadmate.createdMadmate != null && CreatedMadmate.createdMadmate.PlayerId == player.PlayerId && CreatedMadmate.hasImpostorVision)
                || (Moriarty.moriarty != null && Moriarty.moriarty.PlayerId == player.PlayerId)
                || (JekyllAndHyde.jekyllAndHyde != null && !JekyllAndHyde.isJekyll() && JekyllAndHyde.jekyllAndHyde.PlayerId == player.PlayerId)
                || (Fox.fox != null && Fox.fox.PlayerId == player.PlayerId)
                || (SchrodingersCat.schrodingersCat != null && SchrodingersCat.schrodingersCat.PlayerId == player.PlayerId && SchrodingersCat.hasTeam() && SchrodingersCat.team != SchrodingersCat.Team.Crewmate);
        }
        
        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }

        public static void SetDeadBodyOutline(DeadBody target, Color? color)
        {
            if (target == null || target.bodyRenderers[0] == null) return;
            target.bodyRenderers[0].material.SetFloat("_Outline", color == null ? 0f : 1f);
            if (color != null) target.bodyRenderers[0].material.SetColor("_OutlineColor", color.Value);
        }

        public static int GetClientId(PlayerControl control)
        {
            for (int i = 0; i < AmongUsClient.Instance.allClients.Count; i++)
            {
                InnerNet.ClientData data = AmongUsClient.Instance.allClients[i];
                if (data.Character == control)
                    return data.Id;
            }
            return -1;
        }

        public static DeadBody setDeadTarget(float maxDistance = 0f, PlayerControl targetingPlayer = null)
        {
            DeadBody result = null;
            float closestDistance = float.MaxValue;

            if (!MapUtilities.CachedShipStatus) return null;

            if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
            if (targetingPlayer.Data.IsDead) return null;

            maxDistance = maxDistance == 0f ? 1f : maxDistance + 0.1f;

            Vector2 truePosition = targetingPlayer.GetTruePosition() - new Vector2(-0.2f, -0.22f);

            bool flag = GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks
                        && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver);

            Collider2D[] allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask("Players", "Ghost"));


            foreach (Collider2D collider2D in allocs)
            {
                if (!flag || collider2D.tag != "DeadBody") continue;
                DeadBody component = collider2D.GetComponent<DeadBody>();

                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                float distance = Vector2.Distance(truePosition, component.TruePosition);

                if (!(distance < closestDistance)) continue;

                result = component;
                closestDistance = distance;
            }

            if (result && Undertaker.undertaker == targetingPlayer)
                SetDeadBodyOutline(result, Undertaker.color);

            return result;
        }

        public static void HandleUndertakerDropOnBodyReport()
        {
            if (Undertaker.undertaker == null) return;
            var position = Undertaker.DraggedBody != null
                ? Undertaker.DraggedBody.transform.position
                : Vector3.zero;
            Undertaker.DropBody(position);
        }
    }
}
