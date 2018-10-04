using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Skills;
using Scripts.DarkParadise;
using System.Threading;
using System.Text.RegularExpressions;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class Loot
  {
    //---------------------------------------------------------------------------------------------

    public static Graphic BodyGraphic { get { return new Graphic(0x2006); } }
    public static string LootDefaultItemTypes = @"0x0E76:0x049A//loot bag
0x1F13:0x0000//??
0x0F0D:0x000E//lavabomb
0x0F7D:0x031D//darkblood
0x0F82:0x0000//dragonblood
0x1078:0x0615//darkhide
0x0E80:0x0123//pokladek
0x1010:0x0000//wrong klicek
0x1C18:0x0000//CRAFT OLEJ
0x0F3F:0x0000//sipy
0x1BFB:0x0000//sipky
0x166F:0x0000//parat harpye
0x1BD1:0x0000//peri
0x0EED:0x0000//GP
0x100E:0x0000//Klic Q3
0x0F27:0x0000//slzy FDD a Blood
0x0E21:0x0000//bandy
";

    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection DefaultLootItems
    {
      get
      {
        UOItemTypeCollection col = new UOItemTypeCollection();

        string currentLootTypes = CalebConfig.LootItemTypes;
        string[] lines = (currentLootTypes + String.Empty).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
          string line = lines[i].Trim();
          if (line.StartsWith("--") || String.IsNullOrEmpty(line))
            continue;

          try
          {
            Match m = Regex.Match(line + String.Empty, "^(?<graphic>0x[A-Za-z0-9]{4}):?(?<color>0x[A-Za-z0-9]{4})?.*$");

            Graphic g = 0;
            UOColor c = 0x0000;

            if (m.Success)
            {
              g = Graphic.Parse(m.Groups["graphic"].Value);
              
              if (m.Groups["color"].Success && !String.IsNullOrEmpty(m.Groups["color"].Value))
                c = UOColor.Parse(m.Groups["color"].Value);
            }

            if (g > 0 && !g.IsInvariant)
            {
              col.Add(new UOItemType() { Graphic = g, Color = c });
            }


          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine("DefaultLootItems " + i + " Ex: " + ex.Message);
          }

        }

        return col;
      }
    }


    //---------------------------------------------------------------------------------------------

    public static UOItemTypeCollection lootItems;
    public static UOItemTypeCollection LootItems
    {
      get
      {
        return DefaultLootItems;
        //if (lootItems == null)
        //{
        //  lootItems = new UOItemTypeCollection();
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E76, Color = 0x049A });//loot bag
        //  lootItems.Add(new UOItemType() { Graphic = 0x1F13, Color = 0x0000 });
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F0D, Color = 0x000E });//lavabomb
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F7D, Color = 0x031D });//darkblood
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F82, Color = 0x0000 });//dragonblood
        //  lootItems.Add(new UOItemType() { Graphic = 0x1078, Color = 0x0615 });//darkhide
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E80, Color = 0x0123 });//pokladek
        //  lootItems.Add(new UOItemType() { Graphic = 0x1010, Color = 0x0000 });//wrong klicek
        //  lootItems.Add(new UOItemType() { Graphic = 0x1C18, Color = 0x0000 });//CRAFT OLEJ
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E34, Color = 0x0000 });//Blanky
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F3F, Color = 0x0000 });//sipy
        //  lootItems.Add(new UOItemType() { Graphic = 0x1BFB, Color = 0x0000 });//sipky
        //  lootItems.Add(new UOItemType() { Graphic = 0x166F, Color = 0x0000 });//parat harpye
        //  lootItems.Add(new UOItemType() { Graphic = 0x1BD1, Color = 0x0000 });//peri
        //  lootItems.Add(new UOItemType() { Graphic = 0x0EED, Color = 0x0000 });//GP
        //  lootItems.Add(new UOItemType() { Graphic = 0x100E, Color = 0x0000 });//Klic Q3
        //  lootItems.Add(new UOItemType() { Graphic = 0x0F27, Color = 0x0000 });//slzy FDD a Blood
        //  lootItems.Add(new UOItemType() { Graphic = 0x0E21, Color = 0x0000 });//bandy

        //  //0x0F27 slzy FDD a Blood
        //  //0x0F27 
        //  //
        //  //0x1BFB  
        //}
        //return lootItems;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsLootItem(UOItem item)
    {
      UOItemTypeBaseCollection regs = ReagentCollection.Reagents.ToItemTypeCollection();

      if (Array.IndexOf(regs.GraphicList.ToArray(), item.Graphic) > -1)
        return true;

      foreach (UOItemType o in LootItems)
      {
        if (item.Graphic == o.Graphic && o.Color == item.Color)
          return true;

        if (o.Color == 0x0000 && item.Graphic == o.Graphic)
          return true;

        if (item.Graphic == 0x1F13) //skilpoint
          return true;
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public UOItem LootBag
    {
      get
      {
        if (World.Player.Backpack.Items.FindType(0x0E76).Exist)
          return World.Player.Backpack.Items.FindType(0x0E76);

        return World.Player.Backpack;
      }
    }

    //---------------------------------------------------------------------------------------------

    protected UOItem dwarfKnife;
    public UOItem DwarfKnife
    {
      get
      {
        if (dwarfKnife == null)
          dwarfKnife =  World.Player.Backpack.Items.FindType(0x10E4);
        return dwarfKnife;
      }
    }

    //---------------------------------------------------------------------------------------------

    public enum LootType
    {
      None = 0,
      Quick = 1,
      QuickCut = 2,
      Safe = 3,
      SafeCut = 4
    }


    //---------------------------------------------------------------------------------------------

    private List<Serial> cutedBodies = new List<Serial>();

    [Executable]
    [BlockMultipleExecutions]
    public void LootGround()
    {
      this.LootGround(LootType.Quick);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void LootGround(LootType lootType)
    {
      LootGround(lootType, false);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void LootGround(LootType lootType, bool lootAll)
    {
      //List<UOItem> searchItems = new List<UOItem>();
      List<UOItem> ground = new List<UOItem>();
      ground.AddRange(World.Ground.ToArray());

      List<UOItem> search = new List<UOItem>();

      int done = 0;
      int toDo = 0;
      int bodies = 0;

      List<Serial> bpkState = ItemHelper.ContainerState(World.Player.Backpack);

      foreach (UOItem item in ground)
      {
        if (item.Graphic == 0x2006)
        {
          bodies++;
          if (cutedBodies.Contains(item.Serial))
          {
            done++;
          }
          else
          {
            toDo++;
            if (item.Distance <= 6)
              search.Add(item);
          }
        }
      }
      
      for (int i = 0; i < search.Count; i++)//(UOItem item in search)
      {
        UOItem item = search[i];
        Game.PrintMessage(String.Format("Try Loot Body [{0}/{1}]", i + 1, search.Count), Game.Val_LightGreen);

        if (!item.Opened)
        {
          Journal.Clear();
          item.Use();
          if (Journal.WaitForText(true, 250, "You can't reach that"))
          {
            UO.PrintObject(item.Serial, Game.Val_LightGreen, "[Can't reach]");
            continue;
          }
        }

        List<UOItem> items = new List<UOItem>();
        items.AddRange(item.Items.ToArray());


        if (items.FirstOrDefault(si => si.Graphic == 0x0E76  && si.Color == 0x049A) != null || lootAll) //item.Items.FindType(0x0E76, 0x049A).Exist)// lootbag
        {
          UO.PrintObject(item.Serial, Game.Val_LightGreen, "[Looting...]");

          if (DwarfKnife.Exist && item.Distance <= 3 && !lootAll)//TODO predelat nejak jinak, ted kvuly tamingu aby to nelotovalo maso a kuze
          {
            toDo--;
            done++;

            foreach (UOItem lootItem in items)
            {
              if (Loot.IsLootItem(lootItem))
              {
                lootItem.Move(65000, LootBag);
                Game.Wait(300);
              }
            }

            DwarfKnife.Use();
            UO.WaitTargetObject(item);
            Game.Wait(350);

            items = new List<UOItem>();
            items.AddRange(item.Items.ToArray());
          }

          if (item.Exist)
          {

            foreach (UOItem lootItem in items)
            {
              if (Loot.IsLootItem(lootItem) || lootAll)
              {
                lootItem.Move(60000, LootBag);
                Game.Wait(425);
              }
            }
          }
        }

        cutedBodies.Add(item.Serial);
      }

      World.Player.PrintMessage(String.Format("Bodies remain [{0}]", toDo), Game.Val_LightGreen);

      foreach (UOItem item in World.Ground)
      {
        if (item.Distance <= 6 && IsLootItem(item))
        {
          item.Move(60000, LootBag);
          Game.Wait(425);
        }
      }

      List<Serial> bpkAfterLoot = ItemHelper.ContainerState(World.Player.Backpack);
      List<Serial> diff = ItemHelper.ContainerStateDiff(bpkState, bpkAfterLoot);

      Game.PrintMessage("Diff... " + diff.Count);
      if (LootBag.Serial != World.Player.Backpack.Serial)
      {
        foreach (Serial lootedItem in diff)
        {
          UOItem item = new UOItem(lootedItem);

          if (item.Container == World.Player.Backpack.Serial)
          {
            if (item.Move(65000, LootBag))
            {
              Game.PrintMessage("LootItem Moved to Bag...");
            }
            Game.Wait();
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    private void LootCollection(List<UOItem> searchItems)
    {
      this.LootCollection(searchItems, false);
    }

    //---------------------------------------------------------------------------------------------

    private void LootCollection(List<UOItem> searchItems, bool lootAll)
    {
      UOItemTypeBaseCollection regs = ReagentCollection.Reagents.ToItemTypeCollection();
      foreach (UOItem item in searchItems)
      {
        bool grabed = false;

        if (lootAll || IsLootItem(item))
        {
          item.Move(1000, LootBag);
          grabed = true;
        }

        if (grabed)
          Game.Wait(435 + Core.Latency + (lootAll ? 1000 : 0));
      }
    }
    //---------------------------------------------------------------------------------------------
  }
}


