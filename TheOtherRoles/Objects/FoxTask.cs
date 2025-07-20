using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheOtherRoles.Modules;
using static TheOtherRoles.TheOtherRoles;
using System;
using TheOtherRoles.MetaContext;
using System.Collections;
using Il2CppSystem.Collections;
using TheOtherRoles.Roles;

namespace TheOtherRoles.Objects
{
    public sealed class FoxTask : Minigame
    {
        static FoxTask() => ClassInjector.RegisterTypeInIl2Cpp<FoxTask>();
        public FoxTask(System.IntPtr ptr) : base(ptr) { }
        public FoxTask() : base(ClassInjector.DerivedConstructorPointer<FoxTask>())
        { ClassInjector.DerivedConstructorBody(this); }

        private GameObject obj;
        private TextMeshProUGUI RemainingTime;
        private TextMeshProUGUI TaskText;
        private bool completed = false;
        private float timer = 0;
        public static GameObject prefab;
        public static Sprite shrine;

        public override void Begin(PlayerTask task)
        {
            this.BeginInternal(task);
            obj = Instantiate(prefab, transform);
            // new(-2.35f, 1.18f, -1000f)
            MetaScreen.InstantiateCloseButton(obj.transform, new(-3.3f, 1.8f, -0.5f)).OnClick.AddListener((Action)(() => Close()));
            List<TextMeshProUGUI> texts = obj.GetComponentsInChildren<TextMeshProUGUI>().ToList();
            RemainingTime = texts.FirstOrDefault(x => x.name == "RemainingTime");
            TaskText = texts.FirstOrDefault(x => x.name == "TaskText");
            TaskText.text = ModTranslation.getString("foxTaskPray");
            timer = Fox.stayTime;
            completed = false;
            enabled = true;
        }

        private void FixedUpdate()
        {
            if (timer > 0)
            {
                RemainingTime.text = string.Format(ModTranslation.getString("foxTimeRemaining"), $"{timer:n}");
                timer -= Time.fixedDeltaTime;
            }
            else if (!completed)
            {
                completed = true;
                obj.SetActive(false);
                MyNormTask.NextStep();
                CustomNormalPlayerTask.completedConsoles.Add(ConsoleId);
                if (MyNormTask.taskStep < MyNormTask.MaxStep)
                {
                    var console = ShipStatus.Instance.AllConsoles.FirstOrDefault(x => x.ConsoleId == MyNormTask.Data[MyNormTask.taskStep]);
                    MyNormTask.StartAt = console.Room;
                }
                Close();
            }
        }

        public override void Close()
        {
            this.CloseInternal();
        }
    }
}
