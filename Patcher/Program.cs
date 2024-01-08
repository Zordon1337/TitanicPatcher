using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;

class Program
{
    static string newpath = "";
    static string path = "";
    static void Main(string[] args)
    {

        path = args[0];
        string originalAssemblyPath = path;
        string modifiedAssemblyPath = "./osu!.exe";
        string searchString = "ppy.sh";
        string replacementString = "lekuru.xyz";
        
        ModifyAssembly(path, modifiedAssemblyPath, searchString, replacementString);
    }
    static void ModifyAssembly(string originalPath, string modifiedPath, string searchString, string replacementString)
    {
        using (var originalAssembly = AssemblyDefinition.ReadAssembly(originalPath))
        {
            foreach (var module in originalAssembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    foreach (var field in type.Fields)
                    {
                        if (field.FieldType.FullName == "System.String" && field.HasConstant && field.Constant is string fieldValue)
                        {
                            field.Constant = fieldValue.Replace(searchString, replacementString);
                        }
                    }
                    foreach (var method in type.Methods)
                    {
                        if (method.HasBody)
                        {
                            foreach (var instruction in method.Body.Instructions)
                            {
                                if (instruction.OpCode == OpCodes.Ldstr && instruction.Operand is string operandString)
                                {
                                    instruction.Operand = operandString.Replace(searchString, replacementString);
                                }
                                
                            }
                        }
                    }
                }
            }

            originalAssembly.Write(modifiedPath);
        }

        Console.WriteLine("Patched succesfully");
    }
}
