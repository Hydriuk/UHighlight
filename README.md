<link rel="stylesheet" href="C:\Users\Antonin\Documents\_workspace\Unturned\Projects\vscode-hydriuk.css"></link>
<style>
    @media print {
        @page { margin: 0; size: 25cm 105cm; }
        body { margin: 0cm; }
    }
</style>

# **UHighlight** <sub>*by [Hydriuk](https://github.com/Hydriuk)*</sub> - 0.9.0

This plugin allows you to create zones and have other plugins react to players entering the zone.  
Creating a zone is easy, and you can see the zone you're creating, as well as show them to the players when you want.

## **Required workshop** : https://steamcommunity.com/sharedfiles/filedetails/?id=3006499456

## Required libraries

- LiteDB
- 0Harmony
- Hydriuk.OpenModModules or Hydriuk.RocketModModules

## **Commands**

### `/uhl`
- **Description**: Opens the zone creation UI.

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

## **Using the UI**

The UI is separated in 3 sections :
- Groups
- Zones
- Properties

At the top of the UI is the close button, a "Hide all aones" button and a "Lock cursor" button. The lock cursor can be used to toggle between keeping the cursor on screen and hiding it. (F.Y.I, to show your cursor, you can use one of the keys on your keyboard, configured in your ingame commands options, under the plugin section, named "Release Cursor")  


### Groups

This is the paginated list of groups configured in the plugin.

In the groups sections you have 4 different types of buttons : 
- Create button : You can create a group by entering a group name and clicking this button
- Delete button : To delete the group
- Display zones button : To display all zones of the group (to you only)
- Show group information : To select the group, and display all its zones in the second section.

### Zones

This is the paginated list of zones in the selected group.

In this section you can :
- Use the "Properties" button to show the 3rd section with properties of the selected group
- Delete a zone
- Show a zone
- Create a zone

To create a zone, you must select a shape, material and color. You will also need to first click the "Create" button to start the creation of a new zone.  
You will then be able to place the zone in the world.  
Once you are happy with your zone, you will have to enter a name for the zone, and click "Validate"

### Properties

In this section you will find the list of properties assigned to the group.

You can add as many properties as you want. Some properties will need some data value to work.  
Here are the existing properties and their required data : 
- `PlaceStructure`: Prevent placing structures. Data: None
- `StructureDamage`: Change the amount of damage the structures take. Data (floating number): damage mutliplier
- `PlayerDamage`: Change the amount of damage the players take. Data (floating number): damage mutliplier
- `VehicleDamage`: Change the amount of damage the vehicles take. Data (floating number): damage mutliplier
- `ZombieDamage`: Change the amount of damage the zombies take. Data (floating number): damage mutliplier
- `AnimalDamage`:  Change the amount of damage the animals take. Data (floating number): damage mutliplier
- `GivePermissionGroup`*: Give a permission group to the player. Data (string): group to give
- `RemovePermissionGroup`*: Remove a permission group from the player. Data (string): group to remove
- `Chat`*: Sends a chat message to the player. Data (string): Message to send
- `Command`*: Executes a command. Data (string): Command to execute
- `Repulse`*: Applies a repulsing force to the player. Data (floating number): Force. *This property doesn't fully work. It won't work in every situation*

With properties marked by *, you will have to decide if the property should be activated when the player enters the zone, or when he exits the zone. You will have two new buttons for this when you select one of these properties.

For `GivePermissionGroup`, `RemovePermissionGroup`, `Chat` and `Command`, you can write texts that will be replaced by the plugin:
- `{Player}`: Will be replaced by the character name of the player who triggered the event
- `{PlayerID}`: Will be replaced by the player id of the player who triggered the event
- `{ZoneName}`: Will be replaced by the zone name of the zone from which the event was triggered

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