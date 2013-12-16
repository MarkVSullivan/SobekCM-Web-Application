<!-- EAD Cookbook Style 1      Version 0.92   2002 March 19  -->

<!--  This stylesheet generates Style 1 which has a Table of Contents inline at the beginning of the finding aid in the manner of book.  -->


<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:strip-space elements="*"/>

<!-- Creates the body of the finding aid.-->
<xsl:template match="/">
<xsl:variable name="file">
<xsl:value-of select="ead/eadheader/eadid"/>
</xsl:variable>

<xsl:call-template name="body"/>
</xsl:template>


<xsl:template name="body">
<xsl:variable name="file">
<xsl:value-of select="ead/eadheader/eadid"/>
</xsl:variable>

<xsl:call-template name="eadheader"/>
<xsl:call-template name="copyright"/>
<xsl:call-template name="archdesc-did"/>
<xsl:call-template name="archdesc-bioghist"/>
<xsl:call-template name="archdesc-scopecontent"/>
<xsl:call-template name="archdesc-organization"/>
<xsl:call-template name="archdesc-arrangement"/>
<xsl:call-template name="archdesc-restrict"/>
<xsl:call-template name="archdesc-relatedmaterial"/>
<xsl:call-template name="archdesc-admininfo"/>
<xsl:call-template name="archdesc-otherfindaid"/>
<xsl:call-template name="archdesc-odd"/>
<xsl:call-template name="archdesc-index"/>
<xsl:call-template name="archdesc-bibliography"/>
<xsl:call-template name="archdesc-control"/>
</xsl:template>

<xsl:template name="eadheader">
<xsl:for-each select="ead/eadheader/filedesc/titlestmt">

<h3 align="center">
<xsl:value-of select="titleproper"/>
</h3>
<h4 align="center">
<xsl:value-of select="subtitle"/>
</h4>
<p align="center"><strong>
<xsl:value-of select="author"/>
</strong></p>
</xsl:for-each>

</xsl:template>

<xsl:template name="copyright">
<xsl:for-each select="ead/eadheader/filedesc/publicationstmt">
<p align="center"><strong>University of Florida Smathers Libraries - <a href="http://www.uflib.ufl.edu/spec/">Special and Area Studies Collections</a>
<br></br>
<xsl:value-of select="date"/>
</strong></p>
<br></br>
</xsl:for-each>
</xsl:template>

<!-- The following templates format the display of various RENDER attributes.-->

<xsl:template match="emph[@render='bold']">
	<strong>
		<xsl:apply-templates/>
	</strong>
</xsl:template>

<xsl:template match="emph[@render='italic']">
	<i>
		<xsl:apply-templates/>
	</i>
</xsl:template>

<xsl:template match="emph[@render='underline']">
	<u>
		<xsl:apply-templates/>
	</u>
</xsl:template>

<xsl:template match="emph[@render='sub']">
	<sub>
		<xsl:apply-templates/>
	</sub>
</xsl:template>

<xsl:template match="emph[@render='super']">
	<super>
		<xsl:apply-templates/>
	</super>
</xsl:template>

<xsl:template match="emph[@render='quoted']">
	<xsl:text>"</xsl:text>
		<xsl:apply-templates/>
	<xsl:text>"</xsl:text>
</xsl:template>

<xsl:template match="emph[@render='boldquoted']">
	<strong>
		<xsl:text>"</xsl:text>
			<xsl:apply-templates/>
		<xsl:text>"</xsl:text>
	</strong>
</xsl:template>

<xsl:template match="emph[@render='boldunderline']">
	<strong>
		<u>
			<xsl:apply-templates/>
		</u>
	</strong>
</xsl:template>

<xsl:template match="emph[@render='bolditalic']">
	<strong>
		<i>
			<xsl:apply-templates/>
		</i>
	</strong>
</xsl:template>

<xsl:template match="emph[@render='boldsmcaps']">
	<font style="font-variant: small-caps">
		<strong>
			<xsl:apply-templates/>
		</strong>
	</font>
</xsl:template>

<xsl:template match="emph[@render='smcaps']">
	<font style="font-variant: small-caps">
		<xsl:apply-templates/>
	</font>
</xsl:template>

<xsl:template match="title[@render='bold']">
	<strong>
		<xsl:apply-templates/>
	</strong>
</xsl:template>

<xsl:template match="title[@render='italic']">
	<i>
		<xsl:apply-templates/>
	</i>
</xsl:template>

