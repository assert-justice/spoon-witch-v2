using System;
using System.Text.Json.Nodes;
using Godot;
using SW.Src.Global;

namespace SW.Src.Utils;

public partial class SwDirtyDb
{
    private string DbPath;
    private int Hash = 0;
    private SwJsonDb Db_;
    public SwJsonDb Db{get=>Db_??=Read();}
    public SwDirtyDb(){}
    public SwDirtyDb(string path){
        DbPath = path;
        Db_ = Read();
    }
    private string ReadString()
    {
        string path = GetPath();
        if(!FileAccess.FileExists(path)) return "";
        return FileAccess.GetFileAsString(path);
    }
    private SwJsonDb Read()
    {
        string res = ReadString();
        Hash = 0;
        if(res.Length == 0) return new();
        try
        {
            var json = JsonNode.Parse(res);
            Hash = res.GetHashCode();
            return new(json);
        }
        catch
        {
            return new();
        }
    }
    public void Clean()
    {
        string path = GetPath();
        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        if(file == null) SwStatic.LogError($"Failed to write db data to path '{path}'");
        else file.StoreString(Db.ToString());
    }
    public string GetPath(){return DbPath??=$"res://db/{Guid.NewGuid()}";}
    public bool IsDirty(){return Db is null || Db.ToString().GetHashCode() != Hash;}
    public bool IsOutdated(){return ReadString().GetHashCode() != Hash;}
    public SwJsonDb Update()
    {
        var data = Read();
        Db_ = data;
        return data;
    }
}
