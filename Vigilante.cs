﻿//Copyright 2015 Guadmaz
//Do not redistribute this project without my permission.
//Contact me at rockeurp@gmail.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using GTA;
using GTA.Native;
using GTA.Math;

public class Vigilante : Script
{
    private UIText _debug;
    private Ped _player;

    private bool OnMission = false;
    private int level = 1;
    private int kills = 0;
    private bool Fighting = false;

    private List<Ped> Criminals = new List<Ped>();
    private List<WorkingBlip> CriminalBlips = new List<WorkingBlip>();

    private Random rndGet = new Random();
    
    private int[] weaponList = new int[] { 0x1B06D571, 0x5EF9FEC4, 0x22D8FE39, 0x13532244, 0x2BE6766B, 0x1D073A89, 0x7846A318, 0x3AABBBAA };
    //private WeaponHash[] weaponList = new WeaponHash[] { WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.APPistol, WeaponHash.BullpupShotgun, WeaponHash.SawnOffShotgun, WeaponHash.MicroSMG, WeaponHash.SMG, WeaponHash.AssaultRifle, WeaponHash.CarbineRifle };

    private int CriminalGroup;
    private int PlayerGroup;

    private UIText headsup;
    private UIRectangle headsupRectangle;

    private Model[] VehicleList = new Model[] { new Model(VehicleHash.Oracle),
        new Model(VehicleHash.Buffalo),
        new Model(VehicleHash.Exemplar),
        new Model(VehicleHash.Sultan),
        new Model(VehicleHash.Tailgater),
    };

    private int seconds = 0;
    private int tick = 0;

    public Vigilante()
    {
        KeyDown += OnKeyDown;
        Tick += OnTick;

        //this._debug = new UIText("debug goes here", new Point(10, 10), 0.5f, Color.White, 0, false);
        this._player = Game.Player.Character;
        //this._player.IsEnemy = true;
        //CriminalGroup = World.AddRelationShipGroup("Criminals");
        //PlayerGroup = World.AddRelationShipGroup("Player");
        OutputArgument outArg = new OutputArgument();
        Function.Call(Hash.ADD_RELATIONSHIP_GROUP, new InputArgument("CRIMINALS_MOD"), outArg);
        CriminalGroup = outArg.GetResult<int>();

        //Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, 5, 0x6F0783F5, CriminalGroup);
        //Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, 5, CriminalGroup, 0x6F0783F5);

        //this._player.RelationshipGroup = PlayerGroup;
        this.headsup = new UIText("Level: ~b~" + this.level.ToString(), new Point(2, 325), 0.7f, Color.WhiteSmoke, 1, false);
        this.headsupRectangle = new UIRectangle(new Point(0, 320), new Size(180, 110), Color.FromArgb(100, 0, 0, 0));

        //World.SetRelationshipBetweenGroups(Relationship.Hate, CriminalGroup, PlayerGroup);
    }

