using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Modding;
using MonoMod;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using UnityEngine;
using Mono.Cecil.Cil;

namespace FasterTransitions
{
    public class FasterTransitions : Mod, ITogglableMod
    {
        public static ILHook fasterTransitionHook;

        public override void Initialize()
        {
            Type T = typeof(HeroController).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance).First(t => t.Name == "<EnterScene>c__Iterator0");
            MethodBase M = T.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public);
            fasterTransitionHook = new ILHook(M, RemoveTransitionWaits);
        }

        private void RemoveTransitionWaits(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il).Goto(0);
            while (c.TryGotoNext(
                i => i.OpCode == OpCodes.Ldc_R4,
                i => i.OpCode == OpCodes.Newobj && i.MatchNewobj<WaitForSeconds>()
                ))
            {
                if ((float)c.Instrs[c.Index].Operand == .4f)
                {
                    c.Remove();
                    c.Emit(OpCodes.Ldc_R4, 0f);
                    c.Index += 2;
                }
                else if ((float)c.Instrs[c.Index].Operand == .165f)
                {
                    c.Remove();
                    c.Emit(OpCodes.Ldc_R4, 0f);
                    c.Index += 2;
                }
                else if ((float)c.Instrs[c.Index].Operand == .25f || (float)c.Instrs[c.Index].Operand == .33f)
                {
                    c.Index += 2;
                }
            }
        }

        public override string GetVersion()
        {
            return "1.0";
        }

        public void Unload()
        {
            fasterTransitionHook.Dispose();
        }
    }
}
