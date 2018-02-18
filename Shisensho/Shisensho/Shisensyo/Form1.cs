// C#研究会
// 2017年夏特別企画
// 四川省
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Shisensho
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// スコア
        /// </summary>
        private int score;

        /// <summary>
        /// 牌の画像
        /// </summary>
        private Image[,] imagePai;

        /// <summary>
        /// セルの画像
        /// </summary>
        private PictureBox[] pictCell;

        /// <summary>
        /// 選択された牌
        /// </summary>
        private int paiSelectNo;

        /// <summary>
        /// 選択数
        /// </summary>
        private int paiSelectCount;

        /// <summary>
        /// 場
        /// </summary>
        private CField field;

        /// <summary>
        /// 表示タイプ(通常)
        /// </summary>
        private const int ImageNormal = 0;

        /// <summary>
        /// 表示タイプ(選択中)
        /// </summary>
        private const int ImageSelect = 1;

        /// <summary>
        /// 取得可能パターン判定メソッドの型定義
        /// </summary>
        /// <param name="no1">引数1の型=int</param>
        /// <param name="no2">引数2の型=int</param>
        /// <returns>戻り値の型=bool</returns>
        private delegate bool CheckLine(int no1, int no2);

        /// <summary>
        /// 取得可能パターン判定メソッドの配列
        /// </summary>
        private CheckLine[] checkLinePattern;

        /// <summary>
        /// 隣接セル番号取得メソッドの型定義
        /// </summary>
        /// <param name="no">引数1の型=int</param>
        /// <returns>戻り値の型=int</returns>
        private delegate int MoveOne(int no);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // インスタンスの生成
            field = new CField();                               // 場
            imagePai = new Image[field.Pai.MaxKind + 1, 2];     // 牌の絵柄
            pictCell = new PictureBox[field.MaxCell];           // 場のセル

            // 通路判定メソッド
            checkLinePattern = new CheckLine[8];
            checkLinePattern[0] = CheckPattern0;
            checkLinePattern[1] = CheckPattern1;
            checkLinePattern[2] = CheckPattern2;
            checkLinePattern[3] = CheckPattern3;
            checkLinePattern[4] = CheckPattern4;
            checkLinePattern[5] = CheckPattern5;
            checkLinePattern[6] = CheckPattern6;
            checkLinePattern[7] = CheckPattern7;
        }

        /// <summary>
        /// Form1_Loadイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //PictureBoxコントロールのプロパティを設定する
            SuspendLayout();
            for (int no = 0; no < field.MaxCell; no++)
            {
                //PictureBoxクラスのインスタンスを作成する
                pictCell[no] = new PictureBox()
                {
                    //サイズと位置を設定する
                    Location = new Point((field.MaxX - field.GetCellX(no)) * (field.SizeCellX - 3),
                                         (field.MaxY - field.GetCellY(no)) * (field.SizeCellY - 3)),
                    Size = new Size(field.SizeCellX, field.SizeCellY),
                    Tag = no,
                };
                //イベントハンドラに関連付け
                pictCell[no].Click += new EventHandler(PictPai_Click);
            }

            //フォームにコントロールを追加
            Controls.AddRange(pictCell);
            ResumeLayout(false);

            SetScore(false);

            NewGame();
        }

        /// <summary>
        /// PictureBoxのクリックイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictPai_Click(object sender, EventArgs e)
        {
            PictureBox pai = (PictureBox)sender;
            int no = (int)pai.Tag;
            int kind;

            Debug.WriteLine("PictPai_Click no=" + no.ToString());

            if (0 == paiSelectCount)
            {                               // 1個目を選択
                paiSelectNo = no;
                kind = field.Cell[no];
                pictCell[no].Image = imagePai[kind, ImageSelect];
                paiSelectCount++;
            }
            else
            {
                if ((paiSelectNo != no) &&      // 1個目と異なる牌がクリックされた
                    (CheckLinking(no)))         // 接続判定
                {
                    pictCell[paiSelectNo].Visible = pictCell[no].Visible = false;
                    pictCell[paiSelectNo].Enabled = pictCell[no].Enabled = false;
                    pictCell[paiSelectNo].Image = null;
                    SetScore(true);
                    if (score >= field.Pai.MaxPai)
                    {                           // すべての牌を取得した
                        MessageBox.Show("クリアおめでとう！");
                        NewGame();
                        return;
                    }
                    if (CheckGameOver())
                    {                           // 取れる牌がない
                        MessageBox.Show("取れる牌がありません");
                        NewGame();
                        return;
                    }
                }
                else
                {
                    kind = field.Cell[no];
                    pictCell[no].Image = imagePai[kind, ImageNormal];   // キャンセルする
                    if (paiSelectNo != no)
                    {
                        kind = field.Cell[paiSelectNo];
                        pictCell[paiSelectNo].Image = imagePai[kind, ImageNormal];   // キャンセルする
                    }
                }
                paiSelectCount = 0;
            }
        }

        /// <summary>
        /// 牌が取れるかのチェック
        /// </summary>
        /// <returns></returns>
        private bool CheckLinking(int no)
        {
            if (field.Cell[no] != field.Cell[paiSelectNo])
            {                           // 選択された2つの牌の種類が異なる
                return false;
            }

            // 2角取り判定
            return Check3Lines(paiSelectNo, no);
        }

        /// <summary>
        /// 2角取り判定
        /// </summary>
        /// <returns>
        /// true = 取得可能
        /// false = 取得不可能
        /// </returns>
        private bool Check3Lines(int no1, int no2)
        {
            // 始点と終点が同じ場合はチェック不要
            if (no1 == no2)
            {
                return true;
            }

            // 8パターンの線をチェックする
            foreach (CheckLine checkline in checkLinePattern)
            {
                if (checkline(no1, no2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 2点間の通路チェック0
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 最短コースで判定する。
        /// 横→縦の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern0(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern0(" + no1.ToString() + "," + no2.ToString() + ")");

            int no;

            // 水平方向チェック
            if (field.GetCellX(no1) < field.GetCellX(no2))
            {
                no = CheckLeft(ref no1, no2);
                Debug.WriteLine("CheckLeft(" + no1.ToString() + "," + no2.ToString() + ") => " + no.ToString());
            }
            else
            {
                no = CheckRight(ref no1, no2);
                Debug.WriteLine("CheckRight(" + no1.ToString() + "," + no2.ToString() + ") => " + no.ToString());
            }
            if (no == no2)
            {
                return true;            // no2に到達
            }
            if (field.GetCellX(no1) != field.GetCellX(no2))
            {
                return false;           // no2のX座標に届かなかった
            }

            // 続きから垂直方向チェック
            if (field.GetCellY(no1) < field.GetCellY(no2))
            {
                no = CheckUp(ref no1, no2);
                Debug.WriteLine("CheckUp(" + no1.ToString() + "," + no2.ToString() + ")) => " + no.ToString());
            }
            else
            {
                no = CheckDown(ref no1, no2);
                Debug.WriteLine("CheckDown(" + no1.ToString() + "," + no2.ToString() + ")) => " + no.ToString());
            }
            if (no == no2)
            {
                Debug.WriteLine("Goal");
                return true;            // no2に到達
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック1
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 最短コースで判定する。
        /// 縦→横の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern1(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern1(" + no1.ToString() + "," + no2.ToString() + ")");

            int no;

            // 垂直方向チェック
            if (field.GetCellY(no1) < field.GetCellY(no2))
            {
                no = CheckUp(ref no1, no2);
                Debug.WriteLine("CheckUp(" + no1.ToString() + "," + no2.ToString() + ")) => " + no.ToString());
            }
            else
            {
                no = CheckDown(ref no1, no2);
                Debug.WriteLine("CheckDown(" + no1.ToString() + "," + no2.ToString() + ")) => " + no.ToString());
            }
            if (no == no2)
            {
                return true;            // no2に到達
            }

            // 続きから水平方向チェック
            if (field.GetCellX(no1) < field.GetCellX(no2))
            {
                no = CheckLeft(ref no1, no2);
                Debug.WriteLine("CheckLeft(" + no1.ToString() + "," + no2.ToString() + ")) => " + no.ToString());
            }
            else
            {
                no = CheckRight(ref no1, no2);
                Debug.WriteLine("CheckRight(" + no1.ToString() + "," + no2.ToString() + ")) => " + no.ToString());
            }
            if (no == no2)
            {
                Debug.WriteLine("Goal");
                return true;            // no2に到達
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック2
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 2点の内側を判定する。
        /// 横→縦→横の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern2(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern2(" + no1.ToString() + "," + no2.ToString() + ")");

            MoveOne moveOne;

            int x1 = field.GetCellX(no1);
            int x2 = field.GetCellX(no2);
            int dx = 0;

            if (x1 < x2)
            {
                dx = 1;                 // 左方向
                moveOne = new MoveOne(field.GetCellLeft);
            }
            else if (x1 > x2)
            {
                dx = -1;                // 右方向
                moveOne = new MoveOne(field.GetCellRight);
            }
            else
            {
                x2 = 0;
                dx = -1;                // 右方向
                moveOne = new MoveOne(field.GetCellRight);
            }

            int no = no1;
            for (int x = x1 + dx; x != x2; x += dx)
            {
                // 横へ移動
                no = moveOne(no);
                if (no == no2)
                {
                    return true;        // no2に到達
                }
                if (pictCell[no].Enabled == true)
                {
                    return false;       // 行き止まり
                }
                if (CheckPattern1(no, no2)) // 垂直→水平チェック
                {
                    return true;        // no2に到達
                }
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック3
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 2点の内側を判定する。
        /// 縦→横→縦の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern3(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern3(" + no1.ToString() + "," + no2.ToString() + ")");

            MoveOne moveOne;

            int y1 = field.GetCellY(no1);
            int y2 = field.GetCellY(no2);
            int dy = 0;

            if (y1 < y2)
            {
                dy = 1;                 // 上方向
                moveOne = new MoveOne(field.GetCellUp);
            }
            else if (y1 > y2)
            {
                dy = -1;                // 下方向
                moveOne = new MoveOne(field.GetCellDown);
            }
            else
            {
                y2 = 0;
                dy = -1;                // 下方向
                moveOne = new MoveOne(field.GetCellDown);
            }

            int no = no1;
            for (int y = y1 + dy; y != y2 + dy; y += dy)
            {
                // 縦へ移動
                no = moveOne(no);
                if (no == no2)
                {
                    return true;        // no2に到達
                }
                if (pictCell[no].Enabled == true)
                {
                    return false;       // 行き止まり
                }
                if (CheckPattern0(no, no2)) // 水平→垂直チェック
                {
                    return true;        // no2に到達
                }
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック4
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 2点の外側を判定する。
        /// 横→縦→横の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern4(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern4(" + no1.ToString() + "," + no2.ToString() + ")");

            MoveOne moveOne;

            int x1 = field.GetCellX(no1);
            int x2 = field.GetCellX(no2);
            int dx = 0;

            if (x1 < x2)
            {
                x2 = field.MaxX - 1;
                dx = 1;                 // 左方向
                moveOne = new MoveOne(field.GetCellLeft);
            }
            else
            {
                x2 = 0;
                dx = -1;                // 右方向
                moveOne = new MoveOne(field.GetCellRight);
            }

            int no = no1;
            for (int x = x1 + dx; x != x2 + dx; x += dx)
            {
                // 横へ移動
                no = moveOne(no);
                if (no == no2)
                {
                    return true;        // no2に到達
                }
                if (pictCell[no].Enabled == true)
                {
                    return false;       // 行き止まり
                }
                if (CheckPattern1(no, no2)) // 垂直→水平チェック
                {
                    return true;        // no2に到達
                }
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック5
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 2点の外側を判定する。
        /// 縦→横→縦の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern5(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern5(" + no1.ToString() + "," + no2.ToString() + ")");

            MoveOne moveOne;

            int y1 = field.GetCellY(no1);
            int y2 = field.GetCellY(no2);
            int dy = 0;

            if (y1 < y2)
            {
                y2 = 0;
                dy = -1;                 // 下方向
                moveOne = new MoveOne(field.GetCellDown);
            }
            else
            {
                y2 = field.MaxY - 1;
                dy = 1;                // 上方向
                moveOne = new MoveOne(field.GetCellUp);
            }

            int no = no1;
            for (int y = y1 + dy; y != y2 + dy; y += dy)
            {
                // 縦へ移動
                no = moveOne(no);
                if (no == no2)
                {
                    return true;        // no2に到達
                }
                if (pictCell[no].Enabled == true)
                {
                    return false;       // 行き止まり
                }
                if (CheckPattern0(no, no2)) // 水平→垂直チェック
                {
                    return true;        // no2に到達
                }
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック6
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 2点の外側を判定する。
        /// 縦→横→縦の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern6(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern6(" + no1.ToString() + "," + no2.ToString() + ")");

            MoveOne moveOne;

            int y1 = field.GetCellY(no1);
            int y2 = field.GetCellY(no2);
            int dy = 0;

            // 外回りなので逆方向から始める
            if (y1 > y2)
            {
                y2 = 0;
                dy = -1;                 // 下方向
                moveOne = new MoveOne(field.GetCellDown);
            }
            else
            {
                y2 = field.MaxY - 1;
                dy = 1;                // 上方向
                moveOne = new MoveOne(field.GetCellUp);
            }

            int no = no1;
            for (int y = y1 + dy; y != y2 + dy; y += dy)
            {
                // 縦へ移動
                no = moveOne(no);
                if (no == no2)
                {
                    return true;        // no2に到達
                }
                if (pictCell[no].Enabled == true)
                {
                    return false;       // 行き止まり
                }
                if (CheckPattern0(no, no2)) // 水平→垂直チェック
                {
                    return true;        // no2に到達
                }
            }

            return false;               // no2に到達しなかった
        }

        /// <summary>
        /// 2点間の通路チェック7
        /// </summary>
        /// <param name="no1">始点</param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// 到達判定結果
        /// true = 終点まで到達した
        /// false= 終点まで到達できなかった
        /// </returns>
        /// <remarks>
        /// 2点の外側を判定する。
        /// 横→縦→横の順でチェックを行う。
        /// </remarks>
        private bool CheckPattern7(int no1, int no2)
        {
            Debug.WriteLine("CheckPattern7(" + no1.ToString() + "," + no2.ToString() + ")");

            MoveOne moveOne;

            int x1 = field.GetCellX(no1);
            int x2 = field.GetCellX(no2);
            int dx = 0;

            // 外回りなので逆方向から始める
            if (x1 < x2)
            {
                x2 = 0;
                dx = -1;                // 右方向
                moveOne = new MoveOne(field.GetCellRight);
            }
            else
            {
                x2 = field.MaxX - 1;
                dx = 1;                 // 左方向
                moveOne = new MoveOne(field.GetCellLeft);
            }

            int no = no1;
            for (int x = x1 + dx; x != x2 + dx; x += dx)
            {
                // 横へ移動
                no = moveOne(no);
                if (no == no2)
                {
                    return true;        // no2に到達
                }
                if (pictCell[no].Enabled == true)
                {
                    return false;       // 行き止まり
                }
                if (CheckPattern1(no, no2)) // 垂直→水平チェック
                {
                    return true;        // no2に到達
                }
            }

            return false;
        }

        /// <summary>
        /// 左方向検索
        /// </summary>
        /// <param name="no1">
        /// in 始点
        /// out 最終到達点
        /// </param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// チェック終了位置
        /// </returns>
        private int CheckLeft(ref int no1, int no2)
        {
            int no = no1;
            int x1 = field.GetCellX(no1);
            int x2 = field.GetCellX(no2);

            if (x1 == x2)
            {
                return no1;              // 始点と終点とでX座標は一致している
            }

            // 左方向チェック
            for (int x = x1 + 1; x <= x2; x++)
            {
                int noNext = field.GetCellLeft(no);
                if (pictCell[noNext].Enabled == true)
                {                       // 牌にぶつかったので手前の位置を返す
                    no1 = no;
                    return noNext;
                }
                if (field.CheckLeftLine(no))
                {                       // 左端
                    break;
                }
                no = noNext;
            }

            // 終点まで着いたがそこは空白だった
            no1 = no;
            return no;
        }

        /// <summary>
        /// 右方向検索
        /// </summary>
        /// <param name="no1">
        /// in 始点
        /// out 最終到達点
        /// </param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// チェック終了位置
        /// </returns>
        private int CheckRight(ref int no1, int no2)
        {
            int no = no1;
            int x1 = field.GetCellX(no1);
            int x2 = field.GetCellX(no2);

            if (x1 == x2)
            {
                return no1;              // 始点と終点とでX座標は一致している
            }

            // 右方向チェック
            for (int x = x1 - 1; x >= x2; x--)
            {
                int noNext = field.GetCellRight(no);
                if (pictCell[noNext].Enabled == true)
                {                       // 牌にぶつかったので手前の位置を返す
                    no1 = no;
                    return noNext;
                }
                if (field.CheckRightLine(no))
                {                       // 右端
                    break;
                }
                no = noNext;
            }

            // 終点まで着いたがそこは空白だった
            no1 = no;
            return no;
        }

        /// <summary>
        /// 上方向検索
        /// </summary>
        /// <param name="no1">
        /// in 始点
        /// out 最終到達点
        /// </param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// チェック終了位置
        /// </returns>
        private int CheckUp(ref int no1, int no2)
        {
            int no = no1;
            int y1 = field.GetCellY(no1);
            int y2 = field.GetCellY(no2);

            if (y1 == y2)
            {
                return no2;              // 始点と終点とでX座標は一致している
            }

            // 上方向チェック
            for (int y = y1 + 1; y <= y2; y++)
            {
                int noNext = field.GetCellUp(no);
                if (pictCell[noNext].Enabled == true)
                {                       // 牌にぶつかったので手前の位置を返す
                    no1 = no;
                    return noNext;
                }
                if (field.CheckTopLine(no))
                {                       // 右端
                    break;
                }
                no = noNext;
            }

            // 終点まで着いたがそこは空白だった
            no1 = no;
            return no;
        }

        /// <summary>
        /// 下方向検索
        /// </summary>
        /// <param name="no1">
        /// in 始点
        /// out 最終到達点
        /// </param>
        /// <param name="no2">終点</param>
        /// <returns>
        /// チェック終了位置
        /// </returns>
        private int CheckDown(ref int no1, int no2)
        {
            int no = no1;
            int y1 = field.GetCellY(no1);
            int y2 = field.GetCellY(no2);

            if (y1 == y2)
            {
                return no1;              // 始点と終点とでX座標は一致している
            }

            // 下方向チェック
            for (int y = y1 - 1; y >= y2; y--)
            {
                int noNext = field.GetCellDown(no);
                if (pictCell[noNext].Enabled == true)
                {                       // 牌にぶつかったので手前の位置を返す
                    no1 = no;
                    return noNext;
                }
                if (field.CheckBottomLine(no))
                {                       // 右端
                    break;
                }
                no = noNext;
            }

            // 終点まで着いたがそこは空白だった
            no1 = no;
            return no;
        }

        /// <summary>
        /// ゲーム終了判定
        /// </summary>
        /// <returns>
        /// true :ゲームオーバー
        /// false:まだ取れる牌がある
        /// </returns>
        private bool CheckGameOver()
        {
            for (int no1 = 0; no1 < field.MaxCell; no1++)
            {
                if (pictCell[no1].Enabled)
                {
                    for (int no2 = no1+1; no2 < field.MaxCell-2; no2++)
                    {
                        if (pictCell[no2].Enabled)
                        {
                            if (field.Cell[no1] == field.Cell[no2])
                            {
                                if (Check3Lines(no1, no2))
                                {
                                    Debug.WriteLine("remain " + no1.ToString() + "," + no2.ToString());
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 画像設定
        /// </summary>
        private void SetPicture()
        {
            // 牌画像設定
            int kind = 0;

            imagePai[kind, ImageNormal] = null;
            imagePai[kind++, ImageSelect] = null;
            imagePai[kind, ImageNormal] = Properties.Resources.man1;
            imagePai[kind++, ImageSelect] = Properties.Resources.man1s;
            imagePai[kind, ImageNormal] = Properties.Resources.man2;
            imagePai[kind++, ImageSelect] = Properties.Resources.man2s;
            imagePai[kind, ImageNormal] = Properties.Resources.man3;
            imagePai[kind++, ImageSelect] = Properties.Resources.man3s;
            imagePai[kind, ImageNormal] = Properties.Resources.man4;
            imagePai[kind++, ImageSelect] = Properties.Resources.man4s;
            imagePai[kind, ImageNormal] = Properties.Resources.man5;
            imagePai[kind++, ImageSelect] = Properties.Resources.man5s;
            imagePai[kind, ImageNormal] = Properties.Resources.man6;
            imagePai[kind++, ImageSelect] = Properties.Resources.man6s;
            imagePai[kind, ImageNormal] = Properties.Resources.man7;
            imagePai[kind++, ImageSelect] = Properties.Resources.man7s;
            imagePai[kind, ImageNormal] = Properties.Resources.man8;
            imagePai[kind++, ImageSelect] = Properties.Resources.man8s;
            imagePai[kind, ImageNormal] = Properties.Resources.man9;
            imagePai[kind++, ImageSelect] = Properties.Resources.man9s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou1;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou1s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou2;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou2s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou3;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou3s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou4;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou4s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou5;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou5s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou6;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou6s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou7;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou7s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou8;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou8s;
            imagePai[kind, ImageNormal] = Properties.Resources.sou9;
            imagePai[kind++, ImageSelect] = Properties.Resources.sou9s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin1;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin1s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin2;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin2s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin3;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin3s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin4;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin4s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin5;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin5s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin6;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin6s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin7;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin7s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin8;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin8s;
            imagePai[kind, ImageNormal] = Properties.Resources.pin9;
            imagePai[kind++, ImageSelect] = Properties.Resources.pin9s;
            imagePai[kind, ImageNormal] = Properties.Resources.ton;
            imagePai[kind++, ImageSelect] = Properties.Resources.tons;
            imagePai[kind, ImageNormal] = Properties.Resources.nan;
            imagePai[kind++, ImageSelect] = Properties.Resources.nans;
            imagePai[kind, ImageNormal] = Properties.Resources.sya;
            imagePai[kind++, ImageSelect] = Properties.Resources.syas;
            imagePai[kind, ImageNormal] = Properties.Resources.pei;
            imagePai[kind++, ImageSelect] = Properties.Resources.peis;
            imagePai[kind, ImageNormal] = Properties.Resources.haku;
            imagePai[kind++, ImageSelect] = Properties.Resources.hakus;
            imagePai[kind, ImageNormal] = Properties.Resources.hatu;
            imagePai[kind++, ImageSelect] = Properties.Resources.hatus;
            imagePai[kind, ImageNormal] = Properties.Resources.chun;
            imagePai[kind++, ImageSelect] = Properties.Resources.chuns;

            // 場のセル表示内容設定
            int paiNo = 0;

            for (int cellNo = 0; cellNo < field.MaxCell; cellNo++)
            {
                if ((field.CheckTopLine(cellNo) == true) ||
                    (field.CheckRightLine(cellNo) == true) ||
                    (field.CheckBottomLine(cellNo) == true) ||
                    (field.CheckLeftLine(cellNo) == true))
                {
                    kind = 0;
                    pictCell[cellNo].Enabled = false;
                    pictCell[cellNo].Visible = false;
                }
                else
                {
                    kind = field.Pai.Kind[paiNo++];
                    pictCell[cellNo].Enabled = true;
                    pictCell[cellNo].Visible = true;
                }
                field.Cell[cellNo] = kind;
                pictCell[cellNo].Image = imagePai[kind, 0];
                pictCell[cellNo].BackgroundImage = imagePai[kind, 1];
            }
        }

        /// <summary>
        /// スコアを更新する
        /// </summary>
        /// <param name="score"></param>
        private void SetScore(bool sw)
        {
            if (sw)
            {
                score += 2;
            }
            else
            {
                score = 0;
            }
            lblScore.Text = score.ToString();
        }

        /// <summary>
        /// 次のゲームを始める
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("新しいゲームを始めますか？", 
                "New Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                NewGame();
            }
        }

        /// <summary>
        /// 新しい面を始める
        /// </summary>
        private void NewGame()
        {
            SuspendLayout();
            // シャッフル
            field.Pai.Shuffle();
            SetPicture();
            paiSelectCount = 0;
            paiSelectNo = 0;
            SetScore(false);
            ResumeLayout(false);
        }
    }
}
;