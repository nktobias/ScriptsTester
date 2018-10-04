using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Phoenix;
using Phoenix.WorldData;
using System.ComponentModel;
using System.Threading;
using CalExtension.UOExtensions;
using CalExtension.Skills;
using Phoenix.Runtime;

namespace CalExtension.UI.Status
{
  public class UtilityForm : InGameWindow
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:SuppliesForm"/> class.
    /// </summary>
    public UtilityForm()
    {
      MouseEnter += StatusForm_MouseEnter;
      MouseLeave += StatusForm_MouseLeave;
      World.Player.Backpack.Changed += Backpack_Changed;
      this.InitializeComponent();

      if (t == null)
      {
        t = new Thread(new ThreadStart(Main));
        t.Start();
      }

      if (tWatch == null)
      {
        tWatch = new Thread(new ThreadStart(MainWatch));
        tWatch.Start();

      }
    }

    //---------------------------------------------------------------------------------------------
    Thread tWatch;
    Thread t;
    private void Main()
    {
      while (true)
      {
        Thread.Sleep(500);
        this.RefreshMixState();
      }
    }

    private void MainWatch()
    {
      while (true)
      {
        Thread.Sleep(1000);
        this.RefresWsWatch();
      }
    }

    //---------------------------------------------------------------------------------------------

    bool ctrlHold = false;
    protected void RefreshMixState()
    {
      if (InvokeRequired)
      {
        BeginInvoke(new ThreadStart(RefreshMixState));
        return;
      }

      bool currCtrlHold = (ModifierKeys & Keys.Control) == Keys.Control;
      if (currCtrlHold != ctrlHold)
      {
        ctrlHold = currCtrlHold;

        if (ctrlHold)
        {
          this.btnMixurePotion.BackColor = Color.Firebrick;
          this.btnMixurePotion.Text = "MIX";
          //FireBrick 
        }
        else
        {
          this.btnMixurePotion.BackColor = Color.DodgerBlue;
          this.btnMixurePotion.Text = "DRINK";
        }
        this.btnMixurePotion.Invalidate();
      }
    }


    //---------------------------------------------------------------------------------------------

    private Potion potion = Potion.Cure;
    public Potion Potion
    {
      get { return this.potion; }
      set
      {
        this.potion = value;
        Config.Profile.UserSettings.SetAttribute(this.potion.Name, "Value", "UtilityForm_Potion");
      }
    }

    //---------------------------------------------------------------------------------------------

    private PotionQuality potionQuality = PotionQuality.Lesser;
    public PotionQuality PotionQuality
    {
      get { return this.potionQuality; }
      set
      {
        this.potionQuality = value;
        Config.Profile.UserSettings.SetAttribute((int)this.potionQuality, "Value", "UtilityForm_PotionQuality");
      }
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
    }

    //---------------------------------------------------------------------------------------------

    protected Color? DefaultColor = Color.Black;
    public bool MouseHovering = false;

    void StatusForm_MouseEnter(object sender, EventArgs e)
    {
      MouseHovering = true;
      BackColor = Color.FromArgb(64, 64, 64);
      //Color.GreenYellow;

      this.Invalidate();
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);


      int defaultX = 0;
      int defaultY = 0;


      int x = Config.Profile.UserSettings.GetAttribute(defaultX, "Value", "UtilityForm_LocationX");
      int y = Config.Profile.UserSettings.GetAttribute(defaultY, "Value", "UtilityForm_LocationY");

      this.BackColor = DefaultColor.Value;
      this.Location = new Point(x, y);

      //this.chbxAuto.Checked = this.auto;
      int index = 0;
      for (int i = 0; i < PotionCollection.Potions.Count; i++)
      {
        if (PotionCollection.Potions[i].Name == this.potion.Name)
        {
          index = i;
          break;
        }
      }

