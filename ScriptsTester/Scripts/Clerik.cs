using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Abilities;
using CalExtension.PlayerRoles;
using CalExtension.Skills;
using CalExtension.UI.Status;
using CalExtension.UOExtensions;
using Phoenix;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.Macros;
using Phoenix.Runtime;
using Phoenix.WorldData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;


namespace CalExtension.Medik
{
    
    public class Clerik
    {
        private UOCharacter[] num = new UOCharacter[6];
        UOItem backPack = new UOItem(0x4028A331);
        UOItem regyDomace = new UOItem(0x4037612B);
                
        public void Cast(string spell, bool self)
        {
            if(World.Player.Mana < 20)
            {
                UO.UseType(0x0F09,0x0003);
                if(World.Player.Backpack.AllItems.Count(0x0F0B,0x0000) < 5)
                {
                    UO.Print(0x0435,"Mal by si capovat!");
                }
            }
            if(self)
            {
                UO.Cast(spell, Aliases.Self);
            }
            else
            {
                UO.Cast(spell);
            }
        }
        [Executable]
        public void hidcler()
        {
            if(World.Player.Layers[Layer.LeftHand].Graphic == 0x0A15)
            {
                UO.MoveItem(World.Player.Layers[Layer.LeftHand].Serial, 1, World.Player.Backpack, 25, 90);
            }
            UO.Say(",exec hide");
            //UO.Wait(1000);
            //UO.UseType(0x0A15);
        }
        
    private static ushort toUShort(string s)
    {
        return ushort.Parse(s.Split('x')[1],System.Globalization.NumberStyles.HexNumber);
    }
    
    [Executable]
    public static int GetKadCount(UOItem kad)
    {        
        UOItemExtInfo info = ItemHelper.GetItemExtInfo(kad);
        UO.Print(0x0435,"Charges:"+info.Charges.Value);
        return info.Charges.Value;
    }
    
    
    [Executable]
    public static void RefniVampa(uint rbs)
    { 
        UOItem rb = new UOItem(rbs);
        if(rb==0)
        {
           UO.Print(0x0435,"Vyber ref backpack");
           rb = new UOItem(UIManager.Target().Serial);   
        }
        
        Refni(rb.Serial, 400, 600, 10, 0, 250, 0, 50, 50, 100, 25, 0, 150, 150);
        
        //sperky
        ItemRequip.RefullSperkyClear(0x40169305, World.Player.Backpack, "Name: Great Reflex Ring, Count: 2, Amount: 2", "Name: Reflex Ring, Count: 2, Amount: 2", "Name: Great Gold Ring, Count: 2, Amount: 2" );
        
        //klamaky
        ItemRequip.RefullKlamakyClear(0x4030965F, rb.Serial, "Name: Cat, Count: 25, X: 120, Y: 5", "Name: Cow, Count: 10, X: 140, Y: 5");
    }
    
    [Executable]
    public static void RefniWh(uint rbs)
    { 
        UOItem rb = new UOItem(rbs);
        if(rb==0)
        {
           UO.Print(0x0435,"Vyber ref backpack");
           rb = new UOItem(UIManager.Target().Serial);   
        }
        //(rbs,rcount,cbanda,bbanda,gh,gb,gc,lava,shrink,night,tmr,tr,gs)
        Refni(rb.Serial, 400, 600, 0, 200, 0, 50, 50, 50, 100, 0, 50, 150, 150);
        
        //sperky
        ItemRequip.RefullSperkyClear(0x40169305, World.Player.Backpack, "Name: Great Reflex Ring, Count: 2, Amount: 2", "Name: Reflex Ring, Count: 2, Amount: 2", "Name: Great Gold Ring, Count: 2, Amount: 2" );
        
        //klamaky
        ItemRequip.RefullKlamakyClear(0x4030965F, rb.Serial, "Name: Cat, Count: 20, X: 120, Y: 5", "Name: Cow, Count: 10, X: 140, Y: 5");
    }
    
    [Executable]
    public static void RefniClera(uint rbs)
    { 
        UOItem rb = new UOItem(rbs);
        if(rb==0)
        {
           UO.Print(0x0435,"Vyber ref backpack");
           rb = new UOItem(UIManager.Target().Serial);   
        }
        
        Refni(rb.Serial, 1500, 1200, 200, 150, 0, 0, 0, 50, 100, 250, 0, 150, 150);
        
        //sperky
        ItemRequip.RefullSperkyClear(0x40169305, World.Player.Backpack, "Name: Great Reflex Ring, Count: 2, Amount: 2", "Name: Reflex Ring, Count: 2, Amount: 2" );
        
        //klamaky
        ItemRequip.RefullKlamakyClear(0x4030965F, rb.Serial, "Name: Cat, Count: 25, X: 120, Y: 5");
    }
    
    [Executable]
    public static void RefniRanga(uint rbs)
    { 
        UOItem rb = new UOItem(rbs);
        if(rb==0)
        {
           UO.Print(0x0435,"Vyber ref backpack");
           rb = new UOItem(UIManager.Target().Serial);   
        }
        
        Refni(rb.Serial, 600, 500, 0, 100, 0, 20, 0, 100, 100, 100, 0, 150, 150);
        
        //sperky
        ItemRequip.RefullSperkyClear(0x40169305, World.Player.Backpack, "Name: Great Reflex Ring, Count: 2, Amount: 2", "Name: Reflex Ring, Count: 2, Amount: 2" );
        
        //klamaky
        ItemRequip.RefullKlamakyClear(0x4030965F, rb.Serial, "Name: Cat, Count: 15, X: 120, Y: 5", "Name: Cow, Count: 10, X: 140, Y: 5");
    }
    
