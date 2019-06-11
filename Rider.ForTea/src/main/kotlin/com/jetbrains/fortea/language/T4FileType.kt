package com.jetbrains.fortea.language

import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageFileTypeBase

object T4FileType : RiderLanguageFileTypeBase(T4Language) {
  override fun getDefaultExtension() = "T4"
  override fun getDescription() = "T4 template file"
  override fun getIcon() = TODO()
  override fun getName() = "T4"
}
