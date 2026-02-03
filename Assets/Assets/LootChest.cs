// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class LootChest : MonoBehaviour
{
    void Start()
    {
        LootChest.Main();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public class gameItem 
    {
        public string rarity;
        public string name;
        // public int quantity;
        public string type;
        public string description;
        public gameItem(string name, string rarity, string type, string description)
        {
            this.name = name;
            this.rarity = rarity;
            this.type = type;
            this.description = description;
        }
    }
    public List<gameItem> drops = new List<gameItem>(); // look into changing into a dictionary in order to group by quantity
    //LootRarities defines the rarity of the items dropped from the chest with the number corresponding 
    //to their frequency of being selected.
    Dictionary <string, int> lootRarities = new  Dictionary<string, int>();
    Dictionary <string, List<gameItem>> lootTable = new Dictionary<string, List<gameItem>>();
    
    public static void Main()
    {      
        LootChest chest = new LootChest();
        LootChest customChest = new LootChest();
        chest.lootTable = chest.insertCustomLootTable("Assets/Assets/Loot Table .csv Files/LootDrops.csv"); // change file path for custom loot tables
        chest.lootRarities = chest.insertCustomRarities("Assets/Assets/Loot Table .csv Files/Rarity_Weight.csv"); // change file path for custom rarities/weights

        // customChest.lootTable = customChest.insertCustomLootTable("Assets/Assets/Loot Table .csv Files/Custom_Loot_Rarities.csv");
        // customChest.lootRarities = customChest.insertCustomRarities("Assets/Assets/Loot Table .csv Files/Custom_Rarities_Weight.csv");
        
            System.Random rand = new System.Random();
            int randomOutput = rand.Next(0,10);
            chest.drops = chest.GenerateLoot(chest.lootRarities, randomOutput);
          
            Debug.Log("==============================");
            Debug.Log("Output: Dropping Loot Chest: with " + randomOutput + " rolls " );
            foreach(var drop in chest.drops)
            {

                Debug.Log("- 1 " + drop.name + " (" + drop.rarity + "): " + drop.type);

            }   
            // System.Random rand = new System.Random();
            // int randomOutput = rand.Next(0,10);
            // customChest.drops = customChest.GenerateLoot(customChest.lootRarities, randomOutput);
            // Debug.Log("==============================");
            // Debug.Log("Output " + i + ": Dropping Custom Loot Chest: with " + randomOutput + " rolls " );
            // foreach(var customDrop in customChest.drops)
            // {

            //     Debug.Log("- 1 " + customDrop.name + " (" + customDrop.rarity + "): " + customDrop.type);

            // }
        
    }
    public Dictionary<string, int> insertCustomRarities(string filePath)
    {
        string path = filePath;
        StreamReader reader;
        Dictionary<string, int> customRarities = new Dictionary<string, int>();
        if(File.Exists(path))
        {
            reader = new StreamReader(File.OpenRead(path));
            if(!reader.EndOfStream)
            {
                reader.ReadLine(); // skip header line
            }
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                string rarityName = values[0];
                int rarityWeight = Int32.Parse(values[1]);
                customRarities.Add(rarityName, rarityWeight);
            }
            reader.Close();
            return customRarities;
        }
        else
        {
            Debug.Log("Error: File not found.");
            return customRarities;
        }
        
    }

    //Takes in .csv file path and parses it into a custom loot table dictionary
    public Dictionary<string, List<gameItem>> insertCustomLootTable(string filePath)
    {
        string path = filePath;
        StreamReader reader;
        Dictionary<string, List<gameItem>> customLootTable = new Dictionary<string, List<gameItem>>();
        if(File.Exists(path))
        {
            reader = new StreamReader(File.OpenRead(path));
            if(!reader.EndOfStream)
            {
                reader.ReadLine(); // skip header line
            }
            while(!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                string itemRarity = values[0];
                string itemName = values[1];
                string itemType = values[2];
                string itemDescription = values[3];
                gameItem newItem = new gameItem (itemName, itemRarity, itemType, itemDescription);

                // if item rarity key already exists, add newItem to the list, else create new rarity key with a new list of items. 
                if(customLootTable.ContainsKey(itemRarity))
                {
                    customLootTable[itemRarity].Add(newItem);
                }
                else
                {
                    customLootTable.Add(itemRarity, new List<gameItem> {newItem});
                }
                 
            }
            reader.Close();
            return customLootTable;
        }
        else
        {
            Debug.Log("Error: File not found.");
            return customLootTable;
        }
    }
    
    // GenerateLoot takes in an integer lootCount which defines how many items to generate from the loot table, and a Dictionary <string, int> for custom rarities. 
    public List<gameItem> GenerateLoot(Dictionary<string, int> myRarity, int lootCount = 1 ) //
    {
        Dictionary <string, int> selectedRarity = new Dictionary<string, int>();
        List<gameItem> selectedItems = new List<gameItem>();
        System.Random rand = new System.Random();
        int weightedSum = 0;

        // Initialize selectedRarity dictionary and calculate weighted sum of rarities
        foreach(var rarity in myRarity)
        {
            selectedRarity.Add(rarity.Key, 0);
            weightedSum += rarity.Value;
        }
        //check if weight sum is 0 
        if(weightedSum <= 0) 
        {
            Debug.Log("Error: No loot available.");
            return selectedItems;
        }

        for(int i = 0; i < lootCount; i++) // number of items to gererate
        {
            int roll = rand.Next(0,weightedSum); // roll random between 0 and sum of weights

            //output += "initial roll: " + roll + " ";  // generate a random number beftween 0 and sum. Take that number and see where it falls in the weighted loot table. 
            //take random generated number and subtract it from the next higest rarity. ex: 70 - 60(common) = 10. Then check if the remaining number is > next rarity. 
            // 10 >= 10 so roll of 70 = uncommon. If the number is less than the next rarity, then that is the selected rarity.
            foreach(var rarity in myRarity)
            {
                roll -= rarity.Value;
                if(roll < 0)
                {
                    /*
                    When a rarity is selected add it to the output dictionary. If already existing increment it's counter. 
                    This allows for the multiple items of the same rarity to be displayed in a grouped fashion. Rather than 
                    listed out individually based on the order they were selcted.  

                    Ex: 2 Common, 1 Rare, 1 Epic rather than Common, Rare, Common, Epic
                    
                    */
                    if(selectedRarity.ContainsKey(rarity.Key))
                    {
                        selectedRarity[rarity.Key] += 1;
                    }
                    else
                    {
                        selectedRarity.Add(rarity.Key,1);
                    }
                    break;
                }
            }
        }
        // With the selected rarities, randomly select items from the loot table based on the number of items per rarity. 
        foreach(var r in selectedRarity)
        {
            for(int i = 0; i < r.Value; i++)
            {
                var itemList = lootTable[r.Key]; 
                gameItem selectedItem = itemList[rand.Next(0,itemList.Count)];
                selectedItems.Add(selectedItem);
            }
        }   
        return selectedItems;
    }
}