    [Executable]
    public static void RefniNecra(uint rbs)
    { 
        UOItem rb = new UOItem(rbs);
        if(rb==0)
        {
           UO.Print(0x0435,"Vyber ref backpack");
           rb = new UOItem(UIManager.Target().Serial);   
        }
        
        Refni(rb.Serial, 2500, 200, 0, 100, 0, 0, 0, 0, 300, 150, 0, 150, 0);
        
        //sperky
        //ItemRequip.RefullSperkyClear(0x40169305, World.Player.Backpack, "Name: Great Reflex Ring, Count: 2, Amount: 2", "Name: Reflex Ring, Count: 2, Amount: 2", "Name: Great Gold Ring, Count: 2, Amount: 2" );
        
        //klamaky
        //ItemRequip.RefullKlamakyClear(0x4030965F, rb.Serial, "Name: Cat, Count: 25, X: 120, Y: 5");
    }
    
    
    public static void Refni(uint rbs, int rcount, ushort cbanda, ushort bbanda, int gh, int gb, int gc, int lava, int shrink, int night, int tmr, int mr, int tr, int gs)
    {   
        ushort bandaCleanCount = cbanda;
        ushort bandaBloodCount = bbanda;
        UOItem mainBedna = new UOItem(0x40314338);
        mainBedna.Use();
        UO.Wait(500);
        UOItem rb = new UOItem(rbs);
        if(rb==0)
        {
           UO.Print(0x0435,"Vyber ref backpack");
           rb = new UOItem(UIManager.Target().Serial);   
        }
        
        //regy TODO NECRO
        UO.Print(0x0435,"Regy...");
        foreach(UOItem item in World.Player.Backpack.AllItems)
        {
            SortToReg(item);
        }        
        RefReg(rcount, rb.Serial);
        UO.Print(0x0435,"Regy done.");
        
        //potiony
        UO.Print(0x0435,"Potiony...");
        Dictionary<string,string> ptkMap = new Dictionary<string,string>();
        ptkMap.Add("0x0F09:0x0B77","0x1843:0x0B77"); //invis
        ptkMap.Add("0x0F0D:0x000E","0x1843:0x000E"); //lava
        ptkMap.Add("0x0F07:0x0000","0x1843:0x0842"); //GC
        ptkMap.Add("0x0F09:0x045E","0x1843:0x0724"); //Shrink
        ptkMap.Add("0x0F06:0x0000","0x1843:0x03C4"); //Night
        ptkMap.Add("0x0F0A:0x0000","0x1843:0x089F"); //Lesser Poison
        ptkMap.Add("0x0F0C:0x0000","0x1843:0x08A7"); //GH
        ptkMap.Add("0x0F09:0x0003","0x1843:0x0003"); //TMR
        ptkMap.Add("0x0F09:0x0005","0x1843:0x0005"); //MR
        ptkMap.Add("0x0F0B:0x0000","0x1843:0x014D"); //TR
        ptkMap.Add("0x0F09:0x0000","0x1843:0x0481"); //GS
        ptkMap.Add("0x0F08:0x0000","0x1843:0x00BD"); //GA
        ptkMap.Add("0x0F0C:0x0025","0x1843:0x0025"); //GB
        
        //vylej co mas u seba
        foreach(UOItem item in World.Player.Backpack.AllItems)
        {
            string data = item.Graphic+":"+item.Color;
            if(ptkMap.Keys.Contains(data))
            {
                ushort g = toUShort(ptkMap[data].Split(':')[0]);
                ushort c = toUShort(ptkMap[data].Split(':')[1]);
                
                UOItem kad = World.Ground.FindType(g,c);
                UO.WaitTargetObject(item);
                kad.Use();
                UO.Wait(500);
            }
        }
        
        //ref lahve
        foreach(UOItem lahev in World.Player.Backpack.AllItems.Where(x => x.Graphic==0x0F0E && x.Color==0x0000))
        {
            UO.Print(0x0435,"Vraciam lahvicky");
            lahev.Move(65000, mainBedna.Serial);
            UO.Wait(500);
        }
        mainBedna.AllItems.FindType(0x0F0E, 0x0000).Move(5, World.Player.Backpack.Serial);
        UO.Wait(500);
        
        RefPotion(rb, gs,0x1843,0x0481,0x0F09,0x0000, 10, false);//GS
        RefPotion(rb, gh,0x1843,0x08A7,0x0F0C,0x0000, 25, false);//GH
        RefPotion(rb, gb,0x1843,0x0025,0x0F0C,0x0025, 25, false);//GB
        RefPotion(rb, tr,0x1843,0x014D,0x0F0B,0x0000, 40, false);//TR
        RefPotion(rb, tmr,0x1843,0x0003,0x0F09,0x0003, 55, false);//TMR
        RefPotion(rb, mr,0x1843,0x0005,0x0F09,0x0005, 55, false);//MR
        RefPotion(rb, shrink,0x1843,0x0724,0x0F09,0x045E, 70, false);//Shrink
        RefPotion(rb, night,0x1843,0x03C4,0x0F06,0x0000, 85, false);//Night
        RefPotion(rb, lava,0x1843,0x000E,0x0F0D,0x000E, 100, false);//lava
        RefPotion(rb, gc,0x1843,0x0842,0x0F07,0x0000, 115, false);//GC

        UO.Print(0x0435,"Potiony done.");
        
        //bandy wasin->0x1008
        UOItem basin = World.Player.Backpack.AllItems.FindType(0x1008,0x0000);
        if(basin.Exist)
        {
            basin.Move(1,mainBedna.Serial);
        }
        foreach(UOItem banda in World.Player.Backpack.AllItems.Where(x => 
        (x.Graphic==0x0E20 && x.Color==0x0000) ||
        (x.Graphic==0x0E21 && x.Color==0x0000) ||
        (x.Graphic==0x0E22 && x.Color==0x0000)))
        {
            UO.Print(0x0435,"Vraciam bandy");
            banda.Move(65000, mainBedna.Serial);
            UO.Wait(500);
        }
        mainBedna.AllItems.FindType(0x0E21, 0x0000).Move(bandaCleanCount,  World.Player.Backpack.Serial);
        UO.Wait(500);
        mainBedna.AllItems.FindType(0x0E20, 0x0000).Move(bandaBloodCount,  World.Player.Backpack.Serial);
        UO.Wait(500);
        mainBedna.AllItems.FindType(0x0E22, 0x0000).Move(bandaBloodCount,  World.Player.Backpack.Serial);
        UO.Wait(500);
        if(basin.Exist)
        {            
            UO.MoveItem(basin, 0, rb, 70, 20 );
        }
       
        
        
        //salaty
        
        //invis
        //ryby
        //scrolly
        //nastroje
        //pracka
        //mystick
        //special -> kpz...
        
        //prazdny pytlik
               
        UO.Print(0x0435,"ALL DONE!");
    }
    

    private static void RefPotion(UOItem moje, int targetCount, ushort kg, ushort kc, ushort pg, ushort pc, ushort x, bool exact)
    { 
        UOItem baglKade = new UOItem(0x4001B9C9);
        if(targetCount==0)
        {
            return;
        }
        UOItem kadBatoh = World.Player.Backpack.AllItems.FindType(kg, kc);
        UOItem kadZem = World.Ground.FindType(kg, kc);
        
        if(!kadBatoh.Exist)
        {
           baglKade.Use();
           UO.Wait(750);  
           UOItem novaKad = baglKade.AllItems.FindType(0x1843, 0x0000);
           novaKad.Move(1, World.Player.Backpack);
           
           UO.Wait(750);                               
           UO.WaitTargetObject(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000));
           kadZem.Use();
           UO.Wait(750);
                    
           UO.WaitTargetObject(World.Player.Backpack.AllItems.FindType(pg, pc));
           novaKad.Use();
           UO.Wait(750);
           
           kadBatoh = World.Player.Backpack.AllItems.FindType(kg, kc);           
        }
        
        
                