<xsl:template match="title[@render='underline']">
	<u>
		<xsl:apply-templates/>
	</u>
</xsl:template>

<xsl:template match="title[@render='sub']">
	<sub>
		<xsl:apply-templates/>
	</sub>
</xsl:template>

<xsl:template match="title[@render='super']">
	<super>
		<xsl:apply-templates/>
	</super>
</xsl:template>

<xsl:template match="title[@render='quoted']">
	<xsl:text>"</xsl:text>
		<xsl:apply-templates/>
	<xsl:text>"</xsl:text>
</xsl:template>

<xsl:template match="title[@render='boldquoted']">
	<strong>
		<xsl:text>"</xsl:text>
			<xsl:apply-templates/>
		<xsl:text>"</xsl:text>
	</strong>
</xsl:template>

<xsl:template match="title[@render='boldunderline']">
	<strong>
		<u>
			<xsl:apply-templates/>
		</u>
	</strong>
</xsl:template>

<xsl:template match="title[@render='bolditalic']">
	<strong>
		<i>
			<xsl:apply-templates/>
		</i>
	</strong>
</xsl:template>

<xsl:template match="title[@render='boldsmcaps']">
	<font style="font-variant: small-caps">
		<strong>
			<xsl:apply-templates/>
		</strong>
	</font>
</xsl:template>

<xsl:template match="title[@render='smcaps']">
	<font style="font-variant: small-caps">
		<xsl:apply-templates/>
	</font>
</xsl:template>

<!-- This template converts a extref element into an HREF link.-->

<xsl:template match="extref">
<a><xsl:attribute name="href"><xsl:value-of select="@href"/></xsl:attribute><xsl:apply-templates/></a>
</xsl:template>

<!-- This template converts a dao element into an HREF link.-->


<xsl:template match="dao">
	<xsl:text>  </xsl:text>
	<a href="{@href}"><xsl:value-of select="@title"/></a>
</xsl:template>

<!-- This template converts a Ref element into an HTML anchor.-->

<xsl:template match="ead/archdesc//ref">
<xsl:variable name="target">
<xsl:value-of select="@target"/>
</xsl:variable>
<a href="#{$target}">
<xsl:value-of select="."/>
</a>
</xsl:template>

<!-- Converts an ID attribute into the name attribute of an HTML anchor to form the target of a Ref element.-->
  
<xsl:template match="*[@id]">
<a name="{@id}">
<xsl:value-of select="."/>
</a>
</xsl:template>


<!--This template rule formats a list element.-->
<xsl:template match="*/list">
<xsl:for-each select="item">
<p style="margin-left: 60pt">
<xsl:apply-templates/>
</p>
</xsl:for-each>
</xsl:template>

<!--Formats a simple table. The width of each column is defined by the colwidth attribute in a colspec element.-->
<xsl:template match="*/table">
<xsl:for-each select="tgroup">
<table width="100%">
<tr>
<xsl:for-each select="colspec">
<td width="{@colwidth}"></td>
</xsl:for-each>
</tr>
<xsl:for-each select="thead">
<xsl:for-each select="row">
<tr>
<xsl:for-each select="entry">
<td valign="top"><strong><xsl:value-of select="."/></strong>
</td>
</xsl:for-each>
</tr>
</xsl:for-each>
</xsl:for-each>

<xsl:for-each select="tbody">
<xsl:for-each select="row">
<tr>
<xsl:for-each select="entry">
<td valign="top"><xsl:value-of select="."/></td>
</xsl:for-each>
</tr>
</xsl:for-each>
</xsl:for-each>
</table>
</xsl:for-each>
</xsl:template>



<!--This template rule formats the top-level did element.-->
<xsl:template name="archdesc-did">
<xsl:variable name="file">
<xsl:value-of select="ead/eadheader/eadid"/>
</xsl:variable>


<!--For each element of the did, this template inserts the value of the LABEL attribute or, if none is present, a default value.-->

<xsl:for-each select="ead/archdesc/did">
<table width="100%">
<tr><td width="5%"> </td><td width="20%"> </td>
<td width="75"> </td></tr>
<tr><td colspan="3"><h3><a name="did">
<xsl:apply-templates select="head"/>
</a></h3> </td></tr>

<xsl:if test="origination[string-length(text()|*)!=0]">
<xsl:for-each select="origination">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top"><strong>
<xsl:value-of select="@label"/>
</strong></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:when>
<xsl:otherwise>
<tr><td> </td><td valign="top">
<strong><xsl:text>Creator: </xsl:text></strong></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:otherwise>
</xsl:choose>
</xsl:for-each>
</xsl:if>

