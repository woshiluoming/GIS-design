using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace _luoming
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //登录
            string userName = this.txtUserName.Text;
            string userPassword = this.txtPassword.Text;

            if (userName.Equals("") || userPassword.Equals(""))
            {
                MessageBox.Show("用户名或密码不能为空！");
            }
            else
            {
                string strcon = "server=localhost;database=login;uid=root;pwd=333;";
                MySqlConnection con = new MySqlConnection(strcon);
                try
                {
                    con.Open();
                    string sqlSel = "select count(*) from login.test where userName = '" + userName + "' and password = '" + userPassword + "'";
                    MySqlCommand com = new MySqlCommand(sqlSel, con);
                    if (Convert.ToInt32(com.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("登录成功！");
                        this.DialogResult = DialogResult.OK;
                        this.Dispose();
                        this.Close();
                        //luoming next = new luoming();//创建将要打开的窗体对象
                        //next.Show(); // 打开新窗体--Show非模式对话框--ShowDialog模式对话框
                        //this.Hide();// this.Close()关闭当前窗体,因为登录是主窗体所以不能关闭
                    }
                    //用户名和密码验证错误，提示错误。
                    else
                    {
                        MessageBox.Show("用户名或密码错误！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString() + "打开数据库失败");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////注册
            //string userName = this.txtUserName.Text;
            //string userPassword = this.txtPassword.Text;
            //string strcon = "server=localhost;database=login;uid=root;pwd=333;";

            //MySqlConnection con = new MySqlConnection(strcon);
            //con.Open();
            //String sql = "INSERT INTO test(userName,password) VALUES('" + userName + "','" + userPassword + "')"; // 没有判断重复插入

            //MySqlCommand cmd = new MySqlCommand(sql, con);
            //cmd.ExecuteNonQuery();
            //MessageBox.Show("注册成功");

            //Login next01 = new Login();//创建将要打开的窗体对象
            //next01.Show(); // 打开新窗体--Show非模式对话框--ShowDialog模式对话框
            //this.Hide();// this.Close()关闭当前窗体,因为登录是主窗体所以不能关闭

            //con.Close();

            Register reg = new Register();
            
            reg.ShowDialog();
            
        }
    }
}