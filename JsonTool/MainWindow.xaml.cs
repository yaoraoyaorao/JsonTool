using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JsonTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currentCSName;
        private string currentJsonName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void JsonOpenFile(object sender, RoutedEventArgs e)
        {
            JsonPath.Text = Core.Tool.OpenPath("选择json文件", "json数据|*.json",out currentJsonName);
        }

        private void CSOpenFile(object sender, RoutedEventArgs e)
        {
            CSPath.Text = Core.Tool.OpenPath("选择C#文件", "C#文件|*.cs", out currentCSName);
        }

        private void LoadData(object sender, RoutedEventArgs e)
        {
            string source = Core.Tool.LoadFile(CSPath.Text);
            object obj = null;
            if (!string.IsNullOrEmpty(source))
            {
                obj = Core.Tool.DP(source, currentCSName.Split('.')[0]);
            }

            if (obj == null)
            {
                MessageBox.Show("加载数据失败");
            }
            else
            {
                MessageBox.Show("加载数据成功");
            }
        }
    }
}