<!-- Tests for and processes various permutations of unittitle and unitdate.-->
<xsl:for-each select="unittitle">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top"><strong>
<xsl:value-of select="@label"/>
</strong></td><td>
<xsl:apply-templates select="text() |* [not(self::unitdate)]"/>
</td></tr>
</xsl:when>
<xsl:otherwise>
<tr><td> </td><td valign="top"><strong>
<xsl:text>Title: </xsl:text>
</strong></td><td>
<xsl:apply-templates select="text() |* [not(self::unitdate)]"/>
</td></tr>
</xsl:otherwise>
</xsl:choose>

<xsl:if test="child::unitdate">
<xsl:choose>
<xsl:when test="./unitdate/@label">
<tr><td> </td><td valign="top">
<strong>
<xsl:value-of select="./unitdate/@label"/>
</strong></td><td>
<xsl:apply-templates select="./unitdate"/>
</td></tr>
</xsl:when>
<xsl:otherwise>
<xsl:if test="@type='bulk'">
<tr><td> </td><td valign="top">
<strong>
<xsl:text>Bulk dates: </xsl:text>
</strong></td><td>
<xsl:apply-templates select="./unitdate"/>
</td></tr>
</xsl:if>
<xsl:if test="not(@type='bulk')">
<tr><td> </td><td valign="top">
<strong>
<xsl:text>Dates: </xsl:text>
</strong></td><td>
<xsl:apply-templates select="./unitdate"/>
</td></tr>
</xsl:if>
</xsl:otherwise>
</xsl:choose>
</xsl:if>
</xsl:for-each>

<!-- Processes the unit date if it is not a child of unit title but a child of did, the current context.-->
<xsl:if test="unitdate">
<xsl:for-each select="unitdate[string-length(text()|*)!=0]">
<xsl:choose>
<xsl:when test="./@label">
<tr><td> </td><td valign="top">
<strong>
<xsl:value-of select="./@label"/>
</strong></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:when>
<xsl:otherwise>
<xsl:if test="@type='bulk'">
<tr><td> </td><td valign="top">
<strong>
<xsl:text>Bulk dates: </xsl:text>
</strong></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:if>
<xsl:if test="not(@type='bulk')">
<tr><td> </td><td valign="top">
<strong>
<xsl:text>Dates: </xsl:text>
</strong></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:if>

</xsl:otherwise>
</xsl:choose>
</xsl:for-each>
</xsl:if>



<xsl:if test="abstract[string-length(text()|*)!=0]">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top">
<strong><xsl:value-of select="@label"/>
</strong></td><td>
<xsl:apply-templates select="abstract"/>
</td></tr>
</xsl:when>
<xsl:otherwise>
<tr><td> </td><td valign="top">
<strong><xsl:text>Abstract: </xsl:text></strong></td><td>
<xsl:apply-templates select="abstract"/>
</td></tr>
</xsl:otherwise>
</xsl:choose>
</xsl:if>

<xsl:if test="physdesc[string-length(text()|*)!=0]">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top">
<strong><xsl:value-of select="@label"/>
</strong></td><td>
<xsl:for-each select="physdesc/extent">
<xsl:apply-templates select="."/><xsl:text> </xsl:text>
</xsl:for-each>
</td></tr>
</xsl:when>

<xsl:otherwise>
<tr><td> </td><td valign="top">
<strong><xsl:text>Extent: </xsl:text></strong></td><td>
<xsl:for-each select="physdesc/extent">
<xsl:apply-templates select="."/><xsl:text> </xsl:text>
</xsl:for-each>
</td></tr>
</xsl:otherwise>
</xsl:choose>
</xsl:if>

<xsl:if test="unitid[string-length(text()|*)!=0]">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top">
<strong><xsl:value-of select="@label"/>
</strong></td><td>
<xsl:apply-templates select="unitid"/>
</td></tr>
</xsl:when>

<xsl:otherwise>
<tr><td> </td><td valign="top">
<strong><xsl:text>Identification: </xsl:text></strong></td><td>
<xsl:apply-templates select="unitid"/>

</td></tr>
</xsl:otherwise>
</xsl:choose>
</xsl:if>

<xsl:if test="physloc[string-length(text()|*)!=0]">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top">
<strong><xsl:value-of select="@label"/>
</strong></td><td>
<xsl:apply-templates select="physloc"/>
</td></tr>
</xsl:when>

