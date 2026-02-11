// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   
using Unity.VisualScripting;
using UnityEditorInternal;
using System.Xml;
using System.Collections;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using System.Text.Json;
using UnityEditor;
public class LootChest : MonoBehaviour
{
    [SerializeField]
    public string LootTableFilePath;
    
    [SerializeField]  
    public int BatchGenerationCount = 1;
    [SerializeField]  
    public int lootCount = 1;
    [SerializeField]
    public bool allowRegeneration = false; public bool OutputToFile;
    [SerializeField]
    public string[] Rarities = new string[] {"Common","Uncommon","Rare","Epic","Legendary"};
    [SerializeField]
    public int[] Rarity_Weights = new int[] {55,30,15,6,1};
    [SerializeField]
    public Button GenerateBTN;
    [SerializeField]
    public TextMeshProUGUI Chest_Output_TMP;
    [SerializeField]
    public GameObject Chest_OutputUI;
    private bool alreadyGenerated;
    private LootChest myChest;

    void Start()
    {
        Button btn = GenerateBTN.GetComponent<Button>();
        myChest = new LootChest();
		btn.onClick.AddListener(Open);
        Chest_OutputUI.SetActive(false);
    }

    // call this function to open chest 
    public void Open(){

        // generates a new chest with loot if 1: chest has not been opened 2: chest allows for regeneration of loot upon open
        // else display existing chest's already generated loot 

        if(!alreadyGenerated || allowRegeneration)
        {
            Debug.Log("New chest created");
            myChest = LootChestGeneration(myChest);
            alreadyGenerated = true;
            GetComponent<Animation>().Play();
            displayOutput(myChest);
            
        }
        else
        {
            Debug.Log("Chest already Exists");
            displayOutput(myChest);
        }
        
	}

    
    // Update is called once per frame

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
    
   

    // Returns a new chest object with attached loot table paramaters attached. 
    public LootChest LootChestGeneration(LootChest chest)
    {
        List<List<gameItem>> batchDrops = new List<List<gameItem>>();
            chest.lootCount = lootCount;
            chest.lootTable = chest.insertCustomLootTable(LootTableFilePath); // change file path for custom loot tables
            chest.lootRarities = chest.insertCustomRarities(Rarities, Rarity_Weights); 
            for(int i = 0; i < BatchGenerationCount; i++)
            {
                chest.drops = chest.GenerateLoot(chest.lootRarities, chest.lootCount);
                if(OutputToFile)
                {
                    batchDrops.Add(chest.drops);
                }
               // inset some output to .csv file
            }
            if(OutputToFile)
            {
                //outputToJSON(batchDrops);
            }
        return chest;
            
    } 
    public void outputToJSON(List<List<gameItem>> batchDrops, string filePath)
    {
       
    }
        // Takes in a LootChest and displays it's generated loot to the Unity UI 
    public void displayOutput(LootChest currentChest)
    {
        string output = "";
        Chest_OutputUI.SetActive(true);
        foreach(var drop in currentChest.drops)
        {
            output += "1 x ("  + drop.rarity + ") " +  drop.name + "\n" ;
        }   
        Chest_Output_TMP.text = output; 
    }
    // This function allows for custom rarities that correspond to the loot table .csv file's rarities, as well as the corresponding weights of each rarity. 
    public Dictionary<string, int> insertCustomRarities(string[] rarities, int[]weights)
    {
     
        Dictionary<string, int> customRarities = new Dictionary<string, int>();
        if(rarities.Length != weights.Length)
        {
            Debug.LogError("ERROR Rarity Categories and Rarity Weights of unequal size"); // error caused by inspector rarities and rarity weights being mismatched
            return customRarities;
        }
        for(int i = 0; i < rarities.Length; i++)
        {
            customRarities.Add(rarities[i],weights[i]);
        }
        return customRarities;
    }

    //Takes in .csv file path and parses it into a custom loot table dictionary
    // outputs a dictionary storing the string of the rarity category, and a list of objects that exist within that rarity
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
    
    // This is the main function that generates the loot from the loot table. 
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

    // closes the Chest's UI output Window
    public void closeOutputWindow()
{
    Chest_OutputUI.SetActive(false);
}
}

