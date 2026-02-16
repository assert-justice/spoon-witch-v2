using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Godot;

namespace SW.Src.Utils;

public class SwFs
{
    public virtual bool FileExists(string path)
    {
        return FileAccess.FileExists(path) | 
        FileAccess.FileExists("res://" + path) | 
        FileAccess.FileExists("user://" + path);
    }
    public virtual bool DirectoryExists(string path)
    {
        return DirAccess.DirExistsAbsolute(path) | 
        DirAccess.DirExistsAbsolute("res://" + path) | 
        DirAccess.DirExistsAbsolute("user://" + path);
    }
    public virtual bool TryReadFile(string path, out string contents)
    {
        contents = default;
        if(!TryResolveFilePath(path, out string resolvedPath))
        {
            GD.PrintErr($"Could not find file at path '{path}'");
			return false;
        }
        using var file = FileAccess.Open(resolvedPath, FileAccess.ModeFlags.Read);
		if(file is null)
		{
			GD.PrintErr($"Could not open file at path '{path}'");
			return false;
		}
		contents = file.GetAsText();
        return true;
    }
    public virtual bool TryReadJson(string path, out JsonNode jsonNode)
    {
        jsonNode = default;
        if(!TryReadFile(path, out string contents)) return false;
        try
        {
            jsonNode = JsonNode.Parse(contents);
            return true;
        }
        catch(Exception e)
        {
            GD.PrintErr($"Error parsing json at path '{path}': {e}");
			return false;
        }
    }
    public virtual bool TryWriteFile(string path, string contents)
    {
        try
        {
            using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.StoreString(contents);
            return true;
        }
        catch(Exception e)
        {
            GD.PrintErr($"Error writing to file at path '{path}': {e}");
            return false;
        }
    }
    public virtual string[] DirectoryListFiles(string path)
    {
        using var dir = DirAccess.Open(path);
		if(dir is null)
		{
			GD.PrintErr($"Failed to open directory at path '{path}'.");
			return [];
		}
		List<string> filenames = [];
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (!dir.CurrentIsDir())
				{
					filenames.Add(fileName);
				}
				fileName = dir.GetNext();
			}
		}
		return [..filenames];
    }
    public virtual string[] DirectoryListDirectories(string path)
    {
        using var dir = DirAccess.Open(path);
		if(dir is null)
		{
			GD.PrintErr($"Failed to open directory at path '{path}'.");
			return [];
		}
		List<string> filenames = [];
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "")
			{
				if (dir.CurrentIsDir())
				{
					filenames.Add(fileName);
				}
				fileName = dir.GetNext();
			}
		}
		return [..filenames];
    }
    protected virtual bool TryResolveFilePath(string path, out string resolvedPath)
    {
        resolvedPath = path;
        if(FileAccess.FileExists(resolvedPath)) return true;
        resolvedPath = "res://" + path;
        if(FileAccess.FileExists(resolvedPath)) return true;
        resolvedPath = "user://" + path;
        return FileAccess.FileExists(resolvedPath);
    }
    protected virtual bool TryResolveDirPath(string path, out string resolvedPath)
    {
        resolvedPath = path;
        if(DirAccess.DirExistsAbsolute(resolvedPath)) return true;
        resolvedPath = "res://" + path;
        if(DirAccess.DirExistsAbsolute(resolvedPath)) return true;
        resolvedPath = "user://" + path;
        return DirAccess.DirExistsAbsolute(resolvedPath);
    }
}
