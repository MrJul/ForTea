package com.jetbrains.fortea.language

import com.jetbrains.rider.ideaInterop.fileTypes.RiderLanguageBase

object T4Language : RiderLanguageBase("T4", "T4") {
    override fun isCaseSensitive(): Boolean = true
}


