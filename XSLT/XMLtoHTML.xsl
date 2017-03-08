<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    xmlns:fn="http://www.w3.org/2005/xpath-functions"
    xmlns:xhtml="http://www.w3.org/1999/xhtml"
    xmlns="http://alef.com/db-comparator/test"
    exclude-result-prefixes="xs"
    version="2.0">
    
    <xsl:output encoding="UTF-8" method="html" doctype-system=""/>
    
    
    <xsl:template match="/TestNodes">
        
       <html>
           <head>
               <title>Result <xsl:value-of select="fn:current-time()"/></title>
           </head>
           
           <body>
               <xsl:apply-templates select="Nodes"/>
           </body>
       </html>
    </xsl:template>
    
   
</xsl:stylesheet>