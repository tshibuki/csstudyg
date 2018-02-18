using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shisensho
{
    /// <summary>
    /// 場クラス
    /// </summary>
    public class CField
    {
        /// <summary>
        /// 牌
        /// </summary>
        private CPai pai = new CPai();
        /// <summary>
        /// 牌の情報を取得する
        /// </summary>
        public CPai Pai
        {
            get
            {
                return pai;
            }
        }

        /// <summary>
        /// 場の横方向の数
        /// </summary>
        private const int maxX = 17+2;
        /// <summary>
        /// 場の横方向の数を取得する
        /// </summary>
        public int MaxX
        {
            get
            {
                return maxX;
            }
        }

        /// <summary>
        /// 場の縦方向の数
        /// </summary>
        private const int maxY = 8+2;
        /// <summary>
        /// 場の縦方向の数を取得する
        /// </summary>
        public int MaxY
        {
            get
            {
                return maxY;
            }
        }

        /// <summary>
        /// 場の総数
        /// </summary>
        private const int max = maxX * maxY;
        /// <summary>
        /// 場の総数の取得
        /// </summary>
        public int MaxCell
        {
            get
            {
                return max;
            }
        }

        /// <summary>
        /// 場のセルデータ
        /// </summary>
        private int[] cell = new int[max];
        /// <summary>
        /// セルデータの取得
        /// </summary>
        public int[] Cell
        {
            set
            {
                cell = value;
            }
            get
            {
                return cell;
            }
        }

        /// <summary>
        /// セルの横サイズ
        /// </summary>
        private const int sizeCellX = 24;
        /// <summary>
        /// セルの横サイズを取得する
        /// </summary>
        public int SizeCellX
        {
            get
            {
                return sizeCellX;
            }
        }

        /// <summary>
        /// セルの縦サイズ
        /// </summary>
        private const int sizeCellY = 32;
        /// <summary>
        /// セルの縦サイズを取得する
        /// </summary>
        public int SizeCellY
        {
            get
            {
                return sizeCellY;
            }
        }

        /// <summary>
        /// セルの横方向の番号を取得する
        /// </summary>
        /// <param name="no">牌の通し番号 0～MaxKoma-1</param>
        /// <returns>0～MaxX-1</returns>
        public int GetCellX(int no)
        {
            //return MaxX - no % MaxX;
            return no % MaxX;
        }

        /// <summary>
        /// セルの縦方向の番号を取得する
        /// </summary>
        /// <param name="no">牌の通し番号 0～MaxKoma-1</param>
        /// <returns>0～MaxY-1</returns>
        public int GetCellY(int no)
        {
            //return MaxY - no / MaxX;
            return no / MaxX;
        }

        /// <summary>
        /// 縦と横の番号からセル番号を取得する
        /// </summary>
        /// <param name="posx">牌の横方向の番号 0～MaxX-1</param>
        /// <param name="posy">牌の縦方向の番号 0～MaxY-1</param>
        /// <returns></returns>
        public int GetCellNo(int posx, int posy)
        {
            return posy * MaxX + posx;
        }

        /// <summary>
        /// 上端
        /// </summary>
        /// <param name="no">セル番号</param>
        /// <returns>true=上端</returns>
        /// <returns>false=上端ではない</returns>
        public bool CheckTopLine(int no)
        {
            if (no >= max - MaxX)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 右端
        /// </summary>
        /// <param name="no">セル番号</param>
        /// <returns>true=右端</returns>
        /// <returns>false=右端ではない</returns>
        public bool CheckRightLine(int no)
        {
            if (0 == no % maxX)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 下端
        /// </summary>
        /// <param name="no">セル番号</param>
        /// <returns>true=下端</returns>
        /// <returns>false=下端ではない</returns>
        public bool CheckBottomLine(int no)
        {
            if (no < MaxX)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 左端
        /// </summary>
        /// <param name="no">セル番号</param>
        /// <returns>true=左端</returns>
        /// <returns>false=左端ではない</returns>
        public bool CheckLeftLine(int no)
        {
            if (maxX - 1 == no % maxX)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 上のセル番号取得
        /// </summary>
        /// <param name="no">セル番号</param>
        /// <returns>上のセル番号</returns>
        public int GetCellUp(int no)
        {
            return no + MaxX;
        }

        /// <summary>
        /// 右の牌番号取得
        /// </summary>
        /// <param name="no">セル番号</param>
        /// <returns>右のセル番号</returns>
        public int GetCellRight(int no)
        {
            return no - 1;
        }

        /// <summary>
        /// 下の牌番号取得
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public int GetCellDown(int no)
        {
            return no - MaxX;
        }

        /// <summary>
        /// 左の牌番号取得
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public int GetCellLeft(int no)
        {
            return no + 1;
        }
    }
}