    void OnTick(object sender, EventArgs e)
    {
        //this._debug.Text = ;
        //this._debug.Draw();
        BigMessage.OnTick();


        if (this.OnMission)
        {
            Game.Player.WantedLevel = 0;
            if (this.tick >= 60)
            {
                this.seconds--;
                this.tick = 0;
            }
            else
            {
                this.tick++;
            }
            /* //FUTURE
            this.headsup.Caption = "Level: ~b~" + this.level.ToString();
            this.headsup.Caption += "~w~\nTime Left: ~b~" + this.ParseTime(this.seconds);
            this.headsup.Caption += "~w~\nKills: ~b~" + this.kills; */
            this.headsup.Text = "Level: ~b~" + this.level.ToString();
            this.headsup.Text += "~w~\nTime Left: ~b~" + this.ParseTime(this.seconds);
            this.headsup.Text += "~w~\nKills: ~b~" + this.kills;
            this.headsup.Draw();
            this.headsupRectangle.Draw();
            if (this.seconds < 0)
            {
                this.StopMissions();
            }
            else
            {
                for (int i = 0; i < this.Criminals.Count; i++)
                {
                    if (this.Criminals[i].IsDead)
                    {
                        this.kills++;
                        AddCash(20 * this.level);
                        this.Criminals[i].MarkAsNoLongerNeeded();
                        this.Criminals.RemoveAt(i);
                        //this.CriminalBlips[i].Remove();
                        this.CriminalBlips[i].Alpha = 0.0f;
                        this.CriminalBlips.RemoveAt(i);
                        if (this.Criminals.Count == 0)
                        {
                            this.level++;
                            int secsadded = rndGet.Next(60, 200);
                            BigMessage.ShowMessage("~b~" + secsadded + " ~w~seconds added", 200, Color.White, 1.0f);
                            this.seconds += secsadded;
                            UI.Notify("Good job officer! You've completed this level.");
                            StartMissions();
                        }
                    }
                    else
                    {
                        if (this.Criminals[i].IsInVehicle())
                        {
                            Ped playa = Game.Player.Character;
                            //this._debug.Caption = this.Criminals[i].CurrentVehicle.Speed.ToString();
                            if ((playa.Position - this.Criminals[i].Position).Length() < 20.0f && this.Criminals[i].CurrentVehicle.Speed < 1.0f)
                            {
                                if (!this.Fighting)
                                {
                                    this.Fighting = true;
                                    this.Criminals[i].Task.LeaveVehicle();
                                    this.Criminals[i].Task.FightAgainst(this._player, 100000);
                                }
                            }
                            else if (this.Fighting)
                            {
                                this.Criminals[i].Task.CruiseWithVehicle(this.Criminals[i].CurrentVehicle, 60.0f, 0);
                                this.Fighting = false;
                            }
                        }
                        else
                        {
                            if ((this._player.Position - this.Criminals[i].Position).Length() < 20.0f)
                            {
                                if (!this.Fighting)
                                {
                                    this.Fighting = true;
                                    this.Criminals[i].Task.FightAgainst(this._player, 100000);
                                    //this.Criminals[i].Task.CruiseWithVehicle(this.Criminals[i].CurrentVehicle, 30.0f, 0);
                                }
                            }
                            else
                            {
                                if (this.Fighting)
                                {
                                    //this.Criminals[i].Task.EnterVehicle();
                                    //this.Criminals[i].Task.CruiseWithVehicle(this.Criminals[i].CurrentVehicle, 30.0f, 0);
                                    //this.Fighting = false;
                                }
                                else
                                {
                                    //
                                }
                            }
                        }


                    }
                }
                if (this._player.IsDead)
                {
                    this.StopMissions();
                }
            }
        }
    }

    //int GET_CLOSEST_VEHICLE_NODE(float x, float y, float z, float *nodeX, int p4, float p5, float p6)
    //int GET_CLOSEST_VEHICLE_NODE(Vector3pos, Vector3 pointer, 1 ,ID, 0)
    //nodeX being a pointer to a Vector3's x value.
    //Vector3 nodePos;
    //GET_CLOSEST_VEHICLE_NODE(x,y,z,&nodePos.x,...)
    //p4, p5 and p6 unknown. Possibly representing radius.

    //Any GET_SAFE_COORD_FOR_PED(float x, float y, float z, Any p3, Object *outVector, Any p5) // B61C8E878A4199CA B370270A
    //                                                      ^radius?     ^pointer          ^??
    //                                                       ^ 0                        ^16 or 0

