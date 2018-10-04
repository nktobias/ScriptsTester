using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using Phoenix.Runtime;

using CalExtension;
using Caleb.Library;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;
using CalExtension.UOExtensions;
using CalExtension.Abilities;




namespace Scripts.DarkParadise
{
    /// <summary>
    /// Trin HQ - 7
    /// 
    /// Nabit - 9
    /// </summary>
    public class Runebook
    {
        [DllImport("user32.dll",CharSet=CharSet.Auto, CallingConvention=CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        uint rune_jelom = 0x4032B4C0  ;  
        uint door_home = 0x40317C51; 
        
        UOItem materialContainer = new UOItem(0x4035C28C);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

    
        private DateTime requestTime;
        private uint? buttonId;
        
        [Executable]
        public static void buymap()
        {
            while(true)
            {
                BuyAtVendor("Seamus");
                UO.Say(",exec hid");
                UO.Wait(1800000);
            }
        }
        
        
        [Executable]
        public void nr()
        {
            UO.DeleteJournal();
            while(true)
            {
/*
RecallByRune(0x4038A79A);  BuyAtVendor("Titus"); BuyAtVendor("Kai");
RecallByRune(0x402B0F8B);  BuyAtVendor("Chimalis"); BuyAtVendor("Denton");	
RecallByRune(0x40376B7A);  BuyAtVendor("Kaladin"); BuyAtVendor("Brit");
RecallByRune(0x4032B4C0  );  BuyAtVendor("Adonia"); BuyAtVendor("Tovi");	
*/
RecallByRune(0x4038D828  );  BuyAtVendor("Stanton"); BuyAtVendor("Kelley");	

RecallByRune(0x403030CC  );  BuyAtVendor("Alexandra"); BuyAtVendor("Chal");
                RecallByRune(rune_jelom); 
                GoHomeAtJelom();
                Vyloz(materialContainer);                
RecallByRune(0x4038A882  );  BuyAtVendor("Kita");	        
RecallByRune(0x403314BF  );  BuyAtVendor("Carita");	
RecallByRune(0x4038D3F3  );  BuyAtVendor("Briana");	
RecallByRune(0x403861C6  );  BuyAtVendor("Reeve"); BuyAtVendor("Hadrian");
                RecallByRune(rune_jelom);
                         
                UO.Wait(5000);
                
                GoHomeAtJelom();                            
                Vyloz(materialContainer);
                UO.Say(",exec hid");
                UO.Wait(1800000);
            }
        
        }
        
        private void GoHomeAtJelom()
        {            
            Robot robot = new Robot();            
            IUOPosition test = robot.CreatePositionInstance(1389, 3856, 0);
            robot.GoTo(test);
            UO.UseObject(door_home);            
            UO.Wait(500);
            test = robot.CreatePositionInstance(1389, 3855, 0);
            robot.GoTo(test);
            UO.UseObject(door_home);
            test = robot.CreatePositionInstance(1385, 3849, 0);
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
            UO.Say(",exec hid");
            string[] save = { "You have been teleported", "World save has been initiated" };
            Journal.WaitForText(save);
            
            if (UO.InJournal("World save has been initiated"))
            {
              UO.DeleteJournal();
              UO.Wait(30000);  
            }
        }
        
        private static void BuyAtVendor(string name)
        {            
                UO.Say(name+" buy");
                UO.Wait(2000);
                uint X = 150;
                uint Y = 150;
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                UO.Wait(1000);
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
                UO.Wait(1000);
        }
        
        [Executable]
        public void nakup()
        {
            while(true)
            {
            
                UO.Say("Skyler buy");
                UO.Wait(2000);
                uint X = 150;
                uint Y = 150;
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                UO.Wait(1000);
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
                
                UO.Wait(2000);
                /*
                UO.Say("Kelley buy");
                UO.Wait(2000);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                UO.Wait(1000);
                mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
                */UO.DeleteJournal();
                 while(!UO.InJournal("You have hidden"))
                               {                               	
                                 UO.DeleteJournal();
             	                 UO.UseSkill("Hiding");            	                 
             	                 UO.Wait(2500);       
                               }
                 /*
                if(World.Player.Weight > 400)
                {
                    UO.UseObject(rune_home);
                    UO.WaitMenu("Jak chces runu pouzit?", "Recall");
                  
                    string[] save = { "You have been teleported", "World save has been initiated" };
                    Journal.WaitForText(save);
                    if (UO.InJournal("World save has been initiated"))
                    {
                       UO.Wait(30000);
                       UO.UseObject(rune_home);
                       UO.WaitMenu("Jak chces runu pouzit?", "Recall");
                       UO.Wait(5000);
                    }
                    
                    UO.UseObject(0x400017DE);
                    UO.Press(System.Windows.Forms.Keys.PageUp);
                    UO.Press(System.Windows.Forms.Keys.PageUp);
                    UO.Press(System.Windows.Forms.Keys.PageUp);
                    UO.UseObject(0x400017DE);
                    while (World.Player.Y != 597)
                    {
                       UO.Press(System.Windows.Forms.Keys.PageUp);                          
                       UO.Wait(500);
                    }                    
                    
                    Vyloz(materialContainer);
                    
                    UO.UseObject(rune);
                    UO.WaitMenu("Jak chces runu pouzit?", "Recall");
                                      
                    Journal.WaitForText(save);
                    if (UO.InJournal("World save has been initiated"))
                    {
                       UO.Wait(30000);
                       UO.UseObject(rune);
                       UO.WaitMenu("Jak chces runu pouzit?", "Recall");
                       UO.Wait(5000);
                    }
                }*/
                UO.Wait(450000);      
            }


        }
        
        public void Vyloz(UOItem container)
        {
            UOItem Vyrobek;
            string chyby = "";
            ushort[] VyrobekType = new ushort[5];
            VyrobekType[0] = 0x1876 ; // Wire
            VyrobekType[1] = 0x14FB ; // Lock
            VyrobekType[2] = 0x0E34 ; // scroll
            VyrobekType[3] = 0x0DF9 ; // bavlna
            VyrobekType[4] = 0x0FA0 ; // threads
              
            
            if (!container.Opened)
            {
              using (ItemOpenedEventWaiter ew = new ItemOpenedEventWaiter(container.Serial))
                {
			               container.Use();
                    if (!ew.Wait(60000))
                    {
                        UO.PrintWarning("Item open timeout");
                        return;
                    }
                }
            }
            
            for (int i = 0; i < 5; i++)
            {
                while ((World.Player.Backpack.AllItems.Count(VyrobekType[i], 0x0000)) > 0)
                {
                    Vyrobek = World.Player.Backpack.AllItems.FindType(VyrobekType[i], 0x0000);
                    using (ItemUpdateEventWaiter ew = new ItemUpdateEventWaiter(Vyrobek))
                    {
                        Vyrobek.Move(Vyrobek.Amount, container.Serial);
                        if (!ew.Wait(5000))
                        {
                            ScriptErrorException.Throw("1");
                            return;
                        }
                    }
                }
            }
            
            UO.PrintWarning(chyby);
            UO.Print("Prebytky vylozeny!!");
        }
        
        public static void EnsureItem(UOItem item)
        {
          if (String.IsNullOrEmpty(item.Name)) item.Click();
          UO.Wait(800);
        }
        [Executable]
        public void RBU(uint destinationId)
        {
          UOItem book = UO.Backpack.AllItems.FindType(0x22C5, 0x0000);
          if (!book.Exist)
            throw new ScriptErrorException("Runebook not found.");
    
          // Wait for gump
          buttonId = destinationId;
          requestTime = DateTime.Now;
    
          // Use runebook
          
          book.Use();
          string[] save = { "You have been teleported", "World save has been initiated", "Kniha uz nejde", "Kniha" };
            UO.DeleteJournal();
            Journal.WaitForText(save);
            
            if (UO.InJournal("World save has been initiated"))
            {
            
              UO.Wait(30000);  
            }
            UO.DeleteJournal();
        }
        

        [ServerMessageHandler(0xB0)]
        public CallbackResult OnGenericGump(byte[] data, CallbackResult prevResult)
        {            
            
            if (prevResult != CallbackResult.Normal)
                return prevResult;
  
            if (buttonId != null && DateTime.Now - requestTime < TimeSpan.FromSeconds(6)) {
                // Respond automatically
             
                uint gumpSerial = ByteConverter.BigEndian.ToUInt32(data,7);

                PacketWriter reply = new PacketWriter(0xB1);
                reply.WriteBlockSize();
                reply.Write(World.Player.Serial);
                reply.Write(gumpSerial);
                reply.Write(buttonId.Value);
                reply.Write(0); // Switches count
                reply.Write(0); // Entries count

                Core.SendToServer(reply.GetBytes());

                // Do not pass gump further
                buttonId = null;
                return CallbackResult.Sent;
            }

            return CallbackResult.Normal;
        }
    
    private static string l="";
    private static string location="";
    [Executable]       
    public static void mark()
    {
        if(location==World.Player.X+"."+World.Player.Y)
        {
            return;
        }
        location = World.Player.X+"."+World.Player.Y;
        if(l.Length==0)
        {
            l = location;
        }
        else
        {
            l = l + "|" + location;
        }
        UO.Print(0x0432, l);
    }
    
    [Executable]       
    public static void restartmark()
    {
        l="";
        location="";
        UO.Print(0x0432, "Mark restarted");
    }
    
    
    [Executable]       
    public void buywalk(params string[] positionsDefinition)
    {
      Robot r = new Robot();
      r.UseTryGoOnly = true;
      r.UseMinWait = true;
      r.UseRun = true;
      r.SearchSuqareSize = 450;
      


      string[] locations = positionsDefinition;

      foreach (string loc in locations)
      {
        string[] options = loc.Split('|');

        int button = -1;
        string bookType = "r";

        if (!String.IsNullOrEmpty(options[0]) && Regex.IsMatch(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})"))
        {
          Match m = Regex.Match(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})");
          bookType = m.Groups["booktype"].Value.ToLower();
          button = Int32.Parse(m.Groups["button"].Value);
        }

        if (button > -1)
        {
          string book = "RuneBookUse";
          if (bookType == "t")
          {
            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
            UO.Wait(1000);
            book = "TravelBookUse";
          }
          else if (bookType == "c")
          {
            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["CestovniKnihaUse"], 1);
            UO.Wait(1000);
            book = "CestovniKnihaUse";
          }



          bool teleported = false;
          while (!teleported)
          {
            UO.DeleteJournal();

            Phoenix.Runtime.RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList[book], button);
            Game.Wait(500);
            if (!World.Player.Hidden)
              UO.UseSkill("Hiding");

            UO.Print("Cekam na kop.. nehybat");

            if (Journal.WaitForText(true, 2000, "Nesmis vykonavat zadnou akci"))
            {
              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
              Game.Wait(1000);
            }
            else if (Journal.WaitForText(true, 120000, "You have been teleported"))
              teleported = true;

            if (Game.CurrentGame.WorldSave())
            {
              UO.Print("WS opakovani kopu za 45s");
              Game.Wait(45000);
              if (bookType == "t")
              {
                Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
                UO.Wait(1000);
              }
              Game.Wait(500);
            }
          }
        }

        for (int i = 1; i < options.Length; i++)
        {
          if (UO.Dead)
            return;

          string[] parm = options[i].Split('.');

          string x = parm[0];
          string[] y = parm[1].Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
          string placeName = "";
          if (y.Length > 1)
            placeName = y[1];


          UOPositionBase pos = new UOPositionBase(ushort.Parse(x), ushort.Parse(y[0]), (ushort)0);

          int distance = parm.Length > 2 ? CalExtension.UOExtensions.Utils.ToNullInt(parm[2]).GetValueOrDefault(1) : 1;
          int gotries = parm.Length > 3 ? CalExtension.UOExtensions.Utils.ToNullInt(parm[3]).GetValueOrDefault(1000) : 1000;

          Game.PrintMessage("GoTo: " + pos);
          if (r.GoTo(pos, distance, gotries))
          {
            Game.PrintMessage("In position: " + pos);

            if (parm[parm.Length - 1].ToLower() == "opendoor")
            {
              ItemHelper.OpenDoorAll();
              Game.Wait();
            }
          }
        }
       }
      }
     }
}
