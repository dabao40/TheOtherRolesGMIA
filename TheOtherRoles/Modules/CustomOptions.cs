using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Hazel;
using System.Reflection;
using System.Text;
using TheOtherRoles.Players;
using TheOtherRoles.Utilities;
using static TheOtherRoles.TheOtherRoles;
using static TheOtherRoles.CustomOption;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;
using TMPro;
using TheOtherRoles.Modules;
using AmongUs.Data;

namespace TheOtherRoles {
    public class CustomOption {
        public enum CustomOptionType {
            General,
            Impostor,
            Neutral,
            Crewmate,
            Modifier,
            Guesser,
            HideNSeekMain,
            HideNSeekRoles
        }

        public static List<CustomOption> options = new();
        public static int preset = 0;
        public static ConfigEntry<string> vanillaSettings;

        public int id;
        public string name;
        public string format;
        public System.Object[] selections;

        public int defaultSelection;
        public ConfigEntry<int> entry;
        public int selection;
        public OptionBehaviour optionBehaviour;
        public CustomOption parent;
        public bool isHeader;
        public CustomOptionType type;
        public string heading = "";

        // Option creation

        public CustomOption(int id, CustomOptionType type, string name,  System.Object[] selections, System.Object defaultValue, CustomOption parent, bool isHeader, string format, string heading = "") {
            this.id = id;
            this.name = parent == null ? name : "- " + name;
            this.format = format;
            this.selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            this.defaultSelection = index >= 0 ? index : 0;
            this.parent = parent;
            this.isHeader = isHeader;
            this.type = type;
            this.heading = heading;
            selection = 0;
            if (id != 0) {
                entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", id.ToString(), defaultSelection);
                selection = Mathf.Clamp(entry.Value, 0, selections.Length - 1);
            }
            options.Add(this);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections, CustomOption parent = null, bool isHeader = false, string format = "", string heading = "") {
            return new CustomOption(id, type, name, selections, "", parent, isHeader, format, heading);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false, string format = "", string heading = "") {
            List<object> selections = new();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, type, name, selections.ToArray(), defaultValue, parent, isHeader, format, heading);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false, string format = "", string heading = "") {
            return new CustomOption(id, type, name, new string[]{ "optionOff", "optionOn" }, defaultValue ? "optionOn" : "optionOff", parent, isHeader, format, heading);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, List<RoleId> roleId, CustomOption parent = null, bool isHeader = false) {
            return new CustomOption(id, type, name, roleId.Select(x => x == RoleId.Jester ? "optionOff" : RoleInfo.allRoleInfos.FirstOrDefault(y => y.roleId == x
            && y.color != Palette.ImpostorRed && !y.isNeutral).nameKey).ToArray(), 0, parent, isHeader, "");
        }

        // Static behaviour

