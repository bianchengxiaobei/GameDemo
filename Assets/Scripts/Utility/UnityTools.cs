using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Security.Cryptography;
namespace Utility
{
    /// <summary>
    /// unity工具类
    /// </summary>
    public class UnityTools
    {
        /// <summary>
        /// 加载文本文件，读取里面的字符内容并返回
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string LoadFileText(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader sr = File.OpenText(filePath))
                {
                    return sr.ReadToEnd();
                }
            }
            else 
            {
                return string.Empty;
                Debug.LogWarning("读取的文本没有内容");
            }
        }
        /// <summary>
        /// 保存文本到指定文件路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="textContent"></param>
        public static void SaveText(string filePath, string textContent)
        {
            //如果不存在该目录就创建
            if (!Directory.Exists(GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(GetDirectoryName(filePath));
            }
            //如果已经存在该文件就删除
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            //创建文件并写入内容
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(textContent);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
        }
        /// <summary>
        /// 取得该文件所在的目录文件夹
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string filePath)
        {
            return filePath.Substring(0, filePath.LastIndexOf('/'));
        }
        /// <summary>
        /// 取得UICamera
        /// </summary>
        public static Camera GetUICamera
        {
            get 
            {
                if (UICamera.currentCamera == null)
                {
                    UICamera.currentCamera = GameObject.Find("UI Root").transform.FindChild("Camera").GetComponent<Camera>();
                }
                return UICamera.currentCamera;
            }
        }
        /// <summary>
        /// 将字符串转换为对应类型的值。
        /// </summary>
        /// <param name="value">字符串值内容</param>
        /// <param name="type">值的类型</param>
        /// <returns>对应类型的值</returns>
        public static object GetValue(String value, Type type)
        {
            if (type == null)
                return null;
            else if (type == typeof(string))
                return value;
            else if (type == typeof(Int32))
                return Convert.ToInt32(Convert.ToDouble(value));
            else if (type == typeof(float))
                return float.Parse(value);
            else if (type == typeof(byte))
                return Convert.ToByte(Convert.ToDouble(value));
            else if (type == typeof(sbyte))
                return Convert.ToSByte(Convert.ToDouble(value));
            else if (type == typeof(UInt32))
                return Convert.ToUInt32(Convert.ToDouble(value));
            else if (type == typeof(Int16))
                return Convert.ToInt16(Convert.ToDouble(value));
            else if (type == typeof(Int64))
                return Convert.ToInt64(Convert.ToDouble(value));
            else if (type == typeof(UInt16))
                return Convert.ToUInt16(Convert.ToDouble(value));
            else if (type == typeof(UInt64))
                return Convert.ToUInt64(Convert.ToDouble(value));
            else if (type == typeof(double))
                return double.Parse(value);
            else if (type == typeof(bool))
            {
                if (value == "0")
                    return false;
                else if (value == "1")
                    return true;
                else
                    return bool.Parse(value);
            }
            else if (type.BaseType == typeof(Enum))
                return GetValue(value, Enum.GetUnderlyingType(type));
            else
                return null;
        }
        /// <summary>
        /// 生成文件的md5码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string BuildFileMd5(string filePath)
        {
            string fileMd5 = null;
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    MD5 md5 = MD5.Create();
                    byte[] fileMd5bytes = md5.ComputeHash(fs);
                    fileMd5 = System.BitConverter.ToString(fileMd5bytes).Replace("_", "").ToLower();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            return fileMd5;
        }
    }
}