using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileTool
{
    /// <summary>
    /// 获取文件路径下的所有文件
    /// </summary>
    /// <param name="dirPath">文件夹绝对路径</param>
    /// <returns></returns>
    public static FileInfo[] GetFileInfosFromDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        DirectoryInfo info = new DirectoryInfo(dirPath);
        return info.GetFiles();
    }
    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="fileFullName">文件名.后缀</param>
    /// <param name="dirPath">文件夹路径</param>
    public static void CreateFile(string fileFullName, string dirPath)
    {
        string filePath = Path.Combine(dirPath, fileFullName);
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
            fs.Close();
        }
       
    }
    /// <summary>
    /// 复制文件
    /// </summary>
    /// <param name="copyFileFullPath">文件绝对路径</param>
    /// <param name="saveFileFullPath">文件绝对路径</param>
    /// <param name="overwrite"></param>
    public static void CopyFile(string copyFileFullPath, string saveFileFullPath, bool overwrite)
    {
        if (File.Exists(copyFileFullPath))//必须判断要复制的文件是否存在
        {
            File.Copy(copyFileFullPath, saveFileFullPath, true);//三个参数分别是源文件路径，存储路径，若存储路径有相同文件是否替换
        }
        else
        {
            Debug.LogError("不存在的文件");
        }
    }
    /// <summary>
    /// 复制文件夹及文件
    /// </summary>
    /// <param name="copyDirPath">原文件路径</param>
    /// <param name="saveDirPath">目标文件路径</param>
    /// <returns></returns>
    public static int CopyDir(string copyDirPath, string saveDirPath)
    {
        try
        {
            string folderName = System.IO.Path.GetFileName(copyDirPath);
            string destfolderdir = System.IO.Path.Combine(saveDirPath, folderName);
            string[] filenames = System.IO.Directory.GetFileSystemEntries(copyDirPath);
            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (System.IO.Directory.Exists(file))
                {
                    string currentdir = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                    if (!System.IO.Directory.Exists(currentdir))
                    {
                        System.IO.Directory.CreateDirectory(currentdir);
                    }
                    CopyDir(file, destfolderdir);
                }
                else
                {
                    string srcfileName = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                    if (!System.IO.Directory.Exists(destfolderdir))
                    {
                        System.IO.Directory.CreateDirectory(destfolderdir);
                    }
                    System.IO.File.Copy(file, srcfileName);
                }
            }

            return 1;
        }
        catch (Exception e)
        {

            Debug.LogError(e.Message);
            return 0;
        }

    }
    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="fileFullPath">文件绝对路径</param>
    /// <returns></returns>
    public static bool ExistsFile(string fileFullPath)
    {
        return File.Exists(fileFullPath);
    }
    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="fileFullName">文件.后缀名</param>
    /// <param name="dirPath">文件夹</param>
    /// <returns></returns>
    public static bool ExistsFile(string fileFullName, string dirPath)
    {
        string fileFullPath = Path.Combine(dirPath, fileFullName);
        if (fileFullName == null || fileFullName.Length == 0)
            return false;
        else
            return File.Exists(fileFullPath);
    }
    /// <summary>
    /// 检测路径是否存在
    /// </summary>
    /// <param name="fileFullPath">文件夹路径</param>
    public static void CheckCreateDirectory(string fileFullPath)
    {
        var dirPath = Path.GetDirectoryName(fileFullPath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }
    /// <summary>
    /// 检测文件夹是否存在
    /// </summary>
    public static bool ExistsPath(string path)
    {
        if (path == null || path.Length == 0)
            return false;
        else
            return Directory.Exists(path);
    }
    /// <summary>
    /// 写入内容
    /// </summary>
    /// <param name="fileFullName">文件.后缀名</param>
    /// <param name="content">内容</param>
    /// <param name="dirPath">文件夹路径</param>
    public static void WriteToFile(string fileFullName, string content, string dirPath)
    {
        string filePath = Path.Combine(dirPath, fileFullName);
        FileInfo file = new FileInfo(filePath);
        StreamWriter sw = file.AppendText();
        //写入信息
        sw.WriteLine(content);
        sw.Flush();
        sw.Close();
    }
    /// <summary>
    /// 写入二进制文件
    /// </summary>
    /// <param name="fileFullName">文件.后缀名</param>
    /// <param name="bytes">内容</param>
    /// <param name="dirPath">文件夹路径</param>
    public static void WriteByteToFile(string fileFullName, byte[] bytes, string dirPath)
    {
        string filePath = Path.Combine(dirPath, fileFullName);
        CheckCreateDirectory(filePath);
        FileInfo file = new FileInfo(filePath);
        FileStream sw = file.OpenWrite();
        sw.Write(bytes, 0, bytes.Length);
        sw.Flush();
        sw.Close();
    }
    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="fileFullName">文件.后缀名</param>
    /// <param name="dirPath">文件夹路径</param>
    /// <returns></returns>
    public static string ReadFromFile(string fileFullName, string dirPath)
    {
        string fileFullPath = Path.Combine(dirPath, fileFullName);
        return ReadFromFile(fileFullPath);
    }
    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="fileFullPath">文件绝对路径</param>
    /// <returns></returns>
    public static string ReadFromFile(string fileFullPath)
    {
        FileInfo file = new FileInfo(fileFullPath);
        string content = "";
        if (file.Exists)
        {
            using (StreamReader sr = file.OpenText())
            {
                content = sr.ReadToEnd();
            }
        }
        return content;
    }
    /// <summary>
    /// 读取多行数据
    /// </summary>
    /// <param name="fileFullPath">文件绝对路径</param>
    /// <returns></returns>
    public static List<string> ReadLineFromFile(string fileFullPath)
    {
        FileInfo file = new FileInfo(fileFullPath);
        List<string> lineList = new List<string>();
        if (file.Exists)
        {
            StreamReader sr = file.OpenText();
            while (sr.Peek() >= 0)
            {
                lineList.Add(sr.ReadLine());
            }
            sr.Close();
        }
        return lineList;
    }
    /// <summary>
    /// 读取多行数据
    /// </summary>
    /// <param name="fileFullName">文件.后缀名</param>
    /// <param name="dirPath">文件夹路径</param>
    /// <returns></returns>
    public static List<string> ReadLineFromFile(string fileFullName, string dirPath)
    {
        string fileFullPath = Path.Combine(dirPath, fileFullName);
        return ReadLineFromFile(fileFullPath);
    }
    /// <summary>
    ///  删除文件
    /// </summary>
    /// <param name="fileFullPath"></param>
    public static void DeleteFile(string fileFullPath)
    {
        FileInfo file = new FileInfo(fileFullPath);
        if (file.Exists)
        {
            file.Delete();
        }
    }
    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="dirPath"></param>
    public static void DeleteDir(string dirPath)
    {
        if (Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
        }
    }

    /// <summary>
    /// 将给的目录路径合并起来
    /// </summary>
    /// <param name="endWithBackslash">路径最后是否以反斜杠结束</param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string CombineDirs(bool isEndWithBackslash, params string[] args)
    {
        string path = CombinePaths(args);

        if (isEndWithBackslash)
        {
            if (false == path.EndsWith("/"))
            {
                path += "/";
            }
        }
        else
        {
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
        }

        return path;
    }

    /// <summary>
    /// 将给的路径合并起来
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string CombinePaths(params string[] args)
    {
        if (args.Length == 0)
        {
            return "";
        }

        string path = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            var node = RemoveStartPathSeparator(args[i]);
            path = Path.Combine(path, node);
        }

        //为了好看
        path = StandardizeBackslashSeparator(path);

        return path;
    }
    /// <summary>
    /// 标准化路径中的路径分隔符（统一使用“/”符号）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string StandardizeBackslashSeparator(string path)
    {
        path = path.Replace("\\", "/");
        return path;
    }

    /// <summary>
    /// 标准化路径中的路径分隔符（统一使用“\”符号）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string StandardizeSlashSeparator(string path)
    {
        path = path.Replace("/", "\\");
        return path;
    }

    /// <summary>
    /// 如果路径开头有文件分隔符，则移除
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string RemoveStartPathSeparator(string path)
    {
        if (path.StartsWith("/"))
        {
            return path.Substring(1);
        }
        else if (path.StartsWith("\\"))
        {
            return path.Substring(2);
        }

        return path;
    }

    /// <summary>
    /// 如果路径结尾有文件分隔符，则移除
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string RemoveEndPathSeparator(string path)
    {
        if (path.EndsWith("/"))
        {
            return path.Substring(1);
        }
        else if (path.EndsWith("\\"))
        {
            return path.Substring(2);
        }

        return path;
    }
}
