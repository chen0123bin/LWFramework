using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class SystemInfoTool
{
    /// <summary>
    /// 获取页面html
    /// </summary>
    /// <param name="url">请求的地址</param>
    /// <param name="encoding">编码方式</param>
    /// <returns></returns>
    private static string HttpGetPageHtml(string url, string encoding)
    {
        string pageHtml = string.Empty;
        try
        {
            using (WebClient MyWebClient = new WebClient())
            {
                Encoding encode = Encoding.GetEncoding(encoding);
                MyWebClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                pageHtml = encode.GetString(pageData);
            }
        }
        catch (Exception e)
        {
            LWDebug.Log(e.StackTrace);
        }
        return pageHtml;
    }
    /// <summary>
    /// 从html中通过正则找到ip信息(只支持ipv4地址)
    /// </summary>
    /// <param name="pageHtml"></param>
    /// <returns></returns>
    private static string GetIPFromHtml(String pageHtml)
    {
        //验证ipv4地址
        string reg = @"(?:(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))\.){3}(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))";
        string ip = "";
        Match m = Regex.Match(pageHtml, reg);
        if (m.Success)
        {
            ip = m.Value;
        }
        return ip;
    }

    /// <summary>
    /// 获取本机所有ip地址
    /// </summary>
    /// <param name="netType">"InterNetwork":ipv4地址，"InterNetworkV6":ipv6地址</param>
    /// <returns>ip地址集合</returns>
    private static List<string> GetLocalIpAddress(string netType)
    {
        string hostName = Dns.GetHostName();                    //获取主机名称  
        IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址  

        List<string> IPList = new List<string>();
        if (netType == string.Empty)
        {
            for (int i = 0; i < addresses.Length; i++)
            {
                IPList.Add(addresses[i].ToString());
            }
        }
        else
        {
            //AddressFamily.InterNetwork表示此IP为IPv4,
            //AddressFamily.InterNetworkV6表示此地址为IPv6类型
            for (int i = 0; i < addresses.Length; i++)
            {
                if (addresses[i].AddressFamily.ToString() == netType)
                {
                    IPList.Add(addresses[i].ToString());
                }
            }
        }
        return IPList;
    }
    //获取联网状态
    public static bool GetNetWorkState()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 获取设备的IP地址，联网的情况下获取外网IP  不联网的情况下获取127.0.0.1
    /// </summary>
    /// <returns></returns>
    public static string GetIpAddress() {
        string retIp = "";
        if (GetNetWorkState())
        {
            var t0_html = HttpGetPageHtml("https://www.ip.cn", "utf-8");

            //var t0_html = HttpGetPageHtml("https://www.ip.cn", "utf-8");
            //var t1_html = HttpGetPageHtml("http://www.ip138.com/ips138.asp", "gbk");
            //var t2_html = HttpGetPageHtml("http://www.net.cn/static/customercare/yourip.asp", "gbk");
            retIp = GetIPFromHtml(t0_html);// 111.198.29.123
        }
        else {
            retIp = GetLocalIpAddress("InterNetwork")[0];
        }
        return retIp;
    }



    public static string GetMacAddress()
    {
        string physicalAddress = "";
        NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adaper in nice)
        {
            //Debug.Log(adaper.Description);
            if (adaper.Description == "en0")
            {
                physicalAddress = adaper.GetPhysicalAddress().ToString();
                break;
            }
            else
            {
                physicalAddress = adaper.GetPhysicalAddress().ToString();
                if (physicalAddress != "")
                {
                    break;
                };
            }
        }
        return physicalAddress;
    }
    /// <summary>
    /// 获取设备唯一id
    /// </summary>
    /// <returns></returns>
    public static string GetDeviceUniqueIdentifier() {
        return SystemInfo.deviceUniqueIdentifier;
    }

    //获取显卡唯一id
    public static string GetGraphicsDeviceID() {
        return SystemInfo.graphicsDeviceID+"-"+ SystemInfo.graphicsDeviceVendorID;
    }

}
