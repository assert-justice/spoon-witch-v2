using System;
using System.Text.Json.Nodes;

namespace SW.Src.Utils;
public class SwJsonDb
{
    JsonNode Data;
    public enum JsonType
    {
        Null,
        String,
        Number,
        Array,
        Object,
        Bool,
    }
    public SwJsonDb()
    {
        Data = new JsonObject();
    }
    public SwJsonDb(JsonNode data)
    {
        Data = data;
    }
    public void SetData(JsonNode data){Data = data;}
    public JsonNode GetData(){return Data;}
    public bool TryGetPath(string path, out JsonNode jsonNode)
    {
        jsonNode = null;
        JsonNode node = Data;
        foreach (var item in path.Split(','))
        {
            if(item.Length == 0) return false;
            if(GetType(node) != JsonType.Object) return false;
            node = node.AsObject()[item];
        }
        jsonNode = node;
        return true;
    }
    public bool TryGetString(string path, out string res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsString(node, out res);
    }
    public bool TryGetNumber(string path, out double res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsNumber(node, out res);
    }
    public bool TryGetNumber(string path, out float res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsNumber(node, out res);
    }
    public bool TryGetNumber(string path, out int res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsNumber(node, out res);
    }
    public bool TryGetBool(string path, out bool res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsBool(node, out res);
    }
    public bool TryGetArray(string path, out JsonArray res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsArray(node, out res);
    }
    public bool TryGetObject(string path, out JsonObject res)
    {
        res = default;
        if(!TryGetPath(path, out var node)) return false;
        return TryAsObject(node, out res);
    }
    public bool TryGetObjectDb(string path, out SwJsonDb db)
    {
        db = default;
        if(!TryGetObject(path, out var obj)) return false;
        db = new(obj);
        return true;
    }
    public bool TrySetPath(string path, JsonNode value)
    {
        if(path == "")
        {
            Data = value;
            return true;
        }
        JsonNode node = Data;
        var p = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        int idx = 0;
        string key;
        while(true)
        {
            key = p[idx];
            if(key.Length == 0) return false;
            if(idx == p.Length - 1) break;
            if(GetType(node) != JsonType.Object) return false;
            JsonObject obj = node.AsObject();
            node = obj[key];
            idx++;
        }
        node[key] = value;
        return true;
    }
    public override string ToString()
    {
        return Data.ToJsonString();
    }

    public static bool TryAsString(JsonNode node, out string res)
    {
        res = default;
        if(GetType(node) != JsonType.String) return false;
        node.AsValue().TryGetValue(out res);
        return true;
    }
    public static bool TryAsNumber(JsonNode node, out double res)
    {
        res = default;
        if(GetType(node) != JsonType.Number) return false;
        node.AsValue().TryGetValue(out res);
        return true;
    }
    public static bool TryAsNumber(JsonNode node, out float res)
    {
        res = default;
        if(GetType(node) != JsonType.Number) return false;
        node.AsValue().TryGetValue(out res);
        return true;
    }
    public static bool TryAsNumber(JsonNode node, out int res)
    {
        res = default;
        if(GetType(node) != JsonType.Number) return false;
        node.AsValue().TryGetValue(out res);
        return true;
    }
    public static bool TryAsBool(JsonNode node, out bool res)
    {
        res = default;
        if(GetType(node) != JsonType.Bool) return false;
        node.AsValue().TryGetValue(out res);
        return true;
    }
    public static bool TryAsArray(JsonNode node, out JsonArray res)
    {
        res = default;
        if(GetType(node) != JsonType.Array) return false;
        res = node.AsArray();
        return true;
    }
    public static bool TryAsObject(JsonNode node, out JsonObject res)
    {
        res = default;
        if(GetType(node) != JsonType.Object) return false;
        res = node.AsObject();
        return true;
    }

    public static JsonType GetType(JsonNode node)
    {
        if(node is null) return JsonType.Null;
        var kind = node.GetValueKind();
        return kind switch
        {
            System.Text.Json.JsonValueKind.Undefined or System.Text.Json.JsonValueKind.Null => JsonType.Null,
            System.Text.Json.JsonValueKind.True or System.Text.Json.JsonValueKind.False => JsonType.Bool,
            System.Text.Json.JsonValueKind.Object => JsonType.Object,
            System.Text.Json.JsonValueKind.Array => JsonType.Array,
            System.Text.Json.JsonValueKind.Number => JsonType.Number,
            _ => throw new Exception("Should be unreachable"),
        };
    }
}