      this.cbxPotions.SelectedIndex = index; 
      this.cbxPotionQualities.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(PotionQuality)), this.potionQuality);
      this.RefresWsWatch();

      this.Invalidate();
      initRun = false;
    }

    //---------------------------------------------------------------------------------------------

    void StatusForm_MouseLeave(object sender, EventArgs e)
    {
      if (this.DefaultColor.HasValue)
      {
        BackColor = this.DefaultColor.Value;
        this.Invalidate();
      }
      MouseHovering = false;

      Config.Profile.UserSettings.SetAttribute(this.Location.X, "Value", "UtilityForm_LocationX");
      Config.Profile.UserSettings.SetAttribute(this.Location.Y, "Value", "UtilityForm_LocationY");
    }

    //---------------------------------------------------------------------------------------------

    protected void RefresCounters()
    {
      int newlcPotionCount = World.Player.Backpack.AllItems.Count(Potion.Cure.DefaultGraphic, Potion.Cure.Qualities[PotionQuality.Lesser].Color);
      int newgcPotionCount = World.Player.Backpack.AllItems.Count(Potion.Cure.DefaultGraphic, Potion.Cure.Qualities[PotionQuality.Greater].Color);
      int newgsPotionCount = World.Player.Backpack.AllItems.Count(Potion.Strength.DefaultGraphic, Potion.Strength.Qualities[PotionQuality.Greater].Color);
      int newtrPotionCount = World.Player.Backpack.AllItems.Count(Potion.Refresh.DefaultGraphic, Potion.Refresh.Qualities[PotionQuality.Total].Color);
      int newghPotionCount = World.Player.Backpack.AllItems.Count(Potion.Heal.DefaultGraphic, Potion.Heal.Qualities[PotionQuality.Greater].Color);
      int newmrPotionCount = World.Player.Backpack.AllItems.Count(Potion.ManaRefresh.DefaultGraphic, Potion.ManaRefresh.Qualities[PotionQuality.None].Color);
      int newtmrPotionCount = World.Player.Backpack.AllItems.Count(Potion.TotalManaRefresh.DefaultGraphic, Potion.TotalManaRefresh.Qualities[PotionQuality.None].Color);
      int newinvPotionCount = World.Player.Backpack.AllItems.Count(Potion.Invisibility.DefaultGraphic, Potion.Invisibility.Qualities[PotionQuality.None].Color);

      bool needRefresh =
        lcPotionCount != newlcPotionCount ||
        gcPotionCount != newgcPotionCount ||
        gsPotionCount != newgsPotionCount ||
        trPotionCount != newtrPotionCount ||
        ghPotionCount != newghPotionCount ||
        mrPotionCount != newmrPotionCount ||
        tmrPotionCount != newtmrPotionCount ||
        invPotionCount != newinvPotionCount;


      if (needRefresh)
      {
        if (lcPotionCount != newlcPotionCount)
        {
          this.btnMixureLC.Text = "LC (" + newlcPotionCount + ")";
          this.btnMixureLC.Invalidate();
        }

        if (gcPotionCount != newgcPotionCount)
        {
          this.btnMixureGC.Text = "GC (" + newgcPotionCount + ")";
          this.btnMixureGC.Invalidate();
        }

        if (gsPotionCount != newgsPotionCount)
        {
          this.btnMixureGS.Text = "GS (" + newgsPotionCount + ")";
          this.btnMixureGS.Invalidate();
        }

        if (trPotionCount != newtrPotionCount)
        {
          this.btnMixureTR.Text = "TR (" + newtrPotionCount + ")";
          this.btnMixureTR.Invalidate();
        }

        if (ghPotionCount != newghPotionCount)
        {
          this.btnMixureGH.Text = "GH (" + newghPotionCount + ")";
          this.btnMixureGH.Invalidate();
        }

        if (mrPotionCount != newmrPotionCount)
        {
          this.btnMixureMR.Text = "MR (" + newmrPotionCount + ")";
          this.btnMixureMR.Invalidate();
        }

        if (tmrPotionCount != newtmrPotionCount)
        {
          this.btnMixureTMR.Text = "TMR (" + newtmrPotionCount + ")";
          this.btnMixureTMR.Invalidate();
        }


        if (invPotionCount != newinvPotionCount)
        {
          this.btnMixureINV.Text = "INV (" + newinvPotionCount + ")";
          this.btnMixureINV.Invalidate();
        }

        this.Invalidate();
      }

      lcPotionCount = newlcPotionCount;
      gcPotionCount = newgcPotionCount;
      gsPotionCount = newgsPotionCount;
      trPotionCount = newtrPotionCount;
      ghPotionCount = newghPotionCount;
      mrPotionCount = newmrPotionCount;
      tmrPotionCount = newtmrPotionCount;
      invPotionCount = newinvPotionCount;
    }

    //---------------------------------------------------------------------------------------------

    protected void RefresWsWatch()
    {
      if (InvokeRequired)
      {
        this.BeginInvoke(new ThreadStart(RefresWsWatch));
        return;
      }
      DateTime? lastWsTime = Game.CurrentGame.LastWorldSaveTime;

      if (lastWsTime.HasValue)
      {
        DateTime now = DateTime.Now;
        WebWorldSaveInfo wsInfo = WebWorldSaveTime.GetInfo(lastWsTime.Value, 0);

        this.wsInfo.Text = String.Format("WS >> {0} ({1:HH:mm:ss})", wsInfo.NextTimeStr, wsInfo.NextTime);
        this.wsInfo.Invalidate();

        //double nextWsMinutes = Math.Abs(wsInfo.NextTimeSpan.TotalMinutes);
      }
    }

    //---------------------------------------------------------------------------------------------
    private void Backpack_Changed(object sender, ObjectChangedEventArgs e)
    {
      this.RefresCounters();
      //if (e.Type == ObjectChangeType)
      //{

      //}
    }

    //---------------------------------------------------------------------------------------------

    protected override void OnClosing(CancelEventArgs e)
    {
      World.Player.Backpack.Changed -= Backpack_Changed;

      if (this.t != null)
      {
        this.t.Abort();
        this.t = null;
      }

      if (this.tWatch != null)
      {
        this.tWatch.Abort();
        this.tWatch = null;
      }

      base.OnClosing(e);
    }

    //---------------------------------------------------------------------------------------------

    protected override void Dispose(bool disposing)
    {
      World.Player.Backpack.Changed -= Backpack_Changed;

      if (this.t != null)
      {
        this.t.Abort();
        this.t = null;
      }

      if (this.tWatch != null)
      {
        this.tWatch.Abort();
        this.tWatch = null;
      }

      base.Dispose(disposing);
    }

    //---------------------------------------------------------------------------------------------

    #region WinForms

    Label name;
    ComboBox cbxPotions; 
    ComboBox cbxPotionQualities;
    Button btnMixurePotion;


    Button btnMixureLC;
    Button btnMixureGC;
    Button btnMixureGS;
    Button btnMixureTR;
    Button btnMixureGH;
    Button btnMixureMR;
    Button btnMixureTMR;
    Button btnMixureINV;

    Button btnStatRepair;
    Button btnSortBackpack;
    Button btnNbRuna;
    Button btnNbCech;
    Button btnPrintItems;

    Button btnSkillTrackAnimal;
    Button btnSkillTrackPk;
    Button btnSkillTrackDetect;
    Button btnSkillTrackForensic;
    Button btnSkillTrackItemId;
    Button btnInfo;

    Button btnL500;
    Button btnL2500;
    Button btnTAll;

    Button btnLatency;
    Button btnHide;
    Button btnResync;



    Label wsInfo;

    private int lcPotionCount = 0;
    private int gcPotionCount = 0;
    private int gsPotionCount = 0;
    private int trPotionCount = 0;
    private int ghPotionCount = 0;
    private int mrPotionCount = 0;
    private int tmrPotionCount = 0;
    private int invPotionCount = 0;


    //---------------------------------------------------------------------------------------------
    private bool initRun = true;
    private void InitializeComponent()
    {
      initRun = true;
      this.SuspendLayout();

      int alchemyValue = SkillsHelper.GetSkillValue("Alchemy").Value;

      Font font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      Font fontSmall = new System.Drawing.Font("Arial", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

      int defaultPadding = 2;
      int maxX = 0;
      int maxY = 0;
      int currentLine = 0 + defaultPadding;
      int currentPosition = 0 + defaultPadding;

      int buttonWidth = 48;
      int buttonHeight = 20;
      int buttonMiddleWidth = 64;

      this.name = new Label();
      this.name.AutoSize = true;
      this.name.BackColor = System.Drawing.Color.Transparent;
      this.name.Enabled = false;
      this.name.Font = font;
      this.name.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.name.Name = "name";
      this.name.TabIndex = 0;
      this.name.Text = "Utility Form";
      this.Controls.Add(this.name);

      currentLine = 24;

      this.cbxPotions = new ComboBox();
      this.cbxPotions.DisplayMember = "Name";
      this.cbxPotions.DataSource = PotionCollection.Potions;
      this.cbxPotions.Name = "cbxPotions";
      this.cbxPotions.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.cbxPotions.Size = new System.Drawing.Size(100, 10);
      this.cbxPotions.Enabled = true;
      this.Controls.Add(this.cbxPotions);

      Control prevCont = this.cbxPotions;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.cbxPotionQualities = new ComboBox();
      this.cbxPotionQualities.DataSource = Enum.GetValues(typeof(PotionQuality));//Enum.GetNames(typeof(StatusFormWrapperSortType));
      this.cbxPotionQualities.Name = "cbxPotionQualities";
      this.cbxPotionQualities.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.cbxPotionQualities.Size = new System.Drawing.Size(50, 10);
      this.cbxPotionQualities.Enabled = true;
      this.Controls.Add(this.cbxPotionQualities);

      prevCont = this.cbxPotionQualities;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixurePotion = new Button();
      this.btnMixurePotion.Name = "btnMixurePotion";
      this.btnMixurePotion.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixurePotion.Enabled = true;
      this.btnMixurePotion.Text = "DRINK";
      this.btnMixurePotion.Font = fontSmall;
      this.btnMixurePotion.Size = new Size(45, buttonHeight);
      // this.btnMixurePotion.AutoSize = true;
      this.btnMixurePotion.BackColor = Color.DodgerBlue;
      this.btnMixurePotion.ForeColor = Color.White;
      this.btnMixurePotion.Padding = new Padding(0);
      this.btnMixurePotion.TabStop = false;
      this.btnMixurePotion.FlatStyle = FlatStyle.Flat;
      this.btnMixurePotion.FlatAppearance.BorderSize = 0;

      this.btnMixurePotion.Click += BtnMixurePotion_Click;
      this.Controls.Add(this.btnMixurePotion);

      prevCont = this.btnMixurePotion;

      #region QuickAlchemyButtons



      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;
      currentPosition = defaultPadding;

      this.btnMixureLC = new Button();
      this.btnMixureLC.Name = "btnMixureLC";
      this.btnMixureLC.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureLC.Enabled = true;
      this.btnMixureLC.Text = "LC (" + lcPotionCount + ")";
      this.btnMixureLC.Font = fontSmall;
      this.btnMixureLC.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureLC.BackColor = Color.Orange;
      this.btnMixureLC.ForeColor = Color.White;
      this.btnMixureLC.Padding = new Padding(0);
      this.btnMixureLC.TabStop = false;
      this.btnMixureLC.FlatStyle = FlatStyle.Flat;
      this.btnMixureLC.FlatAppearance.BorderSize = 0;

      this.btnMixureLC.MouseClick += BtnMixureLC_Click;//.Click += BtnMixureLC_Click;
      this.Controls.Add(this.btnMixureLC);

      prevCont = this.btnMixureLC;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureGC = new Button();
      this.btnMixureGC.Name = "btnMixureGC";
      this.btnMixureGC.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureGC.Enabled = true;
      this.btnMixureGC.Text = "GC (" + gcPotionCount + ")";
      this.btnMixureGC.Font = fontSmall;
      this.btnMixureGC.Size = new Size(buttonWidth, buttonHeight);

      //this.btnMixureLC.AutoSize = true;
      this.btnMixureGC.BackColor = Color.OrangeRed;
      this.btnMixureGC.ForeColor = Color.White;
      this.btnMixureGC.Padding = new Padding(0);
      this.btnMixureGC.TabStop = false;
      this.btnMixureGC.FlatStyle = FlatStyle.Flat;
      this.btnMixureGC.FlatAppearance.BorderSize = 0;


      this.btnMixureGC.MouseClick += BtnMixureGC_Click;
      this.Controls.Add(this.btnMixureGC);

      prevCont = this.btnMixureGC;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureGS = new Button();
      this.btnMixureGS.Name = "btnMixureGS";
      this.btnMixureGS.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureGS.Enabled = true;
      this.btnMixureGS.Text = "GS (" + gsPotionCount + ")";
      this.btnMixureGS.Font = fontSmall;
      this.btnMixureGS.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureGS.BackColor = Color.GhostWhite;
      this.btnMixureGS.ForeColor = Color.Black;
      this.btnMixureGS.Padding = new Padding(0);
      this.btnMixureGS.TabStop = false;
      this.btnMixureGS.FlatStyle = FlatStyle.Flat;
      this.btnMixureGS.FlatAppearance.BorderSize = 0;

      this.btnMixureGS.MouseClick += BtnMixureGS_Click;
      this.Controls.Add(this.btnMixureGS);

      prevCont = this.btnMixureGS;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureTR = new Button();
      this.btnMixureTR.Name = "btnMixureTR";
      this.btnMixureTR.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureTR.Enabled = true;
      this.btnMixureTR.Text = "TR (" + trPotionCount + ")";
      this.btnMixureTR.Font = fontSmall;
      this.btnMixureTR.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureTR.BackColor = Color.Crimson;
      this.btnMixureTR.ForeColor = Color.White;
      this.btnMixureTR.Padding = new Padding(0);
      this.btnMixureTR.TabStop = false;
      this.btnMixureTR.FlatStyle = FlatStyle.Flat;
      this.btnMixureTR.FlatAppearance.BorderSize = 0;

      this.btnMixureTR.MouseClick += BtnMixureTR_Click;
      this.Controls.Add(this.btnMixureTR);

      prevCont = this.btnMixureTR;
      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnMixureGH = new Button();
      this.btnMixureGH.Name = "btnMixureGH";
      this.btnMixureGH.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureGH.Enabled = true;
      this.btnMixureGH.Text = "GH (" + ghPotionCount + ")";
      this.btnMixureGH.Font = fontSmall;
      this.btnMixureGH.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureGH.BackColor = Color.Gold;
      this.btnMixureGH.ForeColor = Color.Black;
      this.btnMixureGH.Padding = new Padding(0);
      this.btnMixureGH.TabStop = false;
      this.btnMixureGH.FlatStyle = FlatStyle.Flat;
      this.btnMixureGH.FlatAppearance.BorderSize = 0;

      this.btnMixureGH.MouseClick += BtnMixureGH_Click;
      this.Controls.Add(this.btnMixureGH);

      prevCont = this.btnMixureGH;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureMR = new Button();
      this.btnMixureMR.Name = "btnMixureMR";
      this.btnMixureMR.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureMR.Enabled = true;
      this.btnMixureMR.Text = "MR (" + mrPotionCount + ")";
      this.btnMixureMR.Font = fontSmall;
      this.btnMixureMR.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureMR.BackColor = Color.CornflowerBlue;
      this.btnMixureMR.ForeColor = Color.White;
      this.btnMixureMR.Padding = new Padding(0);
      this.btnMixureMR.TabStop = false;
      this.btnMixureMR.FlatStyle = FlatStyle.Flat;
      this.btnMixureMR.FlatAppearance.BorderSize = 0;

      this.btnMixureMR.MouseClick += BtnMixureMR_Click;
      this.Controls.Add(this.btnMixureMR);

      prevCont = this.btnMixureMR;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnMixureTMR = new Button();
      this.btnMixureTMR.Name = "btnMixureTMR";
      this.btnMixureTMR.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureTMR.Enabled = true;
      this.btnMixureTMR.Text = "TMR (" + tmrPotionCount + ")";
      this.btnMixureTMR.Font = fontSmall;
      this.btnMixureTMR.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureTMR.BackColor = Color.Blue;
      this.btnMixureTMR.ForeColor = Color.White;
      this.btnMixureTMR.Padding = new Padding(0);
      this.btnMixureTMR.TabStop = false;
      this.btnMixureTMR.FlatStyle = FlatStyle.Flat;
      this.btnMixureTMR.FlatAppearance.BorderSize = 0;

      this.btnMixureTMR.MouseClick += BtnMixureTMR_Click;
      this.Controls.Add(this.btnMixureTMR);

      prevCont = this.btnMixureTMR;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;


      this.btnMixureINV = new Button();
      this.btnMixureINV.Name = "btnMixureINV";
      this.btnMixureINV.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnMixureINV.Enabled = true;
      this.btnMixureINV.Text = "INV (" + invPotionCount + ")";
      this.btnMixureINV.Font = fontSmall;
      this.btnMixureINV.Size = new Size(buttonWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnMixureINV.BackColor = Color.AliceBlue;
      this.btnMixureINV.ForeColor = Color.Black;
      this.btnMixureINV.Padding = new Padding(0);
      this.btnMixureINV.TabStop = false;
      this.btnMixureINV.FlatStyle = FlatStyle.Flat;
      this.btnMixureINV.FlatAppearance.BorderSize = 0;

      this.btnMixureINV.MouseClick += BtnMixureINV_Click;
      this.Controls.Add(this.btnMixureINV);

      prevCont = this.btnMixureINV;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      #endregion

      #region UtilButtons

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnStatRepair = new Button();
      this.btnStatRepair.Name = "btnStatRepair";
      this.btnStatRepair.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnStatRepair.Enabled = true;
      this.btnStatRepair.Text = "Staty";
      this.btnStatRepair.Font = fontSmall;
      this.btnStatRepair.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnStatRepair.BackColor = Color.LightSlateGray;
      this.btnStatRepair.ForeColor = Color.Black;
      this.btnStatRepair.Padding = new Padding(0);
      this.btnStatRepair.TabStop = false;
      this.btnStatRepair.FlatStyle = FlatStyle.Flat;
      this.btnStatRepair.FlatAppearance.BorderSize = 0;

      this.btnStatRepair.MouseClick += BtnStatRepair_Click;
      this.Controls.Add(this.btnStatRepair);

      prevCont = this.btnStatRepair;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSortBackpack = new Button();
      this.btnSortBackpack.Name = "btnSortBackpack";
      this.btnSortBackpack.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSortBackpack.Enabled = true;
      this.btnSortBackpack.Text = "Sortbackpack";
      this.btnSortBackpack.Font = fontSmall;
      this.btnSortBackpack.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSortBackpack.BackColor = Color.LightSlateGray;
      this.btnSortBackpack.ForeColor = Color.Black;
      this.btnSortBackpack.Padding = new Padding(0);
      this.btnSortBackpack.TabStop = false;
      this.btnSortBackpack.FlatStyle = FlatStyle.Flat;
      this.btnSortBackpack.FlatAppearance.BorderSize = 0;

      this.btnSortBackpack.MouseClick += BtnSortBackpack_MouseClick; ;
      this.Controls.Add(this.btnSortBackpack);

      prevCont = this.btnSortBackpack;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnNbRuna = new Button();
      this.btnNbRuna.Name = "btnNbRuna";
      this.btnNbRuna.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNbRuna.Enabled = true;
      this.btnNbRuna.Text = "NB runa";
      this.btnNbRuna.Font = fontSmall;
      this.btnNbRuna.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNbRuna.BackColor = Color.LightSlateGray;
      this.btnNbRuna.ForeColor = Color.Black;
      this.btnNbRuna.Padding = new Padding(0);
      this.btnNbRuna.TabStop = false;
      this.btnNbRuna.FlatStyle = FlatStyle.Flat;
      this.btnNbRuna.FlatAppearance.BorderSize = 0;

      this.btnNbRuna.MouseClick += BtnNbRuna_MouseClick; ; ;
      this.Controls.Add(this.btnNbRuna);

      prevCont = this.btnNbRuna;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnNbCech = new Button();
      this.btnNbCech.Name = "btnNbCech";
      this.btnNbCech.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnNbCech.Enabled = true;
      this.btnNbCech.Text = "Cech/Jlm";
      this.btnNbCech.Font = fontSmall;
      this.btnNbCech.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnNbCech.BackColor = Color.LightSlateGray;
      this.btnNbCech.ForeColor = Color.Black;
      this.btnNbCech.Padding = new Padding(0);
      this.btnNbCech.TabStop = false;
      this.btnNbCech.FlatStyle = FlatStyle.Flat;
      this.btnNbCech.FlatAppearance.BorderSize = 0;

      this.btnNbCech.MouseClick += BtnNbCech_MouseClick;
      this.Controls.Add(this.btnNbCech);

      prevCont = this.btnNbCech;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnPrintItems = new Button();
      this.btnPrintItems.Name = "btnPrintItems";
      this.btnPrintItems.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnPrintItems.Enabled = true;
      this.btnPrintItems.Text = "Print All";
      this.btnPrintItems.Font = fontSmall;
      this.btnPrintItems.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnPrintItems.BackColor = Color.LightSlateGray;
      this.btnPrintItems.ForeColor = Color.Black;
      this.btnPrintItems.Padding = new Padding(0);
      this.btnPrintItems.TabStop = false;
      this.btnPrintItems.FlatStyle = FlatStyle.Flat;
      this.btnPrintItems.FlatAppearance.BorderSize = 0;

      this.btnPrintItems.MouseClick += BtnPrintItems_MouseClick; ;
      this.Controls.Add(this.btnPrintItems);

      prevCont = this.btnPrintItems;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnSkillTrackAnimal = new Button();
      this.btnSkillTrackAnimal.Name = "btnSkillTrackAnimal";
      this.btnSkillTrackAnimal.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackAnimal.Enabled = true;
      this.btnSkillTrackAnimal.Text = "Track Anml";
      this.btnSkillTrackAnimal.Font = fontSmall;
      this.btnSkillTrackAnimal.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackAnimal.BackColor = Color.LightSlateGray;
      this.btnSkillTrackAnimal.ForeColor = Color.Black;
      this.btnSkillTrackAnimal.Padding = new Padding(0);
      this.btnSkillTrackAnimal.TabStop = false;
      this.btnSkillTrackAnimal.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackAnimal.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackAnimal.MouseClick += BtnSkillTrackAnimal_MouseClick; ;
      this.Controls.Add(this.btnSkillTrackAnimal);

      prevCont = this.btnSkillTrackAnimal;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSkillTrackPk = new Button();
      this.btnSkillTrackPk.Name = "btnSkillTrackPk";
      this.btnSkillTrackPk.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackPk.Enabled = true;
      this.btnSkillTrackPk.Text = "Track PK";
      this.btnSkillTrackPk.Font = fontSmall;
      this.btnSkillTrackPk.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackPk.BackColor = Color.LightSlateGray;
      this.btnSkillTrackPk.ForeColor = Color.Black;
      this.btnSkillTrackPk.Padding = new Padding(0);
      this.btnSkillTrackPk.TabStop = false;
      this.btnSkillTrackPk.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackPk.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackPk.MouseClick += BtnSkillTrackPk_MouseClick;
      this.Controls.Add(this.btnSkillTrackPk);

      prevCont = this.btnSkillTrackPk;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSkillTrackDetect = new Button();
      this.btnSkillTrackDetect.Name = "btnSkillTrackDetect";
      this.btnSkillTrackDetect.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackDetect.Enabled = true;
      this.btnSkillTrackDetect.Text = "Detect";
      this.btnSkillTrackDetect.Font = fontSmall;
      this.btnSkillTrackDetect.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackDetect.BackColor = Color.LightSlateGray;
      this.btnSkillTrackDetect.ForeColor = Color.Black;
      this.btnSkillTrackDetect.Padding = new Padding(0);
      this.btnSkillTrackDetect.TabStop = false;
      this.btnSkillTrackDetect.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackDetect.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackDetect.MouseClick += BtnSkillTrackDetect_MouseClick; ;
      this.Controls.Add(this.btnSkillTrackDetect);

      prevCont = this.btnSkillTrackDetect;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnSkillTrackForensic = new Button();
      this.btnSkillTrackForensic.Name = "btnSkillTrackForensic";
      this.btnSkillTrackForensic.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackForensic.Enabled = true;
      this.btnSkillTrackForensic.Text = "Forens.";
      this.btnSkillTrackForensic.Font = fontSmall;
      this.btnSkillTrackForensic.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackForensic.BackColor = Color.LightSlateGray;
      this.btnSkillTrackForensic.ForeColor = Color.Black;
      this.btnSkillTrackForensic.Padding = new Padding(0);
      this.btnSkillTrackForensic.TabStop = false;
      this.btnSkillTrackForensic.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackForensic.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackForensic.MouseClick += BtnSkillTrackForensic_MouseClick;
      this.Controls.Add(this.btnSkillTrackForensic);

      prevCont = this.btnSkillTrackForensic;
      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnSkillTrackItemId = new Button();
      this.btnSkillTrackItemId.Name = "btnSkillTrackItemId";
      this.btnSkillTrackItemId.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnSkillTrackItemId.Enabled = true;
      this.btnSkillTrackItemId.Text = "Item Id.";
      this.btnSkillTrackItemId.Font = fontSmall;
      this.btnSkillTrackItemId.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnSkillTrackItemId.BackColor = Color.LightSlateGray;
      this.btnSkillTrackItemId.ForeColor = Color.Black;
      this.btnSkillTrackItemId.Padding = new Padding(0);
      this.btnSkillTrackItemId.TabStop = false;
      this.btnSkillTrackItemId.FlatStyle = FlatStyle.Flat;
      this.btnSkillTrackItemId.FlatAppearance.BorderSize = 0;

      this.btnSkillTrackItemId.MouseClick += BtnSkillTrackItemId_MouseClick;
      this.Controls.Add(this.btnSkillTrackItemId);

      prevCont = this.btnSkillTrackItemId;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnInfo = new Button();
      this.btnInfo.Name = "btnInfo";
      this.btnInfo.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnInfo.Enabled = true;
      this.btnInfo.Text = "Info";
      this.btnInfo.Font = fontSmall;
      this.btnInfo.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnInfo.BackColor = Color.LightSlateGray;
      this.btnInfo.ForeColor = Color.Black;
      this.btnInfo.Padding = new Padding(0);
      this.btnInfo.TabStop = false;
      this.btnInfo.FlatStyle = FlatStyle.Flat;
      this.btnInfo.FlatAppearance.BorderSize = 0;

      this.btnInfo.MouseClick += BtnInfo_MouseClick;
      this.Controls.Add(this.btnInfo);

      prevCont = this.btnInfo;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnL500= new Button();
      this.btnL500.Name = "btnL500";
      this.btnL500.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnL500.Enabled = true;
      this.btnL500.Text = "Lux 500";
      this.btnL500.Font = fontSmall;
      this.btnL500.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnL500.BackColor = Color.LightSlateGray;
      this.btnL500.ForeColor = Color.Black;
      this.btnL500.Padding = new Padding(0);
      this.btnL500.TabStop = false;
      this.btnL500.FlatStyle = FlatStyle.Flat;
      this.btnL500.FlatAppearance.BorderSize = 0;

      this.btnL500.MouseClick += BtnL500_MouseClick; ;
      this.Controls.Add(this.btnL500);

      prevCont = this.btnL500;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnL2500 = new Button();
      this.btnL2500.Name = "btnL2500";
      this.btnL2500.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnL2500.Enabled = true;
      this.btnL2500.Text = "Lux 2500";
      this.btnL2500.Font = fontSmall;
      this.btnL2500.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnL2500.BackColor = Color.LightSlateGray;
      this.btnL2500.ForeColor = Color.Black;
      this.btnL2500.Padding = new Padding(0);
      this.btnL2500.TabStop = false;
      this.btnL2500.FlatStyle = FlatStyle.Flat;
      this.btnL2500.FlatAppearance.BorderSize = 0;

      this.btnL2500.MouseClick += BtnL2500_MouseClick;
      this.Controls.Add(this.btnL2500);

      prevCont = this.btnL2500;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnTAll= new Button();
      this.btnTAll.Name = "btnTAll";
      this.btnTAll.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnTAll.Enabled = true;
      this.btnTAll.Text = "Term. All";
      this.btnTAll.Font = fontSmall;
      this.btnTAll.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnTAll.BackColor = Color.LightSlateGray;
      this.btnTAll.ForeColor = Color.Black;
      this.btnTAll.Padding = new Padding(0);
      this.btnTAll.TabStop = false;
      this.btnTAll.FlatStyle = FlatStyle.Flat;
      this.btnTAll.FlatAppearance.BorderSize = 0;

      this.btnTAll.MouseClick += BtnTAll_MouseClick;
      this.Controls.Add(this.btnTAll);

      prevCont = this.btnTAll;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;

      this.btnLatency= new Button();
      this.btnLatency.Name = "btnLatency";
      this.btnLatency.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnLatency.Enabled = true;
      this.btnLatency.Text = "Latency";
      this.btnLatency.Font = fontSmall;
      this.btnLatency.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnLatency.BackColor = Color.LightSlateGray;
      this.btnLatency.ForeColor = Color.Black;
      this.btnLatency.Padding = new Padding(0);
      this.btnLatency.TabStop = false;
      this.btnLatency.FlatStyle = FlatStyle.Flat;
      this.btnLatency.FlatAppearance.BorderSize = 0;

      this.btnLatency.MouseClick += BtnLatency_MouseClick;
      this.Controls.Add(this.btnLatency);

      prevCont = this.btnLatency;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnHide = new Button();
      this.btnHide.Name = "btnHide";
      this.btnHide.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnHide.Enabled = true;
      this.btnHide.Text = "Hide";
      this.btnHide.Font = fontSmall;
      this.btnHide.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnHide.BackColor = Color.LightSlateGray;
      this.btnHide.ForeColor = Color.Black;
      this.btnHide.Padding = new Padding(0);
      this.btnHide.TabStop = false;
      this.btnHide.FlatStyle = FlatStyle.Flat;
      this.btnHide.FlatAppearance.BorderSize = 0;

      this.btnHide.MouseClick += BtnHide_MouseClick;
      this.Controls.Add(this.btnHide);

      prevCont = this.btnHide;

      currentPosition = prevCont.Location.X + prevCont.Size.Width + defaultPadding;

      this.btnResync = new Button();
      this.btnResync.Name = "btnHide";
      this.btnResync.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.btnResync.Enabled = true;
      this.btnResync.Text = "Resync";
      this.btnResync.Font = fontSmall;
      this.btnResync.Size = new Size(buttonMiddleWidth, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.btnResync.BackColor = Color.LightSlateGray;
      this.btnResync.ForeColor = Color.Black;
      this.btnResync.Padding = new Padding(0);
      this.btnResync.TabStop = false;
      this.btnResync.FlatStyle = FlatStyle.Flat;
      this.btnResync.FlatAppearance.BorderSize = 0;

      this.btnResync.MouseClick += BtnResync_MouseClick;
      this.Controls.Add(this.btnResync);

      prevCont = this.btnResync;


      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;


      this.wsInfo = new Label();
      this.wsInfo.Name = "wsInfo";
      this.wsInfo.Location = new System.Drawing.Point(currentPosition, currentLine);
      this.wsInfo.Enabled = true;
      this.wsInfo.Text = "WS za ";
      this.wsInfo.Font = font;
      this.wsInfo.Size = new Size(200, buttonHeight);
      //this.btnMixureLC.AutoSize = true;
      this.wsInfo.BackColor = Color.Transparent;
      this.wsInfo.ForeColor = Color.GhostWhite;
      this.wsInfo.Padding = new Padding(0);
      this.wsInfo.TabStop = false;
      this.wsInfo.FlatStyle = FlatStyle.Flat;

      this.Controls.Add(this.wsInfo);

      prevCont = this.wsInfo;

      currentPosition = defaultPadding;
      currentLine = prevCont.Location.Y + prevCont.Size.Height + defaultPadding;


      #endregion

      foreach (Control c in this.Controls)
      {
        if (maxX < c.Location.X + c.Size.Width)
          maxX = c.Location.X + c.Size.Width;

        if (maxY < c.Location.Y + c.Size.Height)
          maxY = c.Location.Y + c.Size.Height;
      }


      this.Size = new Size(maxX + defaultPadding, maxY + defaultPadding);

      this.selectedPotion = this.potion = PotionCollection.Potions.GetItemByName(Config.Profile.UserSettings.GetAttribute(Potion.Cure.name, "Value", "UtilityForm_Potion"));
      this.selectedQuality = this.potionQuality = (PotionQuality)(Config.Profile.UserSettings.GetAttribute((int)PotionQuality.Lesser, "Value", "UtilityForm_PotionQuality"));

      this.cbxPotions.SelectedValueChanged += CbxPotions_SelectedValueChanged;
      this.cbxPotionQualities.SelectedValueChanged += CbxPotionQualities_SelectedValueChanged;

      this.DoubleBuffered = true;
      this.Name = "UtilityForm";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnInfo_MouseClick(object sender, MouseEventArgs e)
    {
      World.Player.PrintMessage("[Vyber Info..]");
      new Thread(new ThreadStart(UO.Info)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnPrintItems_MouseClick(object sender, MouseEventArgs e)
    {
      Game.PrintKownObjects(true);

    }

    //---------------------------------------------------------------------------------------------

    private void BtnResync_MouseClick(object sender, MouseEventArgs e)
    {
      UO.Resync();

    }

    //---------------------------------------------------------------------------------------------

    private void BtnHide_MouseClick(object sender, MouseEventArgs e)
    {
      World.Player.PrintMessage("[Vyber HIDE..]");
      new Thread(new ThreadStart(UO.Hide)).Start();

    }

    //---------------------------------------------------------------------------------------------

    private void BtnLatency_MouseClick(object sender, MouseEventArgs e)
    {
      new Thread(new ThreadStart(UO.Latency)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnTAll_MouseClick(object sender, MouseEventArgs e)
    {
      RuntimeCore.Executions.TerminateAll();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnL2500_MouseClick(object sender, MouseEventArgs e)
    {
      World.Player.PrintMessage("[Lux 500..]");
      ItemHelper.luxing(2500);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnL500_MouseClick(object sender, MouseEventArgs e)
    {
      World.Player.PrintMessage("[Lux 2500..]");
      ItemHelper.luxing(500);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackItemId_MouseClick(object sender, MouseEventArgs e)
    {
      UO.UseSkill(StandardSkill.ItemIdentification);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackForensic_MouseClick(object sender, MouseEventArgs e)
    {
      UO.UseSkill(StandardSkill.ForensicEvaluation);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackDetect_MouseClick(object sender, MouseEventArgs e)
    {
      UO.UseSkill(StandardSkill.DetectingHidden);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackPk_MouseClick(object sender, MouseEventArgs e)
    {
      UO.WaitMenu("Tracking", "Players");
      UO.UseSkill(StandardSkill.Tracking);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSkillTrackAnimal_MouseClick(object sender, MouseEventArgs e)
    {
      UO.WaitMenu("Tracking", "Animals");
      UO.UseSkill(StandardSkill.Tracking);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNbCech_MouseClick(object sender, MouseEventArgs e)
    {
      //Todo
      if (KlicekRakev.Klicek.Exist)
        KlicekRakev.Current.KlicekRakevUse("Chci domu");
      else if (Kniha.CestovniKniha.Exist)
      {
        Kniha.Current.CestovniKnihaUse(1);
        Game.Wait(500);
        Kniha.Current.CestovniKnihaUse(4);
      }
      else if (Kniha.TravelBook.Exist)
        Kniha.Current.TravelBookUse(3);
      else if (Kniha.RuneBook.Exist)
        Kniha.Current.RuneBookUse(1);
      else
        World.Player.PrintMessage("Neni cim!", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnNbRuna_MouseClick(object sender, MouseEventArgs e)
    {
      Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.CommandList["nbruna"]);
    }

    //---------------------------------------------------------------------------------------------

    private void BtnSortBackpack_MouseClick(object sender, MouseEventArgs e)
    {
      new Thread(new ThreadStart(ItemHelper.SortBasicBackpack)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureLC_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.Cure;
      selectedQuality = PotionQuality.Lesser;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureGC_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.Cure;
      selectedQuality = PotionQuality.Greater;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureGS_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.Strength;
      selectedQuality = PotionQuality.Greater;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureTR_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.Refresh;
      selectedQuality = PotionQuality.Total;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureGH_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.Heal;
      selectedQuality = PotionQuality.Greater;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureMR_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.ManaRefresh;
      selectedQuality = PotionQuality.None;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureTMR_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.TotalManaRefresh;
      selectedQuality = PotionQuality.None;

      if ((ModifierKeys & Keys.Control) == Keys.Control)
        new Thread(new ThreadStart(MixureSelection)).Start();
      else
        new Thread(new ThreadStart(DrinkSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixureINV_Click(object sender, MouseEventArgs e)
    {
      selectedPotion = Potion.Invisibility;
      selectedQuality = PotionQuality.None;

      if (e.Button == MouseButtons.Left)
        new Thread(new ThreadStart(DrinkSelection)).Start();
      else
        new Thread(new ThreadStart(MixureSelection)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void BtnStatRepair_Click(object sender, MouseEventArgs e)
    {
      new Thread(new ThreadStart(ItemHelper.opravstaty)).Start();
    }

    //---------------------------------------------------------------------------------------------

    private void CbxPotionQualities_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!this.initRun)
      {
        this.PotionQuality = (PotionQuality)this.cbxPotionQualities.SelectedItem;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void CbxPotions_SelectedValueChanged(object sender, EventArgs e)
    {
      if (!this.initRun)
      {
        this.Potion = (Potion)this.cbxPotions.SelectedItem;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void BtnMixurePotion_Click(object sender, EventArgs e)
    {
      selectedPotion = (Potion)this.cbxPotions.SelectedItem;
      selectedQuality = (PotionQuality)this.cbxPotionQualities.SelectedItem;

      new Thread(new ThreadStart(MixureSelection)).Start();
      //Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["MixurePotion"], ((Potion)this.cbxPotions.SelectedItem).Name, ((PotionQuality)this.cbxPotionQualities.SelectedItem).ToString());
      //Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MixurePotion((Potion)this.cbxPotions.SelectedItem, (PotionQuality)this.cbxPotionQualities.SelectedItem);
      //Alchemy.ExecMixurePotion("", "");
    }

    //---------------------------------------------------------------------------------------------
    private Potion selectedPotion;
    private PotionQuality selectedQuality;
    private void  MixureSelection()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MixurePotion(selectedPotion, selectedQuality);
    }

    private void DrinkSelection()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion(selectedPotion);
    }

    //---------------------------------------------------------------------------------------------

    #endregion

    //---------------------------------------------------------------------------------------------
  }
}
