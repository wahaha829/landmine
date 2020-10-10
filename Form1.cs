using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; //666

namespace landmine
{
    
    public partial class Form1 : Form
    {
        public static ButtonL[,] buttons = new ButtonL[9, 9];  //宣告9*9的二維按鈕陣列
        public static Random rnd = new Random(); // 亂數物件
        public static int rank = 9;
        public DateTime date1 ;
        // 不設static會跳錯誤 CS0120需要有物件參考，才可使用非靜態欄位、方法或屬性
        public static System.Timers.Timer t = new System.Timers.Timer(); 

        public class ButtonL : Button //這個繼承自Button的類別，放到Form1外面會怪怪的???
        {
            public int islandmine = 0; //屬性:是否為地雷
            public int landmine_around = 0;
            public int flag = 0; // 是否被插旗
            public int landmine_isfindout = 0;
            public int row, col;
      
            public void setlandmine(int randnum) //依據輸入的亂數設定是否地雷
            {
                if (randnum == 1)
                {
                    islandmine = 1;
                }

            }
            public void Button_right_down(object sender, MouseEventArgs e)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (this.flag == 0)
                    {
                        // 更換按鈕上的img為插旗圖案
                        this.flag = 1;
                        this.BackgroundImage = global::landmine.Properties.Resources.flag;
                        this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                    }
                    else
                    {
                        this.flag = 0;
                        this.BackgroundImage = null; // 拿掉插旗圖案
                    }
                    if (islandmine == 1 && flag == 1)
                    {
                        landmine_isfindout = 1;
                    }
                    else
                    {
                        landmine_isfindout = 0;
                    }
                    if (findout_landmines() == countlandmines() && countlandmines() == countflags())
                    {
                    //插旗數=找出正確定雷數=地雷數總和
                        MessageBox.Show("Well done!");
                    }
                }

            }
            public void ButtonL_click(object sender, EventArgs e)
            // 按下按鍵要執行的函式，參數不放(object sender, EventArgs e)會error
            {
                if (this.Enabled == true && this.flag == 0)//Button的內建屬性(是否可按)
                // 若不加上面這一行判斷式，遞迴執行時會出錯，蠻怪的
                {
                    this.Enabled = false; //按鈕設為不可按
                    if (islandmine == 1)
                    {
                        this.Text = "B";
                        showbuttons();
                        timer_stop(); //踩到地雷停止計時
                        MessageBox.Show("Game Over!");
                        
                    }
                    else if (landmine_around >= 1)
                        this.Text = "" + landmine_around;
                    else
                    {
                        this.Text = "" + landmine_around;
                        click_around(row, col);
                    }
                }  
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createbuttons();
            count_all_landmines_around();
        }

