using CRUD_Assessment.model;
using CRUD_Assessment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Assessment.Controllers
{
    [Route("User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Get All Users
        /// <summary>
        /// This method is used to Retrieve all the Users
        /// </summary>         
        /// <returns>Return Users List</returns>
        [HttpPost]
        [Route("GetAllUsers")]
        public List<UserInfo> GetAllUsers()
        {

            DataSet ds = new DataSet();
            CommonService service = new CommonService();
            List<UserInfo> users = null;
            try
            {
                //service.executeQuery("drop table Users ");
                //bool ab = service.executeQuery(" CREATE TABLE Users( UserId	VARCHAR(40) NOT NULL,PCode VARCHAR(40) NOT NULL,FirstName VARCHAR(100),LastName	VARCHAR(100)	NOT NULL,Email	VARCHAR(100)	,IsActive	BIT	)");
                //service.executeQuery("insert into Users values( 'admin','8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918','admin','admin','admin@gmail.com',1)");
                ds = service.executeDataset("select UserId,PCode,FirstName,LastName,Email,IsActive from Users");
                if (ds.Tables.Count > 0)
                {
                    users = (from c in ds.Tables[0].AsEnumerable()
                             select new UserInfo
                             {
                                 UserId = c.Field<string>("UserId"),
                                 PCode = c.Field<string>("PCode"),
                                 FirstName = c.Field<string>("FirstName"),
                                 LastName = c.Field<string>("LastName"),
                                 Email = c.Field<string>("Email"),
                                 IsActive = c.Field<bool>("IsActive")

                             }).ToList();
                }
                return users;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Get User
        /// <summary>
        /// This method is used to Retrieve all the Users
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns>Return respective user</returns>
        [HttpPost]
        [Route("GetUser")]
        public UserInfo GetUser(UserInfo user)
        {

            DataSet ds = new DataSet();
            CommonService service = new CommonService();
            UserInfo userinfo = null;
            try
            {
                //service.executeQuery("drop table Users ");
                //bool ab = service.executeQuery(" CREATE TABLE Users( UserId	VARCHAR(40) NOT NULL,PCode VARCHAR(40) NOT NULL,FirstName VARCHAR(100),LastName	VARCHAR(100)	NOT NULL,Email	VARCHAR(100)	,IsActive	BIT	)");
                //service.executeQuery("insert into Users values( 'admin','8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918','admin','admin','admin@gmail.com',1)");
                ds = service.executeDataset("select UserId,PCode,FirstName,LastName,Email,IsActive from Users where UserId='"+user.UserId+"'");
                if (ds.Tables.Count > 0)
                {
                    userinfo = (from c in ds.Tables[0].AsEnumerable()
                             select new UserInfo
                             {
                                 UserId = c.Field<string>("UserId"),
                                // PCode = c.Field<string>("PCode"),
                                 FirstName = c.Field<string>("FirstName"),
                                 LastName = c.Field<string>("LastName"),
                                 Email = c.Field<string>("Email"),
                                 IsActive = c.Field<bool>("IsActive")

                             }).FirstOrDefault();
                }
                return userinfo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Authenticate User
        /// <summary>
        /// this method is used to validate the user with given credentials
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AuthenticateUser")]
        public dynamic AuthenticateUser(UserInfo user)
        {
             DataSet ds = new DataSet();
            dynamic dynamicResult = new ExpandoObject();
            CommonService service = new CommonService();
            string Password = user.PCode.ToString();
            string UserName = user.UserId.ToString();
            try
            {
                string encrypted = ComputeSha256Hash(Password);
                string sqlQuery = "select UserId,PCode,FirstName,LastName,Email,IsActive from Users where PCode='" + encrypted + "' and UserId='" + UserName + "'";
                ds = service.executeDataset(sqlQuery);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    dynamicResult.IsActive = true;
                    dynamicResult.SuccessMessage = "User Logged in successfully";
                    dynamicResult.IsValid = true;

                    if (Convert.ToBoolean(ds.Tables[0].Rows[0]["IsActive"]) == false)
                    {
                        dynamicResult.IsActive = false;
                        dynamicResult.ErrorMessage = "User is InActive";
                        dynamicResult.IsValid = false;
                    }
                }
                else
                {
                    dynamicResult.IsActive = false;
                    dynamicResult.ErrorMessage = "UserId or Password incorrect";
                    dynamicResult.IsValid = false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dynamicResult;
        }
        #endregion

        #region CRUD Method
        /// <summary>
        /// This method is used to Create/Update/Delete user.. Based on the flag UserAction the CRUD operations done
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddUpdateDeleteUser")]
        public dynamic AddUpdateDeleteUser(UserInfo user)
        {
            dynamic dynamicResult = new ExpandoObject();
            CommonService service = new CommonService();
            string sqlQuery = string.Empty;
            try
            {
                if (user.UserAction == "A")
                {

                    sqlQuery = "INSERT INTO USERS VALUES( '" + user.UserId + "','" + ComputeSha256Hash(user.PCode) + "','" + user.FirstName + "','" + user.LastName + "','" + user.Email + "'," + user.IsActive + ")";
                    service.executeQuery(sqlQuery);
                    dynamicResult.ErrorMessage = "User Added Successfully";
                    dynamicResult.IsValid = true;
                }
                else if (user.UserAction == "U")
                {
                    sqlQuery = "UPDATE USERS SET UserId='" + user.UserId + "',PCode='" + ComputeSha256Hash(user.PCode) + "',FirstName='" + user.FirstName + "',LastName='" + user.LastName + "',Email='" + user.Email + "',IsActive=" + user.IsActive + " WHERE UserId='"+user.UserId+"'";
                    service.executeQuery(sqlQuery);
                    dynamicResult.ErrorMessage = "User Updated Successfully";
                    dynamicResult.IsValid = true;
                }
                else
                {
                    sqlQuery = "DELETE FROM USERS WHERE UserId='" + user.UserId + "'";
                    service.executeQuery(sqlQuery);
                    dynamicResult.ErrorMessage = "User Updated Successfully";
                    dynamicResult.IsValid = true;
                }

            }
            catch (Exception ex)
            {
                dynamicResult.ErrorMessage = ex.Message;
                dynamicResult.IsValid = false;
                //throw ex;
            }
            return dynamicResult;
        }
        #endregion

        #region Generate Password
        /// <summary>
        /// This method is used to generate password using SHA256 algorithm
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns>returns hashed password</returns>         
        public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
    }
}