<xsl:otherwise>
<tr><td> </td><td valign="top">
<strong><xsl:text>Location: </xsl:text></strong></td><td>
<xsl:apply-templates select="physloc"/>
</td></tr>
</xsl:otherwise>
</xsl:choose>
</xsl:if>

<xsl:if test="note[string-length(text()|*)!=0]">
<xsl:for-each select="note">
<xsl:choose>
<xsl:when test="@label">
<tr><td> </td><td valign="top">
<strong><xsl:value-of select="@label"/>
</strong></td></tr>
<xsl:for-each select="p">
<tr><td> </td><td valign="top">
<xsl:apply-templates/>
</td></tr>
</xsl:for-each>
</xsl:when>

<xsl:otherwise>
<tr><td> </td><td valign="top">
<strong>Location:</strong></td><td>
<xsl:apply-templates select="note"/>
</td></tr>
</xsl:otherwise>
</xsl:choose>
</xsl:for-each>
</xsl:if>
</table>

<p></p>
<hr size="1"></hr>

</xsl:for-each>
</xsl:template>

<!--This template rule formats the top-level bioghist element.-->
<xsl:template name="archdesc-bioghist">
<xsl:variable name="file">
<xsl:value-of select="ead/eadheader/eadid"/>
</xsl:variable>

<xsl:if test="ead/archdesc/bioghist[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/bioghist">
<xsl:apply-templates/>

<hr size="1"></hr>
</xsl:for-each>
</xsl:if>
</xsl:template>

<xsl:template match="ead/archdesc/bioghist/head">
<h3><a name="bioghist">
<xsl:apply-templates/>
</a></h3>
</xsl:template>

<xsl:template match="ead/archdesc/bioghist/p">
<p style="margin-left: 30pt">
<xsl:apply-templates/>
</p>
</xsl:template>

<xsl:template match="ead/archdesc/bioghist/chronlist">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="ead/archdesc/bioghist/bioghist">
<h3>
<xsl:apply-templates select="head"/>
</h3>
<xsl:for-each select="p">
<p style="margin-left: 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:template>

<!--This template rule formats a chronlist element.-->
<xsl:template match="*/chronlist">
<table width="100%">
<tr><td width="5%"> </td><td width="15%"> </td>
<td width="80%"> </td></tr>

<xsl:for-each select="listhead">
<tr><td><strong>
<xsl:apply-templates select="head01"/>
</strong></td>
<td><strong>
<xsl:apply-templates select="head02"/>
</strong></td></tr>
</xsl:for-each>

<xsl:for-each select="chronitem">
<tr><td></td><td valign="top">
<xsl:apply-templates select="date"/>
</td>
<td valign="top">
<xsl:choose>
<xsl:when test="eventgrp">
<xsl:apply-templates select="eventgrp/event"/>
</xsl:when>
<xsl:otherwise>					
<xsl:apply-templates select="event"/>
</xsl:otherwise>
</xsl:choose>
</td></tr>
</xsl:for-each>
</table>
</xsl:template>

<xsl:template match="eventgrp/event">
<xsl:apply-templates/><BR/>
</xsl:template>

	<xsl:template match="chronitem/event">
		<xsl:apply-templates/>
	</xsl:template>

<!--This template rule formats the scopecontent element.-->
<xsl:template name="archdesc-scopecontent">
<xsl:if test="ead/archdesc/scopecontent[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/scopecontent"> 
<xsl:apply-templates/>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<xsl:template match="ead/archdesc/scopecontent/head">
<h3><a name="scopecontent">
<xsl:apply-templates/>
</a></h3>
</xsl:template>

<!-- This formats an organization list embedded in a scope content statement.-->
<xsl:template match="ead/archdesc/scopecontent/organization">
<xsl:for-each select="p">
<p style="margin-left: 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
<xsl:for-each select="list">
<xsl:for-each select="item">
<p style="margin-left: 60pt">
<a><xsl:attribute name="href">#series<xsl:number/>
</xsl:attribute>
<xsl:apply-templates select="."/>
</a>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:template>

<xsl:template match="ead/archdesc/scopecontent/p">
<p style="margin-left: 30pt">
<xsl:apply-templates/>
</p>
</xsl:template>

<xsl:template match="ead/archdesc/scopecontent/arrangement/p">
<p style="margin-left: 30pt">
<xsl:apply-templates/>
</p>
</xsl:template>



