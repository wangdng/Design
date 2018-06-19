using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleFtpIno
{
    public string FtpUrl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool Passive { get; set; }
    public string SrcDirPath { get; set; }

    public AssetBundleFtpIno(string ftpUrl, string userName, string password, bool passive)
    {
        if (string.IsNullOrEmpty(ftpUrl))
            throw new ArgumentException("dirUrl");
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentException("userName");
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("password");

        FtpUrl = ftpUrl;
        UserName = userName;
        Password = password;
        Passive = passive;
    }
}
