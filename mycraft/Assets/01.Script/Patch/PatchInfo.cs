using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchInfo {

    public long _totalByte = 0;
    public string _filename;

    public PatchInfo(string filename, long totalByte)
    {
        _filename = filename;
        _totalByte = totalByte;
    }

    public void SetTotalByte(long totalByte)
    {
        _totalByte = totalByte;
    }
}
