<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 25cm 110cm; }
        body { margin: 0cm; }
    }
</style>

# **UHighlight** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub> - 0.9.0

This document presents how to integrate UHighlight in your plugins.

## UHighlight objects

- `IHighlightCommands`: Calls UHighlight commands from your plugin.
- `IHighlightSpawner`: Spawns zones.
- `HighlightedZone`: A zone instance.
- `IServiceAdapter`: Get `IHighlightCommands` and `IHighlightSpawner` instances from UHighlight
- `ServiceAdapter`: RocketMod implementation of `IServiceAdapter`

## Concepts

### Groups

Zones are grouped. You can create as many groups as you want. You can spawn all zones of a group at once.  
You should group zones that will be used together, or that share the same properties.  
Groups are shared by all plugins. You should prefix your groups with your plugin's name to prevent conflicts, and ease identification by the users.

## How to adapt commands

Adapting the UHighlight commands will allow you to intercept the creation process, and ask the user for additionnal information needed by your own plugin.  
For this, you will use the `IHighlightCommands` interface.  

OpenMod example : 
```csharp
public class AmbianceZoneCommand : UnturnedCommand
{
    private readonly IHighlightCommands _highlightCommands;

    public CreateZoneCommand(IServiceProvider serviceProvider, IHighlightCommands highlightCommands) : base(serviceProvider)
    {
        _highlightCommands = highlightCommands;
    }

    protected override async UniTask OnExecuteAsync()
    {
        UnturnedUser user = (UnturnedUser)Context.Actor;

        await _highlightCommands.ExecuteCreate(user.Player.Player, "Cube", "Transparent", "Blue");

        /* Your own registration */
    }
}
```

RocketMod example : 
```csharp
public class AmbianceZoneCommand : IRocketCommand
{
    /* ... */

    public async void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

        IHighlightCommands highlightCommands = new HighlightCommands();

        string zoneName = command[0];

        await highlightCommands.ExecuteValidate(uPlayer.Player, Constants.GROUP_NAME, zoneName);

        /* Your own registration */
    }
}
```

## How to spawn zones

### Spawn static zones

At any time, you can spawn existing zones, or groups of zones. For this, you will use the `IHighlightSpawner` interface.  
The methods of IHighlightSpawner will return `HighlightedZone` instances.  

*Note that when spawning zones, the plugin will first wait for both the framework (OpenMod/RocketMod) and the level to be loaded before spawning the zones.*  

Your plugin is responsible for the lifetime of the zones it spawns. When you spawn zones, you must keep a references to them, to later dipose them when not needed anymore.

Spawning a group of zone example : 
```csharp
public class Spawner : IDisposable
{
    private readonly List<HighlightedZone> _zones = new List<HighlightedZone>();

    private readonly IHighlightSpawner _highlightSpawner;

    public Spawner(IHighlightSpawner highlightSpawner)
    {
        _highlightSpawner = highlightSpawner
    }

    public async Task SpawnZones(string groupName)
    {
        IEnumerable<HighlightedZone> zones = _highlightSpawner.BuildZones(groupName);

        _zones.AddRange(zones);
    }

    public void Dispose()
    {
        foreach(HighlightedZone zone in _zones)
            zone.Dipose();

        _zones.Clear();
    }
}
```

### Spawn dynamic zones

Plugins can also dynamically create zones. For this, you will need to create a new `Volume` instance and use `IHighlightSpawner` to spawn it.  

To create a `Volume`, you will need to pass a center position, a size, a rotation, a shape, a material and a color to the constructor. 

## How to use zones

### Zone properties

From `HighlightedZone` instances, you get get access to the list of entities which are inside the zone, as well as enter and exit events fo these entities.  
The supported entities are : 
- `Player`
- `InteractableVehicle`
- `Zombie`
- `Animal`
- `StructureDrop`
- `BarricadeDrop`

You can also access the `Name` and `Group` on the `HighlightedZone` instance.  
On it is also a `Volume` property which contains the shape, position, size and orientation used to generate the zone.  

The events are `EventHandler<>`. The `sender` is the `HighlightedZone` instance, and the argument is the entity which triggered the event.  

Once you don't need to use the zone anymore, you should dispose it. When disposing a zone, it is not necessary to unsubscribe to it.

Using an event example
```csharp
public void Subscribe(HighlightedZone zone)
{
    zone.PlayerEntered += OnPlayerEntered;
}

private void OnPlayerEntered(object sender, Player player)
{
    HighlightedZone zone = (HighlightedZone)sender;

    /* Action on player */
}

public void KillZone(HighlightedZone zone)
{
    zone.Dispose();
}
```

### Zone methods

#### `Show`
At any time, you can show the zone effect to the players.  
The method for this is `Show`. You can either show the effect to all players, a single player or a list of players.  

When you show the zone, the plugin will take care of showing it back when a new player connects, or when a player reconnects.  

`Show` takes a `unique` parameter. When `unique` is set to `true`, the only zone having the same characteristics (Shape, Material and Color) as the calling zone will be displayed to the players. The other zones will be hidden.  

#### `Hide`
In the same way you can show zones to players, you can also hide them at any time. Either to all players, a single player or a list of players.  
When hiding a list of players, be sure to use the same object reference as the one you used to show the zone. The object reference is used to control to which player the zone should be shown when a player connects. 