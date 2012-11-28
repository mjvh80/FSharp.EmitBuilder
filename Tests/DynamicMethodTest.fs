module DynamicMethodTest

open System.Reflection.Emit
open Xunit
open EmitUtils

type HelloWorldDelegate = delegate of int32 -> string

[<Fact>]
let DynamicMethodTest() =
   let meth = new DynamicMethod("HelloWorld", typeof<string>, [| typeof<int32> |])
   let asm = new EmitBuilder(meth.GetILGenerator())
   asm {
         let count = asm.ILGen.DeclareLocal(typeof<int32>)
         yield OpCodes.Ldarg_0  // load count
         yield OpCodes.Stloc, count
         
         yield OpCodes.Ldstr, ""

         for i in (0, count) do
            yield OpCodes.Ldstr, "Hello World! "
            yield OpCodes.Call, typeof<string>.GetMethod("Concat", [| typeof<string>; typeof<string> |])

         yield OpCodes.Ret
       }
   let d = meth.CreateDelegate(typeof<HelloWorldDelegate>) :?> HelloWorldDelegate
   Assert.Equal<string>("", d.Invoke(0))
   Assert.Equal<string>("Hello World! ", d.Invoke(1))
   Assert.Equal<string>("Hello World! Hello World! ", d.Invoke(2))