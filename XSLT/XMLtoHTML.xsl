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
               <link rel="stylesheet" href="css/themes/style.min.css" />
               <link rel="stylesheet" href="css/bootstrap.min.css"/>
               <script src="js/jquery-3.2.0.js"/>
               <script src="js/bootstrap.min.js"/>
               <script src="js/jstree.min.js"/>
               <script>
                   $(document).ready(
                   function() {
                   $(function () { $('#js_tree').jstree(); });
                        console.log( "Tree strucutre ready!" );
                   }
                   );
               </script>
           </head>
           <body>
               <h3>Result Tree</h3>
               <div id="js_tree">
                   <ul> 
                       <li>Node: <xsl:value-of select="af:NodeType"/></li>
                       <li>Description: <xsl:value-of select="af:Description"/></li>
                       <li>Result level: <xsl:value-of select="af:ResultLevel"/></li>
                  <xsl:apply-templates select="af:Nodes/af:TestNodes" mode="testNodes"/>
                  
                   </ul>
               </div>
           </body>
        </html>
    </xsl:template>
    
   <xsl:template match="af:TestNodes" mode="testNodes">
     
         <li class="jstree-open">
              <xsl:value-of select="af:NodeType"/> 
              <ul>
                  <xsl:if test="count(./af:Description) > 0">
                    <li>Description: <xsl:value-of select="af:Description"/></li>
                  </xsl:if>
                 
                  <xsl:if test="count(./af:Nodes/af:TestNodes) > 0">
                      <li>Test Results
                          <ul>
                              <xsl:apply-templates select="af:Nodes/af:TestNodes" mode="testResults"></xsl:apply-templates>    
                          </ul>
                      </li>
                  </xsl:if>
                  <xsl:if test="count(./af:Results/af:TestResult) > 0">
                      Results: <xsl:apply-templates select="./af:Results/af:TestResult"/>
                  </xsl:if>
                  <xsl:if test="count(./af:ResultLevel) > 0">
                      <li>Result level: <xsl:value-of select="af:ResultLevel"/></li>
                  </xsl:if>
              </ul>
         </li>   
     
   </xsl:template>
    
    <xsl:template match="af:TestNodes" mode="testResults">
        <li>Test: <xsl:value-of select="./af:NodeType"/>
            <ul>
                <xsl:if test="./af:Description">
                    <li>Description: <xsl:value-of select="./af:Description"/></li>
                </xsl:if>
                <xsl:if test="./af:ResultLevel">
                    <li>Result level: <xsl:value-of select="./af:ResultLevel"/></li>
                </xsl:if>
                <xsl:if test="count(./af:Nodes/af:TestNodes) > 0">
                    <xsl:apply-templates select="." />
                </xsl:if>
                <xsl:if test="./af:Results">
                    <xsl:apply-templates select="./af:Results/af:TestResult"/>
                </xsl:if>
            </ul>
        </li>
    </xsl:template>
    <xsl:template match="af:TestNodes">
        <li>Test: <xsl:value-of select="./af:NodeType"/>
            <ul>
                <xsl:if test="./af:Description">
                    <li>Description: <xsl:value-of select="./af:Description"/></li>
                </xsl:if>
                <xsl:if test="./af:ResultLevel">
                    <li>Result level: <xsl:value-of select="./af:ResultLevel"/></li>
                </xsl:if>
                <xsl:if test="count(./af:Nodes/af:TestNodes) > 0">
                    <xsl:apply-templates select="./af:Nodes/af:TestNodes"/>
                </xsl:if>
                <xsl:if test="./af:Results">
                    <ul>
                        <xsl:apply-templates select="./af:Results/af:TestResult"/>
                    </ul>
                </xsl:if>
            </ul>
        </li>
    </xsl:template>
    <xsl:template match="af:TestResult">
        <li>Test details: <xsl:value-of select="./af:ObjectType"/>
            <ul>
                <li><xsl:value-of select="./af:TestedObjectName"/></li>
                <li><xsl:value-of select="./af:ErrorType"/></li>
                <li><xsl:value-of select="./af:Description"/></li>
            </ul>
        </li>
    </xsl:template>
    
</xsl:stylesheet>