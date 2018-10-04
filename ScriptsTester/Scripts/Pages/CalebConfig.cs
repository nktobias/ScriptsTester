using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using Phoenix.WorldData;

namespace CalExtension
{
  [RuntimeObject]
  public class CalebConfig
  {
    private static bool autoHeal;
    private static bool healMoby;
    private static bool destroySpiderWeb;
    private static bool handleFrozen;
    private static RenameType rename;
    private static int healMinDamagePerc;
    private static int attackDelay;
    private static bool useWatcher;

    private static MessagePrintType friendHitsMessageType;
    private static MessagePrintType enemyHitsMessageType;
    private static MessagePrintType castMessageType;
    private static MessagePrintType playerHitsMessageType; 

    private static bool useWallTime;

    private static string lootItemTypes;

    private static AutoHealType healType;
    private static LootType loot;

    private static bool trainPoison;

    private static readonly DefaultPublicEvent propertyChanged = new DefaultPublicEvent();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:CorpsesAutoopen"/> class.
    /// </summary>
    public CalebConfig()
    {
      // Pozn: Snazim se mit staticky jen uplne minimum, protoze co je v instanci sezere pri rekompilaci GC

      autoHeal = Config.Profile.UserSettings.GetAttribute(true, "Value", "AutoHeal");
      healMoby = Config.Profile.UserSettings.GetAttribute(true, "Value", "HealMoby");
      destroySpiderWeb = Config.Profile.UserSettings.GetAttribute(true, "Value", "DestroySpiderWeb");
      handleFrozen = Config.Profile.UserSettings.GetAttribute(true, "Value", "HandleFrozen");
      rename = (RenameType)Config.Profile.UserSettings.GetAttribute((int)RenameType.OnAppeared, "Value", "Rename");
      healMinDamagePerc = Config.Profile.UserSettings.GetAttribute(0, "Value", "HealMinDamagePerc");
      attackDelay = Config.Profile.UserSettings.GetAttribute(1500, "Value", "AttackDelay");
      useWatcher = Config.Profile.UserSettings.GetAttribute(false, "Value", "UseWatcher");

      healType = (AutoHealType)Config.Profile.UserSettings.GetAttribute((int)AutoHealType.Automatic, "Value", "HealType");
      loot = (LootType)Config.Profile.UserSettings.GetAttribute((int)LootType.OpenCorpse, "Value", "Loot");

      trainPoison = Config.Profile.UserSettings.GetAttribute(true, "Value", "TrainPoison");

      friendHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "FriendHitsMessageType");
      enemyHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "EnemyHitsMessageType");
      castMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "CastMessageType");
      playerHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "PlayerHitsMessageType");

      useWallTime = Config.Profile.UserSettings.GetAttribute(true, "Value", "UseWallTime");

      lootItemTypes = Config.Profile.UserSettings.GetAttribute(CalExtension.UOExtensions.Loot.LootDefaultItemTypes, "Value", "LootItemTypes");

      Config.Profile.UserSettings.Loaded += new EventHandler(UserSettings_Loaded);
      Config.Profile.UserSettings.Saving += new EventHandler(UserSettings_Saving);
    }

    #region Configuration

    void UserSettings_Loaded(object sender, EventArgs e)
    {
      autoHeal = Config.Profile.UserSettings.GetAttribute(true, "Value", "AutoHeal");
      healMoby = Config.Profile.UserSettings.GetAttribute(true, "Value", "HealMoby");
      destroySpiderWeb = Config.Profile.UserSettings.GetAttribute(true, "Value", "DestroySpiderWeb");
      handleFrozen = Config.Profile.UserSettings.GetAttribute(true, "Value", "HandleFrozen");
      rename = (RenameType)Config.Profile.UserSettings.GetAttribute((int)RenameType.OnAppeared, "Value", "Rename");

      healMinDamagePerc = Config.Profile.UserSettings.GetAttribute(0, "Value", "HealMinDamagePerc");
      attackDelay = Config.Profile.UserSettings.GetAttribute(1500, "Value", "AttackDelay");
      useWatcher = Config.Profile.UserSettings.GetAttribute(false, "Value", "UseWatcher");

      healType = (AutoHealType)Config.Profile.UserSettings.GetAttribute((int)AutoHealType.Automatic, "Value", "HealType");
      loot = (LootType)Config.Profile.UserSettings.GetAttribute((int)LootType.OpenCorpse, "Value", "Loot");

      trainPoison = Config.Profile.UserSettings.GetAttribute(true, "Value", "TrainPoison");

      friendHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "FriendHitsMessageType");
      enemyHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "EnemyHitsMessageType");
      castMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "CastMessageType");
      playerHitsMessageType = (MessagePrintType)Config.Profile.UserSettings.GetAttribute((int)MessagePrintType.Default, "Value", "PlayerHitsMessageType");

      useWallTime = Config.Profile.UserSettings.GetAttribute(true, "Value", "UseWallTime");

      lootItemTypes = Config.Profile.UserSettings.GetAttribute(CalExtension.UOExtensions.Loot.LootDefaultItemTypes, "Value", "LootItemTypes");

      OnPropertyChanged(EventArgs.Empty);
    }

    void UserSettings_Saving(object sender, EventArgs e)
    {
      Config.Profile.UserSettings.SetAttribute(autoHeal, "Value", "AutoHeal");
      Config.Profile.UserSettings.SetAttribute(healMoby, "Value", "HealMoby");
      Config.Profile.UserSettings.SetAttribute(destroySpiderWeb, "Value", "DestroySpiderWeb");
      Config.Profile.UserSettings.SetAttribute(handleFrozen, "Value", "HandleFrozen");
      Config.Profile.UserSettings.SetAttribute((int)rename, "Value", "Rename");

      Config.Profile.UserSettings.SetAttribute((int)healMinDamagePerc, "Value", "HealMinDamagePerc");
      Config.Profile.UserSettings.SetAttribute((int)attackDelay, "Value", "AttackDelay");
      Config.Profile.UserSettings.SetAttribute(useWatcher, "Value", "UseWatcher");


      Config.Profile.UserSettings.SetAttribute((int)healType, "Value", "HealType");
      Config.Profile.UserSettings.SetAttribute((int)loot, "Value", "Loot");

      Config.Profile.UserSettings.SetAttribute(trainPoison, "Value", "TrainPoison");

      Config.Profile.UserSettings.SetAttribute((int)friendHitsMessageType, "Value", "FriendHitsMessageType");
      Config.Profile.UserSettings.SetAttribute((int)enemyHitsMessageType, "Value", "EnemyHitsMessageType");
      Config.Profile.UserSettings.SetAttribute((int)castMessageType, "Value", "CastMessageType");
      Config.Profile.UserSettings.SetAttribute((int)playerHitsMessageType, "Value", "PlayerHitsMessageType");

      Config.Profile.UserSettings.SetAttribute(useWallTime, "Value", "UseWallTime");

      Config.Profile.UserSettings.SetAttribute(lootItemTypes, "Value", "LootItemTypes");
    }

    public static bool AutoHeal
    {
      get { return autoHeal; }
      set
      {
        if (value != autoHeal)
        {
          autoHeal = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool HealMoby
    {
      get { return healMoby; }
      set
      {
        if (value != healMoby)
        {
          healMoby = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool DestroySpiderWeb
    {
      get { return destroySpiderWeb; }
      set
      {
        if (value != destroySpiderWeb)
        {
          destroySpiderWeb = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }


    public static bool HandleFrozen
    {
      get { return handleFrozen; }
      set
      {
        if (value != handleFrozen)
        {
          handleFrozen = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }


    public static RenameType Rename
    {
      get { return rename; }
      set
      {
        if (value != rename)
        {
          rename = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static LootType Loot
    {
      get { return loot; }
      set
      {
        if (value != loot)
        {
          loot = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static AutoHealType HealType
    {
      get { return healType; }
      set
      {
        if (value != healType)
        {
          healType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static int HealMinDamagePerc
    {
      get { return healMinDamagePerc; }
      set
      {
        if (value != healMinDamagePerc)
        {
          healMinDamagePerc = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static int AttackDelay
    {
      get { return attackDelay; }
      set
      {
        if (value != attackDelay)
        {
          attackDelay = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool UseWatcher
    {
      get { return useWatcher; }
      set
      {
        if (value != useWatcher)
        {
          useWatcher = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool TrainPoison
    {
      get { return trainPoison; }
      set
      {
        if (value != trainPoison)
        {
          trainPoison = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType CastMessageType
    {
      get { return castMessageType; }
      set
      {
        if (value != castMessageType)
        {
          castMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType FriendHitsMessageType
    {
      get { return friendHitsMessageType; }
      set
      {
        if (value != friendHitsMessageType)
        {
          friendHitsMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType EnemyHitsMessageType
    {
      get { return enemyHitsMessageType; }
      set
      {
        if (value != enemyHitsMessageType)
        {
          enemyHitsMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static MessagePrintType PlayerHitsMessageType
    {
      get { return playerHitsMessageType; }
      set
      {
        if (value != playerHitsMessageType)
        {
          playerHitsMessageType = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static bool UseWallTime
    {
      get { return useWallTime; }
      set
      {
        if (value != useWallTime)
        {
          useWallTime = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static string LootItemTypes
    {
      get { return lootItemTypes; }
      set
      {
        if (value != lootItemTypes)
        {
          lootItemTypes = value;
          OnPropertyChanged(EventArgs.Empty);
        }
      }
    }

    public static event EventHandler PropertyChanged
    {
      add { propertyChanged.AddHandler(value); }
      remove { propertyChanged.RemoveHandler(value); }
    }

    protected static void OnPropertyChanged(EventArgs e)
    {
      propertyChanged.InvokeAsync(null, e);
    }



    #endregion

    public static IEnumerable<KeyValuePair<int, string>> Of<T>()
    {
      return Enum.GetValues(typeof(T))
          .Cast<T>()
          .Select(p => new KeyValuePair<int, string>(Convert.ToInt32(p), p.ToString()))
          .ToList();
    }

  }


}





