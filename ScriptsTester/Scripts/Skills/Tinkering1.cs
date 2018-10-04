using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;
using Caleb.Library.CAL.Business;

namespace CalExtension.Skills
{
  public class Tinkering1 : Skill
  {
    public static Graphic TinkersTools = 0x1EBC;
    public static UOItemType Thread { get { return new UOItemType() { Graphic = 0x0FA0, Color = 0x0000 }; } }
    public static UOItemType Vlasec { get { return new UOItemType() { Graphic = 0x0FA0, Color = 0x02B3 }; } }
    public static UOItemType IronWire { get { return new UOItemType() { Graphic = 0x1876, Color = 0x0000 }; } }
    public static UOItemType IronString { get { return new UOItemType() { Graphic = 0x1420, Color = 0x0000 }; } }
    public static UOItemType Cotton { get { return new UOItemType() { Graphic = 0x0DF9, Color = 0x0000 }; } }
    public static UOItemType IronIngot { get { return new UOItemType() { Graphic = 0x1BEF, Color = 0x0000 }; } }
    public static UOItemType Log { get { return new UOItemType() { Graphic = 0x1BDD, Color = 0x0000 }; } }
    public static UOItemType Board { get { return new UOItemType() { Graphic = 0x1BD7, Color = 0x0000 }; } }
      

