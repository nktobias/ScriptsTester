using Phoenix;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalExtension.UOExtensions
{
  public class Utils
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void PrintCursor()
    {
      Game.PrintMessage("[" + System.Windows.Forms.Cursor.Position.X + ":" + System.Windows.Forms.Cursor.Position.Y + "]");
    }
    //---------------------------------------------------------------------------------------------

    public static int? ToNullInt(object value)
    {
      if (value == null) return null;
      try { return Int32.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static bool? ToNullBool(object value)
    {
      try { return bool.Parse(value.ToString()); }
      catch { return null; }
    }

    //---------------------------------------------------------------------------------------------

    public static string JoinDelimitedString(string text, string joinString, string delimiter)
    {
      if (String.IsNullOrEmpty(text)) 
      {
        if (String.IsNullOrEmpty(joinString)) return String.Empty;
        else return joinString;
      }
      else return text + delimiter + joinString;
    }

    //---------------------------------------------------------------------------------------------

    public static int GetSwitchIndex(int selected, int direction, int maxCount)
    {
      int index = selected + direction;
      if (index > maxCount - 1) index = 0;
      else if (index < 0) index = maxCount - 1;
      return index;
    }

    //[ClientMessageHandler(0x1C)]

    //public CallbackResult OnTEXT(byte[] data, CallbackResult prevState)//TODO !!!! zrusit atack hlasky !!!!!
    //{
    //  if (prevState == CallbackResult.Normal)
    //  {
    //  }
    //  return CallbackResult.Normal;
    //}
    ////0x1C

    ////    [23:35:17] Phoenix->Client: Packet id: 0x1C; 84 bytes:
    ////1C 00 54 00 1C 87 02 01 90 06 00 22 00 03 52 75 6D 70 65 6C 73 74 69 6C 74 ..T...........Rumpelstilt
    ////73 6B 69 6E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2A 59 6F 75 20 73 skin................You.s
    ////65 65 20 52 75 6D 70 65 6C 73 74 69 6C 74 73 6B 69 6E 20 61 74 74 61 63 6B ee.Rumpelstiltskin.attack
    ////69 6E 67 20 79 6F 75 2A 00                                                 ing.you..


    //---------------------------------------------------------------------------------------------
  }
}