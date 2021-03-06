﻿#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software,
// and you are welcome to redistribute it under certain conditions; See
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion
using System.Collections.Generic;
using System.Linq;
using ProjectPorcupine.Pathfinding;

namespace ProjectPorcupine.Entities.States
{
    public class DumpState : State
    {
        public DumpState(Character character, State nextState = null)
            : base("Dump", character, nextState)
        {
        }

        public override void Update(float deltaTime)
        {
            Inventory tileInventory = character.CurrTile.Inventory;

            // Current tile is empty
            if (tileInventory == null)
            {
                DebugLog(" - Dumping");
                World.Current.InventoryManager.PlaceInventory(character.CurrTile, character.Inventory);
                Finished();
                return;
            }

            // Current tile contains the same type and there is room
            if (tileInventory.Type == character.Inventory.Type && (tileInventory.StackSize + character.Inventory.StackSize) <= tileInventory.MaxStackSize)
            {
                DebugLog(" - Dumping");
                World.Current.InventoryManager.PlaceInventory(character.CurrTile, character.Inventory);
                Finished();
                return;
            }

            List<Tile> path = Pathfinder.FindPathToDumpInventory(character.CurrTile, character.Inventory.Type, character.Inventory.StackSize);
            if (path != null && path.Count > 0)
            {
                character.SetState(new MoveState(character, Pathfinder.GoalTileEvaluator(path.Last(), false), path, this));
            }
            else
            {
                DebugLog(" - Can't find any place to dump inventory!");
                Finished();
            }
        }
    }
}
