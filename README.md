
Allows the use of an F# computation expression to output IL to an ILGenerator.

The code looks something like:

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
       
 For more info see http://vegetarianprogrammer.blogspot.com