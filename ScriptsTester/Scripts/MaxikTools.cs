using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using CalExtension.UOExtensions;
using CalExtension;
using CalExtension.UI.Status;
using System.Threading;
using Caleb.Library.CAL.Business;
using System.Collections;
using System.Runtime.InteropServices;
using Phoenix.Communication;
using System.Linq;
using System.Reflection;

namespace Phoenix.Scripts
{

    public class MaxikTools
    {        
        private static void Move(UOItem item, uint toSerial)
        {            
            item.Move(65000,toSerial);                        
        }
        
        public static void test(string s)
        {
            UO.Print(s);
        }
        
        [Executable]
        public void WriteRune(string input)
        {
            File.WriteAllText("test.txt",input);
        }
        
        [Executable]
        public void ReadRune()
        {
            string text = File.ReadAllText("test.txt");
            UO.Print(text);
        }
        
        [Executable]
        public void GetRuneCoordinates()
        {
            UO.Print("runa ?");
            UOItem rune = new UOItem(UIManager.TargetObject());
            UOItemExtInfo info = ItemHelper.GetItemExtInfo(rune);
            
            string realName = Game.LastEntry.Text; 
            if (!realName.ToLower().StartsWith(rune.Name.ToLower()))
            {
                for (int i = Game.EntryHistory.Count -1; i >= Game.EntryHistory.Count - 5 && i >= 0; i--)
                {
                    if (Game.EntryHistory[i].Text.ToLower().StartsWith(rune.Name.ToLower()))
                    {
                        realName = Game.EntryHistory[i].Text;
                        break;
                    }
                }
            }
            string fullName = realName;
            string coordinates = realName.Split('(')[1].Split(')')[0];
            UO.Print(realName.Split('(')[1].Split(')')[0]);
        }
        
        [Executable]
        public void testruna()
        {
            UO.Print("runa ?");
            UOItem runa = new UOItem(UIManager.TargetObject());
            int counter = 0;
            while(runa.Exist)
            {
                if(true || runa.Graphic!=0x1F14 || runa.Color!=0x0482)
                {
                    //portni domov
                    //UO.Say(",nbruna");
                    //UO.Wait(30000); //TODO ensure WS
                    //chod k truhle
                    //GoHomeAtJelom();
                    //zober si novu runu
                    UOItem bedna = new UOItem(0x40314338);
                    bedna.Click();
                    UO.Wait(500);
                    bedna.Use();
                    UOItem suflik = new UOItem(0x40258A79);
                    suflik.Click();
                    UO.Wait(500);
                    suflik.Use();
                    UO.Wait(500);
                    UOItem newRune = suflik.AllItems.FindType(0x1F14,0x0482);
                    newRune.Move(1,World.Player.Backpack);                    
                    UO.Wait(500);
                    //portni starou runu
                    RecallByRune(runa.Serial);
                    //markni novu
                    UO.Wait(90000);
                    UO.DeleteJournal();
                    UO.UseObject(newRune.Serial);
                    UO.WaitMenu("Jak chces runu pouzit?", "Mark");
                    UO.Wait(5000);
                    //zahod staru   
                    throw new Exception("HOTOVO A MARKNUTE");
                }
                counter++;
                UO.Print("Pocet kopnuti: "+counter);
                if(World.Player.Mana < 25)
                {
                    UO.Say(",exec DrinkPotion \"Total Mana Refresh\"");
                }
                runa.Click();
                UO.Wait(500);                
                runa.Move(1, World.Player.Backpack);
                UO.Wait(500);                
                RecallByRune(runa.Serial);
                UO.Wait(15000);                
            }
            
        }
        
        private void GoHomeAtJelom()
        {           
            uint door_home = 0x40317C51;
            Robot robot = new Robot();            
            IUOPosition test = robot.CreatePositionInstance(1389, 3856, 0);
            robot.GoTo(test);
            UO.UseObject(door_home);            
            UO.Wait(500);
            test = robot.CreatePositionInstance(1389, 3855, 0);
            robot.GoTo(test);
            UO.UseObject(door_home);
            test = robot.CreatePositionInstance(1387, 3851, 0);
            robot.GoTo(test);
            
        }
        
