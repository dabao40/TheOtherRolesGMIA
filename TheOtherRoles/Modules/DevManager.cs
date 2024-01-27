using System.Collections.Generic;
using System.Linq;

namespace TheOtherRoles.Modules;

public class DevUser
{
    public string Code { get; set; }
    public string Color { get; set; }
    public string Tag { get; set; }
    public bool IsUp { get; set; }
    public bool IsDev { get; set; }
    public bool DeBug { get; set; }
    public string UpName { get; set; }
    public DevUser(string code = "", string color = "null", string tag = "null")
    {
        Code = code;
        Color = color;
        Tag = tag;

    }
    public bool HasTag() => Tag != "null";
    public string GetTag() => Color == "null" ? $"<size=1.7>{Tag}</size>\r\n" : $"<color={Color}><size=1.7>{(Tag)}</size></color>\r\n";
}

public static class DevManager
{
    public static DevUser DefaultDevUser = new();
    public static List<DevUser> DevUserList = new();
    public static void Init()
    {
        DevUserList.Add(new(code: "breathzany#4350", color: "#FCCE03FF", tag: ModTranslation.getString("devimp11")));
        DevUserList.Add(new(code: "farwig#2804", color: "#FFFFE0", tag: ModTranslation.getString("devau")));
        DevUserList.Add(new(code: "likebug#3428", color: "#5f1d2e", tag: ModTranslation.getString("devasakuna")));
        //  空白的例子   DevUserList.Add(new(code: "xxxxxx#0000", color: "#000000", tag: ""));




    }
    public static bool IsDevUser(this string code) => DevUserList.Any(x => x.Code == code);
    public static DevUser GetDevUser(this string code) => code.IsDevUser() ? DevUserList.Find(x => x.Code == code) : DefaultDevUser;
}
