module EmitUtils

open System
open System.Reflection
open System.Reflection.Emit

type EmitOp =
   | Call of MethodInfo
   | Label of Label
   | Goto of Label

type EmitBuilder(ilgen: ILGenerator) =
   member this.ILGen = ilgen
   member this.Yield (c: EmitOp) = 
      match c with 
      | Call m  -> ilgen.Emit(OpCodes.Callvirt, m)
      | Label l -> ilgen.MarkLabel(l)
      | Goto l  -> ilgen.Emit(OpCodes.Br, l)

   member this.Yield (u:unit) = u
   member this.Yield (o: OpCode) = ilgen.Emit(o)
   member this.Yield (inp: OpCode * int32) = match inp with (a, b) -> ilgen.Emit(a, b)
   member this.Yield (inp: OpCode * string) = match inp with (a, b) -> ilgen.Emit(a, b)
   member this.Yield (inp: OpCode * LocalBuilder) = match inp with (a, b) -> ilgen.Emit(a, b)
   member this.Yield (inp: OpCode * Type) = match inp with (a, b) -> ilgen.Emit(a, b)
   member this.Yield (inp: OpCode * FieldInfo) = match inp with (a, b) -> ilgen.Emit(a, b)
   member this.Yield (inp: OpCode * MethodInfo) = match inp with (a, b) -> ilgen.Emit(a, b)
   member this.Yield (inp: OpCode * Label) = match inp with (a, b) -> ilgen.Emit(a, b)

   member this.Delay (x: unit -> unit) = x
   member this.Run x = x()

   // e1;e2 translates to something like b.Combine(e1, Delay(e2)) and Delay in our case returns a function.
   member this.Combine (x, y) = y()

   member this.Zero (u:unit) = ()

   member this.TryFinally (body: unit -> unit, final: unit -> unit) = 
      try
         ilgen.BeginExceptionBlock() |> ignore
         body()
      finally
         ilgen.BeginFinallyBlock()
         final()
         ilgen.EndExceptionBlock()
    
   member this.For (exp: 'a * 'b, body: LocalBuilder -> unit) =
      let start = ilgen.DefineLabel()
      let finish = ilgen.DefineLabel()
      let loopVar = ilgen.DeclareLocal(typeof<int32>)
      let (from, length) = exp
      this {
         match box from with
         | :? int32 as i -> ilgen.Emit(OpCodes.Ldc_I4, i) // todo: optimize into ldc_i4_0 etc.
         | :? LocalBuilder as l -> ilgen.Emit(OpCodes.Ldloc, l)
         | _ -> failwith "invalid from type"

         yield OpCodes.Stloc, loopVar
   
         yield Label(start)

         yield OpCodes.Ldloc, loopVar

         // load length
         match box length with
         | :? int32 as i ->        yield OpCodes.Ldc_I4, i // todo: optimize into ldc_i4_0 etc.
         | :? LocalBuilder as l -> yield OpCodes.Ldloc, l
         | _ -> failwith "invalid length type"

         yield OpCodes.Ceq             // pop2 , load 0 or 1
         yield OpCodes.Brtrue, finish
   
         body loopVar

         yield OpCodes.Ldloc, loopVar
         yield OpCodes.Ldc_I4_1        // push 1
         yield OpCodes.Add             // add 1 to array length
         yield OpCodes.Stloc, loopVar
         yield OpCodes.Br, start       // goto next entry

         yield Label(finish)
      }