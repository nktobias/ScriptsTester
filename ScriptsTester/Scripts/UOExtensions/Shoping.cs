using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Communication;

namespace CalExtension.UOExtensions
{
  public class Shoping
  {
    [Executable]
    public static void BuyManual()
    {
      UOObject vendor = new UOObject(UIManager.TargetObject());

      if (vendor.Exist)
      {
        UO.Say(vendor.Name + " buy ");
      }
    }

    [Executable]
    public static void SellManual()
    {
      UOObject vendor = new UOObject(UIManager.TargetObject());

      if (vendor.Exist)
      {
        UO.Say(vendor.Name + " sell ");
      }
    }
  }
}