<!--This template rule formats the organization element.-->
<xsl:template name="archdesc-organization">
<xsl:if test="ead/archdesc/organization[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/organization">
<table width="100%">
<tr><td width="5%"> </td><td width="5%"> </td>
<td width="90%"> </td></tr>

<tr><td colspan="3"> <h3><a name="organization">
<xsl:apply-templates select="head"/>
</a></h3>
</td></tr>

<xsl:for-each select="p">
<tr><td> </td><td colspan="2">
<xsl:apply-templates select="."/>
</td></tr>
</xsl:for-each>

<xsl:for-each select="list">
<tr><td> </td><td colspan="2">
<xsl:apply-templates select="head"/>
</td></tr>
<xsl:for-each select="item">
<tr><td> </td><td> </td><td colspan="1">
<a><xsl:attribute name="href">#series<xsl:number/>
</xsl:attribute>
<xsl:apply-templates select="."/>
</a>
</td></tr>
</xsl:for-each>
</xsl:for-each>
</table>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<!--This template rule formats the top-level arrangement element.-->
<xsl:template name="archdesc-arrangement">
<xsl:if test="ead/archdesc/arrangement[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/arrangement">
<h3><a name="arrangement">
<strong><xsl:apply-templates select="head"/>
</strong></a></h3>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<!--This template rule formats the top-level relatedmaterial element.-->
<xsl:template name="archdesc-relatedmaterial">
<xsl:if test="ead/archdesc/relatedmaterial[string-length(text()|*)!=0] | ead/archdesc/separatedmaterial[string-length(text()|*)!=0]">
<h3><a name="relatedmaterial">
<strong><xsl:text>Related or Separated Material</xsl:text>
</strong></a></h3>
<xsl:for-each select="ead/archdesc/relatedmaterial | ead/archdesc/separatedmaterial">
<xsl:apply-templates select="*[not(self::head)]"/> 
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>


<xsl:template match="ead/archdesc/relatedmaterial/p  | ead/archdesc/separatedmaterial/p">
<p style="margin-left : 30pt">
<xsl:apply-templates/>
</p>
</xsl:template>


<!--This template rule formats the top-level otherfindaid element.-->
<xsl:template name="archdesc-otherfindaid">
<xsl:if test="ead/archdesc/otherfindaid[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/otherfindaid">
<h3><a name="otherfindaid">
<strong><xsl:apply-templates select="head"/>
</strong></a></h3>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<!--This template rule formats the top-level index element.-->
<xsl:template name="archdesc-index">
<xsl:if test="ead/archdesc/index[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/index">
<h3><a name="index">
<strong><xsl:apply-templates select="head"/>
</strong></a></h3>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
<xsl:for-each select="indexentry">
<p style="margin-left : 50pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<!--This template rule formats the top-level bibliography element.-->
<xsl:template name="archdesc-bibliography">
<xsl:if test="ead/archdesc/bibliography[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/bibliography">
<h3><a name="bibliography">
<strong><xsl:apply-templates select="head"/>
</strong></a></h3>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
<xsl:for-each select="bibliography">
<p style="margin-left : 20pt">
<a name="{@id}">
<strong><xsl:apply-templates select="head"/>
</strong></a></p>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
<xsl:for-each select="bibref">
<p style="margin-left : 50pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
<xsl:for-each select="bibref">
<p style="margin-left : 50pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>


<!--This template rule formats the top-level odd element.-->
<xsl:template name="archdesc-odd">
<xsl:if test="ead/archdesc/odd[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/odd">
<h3><a name="odd">
<strong><xsl:apply-templates select="head"/>
</strong></a></h3>
<xsl:for-each select="p">
<p style="margin-left : 30pt"> 
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>



<xsl:template name="archdesc-control">
<xsl:if test="ead/archdesc/controlaccess[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/controlaccess">
<table width="100%">
<tr><td width="5%"> </td><td width="5%"> </td>
<td width="90%"> </td></tr>

<tr><td colspan="3"><h3><a name="controlaccess">
<xsl:apply-templates select="head"/>
</a></h3> </td></tr>

<tr><td> </td><td colspan="2"><em>
<xsl:apply-templates select="note"/></em>
</td></tr>

<tr><td> </td><td colspan="2">
<xsl:apply-templates select="p"/>
</td></tr>
    
<xsl:for-each select="subject |corpname | persname | genreform | title | geogname | occupation">
<xsl:sort select="."/>
<tr><td></td><td></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:for-each>

