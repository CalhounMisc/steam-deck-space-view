using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Forms;

public partial class MainForm : Form
{
    private List<InstalledGame> set = new List<InstalledGame>();

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Populate();

    }

    private long GetDirSize(DirectoryInfo dir)
    {
        var cmd = "du -sb " + dir.FullName;
        var result = AppUtil.Instance.ExecuteCommand(cmd);
        result = result.Trim();

        string val = "";

        foreach (char c in result)
        {
            if (char.IsDigit(c))
            {
                val += c;
            }
            else
            {
                break;
            }
        }

        return long.Parse(val);
    }

    private void Populate()
    {
        try
        {
            set.Clear();

            long totalCompat = 0;
            long totalShader = 0;

            foreach (var compatDataPath in AppUtil.Instance.CompatdataPaths)
            {
                var compatDataFolder = new DirectoryInfo(compatDataPath);

                foreach (var compatChildDir in compatDataFolder.GetDirectories())
                {
                    long dirNameId = 0;
                    if (long.TryParse(compatChildDir.Name, out dirNameId))
                    {
                        var item = set.FirstOrDefault(i => i.Id == dirNameId);

                        if (item == null)
                        {
                            item = new InstalledGame(dirNameId, compatDataFolder.Parent.FullName);
                            set.Add(item);
                        }

                        item.CompatDataSizeLong = GetDirSize(compatChildDir);
                        item.CompatFolderPath = compatChildDir.FullName;
                        totalCompat += item.CompatDataSizeLong;
                    }
                }
            }

            foreach (var shaderCachePath in AppUtil.Instance.ShadercachePaths)
            {
                var shaderCacheFolder = new DirectoryInfo(shaderCachePath);

                foreach (var shaderCacheChildDir in shaderCacheFolder.GetDirectories())
                {
                    long dirNameId = 0;

                    if (long.TryParse(shaderCacheChildDir.Name, out dirNameId))
                    {
                        var item = set.FirstOrDefault(i => i.Id == dirNameId);

                        if (item == null)
                        {
                            item = new InstalledGame(dirNameId, shaderCacheFolder.Parent.FullName);
                            set.Add(item);
                        }

                        item.ShaderCacheSizeLong = GetDirSize(shaderCacheChildDir);
                        item.ShaderCacheFolderPath = shaderCacheChildDir.FullName;
                        totalShader += item.ShaderCacheSizeLong;
                    }

                }
            }

            shaderCacheTotalLabel.Text = "Total ShaderCache (MB): " + InstalledGame.ToSize(totalShader);
            compatDataTotalLabel.Text = "Total CompatData (MB): " + InstalledGame.ToSize(totalCompat);

            Sort(TotalColumn);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), MessageBoxType.Error);
        }
    }

    private void Sort(GridColumn col)
    {

        if (col == NameColumn)
        {
            grid.DataStore = set.OrderBy(keySelector: c => c.Name);
        }
        else if (col == LocationColumn)
        {
            grid.DataStore = set.OrderBy(keySelector: c => c.DataFolder);
        }
        else if (col == IdColumn)
        {
            grid.DataStore = set.OrderBy(keySelector: c => c.Id);
        }
        else if (col == CompactDataColumn)
        {
            grid.DataStore = set.OrderByDescending(keySelector: c => c.CompatDataSizeLong);
        }
        else if (col == ShaderCacheColumn)
        {
            grid.DataStore = set.OrderByDescending(keySelector: c => c.ShaderCacheSizeLong);
        }
        else if (col == TotalColumn)
        {
            grid.DataStore = set.OrderByDescending(keySelector: c => c.TotalSizeLong);
        }
        else if (col == InstalledColumn)
        {
            grid.DataStore = set.OrderByDescending(keySelector: c => c.Installed);
        }
        else
        {
            throw new Exception();
        }
    }


}