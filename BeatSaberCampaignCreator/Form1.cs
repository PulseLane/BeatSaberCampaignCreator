using CustomCampaigns;
using CustomCampaigns.Campaign;
using CustomCampaigns.Campaign.Missions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CustomCampaigns.Campaign.Missions.UnlockableItem;

namespace BeatSaberCampaignCreator
{
    public partial class Form1 : Form
    {
        Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderBrowserDialog1;
        Campaign campaign = null;
        Bitmap backgroundBitmap = null;
        string currentDirectory;
        int currentMission = 0;
        bool isLoading = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddMission();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= campaign.missions.Count() || campaign.missions.Count() < 0) return;
            currentMission = listBox1.SelectedIndex;
            SetDataToCurrentSelected();
        }
        private void AddMission()
        {
            listBox1.Items.Add(listBox1.Items.Count);
            Mission mission = new Mission();
            mission.modifiers = new MissionModifiers();
            mission.modifiers.speedMul = 1;
            mission.requirements = new MissionRequirement[0];
            campaign.missions.Add(mission);
        }
        public void SetDataToCurrentSelected()
        {
            if (campaign == null) return;
            isLoading = true;
            Mission mission = campaign.missions[currentMission];
            missionName.Text = mission.name;
            beatmapCharacteristic.Text = mission.characteristic;
            songID.Text = mission.songid;
            customDownloadURL.Text = mission.customDownloadURL;
            difficulty.SelectedIndex = (int) mission.difficulty;
            disappearingArrows.Checked = mission.modifiers.disappearingArrows;
            strictAngles.Checked = mission.modifiers.strictAngles;
            fastNotes.Checked = mission.modifiers.fastNotes;
            noBombs.Checked = mission.modifiers.noBombs;
            failOnSaberClash.Checked = mission.modifiers.failOnSaberClash;
            instaFail.Checked = mission.modifiers.instaFail;
            noFail.Checked = mission.modifiers.noFail;
            batteryEnergy.Checked = mission.modifiers.batteryEnergy;
            ghostNotes.Checked = mission.modifiers.ghostNotes;
            noArrows.Checked = mission.modifiers.noArrows;
            speedMul.Value = (decimal) mission.modifiers.speedMul;
            energyType.SelectedIndex = (int) mission.modifiers.energyType;
            enabledObstacles.SelectedIndex = (int) mission.modifiers.enabledObstacleType;
            songUnlockable.Checked = mission.unlockMap;

            reqList.Items.Clear();
            foreach(MissionRequirement req in mission.requirements)
            {
                reqList.Items.Add(reqList.Items.Count);
            }
            if (reqList.Items.Count > 0) reqList.SelectedIndex = 0;
            SetRequirementToSelected();
            UpdateRequirementToggleState();

            externalModsList.Items.Clear();
            foreach(KeyValuePair<string, string[]> pair in mission.externalModifiers)
            {
                externalModsList.Items.Add(pair.Key);
            }
            if (externalModsList.Items.Count > 0) externalModsList.SelectedIndex = 0;

            unlockableListBox.Items.Clear();
            foreach (UnlockableItem req in mission.unlockableItems)
            {
                unlockableListBox.Items.Add(unlockableListBox.Items.Count);
            }
            if (unlockableListBox.Items.Count > 0) unlockableListBox.SelectedIndex = 0;
            SetUnlockableToSelected();
            UpdateUnlockableToggleState();

            segments.Items.Clear();
            if (mission.missionInfo != null)
            {
                foreach (InfoSegment segment in mission.missionInfo.segments)
                {
                    segments.Items.Add(segments.Items.Count);
                }
                if (segments.Items.Count > 0) segments.SelectedIndex = 0;
            }
            infoEnabled.Checked = mission.missionInfo != null;
            SetInfoToSelected();
            SetSegmentToSelected();
            UpdateInfoToggleState();

            isExtLoading = true;
            textBox1.Text = "";
            isExtLoading = false;
            isLoading = false;
            
        }
        private void MissionDataValueChange(object sender, EventArgs e)
        {
            if (isLoading) return;
            UpdateMissionInfo();
        }
        private void CampaignDataValueChange(object sender, EventArgs e)
        {
            if (updatingCampaign) return;
            UpdateCampaignInfo();
        }
        public void UpdateMissionInfo()
        {
            if (campaign == null) return;
            Mission mission = campaign.missions[currentMission];
            mission.name = missionName.Text;
            mission.characteristic = beatmapCharacteristic.Text;
            mission.songid = songID.Text;
            mission.customDownloadURL = customDownloadURL.Text;
            mission.difficulty = (BeatmapDifficulty)difficulty.SelectedIndex;
            mission.modifiers.disappearingArrows = disappearingArrows.Checked;
            mission.modifiers.strictAngles = strictAngles.Checked;
            mission.modifiers.fastNotes = fastNotes.Checked;
            mission.modifiers.noBombs = noBombs.Checked;
            mission.modifiers.failOnSaberClash = failOnSaberClash.Checked;
            mission.modifiers.instaFail = instaFail.Checked;
            mission.modifiers.noFail = noFail.Checked;
            mission.modifiers.batteryEnergy = batteryEnergy.Checked;
            mission.modifiers.ghostNotes = ghostNotes.Checked;
            mission.modifiers.noArrows = noArrows.Checked;
            mission.modifiers.speedMul = (float)speedMul.Value;
            mission.modifiers.energyType = (GameplayModifiers.EnergyType)energyType.SelectedIndex;
            mission.modifiers.enabledObstacleType = (GameplayModifiers.EnabledObstacleType)enabledObstacles.SelectedIndex;
            mission.unlockMap = songUnlockable.Checked;
        }
        bool updatingCampaign = false;
        public void SetCampaignInfoData()
        {
            updatingCampaign = true;
            campaignName.Text = campaign.info.name;
            campaignDesc.Text = campaign.info.desc;
            bigDesc.Text = campaign.info.bigDesc;
            allUnlocked.Checked = campaign.info.allUnlocked;
            numericUpDown1.Value = campaign.info.mapHeight;
            backgroundAlpha.Value = (decimal)campaign.info.backgroundAlpha;
            updatingCampaign = false;
        }
        public void UpdateCampaignInfo()
        {
            campaign.info.name = campaignName.Text;
            campaign.info.desc = campaignDesc.Text;
            campaign.info.bigDesc = bigDesc.Text;
            campaign.info.allUnlocked = allUnlocked.Checked;
            campaign.info.backgroundAlpha = (float)backgroundAlpha.Value;
        }

        //Header
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog().Value)
            {
                Location = new Point(Location.X+1, Location.Y);
                string path = folderBrowserDialog1.SelectedPath;
                currentDirectory = path;
                campaign = new Campaign();
                campaign.info = new CampaignInfo();
                campaign.missions = new List<Mission>();
                listBox1.Items.Clear();
                AddMission();
                currentMission = 0;
                tabControl1.Enabled = true;
                SetCampaignInfoData();
                SetDataToCurrentSelected();
                PrepareMap();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog().Value)
            {
                Location = new Point(Location.X + 1, Location.Y);
                string path = folderBrowserDialog1.SelectedPath;
                currentDirectory = path;
                campaign = new Campaign();
                campaign.info = JsonConvert.DeserializeObject<CampaignInfo>(File.ReadAllText(path + "/info.json"));
                campaign.missions = new List<Mission>();
                currentMission = 0;
                int i = 0;
                listBox1.Items.Clear();
                while (File.Exists(path + "/" + i + ".json"))
                {
                    campaign.missions.Add(JsonConvert.DeserializeObject<Mission>(File.ReadAllText(path + "/" + i + ".json").Replace("\n", "")));
                    listBox1.Items.Add(listBox1.Items.Count);
                    i++;
                }
                SetCampaignInfoData();
                SetDataToCurrentSelected();
                SetRequirementToSelected();
                PrepareMap();
                tabControl1.Enabled = true;
                if (File.Exists(path + "/map background.png"))
                {
                    backgroundBitmap = new Bitmap(path + "/map background.png");
                } else
                {
                    backgroundBitmap = null;
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (campaign == null) return;
            File.WriteAllText(currentDirectory + "/info.json", JsonConvert.SerializeObject(campaign.info));
            for(int i = 0; i < campaign.missions.Count; i++)
            {
                File.WriteAllText(currentDirectory + "/" + i + ".json", JsonConvert.SerializeObject(campaign.missions[i]));
            }
        }

        //requirement stuff
        int curRequirementIndex;
        bool reqUpdating = false;
        private void addReq_Click(object sender, EventArgs e)
        {
            reqList.Items.Add(reqList.Items.Count);
            Mission mission = campaign.missions[currentMission];
            List<MissionRequirement> tempList = new List<MissionRequirement>();
            foreach(MissionRequirement req in mission.requirements)
            {
                tempList.Add(req);
            }
            tempList.Add(new MissionRequirement());
            mission.requirements = tempList.ToArray();
            UpdateRequirementToggleState();
        }

        private void remReq_Click(object sender, EventArgs e)
        {
            Mission mission = campaign.missions[currentMission];
            List<MissionRequirement> tempList = new List<MissionRequirement>();
            foreach (MissionRequirement req in mission.requirements)
            {
                tempList.Add(req);
            }
            tempList.RemoveAt(curRequirementIndex);
            mission.requirements = tempList.ToArray();
            reqList.Items.Clear();
            foreach (MissionRequirement req in mission.requirements)
            {
                reqList.Items.Add(reqList.Items.Count);
            }
            if (reqList.Items.Count > 0) reqList.SelectedIndex = 0;
            UpdateRequirementToggleState();
        }

        private void reqList_SelectedIndexChanged(object sender, EventArgs e)
        {
            curRequirementIndex = reqList.SelectedIndex;
            SetRequirementToSelected();
        }
        private void RequirementValueChanged(object sender, EventArgs e)
        {
            if (reqUpdating) return;
            UpdateRequirementInfo();
        }
        public void UpdateRequirementInfo()
        {
            if (campaign == null) return;
            Mission mission = campaign.missions[currentMission];
            if (mission.requirements.Count() == 0 || curRequirementIndex >= mission.requirements.Count()) return;
            MissionRequirement requirement = mission.requirements[curRequirementIndex];
            requirement.type = requirementType.Text;
            requirement.count = (int)requirementValue.Value;
            requirement.isMax = requirementIsMax.Checked;
        }
        public void SetRequirementToSelected()
        {
            if (campaign == null) return;
            Mission mission = campaign.missions[currentMission];
            if (mission.requirements.Count() == 0 || curRequirementIndex >= mission.requirements.Count()) return;
            reqUpdating = true;
            MissionRequirement requirement = mission.requirements[curRequirementIndex];
            requirementType.Text = requirement.type;
            requirementValue.Value = requirement.count;
            requirementIsMax.Checked = requirement.isMax;
            reqUpdating = false;
        }
        public void UpdateRequirementToggleState()
        {
            Mission mission = campaign.missions[currentMission];

            bool isEnabled = !(mission.requirements.Count() == 0 || curRequirementIndex >= mission.requirements.Count());
            requirementType.Enabled = isEnabled;
            requirementValue.Enabled = isEnabled;
            requirementIsMax.Enabled = isEnabled;
        }


        //External Modifier stuff
        private void addExtMod_Click(object sender, EventArgs e)
        {
            Mission mission = campaign.missions[currentMission];
            string modName = ShowDialog("Mod Name (ex GameplayModifiersPlus)", "External Modifier Mod Name");
            mission.externalModifiers.Add(modName, new string[0]);
            externalModsList.Items.Add(modName);
            externalModsList.SelectedIndex = externalModsList.Items.Count - 1;
        }

        private void remExtMod_Click(object sender, EventArgs e)
        {
            Mission mission = campaign.missions[currentMission];
            mission.externalModifiers.Remove((string)externalModsList.SelectedItem);
            externalModsList.Items.Clear();
            foreach (KeyValuePair<string, string[]> pair in mission.externalModifiers)
            {
                externalModsList.Items.Add(pair.Key);
            }
            if (externalModsList.Items.Count > 0) externalModsList.SelectedIndex = 0;
        }
        bool isExtLoading = false;
        private void externalModsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (campaign == null) return;
            Mission mission = campaign.missions[currentMission];
            isExtLoading = true;
            textBox1.Text = "";
            foreach (string line in mission.externalModifiers[(string)externalModsList.SelectedItem])
            {
                textBox1.Text += line + "\r\n";
            }
            textBox1.Text = textBox1.Text.TrimEnd('\n');
            textBox1.Text = textBox1.Text.TrimEnd('\r');
            isExtLoading = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isExtLoading) return;
            if (campaign == null) return;
            Mission mission = campaign.missions[currentMission];
            mission.externalModifiers.Remove((string)externalModsList.SelectedItem);
            string[] lines = textBox1.Text.Split('\n');
            for(int i = 0; i < lines.Count(); i++)
            {
                lines[i] = lines[i].Trim('\r');
            }
            mission.externalModifiers.Add((string)externalModsList.SelectedItem,lines);
        }


        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Width = 1000, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }


        //MAP STUFF
        public enum MapState
        {
            ADD,CONNECT,CONNECTING,DISCONNECT,DISCONNECTING,MOVE,MOVING,EDIT,ADD_GATE,REMOVE_GATES,EDIT_GATES,MOVE_GATES,MOVING_GATE
        }
        private bool CanSwitchState()
        {
            return currentState != MapState.MOVING && currentState != MapState.ADD && currentState != MapState.CONNECTING && currentState != MapState.DISCONNECTING && currentState != MapState.ADD_GATE && currentState != MapState.MOVING_GATE;
        }
        MapState currentState = MapState.EDIT;
        private void setState(MapState newState)
        {
            currentState = newState;
            mapState.Text = "State: " + currentState.ToString();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (updatingCampaign) return;
            int dist = (int)numericUpDown1.Value - campaign.info.mapHeight;
            UpdateMapHeight((int)numericUpDown1.Value);
            MoveEverything(dist);
        }
        public void UpdateMapHeight(int height)
        {
            campaign.info.mapHeight = height;
            mapArea.Height = height;
        }
        public void MoveEverything(int dist)
        {
            foreach(Control control in mapArea.Controls)
            {
                if (control is GateButton)
                {
                    (control as GateButton).SetPosition(new Point(control.Location.X, control.Location.Y + dist));
                }
                if (control is NodeButton)
                {
                    (control as NodeButton).SetPosition(new Point(control.Location.X, control.Location.Y + dist));
                }
            }
        }

        Pen pen = new Pen(Brushes.Red, 5);

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (backgroundBitmap != null)
            {
                int renderedHeight = mapArea.Width * backgroundBitmap.Height / backgroundBitmap.Width;
                g.DrawImage(backgroundBitmap, 0, mapArea.Height-renderedHeight, mapArea.Width, renderedHeight);
            }
            pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            foreach(NodeButton node in nodes)
            {
                foreach(NodeButton childNode in node.children)
                {
                    int closestPointX = Math.Max(childNode.Location.X - childNode.Width / 2, Math.Min(node.Location.X, childNode.Location.X + childNode.Width / 2));
                    int closestPointY = Math.Max(childNode.Location.Y - childNode.Height / 2, Math.Min(node.Location.Y, childNode.Location.Y + childNode.Height / 2));
                    g.DrawLine(pen, closestPointX+childNode.Width/2, closestPointY + childNode.Height / 2, node.Location.X + node.Width / 2, node.Location.Y + node.Height / 2);
                }
            }
        }

        NodeButton currentNode;
        List<NodeButton> nodes = new List<NodeButton>();
        public void PrepareMap()
        {
            foreach (NodeButton node in nodes)
            {
                node.Parent.Controls.Remove(node);
            }
            foreach (GateButton gate in gates)
            {
                gate.Parent.Controls.Remove(gate);
            }
            nodes.Clear();
            gates.Clear();
            UpdateMapHeight(campaign.info.mapHeight);
            foreach (CampaignMapPosition mapPos in campaign.info.mapPositions)
            {
                currentNode = new NodeButton();
                mapArea.Controls.Add(currentNode);
                currentNode.mapPosition = mapPos;
                currentNode.Location = new Point((int)mapPos.x - currentNode.Width / 2 + (currentNode.Parent.Bounds.Width) / 2, (int)mapPos.y - currentNode.Height / 2);
                currentNode.Text = nodes.Count + "";
                currentNode.Click += clickNode;
                nodes.Add(currentNode);
            }
            foreach (CampaignUnlockGate gate in campaign.info.unlockGate)
            {
                currentGate = new GateButton();
                mapArea.Controls.Add(currentGate);
                currentGate.unlockGate = gate;
                currentGate.Location = new Point((int)gate.x - currentGate.Width / 2 + (currentGate.Parent.Bounds.Width) / 2, (int)gate.y - currentGate.Height / 2);
                currentGate.Text = "UNLOCK GATE";
                currentGate.Click += clickGate;
                gates.Add(currentGate);
            }
            foreach (NodeButton node in nodes)
            {
                List<NodeButton> children = new List<NodeButton>();
                foreach(int i in node.mapPosition.childNodes)
                {
                    children.Add(nodes[i]);
                }
                node.children = children;
            }
            mapArea.Refresh();
        }

        private void addNode_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            if (campaign.info.mapPositions.Count >= campaign.missions.Count)
            {
                ShowDialog("You cannot have more nodes than missions, if you wish to add more ndoes, add the missions for those ndoes first." , "Error");
                return;
            }
            setState(MapState.ADD);
            currentNode = new NodeButton();
            currentNode.Text = nodes.Count+"";
            mapArea.Controls.Add(currentNode);
            currentNode.Click += placeNode;
            //this.Controls.Add(button);
        }

        private void placeNode(object sender, EventArgs e)
        {
            if (currentState != MapState.ADD) return;
            currentNode.Click -= placeNode;
            currentNode.Click += clickNode;
            nodes.Add(currentNode);
            campaign.info.mapPositions.Add(currentNode.mapPosition);
            setState(MapState.EDIT);
            mapArea.Refresh();
        }
        private void clickNode(object sender, EventArgs e)
        {
            switch (currentState)
            {
                case MapState.CONNECT:
                    currentNode = (NodeButton)sender;
                    setState(MapState.CONNECTING);
                    break;
                case MapState.CONNECTING:
                    if (sender == currentNode) return;
                    currentNode.children.Add((NodeButton)sender);
                    currentNode.UpdateChildren(nodes);
                    setState(MapState.CONNECT);
                    mapArea.Refresh();
                    break;
                case MapState.DISCONNECT:
                    currentNode = (NodeButton)sender;
                    setState(MapState.DISCONNECTING);
                    break;
                case MapState.DISCONNECTING:
                    currentNode.children.Remove((NodeButton)sender);
                    currentNode.UpdateChildren(nodes);
                    setState(MapState.DISCONNECT);
                    mapArea.Refresh();
                    break;
                case MapState.MOVE:
                    currentNode = (NodeButton)sender;
                    setState(MapState.MOVING);
                    break;
                case MapState.MOVING:
                    setState(MapState.MOVE);
                    break;
                case MapState.EDIT:
                    new FormEditNode(((NodeButton)sender)).ShowDialog();
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentState == MapState.ADD || currentState == MapState.MOVING) currentNode.SetPosition(new Point(MousePosition.X - Bounds.Location.X - 10 - mapArea.Bounds.Location.X - panel1.Location.X - groupBox2.Location.X - tabControl1.Location.X - currentNode.Width / 2, MousePosition.Y - Bounds.Location.Y - 70 - mapArea.Bounds.Location.Y - panel1.Location.Y - groupBox2.Location.Y - tabControl1.Location.Y + currentNode.Height / 2));
            if (currentState == MapState.ADD_GATE || currentState == MapState.MOVING_GATE) currentGate.SetPosition(new Point(MousePosition.X - Bounds.Location.X - 10 - mapArea.Bounds.Location.X - panel1.Location.X - groupBox2.Location.X - tabControl1.Location.X - currentGate.Width / 2, MousePosition.Y - Bounds.Location.Y - 70 - mapArea.Bounds.Location.Y - panel1.Location.Y - groupBox2.Location.Y - tabControl1.Location.Y + currentGate.Height / 2));
            if (currentState == MapState.MOVING) mapArea.Refresh();
        }

        private void connectNodes_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.CONNECT);
        }

        private void disconnectNodes_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.DISCONNECT);
        }

        private void moveNodes_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.MOVE);
        }

        private void editNode_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.EDIT);
        }
        GateButton currentGate = new GateButton();
        List<GateButton> gates = new List<GateButton>();
        private void addGate_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.ADD_GATE);
            currentGate = new GateButton();
            currentGate.Text = "UNLOCK GATE";
            mapArea.Controls.Add(currentGate);
            currentGate.Click += placeGate;
        }

        private void placeGate(object sender, EventArgs e)
        {
            if (currentState != MapState.ADD_GATE) return;
            currentGate.Click -= placeGate;
            currentGate.Click += clickGate;
            gates.Add(currentGate);
            campaign.info.unlockGate.Add(currentGate.unlockGate);
            setState(MapState.EDIT_GATES);
            mapArea.Refresh();
        }
        private void clickGate(object sender, EventArgs e)
        {
            switch (currentState)
            {
                case MapState.REMOVE_GATES:
                    gates.Remove((GateButton)sender);
                    mapArea.Controls.Remove((GateButton)sender);
                    campaign.info.unlockGate.Remove(((GateButton)sender).unlockGate);
                    break;
                case MapState.MOVE_GATES:
                    currentGate = (GateButton)sender;
                    setState(MapState.MOVING_GATE);
                    break;
                case MapState.MOVING_GATE:
                    setState(MapState.MOVE_GATES);
                    break;
                case MapState.EDIT_GATES:
                    new FormEditGate(((GateButton)sender).unlockGate).ShowDialog();
                    break;
            }
        }

        private void removeGates_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.REMOVE_GATES);
        }

        private void editGates_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.EDIT_GATES);
        }

        private void moveGates_Click(object sender, EventArgs e)
        {
            if (!CanSwitchState()) return;
            setState(MapState.MOVE_GATES);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        
            folderBrowserDialog1 = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, mapArea, new object[] { true });
        }
        //Unlockable stuff
        int curUnlockableIndex;
        bool unlockableUpdating = false;
        private void addUnlockable_Click(object sender, EventArgs e)
        {
            unlockableListBox.Items.Add(unlockableListBox.Items.Count);
            Mission mission = campaign.missions[currentMission];
            mission.unlockableItems.Add(new UnlockableItem());
            UpdateUnlockableToggleState();
        }

        private void removeUnlockable_Click(object sender, EventArgs e)
        {
            Mission mission = campaign.missions[currentMission];
            mission.unlockableItems.RemoveAt(curUnlockableIndex);
            unlockableListBox.Items.Clear();
            foreach (UnlockableItem req in mission.unlockableItems)
            {
                unlockableListBox.Items.Add(unlockableListBox.Items.Count);
            }
            if (unlockableListBox.Items.Count > 0) unlockableListBox.SelectedIndex = 0;
            UpdateUnlockableToggleState();
        }

        private void unlockableListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            curUnlockableIndex = unlockableListBox.SelectedIndex;
            SetUnlockableToSelected();
        }
        private void UnlockableValueChanged(object sender, EventArgs e)
        {
            if (unlockableUpdating) return;
            UpdateUnlockableInfo();
        }
        public void UpdateUnlockableInfo()
        {
            Mission mission = campaign.missions[currentMission];
            if (mission.unlockableItems.Count() == 0 || curUnlockableIndex >= mission.unlockableItems.Count()) return;
            UnlockableItem unlockable = mission.unlockableItems[curUnlockableIndex];
            unlockable.fileName = unlockableFile.Text;
            unlockable.name = unlockableName.Text;
            unlockable.type = (UnlockableType)unlockableType.SelectedIndex;
        }
        public void SetUnlockableToSelected()
        {
            if (campaign == null) return;
            Mission mission = campaign.missions[currentMission];
            if (mission.unlockableItems.Count() == 0 || curUnlockableIndex >= mission.unlockableItems.Count()) return;
            unlockableUpdating = true;
            UnlockableItem unlockable = mission.unlockableItems[curUnlockableIndex];
            unlockableFile.Text = unlockable.fileName;
            unlockableName.Text = unlockable.name;
            unlockableType.SelectedIndex = (int)unlockable.type;
            unlockableUpdating = false;
        }
        public void UpdateUnlockableToggleState()
        {
            Mission mission = campaign.missions[currentMission];
            bool isEnabled = !(mission.unlockableItems.Count() == 0 || curUnlockableIndex >= mission.unlockableItems.Count());
            unlockableType.Enabled = isEnabled;
            unlockableFile.Enabled = isEnabled;
            unlockableName.Enabled = isEnabled;
        }


        //Mission Info
        int curSegmentIndex;
        bool infoUpdating = false;
        bool segmentUpdating = false;
        private void addSegment_Click(object sender, EventArgs e)
        {
            segments.Items.Add(segments.Items.Count);
            Mission mission = campaign.missions[currentMission];
            mission.missionInfo.segments.Add(new InfoSegment());
            UpdateInfoToggleState();
        }

        private void removeSegment_Click(object sender, EventArgs e)
        {
            Mission mission = campaign.missions[currentMission];
            mission.missionInfo.segments.RemoveAt(curSegmentIndex);
            segments.Items.Clear();
            foreach (InfoSegment req in mission.missionInfo.segments)
            {
                segments.Items.Add(segments.Items.Count);
            }
            if (segments.Items.Count > 0) segments.SelectedIndex = 0;
            UpdateInfoToggleState();
        }

        private void infoEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoading) return;
            Mission mission = campaign.missions[currentMission];
            if (infoEnabled.Checked)
            {
                mission.missionInfo = new MissionInfo();
            }
            else
            {
                mission.missionInfo = null;
            }
            SetInfoToSelected();
            SetSegmentToSelected();
            UpdateInfoToggleState();
        }

        //info
        private void InfoValueChanged(object sender, EventArgs e)
        {
            if (infoUpdating) return;
            UpdateInfo();
        }
        public void UpdateInfo()
        {
            Mission mission = campaign.missions[currentMission];
            if (mission.missionInfo == null) return;
            mission.missionInfo.showEverytime = appearsEverytime.Checked;
            mission.missionInfo.title = infoTitle.Text;
        }
        public void SetInfoToSelected()
        {
            Mission mission = campaign.missions[currentMission];
            if (mission.missionInfo == null) return;
            infoUpdating = true;
            appearsEverytime.Checked = mission.missionInfo.showEverytime;
            infoTitle.Text = mission.missionInfo.title;
            infoUpdating = false;
        }

        //segment
        private void SegmentValueChanged(object sender, EventArgs e)
        {
            if (segmentUpdating) return;
            UpdateSegmentInfo();
        }
        public void UpdateSegmentInfo()
        {
            Mission mission = campaign.missions[currentMission];
            if (mission.missionInfo == null) return;
            if (mission.missionInfo.segments.Count() == 0 || curSegmentIndex >= mission.missionInfo.segments.Count()) return;
            InfoSegment segment = mission.missionInfo.segments[curSegmentIndex];
            segment.hasSeparator = hasSeperator.Checked;
            segment.imageName = infoImageName.Text;
            segment.text = infoText.Text;
        }
        public void SetSegmentToSelected()
        {
            Mission mission = campaign.missions[currentMission];
            if (mission.missionInfo == null) return;
            if (mission.missionInfo.segments.Count() == 0 || curSegmentIndex >= mission.missionInfo.segments.Count()) return;
            segmentUpdating = true;
            InfoSegment segment = mission.missionInfo.segments[curSegmentIndex];
            hasSeperator.Checked = segment.hasSeparator;
            infoImageName.Text = segment.imageName;
            infoText.Text = segment.text;
            segmentUpdating = false;
        }
        private void segments_SelectedIndexChanged(object sender, EventArgs e)
        {

            curSegmentIndex = segments.SelectedIndex;
            SetSegmentToSelected();
        }
        public void UpdateInfoToggleState()
        {
            Mission mission = campaign.missions[currentMission];

            bool isEnabled = mission.missionInfo != null;
            infoTitle.Enabled = isEnabled;
            appearsEverytime.Enabled = isEnabled;
            segments.Enabled = isEnabled;
            addSegment.Enabled = isEnabled;
            removeSegment.Enabled = isEnabled;

            isEnabled = !(mission.missionInfo == null || mission.missionInfo.segments.Count() == 0 || curSegmentIndex >= mission.missionInfo.segments.Count());
            hasSeperator.Enabled = isEnabled;
            infoImageName.Enabled = isEnabled;
            infoText.Enabled = isEnabled;
        }

        //lights

        private void buttonLightColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            campaign.info.lightColor = new CampaignLightColor(colorDialog1.Color.R/255f, colorDialog1.Color.G / 255f, colorDialog1.Color.B / 255f);
        }
    }
}
