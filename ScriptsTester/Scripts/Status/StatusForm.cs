﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using System.Windows.Forms;
using Phoenix.WorldData;
using System.Drawing;
using Phoenix.Gui.Controls;
using MulLib;
using Phoenix.Communication.Packets;
using Phoenix.Communication;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;
using CalExtension.Skills;
using CalExtension.UOExtensions;

namespace CalExtension.UI.Status
{
  public enum StatusType
  {
    Enemy = 1,
    Friend = 2,
    Mob = 3,
    Player = 4
  }


  public class StatusForm : InGameWindow
  {
    //---------------------------------------------------------------------------------------------

    private UOColor[] notorietyColors = new UOColor[] { Env.DefaultConsoleColor, 0x0063, 0x0044, 0x03E9, 0x03E5, 0x0030, 0x0026, 0x0481 };

    private Label name;
    private Label hits;
    private Label mana;
    private Label stam;
    private Label ar;
    private Label weight;
    private CheckBox chbxKeep;
    private CheckBox chbxHeal;
    private CheckBox chbxAlie;
    private Button btnA;
    private Button btnB;
    private Button btnX;
    private Button btnY;

    private Label runLabel;
    private Label notifiyLabel;

    private UOCharacter mobile;
    private HealthBar healthBar;
    private ManaBar manaBar;
    private StaminaBar staminaBar;

    private WorldLocation lastloc;
    private bool lastWar;
    private DateTime lastRequest;
    private bool statusNeeded = true;

    public bool Transparency { get; set; }
    public StatusType StatusType { get; set; }

    private Color? DefaultColor;
    private Color? CurrentColor;

    private DateTime initTime;
    private short lastMaxHits;
    private bool mouseHovering = false;

    bool statDown = false;
    private double lastPositionAngle = 0;
    private IUOPosition lastPositon = new UOPositionBase(0, 0, 0);
    //    private StatusFormWrapper currentWrapper;

    public bool DisableWrapper = false;
    public bool Manual = false;

    //---------------------------------------------------------------------------------------------

    public bool MouseHovering
    {
      get { return this.mouseHovering;  }
    }

    //---------------------------------------------------------------------------------------------

    public bool Keep
    {
      get { return this.chbxKeep.Checked; }
    }

    //---------------------------------------------------------------------------------------------

    public bool IsNew
    {
      get { return (DateTime.Now - initTime).TotalSeconds < 5; }
    }

    //---------------------------------------------------------------------------------------------

    public DateTime InitTime
    {
      get { return this.initTime;  }
    }


    //---------------------------------------------------------------------------------------------

    public Serial MobileId
    {
      get { return mobile.Serial; }
    }

    //---------------------------------------------------------------------------------------------

    protected override bool Targettable
    {
      get { return true; }
    }

    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="T:StatusForm"/> class.
    /// </summary>
    public StatusForm(Serial id/*, StatusFormWrapper wrapper*/)
    {
      //this.currentWrapper = wrapper;
      initTime = DateTime.Now;
      mobile = new UOCharacter(id);
      InitializeComponent();

      this.chbxAlie.CheckedChanged += ChbxAlie_CheckedChanged;
      this.chbxHeal.CheckedChanged += ChbxHeal_CheckedChanged;

      this.btnA.Click += Btn_Click;
      this.btnB.Click += Btn_Click;
      this.btnX.Click += Btn_Click;
      this.btnY.Click += Btn_Click;

      MouseEnter += new EventHandler(StatusForm_MouseEnter);

      MouseLeave += new EventHandler(StatusForm_MouseLeave);
      MouseDoubleClick += new MouseEventHandler(StatusForm_MouseDoubleClick);
      MouseClick += StatusForm_MouseClick;
      Target += new EventHandler(StatusForm_Target);

      if (id == World.Player.Serial)
        this.StatusType = StatusType.Player;
      else if (mobile.Renamable)
        this.StatusType = StatusType.Mob;
      else if (mobile.Notoriety == Notoriety.Guild || mobile.Notoriety == Notoriety.Innocent || mobile.Notoriety == Notoriety.Neutral || Game.CurrentGame.IsAlie(MobileId))
        this.StatusType = StatusType.Friend;
      else
        this.StatusType = StatusType.Enemy;

      lastMaxHits = mobile.MaxHits;

      if (this.StatusType != StatusType.Player)
      {
        if (this.StatusType == StatusType.Mob)
        {
          if (this.mobile.Model == 0x00D6 && this.mobile.Color == 0x0972)//Leopard
          {
            this.chbxKeep.Checked = true;
            this.chbxHeal.Checked = true;
            this.chbxAlie.Checked = true;
          }
          else if (this.mobile.Model == 0x0005 && this.mobile.Color == 0x0847)//Skyhawk // TODO grizly
          {
            this.chbxKeep.Checked = true;
            this.chbxHeal.Checked = true;
            this.chbxAlie.Checked = true;
          }
          else if (this.mobile.Model == 0x00D4 && this.mobile.Color == 0x0000)//grizly
          {
            this.chbxKeep.Checked = true;
            this.chbxHeal.Checked = true;
            this.chbxAlie.Checked = true;
          }
        }
      }
      else
      {
        this.StartPosition = FormStartPosition.Manual;

        int x = Config.Profile.UserSettings.GetAttribute(this.Location.X, "Value", "StatusForm_Player_LocationX");
        int y = Config.Profile.UserSettings.GetAttribute(this.Location.Y, "Value", "StatusForm_Player_LocationY");
        this.Location = new Point(x, y);

        Game.CurrentGame.RunscriptRun += new UOExtensions.RunscriptCounterEventHandler(Rc_RunscriptRun);
      }
    }



