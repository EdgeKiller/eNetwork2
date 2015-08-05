using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using eNetwork2;
using System.Security.Cryptography;

namespace LoginServer
{
    public class DatabaseManager
    {
        private SQLiteConnection connection;

        public DatabaseManager(string file)
        {
            connection = new SQLiteConnection("Data Source=" + file + ";Version=3;");
        }

        public byte[] LoginRequest(string username, string password)
        {
            connection.Open();

            byte[] response = null;

            SQLiteCommand command = new SQLiteCommand("select * from accounts where username='" + username + "'", connection);
            SQLiteDataReader sqlreader = command.ExecuteReader();
            if (sqlreader.Read())
            {
                if (SHA256Encrypt(password) == sqlreader["password"].ToString())
                {

                    if (!(bool)sqlreader["banned"])
                    {
                        Program.Log("Connection request accepted !");
                        response = new byte[1];
                        using (PacketWriter pw = new PacketWriter())
                        {
                            pw.WriteByte(0);
                            response = pw.ToArray();
                        }
                    }
                    else
                    {
                        Program.Log("Connection request failed : banned user !");
                        using (PacketWriter pw = new PacketWriter())
                        {
                            pw.WriteByte(2);
                            pw.WriteString(sqlreader["bannedr"].ToString());
                            response = pw.ToArray();
                        }
                    }
                }
            }

            if (response == null)
            {
                Program.Log("Connection request failed : bad username or password !");
                using (PacketWriter pw = new PacketWriter())
                {
                    pw.WriteByte(1);
                    response = pw.ToArray();
                }
            }

            connection.Close();

            return response;
        }

        private String SHA256Encrypt(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Join("", hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }
    }
}
