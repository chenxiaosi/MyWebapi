using MyWebapi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace MyWebapi.Controllers
{
    public class UsersController : ApiController
    {
        MySqlConnection myConnection;
        MySqlDataAdapter myDataAdapter;
        DataSet myDataSet;
        string conn = ConfigurationManager.ConnectionStrings["connstring"].ToString();
        String strSQL;

        Users[] users = new Users[]
        {
            new Users { Id=1,Name="chenxuan",Email="chensamsam@163.com"},
            new Users { Id=2,Name="chenxiaosi",Email="chensamsam@163.com"}
        };

        public HttpResponseMessage GetUsers()
        {
            strSQL = "SELECT u_id,u_pwd,u_realname FROM users;";
            myConnection = new MySqlConnection(conn);
            myDataAdapter = new MySqlDataAdapter(strSQL, myConnection);
            myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet, "mytable");
            return toJson(myDataSet);
        }
        public HttpResponseMessage GetAllUsers()
        {
            return toJson(users);
        }
        public HttpResponseMessage GetJson()
        {
            return toJson(users);
        }
        public IEnumerable<Users> GetJsonToString()
        {
            string json=JsonConvert.SerializeObject(users);
            return JsonConvert.DeserializeObject<IEnumerable<Users>>(json);
        }
        public Users GetJsonToStringById(int id)
        {
            var user = users.FirstOrDefault((p) => p.Id == id);
            string json = JsonConvert.SerializeObject(user);
            return JsonConvert.DeserializeObject<Users>(json);
        }
        public HttpResponseMessage GetUser(int id)
        {
            var user = users.FirstOrDefault((p) => p.Id == id);
            if (user == null)
            {
                return toJson("Not Found!");
            }
            return toJson(user);
        }
        [HttpPost]
        public int Add([FromBody]Users users)
        {
            if (users == null)
            {
                throw new HttpRequestException();
            }
            strSQL = "insert into users(u_id,u_pwd,u_realname) values("+users.Id+",'"+users.pwd+"','"+users.Name+"')";
            myConnection = new MySqlConnection(conn);
            myConnection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = myConnection;
           
            cmd.CommandText = strSQL;
            int a=cmd.ExecuteNonQuery();
            return a;
        }
        [HttpPost]
        public int Update([FromBody]Users users)
        {
            if (users == null)
            {
                throw new HttpRequestException();
            }
            strSQL = "update users set u_pwd='" + users.pwd + "',u_realname='" + users.Name + "' where u_id=" + users.Id;
            myConnection = new MySqlConnection(conn);
            myConnection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = myConnection;

            cmd.CommandText = strSQL;
            int a = cmd.ExecuteNonQuery();
            return a;
        }
        [HttpPost]
        public int Delete(int id)
        {
            if (users == null)
            {
                throw new HttpRequestException();
            }
            strSQL = "delete from users where u_id=" + id;
            myConnection = new MySqlConnection(conn);
            myConnection.Open();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = myConnection;

            cmd.CommandText = strSQL;
            int a = cmd.ExecuteNonQuery();
            return a;
        }
        [HttpGet]
        public HttpResponseMessage GetUserById(int id)
        {
            strSQL = "SELECT u_id,u_pwd,u_realname FROM users where u_id="+ id;
            myConnection = new MySqlConnection(conn);
            myDataAdapter = new MySqlDataAdapter(strSQL, myConnection);
            myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet, "mytable");
            Users u = new Users();
            foreach(DataRow dc in myDataSet.Tables["mytable"].Rows)
            {
                u.Id = int.Parse(dc["u_id"].ToString());
                u.Name = dc["u_realname"].ToString();
                u.pwd = dc["u_pwd"].ToString();
            }
            return toJson(u);
        }
        public static HttpResponseMessage toJson(Object obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                str = JsonConvert.SerializeObject(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }

}
