<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 25cm 70cm; }
        body { margin: 0cm; }
    }
</style>

# **UHighlight** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub> - 0.8.0

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

To get reference to a `IHighlightCommands` instance, you will need to use `ServiceAdapter`.  
When getting the `IHighlightCommands` service from the `ServiceAdapter`, you should wait for both the UHighlight plugin and the level to load. Using the `GetServiceAsync` method will take care of that for you, so you can just await it. Don't forget to dispose `ServiceAdapter` when you don't need it anymore.  

Example : 
```csharp
public class AmbianceZoneCommand : IRocketCommand
{
    /* ... */

    public async void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer uPlayer = (UnturnedPlayer)caller;

        IServiceAdapter serviceAdapter = new ServiceAdapter();
        IHighlightCommands highlightCommands = await serviceAdapter.GetServiceAsync<IHighlightCommands>();
        serviceAdapter.Dispose();

        string zoneName = command[0];

        await highlightCommands.ExecuteValidate(uPlayer.Player, Constants.GROUP_NAME, zoneName);

        /* Your own registration */
    }
}
```

## How to spawn zones

At any time, you can spawn existing zones, or groups of zones. For this, you will use the `IHighlightSpawner` interface.  
The methods of IHighlightSpawner will return `HighlightedZone` instances.  

To get reference to a `IHighlightSpawner` instance, you will need to use `ServiceAdapter`.  
When getting the `IHighlightSpawner` service from the `ServiceAdapter`, you should wait for both the UHighlight plugin and the level to load. Using the `GetServiceAsync` method will take care of that for you, so you can just await it. Don't forget to dispose `ServiceAdapter` when you don't need it anymore.  

Your plugin is responsible for the lifetime of the zones it spawns. When you spawn zones, you must keep a references to them, to later dipose them when not needed anymore.

Spawning a group of zone example : 
```csharp
public class Spawner : IDisposable
{
    private readonly List<HighlightedZone> _zones = new List<HighlightedZone>();

    public async Task SpawnZones(string groupName)
    {
        IServiceAdapter serviceAdapter = new ServiceAdapter();
        IHighlightSpawner highlightSpawner = await serviceAdapter.GetServiceAsync<IHighlightSpawner>();
        serviceAdapter.Dispose();

        IEnumerable<HighlightedZone> zones = highlightSpawner.BuildZones(groupName);

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

## How to use zones

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