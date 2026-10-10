#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Put this file in Assets/Editor.
// Menu: MixKitchen > Fill Dish Trivia
// Writes the fun fact (and the origin, if it is empty) into every dish of every AreaData asset,
// matching by dish name. It does NOT touch sprites, choices or steps, so it is safe to run on a
// Luzon.asset / Visayas.asset you already edited. The area builders call it automatically.
public static class DishTriviaFiller
{
    struct Info
    {
        public string origin;
        public string trivia;
        public Info(string o, string t) { origin = o; trivia = t; }
    }

    static readonly Dictionary<string, Info> Data = new Dictionary<string, Info>
    {
        // ---------- LUZON ----------
        { "Longganisang Lucban", new Info("Quezon - CALABARZON (Region IV-A)",
            "Lucban's longganisa is garlicky and tangy from vinegar, not sweet like many other sausages. Lucban is also home to the colorful Pahiyas Festival every May.") },
        { "Bulalo", new Info("Batangas - CALABARZON (Region IV-A)",
            "Bulalo is named after the bone marrow in the beef shank, the prize of the dish. It is a favorite in the cool highlands of Batangas and nearby Tagaytay.") },
        { "Buko Pie", new Info("Laguna - CALABARZON (Region IV-A)",
            "Buko means young coconut. This pie is a famous pasalubong from Laguna, especially Los Banos, filled with tender coconut strips in a creamy custard.") },
        { "Pastillas", new Info("Bulacan - Central Luzon (Region III)",
            "Pastillas de leche is traditionally made from carabao's milk. Bulacan sweet makers wrap each candy in paper cut into delicate lace-like patterns called pabalat.") },
        { "Sinaing na Tulingan", new Info("Batangas - CALABARZON (Region IV-A)",
            "Sinaing means slowly cooked. Tulingan is simmered for hours in a clay pot with kamias and salt until even the fish bones turn soft enough to eat.") },

        // ---------- VISAYAS ----------
        { "Chicken Inasal", new Info("Bacolod, Negros Occidental - Western Visayas (Region VI)",
            "Inasal comes from the Hiligaynon word for grilled. Bacolod's version is basted with annatto oil, which gives the chicken its orange color, and is eaten with rice.") },
        { "Binignit", new Info("Cebu - Central Visayas (Region VII)",
            "Binignit is a warm coconut milk stew of root crops, banana and sticky rice. In many Visayan homes it is made during Holy Week, when people avoid meat.") },
        { "Piaya", new Info("Negros Occidental - Western Visayas (Region VI)",
            "Piaya is a flat, round bread filled with muscovado, the dark sugar of Negros. The island is known for its sugar farms, and piaya is a favorite pasalubong from Bacolod.") },
        { "Puto Maya", new Info("Cebu - Central Visayas (Region VII)",
            "Puto maya is sticky rice cooked in coconut milk with ginger. In Cebu it is a classic breakfast, eaten with sikwate, a thick hot chocolate made from tablea.") },
        { "Paksiw na Isda", new Info("Cebu - Central Visayas (Region VII)",
            "Paksiw means cooked in vinegar. The vinegar helps the dish keep longer, so paksiw na isda is often reheated and tastes even better the next day.") },

        // ---------- MINDANAO ----------
        { "Grilled Tuna", new Info("General Santos City, South Cotabato - SOCCSKSARGEN (Region XII)",
            "General Santos City is known as the Tuna Capital of the Philippines. Its fishing port lands tuna every day, and its Tuna Festival is held every September.") },
        { "Kinilaw na Isda", new Info("Davao City - Davao Region (Region XI)",
            "Kinilaw is raw fish 'cooked' by the acid of vinegar and citrus instead of heat. Dishes like it were eaten in the islands long before the Spanish arrived.") },
        { "Tuna Kinilaw", new Info("Davao City - Davao Region (Region XI)",
            "Tuna kinilaw uses very fresh tuna with vinegar, onion, ginger and chili. Because the fish is eaten raw, it is best made soon after the tuna is caught.") },
        { "Satti", new Info("Zamboanga City - Zamboanga Peninsula (Region IX)",
            "Satti are grilled skewers served with a spicy sauce and puso, rice cooked in a woven coconut-leaf pouch. In Zamboanga City it is a popular breakfast.") },
        { "Ginataang Isda", new Info("Davao City - Davao Region (Region XI)",
            "Ginataan means cooked in coconut milk, or gata. The creamy sauce softens the heat of the chili and brings out the sweetness of the fish.") },
    };

    [MenuItem("MixKitchen/Fill Dish Trivia")]
    public static void FillAll()
    {
        int dishesFilled = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:AreaData"))
        {
            AreaData area = AssetDatabase.LoadAssetAtPath<AreaData>(AssetDatabase.GUIDToAssetPath(guid));
            if (area == null) continue;
            dishesFilled += Apply(area);
            EditorUtility.SetDirty(area);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Dish trivia written to " + dishesFilled + " dishes.");
    }

    // Returns how many dishes were filled. Called by the area builders too.
    public static int Apply(AreaData area)
    {
        int count = 0;
        foreach (Dish dish in area.dishes)
        {
            Info info;
            if (!Data.TryGetValue(dish.dishName, out info))
            {
                Debug.LogWarning("No trivia written for dish '" + dish.dishName + "'.");
                continue;
            }
            dish.trivia = info.trivia;
            if (string.IsNullOrEmpty(dish.origin)) dish.origin = info.origin;
            count++;
        }
        return count;
    }
}
#endif