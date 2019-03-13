using SignalrChat.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SignalRApp.App_Data
{
    public class UserRepo
    {
        private readonly string ConnectionString = "Data source=DESKTOP-4ERLT03\\SQLEXPRESS; database=SignalRDB; integrated security=true";
        string defaultPicture = "D:\\OneDrive - University College Nordjylland\\UCN Aalborg\\Visual Studio\\Projects\\SignalrApplication\\SignalrChat\\Content\\images\\defaultPicture.jpg";

        public bool Add(User u)
        {
            bool added = false;
           using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                var sqlParams = new List<DbParameter>();
                    using (conn)
                    {
                        try
                        {
                            DbCommand command = conn.CreateCommand();
                            command.CommandText = "INSERT INTO tbl_Users (UserName, Password, Photo) VALUES (@username, @password, @photo)";
                            command.CommandType = CommandType.Text;

                            conn.Open();
                            sqlParams.Add(new SqlParameter("@username", u.Name));
                            sqlParams.Add(new SqlParameter("@password", u.Password));
                            if (u.ImagePath != "")
                            {
                                defaultPicture = u.ImagePath;
                            }
                            sqlParams.Add(new SqlParameter("@photo", defaultPicture));
                            command.Parameters.AddRange(sqlParams.ToArray());
                            command.ExecuteNonQuery();
                            added = true;
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Exception.Message: {0}", ex.Message);
                        }
                    }
            }
            return added;
        }

        public User GetUser(string usrname, string password = null)
        {
            var queryString = (password != null) ? "SELECT * FROM tbl_Users WHERE UserName ='" + usrname + "' and " + "Password = '" + password + "'" 
                                                 : "SELECT * FROM tbl_Users WHERE UserName ='" + usrname + "'";
            User user = new User();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.ID = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Password = reader.GetString(2);
                                user.ImagePath = reader.GetString(3);  
                            }
                            conn.Close();
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine("Error while getting the user object " + e.StackTrace);
            }
            return user;
        }

        public User FindUserByID(string username)
        {
            var queryString = "SELECT * FROM tbl_Users WHERE UserName ='" + username + "'";
            
            User user = new User();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                user.ID = reader.GetInt32(0);
                                user.Name = reader.GetString(1);
                                user.Password = reader.GetString(2);
                                user.ImagePath = reader.GetString(3);
                            }
                            conn.Close();
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine("Error while getting the user object " + e.StackTrace);
            }
            return user;
        }

        public void UpdateUser(User u)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                var sqlParams = new List<DbParameter>();
                    using (conn)
                    {
                        try
                        {
                            defaultPicture = (u.ImagePath != "") ? u.ImagePath : GetPicturePath(u.Name);
                            DbCommand command = conn.CreateCommand();
                            command.CommandText = "UPDATE tbl_Users SET UserName ='" + u.Name + "', " +
                                                                       "Password = '" + u.Password + "', " +
                                                                       "Photo = '" + defaultPicture + "' " +
                                                                                                "WHERE ID =" + u.ID;
                            command.CommandType = CommandType.Text;
                            conn.Open();
                            command.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Exception.Message: {0}", ex.Message);
                        }
                    }
            }
        }

        private string GetPicturePath(string name)
        {
            var queryString = "SELECT Photo FROM tbl_Users WHERE UserName ='" + name + "'";
            string imgPath = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(queryString, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                imgPath = reader.GetString(0);
                            }
                            conn.Close();
                        }
                    }

                }
            }
            catch (SqlException e)
            {
                Debug.WriteLine("Error while getting the user object " + e.StackTrace);
            }
            return imgPath;
        }

    }
}