        private void createbuttons()
        {
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    int rndnum = rnd.Next(1, rank); //產生1~9亂數
                    buttons[i, j] = new ButtonL();
                    buttons[i, j].row = i;
                    buttons[i, j].col = j;
                    buttons[i, j].BackColor = System.Drawing.Color.LightGray;
                    buttons[i, j].TabStop = false; //把鍵盤焦點功能關閉，不然會有鍵盤藍色框
                    buttons[i, j].Location = new Point(j * 30, i * 30); //座標x軸是j，y軸是i
                    buttons[i, j].Size = new Size(30, 30);
                    this.Controls.Add(buttons[i, j]);  //在Form1.Controls裡面新增按紐
                    buttons[i, j].setlandmine(rndnum); //設定是否為地雷
                    buttons[i, j].Click += new EventHandler(buttons[i, j].ButtonL_click);
                    // 因為右鍵點button不能觸發Click或MouseClick，改用MouseDown
                    buttons[i, j].MouseDown += new MouseEventHandler(buttons[i, j].Button_right_down);
                    
                }
            }
            this.date1 = DateTime.Now; //設定起始時間
            timer_start(); //開始計時
            label2.Text = "地雷數量 " + countlandmines();
        }
        private void resetbuttons()
        {
        // 重置按鈕狀態
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    int rndnum = rnd.Next(1, rank); //產生1~9亂數
                    buttons[i, j].islandmine = 0;
                    buttons[i, j].BackgroundImage = null;
                    buttons[i, j].flag = 0;
                    buttons[i, j].setlandmine(rndnum); //設定是否為地雷
                    buttons[i, j].Enabled = true; //按鈕設為可按
                    buttons[i, j].Text = "" ;
                }

            }
            label2.Text = "地雷數量 " + countlandmines();
            count_all_landmines_around();
            this.date1 = DateTime.Now; //開始計時
        }
        private static void showbuttons()
        {
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    buttons[i, j].Enabled = false; //按鈕設為不可按
                    if (buttons[i, j].islandmine == 1)
                        buttons[i, j].Text = "B";
                    else
                        buttons[i, j].Text = "" + buttons[i, j].landmine_around;
                }

            }
        }
        
        public static void click_around(int row, int col)
        //按下周圍按鈕
        {
            // ButtonL_click原本是按鈕事件觸發，想直接呼叫就裡面放兩個null參數
            if (row >= 1 && col >= 1)
                buttons[row - 1, col - 1].ButtonL_click(null,null);
            if (row >= 1)
                buttons[row - 1, col].ButtonL_click(null, null);
            if (row >= 1 && col <= rank - 2)
                buttons[row - 1, col + 1].ButtonL_click(null, null);
            if (col >= 1)
                buttons[row, col - 1].ButtonL_click(null, null);
            if (col <= rank - 2)
                buttons[row, col + 1].ButtonL_click(null, null);
            if (row <= rank - 2 && col >= 1)
                buttons[row + 1, col - 1].ButtonL_click(null, null);
            if (row <= rank - 2)
                buttons[row + 1, col].ButtonL_click(null, null);
            if (row <= rank - 2 && col <= rank - 2)
                buttons[row + 1, col + 1].ButtonL_click(null, null);
            
        }


        public int count_landmine_around(int row, int col)
        //計算第[i,j]個按鈕周圍的地雷數量
        {
            int landmine_around = 0;
            if(row >= 1 && col >= 1)
                landmine_around += buttons[row-1,col-1].islandmine;
            if (row >= 1)
                landmine_around += buttons[row-1, col].islandmine;
            if (row >= 1 && col <= rank-2)
                landmine_around += buttons[row-1, col+1].islandmine;
            if (col >= 1)
                landmine_around += buttons[row, col-1].islandmine;
            if (col <= rank-2)
                landmine_around += buttons[row, col+1].islandmine;
            if (row <= rank-2 && col >= 1)
                landmine_around += buttons[row+1, col-1].islandmine;
            if (row <= rank-2)
                landmine_around += buttons[row+1, col].islandmine;
            if (row <= rank-2 && col <= rank-2)
                landmine_around += buttons[row+1, col+1].islandmine;
            //MessageBox.Show("["+row+","+col+"]"+"周圍地雷數量" + landmine_around);
            return landmine_around;
        }

        private void count_all_landmines_around()
        //計算所有按鈕周圍的地雷數量
        {
            for (int i = 0; i <= rank - 1; i++)
            {
                for (int j = 0; j <= rank - 1; j++)
                {
                    //count_landmine_around(i, j);
                    buttons[i, j].landmine_around = count_landmine_around(i, j);
                }
            }
        }
        private static int countlandmines()
        //地雷數量總和
        {
            int landmines_amount = 0;
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    if (buttons[i, j].islandmine == 1)
                        landmines_amount++;
                }
            }
            return landmines_amount;
        }
        private static int countflags()
        //插旗數量總和
        {
            int flags_amount = 0;
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    if (buttons[i, j].flag == 1)
                        flags_amount++;
                }
            }
            return flags_amount;
        }
        private static int findout_landmines()
        // 找出的正確地雷數量總和
        {
            int findout_amount = 0;
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    if (buttons[i, j].landmine_isfindout == 1)
                        findout_amount++;
                }
            }
            return findout_amount;
        }
        private static int count_buttons_enabled()
        //按下按鈕總和
        {
            int buttons_enabled = 0;
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    if (buttons[i, j].Enabled == true)
                        buttons_enabled++;
                }
            }
            return buttons_enabled;
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            resetbuttons();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            showbuttons();
        }



        /* 計時器 & 執行緒委派
         * System.Timers名稱空間下的Timer類，使用Elapsed事件另開一個執行緒。
         * 定義一個System.Timers.Timer物件，然後繫結Elapsed事件，
         * 通過Start()方法來啟動計時，通過Stop()方法或者Enable=false停止計時
        */
        private void timer_start()
        {
            t.Interval = 1000; //多久執行一次
            t.Enabled = true; //是否執行System.Timers.Timer.Elapsed事件
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.AutoReset = true; //每到指定时间Elapsed事件是触发一次(false)还是一直触发(預設)(true)
            t.Start();
        }

        private void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CreateControlsByTimer();
            //this.label3.Text = " 當前時間：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //如果在Elapsed绑定的方法中直接直接更新label3的Text，會跳"跨執行緒作業無效"
        }


        private void CreateControlsByTimer()
        {

            //判斷這個Label3的物件是否在同一個執行緒上
            if (this.InvokeRequired)
            {
                //當InvokeRequired為true時，表示在不同的執行緒上，所以進行委派(調用Invoke方法)
                //移至Action定義，寫道 public delegate void Action();
                Action invokeAction = new Action(CreateControlsByTimer); //遞迴??
                this.Invoke(invokeAction);
            }
            else
            {
                //表示在同一個執行緒上了，所以可以正常的呼叫到這個Label物件
                DateTime date2 = DateTime.Now;
                TimeSpan timespan = date2 - date1;
                label3.Text = "花費時間 " + timespan.Minutes + "分"+ timespan.Seconds +"秒"; 
            }

        }

        private static void timer_stop()
        {
            t.Stop();
        }

    }
}
