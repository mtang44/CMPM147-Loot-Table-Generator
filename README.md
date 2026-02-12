# CMPM147-Loot-Table-Generator

## **Tool Explanation:**

This tool provides a Unity Loot Chest package that can be placed in a 3D    space any number of times. Upon opening, the chest will procedurally generate Loot Drops from a weighted, customizable loot table and display the outputs to a Unity UI Canvas. 

## **How to Import in Project**

## **How to Use**

Place the loot chest in the desired spot in the Unity scene. The chest prefab will automatically have default values attached, which can be changed as desired. A Unity UI button has been provided to showcase how to generate loot. However, this button is only for testing the generation of loot. To open a chest, call the current chest's Open() function. This function will begin the generation of the chest's loot, automatically opening up a UI output screen with the loot. To close the chest's Output UI screen, call the closeOutputWindow() function. 

### Example of a call to open a chest: 
```

 if (Input.GetKeyDown(KeyCode.E))
{
    if(!alreadyInChest){
        currentChest.Open();
        alreadyInChest = true;
    }else{
        alreadyInChest = false;
        CurrentChest.closeOutputWindow();
        }
}
```

### How to Import Custom Loot Tables:
To import a custom loot table, open the provided Loot Table .csv file. In the .csv file, you will find a spreadsheet which allows you to fill out customloot attributes. Loot Rarities, Item Names, Item Type, and Item Description can all be filled in just using the spreadsheet. The .csv file should already be attached to the loot chest, but in the event it is not, copy the path of the file from the Unity folder and paste that into the "Loot Table File Path" input field in the Chest Inspector

#### <img width="902" height="225" alt="image" src="https://github.com/user-attachments/assets/b4436ea0-2277-46bf-b67a-f19d018dc69e" />

If you are using custom loot rarities other than the default rarities: "Common, Uncommon, Rare...etc.", make sure to also change the names of the Rarities in the inspector field to match any custom rarities assigned in the loot table csv file. It is important to note that each Rarity in the Rarities List directly corresponds to the Rarity Weight List. (Rarity's Element 0 is attached to Rarity_Weight's Element 0). 


<img width="908" height="517" alt="image" src="https://github.com/user-attachments/assets/53f2ae56-c1ee-4555-937a-bbc557411c6d" />

It is also important to note that the weights do not have to add up to 100% or even 100. The weights use a ratio-based weighing system where the higher the number, the higher the chances of it spawning. When changing the rarity weights, make sure that they are listed in descending order from highest to lowest. Otherwise, this will result in altered loot drop rates. 

```
Using the image above: 55 + 30 + 15 + 6 + 1 = 107.

Common = 55/107 = .51 or a ~51% spawn chance
Uncommon = 30/107 = .28 or ~28$ spawn chance
Rare = 15/107 = .14 or ~14% spawn chance
Epic = 6/107 = .05 or ~5% spawn chance
Legendary = 1/107 = .009 or ~ <1% spawn chance

```
This ensures that inserting a new weight doesn't force all other weights to have to be changed aswell. 

## **Parameter Explanation**
This tool incorporates a variety of adjustable parameters that can be edited through the Chest's Inspector tab in Unity. 

1: Custom Loot Table .csv Pathway
2: The number of batch Generations 
3: The number of items the chest will generate
4: A bool value for whether the chest is allowed to regenerate its loot 
contents
5: A bool value for whether the chest should output its loot to a file 
6: A list of Raraties, corresponding to the values stored in the loot table csv
7: The weight of the rarities 

<img width="698" height="732" alt="image" src="https://github.com/user-attachments/assets/09a73300-e028-434d-ba9b-d1a5e4380300" />

This tool allows the user to import a custom .csv loot table file that can be changed as needed. The list of items, the name of the rarities, and 

##**Example Outputs**
<img width="915" height="637" alt="image" src="https://github.com/user-attachments/assets/a78c8e37-44be-40ba-a31b-589154a240eb" />

<img width="702" height="633" alt="image" src="https://github.com/user-attachments/assets/79751c5d-8db8-44f0-8e4b-bc73d1b1c590" />

## **Limitations:**

If multiple chest assets are placed around a map, all chests will open and generate loot simultaneously. It is up to the user to figure out how to access the chest the player wants to use. To open a chest call the chest's Open() function. 

The Loot chest does not have functionality to take items from the chest. The chest stores the loot and displays it to the user. This functionality will need to be added by the user by accessing the chest's drops. 

The Loot chest does not accomodate for changes made to the format of the additional item categories in the .csv file. Any addition of new item stat categories will also need to be manually adjusted within insertCustomLootTable(). 


  
