using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Test_detection_result
{
    public partial class Form1 : Form
    {
        public BindingSource bindingSource = new BindingSource();
        public List<UserControl2> userControl2s = new List<UserControl2>();

        public int Count = 1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口关闭中执行，用于保持数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData();
            Console.WriteLine("保存成功");
        }

        /// <summary>
        /// 窗口加载中执行，用于加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveData()
        {
            //遍历userControl2s，为userControl2s的每一个元素转换成 UserControl2Data 对象
            var data = userControl2s.Select(control => new UserControl2Data
            {
                Name = control.name,
                Items = control._items.Select(item => new UserControl3Data
                {
                    Name = item.name,
                    Value = item.Value
                }).ToList() // 将 Select 方法返回的结果转换为 List<UserControl3Data>
            }).ToList(); 

            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText("C:\\Users\\YFGK\\Desktop\\data.json", jsonData);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            if (File.Exists("C:\\Users\\YFGK\\Desktop\\data.json"))
            {
                var jsonData = File.ReadAllText("C:\\Users\\YFGK\\Desktop\\data.json");
                // List<UserControl2Data>
                var data = JsonConvert.DeserializeObject<List<UserControl2Data>>(jsonData);

                // 清空现有的控件
                panel1.Controls.Clear();
                userControl2s.Clear();

                if (data == null)
                {
                    return;
                }

                foreach (var item in data)
                {
                    int index = 0;
                    UserControl2 control = new UserControl2(item.Name);

                    // 先往UserControl2中添加UserControl3
                    foreach (UserControl3Data subItem in item.Items)
                    {
                        control.AddItemByName(subItem.Name);
                        control._items[index].Value = subItem.Value;
                        index++;
                    }

                    // 再往Form1中添加UserControl2
                    AddControlToPanel(control);
                    userControl2s.Add(control);
                }
            }
        }

        private void AddControlToPanel(UserControl2 control)
        {
            // 计算新控件的位置
            int yOffset = 10; // 控件之间的垂直间距
            int xOffset = 10; // 控件之间的水平间距
            int newX = xOffset;
            int newY = yOffset;

            // 查找panel1中最后一个控件的位置
            if (panel1.Controls.Count > 0)
            {
                Control lastControl = panel1.Controls[panel1.Controls.Count - 1];
                newX = lastControl.Right + xOffset;
                newY = lastControl.Top;

                // 检查是否需要换行
                if (newX + control.Width > panel1.Width)
                {
                    newX = xOffset;
                    newY = lastControl.Bottom + yOffset;
                }
            }

            control.Location = new Point(newX, newY);
            panel1.Controls.Add(control);
        }


        /// <summary>
        /// 删除相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            RemoveProcessResultView(this.textBox2.Text);
        }

        /// <summary>
        /// 清空所有流程的检测结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            ClearAllProcessResult(userControl2s);
        }

        /// <summary>
        /// 添加一个名为processName的流程结果
        /// </summary>
        public void AddProcessResultView(string processName)
        {
            if (userControl2s.Count>0)
            {
                foreach (UserControl2 item in userControl2s)
                {
                    if (item.name == processName)
                    {
                        MessageBox.Show("名称不能重复");
                        return;
                    }                  
                }
            }

            UserControl2 myControl = new UserControl2(processName);

            // 计算新控件的位置
            int yOffset = 10; // 控件之间的垂直间距
            int xOffset = 10; // 控件之间的水平间距
            int newX = xOffset;
            int newY = yOffset;

            // 查找groupBox1中最后一个控件的位置
            if (panel1.Controls.Count > 0)
            {
                Control lastControl = panel1.Controls[panel1.Controls.Count - 1];
                newX = lastControl.Right + xOffset;
                newY = lastControl.Top;

                // 检查是否需要换行
                if (newX + myControl.Width > panel1.Width)
                {
                    newX = xOffset;
                    newY = lastControl.Bottom + yOffset;
                }
            }

            myControl.Location = new Point(newX, newY);
            panel1.Controls.Add(myControl);
            userControl2s.Add(myControl);
            Count++;
        }

        /// <summary>
        /// 移除一个名为processName的流程结果，并重新排列剩下的控件
        /// </summary>
        public void RemoveProcessResultView(string processName)
        {
            int i = 0;
            bool res = false;

            foreach (UserControl2 item in userControl2s)
            {
                if (item.name == processName)
                {
                    if (panel1.Controls.Count > 0)
                    {
                        UserControl2 controlToRemove = userControl2s[i];

                        // 确保要删除的控件存在
                        if (controlToRemove is UserControl2)
                        {
                            panel1.Controls.Remove(controlToRemove);
                            controlToRemove.Dispose();
                            userControl2s.RemoveAt(i);

                            // 重新排列剩下的控件
                            RearrangeControls();
                        }
                    }
                    res = true;
                    break;
                }
                i++;
            }
            if (!res)
                MessageBox.Show("输入错误Id,请重试");
        }

        /// <summary>
        /// 重新排列剩下的控件
        /// </summary>
        private void RearrangeControls()
        {
            int yOffset = 10; // 控件之间的垂直间距
            int xOffset = 10; // 控件之间的水平间距
            int newX = xOffset;
            int newY = yOffset;

            foreach (Control control in panel1.Controls)
            {
                control.Location = new Point(newX, newY);

                newX = control.Right + xOffset;

                if (newX + control.Width > panel1.Width)
                {
                    newX = xOffset;
                    newY = control.Bottom + yOffset;
                }
            }
        }


        /// <summary>
        /// 在名为processName的流程结果中添加名为itemName的检测项
        /// </summary>
        /// <param name="processName"></param>
        public void AddItem(string processName, string itemName)
        {
            int i = 0;
            bool res = false;
            foreach (UserControl2 item in userControl2s)
            {
                if (item.name == processName)
                {
                    foreach (UserControl3 item1 in item._items)
                    {
                        if (item1.name == itemName)
                        {
                            MessageBox.Show("重复！");
                            return;
                        }
                    }
                    item.AddItemByName(itemName);
                    res = true;
                    break;
                }
                i++;
            }
            if (!res)
                MessageBox.Show("输入错误Id,请重试");
        }

        /// <summary>
        /// 在名为processName的流程结果中移除名为itemName的检测项
        /// </summary>
        /// <param name="processName"></param>
        public void RemoveItem(string processName, string itemName)
        {
            int i = 0;
            bool res = false;
            foreach (UserControl2 item in userControl2s)
            {
                if (item.name == processName)
                {
                    res = item.RemoveItemByName(itemName);
                }
                i++;
            }
            if (!res)
                MessageBox.Show("输入错误Id,请重试");
        }

        /// <summary>
        /// 指定流程指定检测项的结果加1
        /// </summary>
        /// <param name="processName"></param>
        /// <param name=""></param>
        public void IncreaseOneItemResult(string processName, string itemName)
        {
            if (int.Parse(itemName)<=2)
            {
                MessageBox.Show("必须大于2行");
                return;
            }

            int i = 0;
            bool res = false;
            foreach (UserControl2 item in userControl2s)
            {
                if (item.name == processName)
                {
                    if (panel1.Controls.Count > 0)
                    {
                        UserControl2 lastControl = userControl2s[i];
                        if (lastControl._items.Count < int.Parse(itemName))
                        {
                            MessageBox.Show("输入行数错误");
                            return;
                        }
                        UserControl3 control = lastControl._items[int.Parse(itemName) - 1];
                        control.Value++;
                        lastControl.Computation();
                    }
                    res = true;
                    break;
                }
                i++;
            }
            if (!res)
                MessageBox.Show("输入错误,请重试");         
        }

        /// <summary>
        /// 指定流程指定检测项的结果清空
        /// </summary>
        /// <param name="processName"></param>
        /// <param name=""></param>
        public void ClearItemResult(string processName, string itemName)
        {
            if (int.Parse(itemName) <= 2)
            {
                MessageBox.Show("必须大于2行");
                return;
            }

            int i = 0;
            bool res = false;
            foreach (UserControl2 item in userControl2s)
            {
                if (item.name == processName)
                {
                    if (panel1.Controls.Count > 0)
                    {
                        UserControl2 lastControl = userControl2s[i];
                        if (lastControl._items.Count < int.Parse(itemName))
                        {
                            MessageBox.Show("输入行数错误");
                            return;
                        }
                        UserControl3 control = lastControl._items[int.Parse(itemName) - 1];
                        control.Value = 0;
                        lastControl.Computation();
                    }
                    res = true;
                    break;
                }
                i++;
            }
            if (!res)
                MessageBox.Show("输入错误,请重试");
        }

        ///<summary>
        ///清空单个流程的所有检测项结果
        ///</summary>
        ///<param name = "processName" ></ param > 
        public void ClearProcessResult(string processName)
        {
            int i = 0;
            bool res = false;
            foreach (UserControl2 item in userControl2s)
            {
                if (item.name == processName)
                {
                    if (panel1.Controls.Count > 0)
                    {
                        UserControl2 lastControl = userControl2s[i];
                        foreach (UserControl3 item1 in lastControl._items)
                        {
                            item1.Value = 0;
                        }
                    }
                    res = true;
                    break;
                }
                i++;
            }
            if (!res)
                MessageBox.Show("输入错误名称,请重试");          
        }

        ///<summary>
        /// 清空所有流程的所有检测项结果
        ///</summary>
        ///<param name="processNamelist"></param>
        public void ClearAllProcessResult(List<UserControl2> userControl2s)
        {
            foreach (UserControl2 lastControl in userControl2s)
            {
                foreach (UserControl3 control in lastControl._items)
                {
                    control.Value = 0;
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ClearItemResult(this.textBox4.Text,this.textBox6.Text);
        }

        /// <summary>
        /// 增加相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            AddProcessResultView(this.textBox1.Text);
        }

        /// <summary>
        /// 在名称为？的流程结果的第？行加一
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            IncreaseOneItemResult(this.textBox3.Text, this.textBox5.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearProcessResult(this.textBox7.Text);
        }

        /// <summary>
        /// 添加待检测项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click_1(object sender, EventArgs e)
        {
            AddItem(this.textBox8.Text, this.textBox9.Text);
        }

        /// <summary>
        /// 删除待检测项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            RemoveItem(this.textBox11.Text, this.textBox10.Text);
        }
    }

    public class UserControl3Data
    {
        public string Name { get; set; }
        public float Value { get; set; }  
    }

    public class UserControl2Data
    {
        public string Name { get; set; }
        public List<UserControl3Data> Items { get; set; }
    }
}
