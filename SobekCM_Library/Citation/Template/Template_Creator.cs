#region Using directives

using System.IO;

#endregion

namespace SobekCM.Library.Citation.Template
{
    /// <summary> Can be used to generate a simple template XML configuration file for testing purposes  </summary>
    public class Template_Creator
    {
        /// <summary> Create the template XML configuration file </summary>
        /// <param name="directory"> Filename and directory for the configuration file </param>
        public static void Create(string directory)
        {
            StreamWriter writer = new StreamWriter(directory, false);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
            writer.WriteLine("");
            writer.WriteLine("<!-- Begin the XML for this input template -->");
            writer.WriteLine("<input_template>");
            writer.WriteLine("");
            writer.WriteLine("	<!-- Define the information about this input template -->");
            writer.WriteLine("	<name>Metadata CompleteTemplate</name>");
            writer.WriteLine("	<name language=\"spa\">Plantilla </name>");
            writer.WriteLine("	<name language=\"fre\">Calibre </name>");
            writer.WriteLine("	<notes>Demo template</notes>");
            writer.WriteLine("	<dateCreated>November 29, 2005</dateCreated>");
            writer.WriteLine("	<lastModified>November 9, 2006</lastModified>");
            writer.WriteLine("	<creator>Mark V Sullivan</creator>");
            writer.WriteLine("	");
            writer.WriteLine("	<!-- This defines the inputs which are available for the user -->");
            writer.WriteLine("	<inputs>");
            writer.WriteLine("		<page>");
            writer.WriteLine("			<name language=\"eng\">General Information</name>");
            writer.WriteLine("			<name language=\"fre\">Informations Générales</name>");
            writer.WriteLine("			<name language=\"spa\">Datos Generales</name>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name>Codes</name>");
            writer.WriteLine("				<element type=\"Bib\" mandatory=\"true\" />");
            writer.WriteLine("        			<element type=\"VID\" mandatory=\"true\" />");
            writer.WriteLine("				<element type=\"Collection_Primary\" mandatory=\"true\" />");
            writer.WriteLine("				<element type=\"Collection_Alternate\" repeatable=\"true\" />");
            writer.WriteLine("				<element type=\"Subcollection\" />");
            writer.WriteLine("				<element type=\"PALMM Code\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Main Panel</name>");
            writer.WriteLine("				<name language=\"spa\">Panel Principal</name>");
            writer.WriteLine("				<name language=\"fre\">Panneau Principal</name>");
            writer.WriteLine("");
            writer.WriteLine("				<element type=\"Language\" repeatable=\"true\">");
            writer.WriteLine("					<element_data>");
            writer.WriteLine("						<source_xml>\\Data\\language3way.xml</source_xml>");
            writer.WriteLine("					</element_data>");
            writer.WriteLine("				</element>");
            writer.WriteLine("				<element type=\"Type\" mandatory=\"true\" />");
            writer.WriteLine("				<element type=\"Format\" subtype=\"simple\" repeatable=\"false\" />");
            writer.WriteLine("				<element type=\"Identifier\" subtype=\"complex\" repeatable=\"true\" />");
            writer.WriteLine("				<element type=\"Source\" subtype=\"complex\">");
            writer.WriteLine("					<element_data>");
            writer.WriteLine("						<options>UF, FAMU, UF, FSU, UWF, UNF, UCF, USF, FIU, MHM, MCPL, FLNG, ROSSICA, CARICOM, ANH, AUF, BHPSE, BFIC, BNH, BNPHU, FUNGLODE, IFH, NLJ, PUCCMA, OAS, UOV, UVI, UNPHU, UWI, SWFLN, FSA, FAU, FGCU, UM, UWF, JU, CPL, FNG, UASD, NLG, BFIC </options>");
            writer.WriteLine("						<statement></statement>");
            writer.WriteLine("					</element_data>");
            writer.WriteLine("				</element>");
            writer.WriteLine("				<element type=\"Holding\" subtype=\"complex\">");
            writer.WriteLine("					<element_data>");
            writer.WriteLine("						<options>UF, FAMU, UF, FSU, UWF, UNF, UCF, USF, FIU, MHM, MCPL, FLNG, ROSSICA, CARICOM, ANH, AUF, BHPSE, BFIC, BNH, BNPHU, FUNGLODE, IFH, NLJ, PUCCMA, OAS, UOV, UVI, UNPHU, UWI, SWFLN, FSA, FAU, FGCU, UM, UWF, JU, CPL, FNG, UASD, NLG, BFIC </options>");
            writer.WriteLine("						<statement></statement>");
            writer.WriteLine("					</element_data>");
            writer.WriteLine("				</element>");
            writer.WriteLine("			</panel>");
            writer.WriteLine("		</page>");
            writer.WriteLine("		<page>");
            writer.WriteLine("			<name language=\"eng\">Source Document</name>");
            writer.WriteLine("			<name language=\"fre\">Document de Source</name>");
            writer.WriteLine("			<name language=\"spa\">Documento de Fuente</name>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Title Information</name>");
            writer.WriteLine("				<name language=\"spa\">Información Titular</name>");
            writer.WriteLine("				<name language=\"fre\">Information de Titre</name>");
            writer.WriteLine("				<element type=\"Title\" subtype=\"panel\" mandatory=\"true\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Name Authorities</name>");
            writer.WriteLine("				<name language=\"spa\">Autoridad de Nombre</name>");
            writer.WriteLine("				<name language=\"fre\">Autorités du Nom</name>");
            writer.WriteLine("				<element type=\"Creator\" subtype=\"complex\" repeatable=\"true\" />");
            writer.WriteLine("				<element type=\"Contributor\" subtype=\"complex\" repeatable=\"true\" />");
            writer.WriteLine("				<element type=\"Publisher\" subtype=\"complex\" />");
            writer.WriteLine("				<element type=\"Date\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("		</page>");
            writer.WriteLine("		<page>");
            writer.WriteLine("			<name language=\"eng\">Subjects and Keywords</name>");
            writer.WriteLine("			<name language=\"fre\">Sujets et Mots-Clés</name>");
            writer.WriteLine("			<name language=\"spa\">Temas y Palabaras Claves</name>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Subjects</name>");
            writer.WriteLine("				<name language=\"spa\">Sujetos</name>");
            writer.WriteLine("				<name language=\"fre\">Sujets</name>");
            writer.WriteLine("				<element type=\"Subject\" subtype=\"complex\" repeatable=\"true\" />");
            writer.WriteLine("				<element type=\"Temporal\" subtype=\"complex\" />");
            writer.WriteLine("				<element type=\"Spatial\" subtype=\"hierarchical\" repeatable=\"true\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Affiliation</name>");
            writer.WriteLine("				<name language=\"spa\">Affiliation</name>");
            writer.WriteLine("				<name language=\"fre\">Affiliation</name>");
            writer.WriteLine("				<element type=\"Affiliation\" subtype=\"hierarchical\" repeatable=\"true\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Notes</name>");
            writer.WriteLine("				<name language=\"spa\">Notas</name>");
            writer.WriteLine("				<name language=\"fre\">Notes</name>");
            writer.WriteLine("				<element type=\"Abstract\" subtype=\"complex\" repeatable=\"true\" />");
            writer.WriteLine("				<element type=\"Description\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("		</page>");
            writer.WriteLine("		<page>");
            writer.WriteLine("			<name language=\"eng\">Behaviors</name>");
            writer.WriteLine("			<name language=\"fre\">Behaviors</name>");
            writer.WriteLine("			<name language=\"spa\">Comportamiento</name>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Watermarks for web page</name>");
            writer.WriteLine("				<name language=\"spa\">Iconos para el Internet </name>");
            writer.WriteLine("				<name language=\"fre\">Icônes pour l'Internet</name>");
            writer.WriteLine("				<element type=\"Icon\" repeatable=\"true\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Other Files</name>");
            writer.WriteLine("				<name language=\"spa\">Otros Archivos</name>");
            writer.WriteLine("				<name language=\"fre\">D'autres Dossiers</name>");
            writer.WriteLine("				<element type=\"Download\" subtype=\"select\" repeatable=\"false\" />");
            writer.WriteLine("				<element type=\"Thumbnail\" subtype=\"select\" repeatable=\"false\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name>Viewers for this resource</name>");
            writer.WriteLine("				<element type=\"Viewer\"  repeatable=\"true\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name>Skins for this resource</name>");
            writer.WriteLine("				<element type=\"Interface\" repeatable=\"true\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("		</page>");
            writer.WriteLine("		<page>");
            writer.WriteLine("				<name language=\"eng\">Serial</name>");
            writer.WriteLine("				<name language=\"spa\">Serial</name>");
            writer.WriteLine("				<name language=\"fre\">Périodique</name>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Serial Hierarchy</name>");
            writer.WriteLine("				<name language=\"spa\">Jerarquía Serial</name>");
            writer.WriteLine("				<name language=\"fre\">Hiérarchie Périodique</name>");
            writer.WriteLine("				<element type=\"SerialHierarchy\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("		</page>");
            writer.WriteLine("		<page>");
            writer.WriteLine("				<name language=\"eng\">TOC</name>");
            writer.WriteLine("				<name language=\"spa\">Indice</name>");
            writer.WriteLine("				<name language=\"fre\">Table des Matières</name>");
            writer.WriteLine("			<panel>");
            writer.WriteLine("				<name language=\"eng\">Structure Map</name>");
            writer.WriteLine("				<name language=\"spa\">Mapa de Estructura</name>");
            writer.WriteLine("				<name language=\"fre\">Carte de Structure</name>");
            writer.WriteLine("				<element type=\"Structure Map\" />");
            writer.WriteLine("			</panel>");
            writer.WriteLine("		</page>");
            writer.WriteLine("	</inputs>");
            writer.WriteLine("");
            writer.WriteLine("</input_template>");
            writer.WriteLine("<!-- End of input template XML -->");
            writer.Flush();
            writer.Close();
        }
    }
}