<xsl:for-each select="./controlaccess">
<tr><td> </td><td colspan="2"><strong>
<xsl:apply-templates select="head"/>
</strong></td></tr>

<xsl:for-each select="subject |corpname | persname | genreform | title | geogname | occupation">
<xsl:sort select="."/>
<tr><td></td><td></td><td>
<xsl:apply-templates select="."/>
</td></tr>
</xsl:for-each>
</xsl:for-each>
</table>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<!--This template rule formats a top-level accessretrict element.-->
<xsl:template name="archdesc-restrict">
<xsl:if test="ead/archdesc/accessrestrict[string-length(text()|*)!=0] | ead/archdesc/userestrict[string-length(text()|*)!=0]">
<h3>
<a name="accessrestrict">
<strong><xsl:text>Access or Use Restrictions</xsl:text>
</strong></a></h3>
<xsl:for-each select="ead/archdesc/accessrestrict">
<h4 style="margin-left : 15pt"><strong><xsl:value-of select="head"/></strong></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<xsl:for-each select="ead/archdesc/userestrict">
<h4 style="margin-left : 15pt"><strong><xsl:value-of select="head"/></strong></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>

<hr size="1"></hr>
</xsl:if>
</xsl:template>


<xsl:template name="archdesc-admininfo">
<xsl:if test="ead/archdesc/prefercite[string-length(text()|*)!=0] | ead/archdesc/custodhist[string-length(text()|*)!=0] | 
ead/archdesc/altformavail[string-length(text()|*)!=0] |
ead/archdesc/acqinfo[string-length(text()|*)!=0] | 
ead/archdesc/processinfo[string-length(text()|*)!=0] | ead/archdesc/appraisal[string-length(text()|*)!=0] | 
ead/archdesc/accruals[string-length(text()|*)!=0]">
<h3><a name="admininfo">
<xsl:text>Administrative Information</xsl:text>
</a></h3>
<xsl:call-template name="archdesc-custodhist"/>
<xsl:call-template name="archdesc-altform"/>
<xsl:call-template name="archdesc-prefercite"/>
<xsl:call-template name="archdesc-acqinfo"/>
<xsl:call-template name="archdesc-processinfo"/>
<xsl:call-template name="archdesc-appraisal"/>
<xsl:call-template name="archdesc-accruals"/>

<hr size="1"></hr>
</xsl:if>
</xsl:template>

<!--This template rule formats a top-level custodhist element.-->
<xsl:template name="archdesc-custodhist">
<xsl:if test="ead/archdesc/custodhist[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/custodhist">
<h4 style="margin-left : 15pt">
<a name="custodhist">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>


<!--This template rule formats a top-level altformavail element.-->
<xsl:template name="archdesc-altform">
<xsl:if test="ead/archdesc/altformavail[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/altformavail">
<h4 style="margin-left : 15pt">
<a name="altformavail">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>



<!--This template rule formats a top-level prefercite element.-->
<xsl:template name="archdesc-prefercite">
<xsl:if test="ead/archdesc/prefercite[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/prefercite">
<h4 style="margin-left : 15pt">
<a name="prefercite">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>


<!--This template rule formats a top-level acqinfo element.-->
<xsl:template name="archdesc-acqinfo">
<xsl:if test="ead/archdesc/acqinfo[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/acqinfo">
<h4 style="margin-left : 15pt"> 
<a name="acqinfo">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>

<!--This template rule formats a top-level procinfo element.-->
<xsl:template name="archdesc-processinfo">
<xsl:if test="ead/archdesc/processinfo[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/processinfo">
<h4 style="margin-left : 15pt">
<a name="processinfo">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>

<!--This template rule formats a top-level appraisal element.-->
<xsl:template name="archdesc-appraisal">
<xsl:if test="ead/archdesc/appraisal[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/appraisal">
<h4 style="margin-left : 15pt"> 
<a name="appraisal">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 30pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>

<!--This template rule formats a top-level accruals element.-->
<xsl:template name="archdesc-accruals">
<xsl:if test="ead/archdesc/accruals[string-length(text()|*)!=0]">
<xsl:for-each select="ead/archdesc/accruals">
<h4 style="margin-left : 15pt">
<a name="accruals">
<strong><xsl:apply-templates select="head"/>
</strong></a></h4>
<xsl:for-each select="p">
<p style="margin-left : 25pt">
<xsl:apply-templates select="."/>
</p>
</xsl:for-each>
</xsl:for-each>
</xsl:if>
</xsl:template>


</xsl:stylesheet>