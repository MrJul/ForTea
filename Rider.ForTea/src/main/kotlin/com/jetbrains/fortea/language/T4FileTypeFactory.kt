package com.jetbrains.fortea.language

import com.intellij.openapi.fileTypes.FileTypeConsumer
import com.intellij.openapi.fileTypes.FileTypeFactory
import com.jetbrains.fortea.language.T4FileType

class T4FileTypeFactory : FileTypeFactory() {
    override fun createFileTypes(consumer: FileTypeConsumer) = consumer.consume(T4FileType)
}

