package com.jetbrains.fortea.language

import com.intellij.icons.AllIcons
import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageFileTypeBase
import javax.swing.Icon

object T4FileType : RiderLanguageFileTypeBase(T4Language) {
  override fun getDefaultExtension() = "T4"
  override fun getDescription() = "T4 template file"
  override fun getIcon(): Icon = AllIcons.FileTypes.Text
  override fun getName() = "T4"
}
