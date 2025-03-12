using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
namespace Music_Player_Application
{
    public class Database
    {
        private static string connectionString = "Data Source=DESKTOP-P6UD69C;Initial Catalog=MusicPlayerDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
