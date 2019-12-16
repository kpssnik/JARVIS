using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Data.SQLite;

namespace kpssJarvis
{
    public partial class SettingsForm : MetroForm
    {
        public static List<MetroTextBox> textBoxes;

        static SQLiteConnection connection;
        static SQLiteCommand command;

        static string dbPath = Form1.dbPath;
        static string progTable = Form1.progTable;
        static string cmdTable = Form1.cmdTable;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxes = new List<MetroTextBox>()
            {
                open_prog_textBox,
                close_prog_textBox,
                start_listening_textBox,
                stop_listening_textBox,
                hide_tray_textBox,
                show_tray_textBox,
                shutdown_jarvis_textBox,
                shutdown_pc_textBox
            };
            connection = new SQLiteConnection($"Data source={dbPath}");

            InitCommandTextBoxes();
            InitDataGrid();
        }

        private void InitCommandTextBoxes()
        {
            connection.Open();
            DataSet ds = new DataSet();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter($"SELECT * FROM {cmdTable}", connection);
            adapter.Fill(ds);

            for (int i = 0; i < textBoxes.Count; i++) textBoxes[i].Text = ds.Tables[0].Rows[0][i].ToString().Trim();

            connection.Close();
        }

        private void InitDataGrid()
        {
            connection.Open();
            DataSet ds = new DataSet();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter($"SELECT * FROM {progTable}", connection);
            adapter.Fill(ds);

            for (int i = 1; i < ds.Tables[0].Columns.Count; i++)
            {
                var column1 = new DataGridViewColumn();
                column1.HeaderText = ds.Tables[0].Columns[i].ColumnName;
                column1.Width = dataGridView1.Width / 2 - 20;
                column1.ReadOnly = false;
                column1.Name = $"{ds.Tables[0].Columns[i].ColumnName}_column";
                column1.Frozen = true;
                column1.CellTemplate = new DataGridViewTextBoxCell();

                dataGridView1.Columns.Add(column1);
            }

            if (dataGridView1.Rows.Count > 0) dataGridView1.Rows.Clear();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                dataGridView1.Rows.Add(row["key"].ToString(), row["value"].ToString());
            }

            connection.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            connection.Open();

            foreach (var tb in textBoxes)
            {
                command = new SQLiteCommand($"UPDATE {cmdTable} SET {tb.Name.Replace("_textBox", "")}='{tb.Text.ToLower()}'", connection);
                command.ExecuteNonQuery();
            }
            connection.Close();

            MessageBox.Show("Применено! Перезапустите приложение для изменения словаря.");
        }

        private void DelGridButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow del = dataGridView1.SelectedCells[0].OwningRow;
                dataGridView1.Rows.Remove(del);
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно удалить строку добавления");
            }
        }

        private void ApplyGridButton_Click(object sender, EventArgs e)
        {
            connection.Open();

            command = new SQLiteCommand($"DELETE FROM {progTable}", connection);
            command.ExecuteNonQuery();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                command = new SQLiteCommand($"INSERT INTO {progTable}(key, value) " +
                $"VALUES('{row.Cells[0].Value}', '{row.Cells[1].Value}')", connection);

                command.ExecuteNonQuery();
            }

            command = new SQLiteCommand($"DELETE FROM {progTable} WHERE key=''", connection);
            command.ExecuteNonQuery();

            connection.Close();

            MessageBox.Show("Применено! Перезапустите приложение для изменения словаря.");
        }
    }
}