    //---------------------------------------------------------------------------------------------
    [Executable]
    public static void CraftPickAxes()
    {
      while (Mining2.FindIngot(UO.Backpack.AllItems).Exist && UO.Backpack.AllItems.FindType(0x1BDD).Exist)//log
      {
        JournalEventWaiter jew = new JournalEventWaiter(true, "You put the", "You have failed");
        UO.WaitMenu("Blacksmithing", "Tools", "Tools", "Pick axe");
        Mining2.FindIngot(UO.Backpack.AllItems).Use();
        jew.Wait(6000);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MakeVlasce()
    {
        
        UO.Print(0x456,"Start");
        UOItem backpack = World.Player.Backpack;
        UO.Print("Vyber container s matrosem:");
        UOItem containerFrom = new UOItem(UIManager.TargetObject());

        ItemHelper.EnsureContainer(containerFrom);
        //3 odloz drevo
        UO.Backpack.AllItems.FindType(Log.Graphic, Log.Color).Move(100, containerFrom);
        Game.Wait();
        //4 zober 25 dreva
        containerFrom.AllItems.FindType(Log.Graphic, Log.Color).Move(10, backpack);
        Game.Wait();
        
        UO.Print(0x456,"Container check");
          if (containerFrom.Exist)
          {
            UO.Print(0x456,"Threads check: "+containerFrom.AllItems.FindType(Thread.Graphic, Thread.Color).Amount);
            while (containerFrom.AllItems.FindType(Thread.Graphic, Thread.Color).Amount > 4)
            {                    
                //1 odloz ingoty
                UO.Backpack.AllItems.FindType(IronIngot.Graphic, IronIngot.Color).Move(100, containerFrom);
                Game.Wait();
                //2 zober si 10 ingotov
                containerFrom.AllItems.FindType(IronIngot.Graphic, IronIngot.Color).Move(50, backpack);
                Game.Wait();
                
                while(
                containerFrom.AllItems.Count(IronString.Graphic, IronString.Color) > 4
                && backpack.AllItems.Count(IronString.Graphic, IronString.Color) < 2)
                {
                    containerFrom.AllItems.FindType(IronString.Graphic, IronString.Color).Move(1, backpack);
                    Game.Wait();
                }
                
                while(backpack.AllItems.Count(IronString.Graphic, IronString.Color) < 2)
                {
                    while(backpack.AllItems.Count(IronWire.Graphic, IronWire.Color) < 1)
                    {
                        containerFrom.AllItems.FindType(IronIngot.Graphic, IronIngot.Color).Move(2, backpack);
                        Game.Wait();
                        backpack.AllItems.FindType(IronIngot.Graphic, IronIngot.Color).Move(1, containerFrom);
                        Game.Wait();
                        MakeOneWire();
                        Game.Wait();
                    }
                    MakeOneString();
                    Game.Wait();
                }
                
                if (UO.Backpack.AllItems.FindType(Thread.Graphic, Thread.Color).Amount < 4)
                {
                    containerFrom.AllItems.FindType(Thread.Graphic, Thread.Color).Move(4, UO.Backpack);
                    Game.Wait();
                }
                MakeOneVlasec();
                
                if(backpack.AllItems.Count(Vlasec.Graphic, Vlasec.Color) > 0)
                {
                    UO.Print(0x456,"Vlasce: "+containerFrom.AllItems.FindType(Vlasec.Graphic, Vlasec.Color).Amount);
                    UO.Print(0x456,"Thready: "+containerFrom.AllItems.FindType(Thread.Graphic, Thread.Color).Amount);
                    backpack.AllItems.FindType(Vlasec.Graphic, Vlasec.Color).Move(1000, containerFrom);
                }
            }
        }
    }
       
    
    private static void MakeOneString()
    {
        JournalEventWaiter jew = new JournalEventWaiter(true, "You put the", "Tinkering failed");
        UO.WaitMenu("Tinkering", "Wires", "Wires", "Iron String"); 
        UOItem tools = UO.Backpack.Items.FindType(TinkersTools);
        if (tools.Exist)
        {
            tools.Use();
            jew.Wait(7500);
            if (Journal.Contains(true, "You can't make anything with what you have"))
            {
              Journal.Clear();              
            }
         }
         else
         {
            UO.Print("Nejsou toolsy.");
            UO.Say(",terminateall");
         }
    }
    
    private static void MakeOneVlasec()
    {
        JournalEventWaiter jew = new JournalEventWaiter(true, "You put the", "Tinkering failed");
        UO.WaitMenu("Tinkering", "Wires", "Wires", "Vlasec");
        UOItem tools = UO.Backpack.Items.FindType(TinkersTools);
        if (tools.Exist)
        {
            tools.Use();
            jew.Wait(7500);
            if (Journal.Contains(true, "You can't make anything with what you have"))
            {
              Journal.Clear();             
            }
         }
         else
         {
            UO.Print("Nejsou toolsy.");
            UO.Say(",terminateall");
         }
    }
    
    private static void MakeOneWire()
    {
        JournalEventWaiter jew = new JournalEventWaiter(true, "You put the", "Tinkering failed");
        UO.WaitMenu("Tinkering", "Wires", "Wires", "Iron Wire");
        UOItem tools = UO.Backpack.Items.FindType(TinkersTools);
        if (tools.Exist)
        {
            tools.Use();
            jew.Wait(7500);
            if (Journal.Contains(true, "You can't make anything with what you have"))
            {
              Journal.Clear();              
            }
         }
         else
         {
            UO.Print("Nejsou toolsy.");
            UO.Say(",terminateall");
         }
    }
    
    [Executable]
    public static void MakeNite()
    {
      UOItem backpack = World.Player.Backpack;
      UO.Print("Vyber container s matrosem:");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      
      while(containerFrom.AllItems.FindType(Cotton.Graphic).Exist)
      {
        if (UO.InJournal("World save has been initiated"))
            {
              UO.DeleteJournal();
              UO.Wait(30000);  
            }
        containerFrom.AllItems.FindType(Cotton.Graphic).Move(2, backpack);
        UO.Wait(500);
        UO.WaitTargetObject(0x40306F2B);
        backpack.AllItems.FindType(Cotton.Graphic).Use();
        UO.Wait(500);
        UO.WaitTargetObject(0x40306F2B);
        backpack.AllItems.FindType(Cotton.Graphic).Use();
        UO.Wait(500);
        backpack.AllItems.FindType(Thread.Graphic).Move(1000, containerFrom);
      }
    }

    [Executable]
    public static void MakeKade1()
    {
      UO.Print("Vyber container s matrosem:");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      
      UO.Print("Vyber container na vyrobky:");
      UOItem containerForProduct = new UOItem(UIManager.TargetObject());

      ItemHelper.OpenContainerRecursive(containerFrom);
      ItemHelper.OpenContainerRecursive(World.Player.Backpack);


      var stavesGraphic = (Graphic)0x1EB1;
      var lidGraphic = (Graphic)0x1DB8;
      var logGraphic = (Graphic)0x1BDD;
      var ingotGraphic = (Graphic)0x1BEF;
      var bronzeColor = (UOColor)0x06D6;
      var toolsGraphic = (Graphic)0x1EBC;
      var sawGraphic = (Graphic)0x1034;
      var formaGraphic = (Graphic)0x0E7F;
      var kadGraphic = (Graphic)0x1843;
      var oreGraphic = (Graphic)0x19B9;
      var oreGraphics = new Graphic[] { 0x19B9, 0x19B8, 0x19B7 };

      UOItem backpack = World.Player.Backpack;
      UOItem saw = backpack.AllItems.FindType(sawGraphic);
      UOItem tools = backpack.AllItems.FindType(toolsGraphic);

      if (backpack.AllItems.FindType(Log.Graphic, Log.Color).Exist || backpack.AllItems.FindType(Log.Graphic, Log.Color).Amount < 10)
      {
        containerFrom.AllItems.FindType(Log.Graphic, Log.Color).Move(10, backpack);
        Game.Wait();
      }

      while (saw.Exist && tools.Exist)
      {
        
        if (backpack.AllItems.FindType(Log.Graphic, Log.Color).Exist || backpack.AllItems.FindType(Log.Graphic, Log.Color).Amount < 2)
        {
            backpack.AllItems.FindType(Log.Graphic, Log.Color).Move(1000, containerFrom);
            Game.Wait();
            containerFrom.AllItems.FindType(Log.Graphic, Log.Color).Move(20, backpack);
            Game.Wait();
        }
      
        if (!containerFrom.AllItems.FindType(logGraphic).Exist || !containerFrom.AllItems.FindType(ingotGraphic, bronzeColor).Exist || !containerFrom.AllItems.FindType(oreGraphic).Exist) 
        {
          UO.Print("Dosel matros!");
          return;
        }


        UOItem forma = backpack.AllItems.FindType(formaGraphic);
        UO.Print("Staves: " + backpack.AllItems.Count(stavesGraphic) + "Forma: " + forma.Exist);
         
        while (!forma.Exist)
        {
          UO.Print("Staves: " + backpack.AllItems.Count(stavesGraphic));
          while (backpack.AllItems.Count(stavesGraphic) < 2)
          {
            if (containerFrom.AllItems.FindType(stavesGraphic).Exist && containerFrom.AllItems.Count(stavesGraphic) > 1)
            {
              containerFrom.AllItems.FindType(stavesGraphic).Move(2, backpack);
            }
            else
            {
              containerFrom.AllItems.FindType(Log.Graphic, Log.Color).Move(3, backpack);
              Game.Wait();

              Journal.Clear();
              UO.WaitMenu("Carpentry", "Containers & Cont. parts", "Containers & Cont. parts", "Barrel Staves");
              saw.Use();

              new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

              if (Journal.Contains(true, "targeting"))
              {
                UO.Print("Dosel matros Stavesy!");
                return;
              }
            }
          }
          
          while (backpack.AllItems.Count(lidGraphic) < 2)
          {
            if (containerFrom.AllItems.FindType(lidGraphic).Exist && containerFrom.AllItems.Count(lidGraphic) > 1)
            {
              containerFrom.AllItems.FindType(lidGraphic).Move(2, backpack);
            }
            else
            {
                while (backpack.AllItems.Count(Board.Graphic) < 2)
                {
                    if (containerFrom.AllItems.FindType(Board.Graphic).Exist && containerFrom.AllItems.Count(Board.Graphic) > 1)
                    {
                        containerFrom.AllItems.FindType(Board.Graphic).Move(2, backpack);
                    }
                    else
                    {
                        while (backpack.AllItems.Count(Log.Graphic) < 2)
                        {
                            if (containerFrom.AllItems.FindType(Log.Graphic, Log.Color).Exist && containerFrom.AllItems.Count(Log.Graphic, Log.Color) > 1)
                            {
                                containerFrom.AllItems.FindType(Log.Graphic, Log.Color).Move(2, backpack);
                            }
                        }
                        Game.Wait();
                        Journal.Clear();
                        UO.WaitMenu("Carpentry", "Miscellaneous", "Miscellaneous", "Boards");
                        saw.Use();
                        new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);
                        if (Journal.Contains(true, "targeting"))
                        {
                           UO.Print("Dosel matros boardy!");
                           return;
                        }
                    }
                }            
                Game.Wait();
                Journal.Clear();
                UO.WaitMenu("Carpentry", "Containers & Cont. parts", "Containers & Cont. parts", "Barrel Lid");
                saw.Use();
                new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);
                if (Journal.Contains(true, "targeting"))
                {
                    UO.Print("Dosel matros Barel lids!");
                    return;
                }
            }
            
            
          }
          
          Game.Wait();

          Journal.Clear();
          UO.WaitMenu("Carpentry", "Containers & Cont. parts", "Containers & Cont. parts", "Forma na lahve");
          saw.Use();

          new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

          if (Journal.Contains(true, "targeting"))
          {
            UO.Print("Dosel matros Forma!");
            return;
          }

          forma = backpack.AllItems.FindType(formaGraphic);
        }


        containerFrom.AllItems.FindType(oreGraphic).Move(2, backpack);
        Game.Wait();

        containerFrom.AllItems.FindType(logGraphic,Log.Color).Move(2, backpack);
        Game.Wait();

        if (!backpack.AllItems.FindType(ingotGraphic, bronzeColor).Exist)
        {
          containerFrom.AllItems.FindType(ingotGraphic, bronzeColor).Move(2, backpack);
          Game.Wait();
        }


        Journal.Clear();
        UO.WaitMenu("Tinkering", "Containers", "Containers", "Kad na potiony");
        tools.Use();

        new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

        if (Journal.Contains(true, "targeting"))
        {
          UO.Print("Dosel matros Kad!");
          return;
        }

        if (backpack.AllItems.FindType(kadGraphic, 0x0000).Exist)
        {
          UOItem kade = containerFrom.AllItems.FindType(kadGraphic, 0x0000);
          if (kade.Exist)
          {
            backpack.AllItems.FindType(kadGraphic, 0x0000).Move(1, kade.Container, kade.X, kade.Y);
          }
          else
          {
            backpack.AllItems.FindType(kadGraphic, 0x0000).Move(1, containerForProduct);
          }
        }
      }
    }


