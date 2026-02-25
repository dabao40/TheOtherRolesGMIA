using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innersloth.IO;

namespace TheOtherRoles.Modules;

public abstract class DataEntry<T> where T : notnull
{
    private T value;
    string name;
    DataSaver saver;

    public T Value
    {
        get { return value; }
        set
        {
            this.value = value;
            saver.SetValue(name, Serialize(value));
        }
    }

    public void SetValueWithoutSave(T value)
    {
        this.value = value;
        saver.SetValue(name, Serialize(value), true);
    }

    public DataEntry(string name, DataSaver saver, T defaultValue)
    {
        this.name = name;
        this.saver = saver;
        value = Parse(saver.GetValue(name, Serialize(defaultValue)));
    }

    public abstract T Parse(string str);
    protected virtual string Serialize(T value) => value.ToString()!;
}

public class IntegerDataEntry : DataEntry<int>
{
    public override int Parse(string str) { return int.Parse(str); }
    public IntegerDataEntry(string name, DataSaver saver, int defaultValue) : base(name, saver, defaultValue) { }
}

public class StringDataEntry : DataEntry<string>
{
    public override string Parse(string str) { return str; }
    public StringDataEntry(string name, DataSaver saver, string defaultValue) : base(name, saver, defaultValue) { }
}

public class BooleanDataEntry : DataEntry<bool>
{
    public override bool Parse(string str) { return bool.TryParse(str, out var result) ? result : false; }
    public BooleanDataEntry(string name, DataSaver saver, bool defaultValue) : base(name, saver, defaultValue) { }
}

public class DataSaver
{
    private Dictionary<string, string> contents = new();
    string filename;

    public string GetValue(string name, object defaultValue)
    {
        if (contents.TryGetValue(name, out string value))
        {
            return value!;
        }
        var res = contents[name] = defaultValue.ToString()!;
        return res;
    }

    public void SetValue(string name, object value, bool skipSave = false)
    {
        contents[name] = value.ToString()!;
        if (!skipSave) Save();
    }

    public DataSaver(string filename)
    {
        this.filename = "TheOtherRolesGMIA\\" + filename + ".dat";
        Load();
    }

    public void Load()
    {
        string dataPathTo = FileIO.GetDataPathTo([filename]);

        if (!FileIO.Exists(dataPathTo)) return;

        string[] vals = (FileIO.ReadAllText(dataPathTo)).Split("\n");
        foreach (string val in vals)
        {
            string[] str = val.Split(":", 2);
            if (str.Length != 2) continue;
            contents[str[0]] = str[1];
        }
    }

    public void Save()
    {
        string strContents = "";
        foreach (var entry in contents)
        {
            strContents += entry.Key + ":" + entry.Value + "\n";
        }
        FileIO.WriteAllText(FileIO.GetDataPathTo([filename]), strContents);
    }
}