        public static void switchPreset(int newPreset) {
            saveVanillaOptions();
            CustomOption.preset = newPreset;
            vanillaSettings = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", "GameOptions", "");
            loadVanillaOptions();
            foreach (CustomOption option in CustomOption.options) {
                if (option.id == 0) continue;

                option.entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", option.id.ToString(), option.defaultSelection);
                option.selection = Mathf.Clamp(option.entry.Value, 0, option.selections.Length - 1);
                if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption) {
                    stringOption.oldValue = stringOption.Value = option.selection;
                    stringOption.ValueText.text = option.getString();
                }
            }
        }

        public static void saveVanillaOptions() {
            vanillaSettings.Value = Convert.ToBase64String(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions, false));
        }

        public static bool loadVanillaOptions() {
            string optionsString = vanillaSettings.Value;
            if (optionsString == "") return false;
            IGameOptions gameOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
            if (gameOptions.Version < 8)
            {
                TheOtherRolesPlugin.Logger.LogMessage("tried to paste old settings, not doing this!");
                return false;
            }
            GameOptionsManager.Instance.GameHostOptions = gameOptions;
            GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
            GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
            GameManager.Instance.LogicOptions.SyncOptions();
            return true;
        }

        public static void ShareOptionChange(uint optionId) {
            var option = options.FirstOrDefault(x => x.id == optionId);
            if (option == null) return;
            var writer = AmongUsClient.Instance!.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareOptions, SendOption.Reliable, -1);
            writer.Write((byte)1);
            writer.WritePacked((uint)option.id);
            writer.WritePacked(Convert.ToUInt32(option.selection));
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void ShareOptionSelections() {
            if (CachedPlayer.AllPlayers.Count <= 1 || AmongUsClient.Instance!.AmHost == false && CachedPlayer.LocalPlayer.PlayerControl == null) return;
            var optionsList = new List<CustomOption>(CustomOption.options);
            while (optionsList.Any())
            {
                byte amount = (byte) Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
                var writer = AmongUsClient.Instance!.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareOptions, SendOption.Reliable, -1);
                writer.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    var option = optionsList[0];
                    optionsList.RemoveAt(0);
                    writer.WritePacked((uint) option.id);
                    writer.WritePacked(Convert.ToUInt32(option.selection));
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        // Getter

        public int getSelection() {
            return selection;
        }

        public bool getBool() {
            return selection > 0;
        }

        public float getFloat() {
            return (float)selections[selection];
        }

        public int getQuantity() {
            return selection + 1;
        }

        public string getString()
        {
            string sel = selections[selection].ToString();
            if (format != "")
            {
                return string.Format(ModTranslation.getString(format), sel);
            }

            if (sel is "optionOn" or "mayorBeforeVoting" or "mayorUntilMeetingEnd" or "deputyOnImmediately" or "deputyOnAfterMeeting")
            {
                return "<color=#FFFF00FF>" + ModTranslation.getString(sel) + "</color>";
            }
            else if (sel == "optionOff")
            {
                return "<color=#CCCCCCFF>" + ModTranslation.getString(sel) + "</color>";
            }

            return ModTranslation.getString(sel);
        }

        public string getName()
        {
            return ModTranslation.getString(name);
        }

        public string getHeading()
        {
            if (heading == "") return "";
            return ModTranslation.getString(heading);
        }

        // Option changes

        public void updateSelection(int newSelection, bool notifyUsers = true) {
            newSelection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
            bool doNeedNotifier = AmongUsClient.Instance?.AmClient == true && notifyUsers && selection != newSelection;
            if (doNeedNotifier)
            {
                try
                {
                    if (GameStartManager.Instance != null && GameStartManager.Instance.LobbyInfoPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf)
                    {
                        LobbyViewSettingsPaneChangeTabPatch.Postfix(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
                    }
                }
                catch { }
            }
            selection = newSelection;
            if (doNeedNotifier) {
                CustomOption originalParent = parent;
                if (originalParent != null) {
                    while (originalParent.parent != null)
                        originalParent = originalParent.parent;
                }
                DestroyableSingleton<HudManager>.Instance.Notifier.AddModSettingsChangeMessage((StringNames)(this.id + 6000), getString(),
                    (originalParent != null ? originalParent.getName().Replace("- ", "") + ": " : "") + getName().Replace("- ", ""), false);
            }
            if (optionBehaviour != null && optionBehaviour is StringOption stringOption) {
                stringOption.oldValue = stringOption.Value = selection;
                stringOption.ValueText.text = getString();

                if (AmongUsClient.Instance?.AmHost == true && CachedPlayer.LocalPlayer.PlayerControl) {
                    if (id == 0 && selection != preset) {
                        switchPreset(selection); // Switch presets
                        ShareOptionSelections();
                    } else if (entry != null) {
                        entry.Value = selection; // Save selection to config
                        ShareOptionChange((uint)id);// Share single selection
                    }
                }
            } else if (id == 0 && AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer) {  // Share the preset switch for random maps, even if the menu isnt open!
                switchPreset(selection);
                ShareOptionSelections();// Share all selections
            }
        }

        public static byte[] serializeOptions() {
            using (MemoryStream memoryStream = new()) {
                using (BinaryWriter binaryWriter = new(memoryStream)) {
                    int lastId = -1;
                    foreach (var option in CustomOption.options.OrderBy(x => x.id)) {
                        if (option.id == 0) continue;
                        bool consecutive = lastId + 1 == option.id;
                        lastId = option.id;

                        binaryWriter.Write((byte)(option.selection + (consecutive ? 128 : 0)));
                        if (!consecutive) binaryWriter.Write((ushort)option.id);
                    }
                    binaryWriter.Flush();
                    memoryStream.Position = 0L;
                    return memoryStream.ToArray();
                }
            }
        }

        public static int deserializeOptions(byte[] inputValues) {
            BinaryReader reader = new(new MemoryStream(inputValues));
            int lastId = -1;
            bool somethingApplied = false;
            int errors = 0;
            while (reader.BaseStream.Position < inputValues.Length) {
                try {
                    int selection = reader.ReadByte();
                    int id = -1;
                    bool consecutive = selection >= 128;
                    if (consecutive) {
                        selection -= 128;
                        id = lastId + 1;
                    } else {
                        id = reader.ReadUInt16();
                    }
                    if (id == 0) continue;
                    lastId = id;
                    CustomOption option = options.First(option => option.id == id);
                    option.entry = TheOtherRolesPlugin.Instance.Config.Bind($"Preset{preset}", option.id.ToString(), option.defaultSelection);
                    option.selection = selection;
                    if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption)
                    {
                        stringOption.oldValue = stringOption.Value = option.selection;
                        stringOption.ValueText.text = option.getString();
                    }
                    somethingApplied = true;
                } catch (Exception e) {
                    TheOtherRolesPlugin.Logger.LogWarning($"id:{lastId}:{e}: while deserializing - tried to paste invalid settings!");
                    errors++;
                }
            }
            return Convert.ToInt32(somethingApplied) + (errors > 0 ? 0 : 1);
        }

        // Copy to or paste from clipboard (as string)
        public static void copyToClipboard() {
            GUIUtility.systemCopyBuffer = $"{TheOtherRolesPlugin.VersionString}!{Convert.ToBase64String(serializeOptions())}!{vanillaSettings.Value}";
        }

        public static int pasteFromClipboard() {
            string allSettings = GUIUtility.systemCopyBuffer;
            int torOptionsFine = 0;
            bool vanillaOptionsFine = false;
            try {
                var settingsSplit = allSettings.Split("!");
                Version versionInfo = Version.Parse(settingsSplit[0]);
                string torSettings = settingsSplit[1];
                string vanillaSettingsSub = settingsSplit[2];
                torOptionsFine = deserializeOptions(Convert.FromBase64String(torSettings));
                ShareOptionSelections();
                if (TheOtherRolesPlugin.Version > versionInfo && versionInfo < Version.Parse("1.2.7"))
                {
                    vanillaOptionsFine = false;
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Host Info: Pasting vanilla settings failed, TOR Options applied!");
                }
                else
                {
                    vanillaSettings.Value = vanillaSettingsSub;
                    vanillaOptionsFine = loadVanillaOptions();
                }
            } catch (Exception e) {
                TheOtherRolesPlugin.Logger.LogWarning($"{e}: tried to paste invalid settings!\n{allSettings}");
                string errorStr = allSettings.Length > 2 ? allSettings.Substring(0, 3) : "(empty clipboard) ";
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"Host Info: You tried to paste invalid settings: \"{errorStr}...\"");
                SoundEffectsManager.Load();
                SoundEffectsManager.play("fail");
            }
            return Convert.ToInt32(vanillaOptionsFine) + torOptionsFine;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    class GameOptionsMenuChangeTabPatch
    {
        public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly)
        {
            if (previewOnly) return;
            foreach (var tab in GameOptionsMenuStartPatch.currentTabs)
            {
                if (tab != null)
                    tab.SetActive(false);
            }
            foreach (var pbutton in GameOptionsMenuStartPatch.currentButtons)
            {
                pbutton.SelectButton(false);
            }
            if (tabNum > 2)
            {
                tabNum -= 3;
                GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
                GameOptionsMenuStartPatch.currentButtons[tabNum].SelectButton(true);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
    class LobbyViewSettingsPaneRefreshTabPatch
    {
        public static bool Prefix(LobbyViewSettingsPane __instance)
        {
            if ((int)__instance.currentTab < 15)
            {
                LobbyViewSettingsPaneChangeTabPatch.Postfix(__instance, __instance.currentTab);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
    class LobbyViewSettingsPaneChangeTabPatch
    {
        public static void Postfix(LobbyViewSettingsPane __instance, StringNames category)
        {
            int tabNum = (int)category;

            foreach (var pbutton in LobbyViewSettingsPatch.currentButtons)
            {
                pbutton.SelectButton(false);
            }
            if (tabNum > 20) // StringNames are in the range of 3000+ 
                return;
            __instance.taskTabButton.SelectButton(false);

            if (tabNum > 2)
            {
                tabNum -= 3;
                //GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
                LobbyViewSettingsPatch.currentButtons[tabNum].SelectButton(true);
                LobbyViewSettingsPatch.drawTab(__instance, LobbyViewSettingsPatch.currentButtonTypes[tabNum]);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
    class LobbyViewSettingsPaneUpdatePatch
    {
        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            if (LobbyViewSettingsPatch.currentButtons.Count == 0)
            {
                LobbyViewSettingsPatch.gameModeChangedFlag = true;
                LobbyViewSettingsPatch.Postfix(__instance);

            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    class LobbyViewSettingsPatch
    {
        public static List<PassiveButton> currentButtons = new();
        public static List<CustomOptionType> currentButtonTypes = new();
        public static bool gameModeChangedFlag = false;

        public static void createCustomButton(LobbyViewSettingsPane __instance, int targetMenu, string buttonName, string buttonText, CustomOptionType optionType)
        {
            buttonName = "View" + buttonName;
            var buttonTemplate = GameObject.Find("OverviewTab");
            var torSettingsButton = GameObject.Find(buttonName);
            if (torSettingsButton == null)
            {
                torSettingsButton = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
                torSettingsButton.transform.localPosition += Vector3.right * 1.75f * (targetMenu - 2);
                torSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { torSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var torSettingsPassiveButton = torSettingsButton.GetComponent<PassiveButton>();
                torSettingsPassiveButton.OnClick.RemoveAllListeners();
                torSettingsPassiveButton.OnClick.AddListener((System.Action)(() => {
                    __instance.ChangeTab((StringNames)targetMenu);
                }));
                torSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                torSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                torSettingsPassiveButton.SelectButton(false);
                currentButtons.Add(torSettingsPassiveButton);
                currentButtonTypes.Add(optionType);
            }
        }

        public static void Postfix(LobbyViewSettingsPane __instance)
        {
            currentButtons.ForEach(x => x?.Destroy());
            currentButtons.Clear();
            currentButtonTypes.Clear();

            removeVanillaTabs(__instance);

            createSettingTabs(__instance);

        }

        public static void removeVanillaTabs(LobbyViewSettingsPane __instance)
        {
            GameObject.Find("RolesTabs")?.Destroy();
            var overview = GameObject.Find("OverviewTab");
            if (!gameModeChangedFlag)
            {
                overview.transform.localScale = new Vector3(0.5f * overview.transform.localScale.x, overview.transform.localScale.y, overview.transform.localScale.z);
                overview.transform.localPosition += new Vector3(-1.2f, 0f, 0f);

            }
            overview.transform.Find("FontPlacer").transform.localScale = new Vector3(1.35f, 1f, 1f);
            overview.transform.Find("FontPlacer").transform.localPosition = new Vector3(-0.6f, -0.1f, 0f);
            gameModeChangedFlag = false;
        }

        public static void drawTab(LobbyViewSettingsPane __instance, CustomOptionType optionType)
        {

            var relevantOptions = options.Where(x => x.type == optionType || x.type == CustomOption.CustomOptionType.Guesser && optionType == CustomOptionType.General).ToList();

            if ((int)optionType == 99)
            {
                // Create 4 Groups with Role settings only
                relevantOptions.Clear();
                relevantOptions.AddRange(options.Where(x => x.type == CustomOptionType.Impostor && x.isHeader));
                relevantOptions.AddRange(options.Where(x => x.type == CustomOptionType.Neutral && x.isHeader));
                relevantOptions.AddRange(options.Where(x => x.type == CustomOptionType.Crewmate && x.isHeader));
                relevantOptions.AddRange(options.Where(x => x.type == CustomOptionType.Modifier && x.isHeader));
                foreach (var option in options)
                {
                    if (option.parent != null && option.parent.getSelection() > 0)
                    {
                        if (option.id == 103) //Deputy
                            relevantOptions.Insert(relevantOptions.IndexOf(CustomOptionHolder.sheriffSpawnRate) + 1, option);
                        else if (option.id == 224) //Sidekick
                            relevantOptions.Insert(relevantOptions.IndexOf(CustomOptionHolder.jackalSpawnRate) + 1, option);
                        else if (option.id == 8000) //Prosecutor
                            relevantOptions.Insert(relevantOptions.IndexOf(CustomOptionHolder.evilHackerSpawnRate) + 1, option);
                    }
                }
            }

            if (TORMapOptions.gameMode == CustomGamemodes.Guesser) // Exclude guesser options in neutral mode
                relevantOptions = relevantOptions.Where(x => !(new List<int> { 310, 311, 312, 313, 314, 315, 316, 317, 318, 7006 }).Contains(x.id)).ToList();
            else
                relevantOptions = relevantOptions.Where(x => x.id != 7007).ToList();

            if (TORMapOptions.gameMode != CustomGamemodes.FreePlay)
                relevantOptions = relevantOptions.Where(x => x.id != 10424).ToList();

            for (int j = 0; j < __instance.settingsInfo.Count; j++)
            {
                __instance.settingsInfo[j].gameObject.Destroy();
            }
            __instance.settingsInfo.Clear();

            float num = 1.44f;
            int i = 0;
            int singles = 0;
            int headers = 0;
            int lines = 0;
            var curType = CustomOptionType.Modifier;

            foreach (var option in relevantOptions)
            {
                if (option.isHeader && (int)optionType != 99 || (int)optionType == 99 && curType != option.type)
                {
                    curType = option.type;
                    if (i != 0) num -= 0.59f;
                    if (i % 2 != 0) singles++;
                    headers++; // for header
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                    string titleText = option.heading != "" ? option.getHeading() : option.getName();
                    categoryHeaderMasked.Title.text = titleText;
                    if ((int)optionType == 99)
                        categoryHeaderMasked.Title.text = new Dictionary<CustomOptionType, string>() { { CustomOptionType.Impostor, ModTranslation.getString("impostorRoles") }, { CustomOptionType.Neutral, ModTranslation.getString("neutralRoles") },
                            { CustomOptionType.Crewmate, ModTranslation.getString("crewmateRoles") }, { CustomOptionType.Modifier, ModTranslation.getString("modifiers") } }[curType];
                    categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 0.85f;
                    i = 0;
                }

                ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
                viewSettingsInfoPanel.transform.SetParent(__instance.settingsContainer);
                viewSettingsInfoPanel.transform.localScale = Vector3.one;
                float num2;
                if (i % 2 == 0)
                {
                    lines++;
                    num2 = -8.95f;
                    if (i > 0)
                    {
                        num -= 0.59f;
                    }
                }
                else
                {
                    num2 = -3f;
                }
                viewSettingsInfoPanel.transform.localPosition = new Vector3(num2, num, -2f);
                int value = option.getSelection();
                viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, option.getString(), 61);
                viewSettingsInfoPanel.titleText.text = option.getName();
                if (option.isHeader && (int)optionType != 99 && option.heading == "" && (option.type == CustomOptionType.Neutral || option.type == CustomOptionType.Crewmate || option.type == CustomOptionType.Impostor || option.type == CustomOptionType.Modifier))
                {
                    viewSettingsInfoPanel.titleText.text = ModTranslation.getString("optionSpawnChance");
                }
                if ((int)optionType == 99)
                {
                    if (option.type == CustomOptionType.Modifier)
                        viewSettingsInfoPanel.settingText.text = viewSettingsInfoPanel.settingText.text + GameOptionsDataPatch.buildModifierExtras(option);
                }
                __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

                i++;
            }
            float actual_spacing = (headers * 0.85f + lines * 0.59f) / (headers + lines);
            __instance.scrollBar.CalculateAndSetYBounds((float)(__instance.settingsInfo.Count + singles * 2 + headers), 2f, 6f, actual_spacing);

        }

        public static void createSettingTabs(LobbyViewSettingsPane __instance)
        {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;
            if (TORMapOptions.gameMode == CustomGamemodes.Guesser || TORMapOptions.gameMode == CustomGamemodes.Classic || TORMapOptions.gameMode == CustomGamemodes.FreePlay)
            {

                // create TOR settings
                createCustomButton(__instance, next++, "TORSettings", ModTranslation.getString("torNewSettings"), CustomOptionType.General);
                // create TOR settings
                createCustomButton(__instance, next++, "RoleOverview", ModTranslation.getString("roleOverview"), (CustomOptionType)99);
                // IMp
                createCustomButton(__instance, next++, "ImpostorSettings", ModTranslation.getString("impostorRoles"), CustomOptionType.Impostor);

                // Neutral
                createCustomButton(__instance, next++, "NeutralSettings", ModTranslation.getString("neutralRoles"), CustomOptionType.Neutral);
                // Crew
                createCustomButton(__instance, next++, "CrewmateSettings", ModTranslation.getString("crewmateRoles"), CustomOptionType.Crewmate);
                // Modifier
                createCustomButton(__instance, next++, "ModifierSettings", ModTranslation.getString("modifiers"), CustomOptionType.Modifier);

            }
            else if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek)
            {
                // create Main HNS settings
                createCustomButton(__instance, next++, "HideNSeekMain", ModTranslation.getString("hideNSeekMain"), CustomOptionType.HideNSeekMain);
                // create HNS Role settings
                createCustomButton(__instance, next++, "HideNSeekRoles", ModTranslation.getString("hideNSeekRoles"), CustomOptionType.HideNSeekRoles);
            }
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    class GameOptionsMenuCreateSettingsPatch
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (__instance.gameObject.name == "GAME SETTINGS TAB")
                adaptTaskCount(__instance);
        }

        private static void adaptTaskCount(GameOptionsMenu __instance)
        {
            // Adapt task count for main options
            var commonTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks).Cast<NumberOption>();
            if (commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);
            var shortTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks).TryCast<NumberOption>();
            if (shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);
            var longTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks).TryCast<NumberOption>();
            if (longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    class GameOptionsMenuInitializePatch
    {
        public static void Postfix(GameOptionsMenu __instance)
        {
            if (Helpers.isCustomServer() || (AmongUsClient.Instance?.AmLocalHost ?? false))
            {
                var impostorsOption = __instance.Children.Find((Il2CppSystem.Predicate<OptionBehaviour>)(x => x.TryGetComponent<NumberOption>(out var op) && op.intOptionName == AmongUs.GameOptions.Int32OptionNames.NumImpostors))?.TryCast<NumberOption>();
                if (impostorsOption != null) impostorsOption.ValidRange = new FloatRange(1f, 6f);
            }
        }
    }

    class CreateGameOptionsTORBehaviour : MonoBehaviour
    {
        static CreateGameOptionsTORBehaviour() => ClassInjector.RegisterTypeInIl2Cpp<CreateGameOptionsTORBehaviour>();

        public CreateOptionsPicker MyPicker;

        void Awake()
        {
            MyPicker = gameObject.GetComponent<CreateOptionsPicker>();

            MyPicker.transform.FindChild("Game Mode").gameObject.SetActive(false);

            var impostorsRoot = MyPicker.transform.FindChild("Impostors");
            if (impostorsRoot)
            {
                impostorsRoot.transform.localPosition = new(-1.955f, -0.44f, 0f);

                var temp = impostorsRoot.transform.GetChild(1);

                var list = MyPicker.ImpostorButtons.ToList();
                for (int i = 4; i <= 6; i++)
                {
                    var obj = GameObject.Instantiate(temp, impostorsRoot);
                    obj.name = i.ToString();
                    obj.transform.localPosition = new((i - 1) * 0.6f, 0f, 0f);
                    obj.GetChild(0).GetComponent<TextMeshPro>().text = i.ToString();
                    var passiveButton = obj.gameObject.GetComponent<PassiveButton>();
                    passiveButton.OnClick = new();
                    int impostors = i;
                    passiveButton.OnClick.AddListener((Action)(() => MyPicker.SetImpostorButtons(impostors)));

                    list.Add(obj.GetComponent<ImpostorsOptionButton>());
                }

                MyPicker.ImpostorButtons = list.ToArray();
            }
        }

        void OnEnable()
        {
            if (!MyPicker) return;

            bool isCustomServer = Helpers.isCustomServer();

            if (MyPicker.MaxPlayersRoot)
            {
                //以前のボタンを削除する
                MyPicker.optionsMenu.ControllerSelectable.Clear();
                MyPicker.MaxPlayerButtons.Clear();

                Helpers.Sequential(MyPicker.MaxPlayersRoot.childCount).Skip(1).Select(i => MyPicker.MaxPlayersRoot.GetChild(i).gameObject).ToArray().Do(GameObject.Destroy);

                for (int i = 4; i <= (isCustomServer ? 24 : 15); i++)
                {
                    SpriteRenderer spriteRenderer = GameObject.Instantiate<SpriteRenderer>(MyPicker.MaxPlayerButtonPrefab, MyPicker.MaxPlayersRoot);
                    spriteRenderer.transform.localPosition = new Vector3((float)((i - 4) % 12) * 0.5f, (float)(i / 16) * -0.47f, 0f);
                    int numPlayers = i;
                    spriteRenderer.name = numPlayers.ToString();
                    PassiveButton component = spriteRenderer.GetComponent<PassiveButton>();
                    component.OnClick.AddListener((Action)(() => MyPicker.SetMaxPlayersButtons(numPlayers)));
                    spriteRenderer.GetComponentInChildren<TextMeshPro>().text = numPlayers.ToString();
                    MyPicker.MaxPlayerButtons.Add(spriteRenderer);
                    MyPicker.optionsMenu.ControllerSelectable.Add(component);
                }
            }

            var subMenu = MyPicker.transform.FindChild("SubMenu");
            subMenu.transform.localPosition = new(1.11f, isCustomServer ? -0.4f : 0f, 0f);
            subMenu.GetComponent<ShiftButtonsCrossplayEnabled>().enabled = false;

            //4人以上のオプションはカスタムサーバーのみ使用可能
            for (int i = 4; i <= 6; i++) MyPicker.ImpostorButtons[i - 1].gameObject.SetActive(isCustomServer);

            var options = MyPicker.GetTargetOptions();
            if (!isCustomServer)
            {
                if (options.MaxPlayers > 15) MyPicker.SetMaxPlayersButtons(15);
                if (options.NumImpostors > 3) MyPicker.SetImpostorButtons(3);
            }

            MyPicker.Refresh();
        }
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.Awake))]
    class CreateGameOptionsShowPatch
    {
        public static bool Prefix(CreateOptionsPicker __instance)
        {
            //各種設定の配列長を24人分まで伸ばす
            NormalGameOptionsV07.MaxImpostors = NormalGameOptionsV08.MaxImpostors = new int[] {
            0,
            0, 0, 0, 1, 1,
            1, 2, 2, 3, 3,
            3, 3, 4, 4, 5,
            5, 5, 6, 6, 6,
            6, 6, 6, 6
            };
            NormalGameOptionsV07.RecommendedImpostors = NormalGameOptionsV08.RecommendedImpostors = new int[] {
            0,
            0, 0, 0, 1, 1,
            1, 2, 2, 2, 2,
            2, 3, 3, 3, 3,
            3, 4, 4, 4, 4,
            5, 5, 5, 5
            };
            NormalGameOptionsV07.RecommendedKillCooldown = NormalGameOptionsV08.RecommendedKillCooldown = new int[] {
            0,
            0, 0, 0, 45, 30,
            15, 35, 30, 25, 20,
            20, 20, 20, 20, 20,
            20, 20, 20, 20, 20,
            20, 20, 20, 20
            };
            NormalGameOptionsV07.MinPlayers = NormalGameOptionsV08.MinPlayers = new int[] {
            4, 4, 7, 9, 13, 15, 18
            };

            //ゲームモードはノーマル固定
            DataManager.Settings.Multiplayer.LastPlayedGameMode = AmongUs.GameOptions.GameModes.Normal;
            DataManager.Settings.Save();
            GameOptionsManager.Instance.SwitchGameMode(AmongUs.GameOptions.GameModes.Normal);

            __instance.gameObject.AddComponent<CreateGameOptionsTORBehaviour>();

            return false;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    class GameOptionsMenuStartPatch
    {
        public static List<GameObject> currentTabs = new();
        public static List<PassiveButton> currentButtons = new();

        public static void Postfix(GameSettingMenu __instance)
        {
            currentTabs.ForEach(x => x?.Destroy());
            currentButtons.ForEach(x => x?.Destroy());
            currentTabs = new();
            currentButtons = new();

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

            removeVanillaTabs(__instance);

            createSettingTabs(__instance);

            var GOMGameObject = GameObject.Find("GAME SETTINGS TAB");


            // create copy to clipboard and paste from clipboard buttons.
            var template = GameObject.Find("PlayerOptionsMenu(Clone)").transform.Find("CloseButton").gameObject;
            var holderGO = new GameObject("copyPasteButtonParent");
            var bgrenderer = holderGO.AddComponent<SpriteRenderer>();
            bgrenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CopyPasteBG.png", 175f);
            holderGO.transform.SetParent(template.transform.parent, false);
            holderGO.transform.localPosition = template.transform.localPosition + new Vector3(-8.3f, 0.73f, -2f);
            holderGO.layer = template.layer;
            holderGO.SetActive(true);
            var copyButton = GameObject.Instantiate(template, holderGO.transform);
            copyButton.transform.localPosition = new Vector3(-0.3f, 0.02f, -2f);
            var copyButtonPassive = copyButton.GetComponent<PassiveButton>();
            var copyButtonRenderer = copyButton.GetComponentInChildren<SpriteRenderer>();
            var copyButtonActiveRenderer = copyButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            copyButtonRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Copy.png", 100f);
            copyButton.transform.GetChild(1).transform.localPosition = Vector3.zero;
            copyButtonActiveRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.CopyActive.png", 100f);
            copyButtonPassive.OnClick.RemoveAllListeners();
            copyButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            copyButtonPassive.OnClick.AddListener((System.Action)(() => {
                copyToClipboard();
                copyButtonRenderer.color = Color.green;
                copyButtonActiveRenderer.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95)
                    {
                        copyButtonRenderer.color = Color.white;
                        copyButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
            var pasteButton = GameObject.Instantiate(template, holderGO.transform);
            pasteButton.transform.localPosition = new Vector3(0.3f, 0.02f, -2f);
            var pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
            var pasteButtonRenderer = pasteButton.GetComponentInChildren<SpriteRenderer>();
            var pasteButtonActiveRenderer = pasteButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            pasteButtonRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Paste.png", 100f);
            pasteButtonActiveRenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.PasteActive.png", 100f);
            pasteButtonPassive.OnClick.RemoveAllListeners();
            pasteButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            pasteButtonPassive.OnClick.AddListener((System.Action)(() => {
                pasteButtonRenderer.color = Color.yellow;
                int success = pasteFromClipboard();
                pasteButtonRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                pasteButtonActiveRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95)
                    {
                        pasteButtonRenderer.color = Color.white;
                        pasteButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
        }

        private static void createSettings(GameOptionsMenu menu, List<CustomOption> options)
        {
            float num = 1.5f;
            foreach (CustomOption option in options)
            {
                if (option.isHeader)
                {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                    string titleText = option.heading != "" ? option.getHeading() : option.getName();
                    categoryHeaderMasked.Title.text = titleText;
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                }
                OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<StringOption>(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                optionBehaviour.SetClickMask(menu.ButtonClickMask);

                // "SetUpFromData"
                SpriteRenderer[] componentsInChildren = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, 20);
                }
                foreach (TextMeshPro textMeshPro in optionBehaviour.GetComponentsInChildren<TextMeshPro>(true))
                {
                    textMeshPro.fontMaterial.SetFloat("_StencilComp", 3f);
                    textMeshPro.fontMaterial.SetFloat("_Stencil", (float)20);
                }

                var stringOption = optionBehaviour as StringOption;
                stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
                stringOption.TitleText.text = option.getName();
                if (option.isHeader && option.heading == "" && (option.type == CustomOptionType.Neutral || option.type == CustomOptionType.Crewmate || option.type == CustomOptionType.Impostor || option.type == CustomOptionType.Modifier))
                {
                    stringOption.TitleText.text = ModTranslation.getString("optionSpawnChance");
                }
                if (stringOption.TitleText.text.Length > 25)
                    stringOption.TitleText.fontSize = 2.2f;
                if (stringOption.TitleText.text.Length > 40)
                    stringOption.TitleText.fontSize = 2f;
                stringOption.Value = stringOption.oldValue = option.selection;
                stringOption.ValueText.text = option.getString();
                option.optionBehaviour = stringOption;

                menu.Children.Add(optionBehaviour);
                num -= 0.45f;
                menu.scrollBar.SetYBoundsMax(-num - 1.65f);
            }

            for (int i = 0; i < menu.Children.Count; i++)
            {
                OptionBehaviour optionBehaviour = menu.Children[i];
                if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost)
                {
                    optionBehaviour.SetAsPlayer();
                }
            }
        }

        private static void removeVanillaTabs(GameSettingMenu __instance)
        {
            GameObject.Find("What Is This?")?.Destroy();
            GameObject.Find("GamePresetButton")?.Destroy();
            GameObject.Find("RoleSettingsButton")?.Destroy();
            __instance.ChangeTab(1, false);
        }

        public static void createCustomButton(GameSettingMenu __instance, int targetMenu, string buttonName, string buttonText)
        {
            var leftPanel = GameObject.Find("LeftPanel");
            var buttonTemplate = GameObject.Find("GameSettingsButton");
            if (targetMenu == 3)
            {
                buttonTemplate.transform.localPosition -= Vector3.up * 0.85f;
                buttonTemplate.transform.localScale *= Vector2.one * 0.75f;
            }
            var torSettingsButton = GameObject.Find(buttonName);
            if (torSettingsButton == null)
            {
                torSettingsButton = GameObject.Instantiate(buttonTemplate, leftPanel.transform);
                torSettingsButton.transform.localPosition += Vector3.up * 0.5f * (targetMenu - 2);
                torSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { torSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var torSettingsPassiveButton = torSettingsButton.GetComponent<PassiveButton>();
                torSettingsPassiveButton.OnClick.RemoveAllListeners();
                torSettingsPassiveButton.OnClick.AddListener((System.Action)(() => {
                    __instance.ChangeTab(targetMenu, false);
                }));
                torSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                torSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                torSettingsPassiveButton.SelectButton(false);
                currentButtons.Add(torSettingsPassiveButton);
            }
        }

        public static void createGameOptionsMenu(GameSettingMenu __instance, CustomOptionType optionType, string settingName)
        {
            var tabTemplate = GameObject.Find("GAME SETTINGS TAB");
            currentTabs.RemoveAll(x => x == null);

            var torSettingsTab = GameObject.Instantiate(tabTemplate, tabTemplate.transform.parent);
            torSettingsTab.name = settingName;

            var torSettingsGOM = torSettingsTab.GetComponent<GameOptionsMenu>();
            foreach (var child in torSettingsGOM.Children)
            {
                child.Destroy();
            }
            torSettingsGOM.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
            torSettingsGOM.Children.Clear();
            var relevantOptions = options.Where(x => x.type == optionType).ToList();
            if (TORMapOptions.gameMode == CustomGamemodes.Guesser) // Exclude guesser options in neutral mode
                relevantOptions = relevantOptions.Where(x => !(new List<int> { 310, 311, 312, 313, 314, 315, 316, 317, 318, 7006 }).Contains(x.id)).ToList();
            else
                relevantOptions = relevantOptions.Where(x => x.id != 7007).ToList();

            if (TORMapOptions.gameMode != CustomGamemodes.FreePlay)
                relevantOptions = relevantOptions.Where(x => x.id != 10424).ToList();
            createSettings(torSettingsGOM, relevantOptions);

            currentTabs.Add(torSettingsTab);
            torSettingsTab.SetActive(false);
        }

        private static void createSettingTabs(GameSettingMenu __instance)
        {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;
            if (TORMapOptions.gameMode == CustomGamemodes.Guesser || TORMapOptions.gameMode == CustomGamemodes.Classic || TORMapOptions.gameMode == CustomGamemodes.FreePlay)
            {

                // create TOR settings
                createCustomButton(__instance, next++, "TORSettings", ModTranslation.getString("torNewSettings"));
                createGameOptionsMenu(__instance, CustomOptionType.General, "TORSettings");
                // Guesser if applicable
                if (TORMapOptions.gameMode == CustomGamemodes.Guesser)
                {
                    createCustomButton(__instance, next++, "GuesserSettings", ModTranslation.getString("guesserNewSettings"));
                    createGameOptionsMenu(__instance, CustomOptionType.Guesser, "GuesserSettings");
                }
                // IMp
                createCustomButton(__instance, next++, "ImpostorSettings", ModTranslation.getString("impostorRoles"));
                createGameOptionsMenu(__instance, CustomOptionType.Impostor, "ImpostorSettings");

                // Neutral
                createCustomButton(__instance, next++, "NeutralSettings", ModTranslation.getString("neutralRoles"));
                createGameOptionsMenu(__instance, CustomOptionType.Neutral, "NeutralSettings");
                // Crew
                createCustomButton(__instance, next++, "CrewmateSettings", ModTranslation.getString("crewmateRoles"));
                createGameOptionsMenu(__instance, CustomOptionType.Crewmate, "CrewmateSettings");
                // Modifier
                createCustomButton(__instance, next++, "ModifierSettings", ModTranslation.getString("modifiers"));
                createGameOptionsMenu(__instance, CustomOptionType.Modifier, "ModifierSettings");

            }
            else if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek)
            {
                // create Main HNS settings
                createCustomButton(__instance, next++, "HideNSeekMain", ModTranslation.getString("hideNSeekMain"));
                createGameOptionsMenu(__instance, CustomOptionType.HideNSeekMain, "HideNSeekMain");
                // create HNS Role settings
                createCustomButton(__instance, next++, "HideNSeekRoles", ModTranslation.getString("hideNSeekRoles"));
                createGameOptionsMenu(__instance, CustomOptionType.HideNSeekRoles, "HideNSeekRoles");
            }
        }
    }



    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public class StringOptionEnablePatch {
        public static bool Prefix(StringOption __instance) {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => {});
            __instance.Value = __instance.oldValue = option.selection;
            __instance.ValueText.text = option.getString();
            
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public class StringOptionIncreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.updateSelection(option.selection + 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public class StringOptionDecreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.updateSelection(option.selection - 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public class RpcSyncSettingsPatch
    {
        public static void Postfix()
        {
            CustomOption.saveVanillaOptions();
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public class AmongUsClientOnPlayerJoinedPatch {
        public static void Postfix() {
            if (PlayerControl.LocalPlayer != null && AmongUsClient.Instance.AmHost) {
                GameManager.Instance.LogicOptions.SyncOptions();
                CustomOption.ShareOptionSelections();
            }
        }
    }


    [HarmonyPatch] 
    class GameOptionsDataPatch
    {
        /*private static IEnumerable<MethodBase> TargetMethods() {
            return typeof(IGameOptionsExtensions.).GetMethods().Where(x => x.ReturnType == typeof(string) && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(int));
        }*/

        private static string buildRoleOptions() {
            var impRoles = buildOptionsOfType(CustomOption.CustomOptionType.Impostor, true) + "\n";
            var neutralRoles = buildOptionsOfType(CustomOption.CustomOptionType.Neutral, true) + "\n";
            var crewRoles = buildOptionsOfType(CustomOption.CustomOptionType.Crewmate, true) + "\n";
            var modifiers = buildOptionsOfType(CustomOption.CustomOptionType.Modifier, true);
            return impRoles + neutralRoles + crewRoles + modifiers;
        }
        public static string buildModifierExtras(CustomOption customOption) {
            // find options children with quantity
            var children = CustomOption.options.Where(o => o.parent == customOption);
            var quantity = children.Where(o => o.name.Contains("Quantity")).ToList();
            if (customOption.getSelection() == 0) return "";
            if (quantity.Count == 1) return $" ({quantity[0].getQuantity()})";
            if (customOption == CustomOptionHolder.modifierLover) {
                return $" ({ModTranslation.getString("loverIsImpOption")}: {CustomOptionHolder.modifierLoverImpLoverRate.getSelection() * 10}%)";
            }
            return "";
        }

        private static string buildOptionsOfType(CustomOption.CustomOptionType type, bool headerOnly) {
            StringBuilder sb = new("\n");
            var options = CustomOption.options.Where(o => o.type == type);
            if (TORMapOptions.gameMode == CustomGamemodes.Guesser) {
                if (type == CustomOption.CustomOptionType.General)
                    options = CustomOption.options.Where(o => o.type == type || o.type == CustomOption.CustomOptionType.Guesser);
                List<int> remove = new() { 308, 310, 311, 312, 313, 314, 315, 316, 317, 318, 7006 };
                options = options.Where(x => !remove.Contains(x.id));
            } else if (TORMapOptions.gameMode == CustomGamemodes.Classic) 
                options = options.Where(x => !(x.type == CustomOption.CustomOptionType.Guesser || x == CustomOptionHolder.crewmateRolesFill || x.id == 7007));
            else if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek)
                options = options.Where(x => (x.type == CustomOption.CustomOptionType.HideNSeekMain || x.type == CustomOption.CustomOptionType.HideNSeekRoles));
            if (TORMapOptions.gameMode != CustomGamemodes.FreePlay)
                options = options.Where(x => x.id != 10424);
            foreach (var option in options) {
                if (option.parent == null) {
                    string line = $"{option.getName()}: {option.getString()}";
                    if (type == CustomOption.CustomOptionType.Modifier) line += buildModifierExtras(option);
                    sb.AppendLine(line);
                }
                else if (option.parent.getSelection() > 0) {
                    if (option.id == 103) //Deputy
                        sb.AppendLine($"- {Helpers.cs(Deputy.color, ModTranslation.getString("deputy"))}: {option.getString()}");
                    else if (option.id == 224) //Sidekick
                        sb.AppendLine($"- {Helpers.cs(Sidekick.color, ModTranslation.getString("sidekick"))}: {option.getString()}");
                    else if (option.id == 8000) // Created Madmate
                        sb.AppendLine($"- {Helpers.cs(Madmate.color, Madmate.fullName)}: {option.getString()}");
                    //else if (option.id == 358) //Prosecutor
                    //sb.AppendLine($"- {Helpers.cs(Lawyer.color, "Prosecutor")}: {option.selections[option.selection].ToString()}");
                }
            }
            if (headerOnly) return sb.ToString();
            else sb = new StringBuilder();

            foreach (CustomOption option in options) {
                if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek && option.type != CustomOptionType.HideNSeekMain && option.type != CustomOptionType.HideNSeekRoles) continue;
                if (option.parent != null) {
                    bool isIrrelevant = option.parent.getSelection() == 0 || (option.parent.parent != null && option.parent.parent.getSelection() == 0);

                    Color c = isIrrelevant ? Color.grey : Color.white;  // No use for now
                    if (isIrrelevant) continue;
                    sb.AppendLine(Helpers.cs(c, $"{option.getName()}: {option.getString()}"));
                } else {
                    if (option == CustomOptionHolder.crewmateRolesCountMin) {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("crewmateRoles"));
                        var min = CustomOptionHolder.crewmateRolesCountMin.getSelection();
                        var max = CustomOptionHolder.crewmateRolesCountMax.getSelection();
                        string optionValue = "";
                        if (CustomOptionHolder.crewmateRolesFill.getBool()) {
                            var crewCount = PlayerControl.AllPlayerControls.Count - GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                            int minNeutral = CustomOptionHolder.neutralRolesCountMin.getSelection();
                            int maxNeutral = CustomOptionHolder.neutralRolesCountMax.getSelection();
                            if (minNeutral > maxNeutral) minNeutral = maxNeutral;
                            min = crewCount - maxNeutral;
                            max = crewCount - minNeutral;
                            if (min < 0) min = 0;
                            if (max < 0) max = 0;
                            optionValue = ModTranslation.getString("crewmateFill");
                        }
                        if (min > max) min = max;
                        optionValue += (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    } else if (option == CustomOptionHolder.neutralRolesCountMin) {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("neutralRoles"));
                        var min = CustomOptionHolder.neutralRolesCountMin.getSelection();
                        var max = CustomOptionHolder.neutralRolesCountMax.getSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    } else if (option == CustomOptionHolder.impostorRolesCountMin) {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("impostorRoles"));
                        var min = CustomOptionHolder.impostorRolesCountMin.getSelection();
                        var max = CustomOptionHolder.impostorRolesCountMax.getSelection();
                        if (max > GameOptionsManager.Instance.currentGameOptions.NumImpostors) max = GameOptionsManager.Instance.currentGameOptions.NumImpostors;
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    } else if (option == CustomOptionHolder.modifiersCountMin) {
                        var optionName = CustomOptionHolder.cs(new Color(204f / 255f, 204f / 255f, 0, 1f), ModTranslation.getString("modifiers"));
                        var min = CustomOptionHolder.modifiersCountMin.getSelection();
                        var max = CustomOptionHolder.modifiersCountMax.getSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    } else if ((option == CustomOptionHolder.crewmateRolesCountMax) || (option == CustomOptionHolder.neutralRolesCountMax) || (option == CustomOptionHolder.impostorRolesCountMax) || option == CustomOptionHolder.modifiersCountMax) {
                        continue;
                    } else {
                        sb.AppendLine($"\n{option.getName()}: {option.getString()}");
                    }
                }
            }
            return sb.ToString();
        }

        public static string buildAllOptions(string vanillaSettings = "", bool hideExtras = false) {
            if (vanillaSettings == "")
                vanillaSettings = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(PlayerControl.AllPlayerControls.Count);
            int counter = TheOtherRolesPlugin.optionsPage;
            string hudString = counter != 0 && !hideExtras ? Helpers.cs(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, $"{ModTranslation.getString("useScrollWheel")}\n\n") : "";
            int maxPage = 7;
            if (TORMapOptions.gameMode == CustomGamemodes.HideNSeek) {
                if (TheOtherRolesPlugin.optionsPage > 1) TheOtherRolesPlugin.optionsPage = 0;
                maxPage = 2;
                switch (counter) {
                    case 0:
                        hudString += ModTranslation.getString("hideNSeekPage1") + buildOptionsOfType(CustomOption.CustomOptionType.HideNSeekMain, false);
                        break;
                    case 1:
                        hudString += ModTranslation.getString("hideNSeekPage2") + buildOptionsOfType(CustomOption.CustomOptionType.HideNSeekRoles, false);
                        break;
                }
            } else {
                switch (counter) {
                    case 0:
                        hudString += (!hideExtras ? "" : ModTranslation.getString("page1")) + vanillaSettings;
                        break;
                    case 1:
                        hudString += ModTranslation.getString("page2") + buildOptionsOfType(CustomOption.CustomOptionType.General, false);
                        break;
                    case 2:
                        hudString += ModTranslation.getString("page3") + buildRoleOptions();
                        break;
                    case 3:
                        hudString += ModTranslation.getString("page4") + buildOptionsOfType(CustomOption.CustomOptionType.Impostor, false);
                        break;
                    case 4:
                        hudString += ModTranslation.getString("page5") + buildOptionsOfType(CustomOption.CustomOptionType.Neutral, false);
                        break;
                    case 5:
                        hudString += ModTranslation.getString("page6") + buildOptionsOfType(CustomOption.CustomOptionType.Crewmate, false);
                        break;
                    case 6:
                        hudString += ModTranslation.getString("page7") + buildOptionsOfType(CustomOption.CustomOptionType.Modifier, false);
                        break;
                }
            }

            if (!hideExtras || counter != 0) hudString += string.Format(ModTranslation.getString("pressTabForMore"), (counter + 1), maxPage);
            return hudString;
        }


        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static void Postfix(ref string __result)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return; // Allow Vanilla Hide N Seek
            __result = buildAllOptions(vanillaSettings:__result);
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class GameOptionsNextPagePatch
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            int page = TheOtherRolesPlugin.optionsPage;
            if (Input.GetKeyDown(KeyCode.Tab)) {
                TheOtherRolesPlugin.optionsPage = (TheOtherRolesPlugin.optionsPage + 1) % 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
                TheOtherRolesPlugin.optionsPage = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
                TheOtherRolesPlugin.optionsPage = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
                TheOtherRolesPlugin.optionsPage = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) {
                TheOtherRolesPlugin.optionsPage = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) {
                TheOtherRolesPlugin.optionsPage = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) {
                TheOtherRolesPlugin.optionsPage = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) {
                TheOtherRolesPlugin.optionsPage = 6;
            }
            if (Input.GetKeyDown(KeyCode.F1))
                HudManagerUpdate.ToggleSettings(HudManager.Instance);
        }
    }

    // This class is taken and adapted from Town of Us Reactivated, https://github.com/eDonnes124/Town-Of-Us-R/blob/master/source/Patches/CustomOption/Patches.cs, Licensed under GPLv3
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate {
        [HarmonyPrefix]
        public static void Prefix2(HudManager __instance) {
            if (!settingsTMPs[0]) return;
            foreach (var tmp in settingsTMPs) tmp.text = "";
            var settingsString = GameOptionsDataPatch.buildAllOptions(hideExtras: true);
            var blocks = settingsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries); ;
            string curString = "";
            string curBlock;
            int j = 0;
            for (int i = 0; i < blocks.Length; i++) {
                if (AmongUs.Data.DataManager.Settings.Language.CurrentLanguage != SupportedLangs.English)
                    blocks[i] = "<line-height=97%>" + blocks[i] + "</line-height>";
                curBlock = blocks[i];
                if (Helpers.lineCount(curBlock) + Helpers.lineCount(curString) < (AmongUs.Data.DataManager.Settings.Language.CurrentLanguage == SupportedLangs.English ? 46 : 42)) { // original: 43
                    curString += curBlock + "\n\n";
                } else {
                    settingsTMPs[j].text = curString;
                    j++;

                    curString = "\n" + curBlock + "\n\n";
                    if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
                }
            }
            if (j < settingsTMPs.Length) settingsTMPs[j].text = curString;
            int blockCount = 0;
            foreach (var tmp in settingsTMPs) {
                if (tmp.text != "")
                    blockCount++;
            }
            for (int i = 0; i < blockCount; i++) {
                TheOtherRolesPlugin.Logger.LogMessage(blockCount);
                settingsTMPs[i].transform.localPosition = new Vector3(- blockCount * 1.2f + 2.7f * i, 2.2f, -500f);
            }
        }

        private static TMPro.TextMeshPro[] settingsTMPs = new TMPro.TextMeshPro[4];
        private static GameObject settingsBackground;
        public static void OpenSettings(HudManager __instance) {
            if (__instance.FullScreen == null || MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) return;
            settingsBackground = GameObject.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
            settingsBackground.SetActive(true);
            var renderer = settingsBackground.GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            renderer.enabled = true;

            for (int i = 0; i < settingsTMPs.Length; i++) {
                settingsTMPs[i] = GameObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                settingsTMPs[i].alignment = TMPro.TextAlignmentOptions.TopLeft;
                settingsTMPs[i].enableWordWrapping = false;
                settingsTMPs[i].transform.localScale = Vector3.one * 0.23f; 
                settingsTMPs[i].gameObject.SetActive(true);
            }
        }

        public static void CloseSettings() {
            foreach (var tmp in settingsTMPs)
                if (tmp) tmp.gameObject.Destroy();

            if (settingsBackground) settingsBackground.Destroy();
        }

        public static void ToggleSettings(HudManager __instance) {
            CustomOverlay.hideInfoOverlay();
            if (settingsTMPs[0]) CloseSettings();
            else OpenSettings(__instance);
        }

        static PassiveButton toggleSettingsButton;
        static GameObject toggleSettingsButtonObject;
        static GameObject toggleZoomButtonObject;
        static PassiveButton toggleZoomButton;
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance) {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (!toggleSettingsButton || !toggleSettingsButtonObject) {
                // add a special button for settings viewing:
                toggleSettingsButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                toggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                toggleSettingsButtonObject.name = "TOGGLESETTINGSBUTTON";
                SpriteRenderer renderer = toggleSettingsButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer rendererActive = toggleSettingsButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                toggleSettingsButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                renderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Settings_Button.png", 100f);
                rendererActive.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Settings_ButtonActive.png", 100);
                toggleSettingsButton = toggleSettingsButtonObject.GetComponent<PassiveButton>();
                toggleSettingsButton.OnClick.RemoveAllListeners();
                toggleSettingsButton.OnClick.AddListener((Action)(() => ToggleSettings(__instance)));
            }
            toggleSettingsButtonObject.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.HideNSeek);
            toggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -0.8f, -500f);


            if (!toggleZoomButton || !toggleZoomButtonObject)
            {
                // add a special button for settings viewing:
                toggleZoomButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                toggleZoomButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                toggleZoomButtonObject.name = "TOGGLEZOOMBUTTON";
                SpriteRenderer tZrenderer = toggleZoomButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer tZArenderer = toggleZoomButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                toggleZoomButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                tZrenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Minus_Button.png", 100f);
                tZArenderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Minus_ButtonActive.png", 100);
                toggleZoomButton = toggleZoomButtonObject.GetComponent<PassiveButton>();
                toggleZoomButton.OnClick.RemoveAllListeners();
                toggleZoomButton.OnClick.AddListener((Action)(() => Helpers.toggleZoom()));
            }
            if (CachedPlayer.LocalPlayer != null && CachedPlayer.LocalPlayer.PlayerControl != null)
            {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.PlayerControl.Data);
                int numberOfLeftTasks = playerTotal - playerCompleted;
                bool zoomButtonActive = !(CachedPlayer.LocalPlayer.PlayerControl == null || !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || (CachedPlayer.LocalPlayer.PlayerControl == Busker.busker && Busker.pseudocideFlag));
                zoomButtonActive &= numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.getBool();
                toggleZoomButtonObject.SetActive(zoomButtonActive);
                var posOffset = Helpers.zoomOutStatus ? new Vector3(-1.27f, -7.92f, -52f) : new Vector3(0, -1.6f, -52f);
                toggleZoomButtonObject.transform.localPosition = HudManager.Instance.MapButton.transform.localPosition + posOffset;
            }
        }
    }
}
