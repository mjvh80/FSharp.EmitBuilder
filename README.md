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
 
--
 
Copyright (C) 2012 Marcus van Houdt

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.