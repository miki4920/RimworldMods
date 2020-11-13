using System;
using RimWorld;
using Verse;

namespace Rimdungeon
{
    [StaticConstructorOnStartup]
    public static class RimDungeon
    {
        static RimDungeon() //our constructor
        {
            Log.Message("Hello World!"); //Outputs "Hello World!" to the dev console.
        }
    }
}