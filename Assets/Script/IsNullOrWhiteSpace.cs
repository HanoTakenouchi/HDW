using System;

public static partial class StringCommon{
	public static bool IsNullOrWhiteSpace(string value)
    {
        return value == null || value.Trim() == "";
    }
}

