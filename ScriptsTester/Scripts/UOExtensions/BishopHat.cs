using Phoenix.WorldData;
using Phoenix;

namespace CalExtension.UOExtensions
{
  [RuntimeObject]
  public class BishopHat
  {
    //------------------------------------------------------------------------------------------

    public static readonly Graphic BishopGraphic = 0x1DB9;
    public static readonly UOColor BishopColor = 0x0BB0;

    //------------------------------------------------------------------------------------------

    private UOItem lastHeadArmor;
    private ushort? lastHatX;
    private ushort? lastHatY;
    private Serial lastHatContainer = Serial.Invalid;

    //------------------------------------------------------------------------------------------

    [Executable]
    public void SwitchBishopHat()
    {
      UOItem bishopHat = World.Player.FindType(BishopGraphic, BishopColor);

      if (bishopHat.ExistCust())
      {
        if (bishopHat.Layer == Layer.Hat)
        {
          UOItem lastContainer = new UOItem(lastHatContainer);
          if (!lastContainer.ExistCust())
            lastContainer = World.Player.Backpack;

          if (lastHeadArmor != null && lastHeadArmor.ExistCust())
            lastHeadArmor.Use();
          else
            bishopHat.Move(1, lastContainer, lastHatX.GetValueOrDefault(0), lastHatY.GetValueOrDefault(0));

          World.Player.Print(0x005d, "<Bishop sundan>");
        }
        else
        {
          lastHeadArmor = World.Player.Layers[Layer.Hat];
          lastHatContainer = bishopHat.Container;
          lastHatX = bishopHat.X;
          lastHatY = bishopHat.Y;
          bishopHat.Use();

          Game.Wait(100);

          World.Player.Print(0x0044, "<Bishop nasazen " + World.Player.Hits + ">");
        }
      }
      else
        World.Player.PrintMessage("[Nemas Bishop hat!]", MessageType.Error);

      //if (World.Player.Layers[Layer.Hat].Graphic == BishopGraphic && World.Player.Layers[Layer.Hat].Color == BishopColor)
      //{
      //  if (World.Player.Hits >= 50)
      //  {
      //    if (lastHatX.HasValue && lastHatY.HasValue && lastHatContainer != null && lastHatContainer != Serial.Invalid)
      //    {
      //      World.Player.Layers[Layer.Hat].Move(1, lastHatContainer, lastHatX.Value, lastHatY.Value);
      //    }
      //    else
      //      World.Player.Layers[Layer.Hat].Move(1, World.Player.Backpack);

      //    World.Player.Print(0x005d, "<Bishop sundan>");

      //    Game.Wait();

      //    if (lastHeadArmor != null && lastHeadArmor.Exist)
      //      lastHeadArmor.Use();
      //  }
      //  else
      //    World.Player.PrintHitsMessage("[Mas malo HP!]");
      //}
      //else if (World.Player.Backpack.AllItems.FindType(BishopGraphic, BishopColor).Exist)
      //{
      //  UOItem bishop = World.Player.Backpack.AllItems.FindType(BishopGraphic, BishopColor);
      //  lastHeadArmor = World.Player.Layers[Layer.Hat];
      //  lastHatContainer = bishop.Container;
      //  lastHatX = bishop.X;
      //  lastHatY = bishop.Y;

      //  bishop.Use();

      //  Game.Wait(100);

      //  World.Player.Print(0x0044, "<Bishop nasazen " + World.Player.Hits + ">");
      //}
      //else
      //{
      //  World.Player.PrintMessage("[Nemas Bishop hat!]", MessageType.Error);
      //}
    }

    //------------------------------------------------------------------------------------------
  }
}