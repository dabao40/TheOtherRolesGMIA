using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOtherRoles.Objects
{
    public abstract class MapData
    {
        abstract protected Vector2[] MapArea { get; }

        public bool CheckMapArea(Vector2 position, float radius = 0.1f)
        {
            if (radius > 0f)
            {
                int num = Physics2D.OverlapCircleNonAlloc(position, radius, PhysicsHelpers.colliderHits, Constants.ShipAndAllObjectsMask);
                if (num > 0) for (int i = 0; i < num; i++) if (!PhysicsHelpers.colliderHits[i].isTrigger) return false;
            }

            return CheckMapAreaInternal(position);
        }


        public bool CheckMapAreaInternal(Vector2 position)
        {
            Vector2 vector;
            float magnitude;

            foreach (Vector2 p in MapArea)
            {
                vector = p - position;
                magnitude = vector.magnitude;
                if (magnitude > 12.0f) continue;

                if (!PhysicsHelpers.AnyNonTriggersBetween(position, vector.normalized, magnitude, Constants.ShipAndAllObjectsMask)) return true;
            }

            return false;
        }

        static private MapData[] AllMapData = [new SkeldData(), new MiraData(), new PolusData(), null!, new AirshipData(), new FungleData()];
        static public MapData GetCurrentMapData() => AllMapData[GameOptionsManager.Instance.CurrentGameOptions.MapId];
    }

    public class FungleData : MapData
    {
        static private Vector2[] MapPositions =
            [ 
        //ドロップシップ
        new(-9.2f,13.4f),
        //カフェテリア
        new(-19.1f, 7.0f),new(-13.6f,5.0f),new(-20.5f,6.0f),
        //カフェ下
        new(-12.9f,2.3f),new(-21.7f,2.41f),
        //スプラッシュゾーン
        new(-20.2f,-0.3f),new(-19.8f,-2.1f),new(-16.1f,-0.1f),new(-15.6f,-1.8f),
        //キャンプファイア周辺
        new(-11.3f,2.0f),new(-0.83f,2.4f),new(-9.4f,0.2f),new(-6.9f,0.2f),
        //スプラッシュゾーン下
        new(-17.3f,-4.5f),
        //キッチン
        new(-15.4f,-9.5f),new(-17.4f,-7.5f),
        //キッチン・ジャングル間通路
        new(-11.2f,-6.1f),new(-5.5f,-14.8f),
        //ミーティング上
        new(-2.8f,2.2f),new(2.2f,1.0f),
        //ストレージ
        new(-0.6f,4.2f),new(2.3f,6.2f),new(3.3f,6.7f),
        //ミーティング・ドーム
        new(-0.15f,-1.77f),new(-4.65f,1.58f),new(-4.8f,-1.44f),
        //ラボ
        new(-7.1f,-11.9f),new(-4.5f,-6.8f),new(-3.3f,-8.9f),new(-5.4f,-10.2f),
        //ジャングル(左)
        new(-1.44f,-13.3f),new(3.8f,-12.5f),
        //ジャングル(中)
        new(7.08f,-15.3f),new(11.6f,-14.3f),
        //ジャングル(上)
        new(2.7f,-6.0f),new(12.1f,-7.3f),
        //グリーンハウス・ジャングル
        new(13.6f,-12.1f),new(6.4f,-10f),
        //ジャングル(右)
        new(15.0f,-6.7f),new(18.1f,-9.1f),
        //ジャングル(下)
        new(14.9f,-16.3f),
        //リアクター
        new(21.1f,-6.7f),
        //高台
        new(15.9f,0.4f),new(15.6f,4.3f),new(19.2f,1.78f),
        //鉱山
        new(12.5f,7.7f),new(13.4f,9.7f),
        //ルックアウト
        new(6.6f,3.8f),new(8.7f,1f),
        //梯子中間
        new(20.1f,7.2f),
        //コミュ
        new(20.9f,10.8f),new(24.1f,13.2f),new(17.9f,12.7f),
            ];

        protected override Vector2[] MapArea => MapPositions;
    }

    public class AirshipData : MapData
    {
        static private Vector2[] MapPositions =
            [ 
        //金庫
        new(-9f, 12.8f), new(-8.7f, 4.9f), new(-12.8f, 8.7f), new(-4.8f, 8.7f), new(-7.1f, 6.8f), new(-10.4f, 6.9f), new(-7f, 10.2f),
        //宿舎前
        new(-0.5f, 8.5f),
        //エンジン上
        new(-0.4f, 5f),
        //エンジン
        new(0f, -1.4f), new(3.6f, 0.1f), new(0.4f, -2.5f), new(-6.9f, 1.1f),
        //コミュ前
        new(-11f, -1f),
        //コミュ
        new(-12.3f, 0.9f),
        //コックピット
        new(-19.9f, -2.6f), new(-19.9f, 0.5f),
        //武器庫
        new(-14.5f, -3.6f), new(-9.9f, -6f), new(-15f, -9.4f),
        //キッチン
        new(-7.5f, -7.5f), new(-7f, -12.8f), new(-2.5f, -11.2f), new(-3.9f, -9.3f),
        //左展望
        new(-13.8f, -11.8f),
        //セキュ
        new(7.3f, -12.3f), new(5.8f, -10.6f),
        //右展望
        new(10.3f, -15f),
        //エレク
        new(10.5f, -8.5f),
        //エレクの9部屋
        new(10.5f, -6.3f), new(13.5f, -6.3f), new(16.5f, -6.3f), new(19.4f, -6.3f), new(13.5f, -8.8f), new(16.5f, -8.8f), new(19.4f, -8.8f), new(16.5f, -11f), new(19.4f, -11f),
        //エレク右上
        new(19.4f, -4.2f),
        //メディカル
        new(25.2f, -9.8f), new(22.9f, -6f), new(25.2f, -9.8f), new(29.5f, -6.3f),
        //貨物
        new(31.8f, -3.3f), new(34f, 1.4f), new(39f, -0.9f), new(37.6f, -3.4f), new(32.8f, 3.6f), new(35.3f, 3.6f),
        //ロミジュリ右
        new(29.8f, -1.5f),
        //ラウンジ
        new(33.7f, 7.1f), new(32.4f, 7.1f), new(30.9f, 7.1f), new(29.2f, 7.1f), new(30.8f, 5.3f), new(24.9f, 4.9f), new(27.1f, 7.3f),
        //レコード
        new(22.3f, 9.1f), new(20f, 11.5f), new(17.6f, 9.4f), new(20.1f, 6.6f),
        //ギャップ右
        new(15.4f, 9.2f), new(11.2f, 8.5f), new(12.6f, 6.2f),
        //シャワー/ロミジュリ左
        new(18.9f, 4.5f), new(17.2f, 5.2f), new(18.5f, 0f), new(21.2f, -2f), new(24f, 0.7f), new(22.3f, 2.5f),
        //メインホール
        new(10.8f, 0f), new(14.8f, 1.9f), new(11.8f, 1.8f), new(9.7f, 2.5f), new(6.2f, 2.4f), new(6.6f, -3f), new(12.7f, -2.9f),
        //ギャップ左
        new(3.8f, 8.8f),
        //ミーティング
        new(6.5f, 15.3f), new(11.8f, 14.1f), new(11.8f, 16f), new(16.3f, 15.2f),
            ];

        protected override Vector2[] MapArea => MapPositions;
    }

    public class PolusData : MapData
    {
        static private Vector2[] MapPositions =
        [
        //ドロップシップ
        new(16.7f, -2.6f),
        //ドロップシップ下
        new(14.1f, -10f), new(22.0f, -7.1f),
        //エレクトリカル
        new(7.5f, -9.7f), new(3.1f, -11.7f), new(5.4f, -11.5f), new(9.6f, -12.1f),
        //O2
        new(4.7f, -19f), new(2.4f, -17f), new(3.1f, -21.7f), new(1.9f, -19.4f), new(2.4f, -23.6f), new(6.3f, -21.3f),
        //Elec,O2,Comm周辺外
        new(7.9f, -23.6f), new(9.4f, -20.1f), new(8.2f, -16.0f), new(8.0f, -14.3f), new(13.4f, -13f),
        //左上リアクター前通路
        new(10.3f, -7.4f),
        //左上リアクター
        new(4.6f, -5f),
        //Comm
        new(11.4f, -15.9f), new(11.7f, -17.3f),
        //Weapons
        new(13f, -23.5f),
        //Storage
        new(19.4f, -11.2f),
        //オフィス左下
        new(18f, -24.5f),
        //オフィス
        new(18.6f, -21.5f), new(20.2f, -19.2f), new(19.6f, -17.6f), new(19.6f, -16.4f), new(26.5f, -17.4f),
        //アドミン
        new(20f, -22.5f), new(21.4f, -25.2f), new(22.4f, -22.6f), new(25f, -20.8f),
        //デコン（左）
        new(24.1f, -24.7f),
        //スペシメン左通路
        new(27.7f, -24.7f), new(33f, -20.6f),
        //スペシメン
        new(36.8f, -21.6f), new(36.5f, -19.3f),
        //スペシメン右通路
        new(39.2f, -15.2f),
        //デコン(上)
        new(39.8f, -10f),
        //ラボ
        new(34.7f, -10.2f), new(36.4f, -8f), new(40.5f, -7.6f), new(34.5f, -6.2f), new(31.2f, -7.6f), new(28.4f, -9.6f), new(26.5f, -7f), new(26.5f, -8.3f),
        //右リアクター
        new(24.2f, -4.5f),
        //ストレージ・ラボ下・オフィス右
        new(24f, -14.6f), new(26f, -12.2f), new(29.8f, -15.7f)
        ];

        protected override Vector2[] MapArea => MapPositions;
    }

    public class MiraData : MapData
    {
        static private Vector2[] MapPositions =
        [
        //ラウンチパッド
        new(-4.4f, 3.3f),
        //ランチパッド下通路
        new(3.7f, -1.7f),
        //メッドベイ
        new(15.2f, 0.4f),
        //コミュ
        new(14f, 4f),
        //三叉路
        new(12.3f, 6.7f), new(23.6f, 6.8f),
        //ロッカー
        new(9f, 5f), new(8.4f, 1.4f),
        //デコン
        new(6.0f, 5.6f),
        //デコン上通路
        new(6.0f, 11.6f),
        //リアクター
        new(2.5f, 10.3f), new(2.5f, 13f),
        //ラボラトリ
        new(7.6f, 13.9f), new(9.7f, 10.4f), new(10.7f, 12.2f),
        //カフェ
        new(21.8f, 5f), new(10.7f, 12.2f), new(28.3f, 0.2f), new(25.5f, 2.3f), new(22.1f, 2.6f),
        //ストレージ
        new(19.2f, 1.7f), new(18.5f, 4.2f),
        //バルコニー
        new(18.3f, -3.2f), new(23.7f, -1.9f),
        //三叉路上通路
        new(17.8f, 19f),
        //オフィス
        new(15.7f, 17.2f), new(13.7f, 20.4f), new(13.6f, 18.7f),
        //アドミン
        new(20.6f, 20.8f), new(22.3f, 18.6f), new(21.2f, 17.3f), new(19.4f, 17.6f),
        //グリーンハウス
        new(13.2f, 22.3f), new(22.4f, 23.3f), new(20.2f, 24.3f), new(16.5f, 24.4f), new(20.7f, 22.2f), new(18f, 25.3f),
        ];

        protected override Vector2[] MapArea => MapPositions;
    }

    public class SkeldData : MapData
    {
        static private Vector2[] MapPositions =
        [
        //カフェ
        new(0f, 5.3f), new(-5.2f, 1.2f), new(-0.9f, -3.1f), new(4.6f, 1.2f),
        //ウェポン
        new(10.1f, 3f),
        //コの字通路/O2
        new(9.6f, -3.4f), new(11.8f, -6.5f),
        //ナビ
        new(16.7f, -4.8f),
        //シールド
        new(9.3f, -10.3f), new(9.5f, -14.1f),
        //コミュ上
        new(5.2f, -12.2f),
        //コミュ
        new(3.8f, -15.4f),
        //ストレージ
        new(-0.3f, -9.8f), new(-0.28f, -16.4f), new(-4.5f, -14.3f),
        //エレク
        new(-9.6f, -11.3f), new(-7.5f, -8.4f),
        //ロアエンジン右
        new(-12.1f, -11.4f),
        //ロアエンジン
        new(-15.4f, -13.1f), new(-16.8f, -9.8f),
        //アッパーエンジン
        new(-16.8f, -1f), new(-15.2f, 2.4f),
        //セキュ
        new(-13.8f, -4.5f),
        //リアクター
        new(-20.9f, -5.4f),
        //メッドベイ
        new(-7.3f, -4.6f), new(-9.2f, -2.1f),
        //アドミン
        new(2.6f, -7.1f), new(6.3f, -9.5f)
        ];
        protected override Vector2[] MapArea => MapPositions;
    }
}
