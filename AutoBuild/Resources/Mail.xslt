<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="Status"></xsl:param>
  <xsl:param name="InstallerPath"></xsl:param>
  <xsl:variable name="Path">
    <xsl:value-of select="$InstallerPath"/>
  </xsl:variable>
  <xsl:template match="/">
    <table border="0" cellpadding="0" cellspacing="0" height="100%" width="100%" id="bodyTable">
      <tr>
        <td align="center" valign="top">
          <table border="10" cellpadding="20" cellspacing="0" width="600" style="border-color: #cce6ff;" id="emailContainer">
            <tr>
              <td align="center" valign="top">
                <table border="0" cellpadding="20" cellspacing="0" width="100%" id="emailHeader">
                  <tr>
                    <td align="center" valign="top" style="font-size: 25px;color:coral;border-color: #cce6ff;background-color: aliceblue">
                      Bally MultiConnect 16.x - Daily Build Status
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr>
              <td align="center" valign="top">
                <table border="1" cellpadding="20" cellspacing="0" width="100%" id="emailBody" style="border-color: #cce6ff;border-width: 5px;">
                  <tr>
                    <xsl:choose>
                      <xsl:when test="$Status = 'Failed'">
                        <td align="center" style="font-size: 20px;">
                          Build Status - <span style="color: red;">
                            <xsl:value-of select="$Status" />
                          </span>
                        </td>
                      </xsl:when>
                      <xsl:otherwise>
                        <td align="center"  style="font-size: 20px;">
                          Build Status - <span style="color: green;">
                            <xsl:value-of select="$Status" />
                          </span>
                        </td>
                      </xsl:otherwise>
                    </xsl:choose>
                  </tr>
                  <tr>
                    <td align="center" valign="top">
                      <!--Mail Content-->
                      <table width="100%" border="1" style="border-color:#bfbbbb">
                        <tr style="background-color: gainsboro;">
                          <th>Task</th>
                          <th>Status</th>
                        </tr>
                        <xsl:for-each select="TaskStatus/Tasks">
                          <tr>
                            <td align="center">
                              <xsl:choose>
                                <xsl:when test="$Status = 'Success' and TaskName = 'Copy Installer'">
                                  <a href="{$InstallerPath}">
                                    <xsl:value-of select="TaskName" />
                                  </a>
                                </xsl:when>
                                <xsl:otherwise>
                                  <xsl:value-of select="TaskName" />
                                </xsl:otherwise>
                              </xsl:choose>
                            </td>
                            <xsl:choose>
                              <xsl:when test="Status = 'Failed'">
                                <td align="center" style="color: red">
                                  <xsl:value-of select="Status" />
                                </td>
                              </xsl:when>
                              <xsl:otherwise>
                                <td align="center">
                                  <xsl:value-of select="Status" />
                                </td>
                              </xsl:otherwise>
                            </xsl:choose>
                          </tr>
                        </xsl:for-each>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
            <tr>
              <td align="center" valign="top">
                <table border="0" cellpadding="20" cellspacing="0" width="100%" id="emailFooter">
                  <tr>
                    <td align="center" valign="top" style="background-color: aliceblue;color:#696969;">
                      Auto Generated Mail
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
</xsl:stylesheet>