        private void RecallByRune(uint serial)
        {
            if(World.Player.Mana < 20)            
            {
                UO.Wait(10000);
            }
            UO.DeleteJournal();            
            UO.UseObject(serial);
            UO.WaitMenu("Jak chces runu pouzit?", "Recall");
            UO.Wait(5000);
            
            if (UO.InJournal("World save has been initiated"))
            {
              UO.DeleteJournal();
              UO.Wait(30000);  
            }
            string[] save = { "You have been teleported", "World save has been initiated" };
            Journal.WaitForText(save);
            
            if (UO.InJournal("World save has been initiated"))
            {
              UO.DeleteJournal();
              UO.Wait(30000);  
            }
        }
        
        [Executable]
        public void SortLowWeapons()
        {
            UO.Print("Kde ?");
            UOItem from = new UOItem(UIManager.TargetObject());
            
            UO.Print("Kam ?");
            UOItem to = new UOItem(UIManager.TargetObject());
            
            foreach(UOItem item in from.AllItems)
            {
                item.Click();
                UO.Wait(500);
                if(!item.Name.Contains("Vanquishing"))
                {
                    item.Move(1,to.Serial);
                    
                }
            }
            UO.Print(0x0435,"Done.");
        }

        [Executable]
        public static void SortLoot()
        {
            
            UO.Print(0x0435,"Kde je loot?");
            UOItem from = new UOItem(UIManager.Target().Serial);
            EnsureContainer(from);
            
            UO.Print(0x0435,"Sperky prve...");
            UOItem refbedna = new UOItem(0x40314338);
            EnsureContainer(refbedna);
            UOItem sperkovnice = new UOItem(0x40169305);
            EnsureContainer(sperkovnice);
            ItemHelper.naplnsperky(from, sperkovnice);
            UO.Print(0x0435,"Sperky hotovo.");
            
            foreach(UOItem lootItem in from.Items)            
            {   
                SortToPoklad(lootItem);
            }
            
            
            foreach(UOItem lootItem in from.Items)            
            {   
                SortToReg(lootItem);
            }
            
            VylozArmor(from.Serial);            
            VylozNecro(from.Serial);            
            VylozZbrane(from.Serial);
            VylozStity(from.Serial);
            

            UO.Print(0x0435,"Vsetko hotovo !");
        }
        
        [Executable]
        public static void SortLootRecursive()
        {
            List<UOItem> containers = new List<UOItem>();
            containers.Add(new UOItem(0x40365E85)); //Main
            containers.Add(new UOItem(0x4037612B)); //Regy  
            
            List<UOItem> allContainers = new List<UOItem>();
            allContainers.AddRange(containers);
            
            UO.Print(0x0435,"Kde je loot?");
            UOItem from = new UOItem(UIManager.Target().Serial);
            EnsureContainer(from);
            
            UO.Print(0x0435,"Sperky prve...");
            UOItem main = new UOItem(0x40314338);
            EnsureContainer(main);
            UOItem sperkovnice = new UOItem(0x40169305);
            EnsureContainer(sperkovnice);
            ItemHelper.naplnsperky(from, sperkovnice);
            UO.Print(0x0435,"Sperky hotovo.");
            
            UO.Print(0x0435,"Hladam bedne...");
            foreach(UOItem container in containers)            
            {   
                allContainers.AddRange(GetContainerRecursive(container));                
            }
            UO.Print(0x0435,"Pocet bedni: "+allContainers.Count());
            
            UO.Print(0x0435,"Zaciname, ostava "+from.AllItems.Count()+" itemov");
            foreach(UOItem container in allContainers)            
            {
                EnsureContainer(container);
                foreach(UOItem item in from.AllItems)
                {   
                    PlaceToParentContainer(item, container);
                    UO.Wait(500);                                    
                }
            }
            UO.Print(0x0435,"Hotovo!");
        }
        
        [Executable]
        public static void sl()
        {
            UOItem from = new UOItem(UIManager.Target().Serial);
            SortToReg(from);
            UO.Print(0x0435,"Hotovo!");
        }
        
