<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="width" select="500"/>
  <xsl:param name="height" select="500"/>
  <xsl:template name="main" match="/">  
  <div>
  <style type="text/css" media="all">
    table.board { border: 3px outset #8B4513; border-collapse:collapse; }
    td.at {        width:  <xsl:value-of select="$width * 0.09"/>px; height: <xsl:value-of select="$height * 0.09"/>px;     margin: 0px; padding: 0px;   border: 1px solid #2A1506;  align: center;  valign: middle;    }
    img.piece {  margin: 0px; padding: 0px; border 0px;   width:   <xsl:value-of select="$width * 0.85 * 0.09"/>px;        height:   <xsl:value-of select="$height * 0.85 * 0.09"/>px;              }     
  </style>
    <xsl:call-template   name="board">      
      <xsl:with-param name="cols" select="8"/>   
      <xsl:with-param name="rows" select="4"/>   
    </xsl:call-template>
  </div>
  </xsl:template>

  
  <xsl:template name="board" match="state/fact"> 
    <xsl:param name="cols" select="1"/>
    <xsl:param name="rows" select="1"/>
    <xsl:choose>
      <xsl:when test="//fact[relation='control' and argument[1]='p1']">
        <P>p1 to play</P>
      </xsl:when>
      <xsl:when test="//fact[relation='control' and argument[1]='p2']">
        <P>p2 to play</P>
      </xsl:when>
<!--
      <xsl:when test="(//fact[relation='piece' and argument[1]='red' and argument[5]='p1'])[1]">
        <P>p1 is red</P>
      </xsl:when>
      <xsl:when test="(//fact[relation='piece' and argument[1]='black' and argument[5]='p1'])[1]">
        <P>p1 is black</P>
      </xsl:when>
      <xsl:when test="(//fact[relation='piece' and argument[1]='red' and argument[5]='p2'])[1]">
        <P>p2 is red</P>
      </xsl:when>
      <xsl:when test="(//fact[relation='piece' and argument[1]='black' and argument[5]='p2'])[1]">
        <P>p2 is black</P>
      </xsl:when>
-->
      <xsl:when test="//fact[relation='piece']">
        <P>p2 is black</P>
      </xsl:when>

    </xsl:choose>
    <table class="board" background="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/002-wood-melamine-subttle-pattern-background-pat.jpg"> 
    <xsl:call-template name="board_rows">  
      <xsl:with-param name="cols"   select="$cols"/>  
      <xsl:with-param name="rows"   select="$rows"/> 
    </xsl:call-template> 
    </table>
  </xsl:template>

  <xsl:template name="board_rows">  
    <xsl:param name="cols" select="1"/>  <xsl:param name="rows"   select="1"/>  
    <xsl:param name="row" select="1"/>  
    <tr>   
      <xsl:call-template name="board_row"> 
        <xsl:with-param   name="cols" select="$cols"/> 
        <xsl:with-param name="rows"   select="$rows"/>  
        <xsl:with-param name="row" select="$row"/>  
      </xsl:call-template>  
    </tr> 
    <xsl:if test="$row &lt; $rows"> 
      <xsl:call-template name="board_rows">  
        <xsl:with-param   name="cols" select="$cols"/>   
        <xsl:with-param name="rows"   select="$rows"/>  
        <xsl:with-param name="row" select="$row + 1"/>   
      </xsl:call-template>  
     </xsl:if>
  </xsl:template>

  <xsl:template   name="board_row">  
    <xsl:param name="cols" select="1"/>   <xsl:param name="rows" select="1"/>
    <xsl:param name="row"  select="1"/>  <xsl:param name="col" select="1"/>
    <xsl:call-template name="at">    
      <xsl:with-param name="row" select="$row"/>  
      <xsl:with-param name="col" select="$col"/>   
    </xsl:call-template>  
    <xsl:if test="$col &lt; $cols">
      <xsl:call-template name="board_row">     
        <xsl:with-param   name="cols" select="$cols"/>     
        <xsl:with-param name="rows"   select="$rows"/>  
        <xsl:with-param name="row"   select="$row"/> 
        <xsl:with-param name="col" select="$col + 1"/>  
      </xsl:call-template>  
    </xsl:if>
  </xsl:template>

  <xsl:template name="at" match="state/fact"> 
  <xsl:param name="row" select="1"/> 
  <xsl:param name="col" select="1"/> 
  <td class="at">  
  <xsl:attribute name="id">  
    <xsl:value-of select="'at_'"/>   
      <xsl:value-of   select="$row"/>   <xsl:value-of select="$col"/>   
  </xsl:attribute>    
  <center>
  <xsl:choose>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[6] = 'hidden']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Blank_(Trad).png"/>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='a' and argument[6] = 'showing']">  
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_Advisor_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">6</FONT>
    </xsl:when>  
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='a' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_Advisor_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">6</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='c' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_Cannon_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">2</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='c' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_Cannon_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">2</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='r' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_Chariot_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">4</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='r' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_Chariot_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">4</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='e' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_Elephant_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">5</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='e' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_Elephant_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">5</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='g' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_General_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">7</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='g' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_General_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">7</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='h' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_Horse_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">3</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='h' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_Horse_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">3</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='red' and argument[2]='s' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Red_Soldier_(Trad).png"/>
      <FONT SIZE="-1" COLOR="red">1</FONT>
    </xsl:when>
    <xsl:when test="//fact[relation='piece' and argument[4]=5-$row and argument[3]=$col and argument[1]='black' and argument[2]='s' and argument[6] = 'showing']">
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_Black_Soldier_(Trad).png"/>
      <FONT SIZE="-1" COLOR="black">1</FONT>
    </xsl:when>
    <xsl:otherwise>
      <img class="piece"   src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/DarkChess/images/200px-Xiangqi_NoImage_(Trad).png"/>
    </xsl:otherwise>
  </xsl:choose>
  </center>  
  </td>  
  </xsl:template>
  
</xsl:stylesheet> 













