using System;
using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;

// https://github.com/picoe/Eto/wiki/Quick-Start

public partial class MainForm : Form
{
    private GridView grid = new GridView();
    private Label shaderCacheTotalLabel = new Label() { Text = "??" };
    private Label compatDataTotalLabel = new Label() { Text = "??" };

    private Button refreshButton = new Button()
    {
        Text = "Refresh"
    };


    public void OnColumnHeaderClick(object sender, Eto.Forms.GridColumnEventArgs e)
    {
        Sort(e.Column);
    }

    private GridColumn InstalledColumn = new GridColumn
    {
        Sortable = true,
        Expand = true,
        HeaderText = "Installed",
        DataCell = new TextBoxCell { Binding = Binding.Property<InstalledGame, string>(r => r.Installed) }
    };

    private GridColumn LocationColumn = new GridColumn
    {
        Sortable = true,
        Expand = true,
        HeaderText = "Location",
        DataCell = new TextBoxCell
        {
            Binding = Binding.Property<InstalledGame, string>(r => r.DataFolder)
        }
    };
    private GridColumn IdColumn = new GridColumn
    {
        Sortable = true,
        Expand = true,
        HeaderText = "AppId",
        DataCell = new TextBoxCell
        {
            // Binding = Binding.Property<InstalledGame, string>(r => r.Id)
            Binding = Binding.Property<InstalledGame, string>(r => r.Id.ToString()),

        }
    };

    private GridColumn NameColumn = new GridColumn
    {
        Sortable = true,
        Expand = true,
        HeaderText = "Name",
        DataCell = new TextBoxCell
        {
            Binding = Binding.Property<InstalledGame, string>(r => r.Name)
        }
    };
    private GridColumn CompactDataColumn = new GridColumn
    {
        Sortable = true,
        HeaderText = "CompatData (MB)",
        DataCell = new TextBoxCell
        {
            Binding = Binding.Property<InstalledGame, string>(r => r.CompatDataSize)
        }
    };
    private GridColumn ShaderCacheColumn = new GridColumn
    {
        Sortable = true,
        HeaderText = "ShaderCache (MB)",
        DataCell = new TextBoxCell
        {
            Binding = Binding.Property<InstalledGame, string>(r => r.ShaderCacheSize)
        }
    };

    private GridColumn TotalColumn = new GridColumn
    {
        Sortable = true,
        HeaderText = "Total (MB)",
        DataCell = new TextBoxCell
        {
            Binding = Binding.Property<InstalledGame, string>(r => r.TotalSize)
        }
    };

    public MainForm()
    {
        Application.Instance.UnhandledException += UnhandledException;

        Title = "Steam Deck Space View";
        MinimumSize = new Size(600, 600);

        grid.ColumnHeaderClick += OnColumnHeaderClick;
        grid.CellDoubleClick += OnCellDoubleClick;
        refreshButton.Click += OnRefreshClick;

        grid.Columns.Add(InstalledColumn);
        grid.Columns.Add(IdColumn);
        grid.Columns.Add(NameColumn);
        grid.Columns.Add(CompactDataColumn);
        grid.Columns.Add(ShaderCacheColumn);
        grid.Columns.Add(TotalColumn);
        grid.Columns.Add(LocationColumn);

        StackLayout labelStack = new StackLayout()
        {
            Orientation = Orientation.Vertical,
            Items =
                {
                    new StackLayoutItem(shaderCacheTotalLabel),
                    new StackLayoutItem(compatDataTotalLabel)
                }
        };

        StackLayout stack = new StackLayout(refreshButton, labelStack)
        {
            Orientation = Orientation.Horizontal,
            VerticalContentAlignment = VerticalAlignment.Center,
            Spacing = 5
        };

        Content = new TableLayout
        {
            Padding = new Padding(10),
            Spacing = new Size(5, 5),
            Rows =
                {
                    new TableRow(TableLayout.AutoSized(stack)),
                    new TableRow(grid)
                }
        };

        var quitCommand = new Command { MenuText = "Quit" };
        quitCommand.Executed += (sender, e) => Application.Instance.Quit();

        // create menu
        // Menu = new MenuBar
        // {
        //     Items =
        //         {
        // 			// File submenu
        // 			new SubMenuItem { Text = "&File", Items = { } },
        //         },
        //     ApplicationItems =
        //         {
        //             new ButtonMenuItem { Text = "&Preferences..." },
        //         },
        //     QuitItem = quitCommand
        // };
    }

    private void OnRefreshClick(object sender, EventArgs e)
    {
        Populate();
        MessageBox.Show("done");
    }

    private void UnhandledException(object sender, Eto.UnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.ToString(), "error", MessageBoxType.Error);
    }


    private void OnCellDoubleClick(object sender, GridCellMouseEventArgs e)
    {

        InstalledGame game = e.Item as InstalledGame;

        if (e.GridColumn == IdColumn)
        {
            Clipboard.Instance.Text = game.Id.ToString();
            MessageBox.Show($"{game.Id} copied to clipboard");
        }
        else if (e.GridColumn == CompactDataColumn)
        {
            if (game.CompatFolderPath != null)
            {
                ProcessStartInfo p = new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = $"\"{game.CompatFolderPath}\""
                };
                Process.Start(p);
            }
        }
        else if (e.GridColumn == ShaderCacheColumn)
        {
            if (game.ShaderCacheFolderPath != null)
            {


                ProcessStartInfo p = new ProcessStartInfo
                {
                    FileName = "xdg-open",
                    Arguments = $"\"{game.ShaderCacheFolderPath}\""
                };
                Process.Start(p);
            }

        }
    }
}