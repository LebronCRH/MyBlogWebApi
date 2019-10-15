using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MyBlog.Core.Common.Helper
{
    public class FtpHelper
    {
        private string ftpAddress = "";
        private string username = "";
        private string password = "";
        private string serverAddress = "";//ftp上传的公共图片http访问方式地址
        public FtpHelper()
        {
            ftpAddress= Appsettings.app(new string[] { "FtpConfig", "ftpAddress" });
            username = Appsettings.app(new string[] { "FtpConfig", "username" });
            password = Appsettings.app(new string[] { "FtpConfig", "password" });
            serverAddress = Appsettings.app(new string[] { "ImageCommonPath", "ImageHttpAddress" });
        }
        public string CreateFolderAtFtp(string modelName, string dicName)
        {
            string ftpPath = string.Empty;
            if (!string.IsNullOrEmpty(modelName))
            {
                //检测是否有该企业的模块文件夹
                bool isExist = IsDirectoryExist(ftpAddress + modelName + "/");
                if (!isExist)
                {
                    string dic = CreateDirectoryAtFtp(modelName, "", "");
                    if (string.IsNullOrEmpty(dic))
                    {
                        throw new Exception("创建文件夹失败");
                    }
                }
                ftpPath += "/" + modelName;
            }
            if (!string.IsNullOrEmpty(dicName))
            {

                //检测是否有该企业模块下的子文件夹
                bool isExist = IsDirectoryExist(ftpAddress + modelName + "/" + dicName + "/");
                if (!isExist)
                {
                    string dic = CreateDirectoryAtFtp(modelName, dicName, "");
                    if (string.IsNullOrEmpty(dic))
                    {
                        throw new Exception("创建文件夹失败");
                    }
                }
                ftpPath += "/" + dicName;
            }
            return ftpPath;
        }

        public string UploadFie(IFormFile file,IList<string> dicpathlist,string filename)
        {
            Stream fs = file.OpenReadStream();
            string FullPath = string.Empty;
            foreach (var item in dicpathlist)
            {
                FullPath = FullPath + "/" + item;
            }
            CreateFullDirectoryAtFtp(ftpAddress+FullPath);
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpAddress + FullPath + "/" + filename));
            reqFTP.Credentials = new NetworkCredential(username, password);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = file.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            try
            {
                Stream strm = reqFTP.GetRequestStream();// 把上传的文件写入流
                contentLen = fs.Read(buff, 0, buffLength);// 每次读文件流的2kb

                while (contentLen != 0)// 流内容没有结束
                {
                    // 把内容从file stream 写入 upload stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // 关闭两个流
                strm.Close();
                fs.Close();
                return serverAddress + FullPath + "/" + filename;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// 判斷指定得路徑是否存在于ftp上
        /// </summary>
        /// <param name="fileFullPath"></param>
        public bool IsDirectoryExist(string fullDirectory)
        {
            if (!fullDirectory.EndsWith("/"))
                fullDirectory += "/";
            bool result = false;
            //執行ftp命令 活動目錄下所有文件列表
            Uri uriDir = new Uri(fullDirectory);
            WebRequest listRequest = WebRequest.Create(uriDir);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            listRequest.Credentials = new NetworkCredential(username, password);
            //listRequest.KeepAlive = false;  //執行一個命令后關閉連接
            WebResponse listResponse = null;

            try
            {
                listResponse = listRequest.GetResponse();
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
            }
            finally
            {
                if (listResponse != null)
                    listResponse.Close();
            }

            return result;
        }

        /// <summary>
        /// 在FTP創建一個新文件夾
        /// </summary>
        /// <param name="root">要在那个路径下创建文件夹</param>
        /// <param name="DicLayer3"></param>
        /// <returns>创建成功的ftp上的全路径</returns>
        private string CreateDirectoryAtFtp(string DicLayer1, string DicLayer2, string DicLayer3)
        {

            try
            {
                //在ftp上的路径
                string ftpPath = DicLayer1;
                if (!string.IsNullOrEmpty(DicLayer2))
                {
                    ftpPath += "/" + DicLayer2;
                }
                if (!string.IsNullOrEmpty(DicLayer3))
                {
                    ftpPath += "/" + DicLayer3;
                }
                Uri uri = new Uri(ftpAddress + ftpPath);
                FtpWebRequest listRequest = (FtpWebRequest)FtpWebRequest.Create(uri);
                if (!IsDirectoryExist(uri.ToString()))
                {
                    listRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                }
                else
                {
                    CreateFullDirectoryAtFtp(uri.ToString());
                    listRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                }
                listRequest.Credentials = new NetworkCredential(username, password);
                listRequest.KeepAlive = false;                                  //執行一個命令后關閉連接
                FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();

                string fullPath = ftpAddress + ftpPath + "/";
                Stream write = GetWriteStream(fullPath + "ftpPath.ini");  //在ftp上創建文件
                byte[] context = System.Text.Encoding.Default.GetBytes("ftpPath=" + ftpPath);
                write.Write(context, 0, context.Length);
                write.Close();
                return ftpPath;    // 返回創建目錄路徑
            }
            catch (Exception ex)
            {
                //Logger.Error("创建文件夹失败" + ex.Message);
                return String.Empty;
            }
        }

        /// <summary>
        /// 在ftp上創建文件夾(若目錄不存在則依序創建)。
        /// </summary>
        /// <param name="directoryName"></param>
        public void CreateFullDirectoryAtFtp(string directoryPath)
        {
            Uri uriDir = new Uri(directoryPath);
            directoryPath = uriDir.AbsolutePath;
            directoryPath = directoryPath.Replace(@"\", "/");
            directoryPath = directoryPath.Replace("//", "/");
            string[] aryDirctoryName = directoryPath.Split('/');
            string realPath = "";
            realPath = ftpAddress;
            for (int i = 0; i < aryDirctoryName.Length; i++)
            {
                if (aryDirctoryName[i] != String.Empty)
                {
                    realPath = realPath + "/" + aryDirctoryName[i];
                    if (!IsDirectoryExist(realPath))
                    {
                        CreateDirectoryAtFtp(realPath);
                    }

                }
            }
        }
        /// <summary>
        /// 在ftp上創建文件夾，用於對zip文檔得解壓。
        /// </summary>
        /// <param name="directoryName"></param>
        public void CreateDirectoryAtFtp(string directoryName)
        {
            try
            {
                Uri uri = new Uri(directoryName);
                FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(uri);
                listRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                listRequest.Credentials = new NetworkCredential(username, password);
                listRequest.KeepAlive = false;                                  //執行一個命令后關閉連接
                FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建文件夹  
        /// </summary>  
        /// <param name="ftpPath">FTP路径</param>  
        /// <param name="dirName">创建文件夹名称</param>  
        public void MakeDir(string ftpPath, string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                string ui = (ftpPath + dirName).Trim();
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(ui);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(username, password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Stream GetWriteStream(string fileFullName)
        {
            FtpWebRequest uploadRequest = (FtpWebRequest)WebRequest.Create(new Uri(fileFullName));
            uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
            uploadRequest.Credentials = new NetworkCredential(username, password);
            uploadRequest.KeepAlive = false;    //執行一個命令后關閉連接.
            uploadRequest.UseBinary = true;
            return uploadRequest.GetRequestStream();
        }
        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="strPath"></param>
        /// <returns></returns>
        //private string MapPath(string strPath)
        //{
        //    if (HttpContext.Current != null)
        //    {
        //        return HttpContext.Current.Server.MapPath(strPath);
        //    }
        //    else //非web程序引用            
        //    {
        //        strPath = strPath.Replace("/", "\\");
        //        if (strPath.StartsWith("\\"))
        //        {
        //            strPath = strPath.TrimStart('\\');
        //        }
        //        return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        //    }
        //}
    }
}
