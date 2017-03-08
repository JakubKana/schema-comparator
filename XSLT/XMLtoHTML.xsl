<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:xhtml="http://www.w3.org/1999/xhtml"
    xmlns:af="http://alef.com/db-comparator/test"
    xmlns="http://alef.com/db-comparator/test"
    exclude-result-prefixes="xs"
    version="2.0">
    
    <xsl:output method="html" encoding="utf-8" indent="yes"/>
    
    
    <xsl:template match="/af:TestNodes">
        <xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
        <html>
           <head>
               <title>Result <xsl:value-of select="fn:current-time()"/></title>
           </head>
           <body>
               <h3>Result Tree</h3>
               <table>
                  <xsl:apply-templates select="af:Nodes/af:TestNodes" mode="testNodes"/>
               </table>
           </body>
        </html>
    </xsl:template>
    
   <xsl:template match="af:TestNodes" mode="testNodes">
       <tr>
           <td>NodeType</td>
            <td><xsl:value-of select="af:NodeType"/></td>
       </tr>
   </xsl:template>
</xsl:stylesheet>