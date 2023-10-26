<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 25cm 150cm; }
        body { margin: 0cm; }
    }
</style>

# **UHighlight** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub> - 0.3.0

This plugin allows you to create zones and have other plugins react to players entering the zone.  
Creating a zone is easy, and you can see the zone you're creating, as well as show them to the players when you want.

### **Required workshop** : https://steamcommunity.com/sharedfiles/filedetails/?id=3006499456

1. [**Commands**](#commands)
    1. [`/uhl {create | c} <shape> <material> <color>`](#uhl-create--c-shape-material-color)
    2. [`/uhl {validate | v} <group> <name>`](#uhl-validate--v-group-name)
    3. [`/uhl cancel`](#uhl-cancel)
    4. [`/uhl {show | s} <category> <name>`](#uhl-show--s-category-name)
    5. [`/uhl {categories | cat}`](#uhl-categories--cat)
    6. [`/uhl {volumes | vol} <category>`](#uhl-volumes--vol-category)
    7. [`/uhl delete <category> <name>`](#uhl-delete-category-name)
    8. [`/uhl test start <category> <name>`](#uhl-test-start-category-name)
    9. [`/uhl test stop`](#uhl-test-stop)
2. [**Creating a zone**](#creating-a-zone)
    1. [Cube edition strategy](#cube-edition-strategy)
    2. [Sphere edition strategy](#sphere-edition-strategy)
3. [**Dev guide**](#dev-guide)
    1. [Objects description](#objects-description)
        1. [**IHighlightCommands**](#ihighlightcommands)
        2. [**IHighlightSpawner**](#ihighlightspawner)
        3. [**HighlightedZone**](#highlightedzone)
        4. [**IServiceAdapter** and **ServiceAdapter**](#iserviceadapter-and-serviceadapter)
    2. [Code examples](#code-examples)
        1. [Getting the services](#getting-the-services)
        2. [Using IHighlightCommands](#using-ihighlightcommands)
        3. [Using IHighlightSpawner](#using-ihighlightspawner)
4. [**TODO**](#todo)


## **Commands**

### `/uhl {create | c} <shape> <material> <color>`
- **Description**: Start creating a new zone.
- **Parameters**: 
  - **`<shape>`** : Shape of the zone.  
    Possible values : `Cube`, `Sphere`

  - **`<material>`** : Material of the zone.  
    Possible values : `Solid`, `Transparent`


  - **`<color>`** : Color of the zone  
    Possible values : `Red`, `Green`, `Blue`, `Gold`, `Silver`, `Copper`, `Cyan`, `Lime`, `Magenta`, `Pink`

### `/uhl {validate | v} <group> <name>`
- **Description**: Confirm the creation of the zone. Must be done after `/uhl create` and once you set the zone.
- **Parameters**: 
  - **`<group>`** : Group of the zone.
  - **`<name>`** : Name of the zone.

### `/uhl cancel`
- **Description**: Cancels the zone creation.

### `/uhl {show | s} <category> <name>`
- **Description**: Shows you the zone.
- **Parameters**: 
  - **`<category>`** : Group of the zone to show.
  - **`<name>`** : Name of the zone to show.

### `/uhl {categories | cat}`
- **Description**: Lists all created groups.

### `/uhl {volumes | vol} <category>`
- **Description**: Lists all created the zones of a group.
- **Parameters**: 
  - **`<category>`** : Group to show the zone of.

### `/uhl delete <category> <name>`
- **Description**: Deletes a zone.
- **Parameters**: 
  - **`<category>`** : Group of the zone to show.
  - **`<name>`** : Name of the zone to show.

### `/uhl test start <category> <name>`
- **Description**: Spawns the zone for testing purpose. A message will be sent to the player in chat for each events.
- **Parameters**: 
  - **`<category>`** : Group of the zone to test.
  - **`<name>`** : Name of the zone to test.

### `/uhl test stop`
- **Description**: Despawn the tested zone.

## **Creating a zone**

### Cube edition strategy
When creating a cube zone, use the following keys to edit the shape
- `LeftPunch` : Set the first corner of the cube
- `RightPunch` : Set the opposite corner of the cube
- `,` : Rotate the cube by looking around with your player

### Sphere edition strategy
- `LeftPunch` : Set sphere center
- `RightPunch` : Set sphere radius
- `,` : Increase sphere radius by 1m
- `;` : Decrease sphere radius by 1m
- `:` : Increase sphere radius by 1%
- `$` : Decrease sphere radius by 1%

## **Dev guide**
> OpenMod and RocketMod both work. This guide is focus toward RocketMod. OpenMod will come later :)

### Objects description

#### **IHighlightCommands**

This interface can be used to execute the different UHighlight commands from your plugin.  

It is recommended to recreate all the commands from your plugin and use the `<category>` as an identifier for your plugin.  
For example, you would create the command `/myplugin show <name>` that would execute `/uhl show myplugin <name>` to show the zone created from your plugin.  

> Recreating the commands allows you to manage permissions yourself, as well as intercepting the command to create your own objects on your plugin.  
> It also makes the command shorter to write for the user

#### **IHighlightSpawner**

This interface can be used to spawn zones. You can either create a single zone, or all zones from a category at once.

#### **HighlightedZone**

This class represents a zone. It is returned from `IHighlightSpawner` when building zones.  
This class exposes 4 events :
- `PlayerEntered` : A player entered the zone.
- `PlayerExited` : A player exited the zone.
- `VehicleEntered` : A vehicle entered the zone.
- `VehicleExited` : A vehicle exited the zone.

These events use `PlayerArgs` or `VehicleArgs` parameters.  
In these parameters, you will find the entity (`Player` or `InteractableVehicle`) which triggered the event.  
You will also find the `Category` and `Name` of the entered zone.

On this object, you will also find a `Volume` property. It contains the different properties of the zone like center, size and orientation.

#### **IServiceAdapter** and **ServiceAdapter**

These objects will allow you to get a reference to both `IHighlightCommands` and `IHighlightSpawner` from the UHighlight plugin.  
(They were necessary to make the plugin work for both OpenMod and RocketMod, consistently, by sharing the same code base).  
You will need to get the services asynchronously. This is to ensure your plugin waits for UHighlight to load so that it can get the reference to the services.

### Code examples

#### Getting the services
(How to get a reference to `IHighlightSpawner` and `IHighlightCommands` implementations.)

```csharp
public class MyPlugin : RocketPlugin
{
    private IServiceAdapter _uhighlightServiceAdapter;
    private IHighlightSpawner _highlightSpawner;
    private IHighlightCommands _highlightCommands;

    protected override void Load()
    {
        _uhighlightServiceAdapter = new ServiceAdapter(UHighlightPlugin.Instance);

        Task.Run(async () => {
            _highlightSpawner = await serviceAdapter.GetServiceAsync<IHighlightSpawner>();
            _highlightCommands = await serviceAdapter.GetServiceAsync<IHighlightCommands>();
        });
    }

    protected override void Unload()
    {
        _uhighlightServiceAdapter.Dispose();
    }
}
```

#### Using IHighlightCommands

```csharp
    private const string MY_PLUGIN_CATEGORY = "myplugin";

    public void ShowZone(Player player, string zoneName)
    {
        _highlightCommands.ExecuteShow(player, MY_PLUGIN_CATEGORY, zoneName);
    }
```

#### Using IHighlightSpawner

```csharp
    private HighlightedZone? _zone;

    public void SpawnZone(string zoneName)
    {
        _zone = await _highlightSpawner.BuildZone(MY_PLUGIN_CATEGORY, zoneName);

        _zone.PlayerEntered += OnPlayerEntered;
    }

    private void OnPlayerEntered(object sender, PlayerArgs args)
    {
        Console.WriteLine($"{args.Player.name} just entered the zone {args.Name}");
    }

    public DespawnZone()
    {
        _zone?.Dispose();
    }
```

## **TODO**

- Error managing : Manage all edge cases. (Stoping a test before starting one is not managed yet :/)
- Add comments to interfaces
- Improve documentation (+add openmod vs. rocketmod +add the size configuration part)
- Add edit zone command
- Rename category to group
- Add new edition strategies and improve existing ones (Create cubes and spheres in other ways)
  - Per center cube
  - Fix sphere strategy (keys both increasing size)
  - Add Sizing keys to current cube strategy
- Add cylinder zone shape
- Add zone spawn strategies
  - Random spawn from category
  - First entered zone of category
  - All spawn of category
  - Sequence spawn of category
  - All vs. player specific
- Rework events parameters
- Allow zone duplication
- Allow asset zone creation (creating a zone on a house for example, so that all houses have the same zone. Configurable to choose which houses on the map needs a zone)
- Add a way to show the zones to the player...
- Suggestions ?