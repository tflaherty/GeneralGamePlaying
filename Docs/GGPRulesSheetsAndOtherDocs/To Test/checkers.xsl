<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
<head>
  <style type="text/css">

    /* Set a default background color to get a two-color effect,
    or apply only the second rule for a single color. */
    .grid tbody tr td 
    { 
      background-color: #ffc; 
    }

    .grid tbody tr:nth-child(odd) td:nth-child(even),
    .grid tbody tr:nth-child(even) td:nth-child(odd) { background-color: #eee; }

  </style>
</head>
<table class="grid" width='239' height='180' cellspacing='0' cellpadding='0' border='1'>
  <tr>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='8']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
  </tr>

  <tr>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
    <td width='30' height='30' align='center' valign='center'>
      <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='7']">
        <xsl:call-template name="square"/>
      </xsl:for-each>
    </td>
  </tr>
  <tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='6']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='5']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='4']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='3']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='2']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
<tr>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='a' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='b' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='c' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='d' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='e' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='f' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center'>
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='g' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
<td width='30' height='30' align='center' valign='center' style="background-color:#FF0000">
  <xsl:for-each select="/state/fact[relation='cell' and argument[1]='h' and argument[2]='1']">
    <xsl:call-template name="square"/>
  </xsl:for-each>
</td>
</tr>
</table>
</xsl:template>

  <xsl:template name="square">
    <!--
    <xsl:if test="argument[3]='red'"><img src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/connectfour/red.jpg"/></xsl:if>
    <xsl:if test="argument[3]='black'"><img src="D:/AI/Docs/GGPRulesSheetsAndOtherDocs/connectfour/blue.jpg"/></xsl:if>
    -->
    <xsl:if test="argument[3]='wp'">
      wp
    </xsl:if>
    <xsl:if test="argument[3]='wk'">
      wk
    </xsl:if>
    <xsl:if test="argument[3]='bp'">
      bp
    </xsl:if>
    <xsl:if test="argument[3]='bk'">
      bk
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>