    [Executable]
    public static void MakeKrabiceKade()
    {
      UO.Print("Vyber container s matrosem:");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      ItemHelper.OpenContainerRecursive(containerFrom);
      ItemHelper.OpenContainerRecursive(World.Player.Backpack);


      var stavesGraphic = (Graphic)0x1EB1;
      var lidGraphic = (Graphic)0x1DB8;
      var logGraphic = (Graphic)0x1BDD;
      var ingotGraphic = (Graphic)0x1BEF;
      var bronzeColor = (UOColor)0x06D6;
      var toolsGraphic = (Graphic)0x1EBC;
      var sawGraphic = (Graphic)0x1034;
      var formaGraphic = (Graphic)0x0E7F;
      var kadGraphic = (Graphic)0x1843;
      var oreGraphic = (Graphic)0x19B9;
      var oreGraphics = new Graphic[] { 0x19B9, 0x19B8, 0x19B7 };

      UOItem backpack = World.Player.Backpack;
      UOItem saw = backpack.AllItems.FindType(sawGraphic);
      UOItem tools = backpack.AllItems.FindType(toolsGraphic);

      if (backpack.AllItems.FindType(logGraphic).Exist || backpack.AllItems.FindType(logGraphic).Amount < 10)
      {
        containerFrom.AllItems.FindType(logGraphic,Log.Color).Move(10, backpack);
        Game.Wait();
      }

      while (saw.Exist && tools.Exist)
      {
        if (!containerFrom.AllItems.FindType(logGraphic).Exist || !containerFrom.AllItems.FindType(ingotGraphic, bronzeColor).Exist || !containerFrom.AllItems.FindType(lidGraphic).Exist || !containerFrom.AllItems.FindType(oreGraphic).Exist)
        {
          UO.Print("Dosel matros!");
          return;
        }

        while (World.Player.Backpack.AllItems.Count(kadGraphic, 0x0000) < 20)
        {
          if (containerFrom.AllItems.Count(kadGraphic, 0x0000) >= 20)
          {
            while (World.Player.Backpack.AllItems.Count(kadGraphic, 0x0000) < 20)
            {
              containerFrom.AllItems.FindType(kadGraphic).Move(1, World.Player.Backpack);
              Game.Wait(500);
            }

          }
          else
          {
            UOItem forma = backpack.AllItems.FindType(formaGraphic);
            UO.Print("Staves: " + backpack.AllItems.Count(stavesGraphic) + "Forma: " + forma.Exist);

            while (!forma.Exist)
            {
              UO.Print("Staves: " + backpack.AllItems.Count(stavesGraphic));
              while (backpack.AllItems.Count(stavesGraphic) < 2)
              {
                if (containerFrom.AllItems.FindType(stavesGraphic).Exist && containerFrom.AllItems.Count(stavesGraphic) > 1)
                {
                  containerFrom.AllItems.FindType(stavesGraphic).Move(2, backpack);
                }
                else
                {
                  containerFrom.AllItems.FindType(logGraphic,Log.Color).Move(3, backpack);
                  Game.Wait();

                  Journal.Clear();
                  UO.WaitMenu("Carpentry", "Containers & Cont. parts", "Containers & Cont. parts", "Barrel Staves");
                  saw.Use();

                  new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

                  if (Journal.Contains(true, "targeting"))
                  {
                    UO.Print("Dosel matros Stavesy!");
                    return;
                  }
                }
              }

              containerFrom.AllItems.FindType(lidGraphic).Move(2, backpack);
              Game.Wait();

              Journal.Clear();
              UO.WaitMenu("Carpentry", "Containers & Cont. parts", "Containers & Cont. parts", "Forma na lahve");
              saw.Use();

              new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

              if (Journal.Contains(true, "targeting"))
              {
                UO.Print("Dosel matros Forma!");
                return;
              }

              forma = backpack.AllItems.FindType(formaGraphic);
            }


            containerFrom.AllItems.FindType(oreGraphic).Move(2, backpack);
            Game.Wait();

            containerFrom.AllItems.FindType(logGraphic,Log.Color).Move(2, backpack);
            Game.Wait();

            if (!backpack.AllItems.FindType(ingotGraphic, bronzeColor).Exist)
            {
              containerFrom.AllItems.FindType(ingotGraphic, bronzeColor).Move(2, backpack);
              Game.Wait();
            }


            Journal.Clear();
            UO.WaitMenu("Tinkering", "Containers", "Containers", "Kad na potiony");
            tools.Use();

            new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

            if (Journal.Contains(true, "targeting"))
            {
              UO.Print("Dosel matros Kad!");
              return;
            }

          }
        }
        

        if (World.Player.Backpack.Items.Count(logGraphic) < 10)
        {
          containerFrom.AllItems.FindType(logGraphic,Log.Color).Move(10, backpack);
        }

        Journal.Clear();
        UO.WaitMenu("Carpentry", "Miscellaneous", "Miscellaneous", "Krabice kadi");
        saw.Use();

        new JournalEventWaiter(true, "fail", "you put", "targeting").Wait(10000);

        if (World.Player.Backpack.AllItems.FindType(0x185E, 0x07E0).Exist)
        {
          UOItem kade = containerFrom.AllItems.FindType(0x185E, 0x07E0);
          if (kade.Exist)
          {
            backpack.AllItems.FindType(0x185E, 0x07E0).Move(1, kade.Container, kade.X, kade.Y);
          }
          else
          {
            backpack.AllItems.FindType(0x185E, 0x07E0).Move(1, containerFrom);
          }
        }

      }
    }