    //---------------------------------------------------------------------------------------------

    private void AddRemoveHealAlie()
    {
      if (this.chbxHeal != null)
      {
        try
        {

          if (this.chbxHeal.Checked)
          {
            this.chbxAlie.Checked = true;
            this.chbxKeep.Checked = true;

            Game.CurrentGame.HealAlies.Add(this.mobile);
            Game.PrintMessage("HealAlie Add");
          }
          else
          {
            for (int i = Game.CurrentGame.HealAlies.Count - 1; i > -1; i--)
            {
              try
              {
                if (Game.CurrentGame.HealAlies[i].Serial == this.mobile.Serial)
                {
                  Game.CurrentGame.HealAlies.RemoveAt(i);

                  Game.PrintMessage("HealAlie Remove");
                }
              }
              catch (Exception ex)
              {
                Game.PrintMessage("Err " + ex.Message);
              }
            }
          }
        }
        catch
        {

          Game.PrintMessage("AddRemoveHealAlie Fail");
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    private void AddRemoveAlie()
    {
      if (this.chbxAlie != null)
      {
        try
        {

          if (this.chbxAlie.Checked)
          {
            this.chbxKeep.Checked = true;
            Game.CurrentGame.Alies.Add(this.mobile);
            Game.PrintMessage("Alie Add");
          }
          else
          {
            for (int i = Game.CurrentGame.Alies.Count - 1; i > -1; i--)
            {
              try
              {
                if (Game.CurrentGame.Alies[i].Serial == this.mobile.Serial)
                {
                  Game.CurrentGame.Alies.RemoveAt(i);

                  Game.PrintMessage("Alie Remove");
                }
              }
              catch (Exception ex)
              {
                Game.PrintMessage("Err " + ex.Message);
              }
            }
          }
        }
        catch
        {

          Game.PrintMessage("AddRemoveAlie Fail");
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    private void UpdatePositionAndDirection()
    {
      if (this.StatusType != StatusType.Player)
      {
        string sufix = "-";
        double distance = 0;

        if (mobile.ExistCust() && mobile.Hits > -1)
        {
          lastPositon = mobile.GetPosition();
        }

        distance = Robot.GetRealDistance(World.Player.GetPosition(), lastPositon);
        lastPositionAngle = Robot.GetAngle(World.Player.GetPosition(), lastPositon);

        MovementDirection direction = Robot.GetMovementDirection(lastPositionAngle);
        if (direction == MovementDirection.Up)
          sufix = "↑";
        else if (direction == MovementDirection.UpRight)
          sufix = "↗";
        else if (direction == MovementDirection.Right)
          sufix = "→";
        else if (direction == MovementDirection.DownRight)
          sufix = "↘";
        else if (direction == MovementDirection.Down)
          sufix = "↓";
        else if (direction == MovementDirection.DownLeft)
          sufix = "↙";
        else if (direction == MovementDirection.Left)
          sufix = "←";
        else if (direction == MovementDirection.UpLeft)
          sufix = "↖";
        
        Color c = Color.Transparent;

        if (distance <= 1)
          c = Color.FromArgb(0, 128, 255);
        else if (distance <= 5)
          c = Color.FromArgb(0, 191, 255);
        else if (distance <= 10)
          c = Color.FromArgb(0, 255, 255);
        else if (distance <= 15)
          c = Color.FromArgb(0, 255, 191);
        else if (distance > 15)
          c = Color.FromArgb(0, 255, 128);


        this.runLabel.BackColor = c;

        if (distance > 99 || distance < 0)
          distance = 0;

        //-/|\---↖↗↘↙↑← →↓
        string runLabelText = String.Format("{0:N0}", distance);
        runLabelText += " " + sufix;
        this.runLabel.Text = runLabelText;
        this.runLabel.Invalidate();
      }
    }

    //---------------------------------------------------------------------------------------------

    private void UpdateStats()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new ThreadStart(UpdateStats));
        return;
      }

      if (mobile.Name != null)
        name.Text = mobile.Name;

      bool isDmg = lastMaxHits > mobile.MaxHits && mobile.MaxHits > -1;

      if (mobile.MaxHits > -1)
      {

        hits.Text = String.Format("{0}/{1}", mobile.Hits, mobile.MaxHits);
        healthBar.Hits = mobile.Hits;
        healthBar.MaxHits = mobile.MaxHits;
        healthBar.Poison = mobile.Poisoned;
        healthBar.Unknown = false;
        statusNeeded = false;

        if (this.StatusType == StatusType.Player)
        {
          mana.Text = String.Format("{0}/{1}", mobile.Mana, mobile.MaxMana);
          manaBar.Mana = mobile.Mana;
          manaBar.MaxMana = mobile.MaxMana;
          manaBar.Unknown = false;

          stam.Text = String.Format("{0}/{1}", mobile.Stamina, mobile.MaxStamina);
          staminaBar.Stam = mobile.Stamina;
          staminaBar.MaxStam = mobile.MaxStamina;
          staminaBar.Unknown = false;

          ar.Text = String.Format("ar: {0}", World.Player.Armor);
          weight.Text = String.Format("w: {0}", World.Player.Weight);
        }
        else
        {
          UpdatePositionAndDirection();

          lastMaxHits = mobile.MaxHits;
        }
      }
      else
      {
        healthBar.Unknown = true;

        if (this.StatusType == StatusType.Player)
        {
          staminaBar.Unknown = true;
          manaBar.Unknown = true;
        }
        else
        {
          if (this.chbxKeep.Checked)
          {
            UpdatePositionAndDirection();
          }
          else
          {
            int limit = 8;

            if ((DateTime.Now - initTime).TotalSeconds > limit && mobile.Hits <= 1)
            {
              WindowManager.GetDefaultManager().BeginInvoke(Close);
              return;
            }
          }
        }
      }

      Notoriety n = mobile.Notoriety;
      if (mobile.Distance > 18)
      {
        n = Notoriety.Unknown;
        healthBar.Unknown = true;
        if (mobile.Serial == World.Player.Serial)
        {
          staminaBar.Unknown = true;
          manaBar.Unknown = true;
        }
      }

      HueEntry notoh = DataFiles.Hues.Get(notorietyColors[(int)n]);
      ushort noto = notoh.Colors[12];

      if (statDown && this.chbxAlie.Checked)
        noto = DataFiles.Hues.Get(0x01c0).Colors[12];
      else
      {
        if (this.StatusType == StatusType.Mob)
          noto = DataFiles.Hues.Get(0x0035).Colors[12];

        if (this.StatusType == StatusType.Player && Game.IsHealOn)
          noto = DataFiles.Hues.Get(0x0035).Colors[12];
      }

      if (!DefaultColor.HasValue || !MouseHovering)
      {
        if (this.StatusType == StatusType.Player)
          BackColor = Color.Black;
        else
          BackColor = Color.FromArgb(UOColorConverter.ToArgb(noto) | (0xFF << 24));

        if (!DefaultColor.HasValue)
          DefaultColor = CurrentColor = BackColor;
      }

      if (healthBar.Unknown)
        statusNeeded = true;

      // Request if mob is visible
      if (statusNeeded && mobile.Distance < 22 && DateTime.Now - lastRequest < TimeSpan.FromSeconds(10))
      {
        statusNeeded = false;
        lastRequest = DateTime.Now;
        mobile.RequestStatus();
      }

      //if (this.currentWrapper != null)
      //{
      //  if ((DateTime.Now - initTime).TotalSeconds > 2 && (this.healthBar.Unknown || this.mobile.Hits <= 0 || this.mobile.Distance > 18))
      //  {
      //    WindowManager.GetDefaultManager().BeginInvoke(Close);
      //    return;
      //  }
      //  else
      //    this.currentWrapper.UpdateLayout();
      //}
    }

    //---------------------------------------------------------------------------------------------

    private RunscriptCounterEventArgs currRunArg = null;
    private void Rc_RunscriptRun(object sender, RunscriptCounterEventArgs e)
    {
      currRunArg = e;

      if (InvokeRequired)
      {
        BeginInvoke(new ThreadStart(UpdateRunScript));
        return;
      }

      UpdateRunScript();
    }

    //---------------------------------------------------------------------------------------------

    private void UpdateRunScript()
    {
      if (currRunArg != null)
      {
        bool change = false;
        if (currRunArg.IsStoped)
        {
          change = true;
          this.runLabel.Text = String.Format("{0:N1}", 0);
          this.runLabel.BackColor = Color.Transparent;
        }
        else if (currRunArg.Count == 0 || currRunArg.Count % 50 == 0)
        {
          change = true;
          this.runLabel.Text = String.Format("{0:N1}", (currRunArg.Duration / 1000m));
          this.runLabel.BackColor = Color.Coral;
        }

        if (change)
        {
          this.runLabel.Invalidate();
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    #region Events

    //---------------------------------------------------------------------------------------------

    private void Btn_Click(object sender, EventArgs e)
    {
      string commandName = ((Button)sender).Name[((Button)sender).Name.Length - 1].ToString();

      Game.OnUserAction(commandName, new StatusFormActionEventArgs() { mobile = mobile });
    }

    //---------------------------------------------------------------------------------------------

    private void ChbxAlie_CheckedChanged(object sender, EventArgs e)
    {
      this.AddRemoveAlie();
    }

    //---------------------------------------------------------------------------------------------

    private void ChbxHeal_CheckedChanged(object sender, EventArgs e)
    {
      this.AddRemoveHealAlie();
    }

    //---------------------------------------------------------------------------------------------

    private void StatusForm_MouseClick(object sender, MouseEventArgs e)
    {
      this.mobile.RequestStatus(100);

      if ((Game.IsMob(mobile.Serial) && Game.IsMobActive(mobile.Serial)) || mobile.Renamable)
      {
        Aliases.SetObject("SelectedMob", mobile.Serial);
      }
    }


    //---------------------------------------------------------------------------------------------
    Point? dragStart = null;
    protected override void OnMove(EventArgs e)
    {
      if (this.MouseHovering)
      {
        if (dragStart == null)
          dragStart = new Point(this.Location.X, this.Location.Y);

        //string str = "OnMove " + dragStart;
        //str = str.Replace("{", "[").Replace("}", "]");

        //Game.PrintMessage(str);
        if (!DisableWrapper)
        {
          int moveX = Math.Abs(this.Location.X - (this.dragStart.HasValue ? this.dragStart.Value.X : this.Location.X));
          int moveY = Math.Abs(this.Location.Y - (this.dragStart.HasValue ? this.dragStart.Value.Y : this.Location.Y));

          //Game.PrintMessage(str + " - " + moveX + " : " + moveY);
          if (moveX > 10 || moveY > 10)
            DisableWrapper = true;
        }
      }

      if (this.StatusType == StatusType.Player)
      {
        Config.Profile.UserSettings.SetAttribute(this.Location.X, "Value", "StatusForm_Player_LocationX");
        Config.Profile.UserSettings.SetAttribute(this.Location.Y, "Value", "StatusForm_Player_LocationY");
      }

      base.OnMove(e);
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      mobile.Changed += new ObjectChangedEventHandler(mobile_Changed);
      World.Player.Changed += new ObjectChangedEventHandler(Player_Changed);
      World.CharacterAppeared += new CharacterAppearedEventHandler(World_CharacterAppeared);

      if (Transparency)
      {
        Opacity = 0.9;
      }

      UpdateStats();
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);
      UpdateStats();
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnClosed(EventArgs e)
    {
      mobile.Changed -= mobile_Changed;

      if (MouseHovering || Aliases.GetObject("CurrentHoverStatus") == mobile.Serial)
        Aliases.SetObject("CurrentHoverStatus", Serial.Invalid);

      base.OnClosed(e);
    }

    //---------------------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
      if (MouseHovering || Aliases.GetObject("CurrentHoverStatus") == mobile.Serial)
        Aliases.SetObject("CurrentHoverStatus", Serial.Invalid);

      mobile.Changed -= new ObjectChangedEventHandler(mobile_Changed);
      World.Player.Changed -= new ObjectChangedEventHandler(Player_Changed);
      World.CharacterAppeared -= new CharacterAppearedEventHandler(World_CharacterAppeared);
      Game.CurrentGame.RunscriptRun -= new UOExtensions.RunscriptCounterEventHandler(Rc_RunscriptRun);

      this.chbxAlie.Checked = false;
      this.chbxHeal.Checked = false;
      this.AddRemoveHealAlie();
      this.AddRemoveAlie();
      base.Dispose(disposing);
    }



    //---------------------------------------------------------------------------------------------

    protected void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      if (e.Serial == mobile.Serial)
      {
        UpdateStats();
      }
    }

    //---------------------------------------------------------------------------------------------

    protected void Player_Changed(object sender, ObjectChangedEventArgs e)
    {
      if (WalkHandling.DesiredPosition.X != lastloc.X && WalkHandling.DesiredPosition.Y != lastloc.Y)
      {
        UpdateStats();
      }
      if (World.Player.Warmode != lastWar)
      {
        lastWar = World.Player.Warmode;
        UpdateCursor();
      }
    }

    //---------------------------------------------------------------------------------------------

    private void mobile_Changed(object sender, ObjectChangedEventArgs e)
    {
      UpdateStats();
    }

    public static Serial prevEnter = Serial.Invalid;
    public static DateTime prevEnterTime = DateTime.Now;
    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseEnter(object sender, EventArgs e)
    {
      mouseHovering = true;
      Aliases.SetObject("CurrentHoverStatus", mobile.Serial);

      UpdatePositionAndDirection();
      BackColor = Color.BlueViolet;

      if (prevEnter.IsValidCust() && new UOCharacter(prevEnter).ExistCust() && (DateTime.Now - prevEnterTime).TotalSeconds < 3 && prevEnter != this.MobileId)
        new UOCharacter(prevEnter).PrintMessage("", Game.Val_Green, true);

      prevEnter = this.MobileId;
      prevEnterTime = DateTime.Now;


      if (this.MobileId != World.Player.Serial && this.mobile.ExistCust() && this.mobile.Hits > 0 && this.mobile.GetDistance() < 18)
        this.mobile.PrintMessage("*", Game.Val_Radiation, true);

      this.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseLeave(object sender, EventArgs e)
    {
      mouseHovering = false;

      if (Aliases.GetObject("CurrentHoverStatus") == this.MobileId)
        Aliases.SetObject("CurrentHoverStatus", Serial.Invalid);

      if (this.DefaultColor.HasValue)
      {
        BackColor = this.DefaultColor.Value;
        this.Invalidate();
      }

      dragStart = null;
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnPaintBackground(PaintEventArgs e)
    {
      base.OnPaintBackground(e);
      e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
    }

    //---------------------------------------------------------------------------------------------

    protected void StatusForm_Target(object sender, EventArgs e)
    {
      byte[] data = PacketBuilder.Target(0, UIManager.ClientTargetId, 0, mobile.Serial, mobile.X, mobile.Y, mobile.Z, 0);
      Core.SendToServer(data, true);

      Client.Window.PostMessage(WM_KEYDOWN, 27, 0x00010001);
      UpdateCursor();
    }

    //---------------------------------------------------------------------------------------------

    protected void StatusForm_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (Game.CurrentGame.IsAlie(mobile.Serial) || Game.CurrentGame.IsHealAlie(mobile.Serial) || this.mobile.Serial == World.Player.Serial)
      {
        Healing.LastCharacter = mobile;
        UO.PrintObject(mobile, Game.Val_LightPurple, "[LastChar...]");
      }
      else
      {
        Game.RunScript(CalebConfig.AttackDelay);
        World.Player.ChangeWarmode(WarmodeChange.War);
        UO.Attack(mobile.Serial);
      }
    }

    //---------------------------------------------------------------------------------------------

    #endregion

    #region System

    private void MarkAsAttacked()
    {
      byte[] data = new byte[5];
      data[0] = 0xAA;
      ByteConverter.BigEndian.ToBytes((uint)mobile.Serial, data, 1);

      Core.SendToClient(data, false);
    }

    #endregion

    #region WinForms
    public int OriginalWidth = 190;
    public int OriginalHeight = 50;
    public static int DefaultHeight = 44;

    private void InitializeComponent()
    {
      Font font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      //238

      this.name = new System.Windows.Forms.Label();
      this.hits = new System.Windows.Forms.Label();
      this.healthBar = new CalExtension.UI.Status.HealthBar();
      this.manaBar = new ManaBar();
      this.mana = new Label();
      this.staminaBar = new StaminaBar();
      this.stam = new Label();
      this.ar = new Label();
      this.weight = new Label();
      this.btnA = new Button();
      this.btnB = new Button();
      this.btnX = new Button();
      this.btnY = new Button();
      this.chbxKeep = new CheckBox();
      this.chbxHeal = new CheckBox();
      this.chbxAlie = new CheckBox();
      this.runLabel = new System.Windows.Forms.Label();
      this.notifiyLabel = new Label();

      this.SuspendLayout();

      if (this.mobile.Serial == World.Player.Serial)
      {
        // 
        // name
        // 
        this.runLabel.AutoSize = true;
        this.runLabel.BackColor = System.Drawing.Color.Transparent;
        this.runLabel.Enabled = false;
        this.runLabel.Font = font;
        this.runLabel.Name = "runLabel";
        this.runLabel.Size = new System.Drawing.Size(20, 14);
        this.runLabel.TabIndex = 0;
        this.runLabel.Text = "-";

        this.notifiyLabel.AutoSize = true;
        this.notifiyLabel.BackColor = System.Drawing.Color.Transparent;
        this.notifiyLabel.Enabled = false;
        this.notifiyLabel.Font = font;
        this.notifiyLabel.Name = "notifiyLabel";
        this.notifiyLabel.Size = new System.Drawing.Size(20, 14);
        this.notifiyLabel.TabIndex = 0;
        this.notifiyLabel.Text = "";


        //-4 -2
        this.name.AutoSize = true;
        this.name.BackColor = System.Drawing.Color.Transparent;
        this.name.Enabled = false;
        this.name.Font = font;

        this.name.Location = new System.Drawing.Point(3, 3);
        this.name.Name = "name";
        this.name.Size = new System.Drawing.Size(100, 14);
        this.name.TabIndex = 0;
        this.name.Text = "label1";


        this.ar.AutoSize = true;
        this.ar.BackColor = System.Drawing.Color.Transparent;
        this.ar.Enabled = false;
        this.ar.Font = font;
        this.ar.ForeColor = System.Drawing.Color.Silver;
        this.ar.Location = new System.Drawing.Point(105, 3);
        this.ar.Name = "ar";
        this.ar.Size = new System.Drawing.Size(40, 14);
        this.ar.TabIndex = 0;
        this.ar.Text = "ar: 0000";

        this.weight.AutoSize = true;
        this.weight.BackColor = System.Drawing.Color.Transparent;
        this.weight.Enabled = false;
        this.weight.Font = font;
        this.weight.ForeColor = System.Drawing.Color.Silver;
        this.weight.Location = new System.Drawing.Point(142, 3);
        this.weight.Name = "weight";
        this.weight.Size = new System.Drawing.Size(40, 14);
        this.weight.TabIndex = 0;
        this.weight.Text = "w: 0000";




        // 
        // healthBar
        // 
        this.healthBar.Enabled = false;
        this.healthBar.Hits = 0;
        this.healthBar.Location = new System.Drawing.Point(56, 20);
        this.healthBar.MaxHits = 0;
        this.healthBar.Name = "healthBar";
        this.healthBar.Size = new System.Drawing.Size(128, 10);
        this.healthBar.TabIndex = 2;
        this.healthBar.Text = "healthBar1";
        this.healthBar.Unknown = false;

        this.manaBar.Enabled = false;
        this.manaBar.Mana = 0;
        this.manaBar.Location = new System.Drawing.Point(56, 34);
        this.manaBar.MaxMana = 0;
        this.manaBar.Name = "manaBar";
        this.manaBar.Size = new System.Drawing.Size(128, 10);
        this.manaBar.TabIndex = 2;
        this.manaBar.Text = "manaBar1";
        this.manaBar.Unknown = false;
        //-4 -2

        this.staminaBar.Enabled = false;
        this.staminaBar.Stam = 0;
        this.staminaBar.Location = new System.Drawing.Point(56, 48);
        this.staminaBar.MaxStam = 0;
        this.staminaBar.Name = "staminaBar";
        this.staminaBar.Size = new System.Drawing.Size(128, 10);
        this.staminaBar.TabIndex = 2;
        this.staminaBar.Text = "staminaBar1";
        this.staminaBar.Unknown = false;


        // 
        // hits
        // 
        this.hits.AutoSize = true;
        this.hits.BackColor = System.Drawing.Color.Transparent;
        this.hits.Enabled = false;
        this.hits.Font = font;

        this.hits.ForeColor = System.Drawing.Color.Silver;
        this.hits.Location = new System.Drawing.Point(3, 18);
        this.hits.Name = "hits";
        this.hits.Size = new System.Drawing.Size(50, 10);
        this.hits.TabIndex = 1;
        this.hits.Text = "0000/0000";

        this.mana.AutoSize = true;
        this.mana.BackColor = System.Drawing.Color.Transparent;
        this.mana.Enabled = false;
        this.mana.Font = font;
        this.mana.ForeColor = System.Drawing.Color.Silver;
        this.mana.Location = new System.Drawing.Point(3, 32);
        this.mana.Name = "hits";
        this.mana.Size = new System.Drawing.Size(50, 10);
        this.mana.TabIndex = 1;
        this.mana.Text = "0000/0000";

        this.stam.AutoSize = true;
        this.stam.BackColor = System.Drawing.Color.Transparent;
        this.stam.Enabled = false;
        this.stam.Font = font;
        this.stam.ForeColor = System.Drawing.Color.Silver;
        this.stam.Location = new System.Drawing.Point(3, 46);
        this.stam.Name = "hits";
        this.stam.Size = new System.Drawing.Size(50, 10);
        this.stam.TabIndex = 1;
        this.stam.Text = "0000/0000";
        // 
        // StatusForm
        // 

        //int lastX = 0;
        int lastY = 0;


        lastY = staminaBar.Location.Y + staminaBar.Size.Height;



        this.ClientSize = new System.Drawing.Size(190, lastY + 20);
        this.Controls.Add(this.manaBar);
        this.Controls.Add(this.mana);

        this.Controls.Add(this.staminaBar);
        this.Controls.Add(this.stam);
        this.Controls.Add(this.ar);
        this.Controls.Add(this.weight);

        this.Controls.Add(this.btnA);
        this.Controls.Add(this.btnB);
        this.Controls.Add(this.btnX);
        this.Controls.Add(this.btnY);

        this.Controls.Add(this.runLabel);
        //   this.Controls.Add(this.notifiyLabel); TODO

        this.runLabel.Location = new Point(3, lastY);





        this.btnA.Name = "btnA";
        this.btnA.Location = new System.Drawing.Point(56, lastY + 3);
        this.btnA.Size = new System.Drawing.Size(12, 12);
        this.btnA.TabIndex = 1;
        this.btnA.BackColor = Color.LawnGreen;
        this.btnA.FlatStyle = FlatStyle.Flat;
        this.btnA.FlatAppearance.BorderSize = 0;
        this.btnA.ForeColor = System.Drawing.Color.Black;
        this.btnA.TabStop = false;
        this.btnA.Padding = new Padding(0);
        Control prevCont = this.btnA;

        this.btnB.Name = "btnB";
        this.btnB.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.btnB.Size = new System.Drawing.Size(12, 12);
        this.btnB.TabIndex = 1;
        this.btnB.BackColor = Color.Firebrick;
        this.btnB.FlatStyle = FlatStyle.Flat;
        this.btnB.FlatAppearance.BorderSize = 0;
        this.btnB.ForeColor = System.Drawing.Color.Black;
        this.btnB.TabStop = false;
        this.btnB.Padding = new Padding(0);
        prevCont = this.btnB;

        this.btnX.Name = "btnX";
        this.btnX.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.btnX.Size = new System.Drawing.Size(12, 12);
        this.btnX.TabIndex = 1;
        this.btnX.BackColor = Color.Blue;
        this.btnX.FlatStyle = FlatStyle.Flat;
        this.btnX.FlatAppearance.BorderSize = 0;
        this.btnX.ForeColor = System.Drawing.Color.Black;
        this.btnX.TabStop = false;
        this.btnX.Padding = new Padding(0);
        prevCont = this.btnX;

        this.btnY.Name = "btnY";
        this.btnY.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.btnY.Size = new System.Drawing.Size(12, 12);
        this.btnY.TabIndex = 1;
        this.btnY.BackColor = Color.Orange;
        this.btnY.FlatStyle = FlatStyle.Flat;
        this.btnY.FlatAppearance.BorderSize = 0;
        this.btnY.ForeColor = System.Drawing.Color.Black;
        this.btnY.TabStop = false;
        this.btnY.Padding = new Padding(0);
        this.btnY.Margin = new Padding(0);
        prevCont = this.btnY;

        this.chbxHeal.Name = "heal";
        this.chbxHeal.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.chbxHeal.Size = new System.Drawing.Size(12, 12);
        this.chbxHeal.TabIndex = 1;
        this.chbxHeal.BackColor = Color.Transparent;
        prevCont = this.chbxHeal;

        this.chbxAlie.Name = "alie";
        this.chbxAlie.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.chbxAlie.Size = new System.Drawing.Size(12, 12);
        this.chbxAlie.TabIndex = 1;
        this.chbxAlie.BackColor = Color.Transparent;
        prevCont = this.chbxAlie;

        this.chbxKeep.Name = "keep";
        this.chbxKeep.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.chbxKeep.Size = new System.Drawing.Size(12, 12);
        this.chbxKeep.TabIndex = 1;
        this.chbxKeep.BackColor = Color.Transparent;

        this.BackColor = System.Drawing.Color.Black;

        this.Controls.Add(this.hits);
        this.Controls.Add(this.name);
        this.Controls.Add(this.healthBar);

      }
      else
      {
        font = new System.Drawing.Font("Arial", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        int minPading = 2;

        this.name.AutoSize = true;
        this.name.BackColor = System.Drawing.Color.Transparent;
        this.name.Enabled = false;
        this.name.Font = font;
        this.name.Location = new System.Drawing.Point(minPading, minPading);
        this.name.Name = "name";
        this.name.TabIndex = 0;
        this.name.Text = "label1";

        this.hits.AutoSize = true;
        this.hits.BackColor = System.Drawing.Color.Transparent;
        this.hits.Enabled = false;
        this.hits.Font = font;
        this.hits.ForeColor = System.Drawing.Color.Silver;
        this.hits.Location = new System.Drawing.Point(minPading, 14);//12
        this.hits.Name = "hits";
        this.hits.TabIndex = 1;
        this.hits.Text = "000/000";

        this.healthBar.Enabled = false;
        this.healthBar.Hits = 0;
        this.healthBar.Location = new System.Drawing.Point(48, this.hits.Location.Y + 2);//38
        this.healthBar.MaxHits = 0;
        this.healthBar.Name = "healthBar";
        this.healthBar.Size = new System.Drawing.Size(104, 10);//8
        this.healthBar.TabIndex = 2;
        this.healthBar.Text = "healthBar1";
        this.healthBar.Unknown = false;

        this.runLabel.AutoSize = true;
        this.runLabel.BackColor = System.Drawing.Color.Transparent;
        this.runLabel.Enabled = false;
        this.runLabel.Font = font;
        this.runLabel.Name = "runLabel";
        //this.runLabel.Size = new System.Drawing.Size(20, 8);
        this.runLabel.TabIndex = 0;
        this.runLabel.Text = "-";
        this.runLabel.Location = new Point(minPading, 28);

        int lastY = 0;

        int chbxBoxSize = 11;

        lastY = this.healthBar.Location.Y + this.healthBar.Height;//30 - (chbxBoxSize + 2);

        this.btnA.Name = "btnA";
        this.btnA.Location = new System.Drawing.Point(48, lastY + 3);
        this.btnA.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.btnA.TabIndex = 1;
        this.btnA.BackColor = Color.LawnGreen;
        this.btnA.FlatStyle = FlatStyle.Flat;
        this.btnA.FlatAppearance.BorderSize = 0;
        this.btnA.ForeColor = System.Drawing.Color.Black;
        this.btnA.TabStop = false;
        this.btnA.Padding = new Padding(0);

        Control prevCont = this.btnA;

        this.btnB.Name = "btnB";
        this.btnB.Location = new System.Drawing.Point(prevCont.Location.X + chbxBoxSize + 3, lastY + 3);
        this.btnB.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.btnB.TabIndex = 1;
        this.btnB.BackColor = Color.Firebrick;
        this.btnB.FlatStyle = FlatStyle.Flat;
        this.btnB.FlatAppearance.BorderSize = 0;
        this.btnB.ForeColor = System.Drawing.Color.Black;
        this.btnB.TabStop = false;
        this.btnB.Padding = new Padding(0);
        prevCont = this.btnB;

        this.btnX.Name = "btnX";
        this.btnX.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.btnX.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.btnX.TabIndex = 1;
        this.btnX.BackColor = Color.Blue;
        this.btnX.FlatStyle = FlatStyle.Flat;
        this.btnX.FlatAppearance.BorderSize = 0;
        this.btnX.ForeColor = System.Drawing.Color.Black;
        this.btnX.TabStop = false;
        this.btnX.Padding = new Padding(0);
        prevCont = this.btnX;

        this.btnY.Name = "btnY";
        this.btnY.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.btnY.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.btnY.TabIndex = 1;
        this.btnY.BackColor = Color.Orange;
        this.btnY.FlatStyle = FlatStyle.Flat;
        this.btnY.FlatAppearance.BorderSize = 0;
        this.btnY.ForeColor = System.Drawing.Color.Black;
        this.btnY.TabStop = false;
        this.btnY.Padding = new Padding(0);
        this.btnY.Margin = new Padding(0);
        prevCont = this.btnY;

        this.chbxHeal.Name = "heal";
        this.chbxHeal.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.chbxHeal.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.chbxHeal.TabIndex = 1;
        this.chbxHeal.BackColor = Color.Transparent;
        prevCont = this.chbxHeal;

        this.chbxAlie.Name = "alie";
        this.chbxAlie.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.chbxAlie.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.chbxAlie.TabIndex = 1;
        this.chbxAlie.BackColor = Color.Transparent;
        prevCont = this.chbxAlie;

        this.chbxKeep.Name = "keep";
        this.chbxKeep.Location = new System.Drawing.Point(prevCont.Location.X + prevCont.Size.Width + 3, lastY + 3);
        this.chbxKeep.Size = new System.Drawing.Size(chbxBoxSize, chbxBoxSize);
        this.chbxKeep.TabIndex = 1;
        this.chbxKeep.BackColor = Color.Transparent;


        this.Controls.Add(this.hits);
        this.Controls.Add(this.name);
        this.Controls.Add(this.healthBar);

        this.Controls.Add(this.btnA);
        this.Controls.Add(this.btnB);
        this.Controls.Add(this.btnX);
        this.Controls.Add(this.btnY);

        this.Controls.Add(this.chbxHeal);
        this.Controls.Add(this.chbxAlie);
        this.Controls.Add(this.chbxKeep);

        this.Controls.Add(this.runLabel);

        this.ClientSize = new System.Drawing.Size(158, 44);
      }

      this.DoubleBuffered = true;
      this.Name = "StatusForm";
      this.ResumeLayout(false);
      this.PerformLayout();

      this.OriginalWidth = this.Width;
      this.OriginalHeight = this.Height;

    }

    #endregion
  }


  public class StatusFormActionEventArgs : EventArgs
  {
    public UOCharacter mobile;
  }
}
