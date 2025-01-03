using System.Text;
using UnityEngine;


public static class Logger
{
    public static StringBuilder FullLog { get; private set; }
    public static void PrintFullLog() => Debug.Log(FullLog);


    #region -------------------------------------------------------------------- Log
    public static void Log(object _message, Object _context)
    {
        Append(_message, _context.name);
        Debug.Log(_message, _context);
    }

    public static void Log(object _message, Object _context, string _filter)
    {
        Append(_message, _context.name);
        Debug.Log($"{FormateFilter(_filter)}{_message}", _context);
    }

    public static void Log(object _message, Object _context, string _filter, Color _color)
    {
        Append(_message, _context.name);
        Debug.Log($"{FormateFilter(_filter, ColorUtility.ToHtmlStringRGB(_color))}{_message}", _context);
    }
    #endregion





    #region -------------------------------------------------------------------- Warning
    public static void LogWarning(object _message, Object _context)
    {
        Append(_message, _context.name);
        Debug.LogWarning(_message, _context);
    }

    public static void LogWarning(object _message, Object _context, string _filter)
    {
        Append(_message, _context.name);
        Debug.LogWarning($"{FormateFilter(_filter)}{_message}", _context);
    }

    public static void LogWarning(object _message, Object _context, string _filter, Color _color)
    {
        Append(_message, _context.name);
        Debug.LogWarning($"{FormateFilter(_filter, ColorUtility.ToHtmlStringRGB(_color))}{_message}", _context);
    }
    #endregion





    #region -------------------------------------------------------------------- Error
    public static void LogError(object _message, Object _context)
    {
        Append(_message, _context.name);
        Debug.LogError(_message, _context);
    }

    public static void LogError(object _message, Object _context, string _filter)
    {
        Append(_message, _context.name);
        Debug.LogError($"{FormateFilter(_filter)}{_message}", _context);
    }

    public static void LogError(object _message, Object _context, string _filter, Color _color)
    {
        Append(_message, _context.name);
        Debug.LogError($"{FormateFilter(_filter, ColorUtility.ToHtmlStringRGB(_color))}{_message}", _context);
    }
    #endregion





    private static void Append(object _object, string _className)
    {
        if (FullLog == null) FullLog = new StringBuilder(GetInfo());
        FullLog.Append($"{_object} ({_className}.cs)\n");
    }

    private static string FormateFilter(string _title, string _color = "") => string.IsNullOrWhiteSpace(_color) ? $"<b>[{_title}]</b> " : $"<color=#{_color}><b>[{_title}]</b></color> ";

    private static string GetInfo()
    {
        string _date = System.DateTime.Now.ToString();
        string _systemInfo = $"System Info: \nName: {SystemInfo.deviceName}\nModel: {SystemInfo.deviceModel}\nGraphics: {SystemInfo.graphicsDeviceName}\nGraphics Memory: {SystemInfo.graphicsMemorySize}\nCPU: {SystemInfo.processorType}\nRAM: {SystemInfo.systemMemorySize}";
        return $"Food Fury v{Application.version} | {_date} | {System.TimeZoneInfo.Local.DisplayName}\n\n{_systemInfo} \n\n----------------------------------------------------------------------\n\n";
    }




}
