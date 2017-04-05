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
                       <li><xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-info-sign"}</xsl:attribute>Description: <xsl:value-of select="af:Description"/></li>
                       <li>
                           <xsl:choose>
                               <xsl:when test="af:ResultLevel='Error'">
                                   <xsl:attribute name="class">text-danger</xsl:attribute>
                                   <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-remove"}</xsl:attribute>
                               </xsl:when>
                               <xsl:when test="af:ResultLevel='Success'">
                                   <xsl:attribute name="class">text-success</xsl:attribute>
                                   <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-ok"}</xsl:attribute>
                               </xsl:when>
                               <xsl:when test="af:ResultLevel='None'">
                                   <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-unchecked"}</xsl:attribute>
                               </xsl:when>
                           </xsl:choose>
                           Result level: <xsl:value-of select="af:ResultLevel"/></li>
                       <xsl:apply-templates select="af:Nodes/af:TestNodes" mode="testNodes"/>
                        <li>Results
                            <ul>
                            <xsl:apply-templates select="./af:Results/af:TestResult"></xsl:apply-templates>
                            </ul>
                        </li>
                   </ul>
               </div>
           </body>
        </html>
    </xsl:template>
   <xsl:template match="af:TestNodes" mode="testNodes">
       <li><xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon glyphicon-list-alt"}</xsl:attribute>
              <xsl:value-of select="af:NodeType"/> 
              <ul>
                  <xsl:if test="./af:Description">
                      <li><xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon glyphicon-info-sign"}</xsl:attribute>Description: <xsl:value-of select="./af:Description"/></li>
                  </xsl:if>
                  <xsl:if test="./af:ResultLevel">                      
                      <li><xsl:choose>
                          <xsl:when test="./af:ResultLevel='Error'">
                              <xsl:attribute name="class">text-danger</xsl:attribute>
                              <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-remove"}</xsl:attribute>
                          </xsl:when>
                          <xsl:when test="./af:ResultLevel='Success'">
                              <xsl:attribute name="class">text-success</xsl:attribute>
                              <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-ok"}</xsl:attribute>
                          </xsl:when>
                          <xsl:when test="./af:ResultLevel='None'">
                              <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-unchecked"}</xsl:attribute>
                          </xsl:when>
                      </xsl:choose>Result level: <xsl:value-of select="./af:ResultLevel"/></li>
                  </xsl:if>
                  <xsl:if test="count(./af:Nodes/af:TestNodes) > 0">
                      <li><xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon glyphicon-list-alt"}</xsl:attribute>Test Results
                          <ul>
                              <xsl:apply-templates select="af:Nodes/af:TestNodes"/>    
                          </ul>
                      </li>
                  </xsl:if>
                  <xsl:if test="count(./af:Results/af:TestResult) > 0">
                      <li>Results
                          <ul>
                      Results: <xsl:apply-templates select="./af:Results/af:TestResult"/>
                          </ul>
                      </li>
                  </xsl:if>
              </ul>
         </li>   
   </xsl:template>
    <xsl:template match="af:TestNodes">
        <li><xsl:value-of select="./af:NodeType"/>
            <ul>
                <xsl:if test="./af:Description">
                    <li><xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon glyphicon-info-sign"}</xsl:attribute>Description: <xsl:value-of select="./af:Description"/></li>
                </xsl:if>
                <xsl:if test="./af:ResultLevel">
                    <li><xsl:choose>
                        <xsl:when test="./af:ResultLevel='Error'">
                            <xsl:attribute name="class">text-danger</xsl:attribute>
                            <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-remove"}</xsl:attribute>
                        </xsl:when>
                        <xsl:when test="./af:ResultLevel='Success'">
                            <xsl:attribute name="class">text-success</xsl:attribute>
                            <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-ok"}</xsl:attribute>
                        </xsl:when>
                        <xsl:when test="./af:ResultLevel='None'">
                            <xsl:attribute name="data-jstree">{"icon":"glyphicon glyphicon-unchecked"}</xsl:attribute>
                        </xsl:when>
                    </xsl:choose>Result level: <xsl:value-of select="./af:ResultLevel"/></li>
                </xsl:if>
                <xsl:if test="count(./af:Nodes/af:TestNodes) > 0">
                    <xsl:apply-templates select="./af:Nodes/af:TestNodes"/>
                </xsl:if>
                <xsl:if test="count(./af:Results/af:TestResult) > 0">
                <li>Test Results
                    <ul>
                        <xsl:apply-templates select="./af:Results/af:TestResult"/>
                    </ul>
                </li>
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