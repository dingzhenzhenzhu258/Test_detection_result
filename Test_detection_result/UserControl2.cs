using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Test_detection_result
{
    public partial class UserControl2 : UserControl
    {
        public List<UserControl3> _items = new List<UserControl3>();
        public static int Id = 0;
        public int Id2 = 1;
        public string name;

        public UserControl2()
        {
            InitializeComponent();
        }

        public UserControl2(string name)
        {
            InitializeComponent();
            Id++;
            UserControl3 con1 = new UserControl3("检测总数", "0", Id2, Color.Black);
            Id2++;
            UserControl3 con2 = new UserControl3("良率(%)", "0", Id2, Color.Black);
            Id2++;
            UserControl3 con3 = new UserControl3("焊接OK数", "0", Id2, Color.Black);
            Id2++;
            UserControl3 con4 = new UserControl3("漏焊数", "0", Id2, Color.Red);
            Id2++;
            UserControl3 con5 = new UserControl3("悍高数", "0", Id2, Color.Red);
            Id2++;
            UserControl3 con6 = new UserControl3("焊珠数", "0", Id2, Color.Red);
            Id2++;
            UserControl3 con7 = new UserControl3("焊烟数", "0", Id2, Color.Red);
            Id2++;

            con1.Dock = DockStyle.Top;
            con2.Dock = DockStyle.Top;
            con3.Dock = DockStyle.Top;
            con4.Dock = DockStyle.Top;
            con5.Dock = DockStyle.Top;
            con6.Dock = DockStyle.Top;
            con7.Dock = DockStyle.Top;

            panel1.Controls.Add(con1);
            panel1.Controls.Add(con2);
            panel1.Controls.Add(con3);
            panel1.Controls.Add(con4);
            panel1.Controls.Add(con5);
            panel1.Controls.Add(con6);
            panel1.Controls.Add(con7);
            _items.Add(con1);
            _items.Add(con2);
            _items.Add(con3);
            _items.Add(con4);
            _items.Add(con5);
            _items.Add(con6);
            _items.Add(con7);

            // 调整控件顺序
            panel1.Controls.SetChildIndex(con1, 6);
            panel1.Controls.SetChildIndex(con2, 5);
            panel1.Controls.SetChildIndex(con3, 4);
            panel1.Controls.SetChildIndex(con4, 3);
            panel1.Controls.SetChildIndex(con5, 2);
            panel1.Controls.SetChildIndex(con6, 1);
            panel1.Controls.SetChildIndex(con7, 0);

            this.groupBox1.Text = Id.ToString();
            this.name = this.label1.Text = name;
        }

        /// <summary>
        /// 添加检查项
        /// </summary>
        /// <param name="name"></param>
        public void AddItemByName(string name)
        {
            if (_items.Any(item => item.name == name))
            {
                return; // 如果已经存在同名控件，则不添加新的控件
            }

            UserControl3 con = new UserControl3(name, "0", Id2, Color.Red);
            Id2++;
            con.Dock = DockStyle.Top;
            panel1.Controls.Add(con);
            _items.Add(con);
            panel1.Controls.SetChildIndex(con, 0); // 将新添加的控件放在最下面

            // 重新分配 id
            ReassignIds();
        }


        private void ReassignIds()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].id = i + 1;
                _items[i].SetLabel(i + 1);
                panel1.Controls.SetChildIndex(_items[i], _items.Count - 1 - i); //将每个元素的位置调整为倒序排列。
            }
        }

        /// <summary>
        /// 移除检查项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveItemByName(string name)
        {
            UserControl3 controlToRemove = null;

            // 找到要移除的控件
            foreach (UserControl3 item in _items)
            {
                if (item.name == name)
                {
                    controlToRemove = item;
                    break;
                }
            }
            // 移除控件并调整顺序
            if (controlToRemove != null)
            {
                panel1.Controls.Remove(controlToRemove);
                _items.Remove(controlToRemove);

                // 重新分配 id
                ReassignIds();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 计算值
        /// </summary>
        public void Computation()
        {
            int total = 0;
            int OK = 0;

            // 计算检测总数
            for (int k = 2; k < _items.Count; k++)
            {
                total += (int)_items[k].Value;
            }
            _items[0].Value = total;

            OK = (int)_items[2].Value;

            if (total - OK == 0)
            {               
                _items[1].Value = 100;
                return;
            }

            // 计算良率
            if (total != 0)
            {
                float yieldRate = (float)OK / total * 100;
                _items[1].Value = yieldRate;
            }
            else
            {
                _items[1].Value = 0;
            }
        }
    }
}
