﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace PhotographyEvent.Models
{
    
    // Class for Users Table
    public class User
    {
        public string userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public Boolean isAdmin { get; set; }
        public string password { get; set; }

        // Code for verifying when new administrator is enrolled
        public static string adminKeyCode { get { return System.Web.Configuration.WebConfigurationManager.AppSettings["adminkeycode"]; } }

        // Constructor
        public User(string userId, string password, string email)
        {
            this.userId = userId;
            this.password = password;
            this.emailAddress = email;
        }

        // Constructor overload
        public User(string userId, string password, string email, string firstName, string lastName, Boolean isAdmin) : this(userId, password, email)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.isAdmin = isAdmin;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="newUserId"></param>
        /// <returns></returns>
        public User getUser(string newUserId)
        {
            string selectSql = "select * from Users where userId = @findingId";
            Dictionary<string, string> pList = new Dictionary<string, string>();
            pList.Add("findingId", newUserId);
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReaderDicParam(selectSql, pList))
            {
                if (reader != null && reader.HasRows)
                {
                    if (reader.Read())
                    {
                        User newUser = new User(newUserId, reader["password"].ToString(), reader["emailAddress"].ToString(),
                            reader["firstName"] == null ? null : reader["emailAddress"].ToString(),
                            reader["lastName"] == null ? null : reader["lastName"].ToString(),
                            Convert.ToBoolean(reader["isAdmin"].ToString()));
                        return newUser;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// validate the new user id whether new id is available
        /// </summary>
        /// <param name="userId">user id to find in users table</param>
        /// <returns></returns>
        public static Boolean CheckId(string userId)
        {
            string findsql = "select userId from Users Where userId = @userId";
            Dictionary<string, string> pList = new Dictionary<string, string>();
            pList.Add("userId", userId);
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReaderDicParam(findsql, pList))
            {
                if (reader != null && reader.HasRows == true)
                {
                    // existing user id
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// validate if email address is duplicate
        /// </summary>
        /// <param name="emailAddr">email id to find</param>
        /// <returns></returns>
        public static Boolean CheckEmail(string emailAddr)
        {
            string findsql = @"select count(emailAddress) as cnt from Users where emailAddress = @email";
            Dictionary<string, string> pList = new Dictionary<string, string>();
            pList.Add("email", emailAddr);
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReaderDicParam(findsql, pList))
            {
                if (reader.Read())
                {
                    int count = Int32.Parse(reader["cnt"].ToString());
                    if (count == 0)
                        return true;    // no email id
                    else
                        return false;
                }
                return false;
            }
        }

        /// <summary>
        /// not used
        /// </summary>
        /// <param name="emailAddr"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Boolean CheckEmail(string emailAddr, string userId)
        {
            string findsql = @"select count(emailAddress) as cnt from Users where emailAddress = @email and userId <> @uid";
            Dictionary<string, string> pList = new Dictionary<string, string>();
            pList.Add("email", emailAddr);
            pList.Add("uid", userId);
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReaderDicParam(findsql, pList))
            {
                if (reader.Read())
                {
                    int count = Int32.Parse(reader["cnt"].ToString());
                    if (count == 0)
                        return true;
                    else
                        return false;
                }
                return false;
            }
        }

        /// <summary>
        /// Create new account
        /// </summary>
        /// <returns></returns>
        public Boolean CreateUser()
        {
            string updateSql = "INSERT INTO USERS(userId, password, emailAddress) VALUES(@userId, @password, @emailAddress)";
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("userId", this.userId));
            pList.Add(new SqlParameter("password", this.password));
            pList.Add(new SqlParameter("emailAddress", this.emailAddress));

            return Libs.DbHandler.updateData(updateSql, pList);
        }

        /// <summary>
        /// Create new administrator account
        /// </summary>
        /// <param name="keyCode">predefined code to quialify admin</param>
        /// <returns></returns>
        public Boolean CreateAdminUser(string keyCode)
        {
            if (adminKeyCode != keyCode)    // false administrator
                return false;

            string updateSql = "INSERT INTO USERS(userId, password, emailAddress, IsAdmin) VALUES(@userId, @password, @emailAddress, @isAdmin)";
            List<SqlParameter> pList = new List<SqlParameter>();
            pList.Add(new SqlParameter("userId", this.userId));
            pList.Add(new SqlParameter("password", this.password));
            pList.Add(new SqlParameter("emailAddress", this.emailAddress));
            pList.Add(new SqlParameter("isAdmin", this.isAdmin));

            return Libs.DbHandler.updateData(updateSql, pList);
        }

        /// <summary>
        /// validate user
        /// </summary>
        /// <param name="userId">user id to find</param>
        /// <param name="password">password to compare</param>
        /// <returns>true if authenticated or false if not</returns>
        public static Boolean AuthenticateUser(string userId, string password)
        {
            List<SqlParameter> pList = new List<SqlParameter>();
            string select = "Select Password From Users Where userId = @userid";
            pList.Add(new SqlParameter("@userid", userId));
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReader(select, pList))
            {
                if (reader != null && reader.HasRows == true)
                {
                    if (reader.Read())
                    {
                        if (password == reader["Password"].ToString())
                        {
                            return true;    // user authenticated
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// gets predefined key code form web.config
        /// </summary>
        /// <returns></returns>
        public static string getAdminKeyCode()
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["adminkeycode"];
        }

        /// <summary>
        /// check if the userid given is administrator
        /// </summary>
        /// <param name="userId">user id to check</param>
        /// <returns>true if administrator or false if not administrator</returns>
        public static Boolean isAdministrator(string userId)
        {
            string select = "Select IsAdmin From Users where userid = @findingId";
            Dictionary<string, string> pList = new Dictionary<string, string>();
            pList.Add("findingId", userId);
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReaderDicParam(select, pList))
            {
                if (reader != null && reader.HasRows)
                {
                    reader.Read();
                    return Convert.ToBoolean(reader["IsAdmin"].ToString());
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// get user's first name
        /// </summary>
        /// <param name="userId">user id to find</param>
        /// <returns>found user's first name</returns>
        public static string getUserFirstName(string userId)
        {
            string select = @"SELECT firstName From Users Where userId = @findingId";
            string fName = string.Empty;
            Dictionary<string, string> pList = new Dictionary<string, string>();
            pList.Add("findingId", userId);
            using (SqlDataReader reader = Libs.DbHandler.getResultAsDataReaderDicParam(select, pList))
            {
                if (reader.Read())
                {
                    fName = reader["firstName"].ToString();
                }                
            }
            return fName;
        }
    }
}