        if(GetKadCount(kadBatoh)<targetCount)
        {            
            int delta = targetCount - GetKadCount(kadBatoh);
            if(delta<0 && exact)
            {
                delta = delta * -1;
                int c = (delta / 50)+1;
                for(int i=0;i<c;i++)
                {
                    UO.WaitTargetObject(kadZem);
                    kadBatoh.Use();
                    UO.Wait(500);
                }
                delta = targetCount - GetKadCount(kadBatoh);
            }
            
            if(delta>=GetKadCount(kadZem))
            {
                throw new Exception("Not enought potions!");
            }
            if(delta>0)
            {
                for(int i=delta;i>=50;i=i-50)
                {
                    UO.Print(0x0435,"Delta: "+delta);
                    UO.WaitTargetObject(kadBatoh);
                    kadZem.Use();
                    UO.Wait(500);
                    delta =- 50;
                }
                while(delta > 0)
                {
                    UO.Print(0x0435,"Delta: "+delta);
                    UO.WaitTargetObject(World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000));
                    kadZem.Use();
                    UO.Wait(1000);
                    
                    UO.WaitTargetObject(World.Player.Backpack.AllItems.FindType(pg, pc));
                    UO.Wait(750);
                    kadBatoh.Use();
                    UO.Wait(750);
                    delta = delta - 1; 
                }
            }
        }
        UO.Wait(100);    
        UO.MoveItem(kadBatoh, 0, moje, x, 5);
        UO.Wait(100);  
    }
    
    [Executable]
    public static void kadtokad()
    {
      UO.Print(0x0435,"Vyber zdrojovou kad:");
      UOItem kadFrom = new UOItem(UIManager.Target().Serial);

      UO.Print(0x0435,"Vyber cilovou kad:");
      UOItem kadDo = new UOItem(UIManager.Target().Serial);
      
      
      while(!Journal.Contains(true, "Tohle nejde!"))
      {
        UO.WaitTargetObject(kadDo);
        kadFrom.Use();
        UO.Wait(500);
      }
      UOItem flaska = World.Player.Backpack.Items.FindType(0x0F0E, 0x0000);
      if(kadFrom.Exist && flaska.Exist)
      {
        List<Serial> origItems = new List<Serial>();
        foreach (UOItem oi in World.Player.Backpack.Items)
        {
            origItems.Add(oi.Serial);
            //Game.CurrentGame.Messages.Add(oi.Serial+"");
        }
        
        while(kadFrom.Exist)
        {
            UO.WaitTargetObject(flaska);    
            kadFrom.Use();
            flaska = World.Player.Backpack.Items.FindType(0x0F0E, 0x0000);
             
            UO.Wait(1500);
            //Game.CurrentGame.Messages.Add("-----------------------------");
            foreach (UOItem lastItem in World.Player.Backpack.Items)
            {   
                //Game.CurrentGame.Messages.Add(lastItem.Serial+"");
                if (!origItems.Contains(lastItem.Serial)&&lastItem.Graphic!=0x0F0E)
                {   UO.WaitTargetObject(lastItem);                    
                    kadDo.Use();
                    
                    UO.Wait(1500);
                    
                }
            }
         }
      }
      
    }

        
        [Executable]
        public void moveall()
        {        
            UO.Print("From ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            UO.Print("to ?");
            UOItem to = new UOItem(UIManager.TargetObject());
            
            foreach(UOItem item in from.AllItems)
            {
                item.Move(65000,to);
            }
        }
        
        [Executable]
        public void Mystics()
        {
            int Leaf=0;
            int Flower=0;
            int Stone=0;
            int Plant=0;
            int Beeds=0;
            int Ball=0;
            int Stick=0;
            int Mushroom=0;
            UO.Print("From ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            foreach(UOItem Item in from.AllItems)
            {
                if(Item.Color==0x0B9F && Item.Graphic==0x0DBD) {Leaf++;}
                if(Item.Color==0x005B && Item.Graphic==0x0DC3) {Flower++;}
                if(Item.Color==0x0B94 && Item.Graphic==0x136C) {Stone++;} 
                if(Item.Color==0x0899 && Item.Graphic==0x0CB0) {Plant++;} 
                if(Item.Color==0x0BB5 && Item.Graphic==0x108B) {Beeds++;} 
                if(Item.Color==0x0B9F && Item.Graphic==0x0E73) {Ball++;} 
                if(Item.Color==0x0481 && Item.Graphic==0x1A9D) {Stick++;} 
                if(Item.Color==0x00A3 && Item.Graphic==0x0D16) {Mushroom++;}
            }
            UO.Print("Leaf: " +Leaf);
            UO.Print("Flower: " +Flower);
            UO.Print("Stone: " +Stone);
            UO.Print("Plant: " +Plant);
            UO.Print("Beeds: " +Beeds);
            UO.Print("Ball: " +Ball);
            UO.Print("Stick: " +Stick);
            UO.Print("Mushroom: " +Mushroom);
            
            
        }        
        [Executable]
        public void movemystics()
        {
            UO.Print("From ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            UO.Print("To ?");
            UOItem to = new UOItem(UIManager.TargetObject());
            foreach(UOItem item in from.AllItems)
            {
                if(IsMystic(item))
                {
                    item.Move(65000, to);
                    UO.Wait(500);
                }
            }
            UO.Print("Done");
        }
        
        private bool IsMystic(UOItem item)
        {
            bool retVal = false;
            if((item.Color==0x0B9F && item.Graphic==0x0DBD) 
                ||(item.Color==0x005B && item.Graphic==0x0DC3) 
                ||(item.Color==0x0B94 && item.Graphic==0x136C) 
                ||(item.Color==0x0899 && item.Graphic==0x0CB0) 
                ||(item.Color==0x0BB5 && item.Graphic==0x108B) 
                ||(item.Color==0x0B9F && item.Graphic==0x0E73) 
                ||(item.Color==0x0481 && item.Graphic==0x1A9D)  
                ||(item.Color==0x00A3 && item.Graphic==0x0D16))
            {
                retVal = true;
            }
            return retVal;
        }
        
        
        [Executable]
        public void rozpusti()
        {
            //
            UO.Print("Kde je bordel ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            UOItem trainer = World.Ground.FindType(0x0E32, 0x0B82);
            
            foreach(UOItem item in from.AllItems)
            {
                item.Move(1,World.Player.Backpack);
                UO.Wait(1000);
                trainer.Use();
                UO.WaitTargetObject(item);
                UO.Wait(1000);
                
            }
        }
        
        [Executable]
        public void VylozLoot()
        {
            UO.Print("Kde je loot ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            UO.Print("Kam regy ?");
            UOItem regto = new UOItem(UIManager.TargetObject());
            
            Robot r = new Robot();
            //chod na zaciatok
            r.GoTo(UOPositionBase.Parse("3170.23"));
            
            foreach(UOItem item in from.AllItems)
            {
                UO.MoveItem(item, (ushort)10000, (ushort)(World.Player.X), (ushort)(World.Player.Y), World.Player.Z);
                UO.Wait(500);
                r.GoTo(GetNextPosition((ushort)(World.Player.X), (ushort)(World.Player.Y)));                
            }
        }
        
        [Executable]
        public void citaj()
        {
            UO.Print("Kde su knihy?");
            UOItem from = new UOItem(UIManager.TargetObject());
                        
            foreach(UOItem item in from.AllItems)
            {
                while(item != null && item.Exist)
                {
                    item.Use();
                    UO.Wait(750);
                }
                
            }
        }
        
        [Executable]
        public void vampCrystal()
        {
            UO.Print("Kde su knihy?");
            UOItem from = new UOItem(UIManager.TargetObject());
            foreach(UOItem item in from.AllItems)
            {
                while(
                    item != null && 
                    item.Exist && 
                    item.Graphic==0x1BEF  && 
                    item.Color == 0x0481  )
                {
                    item.Move(1, World.Player.Backpack.Serial);
                    UO.Wait(500);
                    item.Use();
                    UO.Wait(500);
                }
                
            }
        
        }
        
        private static IUOPosition GetNextPosition(ushort x, ushort y)
        {
            x++;
            if(x>=3177)
            {
                x=(ushort)3170;
                y++;
            }
            if(y>29)
            {
                UO.Say("Mala strecha");
            }
            return UOPositionBase.Parse(x+"."+y);
        }
                
        [Executable]
        public void Transport()
        {
        UO.Say("1");
            Robot r = new Robot();
            List<String> path = new List<String>();
            path.Add("1385.3849");
            path.Add("1389.3854");
            path.Add("0x40317C51");
            path.Add("1389.3856");
            path.Add("0x40317C51");
            path.Add("1440.3866");
            path.Add("0x40125505");
            path.Add("1438.3866");
            path.Add("0x40125505");
            path.Add("1432.3871");
            path.Add("3172.26"); 
            path.Add("1372.2741"); 
            path.Add("0x40125505");
            path.Add("1372.2743"); 
            path.Add("0x40125505");
            foreach(String sektor in path)
            {
            UO.Wait(1000);
            UO.Say("2 -> " + path.Count);
                if(sektor.Contains("0x"))
                {
                UO.Say("3");
                    UO.Say(",UseObject "+sektor);
                }
                else
                {
                UO.Say("4");
                    r.GoTo(UOPositionBase.Parse(sektor));
                }
            }
            
            
        }
        
        [Executable]
        public void moveore()
        {
            UO.Print("Odkial ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            UO.Print("Kam ?");
            UOItem to = new UOItem(UIManager.TargetObject());
            foreach(UOItem item in from.AllItems)
            {
                while(from.AllItems.Count(item.Graphic, item.Color) >1)
                {
                    item.Move(50, to.Serial); 
                    UO.Wait(500);
                }
            }
        }
        
        
        [Executable]
        public void ManaCheck()
        {
            if(World.Player.Mana < 25){
                UO.Say(",exec DrinkPotion \"Total Mana Refresh\"");
            }
        }
       
        [Executable]
        public void BoostSelf()
        {
            Cast("Bless", true);
        }
        
        [Executable]
        public void Protection()
        {
            Cast("Protection", false);
        }
        
        [Executable]
        public void Luskaj()
        {
            UO.Print("Kde su flasky ?");
            UOItem bottlesContainer = new UOItem(UIManager.TargetObject());            
            UOItem lvl1 = new UOItem(0x4007E74A);  
            UOItem lvl2 = new UOItem(0x40165276);  
            UOItem lvl3 = new UOItem(0x40318A7F);  
            UOItem other = new UOItem(0x4009B110);  
            while(bottlesContainer.AllItems.Count(0x099B,0x08A4) > 0)
            {            
                UOItem bottle = World.Player.Backpack.AllItems.FindType(0x099B,0x08A4);
                bottle.Move(1, World.Player.Backpack.Serial);                
                bottle.Use();
                UO.Wait(1500);
                while(World.Player.Backpack.AllItems.Count(0x14EB,0x0000) > 0)
                {
                    UOItem map = World.Player.Backpack.AllItems.FindType(0x14EB,0x0000);
                    UO.DeleteJournal();
                    map.Use();
                    UO.Wait(2000);
                    
                    string[] asdas = { "Level 1", "Level 2", "Level 3", "Nejsi" };
                    Journal.WaitForText(asdas); 
                    
                    if (UO.InJournal("Level 1"))
                    {
                      map.Move(1, lvl1.Serial);  
                    }
                    else if (UO.InJournal("Level 2"))
                    {
                      map.Move(1, lvl2.Serial);  
                    }
                    else if (UO.InJournal("Level 3"))
                    {
                      map.Move(1, lvl3.Serial);  
                    }
                    else
                    {
                      map.Move(1, other.Serial);  
                    }
                    
                }
                
            }
            
        }
        
        [Executable]
        public void Gp(int members)
        {//0x0B81
            UO.Print("Vyber container na prepocitavanie:");
            UOItem baglik = new UOItem(UIManager.TargetObject());
      UO.Press(System.Windows.Forms.Keys.Home); 
            int amount = World.Player.Backpack.AllItems.Count(0x0EED,0x0000  );
            UO.Say(0x0435, "Loot je: "+amount+" GP");
            UO.Wait(2000);
                
            amount = World.Player.Backpack.AllItems.Count(0x0EED,0x0000  );
            int part = amount / members;
            UO.Say(0x0435, "Kazdy dostane: "+part+" GP");
            UO.Wait(2000);
                       
            while(members>0)
            {
                UOItem coins = World.Player.Backpack.AllItems.FindType(0x0EED,0x0000  );            
                if(coins.Amount >= part)
                {
                    UO.MoveItem(coins, (ushort)part, (ushort)(World.Player.X), (ushort)(World.Player.Y), World.Player.Z); 
                    members--;
                    UO.Press(System.Windows.Forms.Keys.Home); 
                }
                else
                {
                    coins.Move((ushort)part, baglik.Serial);    
                }
                UO.Wait(1000);
            }            
        }
        
        [Executable]
        public void Gpg(int members)
        {
            UO.Press(System.Windows.Forms.Keys.Home); 
            int amount = World.Player.Backpack.AllItems.Count(0x0EED,0x0000);
            UO.Say(0x0435, "Loot je: "+amount+" GP");
            UO.Wait(2000);
            
            int guildpart = amount / 10;
            UO.Say(0x0435, "Podiel pre guildu: "+guildpart+" GP");
            UO.Wait(2000);
            
            UOItem coinsf = World.Player.Backpack.AllItems.FindType(0x0EED,0x0000);            
            UO.MoveItem(coinsf, (ushort)guildpart, World.Player.X, World.Player.Y, World.Player.Z);
            UO.Press(System.Windows.Forms.Keys.Home); 
            UO.Press(System.Windows.Forms.Keys.Home);             
                        
            amount = World.Player.Backpack.AllItems.Count(0x0EED,0x0000);
            int part = amount / members;
            UO.Say(0x0435, "Kazdy dostane: "+part+" GP");
            UO.Wait(2000);
            
            for(int i=0; i<members; i++)
            {
                UOItem coins = World.Player.Backpack.AllItems.FindType(0x0EED,0x0000);            
                UO.MoveItem(coins, (ushort)part, World.Player.X, World.Player.Y, World.Player.Z);
                UO.Press(System.Windows.Forms.Keys.Home); 
                UO.Wait(500);
            }
        }
        
        [Executable]
        public void Hid()
        {
            if (World.Player.Layers[Layer.LeftHand].Exist && World.Player.Layers[Layer.LeftHand].Serial == 0x400C89B3)
		    {
			    UO.MoveItem(World.Player.Layers[Layer.LeftHand].Serial, 1, World.Player.Backpack, 25, 90);
		    }
            while(!UO.InJournal("You have hidden"))
                           {                               	
                             UO.DeleteJournal();
         	                 UO.UseSkill("Hiding");            	                 
         	                 UO.Wait(2500);       
                           }
        }
        
        [Executable]
        public void Banda()
        {
            UO.UseType(0x0E21);
        }
        
        [Executable]
        public void HarmZalozka()
        {
            UO.Print(0x0435, "Vyber pacienta");            
            UOCharacter pacient = new UOCharacter(UIManager.TargetObject());
            while(true)
            {
                if(World.Player.Mana < 10)
                {
                    UO.UseType(0x0F09,0x0003);
                    if(World.Player.Backpack.AllItems.Count(0x0F0B,0x0000) < 10)
                    {
                        UO.Print(0x0435,"Mal by si capovat!");
                    }
                }
                UO.Cast("Harm", pacient);            
                UO.Wait(2000);  
            }
        }
        
        [Executable]
        public void bzq()
        {
            UO.Print(0x0435, "Vyber pacienta");            
            UOCharacter pacient = new UOCharacter(UIManager.TargetObject());
            UO.Cast("Agility", pacient);            
            UO.Wait(2000);
            UO.Cast("Reactive Armor", pacient);
            UO.Wait(2000);
            while(true)
            {
                if(World.Player.Mana < 45)
                {
                    UO.Say(",exec DrinkPotion \"Total Mana Refresh\"");
                    //UO.UseType(0x0F09,0x0003);
                }
                                
                if (UO.InJournal("Ex Corp Jux Ylem"))
                {
                    UO.DeleteJournal();                
                    UO.Cast("Agility", pacient);            
                    if (UO.InJournal("The spell fizzles"))
                    {
                        UO.Cast("Agility", pacient);            
                    }
                    UO.Wait(2000);    
                }
                else if (UO.InJournal("Des An Ort"))
                {
                    UO.DeleteJournal();                
                    UO.Cast("Reactive Armor", pacient);            
                    if (UO.InJournal("The spell fizzles"))
                    {
                        UO.Cast("Reactive Armor", pacient);            
                    }
                    UO.Wait(2000);    
                }
                else
                {
                    UO.Cast("Greater heal", pacient);                            
                    UO.Wait(2500);    
                }
                            
                
            }
        }
        
        [Executable]
        public void Ress()
        {
            UO.Print(0x0435, "Vyber pacienta");            
            UOCharacter pacient = new UOCharacter(UIManager.TargetObject());
            if(World.Player.Mana < 10)
            {
                UO.UseType(0x0F09,0x0003);
                if(World.Player.Backpack.AllItems.Count(0x0F0B,0x0000) < 10)
                {
                    UO.Print(0x0435,"Mal by si capovat!");
                }
            }
            UO.Cast("Ressurection", pacient);
            UO.UseType(0x1F49,0x0000);
            UO.Wait(200);
            UO.Cast("Greater Heal", pacient);
        }
        
                
        [Executable]
        public static void kocky()
        {
            List<UOCharacter> alies = new List<UOCharacter>();
            alies.AddRange(Game.CurrentGame.Alies);
            UO.Print(0x0bb2,alies.Count()+"");
            
            //TODO dorob check na ludi
            
            Dictionary<string, int> results = new Dictionary<string, int>();
            Phoenix.JournalEntry[] safeList = CalExtension.Game.EntryHistory.ToArray();
            foreach(JournalEntry entry in safeList)
            {
                string text = entry.Text + String.Empty;
                if(text.Contains("rolls"))
                {
                    //You see Maxik rolls a 3,4
                    string meno = GetRollerName(text);
                    int hod = GetRollerRoll(text);
                    
                    try
                    {
                        results.Add(meno,hod);
                    }
                    catch(Exception e)
                    {
                        string s = e.Message; // aby ma nejebal compiler
                        //intentionaly blank
                    }
                }
            }
            
            string result = GetRollResults(results);
            UO.Say(result);
            CalExtension.Game.EntryHistory.Clear();
        }
        
        private static string GetRollResults(Dictionary<string, int> results)
        {
            int max = results.Values.Max();
            string retVal = "";
            if(results.Values.Count(x => x == max)>1)
            {
                retVal = "Rozhoz: ";                
                foreach(var item in results.Where(x => x.Value==max))
                {
                    retVal = retVal + item.Key+" ";
                }
            }
            else
            {
                retVal = "Berie: "+results.FirstOrDefault(x => x.Value == max).Key;
            }
            return retVal+" ("+max+")";
        }
        
        private static int GetRollerRoll(string text)
        {
            return Convert.ToInt32(text.Split(new string[] { " rolls a " }, StringSplitOptions.None)[1].Substring(0,1))
                    + Convert.ToInt32(text.Split(new string[] { " rolls a " }, StringSplitOptions.None)[1].Substring(2,1));
        }
        
        private static string GetRollerName(string text)
        {
            string retVal="";
            if(text.Contains("You roll"))
            {
                retVal = World.Player.Name;
            }
            else
            {
                string menoText = text.Split(new string[] { " rolls a " }, StringSplitOptions.None)[0];
                retVal = menoText.Substring(9,menoText.Length-9);
            }
            return retVal;
        }
        
        
        [Executable]
        public void BandaAll()        
        {
            List<UOCharacter> alies = new List<UOCharacter>();
            alies.AddRange(Game.CurrentGame.Alies);
            UOItem bandage = World.Player.Backpack.AllItems.FindType(0x0E21);
            
            UO.WaitTargetObject(World.Player.Serial);
                bandage.Use();
                UO.Wait(500);
            foreach (UOCharacter ch in alies)
            {            
                UO.WaitTargetObject(ch.Serial);
                bandage.Use();
                UO.Wait(500);
            }
        }
        
        
        [Executable]
        public void Boost()
        {
            UO.Print(0x0435, "Vyber pacienta");            
            UOCharacter pacient = new UOCharacter(UIManager.TargetObject());
            if(World.Player.Mana < 10)
            {
                UO.UseType(0x0F09,0x0003);
                if(World.Player.Backpack.AllItems.Count(0x0F0B,0x0000) < 10)
                {
                    UO.Print(0x0435,"Mal by si capovat!");
                }
            }
            UO.Cast("Bless", pacient);            
            UO.Wait(2300);
            //UO.Cast("Protection", pacient);            
            //UO.Wait(2000);
            UO.Cast("Reactive Armor", pacient);                        
        }
        
        
        [Executable]
        public void SipkaSelf()
        {
            Cast("Magic Arrow", true);            
        }
        
        [Executable]
        public void Para()
        {
            Cast("Paralyze", false);            
        }
        
        [Executable]
        public void GHeal()
        {
            Cast("Greater Heal", false);            
        }
        
        [Executable]
        public void Dispel()
        {
            Cast("Dispel", false);            
        }
        
        [Executable]
        public void RefRegF()
        {             
            UO.Print(0x0435,"kam regy");
            UOItem moje = new UOItem(UIManager.TargetObject());
            List<UOItem> allbags = new List<UOItem>();
            allbags.Insert(0,World.Player.Backpack);
            allbags.InsertRange(1,World.Player.Backpack.Items);
            ItemHelper.OpenContainerRecursive(World.Player.Backpack.Serial);
            
            foreach (UOItem item in World.Player.Backpack.AllItems)
            {
                if (ReagentCollection.Reagents.Contains(item))
                {
                    item.Move(65000, moje.Serial);
                }
            }
        
        }
        
        [Executable]
        public static void RefReg(int c, uint mine)
        {
            UO.Print(0x0435,"1: "+mine);
            UOItem moje = new UOItem(mine);
            UOItem doma = new UOItem(0x4037612B);
            doma.Use();
            if(mine==0)
            {
                UO.Print(0x0435,"Moje regy");
                moje = new UOItem(UIManager.TargetObject());
            }
            moje.Use();
            
            foreach (KeyValuePair<Graphic, ushort> item in new Dictionary<Graphic, ushort>
            {
                {0x0F7B,0x0000},
                {0x0F7A,0x0000},
                {0x0F84,0x0000},
                {0x0F85,0x0000},
                {0x0F86,0x0000},
                {0x0F88,0x0000},
                {0x0F8C,0x0000},
                {0x0F8D,0x0000},
                {0x0F7F,0x0000},
                {0x0F7C,0x0000},
                {0x0F83,0x0000},
                {0x0F7E,0x0000},
                {0x0F7D,0x031D},
                {0x0F78,0x0000},
                {0x0F79,0x0000},
                {0x0F80,0x0000},
                {0x0F81,0x0000},
                {0x0F82,0x0000},
                {0x0F89,0x0000},
                {0x0F8B,0x0000},
                {0x0F8E,0x0000},
                {0x0F8F,0x0000},
                {0x0F91,0x0000},
                {0x0F87,0x0000}})
                {
                    ushort count = (ushort)moje.AllItems.Count(item.Key);
                    UOItem reg = moje.Items.FindType(item.Key, item.Value);
                    //UO.Print(0x0435,count+"");
                    reg.Move(count, doma.Serial);
                    UO.Wait(100);    
                }
                
                if(c!=0){
                ushort x = 10;
            foreach (KeyValuePair<Graphic, ushort> item in new Dictionary<Graphic, ushort>
            {
                {0x0F7B,0x0000},
                {0x0F7A,0x0000},
                {0x0F84,0x0000},
                {0x0F85,0x0000},
                {0x0F86,0x0000},
                {0x0F88,0x0000},
                {0x0F8C,0x0000},
                {0x0F8D,0x0000}})
                {    
                
                    UOItem reg = doma.Items.FindType(item.Key, item.Value);                    
                    reg.Move((ushort)c, moje.Serial);
                    UO.Wait(100);    
                    UO.MoveItem(reg, 0, moje, x, 50);
                    UO.Wait(100);    
                    x=(ushort)(x+20);
                    
                }
                }
                
            }
            
        [Executable]
        public void Write()
        {
            UO.UseType(0x0FEF  ,0x0000  );
            UO.Wait(2000);
            UO.Press(System.Windows.Forms.Keys.A);
            UO.Press(System.Windows.Forms.Keys.B);
            UO.Press(System.Windows.Forms.Keys.Back);
            
            
        }
        
        [Executable]
        public void Sortbox(int offset)
        {
            UO.Print(0x0435,"Co upratat");
            UOItem box = new UOItem(UIManager.TargetObject());
            ushort X = 20;
            ushort Y = 10;
            UO.Print(0x0435,"Zaciname");
            while(true)
            {                
                if(X > 150)
                {
                    X = (ushort)20;
                    Y = (ushort)(Y + (ushort)20);
                }
                if(offset==0)
                {
                    UO.Print(0x0435,"Get Item");
                    UOItem itemIn = new UOItem(UIManager.TargetObject());
                    UO.Wait(500);
                    UO.MoveItem(itemIn, 0, box, X, Y);/*
                    foreach(UOItem item1 in box.AllItems)
                    {
                        if(itemIn.Color == item1.Color 
                        && itemIn.Graphic== item1.Graphic)
                        {
                            UO.Wait(500);
                            UO.MoveItem(item1, 0, box, X, Y);
                        }
                    }*/
                }
                else
                {
                    offset--;
                }
                 UO.Print(0x0435,offset+"");
                X = (ushort)(X + (ushort)15);
                
            }
      
        }
        
        [Executable]
        public void SortBoxClose()
        {
            UO.Print(0x0435,"Co upratat");
            UOItem box = new UOItem(UIManager.TargetObject());
            ushort X = 20;
            ushort Y = 20;
            UO.Print(0x0435,"Zaciname");
            
            foreach(UOItem item1 in box.AllItems)
                    {
                    if(X > 110)
                    {
                        X = (ushort)20;
                        Y = (ushort)(Y + (ushort)1);
                    }
                        
                        {
                            UO.Wait(500);
                            UO.MoveItem(item1, 0, box, X, Y);
                        }
                        X=(ushort)(X + (ushort)5);
                    }
            
            
            
      
        }
        
        private static void SortToReg(UOItem item)
        {
        
            Dictionary<string, uint> main = new Dictionary<string, uint>();
            main.Add("0x0F87;0x0005",0x4037612B);
            main.Add("0x0F84;0x0000",0x4037612B);
            main.Add("0x0F82;0x0000",0x4037612B);
            main.Add("0x0F7F;0x0000",0x4037612B);
            main.Add("0x0F87;0x0000",0x4037612B);
            main.Add("0x0F78;0x0000",0x4037612B);
            main.Add("0x0F83;0x0000",0x4037612B);
            main.Add("0x0F8E;0x0000",0x4037612B);
            main.Add("0x0F89;0x0000",0x4037612B);
            main.Add("0x0F7C;0x0000",0x4037612B);
            main.Add("0x0F80;0x0000",0x4037612B);
            main.Add("0x0F7D;0x0000",0x4037612B);
            main.Add("0x0F8F;0x0000",0x4037612B);
            main.Add("0x0F81;0x0000",0x4037612B);
            main.Add("0x0F8B;0x0000",0x4037612B);
            main.Add("0x0F7E;0x0000",0x4037612B);
            main.Add("0x0F91;0x0000",0x4037612B);
            main.Add("0x0F7B;0x0000",0x4037612B);
            main.Add("0x0F7A;0x0000",0x4037612B);
            main.Add("0x0F86;0x0000",0x4037612B);
            main.Add("0x0F88;0x0000",0x4037612B);
            main.Add("0x0F8D;0x0000",0x4037612B);
            main.Add("0x0F79;0x0000",0x4037612B);
            main.Add("0x0F8C;0x0000",0x4037612B);
            main.Add("0x0F85;0x0000",0x4037612B);


            string itemKey = item.Graphic+";"+item.Color;
            if(main.Keys.Contains(itemKey))
            {
                item.Move(65000,main[itemKey]);                                        
            }
        }
        
        
          [Executable]          
        public void SortboxRows()
        {
            UO.Print(0x0435,"Co upratat");
            UOItem box = new UOItem(UIManager.TargetObject());
            ushort X = 10;
            ushort Y = 10;
            UO.Print(0x0435,"Zaciname");
            List<string> check = new List<string>();
            foreach(UOItem itemIn in box.AllItems)
            {       
                if(!check.Contains(itemIn.Graphic+":"+itemIn.Color))
                {                   
                   check.Add(itemIn.Graphic+":"+itemIn.Color);
                
                if(X > 120)
                {
                    X = (ushort)5;
                    Y = (ushort)(Y + (ushort)10);
                }
                
                {                   
                    foreach(UOItem item1 in box.AllItems)
                    {
                        UO.Print(0x0435,"item");
                        if(itemIn.Color == item1.Color 
                        && itemIn.Graphic== item1.Graphic)
                        {
                            UO.Wait(500);
                            X = (ushort)(X + (ushort)3);
                            UO.MoveItem(item1, 0, box, X, Y);                            
                        }
                    }
                }
                 
                X = (ushort)10;
                Y = (ushort)(Y + (ushort)15);
                }
                
            }
      
        }
        
        [Executable]
        public void Capuj()
        {
            CapniTMR();
            CapniTR();
            CapniGS();
            CapniGH();
        }
        
                [Executable]
        public void CapniGH()
        {
            UOItem kad;
            UOItem lahvicky;
            UO.Print(0x0435,"GH");
            UOItem moje = backPack;
            bool presun = false;
            kad = moje.AllItems.FindType(0x1843, 0x08A7);
            lahvicky = World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000);
            while((World.Player.Backpack.AllItems.Count(lahvicky.Graphic,0x0000) > 0)
            && (World.Player.Backpack.AllItems.Count(0x0F0C,0x000) < 10))
            {
                presun = true;
                UO.WaitTargetObject(lahvicky.Serial);
                UO.UseObject(kad.Serial);
                UO.Wait(500);                
            }
            if(presun)
            {
                foreach(UOItem tmr in World.Player.Backpack.AllItems)//gh
                {
                    if (tmr.Graphic == 0x0F0C && tmr.Color == 0x000)
                    {
                        UO.MoveItem(tmr, 0, moje, (ushort)(120), 50);
                        UO.Wait(200);
                    }   
                }
            }
        }  
        
        [Executable]
        public void gmTMR()
        {
            string[] menus = new string[2];
            UOItem lahvicky = World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000);
            menus[0] = "Vyber typ potionu";
            menus[1] = "Total Mana Refresh (612 Eyes of Newt nebo 306 Blue Eyes of Newt";
            UOItem p = new UOItem(UIManager.TargetObject());
            UO.Wait(2000);
            UOItem n = new UOItem(UIManager.TargetObject());
            UO.Wait(2000);
            UOItem m = new UOItem(UIManager.TargetObject());
            UO.Wait(2000);
            
            //UO.Say(p.Amount+" ");
            
            m.Use();
            UO.WaitMenu(menus);
            
            foreach(UOItem kadtmr in World.Player.Backpack.AllItems)//TMR
                {
                UO.Print(0x0435,"hladam kad");
                    if (kadtmr.Graphic == 0x1843 && kadtmr.Color == 0x0003 && kadtmr.Amount == 51)
                    {
                        n = kadtmr;
                        UO.Print(0x0431,"mam kad");
                    }   
                }
            
            UO.Wait(2500);     
            UO.WaitTargetObject(p.Serial);
            UO.UseObject(n.Serial);
            UO.Wait(2500);                
            
            UO.WaitTargetObject(lahvicky.Serial);
            UO.UseObject(n.Serial);
            UO.Wait(500);    
            
            foreach(UOItem tmr in World.Player.Backpack.AllItems)//TMR
                {
                    if (tmr.Graphic == 0x0F09 && tmr.Color == 0x0003)
                    {
                        UO.WaitTargetObject(tmr.Serial);
                        UO.UseObject(p.Serial);
                        UO.Wait(500);
                    }   
                }
            
            
        }
        
        [Executable]
        public void CapniTMR()
        {
            UOItem kad;
            UOItem lahvicky;
            UO.Print(0x0435,"TMR");
            UOItem moje = backPack;
            bool presun = false;
            kad = moje.AllItems.FindType(0x1843, 0x0003);
            lahvicky = World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000);
            while((World.Player.Backpack.AllItems.Count(lahvicky.Graphic,0x0000) > 0)
            && (World.Player.Backpack.AllItems.Count(0x0F09,0x0003) < 15))
            {
                presun = true;
                UO.WaitTargetObject(lahvicky.Serial);
                UO.UseObject(kad.Serial);
                UO.Wait(500);                
            }
            if(presun)
            {
                foreach(UOItem tmr in World.Player.Backpack.AllItems)//TMR
                {
                    if (tmr.Graphic == 0x0F09 && tmr.Color == 0x0003)
                    {
                        UO.MoveItem(tmr, 0, moje, (ushort)(90), 50);
                        UO.Wait(200);
                    }   
                }
            }
        }       
       
        [Executable]
        public void CapniTR()
        {
            UOItem kad;
            UOItem lahvicky;
            UO.Print(0x0435,"TR");
            UOItem moje = backPack;
            bool presun = false;
            kad = moje.AllItems.FindType(0x1843, 0x014D);
            lahvicky = World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000);
            while((World.Player.Backpack.AllItems.Count(lahvicky.Graphic,0x0000) > 0)
            && (World.Player.Backpack.AllItems.Count(0x0F0B,0x0000) < 10))
            {
                presun = true;
                UO.WaitTargetObject(lahvicky.Serial);
                UO.UseObject(kad.Serial);
                UO.Wait(500);                
            }
            if(presun)
            {
                foreach(UOItem tmr in World.Player.Backpack.AllItems)//TMR
                {
                    if (tmr.Graphic == 0x0F0B && tmr.Color == 0x0000)
                    {
                        UO.MoveItem(tmr, 0, moje, (ushort)(105), 50);
                        UO.Wait(200);
                    }   
                }
            }
        }
        
        [Executable]
        public void CapniGS()
        {
            UOItem kad;
            UOItem lahvicky;
            UO.Print(0x0435,"GS");
            UOItem moje = backPack;
            bool presun = false;
            kad = moje.AllItems.FindType(0x1843, 0x0481);
            lahvicky = World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0000);
            while((World.Player.Backpack.AllItems.Count(lahvicky.Graphic,0x0000) > 0)
            && (World.Player.Backpack.AllItems.Count(0x0F09,0x0000) < 7))
            {
                presun = true;
                UO.WaitTargetObject(lahvicky.Serial);
                UO.UseObject(kad.Serial);
                UO.Wait(500);                
            }
            if(presun)
            {
                foreach(UOItem tmr in World.Player.Backpack.AllItems)//GS
                {
                    if (tmr.Graphic == 0x0F09 && tmr.Color == 0x0000)
                    {
                        UO.MoveItem(tmr, 0, moje, (ushort)(135), 50);
                        UO.Wait(200);
                    }   
                }
            }
        }
        
        [Executable]
        public void pytlikyTrash()        
        {
            UO.Print(0x0435,"Kde je bordel?");
            UOItem source = new UOItem(UIManager.TargetObject());
            UO.Print(0x0435,"Container na pytliky...");
            UOItem container = new UOItem(UIManager.TargetObject());
            
            foreach(UOItem pytlik in source.Items)
            {
                List<UOItem> items = new List<UOItem>();
                items.AddRange(pytlik.Items.ToArray());
                if(pytlik.Graphic==0x0E76 && items.Count()==0)
                {
                    pytlik.Move(1, World.Player.Backpack);
                    UO.Wait(500);
                    UO.WaitTargetObject(pytlik);
                    container.Use();
                    UO.Wait(500);
                    
                    
                }
            }
        }
        
        [Executable]
        public void upratat()
        {
            UO.Print(0x0435,"co upratat");
            UOItem moje = new UOItem(UIManager.TargetObject());
            
                    foreach(UOItem vec in moje.AllItems) //TMR
                    {
                        if (vec.Graphic == 0x0F09 && vec.Color == 0x0003)
                        {
                            UO.MoveItem(vec, 0, moje, (ushort)(90), 50);
                        }                         
                    }
      
                    foreach(UOItem vec in moje.AllItems) //TR
                    {
                        if (vec.Graphic == 0x0F0B && vec.Color == 0x0000)
                        {
                            UO.MoveItem(vec, 0, moje, (ushort)(105), 50);
                        }                         
                    }
                    
                    foreach(UOItem vec in moje.AllItems) //GH
                    {                        
                        if (vec.Graphic == 0x0F0C && vec.Color == 0x0000)
                        {
                            UO.MoveItem(vec, 0, moje, (ushort)(120), 50);
                        }                         
                    }
                
        }
        
        [Executable]
        public void TH()
        {
            UOItem bandage = World.Player.Backpack.AllItems.FindType(0x0E21);
            UOItem bloodyBandage= World.Player.Backpack.AllItems.FindType(0x0E20);
            UO.Print(0x0435, "Vyber pacienta");
            UOCharacter pacient = new UOCharacter(UIManager.TargetObject());
            
            while(!World.Player.Dead)
            {
                while(pacient.Hits < pacient.MaxHits)
                {
                    UO.Print(0x0435,"Healujem: "+pacient.Name);
                    UO.DeleteJournal();
                    UO.WaitTargetObject(pacient.Serial);
                    bandage.Use();
                    string[] asdas = { "Chces vytvorit", "You put", "You apply" };
                    Journal.WaitForText(asdas);   
                }
                
                if(World.Player.Backpack.AllItems.Count(0x0E20,0x0000)>0)
                {
                    UO.WaitTargetType(0x1008, 0x0000);
                    bloodyBandage.Use();
                    UO.MoveItem(bloodyBandage, 0, World.Player.Backpack, (ushort)(90), 50);
                }
                UO.Wait(1000);
            }
        }
        
        [Executable]
        public void FullHealTarget()
        {
            UOItem bandage = World.Player.Backpack.AllItems.FindType(0x0E21);
            UO.Print(0x0435, "Vyber pacienta");
            UOCharacter pacient = new UOCharacter(UIManager.TargetObject());
            
            while(pacient.Hits < pacient.MaxHits)
            {
                UO.Print(0x0435,"Healujem: "+pacient.Name);
                UO.DeleteJournal();
                UO.WaitTargetObject(pacient.Serial);
                bandage.Use();
                string[] asdas = { "Chces vytvorit", "You put", "You apply" };
                Journal.WaitForText(asdas);   
            }
            UO.WaitTargetObject(pacient.Serial);
            bandage.Use();
            UO.Print(0x0435,"Done");
        }
        
        [Executable]
        public void mh(int count)
        {
            UOItem bandage = World.Player.Backpack.AllItems.FindType(0x0E21);
            
            List<UOCharacter> parta = new List<UOCharacter>();
            for(int i=0; i<count; i++)
            {
                UO.Print(0x0435, "Vyber pacienta");
                UOCharacter heal = new UOCharacter(UIManager.TargetObject());
                parta.Add(heal);  
                UO.Wait(2000);
            }

            while (!World.Player.Dead)
            {        
                foreach(UOCharacter pacient in parta)
                {
                    if (pacient.Hits < pacient.MaxHits)
                    {
                        UO.DeleteJournal();
                        UO.WaitTargetObject(pacient.Serial);
                        bandage.Use();
                        string[] asdas = { "Chces vytvorit", "You put", "You apply" };
                        Journal.WaitForText(asdas);   
                    }
                }
                UO.Wait(500);                
            }
        }

	  [Executable]
        public void BishopHat()
        {
            UOItem bishopHat = World.GetItem(Aliases.GetObject("bishophat"));
            UOItem helm = World.GetItem(Aliases.GetObject("helm"));
            if (World.Player.Backpack.AllItems.Contains(bishopHat.Serial))
            {
                UO.PrintObject(World.Player.Serial, 0x0BB0, "Bishop Hat nasazena!");
                bishopHat.Use();
            }
            else if (World.Player.Layers[Layer.Hat].Serial == bishopHat.Serial)
            {
                if (World.Player.Hits > 30)
                {
                    bishopHat.Move(1, World.Player.Backpack);
                    if (World.Player.Backpack.AllItems.Contains(helm.Serial))
                    {
                        helm.Use();
                    }
                    UO.PrintObject(World.Player.Serial, 0x0BB0, "Bishop Hat sundana.");
			  return;
                }
                UO.PrintObject(World.Player.Serial, 0x0BB0, "Malo hp na sundani.");
            }
            else
            {
                UO.PrintObject(World.Player.Serial, 0x0BB0, "Nemas Bishop Hat!");
            }
        }

        [Executable]
        public void RessOne()
        {
            ushort[] clovek = new ushort[2];
            clovek[0] = 0x0192;
            clovek[1] = 0x0193;
            UOItem bandy = World.Player.Backpack.AllItems.FindType(0x0E21);
            if (bandy.Amount < 11)
            {
                UO.PrintWarning("Nemas dostatek band na ress");
                return;
            }
            foreach (UOCharacter character in World.Characters)
            {
                if (character.Distance < 3)
                {
                    for (int i = 0; i < clovek.Length; i++)
                    {
                        if (character.Model == clovek[i])
                        {
                            UO.PrintInformation("Resuji {0}", character.Name);
                            UO.WaitTargetObject(character.Serial);
                            bandy.Use();
                            return;
                        }
                    }
                }
            }
            UO.PrintInformation("Zadny character na ress nenalezen.");
        }
        
        [Executable]
        public void RessOneBlood()
        {
            ushort[] clovek = new ushort[2];
            clovek[0] = 0x0192;
            clovek[1] = 0x0193;
            UOItem bandy = World.Player.Backpack.AllItems.FindType(0x0E22);
            if (bandy.Amount < 11)
            {
                UO.PrintWarning("Nemas dostatek band na ress");
                return;
            }
            foreach (UOCharacter character in World.Characters)
            {
                if (character.Distance < 3)
                {
                    for (int i = 0; i < clovek.Length; i++)
                    {
                        if (character.Model == clovek[i])
                        {
                            UO.PrintInformation("Resuji {0}", character.Name);
                            UO.WaitTargetObject(character.Serial);
                            bandy.Use();
                            return;
                        }
                    }
                }
            }
            UO.PrintInformation("Zadny character na ress nenalezen.");
        }

	   UOCharacter zalozka;

        [Executable]
        public void IvmZalozka()
        {
		zalozka = World.GetCharacter(Aliases.GetObject("zalozka"));
		zalozka.WaitTarget();
		zalozka.Print("Greater heal");
            UO.Cast("Greater Heal");
        }

        [Executable]
        public void IvmSvitekZalozka()
        {
		zalozka = World.GetCharacter(Aliases.GetObject("zalozka"));
		zalozka.WaitTarget();
            if (World.Player.Backpack.AllItems.Count( 0x1F49  ) == 0)
            {
                UO.PrintError("Dosly GH svitky !!!");
                return;
            }
            UO.Print("Zbyva {0} GH svitku !", World.Player.Backpack.Items.Count( 0x1F49  ));
            UOItem svitek = World.Player.Backpack.AllItems.FindType( 0x1F49  );
		zalozka.WaitTarget();
            UO.UseObject(svitek);
		zalozka.Print("Greater heal svitek");
        }

        [Executable]
        public void STRZalozka()
        {
		zalozka = World.GetCharacter(Aliases.GetObject("zalozka"));
		zalozka.WaitTarget();
		zalozka.Print("Strength");
            UO.Cast("Strength");
        }


        [Executable]
        public void REACTIVZalozka()
        {
		zalozka = World.GetCharacter(Aliases.GetObject("zalozka"));
		zalozka.WaitTarget();
		zalozka.Print("Reactive Armor");
            UO.Cast("Reactive Armor");
        }

        [Executable]
        public void ArmorZalozka()
        {
		zalozka = World.GetCharacter(Aliases.GetObject("zalozka"));
		zalozka.WaitTarget();
		zalozka.Print("Protection");
            UO.Cast("Protection");
        }

        [Executable]
        public void BandaZalozka()
        {
            UOItem bandy = World.Player.Backpack.AllItems.FindType(0x0E21);
            if (bandy.Amount < 20)
            {
                UO.PrintWarning("Nemas dostatek band");
                return;
            }
		zalozka = World.GetCharacter(Aliases.GetObject("zalozka"));
		zalozka.WaitTarget();
		zalozka.Print("Banda");
            bandy.Use();
        }
        
        private static void MoveItem(UOItem item, int count, UOItem dest)
        {
            if (!item.Exist)
                return;
            ushort tomove = Math.Min((ushort)Math.Abs(count), item.Amount);
            using (ItemUpdateEventWaiter ew = new ItemUpdateEventWaiter(item))
            {
                item.Move(tomove, dest);
                if (ew.Wait(5000))
                    UO.PrintInformation(item.Graphic + " - " + tomove);
                UO.Wait(500);
            }
        }


    }
}