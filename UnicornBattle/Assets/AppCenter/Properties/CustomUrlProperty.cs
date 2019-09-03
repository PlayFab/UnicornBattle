// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

[Serializable]
public class CustomUrlProperty
{
    public CustomUrlProperty(string urlName)
    {
        UrlName = urlName;
    }
    public string UrlName = "";
    public bool UseCustomUrl;
    public string Url = "";
}
