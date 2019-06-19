using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

using JetBrains.Core;
using JetBrains.Diagnostics;
using JetBrains.Collections;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using JetBrains.Serialization;
using JetBrains.Rd;
using JetBrains.Rd.Base;
using JetBrains.Rd.Impl;
using JetBrains.Rd.Tasks;
using JetBrains.Rd.Util;
using JetBrains.Rd.Text;


// ReSharper disable RedundantEmptyObjectCreationArgumentList
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantOverflowCheckingContext


namespace JetBrains.Rider.Model
{
  
  
  public class RdT4Model : RdExtBase
  {
    //fields
    //public fields
    
    //private fields
    //primary constructor
    internal RdT4Model(
    )
    {
    }
    //secondary constructor
    //statics
    
    
    
    protected override long SerializationHash => 3283706374122174060L;
    
    protected override Action<ISerializers> Register => RegisterDeclaredTypesSerializers;
    public static void RegisterDeclaredTypesSerializers(ISerializers serializers)
    {
      
      serializers.RegisterToplevelOnce(typeof(IdeRoot), IdeRoot.RegisterDeclaredTypesSerializers);
    }
    
    //custom body
    //equals trait
    //hash code trait
    //pretty print
    public override void Print(PrettyPrinter printer)
    {
      printer.Println("RdT4Model (");
      printer.Print(")");
    }
    //toString
    public override string ToString()
    {
      var printer = new SingleLinePrettyPrinter();
      Print(printer);
      return printer.ToString();
    }
  }
  public static class SolutionRdT4ModelEx
   {
    public static RdT4Model GetRdT4Model(this Solution solution)
    {
      return solution.GetOrCreateExtension("rdT4Model", () => new RdT4Model());
    }
  }
}
