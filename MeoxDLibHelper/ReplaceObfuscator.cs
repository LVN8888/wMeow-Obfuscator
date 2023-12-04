using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoxDLibHelper
{
    public class ReplaceObfuscator
    {
        //private static Mode _mode;

        private static Random _random = new Random(Guid.NewGuid().GetHashCode());

        /*public enum Mode
        {
            Simple,
            Homoglyph
        }

        public ReplaceObfuscator(Mode mode = Mode.Homoglyph)
        {
            _mode = mode;
        }*/

        public static void Execute(ModuleDef module)
        {
            var importer = new Importer(module);
            foreach (var type in module.GetTypes().Where(t => t.Methods.Count != 0))
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    var instructions = method.Body.Instructions;

                    for (int i = 0; i < instructions.Count; i++)
                    {
                        if (instructions[i].OpCode != OpCodes.Ldstr)
                            continue;

                        if ((string)instructions[i].Operand == string.Empty)
                            continue;

                        instructions[i].Operand = ObfuscateString((string)instructions[i].Operand);

                        var implant = new List<Instruction>();
                        var replaceMethod = importer.Import(typeof(string).GetMethod("Replace", new[] { typeof(string), typeof(string) }) ?? throw new InvalidDataException());

                        /*string[] glyphs = { "q", "w", "e", "r", "t" };

                        string[] ordered = glyphs.OrderBy(c => _random.Next()).ToArray();

                        for (int j = 0; j < ordered.Length; j++)
                        {
                            implant.Add(new Instruction(OpCodes.Ldstr, ordered[j]));
                            implant.Add(new Instruction(OpCodes.Ldnull));

                            if (j == 0)
                            {
                                implant.Add(new Instruction(OpCodes.Call, replaceMethod));
                                continue;
                            }

                            implant.Add(new Instruction(OpCodes.Callvirt, replaceMethod));
                        }*/

                        implant.Add(new Instruction(OpCodes.Ldstr, "\u115F"));
                        implant.Add(new Instruction(OpCodes.Ldnull));
                        implant.Add(new Instruction(OpCodes.Call, replaceMethod));

                        ((List<Instruction>)instructions).InsertRange(i + 1, implant);

                        i += implant.Count;
                    }

                    instructions.OptimizeMacros();
                }
            }
        }

        private static string ObfuscateString(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (_random.Next(0, 2) == 0)
                {
                    result.Append(new string('\u115F', _random.Next(16)));
                    result.Append(c);
                }
                else
                {
                    result.Append(c);
                    result.Append(new string('\u115F', _random.Next(16)));
                }
            }

            return result.ToString();
        }

        /// Old
        /*private static string ObfuscateString(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (_random.Next(0, 2) == 0)
                {
                    result.Append(_mode == Mode.Homoglyph ? new string(GetHomoglyph(c), _random.Next(4)) : new string('\u115F', _random.Next(16)));
                    result.Append(c);
                }
                else
                {
                    result.Append(c);
                    result.Append(_mode == Mode.Homoglyph ? new string(GetHomoglyph(c), 1) : new string('\u115F', _random.Next(16)));
                }
            }

            return result.ToString();
        }*/

        /*private static char GetHomoglyph(char input)
        {
            char[] glyphs = { 'q', 'w', 'e', 'r', 't' };
            switch (input)
            {
                case 'q':
                    return glyphs[0];
                case 'w':
                    return glyphs[1];
                case 'e':
                    return glyphs[2];
                case 'r':
                    return glyphs[3];
                default:
                    return glyphs[_random.Next(glyphs.Length)];
            }
        }*/
    }
}
