using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public class RolloverMessage
    {
        public bool isActive { get; private set; }

        private TMPro.TMP_Text text;
        private string currentText;
        private static List<RolloverMessage> RolloverMessages = new();

        public Vector3? velocity;
        //テキストをランダムに入れ替える間隔 0より大きい場合に入れ替える
        public float textSwapDuration;
        //テキストの入れ替え量
        public int textSwapGain;
        //次のテキスト入れ替え時間
        private float textSwapLeft;

        //フォントサイズの変化速度
        public Vector3? textSizeVelocity;

        public static void Initialize()
        {
            foreach (RolloverMessage message in RolloverMessages)
            {
                UnityEngine.Object.Destroy(message.text);
            }
            RolloverMessages.Clear();
        }

        private static void UpdateTextAlpha(ref TMPro.TMP_Text text, float phase, float fadeIn, float fadeOut)
        {
            if (fadeIn > phase)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a * (text.color.a * phase / fadeIn));
            }
            else if (1f - fadeOut < phase)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a * (1f - phase) / fadeOut);
            }
        }

        //指定位置に現れる
        public static RolloverMessage Create(Vector3 position, bool stickingMap, string message, float duration, float fadeInDuration, float fadeOutDuration, float fontSizeRate, Color color1, Color? color2 = null)
        {
            return new RolloverMessage(position, stickingMap, message, duration, fadeInDuration, fadeOutDuration, fontSizeRate, color1, color2);
        }

        //ランダムな位置に現れて動く
        public static RolloverMessage Create(float minRadius, float maxRadius, float xRate, Vector3? velocity, string message, float duration, float fadeInDuration, float fadeOutDuration, float fontSizeRate, Color color1, Color? color2 = null)
        {
            double radius = minRadius + TheOtherRoles.rnd.NextDouble() * (maxRadius - minRadius);
            double angle = TheOtherRoles.rnd.NextDouble() * Math.PI * 2f;
            RolloverMessage result = new(new Vector3((float)(xRate * Math.Cos(angle) * radius), (float)(Math.Sin(angle) * radius)),
                false, message, duration, fadeInDuration, fadeOutDuration, 1f, color1, color2)
            {
                velocity = velocity
            };
            return result;
        }

        //下方に現れる
        public static RolloverMessage Create(string message, float duration, float fadeInDuration, float fadeOutDuration, Color color1, Color? color2 = null)
        {
            return new RolloverMessage(new Vector3(0, -1.8f, 0), false, message, duration, fadeInDuration, fadeOutDuration, 1f, color1, color2);
        }



        public RolloverMessage(Vector3 position, bool stickingMap, string message, float duration, float fadeInDuration, float fadeOutDuration, float fontSizeRate, Color color1, Color? color2 = null)
        {

            velocity = null;
            textSwapDuration = 0;
            textSwapGain = 0;
            textSwapLeft = 0;
            textSizeVelocity = null;
            isActive = true;

            RoomTracker roomTracker = HudManager.Instance.roomTracker;

            if (roomTracker != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(HudManager.Instance.roomTracker.gameObject);

                if (!stickingMap)
                {
                    gameObject.transform.SetParent(HudManager.Instance.transform);
                }

                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                text = gameObject.GetComponent<TMPro.TMP_Text>();

                String originalText = message;
                currentText = originalText;
                text.text = originalText;

                text.transform.localScale *= fontSizeRate;

                gameObject.transform.localPosition = new Vector3(0f, 0f, gameObject.transform.localPosition.z) + position;

                RolloverMessages.Add(this);

                float sum = fadeInDuration + duration + fadeOutDuration;
                HudManager.Instance.StartCoroutine(Effects.Lerp(sum, new Action<float>((p) =>
                {
                    if (velocity != null)
                    {
                        gameObject.transform.localPosition += (Vector3)velocity * Time.deltaTime;
                    }
                    if (text != null)
                    {
                        //テキストのランダム入れ替え
                        if (textSwapDuration > 0f && textSwapGain > 0)
                        {
                            textSwapLeft -= Time.deltaTime;
                            if (textSwapLeft < 0)
                            {
                                char[] charArray = originalText.ToCharArray();
                                char temp;
                                int rnd1, rnd2;

                                //入れ替え
                                for (int i = 0; i < textSwapGain; i++)
                                {
                                    rnd1 = TheOtherRoles.rnd.Next(charArray.Length);
                                    rnd2 = TheOtherRoles.rnd.Next(charArray.Length);

                                    temp = charArray[rnd1];
                                    charArray[rnd1] = charArray[rnd2];
                                    charArray[rnd2] = temp;
                                }
                                currentText = new string(charArray);

                                textSwapLeft = textSwapDuration;
                            }
                        }

                        if (color2 != null)
                        {
                            bool even = ((int)(p * sum / 0.25f)) % 2 == 0; // Bool flips every 0.25 seconds
                            text.color = even ? color1 : (Color)color2;
                        }
                        else
                        {
                            text.color = color1;
                        }

                        if (textSizeVelocity != null)
                        {
                            text.transform.localScale += (Vector3)textSizeVelocity * Time.deltaTime;
                        }

                        UpdateTextAlpha(ref text, p, fadeInDuration / sum, fadeOutDuration / sum);
                        text.text = Helpers.cs(text.color, currentText);

                    }

                    //消去する
                    if (p == 1f && text != null && text.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(text.gameObject);
                        RolloverMessages.Remove(this);
                        isActive = false;
                    }
                })));
            }
        }

        private IEnumerator getDestroyer()
        {
            UnityEngine.Object.Destroy(text.gameObject);
            RolloverMessages.Remove(this);
            isActive = false;
            yield break;
        }

        public RolloverMessage(Vector3 position, bool stickingMap, string message, IEnumerator updater, float fontSizeRate, Color color)
        {
            velocity = null;
            textSwapDuration = 0;
            textSwapGain = 0;
            textSwapLeft = 0;
            textSizeVelocity = null;
            isActive = true;

            RoomTracker roomTracker = HudManager.Instance.roomTracker;

            if (roomTracker != null)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(HudManager.Instance.roomTracker.gameObject);

                if (!stickingMap)
                {
                    gameObject.transform.SetParent(HudManager.Instance.transform);
                }

                UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());

                text = gameObject.GetComponent<TMPro.TMP_Text>();

                string originalText = message;
                currentText = originalText;
                text.text = originalText;
                text.color = color;

                text.transform.localScale *= fontSizeRate;

                Vector3 currentPosition = Camera.main.transform.position;

                gameObject.transform.localPosition = new Vector3(0f, 0f, gameObject.transform.localPosition.z) + position;

                RolloverMessages.Add(this);

                HudManager.Instance.StartCoroutine(Effects.Sequence(
                    new Il2CppReferenceArray<Il2CppSystem.Collections.IEnumerator>(
                        [
                        updater.WrapToIl2Cpp(),
                        getDestroyer().WrapToIl2Cpp()
                        ]
                    )));
            }
        }

        public void SetText(string text)
        {
            if (this.text)
            {
                currentText = text;
            }
        }
    }
}
