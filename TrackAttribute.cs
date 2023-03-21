using System;

[AttributeUsage(AttributeTargets.Field)]

public class TrackAttribute : Attribute
{
    public string DisplayName;
    public TrackAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}