    void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.D2)
        {
            if (!this.OnMission && this.IsInPoliceCar())
            {
                this.seconds = 210;
                StartMissions();
                BigMessage.ShowMessage("Vigilante", 300, Color.Goldenrod);
            }
            else if (this.OnMission)
            {
                StopMissions();
            }
        }
    }

    void AddCash(int amount)
    {
        string statNameFull = string.Format("SP{0}_TOTAL_CASH", (Game.Player.Character.Model.Hash == new Model("player_zero").Hash) ? 0 :    //Michael
                                                                (Game.Player.Character.Model.Hash == new Model("player_one").Hash) ? 1 :     //Franklin
                                                                (Game.Player.Character.Model.Hash == new Model("player_two").Hash) ? 2 : 0); //Trevor
        int hash = GTA.Native.Function.Call<int>(GTA.Native.Hash.GET_HASH_KEY, statNameFull);
        int val = 0;
        GTA.Native.OutputArgument outArg = new GTA.Native.OutputArgument();
        GTA.Native.Function.Call<bool>(GTA.Native.Hash.STAT_GET_INT, hash, outArg, -1);
        val = outArg.GetResult<int>() + amount;
        GTA.Native.Function.Call(GTA.Native.Hash.STAT_SET_INT, hash, val, true);
    }

    private string ParseTime(int seconds)
    {
        decimal mins = Math.Floor(Convert.ToDecimal(seconds) / 60.0M);
        int sec = seconds % 60;
        if (sec <= 9)
        {
            return String.Format("{0}:0{1}", mins, sec);
        }
        else
        {
            return String.Format("{0}:{1}", mins, sec);
        }
    }

    Vector3 GetSafeRoadPos(Vector3 OriginalPos)
    {
        OutputArgument outArg = new OutputArgument();
        int tmp = Function.Call<int>(Hash.GET_CLOSEST_VEHICLE_NODE, OriginalPos.X, OriginalPos.Y, OriginalPos.Z, outArg, 1, 1077936128, 0);
        Vector3 output = outArg.GetResult<Vector3>();
        return output;
    }

    Vector3 GetSafePedPos(Vector3 OriginalPos)
    {
        OutputArgument out2 = new OutputArgument();
        Function.Call(Hash.GET_SAFE_COORD_FOR_PED, OriginalPos.X, OriginalPos.Y, OriginalPos.Z, 0, out2, 16);
        Vector3 out2pos = out2.GetResult<Vector3>();
        return out2pos;
    }
    private bool IsInPoliceCar()
    {
        Model[] CopCars = new Model[] {
            new Model(VehicleHash.Police),
            new Model(VehicleHash.Police2),
            new Model(VehicleHash.Police3),
            new Model(VehicleHash.Police4),
            new Model(VehicleHash.PoliceOld1),
            new Model(VehicleHash.PoliceOld2),
            new Model(VehicleHash.Hydra),
            new Model(VehicleHash.Rhino),
            new Model(VehicleHash.Annihilator),
            new Model(VehicleHash.Buzzard),
            new Model(VehicleHash.Savage),
            new Model(VehicleHash.FBI),
            new Model(VehicleHash.FBI2),
            new Model(VehicleHash.Policeb),
            new Model(VehicleHash.PoliceT),
            new Model(VehicleHash.Sheriff),
            new Model(VehicleHash.Sheriff2),
            new Model(VehicleHash.Lazer),
        };
        if (this._player.IsInVehicle())
        {
            if (CopCars.Contains(this._player.CurrentVehicle.Model))
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    //CREATE_PED_INSIDE_VEHICLE(Vehicle vehicle, int pedType, Hash modelHash, int seat, BOOL p4, BOOL p5) // 
    //void SET_PED_INTO_VEHICLE(Ped PedHandle, Vehicle VehicleHandle, int SeatIndex)

    private void StartMissions()
    {
        this.OnMission = true;
        UI.ShowSubtitle("Eliminate the ~r~suspects~w~.", 10000);        
        
        for (int i = 1; i <= Math.Ceiling((decimal)this.level/4); i++)
        {
            Model vehModel = this.VehicleList[rndGet.Next(0, this.VehicleList.Length)];
            //Model vehModel = new Model(VehicleHash.Police2);
            if (vehModel.Request(100))
            {
                Vector3 playerpos = this._player.Position;

                Vector3 v;
			    v.X = (float)(rndGet.NextDouble() - 0.5);
                v.Y = (float)(rndGet.NextDouble() - 0.5);
			    v.Z = 0.0f;
			    v.Normalize();
                playerpos += v * 500.0f;

                Vector3 pedSpawnPoint = GetSafeRoadPos(playerpos);
                Vehicle tmpVeh = World.CreateVehicle(vehModel, pedSpawnPoint);
                //tmpVeh.PlaceOnGround(); //FUTURE
                tmpVeh.SetOnGround();
                tmpVeh.IsPersistent = true;
                
                int maxPasseng = 0;

                if (i == Math.Ceiling((decimal)this.level / 4))
                {
                    maxPasseng = this.level % 4;
                    if (maxPasseng == 0)
                        maxPasseng = 4;
                }
                else
                {
                    maxPasseng = 4;
                }

                for (int d = 0; d < maxPasseng; d++)
                {
                    Ped tmpPed = GTA.Native.Function.Call<Ped>(GTA.Native.Hash.CREATE_RANDOM_PED, pedSpawnPoint.X, pedSpawnPoint.Y, pedSpawnPoint.Z);
                    int gunid;
                    if (this.level > this.weaponList.Length)
                    {
                        gunid = this.weaponList[rndGet.Next(0, this.weaponList.Length)];
                    }
                    else
                    {
                        gunid = this.weaponList[rndGet.Next(0, this.level)];
                    }                 
                    //tmpPed.Weapons.Give(gunid, 999, true, true); //FUTURE
                    Function.Call(Hash.GIVE_WEAPON_TO_PED, new InputArgument(tmpPed.ID), new InputArgument(gunid), new InputArgument(999), new InputArgument(true), new InputArgument(true));
                    //SET_PED_INTO_VEHICLE(Ped PedHandle, Vehicle VehicleHandle, int SeatIndex) // F75B0D629E1C063D 07500C79
                    if (d == 0)
                        Function.Call(Hash.SET_PED_INTO_VEHICLE, tmpPed.ID, tmpVeh.ID, -1); //-1 driver, -2 any
                    else
                        Function.Call(Hash.SET_PED_INTO_VEHICLE, tmpPed.ID, tmpVeh.ID, -2);

                    Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, tmpPed.ID, CriminalGroup);
                    tmpPed.Task.CruiseWithVehicle(tmpPed.CurrentVehicle, 60.0f, 6);
                    tmpPed.IsPersistent = true;
                    //tmpPed.RelationshipGroup = this.CriminalGroup; //FUTURE
                    tmpPed.IsEnemy = true;
                    tmpPed.CanSwitchWeapons = true;

                    WorkingBlip tmpBlip = WorkingBlip.Create(tmpPed);
                    //tmpBlip.Color = BlipColor.Red; //FUTURE
                    tmpBlip.Colour = 1;
                    this.CriminalBlips.Add(tmpBlip);
                    this.Criminals.Add(tmpPed);
                }
                tmpVeh.MarkAsNoLongerNeeded();
            }
            else
            {
                UI.Notify("Error loading vehicle.");
            }
        }
    }

    private void StopMissions()
    {
        this.OnMission = false;        
        this.level = 1;
        this.kills = 0;
        this.seconds = 210;
        UI.ShowSubtitle("");
        foreach (var item in CriminalBlips)
            item.Alpha = 0.0f;
            //item.Remove();
        foreach (var item in Criminals)
            item.MarkAsNoLongerNeeded();
        
        this.Criminals.Clear();
        this.CriminalBlips.Clear();
    }

    private void LogToFile(string text)
    {
        using (StreamWriter w = File.AppendText("log.txt"))
        {
            w.Write(text + "\n");
        }
    }
}
