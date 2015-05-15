using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;

class WorkingBlip
{

    private readonly int id;
    private bool _route = false;

    public WorkingBlip(int id)
    {
        this.id = id;
    }

    public static WorkingBlip Create(Vector3 position)
    {
        return new WorkingBlip(Function.Call<int>(Hash.ADD_BLIP_FOR_COORD, new InputArgument[] { position.X, position.Y, position.Z }));
    }

    public static WorkingBlip Create(Entity entity)
    {
        return new WorkingBlip(Function.Call<int>(Hash.ADD_BLIP_FOR_ENTITY, new InputArgument(entity)));
    }

    public static WorkingBlip Create(Vector3 position, float radius)
    {
        var blip = new WorkingBlip(Function.Call<int>(Hash.ADD_BLIP_FOR_RADIUS, new InputArgument[] { position.X, position.Y, position.Z, radius }));
        return blip;
    }

    public void Remove()
    {
        Function.Call(Hash.REMOVE_BLIP, new InputArgument(this.id));
    }

    public static List<WorkingBlip> GetAllBlips()
    {
        int blipIterator = Function.Call<int>(Hash._GET_BLIP_INFO_ID_ITERATOR);
        WorkingBlip currentBlip = new WorkingBlip(Function.Call<int>(Hash.GET_FIRST_BLIP_INFO_ID, new InputArgument(blipIterator)));
        List<WorkingBlip> blipArray = new List<WorkingBlip>();
        while (currentBlip.Exists)
        {

            blipArray.Add(currentBlip);
            currentBlip = new WorkingBlip(Function.Call<int>(Hash.GET_NEXT_BLIP_INFO_ID, new InputArgument(blipIterator)));
        }
        return blipArray;
    }

    public static WorkingBlip PlayerBlip
    {
        get { return new WorkingBlip(Function.Call<int>(Hash.GET_MAIN_PLAYER_BLIP_ID)); }
    }

    public static bool PoliceBlips
    {
        set { Function.Call(Hash.SET_POLICE_RADAR_BLIPS, new InputArgument(value)); }
    }

    public int ID
    {
        get { return id; }
    }

    public bool Exists
    {
        get { return Function.Call<bool>(Hash.DOES_BLIP_EXIST, new InputArgument(id)); }
    }

    public int Type
    {
        get { return Function.Call<int>(Hash.GET_BLIP_INFO_ID_TYPE, new InputArgument(id)); }
    }

    public int Display
    {
        get { return Function.Call<int>(Hash.GET_BLIP_INFO_ID_DISPLAY, new InputArgument(id)); }
    }

    public Vector3 Position
    {
        get { return Function.Call<Vector3>(Hash.GET_BLIP_COORDS, new InputArgument(id)); }
        set { Function.Call(Hash.SET_BLIP_COORDS, new InputArgument[] { id, value.X, value.Y, value.Z }); }
    }

    public int EntityIndex
    {
        get { return Function.Call<int>(Hash.GET_BLIP_INFO_ID_ENTITY_INDEX, new InputArgument(id)); }
    }

    public bool HighDetail
    {
        set { Function.Call(Hash.SET_BLIP_HIGH_DETAIL, new InputArgument(id), new InputArgument(value)); }
    }

    public bool MissionCreator
    {
        set { Function.Call(Hash.SET_BLIP_AS_MISSION_CREATOR_BLIP, new InputArgument(id), new InputArgument(value)); }
    }

    public bool IsOnMinimap
    {
        get { return Function.Call<bool>(Hash.IS_BLIP_ON_MINIMAP, new InputArgument(id)); }
    }

    public bool Flashes
    {
        set { Function.Call(Hash.SET_BLIP_FLASHES, new InputArgument(value)); }
    }

    public float Scale
    {
        set { Function.Call(Hash.SET_BLIP_SCALE, new InputArgument(id), new InputArgument(value)); }
    }

    public bool ShowCone
    {
        set { Function.Call(Hash.SET_BLIP_SHOW_CONE, new InputArgument(value)); }
    }
    public int Colour
    {
        set { Function.Call(Hash.SET_BLIP_COLOUR, new InputArgument(id), new InputArgument(value)); }
        get { return Function.Call<int>(Hash.GET_BLIP_COLOUR, new InputArgument(id)); }
    }
    public bool Route
    {
        set { Function.Call(Hash.SET_BLIP_ROUTE, new InputArgument(id), new InputArgument(value)); this._route = value; }
        get { return this._route; }
    }
    public int Sprite
    {
        set { Function.Call(Hash.SET_BLIP_SPRITE, new InputArgument(id), new InputArgument(value)); }
        get { return Function.Call<int>(Hash.GET_BLIP_SPRITE, new InputArgument(id)); }
    }
    public float Alpha
    {
        set { Function.Call(Hash.SET_BLIP_ALPHA, new InputArgument(id), new InputArgument(value)); }
        get { return Function.Call<float>(Hash.GET_BLIP_ALPHA, new InputArgument(id)); }
    }
    public bool ShortRange
    {
        set { Function.Call(Hash.SET_BLIP_AS_SHORT_RANGE, new InputArgument(this.id), new InputArgument(value)); }
    }
}