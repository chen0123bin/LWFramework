using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateTool
{
    public const string dataFormat = "yyyy年MM月dd日 HH:mm:ss";
    public static string Date2Str(DateTime dateTime,string format = dataFormat) {
        return dateTime.ToString(format);
    }
    public static DateTime Str2Date(string dateString, string format = dataFormat) { 
        return DateTime.ParseExact(dateString, dataFormat, System.Globalization.CultureInfo.CurrentCulture);
    }

    public static string GetNowStr(string format = dataFormat) {
        return Date2Str(DateTime.Now, format);
    }
   
    /// <summary>
    /// 比较日期
    /// </summary>
    /// <param name="dateStr1"></param>
    /// <param name="dateStr2"></param>
    /// <returns>大于0 1大   小于0 2大</returns>
    public static int CompanyDate(DateTime t1, DateTime t2)
    {
    
        //通过DateTIme.Compare()进行比较（）
        int compNum = DateTime.Compare(t1, t2);

        return compNum;
    }
    /// <summary>
    /// 比较日期
    /// </summary>
    /// <param name="dateStr1"></param>
    /// <param name="dateStr2"></param>
    /// <returns>大于0 1大   小于0 2大</returns>
    public static int CompanyDate(string str1, string str2)
    {        
        //通过DateTIme.Compare()进行比较（）
        int compNum = DateTime.Compare(Str2Date(str1), Str2Date(str2));

        return compNum;
    }
}
