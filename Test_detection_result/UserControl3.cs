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
    public partial class UserControl3 : UserControl
    {
        public int id;
        private float _value;
        public string name;

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                label12.Text = _value.ToString(); // 更新 label12.Text
            }
        }

        public UserControl3(string text, string value, int id, Color color)
        {
            InitializeComponent();
            label0.Text = id.ToString();
            this.id = id;
            Value = int.Parse(value); // 使用属性来设置初始值
            SetText(text);
            SetLabelColor(color);
            
        }

        public void SetText(string text)
        {
            label11.Text = text;
            name = text;
        }

        public void SetValue(string value)
        {
            Value = int.Parse(value); // 使用属性来设置值
        }

        public void SetLabelColor(Color color)
        {
            label0.ForeColor = color;
            label11.ForeColor = color;
            label12.ForeColor = color;
        }

        public void SetLabel(int Id)
        {
            label0.Text = Id.ToString();
        }
    }
}