    //---------------------------------------------------------------------------------------------

    public void MakeLocky(int amount)
    {
      UO.Print(0x456,"Cont z materialem >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      UO.Print(0x456,"Cont kam >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerFrom.AllItems.FindType(0x1BEF, 0x06D6).Move(50, World.Player.Backpack);
        Game.Wait();

        UO.UseType(0x1EBC, 0x0000);
        UO.WaitMenu("Tinkering", "Tools", "Tools", "50x Lockpick");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put", "Tinkering failed"))
        {
          if (Journal.Contains("You put"))
          {
            count += 50;
            World.Player.Backpack.AllItems.FindType(0x14FB, 0x0000).Move(50, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            UO.Print(0x456,"Nemas suroviny");
            break;
          }
        }

        UO.Print(0x456,"Vyrobeno: " + count);

      }

      UO.Print(0x456,"MakeLocky - End");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [Executable("MakeLocky")]
    public static void ExecMakeLocky(int amount)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Tinkering>().MakeLocky(amount);
    }

    [Executable]
    public static void TrainTinkering()
    {
      TrainTinkering((int)Mining2.OreColor.Iron, 0x1055, "Tinkering", "Parts", "Parts", "Hinge");
    }

    [Executable]
    public static void TrainTinkeringLocpick()
    {
      TrainTinkering((int)Mining2.IgnotColor.Bronze, 0x14FB, "Tinkering", "Tools", "Tools", "Lockpick");
    }

    [Executable("MakeTink")]
    [BlockMultipleExecutions]
    public static void ExecMake(int quantity, params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Tinkering>().MakeTink(quantity, menus);
    }

    [Executable("MakeWire")]
    [BlockMultipleExecutions]
    public static void ExecMakeWire(int quantity)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Tinkering>().MakeWire(quantity);
    }

