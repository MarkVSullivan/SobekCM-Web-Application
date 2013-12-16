using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Configuration
{
    public partial class SobekCM_Builder_Folders_Form : Form
    {
        private DataTable folderTable;

        public SobekCM_Builder_Folders_Form()
        {
            InitializeComponent();

            folderTable = Database.SobekCM_Database.Get_Builder_Incoming_Folders();

            // Now, add these to the table
            foreach (DataRow thisRow in folderTable.Rows)
            {
                ListViewItem newListItem = new ListViewItem(new string[] { thisRow["FolderName"].ToString(), thisRow["NetworkFolder"].ToString() });
                newListItem.Tag = thisRow;
                listView1.Items.Add(newListItem);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DataRow sourceRow = (DataRow)listView1.SelectedItems[0].Tag;

                SobekCM_Builder_Incoming_Folder_Form newForm = new SobekCM_Builder_Incoming_Folder_Form(sourceRow);
                this.Hide();
                DialogResult result = newForm.ShowDialog();
                this.Show();

                if (result == DialogResult.OK)
                {
                    folderTable = Database.SobekCM_Database.Get_Builder_Incoming_Folders();

                    // Now, add these to the table
                    listView1.Items.Clear();
                    foreach (DataRow thisRow in folderTable.Rows)
                    {
                        ListViewItem newListItem = new ListViewItem(new string[] { thisRow["FolderName"].ToString(), thisRow["NetworkFolder"].ToString() });
                        newListItem.Tag = thisRow;
                        listView1.Items.Add(newListItem);
                    }

                    deleteButton.Button_Enabled = false;
                    editButton.Button_Enabled = false;
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                editButton.Button_Enabled = true;
                deleteButton.Button_Enabled = true;
            }
            else
            {
                editButton.Button_Enabled = false;
                deleteButton.Button_Enabled = false;
            }
        }

        private void closeButton_Button_Pressed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editButton_Button_Pressed(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DataRow sourceRow = (DataRow)listView1.SelectedItems[0].Tag;

                SobekCM_Builder_Incoming_Folder_Form newForm = new SobekCM_Builder_Incoming_Folder_Form(sourceRow);
                this.Hide();
                DialogResult result = newForm.ShowDialog();
                this.Show();

                if (result == DialogResult.OK)
                {
                    folderTable = Database.SobekCM_Database.Get_Builder_Incoming_Folders();

                    // Now, add these to the table
                    listView1.Items.Clear();
                    foreach (DataRow thisRow in folderTable.Rows)
                    {
                        ListViewItem newListItem = new ListViewItem(new string[] { thisRow["FolderName"].ToString(), thisRow["NetworkFolder"].ToString() });
                        newListItem.Tag = thisRow;
                        listView1.Items.Add(newListItem);
                    }

                    deleteButton.Button_Enabled = false;
                    editButton.Button_Enabled = false;
                }
            }
        }

        private void deleteButton_Button_Pressed(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                DataRow sourceRow = (DataRow)listView1.SelectedItems[0].Tag;

                int id = Convert.ToInt32(sourceRow["IncomingFolderId"]);
                string name = sourceRow["FolderName"].ToString();

                DialogResult result = MessageBox.Show("Are you sure you want to delete '" + name + "'?      ", "Delete Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;


                if (!Database.SobekCM_Database.Delete_Builder_Incoming_Folder(id))
                    MessageBox.Show("Error encountered during the delete!    ");

                folderTable = Database.SobekCM_Database.Get_Builder_Incoming_Folders();

                // Now, add these to the table
                listView1.Items.Clear();
                foreach (DataRow thisRow in folderTable.Rows)
                {
                    ListViewItem newListItem = new ListViewItem(new string[] { thisRow["FolderName"].ToString(), thisRow["NetworkFolder"].ToString() });
                    newListItem.Tag = thisRow;
                    listView1.Items.Add(newListItem);
                }

                deleteButton.Button_Enabled = false;
                editButton.Button_Enabled = false;

            }
        }

        private void newButton_Button_Pressed(object sender, EventArgs e)
        {
            DataRow newRow = folderTable.NewRow();

            SobekCM_Builder_Incoming_Folder_Form newForm = new SobekCM_Builder_Incoming_Folder_Form(newRow);
            this.Hide();
            DialogResult result = newForm.ShowDialog();
            this.Show();

            if (result == DialogResult.OK)
            {
                folderTable = Database.SobekCM_Database.Get_Builder_Incoming_Folders();

                // Now, add these to the table
                listView1.Items.Clear();
                foreach (DataRow thisRow in folderTable.Rows)
                {
                    ListViewItem newListItem = new ListViewItem(new string[] { thisRow["FolderName"].ToString(), thisRow["NetworkFolder"].ToString() });
                    newListItem.Tag = thisRow;
                    listView1.Items.Add(newListItem);
                }

                deleteButton.Button_Enabled = false;
                editButton.Button_Enabled = false;
            }
        }
    }
}
