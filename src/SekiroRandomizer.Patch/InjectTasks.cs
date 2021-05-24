using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SekiroRandomizer.Patch
{
    static class InjectTasks
    {
        [InjectTarget(
            Namespace = "RandomizerCommon",
            Type = "Randomizer",
            Member = "Randomize"
        )]
        public static void Randomizer_Randomize(AssemblyDefinition assembly)
        {
            var type = assembly.Modules.SelectMany(m => m.Types).Single(t => t.Namespace == "RandomizerCommon" && t.Name == "Randomizer");
            var method = type.Methods.Single(m => m.Name == "Randomize");
            var instructions = method.Body.Instructions;

            var il = method.Body.GetILProcessor();

            var ins_Error_MissingDataDirectory = instructions[16];
            var ins_Log_OptionsAndSeeds = instructions[23];
            var ins_Status_LoadingGameData = instructions[34];
            var ins_Error_ModsDirectoryNotFound = instructions[57];
            var ins_Error_AlreadyRunningFromModsDirectory = instructions[73];
            var ins_Log_EnemySearchInstruction = instructions[90];
            var ins_Log_ItemSearchInstruction = instructions[98];
            var ins_Status_RandomizingEnemies = instructions[142];
            var ins_Status_RandomizingItems = instructions[165];
            var ins_Status_EditingGameFiles = new[]{
                instructions[216], instructions[330]
            };
            var ins_Status_WritingGameFiles = new[] {
                instructions[279], instructions[365]
            };
            var ins_Status_Randomizing = instructions[311];

            var ins_GetResource = il.GetResourceInstruction(type.Module);

            #region Error_MissingDataDirectory
            ins_Error_MissingDataDirectory.Operand = ResourceManager.Error_MissingDataDirectory;
            il.InsertAfter(ins_Error_MissingDataDirectory, ins_GetResource);
            #endregion
            #region Log_OptionsAndSeeds
            ins_Log_OptionsAndSeeds.Operand = ResourceManager.Log_OptionsAndSeeds;
            il.InsertAfter(ins_Log_OptionsAndSeeds, ins_GetResource);
            #endregion
            #region Status_LoadingGameData
            ins_Status_LoadingGameData.Operand = ResourceManager.Status_LoadingGameData;
            il.InsertAfter(ins_Status_LoadingGameData, ins_GetResource);
            #endregion
            #region Error_ModsDirectoryNotFound
            ins_Error_ModsDirectoryNotFound.Operand = ResourceManager.Error_ModsDirectoryNotFound;
            il.Remove(ins_Error_ModsDirectoryNotFound.Next.Next.Next.Next);
            il.Remove(ins_Error_ModsDirectoryNotFound.Next.Next.Next);
            il.InsertAfter(ins_Error_ModsDirectoryNotFound.Next.Next, il.Create(
                OpCodes.Call,
                type.Module.ImportReference(typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object) }))
            ));
            il.InsertAfter(ins_Error_ModsDirectoryNotFound, ins_GetResource);
            #endregion
            #region Error_AlreadyRunningFromModsDirectory
            ins_Error_AlreadyRunningFromModsDirectory.Operand = ResourceManager.Error_AlreadyRunningFromModsDirectory;
            il.InsertAfter(ins_Error_AlreadyRunningFromModsDirectory, ins_GetResource);
            #endregion
            #region Log_EnemySearchInstruction
            ins_Log_EnemySearchInstruction.Operand = ResourceManager.Log_EnemySearchInstruction;
            il.InsertAfter(ins_Log_EnemySearchInstruction, ins_GetResource);
            #endregion
            #region Log_ItemSearchInstruction
            ins_Log_ItemSearchInstruction.Operand = ResourceManager.Log_ItemSearchInstruction;
            il.InsertAfter(ins_Log_ItemSearchInstruction, ins_GetResource);
            #endregion
            #region Status_RandomizingEnemies
            ins_Status_RandomizingEnemies.Operand = ResourceManager.Status_RandomizingEnemies;
            il.InsertAfter(ins_Status_RandomizingEnemies, ins_GetResource);
            #endregion
            #region Status_RandomizingItems
            ins_Status_RandomizingItems.Operand = ResourceManager.Status_RandomizingItems;
            il.InsertAfter(ins_Status_RandomizingItems, ins_GetResource);
            #endregion
            #region Status_EditingGameFiles
            foreach (var ins in ins_Status_EditingGameFiles)
            {
                ins.Operand = ResourceManager.Status_EditingGameFiles;
                il.InsertAfter(ins, ins_GetResource); 
            }
            #endregion
            #region Status_WritingGameFiles
            foreach (var ins in ins_Status_WritingGameFiles)
            {
                ins.Operand = ResourceManager.Status_WritingGameFiles;
                il.InsertAfter(ins, ins_GetResource);
            }
            #endregion
            #region Status_Randomizing
            ins_Status_Randomizing.Operand = ResourceManager.Status_Randomizing;
            il.InsertAfter(ins_Status_Randomizing, ins_GetResource);
            #endregion

        }

        [InjectTarget(
            Namespace = "RandomizerCommon",
            Type = "GameData",
            Member = "LoadParams"
        )]
        public static void GameData_LoadParams(AssemblyDefinition assembly)
        {
            var type = assembly.Modules.SelectMany(m => m.Types).Single(t => t.Namespace == "RandomizerCommon" && t.Name == "GameData");
            var method = type.Methods.Single(m => m.Name == "LoadParams");
            var instructions = method.Body.Instructions;

            var il = method.Body.GetILProcessor();

            var ins_Log_UsingModdedFile = new[]{
                instructions[16],instructions[41],instructions[51]
            };
            var ins_Error_MissingParamFiles = instructions[60];

            var ins_GetResource = il.GetResourceInstruction(type.Module);
            var ins_StringFormat = il.Create(
                OpCodes.Call,
                type.Module.ImportReference(typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object) }))
            );

            #region Log_UsingModdedFile
            foreach (var ins in ins_Log_UsingModdedFile)
            {
                ins.Operand = ResourceManager.Log_UsingModdedFile;
                il.Replace(ins.Next.Next, ins_StringFormat);
                il.InsertAfter(ins, ins_GetResource);
            }
            #endregion
            #region Error_MissingParamFiles
            ins_Error_MissingParamFiles.Operand = ResourceManager.Error_MissingParamFiles;
            il.Replace(ins_Error_MissingParamFiles.Next.Next, ins_StringFormat);
            il.InsertAfter(ins_Error_MissingParamFiles, ins_GetResource);
            #endregion

        }

        [InjectTarget(
            Namespace = "RandomizerCommon",
            Type = "EnemyRandomizer",
            NestedTypes = new[] { "<>c__DisplayClass11_0" },
            Member = "<Run>g__fullName|92"
        )]
        public static void EnemyRandomizer_Run_fullname(AssemblyDefinition assembly)
        {
            var type = assembly.Modules.SelectMany(m => m.Types).Single(t => t.Namespace == "RandomizerCommon" && t.Name == "EnemyRandomizer").NestedTypes.Single(t => t.Name == "<>c__DisplayClass11_0");
            var method = type.Methods.Single(m => m.Name == "<Run>g__fullName|92");
            var instructions = method.Body.Instructions;

            var il = method.Body.GetILProcessor();

            var ins_EnemyFromLocation = instructions[37];
            var ins_GetResource = il.GetResourceInstruction(type.Module);

            var ins_ArgsArray = new[]
            {
                instructions[38],
                instructions[39],
                instructions[40],
                instructions[41],
                instructions[43],
                instructions[44],
                instructions[45],
                instructions[48],
                instructions[49],
                instructions[50],
                instructions[51],
                instructions[52],
                instructions[53],
                instructions[54],
                instructions[55],
                instructions[56],
                instructions[57],
                instructions[58],
                instructions[71]
            };
            il.Replace(72, il.Create(
                OpCodes.Call,
                type.Module.ImportReference(typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object), typeof(object) }))
            ));
            instructions[38].OpCode = OpCodes.Ldc_I4_3;
            foreach (var ins in ins_ArgsArray)
            {
                il.Remove(ins);
            }
            var ins_Next = ins_GetResource;
            il.InsertBefore(ins_EnemyFromLocation.Next, ins_Next);
            ins_EnemyFromLocation.Operand = ResourceManager.Log_EnemyFromLocation;
            var ins_EnemyInLocation = il.Create(OpCodes.Ldstr, ResourceManager.Log_EnemyInLocation);
            il.InsertBefore(ins_EnemyFromLocation, (instructions[27].Operand = il.Create(OpCodes.Ldarg_2)) as Instruction);
            il.InsertBefore(ins_EnemyFromLocation, il.Create(OpCodes.Brtrue, ins_EnemyInLocation));
            il.InsertAfter(ins_EnemyFromLocation, ins_EnemyInLocation);
            il.InsertAfter(ins_EnemyFromLocation, il.Create(OpCodes.Br, ins_Next));

        }

        [InjectTarget(
            Namespace = "RandomizerCommon",
            Type = "EnemyRandomizer",
            Member = "Run"
        )]
        public static void EnemyRandomizer_Run(AssemblyDefinition assembly)
        {
            var type = assembly.Modules.SelectMany(m => m.Types).Single(t => t.Namespace == "RandomizerCommon" && t.Name == "EnemyRandomizer");
            var method = type.Methods.Single(m => m.Name == "Run");
            var instructions = method.Body.Instructions;

            var il = method.Body.GetILProcessor();

            var ins_ReplacingEnemyTo = instructions[3327];
            var ins_GetResource = il.GetResourceInstruction(type.Module);

            il.Replace(3340, il.Create(
                OpCodes.Call,
                type.Module.ImportReference(typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string), typeof(object), typeof(object) })))
            );
            il.RemoveAt(3339);
            il.RemoveAt(3333);
            ins_ReplacingEnemyTo.Operand = ResourceManager.Log_ReplacingEnemyTo;
            il.InsertAfter(ins_ReplacingEnemyTo, ins_GetResource);

        }

        [InjectTarget(
            Namespace = "RandomizerCommon",
            Type = "SekiroForm",
            Member = "InitializeComponent"
        )]
        public static void SekiroForm_InitializeComponent(AssemblyDefinition assembly)
        {
            var type = assembly.Modules.SelectMany(m => m.Types).Single(t => t.Namespace == "RandomizerCommon" && t.Name == "SekiroForm");
            var method = type.Methods.Single(m => m.Name == "InitializeComponent");
            var instructions = method.Body.Instructions;

#if DEBUG
            string ilcodes = instructions.GetILCodes();
#endif

            var il = method.Body.GetILProcessor();

            var ins_GetResource = il.GetResourceInstruction(type.Module);

#if DEBUG
            ilcodes = instructions.GetILCodes();
            assembly.Write("test.exe");
#endif
        }

        internal static string GetILCodes(this IEnumerable<Instruction> instructions)
        {
            return string.Join(Environment.NewLine, instructions.Select(ins => $"IL_{ins.Offset:x4}: {ins.OpCode.Name} {(ins.Operand is string ? $"\"{ins.Operand}\"" : (ins.Operand is Instruction) ? $"IL_{(ins.Operand as Instruction).Offset:x4}" : ins.Operand)}"));
        }

        internal static Instruction GetResourceInstruction(this ILProcessor il, ModuleDefinition module) =>
            il.Create(
                OpCodes.Call,
                module.ImportReference(typeof(ResourceManager).GetMethod(nameof(ResourceManager.GetResource)))
            );
    }
}