    public void MakeTink(int quantity, params string[] menus)
    {
      int itemMake = 0;

      Journal.Clear();

      while (!UO.Dead && itemMake < quantity)
      {

        UO.UseType(0x1EBC, 0x0000);
        UO.WaitMenu(menus);

        Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put", "Tinkering failed");
        if (Journal.Contains("You put"))
          itemMake++;

        if (Journal.Contains("You can't make anything"))
        {
          UO.Print(0x456,"Nemas suroviny");
          break;
        }


        Journal.Clear();
      }
    }


    public void MakeWire(int quantity)
    {
      decimal itemMake = 0m;
      decimal itemFail = 0m;
      
      Journal.Clear();

      UO.Print(0x456,"Jaky >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);
      UO.Print(0x456,"Bagl do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      UOItemType t = new UOItemType();
      t.Graphic = type.Graphic;
      t.Color = type.Color;

      type.Click();
      Game.Wait();

      String name = (type.Name + String.Empty).ToLower().Replace("ingot", "").Trim();
      name = name[0].ToString().ToUpper() + name.Substring(1, name.Length - 1).ToLower();

      UO.Print(0x456,"" + name + " Wire");

      string[] menus = new string[] { "Tinkering", "Wires", "Wires", name + " Wire" };

      while (!UO.Dead && itemMake < quantity)
      {
        if (!World.Player.Backpack.Items.FindType(t.Graphic, t.Color).Exist)
        {
          containerFrom.Items.FindType(t.Graphic, t.Color).Move(100, World.Player.Backpack);
          Game.Wait();

          if (World.Player.Backpack.Items.FindType(0x1876).Exist)
          {
            World.Player.Backpack.Items.FindType(0x1876).Move(10000, containerTo);
            Game.Wait();
          }

          if (World.Player.Backpack.Items.FindType(0x1877).Exist)
          {
            World.Player.Backpack.Items.FindType(0x1877).Move(10000, containerTo);
            Game.Wait();
          }

          if (World.Player.Backpack.Items.FindType(0x1878).Exist)
          {
            World.Player.Backpack.Items.FindType(0x1878).Move(10000, containerTo);
            Game.Wait();
          }

          if (World.Player.Backpack.Items.FindType(0x1879).Exist)
          {
            World.Player.Backpack.Items.FindType(0x1879).Move(10000, containerTo);
            Game.Wait();
          }
        }

        UO.UseType(0x1EBC, 0x0000);
        UO.WaitMenu(menus);

        Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put", "Tinkering failed");
        if (Journal.Contains("You put"))
          itemMake++;
        else
          itemFail++;

        if (Journal.Contains("You can't make anything"))
        {
          UO.Print(0x456,"Nemas suroviny");
          break;
        }


        decimal okDivide = (itemMake / (itemMake + itemFail));
        decimal okPerc = okDivide * 100;

        UO.Print(0x456,"Ks: " + itemMake + "/" + (itemMake + itemFail) + " - " + String.Format("{0:n} %", okPerc));


        Journal.Clear();
      }
    }

    //---------------------------------------------------------------------------------------------

    public static void TrainTinkering(UOColor color, Graphic product ,params string[] menus)
    {
      UO.Print("Vyber container z ingoty:");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      
      if (containerFrom.Exist)
      {
        UO.Print("containerFrom.Exist:");
        UOItem ingotSource = new UOItem(Serial.Invalid);

        UOItem ingot = new UOItem(Serial.Invalid);
        if (!(ingot = Mining2.FindIngot(UO.Backpack.Items, (int)Mining2.OreColor.Iron)).Exist || ingot.Amount < 20)
        {
          ingot = Mining2.FindIngot(containerFrom.Items, (int)Mining2.OreColor.Iron);
          ingot.Move(20, UO.Backpack.Serial);
          Game.Wait();
        }

        UO.Print("while.Exist:" + color);

        while ((ingotSource = Mining2.FindIngot(containerFrom.Items, color)).Exist)
        {
          UO.Print("while.Exist:");
          ingotSource.Move(100, UO.Backpack.Serial);
          Game.Wait();

          while ((ingot = Mining2.FindIngot(UO.Backpack.Items, color)).Exist)
          {
            UO.Print("while2.Exist:");
            JournalEventWaiter jew = new JournalEventWaiter(true, "You put the", "Tinkering failed");
            UO.WaitMenu(menus);
            UOItem tools = UO.Backpack.Items.FindType(TinkersTools);
            if (tools.Exist)
            {
              tools.Use();
              jew.Wait(7500);
              if (Journal.Contains(true, "You can't make anything with what you have"))
              {
                Journal.Clear();
                break;
              }
            }
            else
            {
              UO.Print("Nejsou toolsy.");
              break;
            }
          }

          UOItem hinge = UO.Backpack.Items.FindType(product);
          if (hinge.Exist)
          {
            hinge.Move(1000, containerFrom.Serial);
            Game.Wait();
          }
        }
      }
      UO.Print("Konec");
    }
  }
}
