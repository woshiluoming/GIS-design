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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void bt_register_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text.Trim();
            string userPassword = txtPassword.Text.Trim();
            string userPasswordCheck = txtPasswordCheck.Text.Trim();

            string constr = "server=localhost;database=login;uid=root;pwd=333;";
            //// 建立SqlConnection对象
            MySqlConnection con = new MySqlConnection(constr);

            MySqlConnection sqlCon = new MySqlConnection(constr);//建立DataSet对象(相当于建立前台的虚拟数据库)

            DataSet ds = new DataSet();//建立DataTable对象(相当于建立前台的虚拟数据库中的数据表)


            sqlCon.Open();//建立DataAdapter对象

            string sltStr = "select count(*) from login.test where userName = '" + userName + "' and password = '" + userPassword + "'";//编写符合查询条件的sql语句

            MySqlCommand sqlCmd = new MySqlCommand(sltStr, sqlCon);

            MySqlDataAdapter msda = new MySqlDataAdapter(sqlCmd);//将查询的结果存到虚拟数据库ds中的虚拟表tabuser中



            int n = msda.Fill(ds, "test");//将数据表tabuser的数据复制到DataTable对象(取数据)

            if (n == 0)
            {
                MessageBox.Show("用户名已存在！", "提示");
                txtUserName.Text = "";
                txtPassword.Text = "";
                txtPasswordCheck.Text = "";
            }
            else if (txtUserName.TextLength > 16)
            {
                MessageBox.Show("用户名太长，我怕你记不住，请换个短的吧！", "提示");
            }
            else if (txtPassword.Text != txtPasswordCheck.Text)
            {
                MessageBox.Show("两次输入的密码不一致！", "提示");
                txtPassword.Text = "";
                txtPasswordCheck.Text = "";
            }
            else if (txtUserName.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("用户名或密码不能为空！", "提示");
            }
            else if (txtPassword.TextLength < 6 || txtPassword.TextLength > 12)
            {
                MessageBox.Show("密码长度不符合要求！", "提示");
                txtPassword.Text = "";
                txtPasswordCheck.Text = "";
            }
            else
            {
                // 指定MySQL语句
                sqlCmd = new MySqlCommand("INSERT INTO test(userName,password) VALUES('" + userName + "','" + userPassword + "')", sqlCon);
                // 建立SqlDataAdapter和DataSet对象
                sqlCmd.ExecuteNonQuery();
                sqlCmd = null;
                MessageBox.Show("注册成功！", "提示");
                this.Close();
            }
            sqlCon.Close();
        }
    }
}
