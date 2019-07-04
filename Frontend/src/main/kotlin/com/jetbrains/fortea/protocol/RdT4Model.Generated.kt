@file:Suppress("PackageDirectoryMismatch", "UnusedImport", "unused", "LocalVariableName")
package com.jetbrains.rider.model

import com.jetbrains.rd.framework.*
import com.jetbrains.rd.framework.base.*
import com.jetbrains.rd.framework.impl.*

import com.jetbrains.rd.util.lifetime.*
import com.jetbrains.rd.util.reactive.*
import com.jetbrains.rd.util.string.*
import com.jetbrains.rd.util.*
import kotlin.reflect.KClass



class RdT4Model internal constructor(
) : RdExtBase() {
    //companion
    
    companion object : ISerializersOwner {
        
        override fun registerSerializersCore(serializers: ISerializers) {
        }
        
        
        
        
        const val serializationHash = 3283706374122174060L
    }
    override val serializersOwner: ISerializersOwner get() = RdT4Model
    override val serializationHash: Long get() = RdT4Model.serializationHash
    
    //fields
    //initializer
    //secondary constructor
    //equals trait
    //hash code trait
    //pretty print
    override fun print(printer: PrettyPrinter) {
        printer.println("RdT4Model (")
        printer.print(")")
    }
}
val Solution.rdT4Model get() = getOrCreateExtension("rdT4Model", ::RdT4Model)

