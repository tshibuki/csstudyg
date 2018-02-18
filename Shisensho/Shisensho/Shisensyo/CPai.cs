using System;

namespace Shisensho
{
    /// <summary>
    /// 牌クラス
    /// </summary>
    public class CPai
    {
        /// <summary>
        /// 牌の総数
        /// </summary>
        private const int maxPai = 136;
        /// <summary>
        /// 牌の総数の取得
        /// </summary>
        public int MaxPai
        {
            get
            {
                return maxPai;
            }
        }

        /// <summary>
        /// 牌の種類
        /// </summary>
        private const int maxKind = 9*3+7;
        /// <summary>
        /// 牌の種類の取得
        /// </summary>
        public int MaxKind
        {
            get
            {
                return maxKind;
            }
        }

        /// <summary>
        /// 牌
        /// </summary>
        private int[] kind = new int[maxPai];
        /// <summary>
        /// 
        /// </summary>
        public int[] Kind
        {
            get
            {
                return kind;
            }
        }

        /// <summary>
        /// 全ての牌に種類を割り当てる
        /// </summary>
        public void InitialState()
        {
            int id = 0;                 // 牌の種類

            for (int no = 0; no < maxPai; no++)
            {
                kind[no] = id / 4 + 1; // 牌の種類を設定する
                id++;
            }
        }

        /// <summary>
        /// 牌をかき混ぜる
        /// </summary>
        public void Shuffle()
        {
            int ranIdx;
            Random rand = new Random();

            InitialState();

            for (int inx = 0; inx < maxPai; inx++)
            {
                ranIdx = rand.Next(maxPai);
                if (ranIdx != inx)
                {
                    //場所の交換
                    int temp = kind[inx];
                    kind[inx] = kind[ranIdx];
                    kind[ranIdx] = temp;
                }
            }
        }
    }
}
