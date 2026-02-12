# CMPM147-Loot-Table-Generator

**Tool Explanation:**

This tool provides a Unity Loot Chest package that can be placed in a 3D    space any number of times. Upon opening, the chest will procedurally generate Loot Drops from a weighted, customizable loot table and display the outputs to a Unity UI Canvas. 

**How to Import in Project**

**How to Use**
Place the loot chest in the desired spot in the Unity scene. The chest prefab will automatically have default values attached, which can be changed as desired. Upon runtime, the loot chest contains a UI canvas that has a button to open the chest. Clicking the button will automatically generate the loot drops for the chest from the loot table and display the contents to a Unity UI screen. 


**Parameter Explanation**
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

**Example Outputs**
Example 1: <img width="915" height="637" alt="image" src="https://github.com/user-attachments/assets/a78c8e37-44be-40ba-a31b-589154a240eb" />

Example 2: <img width="702" height="633" alt="image" src="https://github.com/user-attachments/assets/79751c5d-8db8-44f0-8e4b-bc73d1b1c590" />

**Limitations:**
The Loot chest does not have functionality to take items from the chest. The chest stores the loot and displays it to the user. This functionality will need to be added by the user by accessing the chest's drops. 

The Loot chest does not accomodate for changes made to the format of the additional item categories in the .csv file. Any addition of new item stat categories will also need to be manually adjusted within insertCustomLootTable(). 


  
