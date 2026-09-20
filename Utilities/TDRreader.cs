using System.IO;

namespace ModifierTool;

public static class TDRreader
{
    public static FileInfo GetTDR(DirectoryInfo bike)
    {
        FileInfo tdrFile = bike.GetFiles("*.tdr")[0];
        return tdrFile;
    }
}