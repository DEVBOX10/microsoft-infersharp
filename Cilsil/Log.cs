﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Cilsil.Utils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cilsil
{
    /// <summary>
    /// TODO: use https://nlog-project.org or log4net instead of this class.
    /// </summary>
    public static class Log
    {
        private static bool debugMode = false;

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void SetDebugMode(bool isDebugMode) => debugMode = isDebugMode;

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static Dictionary<string, int> UnfinishedMethods { get; } =
            new Dictionary<string, int>();

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static Dictionary<string, int> UnknownInstructions { get; } =
            new Dictionary<string, int>();

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void RecordUnfinishedMethod(string methodName, int remainingInstructions) =>
            UnfinishedMethods[methodName] = remainingInstructions;

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void RecordUnknownInstruction(string instruction)
        {
            if (UnknownInstructions.ContainsKey(instruction))
            {
                UnknownInstructions[instruction] += 1;
            }
            else
            {
                UnknownInstructions.Add(instruction, 1);
            }
        }

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void PrintAllUnknownInstruction()
        {
            if (debugMode)
            {
                WriteLine("Unknown instructions:");
                foreach (var instr in UnknownInstructions.OrderBy(kv => kv.Value))
                {
                    WriteLine($"{instr.Key}: {instr.Value}");
                }
            }
        }

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void PrintCoverageStats(IEnumerable<MethodDefinition> methods)
        {
            var totalMethodCount = 0;
            var failMethodCount = 0;
            var succMethodCount = 0;
            var totalInstr = 0;
            var failInstr = 0;
            var succInstr = 0;
            try
            {
                totalMethodCount = methods.Count();
                failMethodCount = UnfinishedMethods.Count;
                succMethodCount = totalMethodCount - failMethodCount;
                totalInstr = methods.Sum(m => m.Body.Instructions.Count);
                failInstr = UnfinishedMethods.Sum(kv => kv.Value);
                succInstr = totalInstr - failInstr;
            }
            catch (NotImplementedException ex)
            {
                WriteWarning(ex.Message);
            }

            WriteLine("Coverage Statistics:\n");
            WriteLine($@"Method successfully translated: {succMethodCount} ({
                ComputePercent(succMethodCount, totalMethodCount)}%)");
            WriteLine($@"Method partially translated: {failMethodCount} ({
                ComputePercent(failMethodCount, totalMethodCount)}%)");
            WriteLine($@"Instructions translated: {succInstr} ({
                ComputePercent(succInstr, totalInstr)}%)");
            WriteLine($@"Instructions skipped: {failInstr} ({
                ComputePercent(failInstr, totalInstr)}%)");
            WriteLine("======================================\n");
        }

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void WriteLine() => Console.WriteLine();

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void WriteLine(string s) => Console.WriteLine(s);

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void WriteLine(string s, ConsoleColor c)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.WriteLine(s);
            Console.ForegroundColor = prevColor;
        }

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void Write(string s) => Console.Write(s);

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void Write(string s, ConsoleColor c)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = c;
            Console.Write(s);
            Console.ForegroundColor = prevColor;
        }

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void WriteError(string s) => WriteLine(s, ConsoleColor.Red);

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void WriteWarning(string s)
        {
            if (debugMode)
            {
                WriteLine(s, ConsoleColor.Yellow);
            }
        }

        /// <summary>
        /// TODO: use https://nlog-project.org or log4net instead of this class.
        /// </summary>
        public static void WriteParserWarning(object invalidObject,
                                            Instruction instruction,
                                            ProgramState state)
        {
            WriteWarning($"Unable to complete translation of {instruction?.ToString()}:");
            WriteWarning(state.GetStateDebugInformation(invalidObject));
        }

        /// <summary>
        /// For use in the extension scenario -- writes a line indicating progress for every 
        /// quarter of progress.
        /// </summary>
        /// <param name="i">Current number of items processed.</param>
        /// <param name="total">Total number of items to process.</param>
        public static void WriteProgressLine(int i, int total)
        {
            if ((((double)i / total) % .25) != ((double)(i - 1) / total) % .25)
            {
                var current = (double)i / total;
                var previous = (double)(i - 1) / total;
                if (current >= 0.25 && previous < 0.25 ||
                    current >= 0.5 && previous < 0.5 ||
                    current >= 0.75 && previous < 0.75)
                {
                    var nearestQuarter =
                        100 * Math.Round(current * 4, MidpointRounding.ToEven) / 4;
                    WriteLine("Progress is " + nearestQuarter.ToString() + '%');
                }
            }
        }

        private static int ComputePercent(double n, double total) =>
            (int)Math.Round(100.0 * n / total, MidpointRounding.ToEven);
    }
}
