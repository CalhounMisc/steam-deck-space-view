using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

public class VdfReader
{
    private byte[] allBytes;
    private int idx;
    private StringBuilder buffer = new StringBuilder();
    private UTF8Encoding utf8 = new UTF8Encoding(true);
    public VdfReader()
    {
    }

    private bool IsFieldLabel(string field)
    {

        string val = buffer.ToString();

        if (val.EndsWith(field))
        {
            return Char.IsControl(val[val.Length - field.Length - 1]);
        }
        else
        {
            return false;
        }
    }

    private char GetNextChar()
    {
        byte[] selectedBytes = allBytes.Skip(idx).Take(1).ToArray();
        return utf8.GetChars(selectedBytes)[0];
    }

    private byte[] ReadBytes(int length)
    {
        byte[] selectedBytes = allBytes.Skip(idx).Take(length).ToArray();
        idx += length;

        buffer.Append(utf8.GetChars(selectedBytes));

        return selectedBytes;
    }

    private char ReadChar()
    {
        ReadBytes(1);
        return buffer[buffer.Length - 1];
    }

    public ShortcutEntry[] Read(string path)
    {

        idx = 0;
        allBytes = File.ReadAllBytes(path);

        List<ShortcutEntry> result = new List<ShortcutEntry>();
        ShortcutEntry current = null;

        while (true)
        {
            buffer.Clear();
            while (!IsFieldLabel("appid"))
            {
                if (idx == allBytes.Length)
                {
                    return result.ToArray();
                }

                ReadChar();
            }

            ReadChar(); // skip control chars between fields
            buffer.Clear();

            current = new ShortcutEntry();
            result.Add(current);

            var bytes = ReadBytes(sizeof(int));
            current.AppId = BitConverter.ToInt32(bytes, 0) & 0xffffffff;

            while (!IsFieldLabel("AppName"))
            {
                ReadChar();
            }

            ReadChar(); // skip control chars between fields
            buffer.Clear();

            while (!char.IsControl(GetNextChar()))
            {
                ReadChar();
            }
            current.AppName = buffer.ToString();
            buffer.Clear();

            while (!IsFieldLabel("Exe"))
            {
                ReadChar();
            }

            ReadChar(); // skip control chars between fields
            buffer.Clear();

            while (!char.IsControl(GetNextChar()))
            {
                ReadChar();
            }
            current.Exe = buffer.ToString();
            buffer.Clear();
        }
    }
}

public class ShortcutEntry
{
    public long AppId { get; set; }
    public string Exe { get; set; }
    public string AppName { get; set; }

    public override string ToString()
    {
        return AppId + "_" + AppName;
    }
}