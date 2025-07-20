using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Hazel;
using Innersloth.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheOtherRoles.MetaContext;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles.Modules
{
    public interface ILifespan
    {
        bool IsDeadObject { get; }
        bool IsAliveObject { get => !IsDeadObject; }
    }

    public interface IReleasable
    {
        internal void Release();
    }

    abstract public class AchievementTokenBase : ILifespan, IReleasable
    {
        public Achievement Achievement { get; private init; }
        abstract public Achievement.ClearState UniteTo(bool update = true);

        public AchievementTokenBase(Achievement achievement)
        {
            this.Achievement = achievement;
            Achievement.allAchievementTokens.Add(this);
        }
        public bool IsDeadObject { get; private set; } = false;

        public void Release()
        {
            IsDeadObject = true;
            Achievement.allAchievementTokens.Remove(this);
        }
    }

    public abstract class DataEntry<T> where T : notnull
    {
        private T value;
        string name;
        DataSaver saver;

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                saver.SetValue(name, Serialize(value));
            }
        }

        public void SetValueWithoutSave(T value)
        {
            this.value = value;
            saver.SetValue(name, Serialize(value), true);
        }

        public DataEntry(string name, DataSaver saver, T defaultValue)
        {
            this.name = name;
            this.saver = saver;
            value = Parse(saver.GetValue(name, Serialize(defaultValue)));
        }

        public abstract T Parse(string str);
        protected virtual string Serialize(T value) => value.ToString()!;
    }

    public class DataSaver
    {
        private Dictionary<string, string> contents = new();
        string filename;

        public string GetValue(string name, object defaultValue)
        {
            if (contents.TryGetValue(name, out string value))
            {
                return value!;
            }
            var res = contents[name] = defaultValue.ToString()!;
            return res;
        }

        public void SetValue(string name, object value, bool skipSave = false)
        {
            contents[name] = value.ToString()!;
            if (!skipSave) Save();
        }

        public DataSaver(string filename)
        {
            this.filename = "TheOtherRolesGMIA\\" + filename + ".dat";
            Load();
        }

        public void Load()
        {
            string dataPathTo = FileIO.GetDataPathTo([filename]);

            if (!FileIO.Exists(dataPathTo)) return;

            string[] vals = (FileIO.ReadAllText(dataPathTo)).Split("\n");
            foreach (string val in vals)
            {
                string[] str = val.Split(":", 2);
                if (str.Length != 2) continue;
                contents[str[0]] = str[1];
            }
        }

        public void Save()
        {
            string strContents = "";
            foreach (var entry in contents)
            {
                strContents += entry.Key + ":" + entry.Value + "\n";
            }
            FileIO.WriteAllText(FileIO.GetDataPathTo([filename]), strContents);
        }
    }

    public class StaticAchievementToken : AchievementTokenBase
    {
        public StaticAchievementToken(string achievement)
            : base(Achievement.GetAchievement(achievement, out var a) ? a : null!) { }


        public override Achievement.ClearState UniteTo(bool update)
        {
            if (IsDeadObject) return Achievement.ClearState.NotCleared;

            return Achievement.Unite(1, update);
        }
    }

    public class AchievementToken<T> : AchievementTokenBase
    {
        public T Value;
        public Func<T, Achievement, int> Supplier { get; set; }

        public AchievementToken(Achievement achievement, T value, Func<T, Achievement, int> supplier) : base(achievement)
        {
            Value = value;
            Supplier = supplier;
        }

        public AchievementToken(string achievement, T value, Func<T, Achievement, int> supplier)
            : this(Achievement.GetAchievement(achievement, out var a) ? a : null!, value, supplier) { }

        public AchievementToken(string achievement, T value, Func<T, Achievement, bool> supplier)
            : this(achievement, value, (t, ac) => supplier.Invoke(t, ac) ? 1 : 0) { }


        public override Achievement.ClearState UniteTo(bool update)
        {
            if (IsDeadObject) return Achievement.ClearState.NotCleared;

            return Achievement.Unite(Supplier.Invoke(Value, Achievement), update);
        }
    }

    public class IntegerDataEntry : DataEntry<int>
    {
        public override int Parse(string str) { return int.Parse(str); }
        public IntegerDataEntry(string name, DataSaver saver, int defaultValue) : base(name, saver, defaultValue) { }
    }

    public class StringDataEntry : DataEntry<string>
    {
        public override string Parse(string str) { return str; }
        public StringDataEntry(string name, DataSaver saver, string defaultValue) : base(name, saver, defaultValue) { }
    }

    public class AchievementType
    {
        static public AchievementType Challenge = new("challenge");
        static public AchievementType Secret = new("secret");

        private AchievementType(string key)
        {
            TranslationKey = key + "AchievementType";
        }
        public string TranslationKey { get; private set; }
    }

    public class Achievement
    {
        static public IDividedSpriteLoader TrophySprite = XOnlyDividedSpriteLoader.FromResource("TheOtherRoles.Resources.Trophy.png", 100f, 3);
        public static AchievementToken<(bool isCleared, bool triggered)> GenerateSimpleTriggerToken(string achievement) => new(achievement, (false, false), (val, _) => val.isCleared);

        int goal;
        bool canClearOnce;
        bool isSecret;
        bool noHint;
        string key;
        string hashedKey;
        public (RoleInfo role, AchievementType type)? Category { get; private init; }
        IntegerDataEntry entry;
        public int Trophy { get; private init; }
        public bool IsCleared => goal <= entry.Value;

        static public TextComponent HiddenComponent = new RawTextComponent("???");
        static public TextComponent HiddenDescriptiveComponent = new ColorTextComponent(new Color(0.4f, 0.4f, 0.4f), new TranslateTextComponent("achievementTitleHidden"));
        static public TextComponent HiddenDetailComponent = new ColorTextComponent(new Color(0.8f, 0.8f, 0.8f), new TranslateTextComponent("achievementTitleHiddenDetail"));

        public Achievement(bool canClearOnce, bool isSecret, bool noHint, string key, int goal, (RoleInfo role, AchievementType type)? category, int trophy)
        {
            this.goal = goal;
            this.isSecret = isSecret;
            this.canClearOnce = canClearOnce;
            this.noHint = noHint;
            this.key = key;
            this.hashedKey = key.ComputeConstantHashAsString();
            this.Category = category;
            this.Trophy = trophy;
            this.entry = new IntegerDataEntry("a." + hashedKey, AchievementDataSaver, 0);
            RegisterAchievement(this, key);
        }

        public bool IsHidden
        {
            get
            {
                return isSecret && !IsCleared;
            }
        }

        public static Shader materialShader;

        static public (int num, int max, int hidden)[] Aggregate()
        {
            (int num, int max, int hidden)[] result = new (int num, int max, int hidden)[3];
            for (int i = 0; i < result.Length; i++) result[i] = (0, 0, 0);
            return achievements.Values.Aggregate(result,
                (ac, achievement) => {
                    if (!achievement.IsHidden)
                    {
                        ac[achievement.Trophy].max++;
                        if (achievement.IsCleared) ac[achievement.Trophy].num++;
                    }
                    else
                    {
                        ac[achievement.Trophy].hidden++;
                    }
                    return ac;
                });
        }

        static public void LoadAchievements()
        {
            using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("TheOtherRoles.Resources.Achievements.dat")!);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;

                var args = line.Split(',');

                if (args.Length < 2) continue;

                bool clearOnce = false;
                bool noHint = false;
                bool secret = false;
                int rarity = int.Parse(args[1]);
                int goal = 1;
                for (int i = 2; i < args.Length - 1; i++)
                {
                    var arg = args[i];

                    switch (arg)
                    {
                        case "once":
                            clearOnce = true;
                            break;
                        case "noHint":
                            noHint = true;
                            break;
                        case "secret":
                            secret = true;
                            break;
                        case string a when a.StartsWith("goal-"):
                            goal = int.Parse(a.Substring(5));
                            break;
                    }
                }

                RoleInfo relatedRole = null;
                AchievementType type = null;

                var nameSplitted = args[0].Split('.');
                if (nameSplitted.Length > 1)
                {
                    nameSplitted[0] = nameSplitted[0].Replace('-', '.');
                    var cand = RoleInfo.allRoleInfos.Where(a => a.nameKey == nameSplitted[0]).ToArray();
                    if (cand.Length >= 1)
                    {
                        relatedRole = cand[0];
                        if (rarity == 2) type = AchievementType.Challenge;
                        else if (secret) type = AchievementType.Secret;
                    }
                }
                new Achievement(clearOnce, secret, noHint, args[0], goal, (relatedRole, type), rarity);
            }
        }

        static private Dictionary<string, Achievement> achievements = new();
        static public Dictionary<byte, Achievement> TitleMap = new();

        public static IEnumerable<Achievement> allAchievements => achievements.Values;
        public static List<AchievementTokenBase> allAchievementTokens = new();
        static public DataSaver AchievementDataSaver = new("Progress");
        static private StringDataEntry myTitleEntry = new("MyTitle", AchievementDataSaver, "-");

        static public Achievement MyTitle
        {
            get
            {
                if (GetAchievement(myTitleEntry.Value, out var achievement) && achievement.IsCleared)
                    return achievement;
                return null;
            }
            set
            {
                if (value?.IsCleared ?? false)
                    myTitleEntry.Value = value.Id;
                else
                    myTitleEntry.Value = "-";

                if (PlayerControl.LocalPlayer && !ShipStatus.Instance && PlayerControl.LocalPlayer.AmOwner)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareAchievement, SendOption.Reliable);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(myTitleEntry.Value);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.shareAchievement(PlayerControl.LocalPlayer.PlayerId, myTitleEntry.Value);
                }
            }
        }

        static public void SetOrToggleTitle(Achievement achievement)
        {
            if (achievement == null || MyTitle == achievement)
                MyTitle = null;
            else
                MyTitle = achievement;
        }

        public enum ClearState
        {
            Clear,
            FirstClear,
            NotCleared
        }
        public ClearState Unite(int localValue, bool update)
        {
            if (localValue < 0) return ClearState.NotCleared;

            int lastValue = entry.Value;
            int newValue = Math.Min(goal, lastValue + localValue);
            if (update) entry.Value = newValue;

            if (newValue >= goal && lastValue < goal)
                return ClearState.FirstClear;

            if (localValue >= goal && !canClearOnce)
                return ClearState.Clear;

            return ClearState.NotCleared;
        }
        public string Id => key;
        public string TranslateKeyInfo
        {
            get
            {
                return key.Replace(".", "");
            }
        }
        public string TranslationKey => TranslateKeyInfo + "AchievementTitle";
        public string GoalTranslationKey => TranslateKeyInfo + "AchievementGoal";
        public string CondTranslationKey => TranslateKeyInfo + "AchievementCond";

        public TextComponent GetHeaderComponent()
        {
            List<TextComponent> list = new();

            if (Category?.role != null)
            {
                list.Add(TORGUIContextEngine.Instance.TextComponent((Color)Category?.role.color, Category?.role.name));
            }

            if (Category?.type != null)
            {
                if (list.Count != 0) list.Add(new RawTextComponent(" "));
                list.Add(new TranslateTextComponent(Category?.type.TranslationKey));
            }

            if (list.Count > 0)
                return new CombinedTextComponent(list.ToArray());
            else
                return null;
        }

        public GUIContext GetOverlayContext(bool hiddenNotClearedAchievement = true, bool showCleared = false, bool showTitleInfo = false, bool showTorophy = false)
        {
            var gui = TORGUIContextEngine.API;

            var attr = new TextAttributes(gui.GetAttribute(AttributeParams.OblongLeft)) { FontSize = new(1.6f) };
            var detailTitleAttr = new TextAttributes(gui.GetAttribute(AttributeParams.StandardBaredBoldLeft)) { FontSize = new(1.8f) };
            var detailDetailAttr = new TextAttributes(gui.GetAttribute(AttributeParams.StandardBaredLeft)) { FontSize = new(1.5f), Size = new(5f, 6f) };

            List<GUIContext> list = new()
            {
                new TORGUIText(GUIAlignment.Left, detailDetailAttr, GetHeaderComponent())
            };
            List<GUIContext> titleList = new();
            if (showTorophy)
            {
                titleList.Add(new TORGUIMargin(GUIAlignment.Left, new(-0.04f, 0.2f)));
                titleList.Add(new TORGUIImage(GUIAlignment.Left, new WrapSpriteLoader(() => TrophySprite.GetSprite(Trophy)), new(0.3f, 0.3f)));
                titleList.Add(new TORGUIMargin(GUIAlignment.Left, new(0.05f, 0.2f)));
            }
            titleList.Add(new TORGUIText(GUIAlignment.Left, detailTitleAttr, GetTitleComponent(hiddenNotClearedAchievement ? HiddenDescriptiveComponent : null)));

            if (showCleared && IsCleared)
            {
                titleList.Add(new TORGUIMargin(GUIAlignment.Left, new(0.2f, 0.2f)));
                titleList.Add(new TORGUIText(GUIAlignment.Left, detailDetailAttr, gui.TextComponent(new(1f, 1f, 0f), "achievementCleared")));
            }

            list.Add(new HorizontalContextsHolder(GUIAlignment.Left, titleList));
            list.Add(new TORGUIText(GUIAlignment.Left, detailDetailAttr, GetDetailComponent()));

            if (showTitleInfo && IsCleared)
            {
                list.Add(new TORGUIMargin(GUIAlignment.Left, new(0f, 0.2f)));
                list.Add(new TORGUIText(GUIAlignment.Left, detailDetailAttr, new LazyTextComponent(() =>
                (MyTitle == this) ?
                ("<b>" + ModTranslation.getString("achievementEquipped").Color(Color.green) + "</b><br>" + ModTranslation.getString("achievementUnequip")) :
                ModTranslation.getString("achievementEquip"))));
            }

            return new VerticalContextsHolder(GUIAlignment.Left, list);
        }

        public TextComponent GetTitleComponent(TextComponent hiddenComponent)
        {
            if (hiddenComponent != null && !IsCleared)
                return hiddenComponent;
            return new TranslateTextComponent(TranslationKey);
        }

        public TextComponent GetDetailComponent()
        {
            List<TextComponent> list = new();
            if (!noHint || IsCleared)
                list.Add(new TranslateTextComponent(GoalTranslationKey));
            else
                list.Add(HiddenDetailComponent);
            list.Add(new LazyTextComponent(() =>
            {
                StringBuilder builder = new();
                var cond = ModTranslation.getString(CondTranslationKey);
                if (cond.Length > 0)
                {
                    builder.Append("<size=75%><br><br>");
                    builder.Append(ModTranslation.getString("achievementCond"));
                    foreach (var c in cond.Split('+'))
                    {
                        builder.Append("<br>   ");
                        builder.Append(c);
                    }
                    builder.Append("</size>");
                }
                return builder.ToString();
            }));

            return new CombinedTextComponent(list.ToArray());
        }

        static public void RegisterAchievement(Achievement achievement, string id)
        {
            achievements[id] = achievement;
        }

        static public bool GetAchievement(string id, [MaybeNullWhen(false)] out Achievement achievement)
        {
            return achievements.TryGetValue(id, out achievement);
        }

        static public (Achievement achievement, ClearState clearState)[] UniteAll()
        {
            List<(Achievement achievement, ClearState clearState)> result = new();

            foreach (var token in allAchievementTokens)
            {
                var state = token.UniteTo();
                if (state == ClearState.NotCleared) continue;
                result.Add(new(token.Achievement, state));
            }

            result.OrderBy(val => val.clearState);

            return result.DistinctBy(a => a.achievement).ToArray();
        }

        static XOnlyDividedSpriteLoader trophySprite = XOnlyDividedSpriteLoader.FromResource("TheOtherRoles.Resources.Trophy.png", 220f, 3);
        static public IEnumerator CoShowAchievements(MonoBehaviour coroutineHolder, params (Achievement achievement, ClearState clearState)[] achievements)
        {
            int num = 0;
            (GameObject holder, GameObject animator, GameObject body, SpriteRenderer white) CreateBillboard(Achievement achievement, ClearState clearState)
            {
                var billboard = Helpers.CreateObject("Billboard", null, new Vector3(3.85f, 1.75f - (float)num * 0.6f, -100f));
                var animator = Helpers.CreateObject("Animator", billboard.transform, new Vector3(0f, 0f, 0f));
                var body = Helpers.CreateObject("Body", animator.transform, new Vector3(0f, 0f, 0f));
                var background = Helpers.CreateObject<SpriteRenderer>("Background", body.transform, new Vector3(0f, 0f, 1f));
                var white = Helpers.CreateObject<SpriteRenderer>("White", animator.transform, new Vector3(0f, 0f, -2f));
                var icon = Helpers.CreateObject<SpriteRenderer>("Icon", body.transform, new Vector3(-0.95f, 0f, 0f));

                background.color = clearState == ClearState.FirstClear ? Color.yellow : new Color(0.7f, 0.7f, 0.7f);
                billboard.AddComponent<SortingGroup>();

                new MetaContextOld.Text(new(TextAttribute.BoldAttr) { Font = VanillaAsset.BrookFont, Size = new(2f, 0.4f), FontSize = 1.16f, FontMaxSize = 1.16f, FontMinSize = 1.16f }) { MyText = achievement.GetHeaderComponent() }.Generate(body, new Vector2(0.25f, 0.13f), out _);
                new MetaContextOld.Text(new(TextAttribute.NormalAttr) { Font = VanillaAsset.BrookFont, Size = new(2f, 0.4f) }) { MyText = achievement.GetTitleComponent(null) }.Generate(body, new Vector2(0.25f, -0.06f), out _);

                foreach (var renderer in new SpriteRenderer[] { background, white })
                {
                    renderer.sprite = VanillaAsset.TextButtonSprite;
                    renderer.drawMode = SpriteDrawMode.Sliced;
                    renderer.tileMode = SpriteTileMode.Continuous;
                    renderer.size = new Vector2(2.6f, 0.55f);
                }
                num++;

                var collider = billboard.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(2.6f, 0.55f);
                var button = billboard.SetUpButton();
                button.OnMouseOver.AddListener((Action)(() => TORGUIManager.Instance.SetHelpContext(button, achievement.GetOverlayContext(showTitleInfo: true))));
                button.OnMouseOut.AddListener((Action)(() => TORGUIManager.Instance.HideHelpContextIf(button)));
                button.OnClick.AddListener((Action)(() => {
                    SetOrToggleTitle(achievement);
                    button.OnMouseOut.Invoke();
                    button.OnMouseOver.Invoke();
                }));

                white.material.shader = materialShader;
                icon.sprite = trophySprite.GetSprite(achievement.Trophy);

                return (billboard, animator, body, white);
            }

            IEnumerator CoShowFirstClear((GameObject holder, GameObject animator, GameObject body, SpriteRenderer white) billboard)
            {
                IEnumerator Shake(Transform target, float duration, float halfWidth)
                {
                    Vector3 origin = target.localPosition;
                    for (float timer = 0f; timer < duration; timer += Time.deltaTime)
                    {
                        float num = timer / duration;
                        Vector3 vector = UnityEngine.Random.insideUnitCircle * halfWidth;
                        target.localPosition = origin + vector;
                        yield return null;
                    }
                    target.localPosition = origin;
                    yield break;
                }

                billboard.body.SetActive(false);
                billboard.holder.transform.localScale = Vector3.one * 1.1f;


                coroutineHolder.StartCoroutine(Helpers.Sequence(
                    Shake(billboard.animator.transform, 0.1f, 0.01f),
                    Shake(billboard.animator.transform, 0.2f, 0.02f),
                    Shake(billboard.animator.transform, 0.3f, 0.03f),
                    Shake(billboard.animator.transform, 0.3f, 0.04f)
                    ));

                float t;

                t = 0f;
                while (t < 0.9f)
                {
                    billboard.holder.transform.localScale = Vector3.one * (1.1f + (t / 0.9f * 0.2f));
                    billboard.white.color = new Color(1f, 1f, 1f, t / 0.9f);
                    t += Time.deltaTime;
                    yield return null;
                }

                billboard.body.SetActive(true);

                float p = 1f;
                while (p > 0.0001f)
                {
                    billboard.holder.transform.localScale = Vector3.one * (1f + (p * 0.2f));
                    billboard.white.color = new Color(1f, 1f, 1f, p);
                    p -= p * 5f * Time.deltaTime;
                    yield return null;
                }

                billboard.white.gameObject.SetActive(false);
                billboard.holder.transform.localScale = Vector3.one;
            }

            IEnumerator CoShowClear((GameObject holder, GameObject animator, GameObject body, SpriteRenderer white) billboard)
            {
                billboard.white.gameObject.SetActive(false);
                float p = 3f;
                while (p > 0.0001f)
                {
                    billboard.animator.transform.localPosition = new Vector3(p, 0f, 0f);
                    p -= p * 8f * Time.deltaTime;
                    yield return null;
                }
                billboard.animator.transform.localPosition = Vector3.zero;
            }


            yield return new WaitForSeconds(1.5f);

            foreach (var ach in achievements)
            {
                var billboard = CreateBillboard(ach.achievement, ach.clearState);
                if (ach.clearState == ClearState.FirstClear)
                {
                    coroutineHolder.StartCoroutine(CoShowFirstClear(billboard).WrapToIl2Cpp());
                    yield return new WaitForSeconds(1.05f);
                }
                else
                {
                    coroutineHolder.StartCoroutine(CoShowClear(billboard).WrapToIl2Cpp());
                    yield return new WaitForSeconds(0.45f);
                }
                yield return new WaitForSeconds(1.05f);
                yield return null;
            }

            yield break;
        }
    }
}
