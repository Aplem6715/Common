using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;
using System;

namespace Aplem.Common
{
    using Random = Unity.Mathematics.Random;

    public static class RandomExtension
    {
        public delegate float GetWeightFunc<T>(T item);

        /// <summary>
        /// 入力されたRandomを元に新たなRandomを生成
        /// </summary>
        public static Random CreateBranch(this ref Random rand)
        {
            // NextUIntをシードにする。NextUIntはState-1を返すので
            // 元のRandom構造体とは別Stateになる。
            return new Random(rand.NextUInt());
        }

        /// <summary>
        /// 正規分布に従う乱数を生成
        /// </summary>
        /// <param name="rand">乱数生成器</param>
        /// <param name="std">標準偏差</param>
        public static float NextNormal(this ref Random rand, float mean, float std)
        {
            // 0~1の一様分布を12回重ねると分散が1(=偏差も1)になる
            float ret = 0;
            for (var i = 0; i < 12; i++)
                ret += rand.NextFloat();
            return (ret - 6) * std + mean;
        }

        /// <summary>
        /// 列挙体内のランダムな値を取得
        /// 大量の呼び出しは負荷・GCの原因になる可能性あり
        /// </summary>
        public static T NextEnum<T>(this ref Random rand) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(rand.NextInt(values.Length));
        }

        /// <summary>
        /// 小数点以下の値に応じてランダムに丸める
        /// </summary>
        /// <param name="value">丸める値</param>
        /// <returns>丸めた値</returns>
        public static int Round(this ref Random rand, float value)
        {
            var frac = math.frac(value);
            return (int)value + (rand.NextFloat() > frac ? 0 : 1);
        }

        /// <summary>
        /// 配列をシャッフルする
        /// </summary>
        /// <param name="array">シャッフルする配列</param>
        public static void Shuffle<T>(this ref Random rand, ref T[] array)
        {
            for (var i = array.Length - 1; i > 0; i--)
            {
                var j = rand.NextInt(i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        /// <summary>
        /// 重み付きランダムでリストのインデックスを取得
        /// </summary>
        /// <param name="weights">重みリスト</param>
        public static int ChoiceWeighted(this ref Random random, IReadOnlyList<float> weights)
        {
            float sum = 0;
            foreach (var weight in weights)
                sum += weight;

            var rand = random.NextFloat(sum);
            sum = 0;
            for (var i = 0; i < weights.Count; i++)
            {
                sum += weights[i];
                if (rand < sum)
                    return i;
            }

            throw new InvalidOperationException("Unreachable");
        }

        /// <summary>
        /// 重み付きランダムで配列のインデックスを取得
        /// </summary>
        /// <param name="array">配列</param>
        /// <param name="GetWeight">配列の要素から重みを取得する関数</param>
        /// <returns>インデックス</returns>
        public static int ChoiceWeighted<T>(this ref Random random, IReadOnlyList<T> array, GetWeightFunc<T> GetWeight)
        {
            float sum = 0;
            foreach (var item in array)
                sum += GetWeight(item);

            var rand = random.NextFloat(sum);
            sum = 0;
            for (var i = 0; i < array.Count; i++)
            {
                sum += GetWeight(array[i]);
                if (rand < sum)
                    return i;
            }

            throw new InvalidOperationException("Unreachable");
        }

        /// <summary>
        /// 重み付き乱数で辞書の要素を取得
        /// </summary>
        /// <param name="dict">辞書</param>
        /// <param name="GetWeight">辞書のPairから重みを取得する関数</param>
        /// <returns>選択されたValue</returns>
        public static V ChoiceWeighted<K, V>(this ref Random random, IReadOnlyDictionary<K, V> dict,
            GetWeightFunc<KeyValuePair<K, V>> GetWeight)
        {
            float sum = 0;
            foreach (var pair in dict)
                sum += GetWeight(pair);

            var rand = random.NextFloat(sum);
            sum = 0;
            foreach (var pair in dict)
            {
                sum += GetWeight(pair);
                if (rand < sum)
                    return pair.Value;
            }

            throw new InvalidOperationException("Unreachable");
        }

        /// <summary>
        /// id-weightペアの辞書から重み付きランダムでIDを取得
        /// </summary>
        /// <param name="weights">id-重み辞書</param>
        /// <param name="random">Randomインスタンス</param>
        /// <returns>選択されたID</returns>
        public static int ChoiceWeighted(this ref Random random, IReadOnlyDictionary<int, float> weights)
        {
            float sum = 0;
            foreach (var pair in weights)
                sum += pair.Value;

            var rand = random.NextFloat(sum);
            sum = 0;
            foreach (var pair in weights)
            {
                sum += pair.Value;
                if (rand < sum)
                    return pair.Key;
            }

            throw new InvalidOperationException("Unreachable");
        }

        /// <summary>
        /// 合計値が指定した値になるように配列を埋める
        /// </summary>
        /// <param name="result">結果を格納する配列</param>
        /// <param name="sum">生成する乱数の合計値</param>
        /// <param name="maxPerItem">要素ごとの最大値</param>
        public static void FillDistribute(this ref Random rand, ref int[] result, int sum, int maxPerItem)
        {
            var remain = sum;
            var numItems = result.Length;

            // for (int i = 0; i < numItems - 1; i++)
            // {
            //     int tier;
            //     float remainAve = remain / (float)(numItems - i - 1);
            //     if (remainAve >= maxPerItem)
            //     {
            //         tier = maxPerItem;
            //     }
            //     else
            //     {
            //         int max = math.min(remain, maxPerItem);
            //         if (max <= 0)
            //         {
            //             tier = 0;
            //         }
            //         else
            //         {
            //             float norm = rand.NextNormal(remainAve, 0.1f);
            //             tier = rand.Round(norm);
            //             tier = math.clamp(tier, 0, max);
            //         }
            //     }

            //     result[i] = tier;
            //     remain -= tier;
            // }

            // result[numItems - 1] = remain;

            for (var i = 0; i < numItems; i++)
                result[i] = math.clamp(rand.Round(rand.NextNormal(sum / (float)result.Length, 0.2f)), 0, maxPerItem);

            rand.Shuffle(ref result);
        }
    }
}
