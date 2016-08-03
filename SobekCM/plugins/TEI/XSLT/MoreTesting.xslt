<?xml version="1.0" encoding="UTF-8"?><TEI xmlns="http://www.tei-c.org/ns/1.0">
    <teiHeader>
        <fileDesc>
            <titleStmt>
                <title>TEI by Example. Module 6: Primary Sources</title>
                <author xml:id="EV">Edward Vanhoutte</author>
                <author xml:id="RvdB">Ron Van den Branden</author>
                <editor>Edward Vanhoutte</editor>
                <editor>Ron Van den Branden</editor>
                <editor xml:id="MT">Melissa Terras</editor>
                <sponsor>Association for Literary and Linguistic Computing (ALLC)</sponsor>
                <sponsor>Centre for Digital Humanities (CDH), University College London, UK</sponsor>
                <sponsor>Centre for Computing in the Humanities (CCH), King's College London, UK</sponsor>
                <sponsor>Centre for Scholarly Editing and Document Studies (CTB) , Royal Academy of Dutch Language and Literature, Belgium</sponsor>
                <funder>
                    <address>
                        <addrLine>Centre for Scholarly Editing and Document Studies (CTB)</addrLine>
                        <addrLine>Royal Academy of Dutch Language and Literature</addrLine>
                        <addrLine>Koningstraat 18</addrLine>
                        <addrLine>9000 Gent</addrLine>
                        <addrLine>Belgium</addrLine>
                    </address>
                    <email>ctb@kantl.be</email>
                </funder>
                <principal>Edward Vanhoutte</principal>
                <principal>Melissa Terras</principal>
                <principal>Ron Van den Branden</principal>
            </titleStmt>
            <publicationStmt>
                <publisher>Centre for Scholarly Editing and Document Studies (CTB) , Royal Academy of Dutch Language and Literature, Belgium</publisher>
                <distributor>Centre for Scholarly Editing and Document Studies (CTB) , Royal Academy of Dutch Language and Literature, Belgium</distributor>
                <pubPlace>Gent</pubPlace>
                <address>
                        <addrLine>Centre for Scholarly Editing and Document Studies (CTB)</addrLine>
                        <addrLine>Royal Academy of Dutch Language and Literature</addrLine>
                        <addrLine>Koningstraat 18</addrLine>
                        <addrLine>9000 Gent</addrLine>
                        <addrLine>Belgium</addrLine>
                    </address>
                <availability status="free">
                    <p>Licensed under a <ref target="http://creativecommons.org/licenses/by-sa/3.0/">Creative Commons Attribution ShareAlike 3.0 License</ref></p>
                </availability>
                <date when="2010-07-09">9 July 2010</date>
            </publicationStmt>
            <seriesStmt>
                <title>TEI By Example.</title>
                <respStmt>
                    <name>Edward Vanhoutte</name>
                    <resp>editor</resp>
                </respStmt>
                <respStmt>
                    <name>Ron Van den Branden</name>
                    <resp>editor</resp>
                </respStmt>
                <respStmt>
                    <name>Melissa Terras</name>
                    <resp>editor</resp>
                </respStmt>
            </seriesStmt>
            <sourceDesc>
                <p>Digitally born</p>
            </sourceDesc>
        </fileDesc>
        <encodingDesc>
            <projectDesc>
                <p>TEI By Example offers a series of freely available online tutorials walking individuals through the different stages in marking up a document in TEI (Text Encoding Initiative). Besides a general introduction to text encoding, step-by-step tutorial modules provide example-based introductions to eight different aspects of electronic text markup for the humanities. Each tutorial module is accompanied with a dedicated examples section, illustrating actual TEI encoding practise with real-life examples. The theory of the tutorial modules can be tested in interactive tests and exercises.</p>
            </projectDesc>
        </encodingDesc>
        <profileDesc>
            <langUsage>
                <language ident="en-GB">en-GB</language>
            </langUsage>
        </profileDesc>
        <revisionDesc>
            <change when="2010-07-09" who="#RvdB">release</change>    
            <change when="2009-11-30" who="#RvdB">corrected typos</change>
            <change when="2009-06-11" who="#RvdB">editing</change>
            <change when="2009-04-27" who="#RvdB">authoring</change>
        </revisionDesc>
    </teiHeader>
    <text xml:id="TBED06v00" type="example">
        <body><head>Examples for Module 6: Primary sources</head><div xml:id="bentham" type="example">
    <head>Jeremy Bentham: <title>JB/088/179</title></head>
    <p>This manuscript page was written by the philosopher and jurist Jeremy Bentham (1748-1832).</p> 
    <figure><graphic url="images/examples/TBED06v00/088_179_002.jpg"/></figure>
    <p>This example encodes the prose text as a <gi>div</gi> inside the <gi>body</gi> of a <gi>text</gi> structure. It distinguishes between a main heading (<q>[Limits]</q>), and a subtitle (the phrase <q>Repugnancy, what</q>, in the right margin), by means of the <att>type</att> attribute on the <gi>head</gi> element. Since this is a prose text, the basic structural units are encoded as paragraphs (<gi>p</gi>), with line breaks encoded as <gi>lb/</gi> where they occur. Note how the usage of <gi>lb/</gi> is pointed out in a comment; although not the formal way to do it (that's what the <gi>tagUsage</gi> element in the header is for -- see TBE module <ref target="../modules/TBED02v00.htm#tagsDecl">2. The TEI Header</ref>), it may serve as a valid reminder for future encoders.  The sixth text line starts with a sequence of a deletion and addition: <q>on</q> is deleted (marked with the <gi>del</gi> tag), and replaced with <q>emane</q> (encoded as <gi>add</gi>). This sequence might as well have been encoded as a whole as a substitution, and wrapped in a <gi>subst</gi> element. This example features another interesting combination of deletion and addition on the penultimate line: the phrase <q>A law which</q> was started as a replacement for the phrase starting with <q>Repugnancy</q>. It was added above the line, but deleted again, without ever becoming an effective replacement. This is reflected in the encoding by encoding the addition first, but marking its contents as deleted:</p>
        <eg:egXML xmlns:eg="http://www.tei-c.org/ns/Examples">
            <!-- ... -->
            Repugnancy <del>
                <add>A law which</add>
            </del> may 
            <!-- ... -->
        </eg:egXML>
        <p>A final point of interest is the use of empty <gi>gap/</gi> elements to indicate places where the transcriber has deliberately left out text. Often these are deletions that have been crossed out  beyond readability. Note, how the reason for these omissions is not stated (which could be done in a <att>reason</att> attribute).</p>
    <egXML xmlns="http://www.tei-c.org/ns/Examples">
        <text>
            <body>
                <div>
                    <head type="main">[Limits]</head>
                    <head type="sub">Repugnancy, what</head>
                    <p>When two laws appear to disagree in their<lb/> terms, <del>it is</del> a great
                        question is often made<lb/> whether they are or are not repugnant. The<lb/>
                        occasion<del>s</del> on which it is brought upon the
                        carpet<lb/><!-- it is the provisional policy of Transcribe Bentham
                            to treat line-end hyphenation in this fashion -->
                        is generally where the two laws in question<lb/>
                        <del>on</del>
                        <add>emane</add> the one of them <gap/><gap/> from a <del>legislature</del> superior<lb/>
                        <del>which</del> the other from a superior legislature.<lb/> The question then
                        is <add>in truth</add> [properly speaking] a <add>great</add> question<lb/> of
                        constitutional law; but since the word which is<lb/> the subject of it is
                        <del>a</del> one of those which appears to be expressive of the <hi rend="underline">aspect</hi> of a superventitious<lb/>
                        <add>law</add> to a primordial one, it seems to have<lb/> some claim to be
                        consider’d here.</p>
                    <p>Hitherto the <gap/> primordial <del>law</del>
                        <add>provision</add> and the<lb/> superventitious have been consider’d as
                        <del>the</del> emaning<lb/> from the same authority <add>source</add>: so
                        long as this is the case<lb/> the word repugnant may be looked upon as
                        synonymous<lb/> to alterative. Repugnancy <del>
                            <add>A law which</add>
                        </del> may accordingly be simply <gap/> revocative or reversive; and in
                        either<lb/> case</p>
                </div>
            </body>
        </text>
    </egXML>
    <note type="bibl">Encoding of <bibl>Manuscripts of Jeremy Bentham, University College London Library:
        JB/088/179</bibl>, a manuscript encoded and made available by the Bentham Project of University College London (<ref target="http://www.ucl.ac.uk/Bentham-Project/">http://www.ucl.ac.uk/Bentham-Project/</ref>).</note>
    <!-- 
        $Date: 2010-07-13 17:36:06 +0200 (di, 13 jul 2010) $
        $Id: bentham.xml 251 2010-07-13 15:36:06Z ron.vandenbranden $  -->
</div><div xml:id="whitman" type="example">
    <head>Walt Whitman: <title>After the Argument</title></head>
    <p>This manuscript, featuring an early version of the poem <title>After the Argument</title>, was likely written in 1890 or early 1891, shortly before the poem's publication.</p> 
    <figure><graphic url="images/examples/TBED06v00/loc.00001.001.jpg"/></figure>
    <p>This example clearly illustrates how the TEI <ident>transcr</ident> module can be applied to verse texts as well. The entire poem is encoded inside <tag>lg type="poem"</tag>, containing a heading (<gi>head</gi>) and two verse lines (<gi>l</gi>). In order to reflect the (typographic) segments of these lines, they are further divided into <gi>seg</gi> elements. As will be clear from the facsimile, this short manuscript features some complex editorial traces. Sequential deletions (<gi>del</gi>) and additions (<gi>add</gi>) are grouped into substitutions (<gi>subst</gi>).  Moreover, inside the substitutions, the exact order of the editing interventions is specified by means of a sequence number in a <att>seq</att> attribute, making explicit that the deletions occurred before the additions.</p>
    <note>The <att>seq</att> attribute is a more advanced concept documented in the <ref target="http://www.tei-c.org/release/doc/tei-p5-doc/en/html/PH.html">TEI Guidelines, 11. Representation of Primary Sources</ref>. Note how this explicit sequence number is not strictly needed here, as deletions logically precede additions, and only one deletion is involved.</note>
    <p>This example illustrates nicely how additions and deletions can nest. In both cases in the example, an addition contains further deletions. The deletions are characterised as <val>overstrike</val> and <val>overwrite</val> in the respective <att>type</att> attributes. The additions are characterised as <val>insertion</val>, <val>overwrite</val>, or <val>unmarked</val>; their <att>place</att> attributes recording that they occurred <val>supralinear</val>, <val>over</val> existing text, or <val>inline</val>.</p>
    <egXML xmlns="http://www.tei-c.org/ns/Examples">
        <body>
            <pb xml:id="leaf01r" type="recto"/>
            <lg type="poem">
                <head rend="underline" type="main-authorial">After <subst>
                    <del type="overstrike" seq="1">an</del>
                    <add place="supralinear" type="insertion" seq="2">the <del type="overstrike">unsolv'd</del>
                    </add>
                </subst> argument</head>
                <l>
                    <seg>
                        <del type="overstrike">The</del>
                        <add place="supralinear" type="insertion">
                            <del type="overstrike">Coming in,</del>
                            <subst>
                                <del type="overwrite" seq="1">a</del>
                                <add place="over" type="overwrite" seq="2">A </add>
                            </subst> group of </add> little children, and their</seg>
                    <seg>ways and chatter, flow <add place="inline" type="unmarked">in, </add>
                        <del type="overstrike">
                            <add place="supralinear" type="unmarked">upon me</add>
                        </del>
                    </seg>
                </l>
                <l>
                    <seg>Like <add place="supralinear" type="insertion">welcome </add> rippling
                        water o'er my </seg>
                    <seg>heated <add place="supralinear" type="insertion">nerves and </add>
                        flesh.</seg>
                </l>
                <closer>
                    <signed>Walt Whitman</signed>
                </closer>
            </lg>
        </body>
    </egXML>
    <note type="bibl">Based on a TEI P4 XML encoding of <bibl><author>Whitman, Walt</author>, <title level="a">After the Argument</title></bibl>, a manuscript encoded and made available by the Walt Whitman Archive at <ref target="http://www.whitmanarchive.org/manuscripts/transcriptions/loc.00001.html">http://www.whitmanarchive.org/manuscripts/transcriptions/loc.00001.html</ref>.</note>
    <!-- 
        $Date: 2010-07-13 17:36:06 +0200 (di, 13 jul 2010) $
        $Id: whitman.xml 251 2010-07-13 15:36:06Z ron.vandenbranden $  -->
</div></body>
    </text>
    <!-- 
        $Date: 2010-07-09 16:34:23 +0200 (vr, 09 jul 2010) $
        $Id: TBED06v00.xml 245 2010-07-09 14:34:23Z ron.vandenbranden $  -->
</TEI>