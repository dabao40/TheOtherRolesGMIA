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

        DevUserList.Add(new(code: "farwig#2804", color: "#FFFFE0", tag: "开发者Among us"));
        DevUserList.Add(new(code: "breathzany#4350", color: "#FCCE03FF", tag: "开发者Imp11"));
        //DevUserList.Add(new(code: "sofaagile#3120", color: "null", tag: "梦初私服提供者:天寸"));  (天寸不知道能不能同意加所以先注释掉）
        //  空白的例子   DevUserList.Add(new(code: "xxxxxx#0000", color: "#000000", tag: ""));




    }
    public static bool IsDevUser(this string code) => DevUserList.Any(x => x.Code == code);
    public static DevUser GetDevUser(this string code) => code.IsDevUser() ? DevUserList.Find(x => x.Code == code) : DefaultDevUser;
}
