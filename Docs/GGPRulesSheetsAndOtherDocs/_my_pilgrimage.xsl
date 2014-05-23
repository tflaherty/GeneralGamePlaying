<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:param name="width" select="500"/>
<xsl:param name="height" select="500"/>
<xsl:template name="main" match="/">  
  <div> <!-- Set Style -->    
  <style type="text/css" media="all"> 
    td.at {        width:  <xsl:value-of select="$width * 0.14"/>px; height: <xsl:value-of select="$height * 0.14"/>px;        border: 2px solid #000;        background-color: #004400;        align: center;  valign: middle;    }      
    table.board {   background-color: #004400;  }      
    img.piece {        
      width:   <xsl:value-of select="$width * 0.8 * 0.14"/>px;        
      height:   <xsl:value-of select="$height * 0.8 * 0.14"/>px;              
    }   
 </style>        
  <!-- Draw Board -->    
  <xsl:call-template   name="board">      
    <xsl:with-param name="cols" select="6"/>   
    <xsl:with-param name="rows" select="6"/>   
  </xsl:call-template>     
 </div>   
</xsl:template>
  <xsl:template name="at" match="state/structure"> 
  <xsl:param name="row" select="1"/>  <xsl:param name="col"   select="1"/> 
  <td class="at">  
  <xsl:attribute name="id">  
    <xsl:value-of select="'at_'"/>   
      <xsl:value-of   select="$row"/>   <xsl:value-of select="$col"/>   
  </xsl:attribute>    
  
  <xsl:choose>   
    <xsl:when test="//structure[name='cell' and arg[1] = $row and arg[2] = $col and arg[3]=1]"> 
      <xsl:attribute   name="style">background-color: #226622</xsl:attribute> 
    </xsl:when>   
    <xsl:when test="//structure[name='cell' and arg[1] = $row and arg[2] = $col and arg[3]=2]"> 
      <xsl:attribute   name="style">background-color: #669922</xsl:attribute> 
    </xsl:when>   
    <xsl:when test="//structure[name='cell' and arg[1] = $row and arg[2] = $col and arg[3]=3]"> 
      <xsl:attribute   name="style">background-color: #99BB22</xsl:attribute> 
    </xsl:when>   
    <xsl:when test="//structure[name='cell' and arg[1] = $row and arg[2] = $col and arg[3]=4]"> 
      <xsl:attribute   name="style">background-color: #BBDD22</xsl:attribute> 
    </xsl:when>   
    <xsl:when test="//structure[name='cell' and arg[1] = $row and arg[2] = $col and arg[3]=5]"> 
      <xsl:attribute   name="style">background-color: #FFFF22</xsl:attribute> 
    </xsl:when>   
  </xsl:choose>  

  <center>   

  <xsl:choose>   
    <xsl:when test="//structure[name='builder' and   arg[2]=$row and arg[3]=$col and arg[1]='red']">  
      <img class="piece"   src="/gamemaster/games/pilgrimage/red.png"/> 
    </xsl:when>  
    <xsl:when test="//structure[name='builder' and arg[2]=$row and   arg[3]=$col and arg[1]='blue']"> 
      <img class="piece"   src="/gamemaster/games/pilgrimage/blue.png"/> 
    </xsl:when> 
    <xsl:when test="//structure[name='pilgrim' and arg[2]=$row and   arg[3]=$col and arg[1]='red']"> 
      <img class="piece"   src="/gamemaster/games/pilgrimage/Red_Bishop.png"/> 
    </xsl:when> 
    <xsl:when test="//structure[name='pilgrim' and arg[2]=$row and   arg[3]=$col and arg[1]='blue']"> 
      <img class="piece"   src="/gamemaster/games/pilgrimage/Blue_Bishop.png"/> 
    </xsl:when> 
  </xsl:choose>
  </center>  
  </td>  
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
  <xsl:template name="board"> 
    <xsl:param name="cols" select="1"/>
    <xsl:param name="rows" select="1"/> 
    <table class="board"> 
    <xsl:call-template   name="board_rows">  
      <xsl:with-param name="cols"   select="$cols"/>  
      <xsl:with-param name="rows"   select="$rows"/> 
    </xsl:call-template> 
    </table>
  </xsl:template>
</xsl:stylesheet> 