        private static void SortToPoklad(UOItem item)
        {
        
            Dictionary<string, uint> main = new Dictionary<string, uint>();
            main.Add("0x0EB2;0x0000",0x401543BA);
            main.Add("0x0E9C;0x0000",0x401543BA);
            main.Add("0x0EB3;0x0000",0x401543BA);
            main.Add("0x09D0;0x0B7A",0x4027940F);
            main.Add("0x1085;0x0B4B",0x4027940F);
            main.Add("0x1728;0x0000",0x4027940F);
            main.Add("0x1420;0x0152",0x402D59B0);
            main.Add("0x0FC4;0x0498",0x402D59B0);
            main.Add("0x0DC3;0x005B",0x40257B6D);
            main.Add("0x136C;0x0B94",0x40257B6D);
            main.Add("0x0CB0;0x0899",0x40257B6D);
            main.Add("0x0DBD;0x0B9F",0x40257B6D);
            main.Add("0x1A9D;0x0481",0x40257B6D);
            main.Add("0x0F5A;0x0044",0x40257B6D);
            main.Add("0x108B;0x0BB5",0x40257B6D);
            main.Add("0x0E73;0x0B9F",0x40257B6D);
            main.Add("0x0D16;0x00A3",0x40257B6D);
            main.Add("0x0FF4;0x03DF",0x40387925);
            main.Add("0x0FF4;0x0BA0",0x40387925);
            main.Add("0x0FF4;0x08AD",0x40387925);
            main.Add("0x0FF4;0x0844",0x40387925);
            main.Add("0x0FF4;0x05E4",0x40387925);
            main.Add("0x0FF4;0x0B47",0x40387925);
            main.Add("0x0FF4;0x0B79",0x40387925);
            main.Add("0x0FF4;0x0964",0x40387925);
            main.Add("0x0FF4;0x071A",0x40387925);
            main.Add("0x0FF4;0x070B",0x40387925);
            main.Add("0x0FCC;0x0BB3",0x4034A535);
            main.Add("0x0FCC;0x0B88",0x4034A535);
            main.Add("0x0FCC;0x0B87",0x4034A535);
            main.Add("0x0FCC;0x0B60",0x4034A535);
            main.Add("0x0FCC;0x0BA4",0x4034A535);
            main.Add("0x103D;0x0B52",0x4034A535);
            main.Add("0x0E2A;0x0718",0x400A936F);
            main.Add("0x0E26;0x0B27",0x400A936F);
            main.Add("0x1848;0x0B4F",0x400A936F);
            main.Add("0x1848;0x0B93",0x400A936F);
            main.Add("0x1848;0x0027",0x400A936F);
            main.Add("0x1848;0x0055",0x400A936F);
            main.Add("0x1848;0x0069",0x400A936F);
            main.Add("0x1848;0x05E6",0x400A936F);
            main.Add("0x1C18;0x0000",0x400A936F);
            main.Add("0x0FEF;0x0000",0x40365E85);
            main.Add("0x0EED;0x0B81",0x40365E85);
            main.Add("0x0EED;0x0000",0x40365E85);
            main.Add("0x14ED;0x0658",0x40365E85);
            main.Add("0x1BD7;0x0360",0x40365E85);
            main.Add("0x1BEF;0x0162",0x40365E85);
            main.Add("0x1BDD;0x0237",0x40365E85);
            main.Add("0x0EED;0x0836",0x40365E85);


            string itemKey = item.Graphic+";"+item.Color;
            if(main.Keys.Contains(itemKey))
            {
                Move(item, main[itemKey]);
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
                Move(item, main[itemKey]);
            }
        }
        
        private static void PlaceToParentContainer(UOItem sourceItem, UOItem sourceContainer)
        { 
            
                EnsureContainer(sourceContainer);
                foreach(UOItem item in sourceContainer.Items)
                {                    
                    if(Compare(item, sourceItem))
                    {                        
                        if(item.Amount > 1 && (item.Amount+sourceItem.Amount) < 65000)
                        {
                            sourceItem.Move(6500, sourceContainer.Serial);
                        }
                        else
                        {
                            UO.MoveItem(sourceItem, 0, sourceContainer.Serial, item.X, item.Y);
                        }
                        return;
                    }
                }                
            
        }      

        
        private static bool IsFromTypeCollection(UOItem item, UOItemTypeCollection collection)
        {
            foreach(UOItemType type in collection)
            {
                if(type.Is(item))
                {
                    return true;
                }
            }
            return false;
        }
        
        [Command]
        public static void moveall()
        {
            UO.Print(0x0435,"Zdroj?");
            UOItem from = new UOItem(UIManager.Target().Serial);
            
            UO.Print(0x0435,"Ciel?");
            UOItem to = new UOItem(UIManager.Target().Serial);
            
            foreach(UOItem item in from.Items)
            {
                item.Move(65000, to.Serial);
                UO.Wait(500);
            }            
            UO.Print(0x0435,"Hotovo");        
        }
        
