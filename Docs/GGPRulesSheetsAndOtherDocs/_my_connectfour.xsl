<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
<table width='240' height='180' cellspacing='0' cellpadding='0' border='1'>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='1' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='2' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='3' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='4' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='5' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='6' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='7' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='8' and arg[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='1' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='2' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='3' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='4' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='5' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='6' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='7' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='8' and arg[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='1' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='2' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='3' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='4' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='5' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='6' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='7' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='8' and arg[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='1' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='2' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='3' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='4' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='5' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='6' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='7' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='8' and arg[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='1' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='2' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='3' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='4' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='5' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='6' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='7' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='8' and arg[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='1' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='2' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='3' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='4' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='5' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='6' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='7' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/structure[name='cell' and arg[1]='8' and arg[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
</table>
</xsl:template>

  <xsl:template name="square">
    <xsl:if test="arg[3]='red'">
      <font size='6'>R</font>
    </xsl:if>
    <xsl:if test="arg[3]='black'">
      <font size='6'>B</font>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>
