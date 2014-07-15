using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace SobekCM.Management_Tool.Importer
{
	public partial class Constant_Assignment_Control : UserControl
	{
		private string list_value;

		protected static string[] mappable_fields;

		static Constant_Assignment_Control()
		{
            Array enumValues = Enum.GetValues(typeof(Resource_Object.Mapped_Fields));
			mappable_fields = new string[enumValues.Length];
			for (int i = 0; i < enumValues.Length; i++)
			{
                mappable_fields[i] = SobekCM.Resource_Object.Bibliographic_Mapping.Mapped_Field_To_String((Resource_Object.Mapped_Fields)enumValues.GetValue(i));
			}
		}

		private static DataTable aggregationTable;
		public static void Set_Aggregation_Table(DataTable Aggregation_Table)
		{
			aggregationTable = Aggregation_Table;
		}
					  
		public Constant_Assignment_Control()
		{
			InitializeComponent();
			this.cboMappedField.Items.AddRange(mappable_fields);           
		}

		public string Mapped_Name
		{
			get { return cboMappedField.Text; }
			set { cboMappedField.Text = value; }
		}

		public string Mapped_Constant
		{
			get {

				try
				{

                    if ((Mapped_Field == SobekCM.Resource_Object.Mapped_Fields.Source_Code) &&
							(cboMappedConstant.Text.Length > 0))
					{
						list_value = cboMappedConstant.Text.Substring(0, cboMappedConstant.Text.IndexOf(' '));

						return list_value;
					}                    
					else if ((Mapped_Field.ToString() == "None") &&
					   (cboMappedConstant.SelectedIndex == 0))
					{                        
						cboMappedConstant.SelectedIndex = -1;                       
						return String.Empty;
					}
					else
					{
						return cboMappedConstant.Text;                        
					}
				}
				catch (Exception)
				{
					return String.Empty;
				}       
			}
			set
			{
				cboMappedConstant.Text = value;
			}
		}

		public bool hasItemInList( string item )
		{
			return cboMappedField.Items.Contains( item );
		}

        public SobekCM.Resource_Object.Mapped_Fields Mapped_Field
		{
			get
			{
                return SobekCM.Resource_Object.Bibliographic_Mapping.String_To_Mapped_Field(cboMappedField.Text);
			}
		}

		private void cboMappedField_SelectedIndexChanged(object sender, EventArgs e)
		{
			cboMappedConstant.Items.Clear();
			cboMappedConstant.DropDownStyle = ComboBoxStyle.Simple;

			int index = 0;
			switch (cboMappedField.Text)
			{
				case "None":
					cboMappedConstant.SelectedIndex = -1;
					cboMappedConstant.Text = String.Empty;
					cboMappedConstant.Enabled = false;
					break;               

				case "Source Institution Code":
					// Populate all the source institutions
					foreach (DataRow thisRow in aggregationTable.Rows)
					{
						if (thisRow["Type"].ToString().ToUpper().IndexOf("INSTITUT") == 0)
						{
							this.cboMappedConstant.Items.Add(thisRow["Code"].ToString());
						}
					}

					cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;
					cboMappedConstant.Enabled = true;

					// select default value
					index = cboMappedConstant.FindString("UF");

					if (index > 0)
						cboMappedConstant.SelectedIndex = index;
					break;   
	  
				case "Holding Location Code":
					// Populate all the holding institutions
					foreach (DataRow thisRow in aggregationTable.Rows)
					{
						if (thisRow["Type"].ToString().ToUpper().IndexOf("INSTITUT") == 0)
						{
							this.cboMappedConstant.Items.Add(thisRow["Code"].ToString());
						}
					}

					cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;
					cboMappedConstant.Enabled = true;

					// select default value
					index = cboMappedConstant.FindString("UF");

					if (index > 0)
						cboMappedConstant.SelectedIndex = index;
					break;                              

				case "Material Type":
					this.cboMappedConstant.Items.Add("Aerial");
					this.cboMappedConstant.Items.Add("Archival");
					this.cboMappedConstant.Items.Add("Artifact");
					this.cboMappedConstant.Items.Add("Audio");
					this.cboMappedConstant.Items.Add("Book");
					this.cboMappedConstant.Items.Add("Map");
					this.cboMappedConstant.Items.Add("Newspaper");
					this.cboMappedConstant.Items.Add("Photograph");
					this.cboMappedConstant.Items.Add("Serial");
					this.cboMappedConstant.Items.Add("Video");
					cboMappedConstant.SelectedIndex = -1;

					cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;
					cboMappedConstant.Enabled = true;                
					break;

				case "Aggregation Code":
					// Populate all the project codes
					foreach (DataRow thisRow in aggregationTable.Rows)
					{
						if (thisRow["Type"].ToString().ToUpper().IndexOf("INSTITUT") < 0)
						{
							this.cboMappedConstant.Items.Add(thisRow["Code"].ToString());
						}
					}
					cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;
					cboMappedConstant.Enabled = true;             
					break;

				case "Visibility":
					this.cboMappedConstant.Items.Add("DARK");
					this.cboMappedConstant.Items.Add("PRIVATE");
					this.cboMappedConstant.Items.Add("RESTRICTED");
					this.cboMappedConstant.Items.Add("PUBLIC");
					cboMappedConstant.SelectedIndex = 1;

					cboMappedConstant.DropDownStyle = ComboBoxStyle.DropDownList;
					cboMappedConstant.Enabled = true;
					break;
				
				default:
					cboMappedConstant.Enabled = true;          
					break;
			}

		}     
	}
}
