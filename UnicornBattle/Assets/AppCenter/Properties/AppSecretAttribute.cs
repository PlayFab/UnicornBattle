// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class AppSecretAttribute : PropertyAttribute
{
    public string Name { get; set; }
    
    public AppSecretAttribute()
    {
    }

    public AppSecretAttribute(string name)
    {
        Name = name;
    }
}
