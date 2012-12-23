<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
	<xsl:output method="xml" indent="yes" />

	<xsl:variable name="ReleaseMajor" select="1" />
	<xsl:variable name="ReleaseMinor" select="0" />
	<xsl:variable name="ReleaseBuild" select="0" />

	<xsl:template match="/RootInfo">
		<UpdateInfos>
			<xsl:variable name="InstalledMajor" select="Plugin/Version/@Major" />
			<xsl:variable name="InstalledMinor" select="Plugin/Version/@Minor" />
			<xsl:variable name="InstalledBuild" select="Plugin/Version/@Build" />

			<xsl:if
				test="($InstalledMajor &lt; $ReleaseMajor) or ($InstalledMajor = $ReleaseMajor and $InstalledMinor &lt; $ReleaseMinor) or ($InstalledMajor = $ReleaseMajor and $InstalledMinor = $ReleaseMinor and $InstalledBuild &lt; $ReleaseBuild)">
				<UpdateInfo>
					<InformationUri>https://github.com/MrJul/ForTea</InformationUri>
					<Title>
						<xsl:value-of select="concat('ForTea ', $ReleaseMajor, '.', $ReleaseMinor, '.', $ReleaseBuild)" />
					</Title>
					<Description>An upgrade for ForTea is available.</Description>

					<DownloadUri>
						<xsl:value-of select="concat('http://download.flynware.com/ForTea/ForTea-', $ReleaseMajor, '.', $ReleaseMinor, '.', $ReleaseBuild, '.msi')" />
					</DownloadUri>
					<CompanyName>Julien Lebosquain</CompanyName>
					<ProductName>ForTea</ProductName>
					<ProductVersion>
						<xsl:value-of select="concat($ReleaseMajor, '.', $ReleaseMinor, '.', $ReleaseBuild, '.0')" />
					</ProductVersion>
					<PriceTag></PriceTag>
					<IsFree>true</IsFree>
					<IconData>
						iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAABp0RVh0U29mdHdhcmUAUGF
						pbnQuTkVUIHYzLjUuMTAw9HKhAAAEK0lEQVRYR7WXW2hcRRjHsyGoeMEHhYKCIvqk+CIqFQUVpYpUqg8KWgUVq4VKXgwUYlELSSuxlVLbkG0jSVtNbDdq11
						tumNa0m4SWarZNum22e7+eve/Zc86ePRvz95tJV9ttNjtRzsKfPRxm5v/75sx8M19DQ42frpeaSiXjHsMoP0V6jfQBaQvpM9IuUiep67L20v8XpDbSZtJ7p
						JdJj9IYq2isxlo+V703DMNCnZo1rXiioKiKXFCQkwvI5mVkcnlksjmk64i1YW1Znzz1ZWPQWAmtqB9VVW2t2+2uzUIAjUQsK6rGO+YuG3PTTBYpQbG2rA8D
						YWOwIBhYJBpTlwVgaKVSKaHrOmgWSBqXykRQTIqqQlFUiooiY9GRZDLIy2REZlkyZQDJVApxSUI4EoHfH4DX54fH68vW/RQEEFoEYIbMjJlUDGhqWWSZDFL
						pNDdJJJOQEgnE4xKisTiLkkyjCIXDCAZD8AeC8P0LIIkAeEwECIkAuEwE8IgAOE0EOC8CcNpEgGkRAIeJAKfqApD52H8BGHSlMXw+xXfBMVcCtukkfNfugp
						MiAEMrBYjEE7jVuoCX7EUO8HBfCXd8NY/AtQBjIgA/1gM47smjzaH+kwccl9Jo2AN8/LuMYCSGG7sW8Nx32lJ5YEgEYGApgHROxqFzKh63lWEhs0e+LWPMn
						cOWcQW9wzYc+XkDdv0yis6RCf7cbe+C3SlVJyK7CEDflQChtIq2KR139/7Fo2TmD/WXse+PAjqmFP7up8HXgQsWrO0ex9YjO+i5AW22DrSO5aoBbCIAPRWA
						wy4dN9F0MpPbuxew6TcdDp9MuX4xFftiaUzOxRGYuB/G7PUYHf0IcxNPcIBjDjtmvHQOXJ2KvxEB2FcB+Py0wc2v6wRaT+i4GKPDpuoscLlmUTx3MzetSDt
						7C5xO51JroEcE4MsKgDeloWXc4NFXQF60G7DNFiCl0jg4LePZg2F82Lefa1v/TpqJG+BzPIjH+hTsmUxVz4BVBGBn9SKUsgXsPlPEA1/PcxCmNd8bsJ4p4L
						b9i2vjrp55vHnoOF8LIyPrcW9vGR0nM9UAu0UAttfahuyCcdSl4PkfDKw+XObb8N1hDU17gYuhBGamtvPPcHayvdZxvEMEYGu9PMDuA3PRDAdg2/G+A2V+H
						/jkVzee7voTM64LtQC2iQC0igBULiRvDRbR7igsAozL2DCkLHch+VQEoGUlACu8EbWKADSbCNAiArDRRIBmEYB3TAR4XwTgDRMB3hYBeNVEgPUiAOtMBHhF
						BGC1iQBP1gUoFouNpBeoMuqmyshDldHC/6yMAlQZHaDSbJ37kqepLsCVDWRZtlDtt4r+n8nn8xtzuXw7HcdWug/0UyYcSCZTNkpENklKDFAm7KfSzBqJRtv
						D4cimUCi8JhAM3un1ByzLmf4NMsOlnTPJV8gAAAAASUVORK5CYII=
					</IconData>
					<xsl:variable name="CallToBugFixAllowed" select="false" />
				</UpdateInfo>
			</xsl:if>
		</UpdateInfos>
	</xsl:template>

</xsl:stylesheet>