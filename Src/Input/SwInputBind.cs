using System;
using System.Collections.Generic;
using Godot;
using SW.Src.Utils;

namespace SW.Src.Input;

public class SwInputBind
{
    public enum SwInputType
    {
        Button,
        Axis,
        Axis2,
    }
    public readonly SwInputType InputType;
    public enum SwDataType
    {
        Key,
        MouseButton,
        JoyButton,
        JoyAxis,
        String,
    }
    public readonly SwDataType DataType;
    public readonly string InputName;
    public readonly string MethodName;
    public readonly int Device;
    public readonly int[] Values;
    public readonly string[] SourceNames;
    private SwInputBind(
        SwInputType inputType,
        SwDataType dataType,
        string inputName,
        string methodName,
        int device,
        int[] values,
        string[] sourceNames)
    {
        InputType = inputType;
        DataType = dataType;
        InputName = inputName;
        MethodName = methodName;
        Device = device;
        Values = values;
        SourceNames = sourceNames;
    }
    public static bool TryFromDb(SwJsonDb db, out SwInputBind inputBind)
    {
        inputBind = default;
        int device = 0;
        if(!db.TryGetString("input_name", out string inputName)) return false;
        if(!db.TryGetString("method_name", out string methodName)) return false;
        if(!db.TryGetArray("values", out var arr)) return false;
        if(!db.TryGetNumber("input_type", out int iType)) return false;
        SwInputType inputType = (SwInputType)iType;
        if(!Enum.IsDefined(inputType)) return false;
        if(!db.TryGetNumber("data_type", out int dType)) return false;
        SwDataType dataType = (SwDataType)dType;
        if(!Enum.IsDefined(dataType)) return false;
        if((dataType == SwDataType.JoyAxis || dataType == SwDataType.JoyButton) && !db.TryGetNumber("device", out device)) return false;
        List<int> values = [];
        List<string> names = [];
        if(dataType == SwDataType.String)
        {
            foreach (var item in arr)
            {
                if(!SwJsonDb.TryAsString(item, out string name)) return false;
                names.Add(name);
            }
        }
        else
        {
            foreach (var item in arr)
            {
                if(!SwJsonDb.TryAsNumber(item, out int value)) return false;
                values.Add(value);
            }
        }
        inputBind = new(inputType, dataType, inputName, methodName, device, [..values], [..names]);
        return true;
    }
}
