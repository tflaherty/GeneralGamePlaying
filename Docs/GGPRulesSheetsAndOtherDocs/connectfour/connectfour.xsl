<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
<table width='240' height='180' cellspacing='0' cellpadding='0' border='1'>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='1' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='2' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='3' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='4' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='5' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='6' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='7' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='8' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='1' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='2' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='3' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='4' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='5' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='6' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='7' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='8' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='1' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='2' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='3' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='4' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='5' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='6' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='7' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='8' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='1' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='2' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='3' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='4' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='5' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='6' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='7' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='8' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='1' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='2' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='3' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='4' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='5' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='6' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='7' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='8' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='1' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='2' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='3' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='4' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='5' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='6' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='7' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='8' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
</table>
</xsl:template>

  <xsl:template name="square">
    <xsl:if test="argument[3]='red'"><img src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/connectfour/red.jpg"/></xsl:if>
    <xsl:if test="argument[3]='black'"><img src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/connectfour/blue.jpg"/></xsl:if>
  </xsl:template>

</xsl:stylesheet>
