using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Management_Tool.Importer
{
    public partial class Column_Assignment_Control : UserControl
    {
        private bool empty;

        protected static string[] mappable_fields;

        static Column_Assignment_Control()
        {
            Array enumValues = Enum.GetValues(typeof(Resource_Object.Mapped_Fields));
            mappable_fields = new string[enumValues.Length];
            for (int i = 0; i < enumValues.Length; i++)
            {
                mappable_fields[i] = SobekCM.Resource_Object.Bibliographic_Mapping.Mapped_Field_To_String((Resource_Object.Mapped_Fields)enumValues.GetValue(i));
            }
        }

        public Column_Assignment_Control()
        {
            InitializeComponent();

            this.comboBox1.Items.AddRange(mappable_fields);
                      
            empty = false;
        }

        public bool Empty
        {
            get
            {
                return empty;
            }
            set
            {
                empty = value;
                if (empty)
                {
                    this.textBox1.ReadOnly = false;
                    this.textBox1.Text = "{Empty}";
                }
                else
                {
                    this.textBox1.ReadOnly = true;
                }
            }
        }

        public string Column_Name
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public string Mapped_Name
        {
            get { return comboBox1.Text; }
        }

        public bool hasItemInList( string item )
        {
            return comboBox1.Items.Contains( item );
        }

        public void Select_List_Item(string columnName)
        {
            SobekCM.Resource_Object.Mapped_Fields mappedField = SobekCM.Resource_Object.Bibliographic_Mapping.String_To_Mapped_Field(columnName);
            string listValue = SobekCM.Resource_Object.Bibliographic_Mapping.Mapped_Field_To_String(mappedField);
                this.comboBox1.SelectedIndex = this.comboBox1.FindStringExact(listValue);           
        }

        public SobekCM.Resource_Object.Mapped_Fields Mapped_Field
        {
            get
            {
                return SobekCM.Resource_Object.Bibliographic_Mapping.String_To_Mapped_Field(comboBox1.Text);
            }
        }
    }
}