        [Executable]
        public static void VylozArmor(uint containerId)
        {
            UOItem container = null;
            if(containerId==0)
            {
                container = new UOItem(UIManager.Target().Serial);
            }
            else
            {
                container = new UOItem(containerId);
            }
            UO.Print(0x0435,"Vykladam armor...");
            UOItem main = new UOItem(0x40181DCD);
            EnsureContainer(main);
            
            UOItem inv = null;
            UOItem fo = null;
            UOItem trash = null;
            
            UOItem linv = new UOItem(0x4007A019);            
            UOItem lfo = new UOItem(0x400A8364);
            UOItem ltrash = new UOItem(0x40230B2B);
            EnsureContainer(linv);
            EnsureContainer(lfo);
            EnsureContainer(ltrash);
            UOItem rinv = new UOItem(0x40070B74);            
            UOItem rfo = new UOItem(0x400953E6);
            UOItem rtrash = new UOItem(0x402A011C);
            EnsureContainer(rinv);
            EnsureContainer(rfo);
            EnsureContainer(rtrash);            
            UOItem cinv = new UOItem(0x4022E74C);            
            UOItem cfo = new UOItem(0x40320EE4);
            UOItem ctrash = new UOItem(0x4000808D);
            EnsureContainer(cinv);
            EnsureContainer(cfo);
            EnsureContainer(ctrash);
            UOItem pinv = new UOItem(0x402CE4D0);            
            UOItem pfo = new UOItem(0x40340144);
            UOItem ptrash = new UOItem(0x400B5218);
            EnsureContainer(pinv);
            EnsureContainer(pfo);
            EnsureContainer(ptrash);
            
            UOItem rminv = new UOItem(0x4033A1FD);
            UOItem rmfo = new UOItem(0x4036B03C);
            UOItem rmtrash = new UOItem(0x4031EC55);
            EnsureContainer(rminv);
            EnsureContainer(rmfo);
            EnsureContainer(rmtrash);
            UOItem rninv = new UOItem(0x402BF858);            
            UOItem rnfo = new UOItem(0x402A9014);
            UOItem rntrash = new UOItem(0x40304403);
            EnsureContainer(rninv);
            EnsureContainer(rnfo);
            EnsureContainer(rntrash);
            
            
            
            foreach(UOItem item in container.Items.Where(x => !IsContainer(x)))            
            {
                UO.Print(0x0435,"1");
                inv = null;
                fo = null;
                trash = null;
                
                item.Click();
                UO.Wait(500);
                UO.Print(0x0435,item.Name);
                if(item.Name.Contains("Leather")||item.Name.Contains("Studded"))
                {
                    UO.Print(0x0435,"L");
                    inv = linv;
                    fo = lfo;
                    trash = ltrash;
                }
                else if(item.Name.Contains("Ringmail"))
                {
                    UO.Print(0x0435,"R");
                    inv = rinv;
                    fo = rfo;
                    trash = rtrash;
                }
                else if(item.Name.Contains("Chainmail"))
                {
                    UO.Print(0x0435,"C");
                    inv = cinv;
                    fo = cfo;
                    trash = ctrash;
                }
                else if(item.Name.Contains("Plate"))
                {
                    UO.Print(0x0435,"P");
                    inv = pinv;
                    fo = pfo;
                    trash = ptrash;
                }
                else if(item.Name.Contains("Mage's Robe"))
                {
                    UO.Print(0x0435,"MR");
                    inv = rminv;
                    fo = rmfo;
                    trash = rmtrash;
                }
                else if(item.Name.Contains("Death Robe"))
                {
                    UO.Print(0x0435,"NR");
                    inv = rninv;
                    fo = rnfo;
                    trash = rntrash;
                }
                
                if(inv==null||fo==null||trash==null)
                {
                    continue;
                }               
                
                if(item.Name.Contains("Defe")||item.Name.Contains("Guard")||item.Name.Contains("Harde"))
                {
                    item.Move(1,trash.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Fort"))
                {
                    item.Move(1,fo.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Invul"))
                {
                    item.Move(1,inv.Serial);
                    UO.Wait(500);
                }
            }
            UO.Print(0x0435,"Hotovo");
        }
        
        [Executable]
        public static void VylozNecro(uint containerId)
        {
            UOItem container = null;
            if(containerId==0)
            {
                container = new UOItem(UIManager.Target().Serial);
            }
            else
            {
                container = new UOItem(containerId);
            }
            UO.Print(0x0435,"Vykladam necro brnka...");
            UOItem main = new UOItem(0x40181DCD);
            EnsureContainer(main);
            UOItem inv = new UOItem(0x40223C35);
            EnsureContainer(inv);
            UOItem fo = new UOItem(0x4022B37B);
            EnsureContainer(fo);
            UOItem trash = new UOItem(0x401FC61A);
            EnsureContainer(trash);
            
            foreach(UOItem item in container.Items.Where(x => !IsContainer(x)))            
            {
                item.Click();
                UO.Wait(500);
                UO.Print(0x0435,item.Name);
                if(item.Name.Contains("Ancient Bone")||item.Name.Contains("a Bone"))
                {
                    item.Move(1,trash.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Ancient Skeleton")||item.Name.Contains("a Skeleton"))
                {
                    item.Move(1,fo.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Ancient Liche")||item.Name.Contains("a Liche"))
                {
                    item.Move(1,inv.Serial);
                    UO.Wait(500);
                }
            }
            UO.Print(0x0435,"Hotovo");
        }
        
        [Executable]
        public static void VylozStity(uint containerId)
        {
            UOItem container = null;
            if(containerId==0)
            {
                container = new UOItem(UIManager.Target().Serial);
            }
            else
            {
                container = new UOItem(containerId);
            }
            UO.Print(0x0435,"Vykladam stity...");
            UOItem main = new UOItem(0x40181DCD);
            EnsureContainer(main);
            UOItem inv = new UOItem(0x402C9DD1);
            EnsureContainer(inv);
            UOItem fo = new UOItem(0x4013D75C);
            EnsureContainer(fo);
            UOItem trash = new UOItem(0x400C093F);
            EnsureContainer(trash);
            
            foreach(UOItem item in container.Items.Where(x => !IsContainer(x)))            
            {
                item.Click();
                UO.Wait(500);
                UO.Print(0x0435,item.Name);
                if(item.Name.Contains("Defense")||item.Name.Contains("Guarding")||item.Name.Contains("Hardening"))
                {
                    item.Move(1,trash.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Fortificatio"))
                {
                    item.Move(1,fo.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Invul"))
                {
                    item.Move(1,inv.Serial);
                    UO.Wait(500);
                }
            }
            UO.Print(0x0435,"Hotovo");
        }
        
        [Executable]
        public static void VylozZbrane(uint containerId)
        {
            UOItem container = null;
            if(containerId==0)
            {
                container = new UOItem(UIManager.Target().Serial);
            }
            else
            {
                container = new UOItem(containerId);
            }
            UO.Print(0x0435,"Vykladam zbrane...");
            UOItem main = new UOItem(0x40181DCD);
            EnsureContainer(main);
            UOItem mf = new UOItem(0x401DB384);
            EnsureContainer(mf);
            UOItem fe = new UOItem(0x40379090);
            EnsureContainer(fe);
            UOItem sw = new UOItem(0x402247A1);
            EnsureContainer(sw);
            UOItem ar = new UOItem(0x4035C431);
            EnsureContainer(ar);
            UOItem trash = new UOItem(0x403388AB);
            EnsureContainer(trash);
            
            foreach(UOItem item in container.Items.Where(x => !IsContainer(x)))            
            {
                item.Click();
                UO.Wait(500);
                if(item.Name.Contains("Ruin")||item.Name.Contains("Force")||item.Name.Contains("Power")||item.Name.Contains("Might"))
                {
                    item.Move(1,trash.Serial);
                    UO.Wait(500);
                }
                else if(item.Name.Contains("Vanqu"))
                {
                    foreach(UOItemType type in ItemLibrary.WeaponsMace)
                    {
                        if(type.Is(item))
                        {
                            item.Move(1,mf.Serial);
                            UO.Wait(500);
                        }
                    }
                    foreach(UOItemType type in ItemLibrary.WeaponsSword)
                    {
                        if(type.Is(item))
                        {
                            item.Move(1,sw.Serial);
                            UO.Wait(500);
                        }
                    }
                    foreach(UOItemType type in ItemLibrary.WeaponsFenc)
                    {
                        if(type.Is(item))
                        {
                            item.Move(1,fe.Serial);
                            UO.Wait(500);
                        }
                    }
                    foreach(UOItemType type in ItemLibrary.WeaponsArch)
                    {
                        if(type.Is(item))
                        {
                            item.Move(1,ar.Serial);
                            UO.Wait(500);
                        }
                    }
                }
            }
            
            UO.Print(0x0435,"Hotovo");
        }
        
        [Executable]
        public static void Foo()
        {
           UOItem main = new UOItem(UIManager.Target().Serial);
           EnsureContainer(main);
           List<UOItem> containers = GetContainerRecursive(main);
           containers.Add(main);
           StringBuilder sb = new StringBuilder();
           Dictionary<string,uint> data = new Dictionary<string,uint>();
           
           int c = containers.Count();
           int i = 0;
           foreach(UOItem container in containers)
           {
                i++;
                UO.Print(0x0435,i+"/"+c);
                int ci = container.Items.Count();
                int ii = 0;
                foreach(UOItem item in container.Items)
                {   
                    ii++;                    
                    UO.Print(0x0435,i+"/"+c+" ("+ii+"/"+ci+")");
                    if(!IsContainer(item))
                    {                     
                        try
                        {
                            string dataToAppend = GetItemCodeData(item);
                            if(!sb.ToString().Contains(dataToAppend))
                            {
                                sb.AppendLine(dataToAppend);
                            }
                            //data.Add(GetItemData(item),container.Serial);
                        }
                        catch (ArgumentException)
                        {
                            //Intentionaly blank
                        }
                    }
                }
            }
            
            string path = @"test.txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                File.WriteAllText(path, sb.ToString());
            }
        
            
        }
      
        
        private static string GetItemData(UOItem sourceItem)
        {            
            sourceItem.Click();
            Pause();
            string data = sourceItem.Graphic+";"+sourceItem.Color+";"+sourceItem.Name+":"+GetItemContainerChain(sourceItem,"");
            return data;            
        }
        
        private static string GetItemCodeData(UOItem sourceItem)
        {            
            sourceItem.Click();
            Pause();
            string data = "main.Add(\""+sourceItem.Graphic+";"+sourceItem.Color+"\","+sourceItem.Container+");";
            return data;            
        }
        
        private static string GetItemContainerChain(UOItem item, string retVal)
        {   
            bool b = false;
            while(item.Container!=0)
            {
                b = true;
                UOItem parent = new UOItem(item.Container);
                EnsureContainer(parent);                
                retVal = retVal+ item.Container+"/"; // GetItemContainerChain(parent,retVal)+"|";
                item = parent;
            }            
            if(!b && IsContainer(item))
            {
                retVal = item.Serial+"";
            }
            retVal = retVal.Trim('/');            
            return retVal;
        }
    
   
        [Executable]
        public static void FindRecursive()
        {
            UO.Print(0x0435,"Ktory item mam hladat?");
            UOItem sourceItem = new UOItem(UIManager.Target().Serial);
            
            UO.Print(0x0435,"Kde mam hladat?");
            UOItem where = new UOItem(UIManager.Target().Serial);
            EnsureContainer(where);
            
            UO.Print(0x0435,"Kam to mam davat?");
            UOItem to = new UOItem(UIManager.Target().Serial);
            EnsureContainer(to);
            
            List<UOItem> containers = GetContainerRecursive(where);
            
            foreach(UOItem container in containers)
            {
                EnsureContainer(container);
                foreach(UOItem item in container.AllItems)
                {
                    if(Compare(item, sourceItem))
                    {
                        item.Move(65000, to.Serial);
                        Pause();
                    }
                }                
            }
            UO.Print(0x0435,"Hotovo!");
           

        }
        
        private static void EnsureContainer(UOItem container)
        {
            //EnsureItem(container);
            if (IsContainer(container))
            {
                container.Use();
                UO.Wait(1000);
            }
        }
        
        private static bool IsContainer(UOItem item)
        {
            return item.Graphic == 0x0E76 || item.Graphic == 0x0E75 || item.Graphic == 0x0E77 || item.Graphic == 0x0E78 || item.Graphic == 0x0E79 || item.Graphic == 0x0E7D;
        }
        
        private static void EnsureItem(UOItem item)
        {
            if (String.IsNullOrEmpty(item.Name)) item.Click();
            Pause();
        }
        
        private static void Pause()
        {
            UO.Wait(500);
        }
        
        private static bool Compare(UOItem i1, UOItem i2)
        {
            return i1.Color==i2.Color && i1.Graphic==i2.Graphic && i1.Name==i2.Name;
        }
        
        private static List<UOItem> GetContainerRecursive(Serial container)
        {
            UOItem cont = new UOItem(container);
            EnsureContainer(cont);
            List<UOItem> list = new List<UOItem>();            
            
            foreach (UOItem item in cont.Items)
            {
                if (IsContainer(item))
                {
                    list.Add(item);
                    list.AddRange(GetContainerRecursive(item.Serial));
                }
            }
            return list;
        }
    }
}