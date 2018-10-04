
 
 /////////////////////////////////////////////////////////////////////////
 //
 //     www.ultima.smoce.net
 //     Name: Massmove _1
 //
 /////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using System.Linq;
using Phoenix.Communication;
using Phoenix.Runtime;
using Phoenix.Runtime.Reflection;

namespace Scripts.DarkParadise
{       
  public class ultima_smoce_net_1530711665
	{ [Command]
        public static void MassMove()
        {
            MassMove(true);
        }

        [Command]
        public static void MassMove(bool specificcolor)
        {
            UO.Print("Select item type:");
            UOItem typeItem = new UOItem(UIManager.TargetObject());

            if (!typeItem.Exist) {
                ScriptErrorException.Throw("Invalid item.");
                return;
            }

            Graphic graphic = typeItem.Graphic;
            UOColor color = specificcolor ? typeItem.Color : UOColor.Invariant;

            //System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Check for color?", "Phoenix - MassMove", System.Windows.Forms.MessageBoxButtons.YesNo);
            //if (result == System.Windows.Forms.DialogResult.No)
            //    color = UOColor.Invariant;

            //UO.Print("Select source container:");
            //UOItem source = new UOItem(UIManager.TargetObject());
            UOItem source = new UOItem(typeItem.Container);

            if (!source.Exist) {
                ScriptErrorException.Throw("Invalid source container.");
                return;
            }

            UO.Print("Select destination container:");
            UOItem dest = new UOItem(UIManager.TargetObject());

            if (!dest.Exist) {
                ScriptErrorException.Throw("Invalid destination.");
                return;
            }

            MassMove(graphic, color, source, dest);
        }

        [Command]
        public static void MassMove(Graphic graphic, UOColor color, Serial sourceContainer, Serial destination)
        {
            UOItem source = World.GetItem(sourceContainer);

            if (!source.Exist) {
                ScriptErrorException.Throw("Invalid source container.");
                return;
            }

            if (!destination.IsValid) {
                ScriptErrorException.Throw("Invalid destination.");
                return;
            }

            UO.PrintInformation("Moving items of type {0} {1} from {2} to {3}", graphic, color, sourceContainer, destination);

            foreach (UOItem item in source.Items) {
                if (item.Graphic == graphic && item.Color == color) {
                    using (ItemUpdateEventWaiter ew = new ItemUpdateEventWaiter(item)) {
                        item.Move(0, destination);
                        ew.Wait(2000);
                        UO.Wait(200);
                    }
                }
            }

            UO.PrintInformation("MassMove finished.");
        }
		}
	}
	
	 