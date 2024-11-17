using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheOtherRoles.Modules;
using TheOtherRoles;
using static TheOtherRoles.TheOtherRoles;
using System;

namespace TheOtherRoles.Objects
{
    public sealed class FoxTask : Minigame
    {
        private GameObject obj;
        private TextMeshProUGUI RemainingTime;
        private TextMeshProUGUI TaskText;
        private Button closeButton;
        private bool completed = false;
        private float timer = 0;
        public static GameObject prefab;
        public static Sprite shrine;

        public void Awake()
        {
            if (obj != null) {
                Destroy(obj);
            }
            obj = Instantiate(prefab, transform);
            List<TextMeshProUGUI> texts = obj.GetComponentsInChildren<TextMeshProUGUI>().ToList();
            RemainingTime = texts.FirstOrDefault(x => x.name == "RemainingTime");
            TaskText = texts.FirstOrDefault(x => x.name == "TaskText");
            TaskText.text = ModTranslation.getString("foxTaskPray");
            closeButton = obj.GetComponentsInChildren<Button>().ToList().FirstOrDefault(x => x.name == "CloseButton");
            closeButton.onClick = new Button.ButtonClickedEvent();
            closeButton.onClick.AddListener((Action)(() => onClick()));
            obj.SetActive(true);
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
                StartCoroutine(CoDestroySelf());
            }
        }

        public void OnEnable()
        {
            enabled = true;
            completed = false;
            timer = Fox.stayTime;
        }

        public void OnDisable()
        {
            obj.SetActive(false);
        }

        void onClick()
        {
            StartCoroutine(CoDestroySelf());
        }
    }
}
