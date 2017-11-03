#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Charlie.Giphy
{
    [CustomEditor(typeof(GiphyService))]
    class GiphyServiceInspector : Editor
    {
        private GiphyService giphyService;

        private static bool showTestConsole = true;
        private static bool showTestConsoleSearch = true;
        private static bool showTestConsoleTrending = true;
        private static bool showTestConsoleRandom = true;
        private static bool showTestConsoleGetByID = true;
        private static bool showTestConsoleListPacks = true;
        private static bool showTestConsoleStickersInPack = true;

        private static string testConsoleSearchQuery = "Cat";
        private static int testConsoleSearchLimit = 25;
        private static int testConsoleSearchOffset = 0;
        private static string testConsoleSearchRating = "G";
        private static string testConsoleSearchLang = "en";

        private static int testConsoleTrendingLimit = 25;
        private static string testConsoleTrendingRating = "G";

        private static string testConsoleRandomTag = "Cat";
        private static string testConsoleRandomRating = "G";

        private static string testConsoleGetByIDID = "";

        private static int testConsoleStickersInPackID;
        private static int testConsoleStickersInPackLimit = 25;
        private static int testConsoleStickersInPackOffset = 0;

        private static bool searching = false;
        private static bool searchingTrending = false;
        private static bool gettingRandom = false;
        private static bool gettingByID = false;
        private static bool listingPacks = false;
        private static bool listingStickersInPack = false;

        public void OnEnable()
        {
            giphyService = (GiphyService)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (Application.isPlaying)
            {
                showTestConsole = EditorGUILayout.Foldout(showTestConsole, new GUIContent("Test Console"));
                if (showTestConsole)
                {
                    EditorGUI.indentLevel++;

                    showTestConsoleSearch = EditorGUILayout.Foldout(showTestConsoleSearch, new GUIContent("Search"));
                    if (showTestConsoleSearch)
                    {
                        EditorGUI.indentLevel++;
                        testConsoleSearchQuery = EditorGUILayout.TextField(new GUIContent("Query", "Search query term or phrase"), testConsoleSearchQuery);
                        testConsoleSearchLimit = EditorGUILayout.IntField(new GUIContent("Limit", "The maximum number of records to return"), testConsoleSearchLimit);
                        testConsoleSearchOffset = EditorGUILayout.IntField(new GUIContent("Offset", "An optional results offset"), testConsoleSearchOffset);
                        testConsoleSearchRating = EditorGUILayout.TextField(new GUIContent("Rating", "Filters results by rating"), testConsoleSearchRating);
                        testConsoleSearchLang = EditorGUILayout.TextField(new GUIContent("Language", "Specify default country for regional content"), testConsoleSearchLang);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !searching;
                        bool searchClicked = GUILayout.Button(new GUIContent(searching ? "Searching" : "Search"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (searchClicked)
                        {
                            HandleSearchClicked();
                        }

                        EditorGUI.indentLevel--;
                    }

                    showTestConsoleTrending = EditorGUILayout.Foldout(showTestConsoleTrending, new GUIContent("Trending"));
                    if (showTestConsoleTrending)
                    {
                        EditorGUI.indentLevel++;
                        testConsoleTrendingLimit = EditorGUILayout.IntField(new GUIContent("Limit", "The maximum number of records to return"), testConsoleTrendingLimit);
                        testConsoleTrendingRating = EditorGUILayout.TextField(new GUIContent("Rating", "Filters results by rating"), testConsoleTrendingRating);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !searchingTrending;
                        bool searchTrendingClicked = GUILayout.Button(new GUIContent(searchingTrending ? "Searching" : "Search"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (searchTrendingClicked)
                        {
                            HandleSearchTrendingClicked();
                        }

                        EditorGUI.indentLevel--;
                    }

                    showTestConsoleRandom = EditorGUILayout.Foldout(showTestConsoleRandom, new GUIContent("Random"));
                    if (showTestConsoleRandom)
                    {
                        EditorGUI.indentLevel++;
                        testConsoleRandomTag = EditorGUILayout.TextField(new GUIContent("Tag", "Filters results by specified tag"), testConsoleRandomTag);
                        testConsoleRandomRating = EditorGUILayout.TextField(new GUIContent("Rating", "Filters results by rating"), testConsoleRandomRating);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !gettingRandom;
                        bool getRandomClicked = GUILayout.Button(new GUIContent(gettingRandom ? "Getting" : "Get"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (getRandomClicked)
                        {
                            HandleGetRandomClicked();
                        }

                        EditorGUI.indentLevel--;
                    }

                    showTestConsoleGetByID = EditorGUILayout.Foldout(showTestConsoleGetByID, new GUIContent("By ID"));
                    if (showTestConsoleGetByID)
                    {
                        EditorGUI.indentLevel++;
                        testConsoleGetByIDID = EditorGUILayout.TextField(new GUIContent("ID", "Filters results by specified GIF ID"), testConsoleGetByIDID);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !gettingByID;
                        bool getByIDClicked = GUILayout.Button(new GUIContent(gettingByID ? "Getting" : "Get"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (getByIDClicked)
                        {
                            HandleGetByIDClicked();
                        }

                        EditorGUI.indentLevel--;
                    }

                    showTestConsoleListPacks = EditorGUILayout.Foldout(showTestConsoleListPacks, new GUIContent("List Packs"));
                    if (showTestConsoleListPacks)
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !listingPacks;
                        bool listPacksClicked = GUILayout.Button(new GUIContent(listingPacks ? "Listing" : "List"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (listPacksClicked)
                        {
                            HandleListPacksClicked();
                        }

                        EditorGUI.indentLevel--;
                    }

                    showTestConsoleStickersInPack = EditorGUILayout.Foldout(showTestConsoleStickersInPack, new GUIContent("Stickers In Pack"));
                    if (showTestConsoleStickersInPack)
                    {
                        EditorGUI.indentLevel++;
                        testConsoleStickersInPackID = EditorGUILayout.IntField(new GUIContent("Pack ID", "Filters results by specified Sticker Pack ID"), testConsoleStickersInPackID);
                        testConsoleStickersInPackLimit = EditorGUILayout.IntField(new GUIContent("Limit", "The maximum number of records to return"), testConsoleStickersInPackLimit);
                        testConsoleStickersInPackOffset = EditorGUILayout.IntField(new GUIContent("Offset", "An optional results offset"), testConsoleStickersInPackOffset);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !listingStickersInPack;
                        bool listStickersInPackClicked = GUILayout.Button(new GUIContent(listingStickersInPack? "Listing" : "List"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (listStickersInPackClicked)
                        {
                            HandleListStickersInPackClicked();
                        }

                        EditorGUI.indentLevel--;
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("A test console will be available here when you enter play mode.",
                    MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        async private void HandleSearchClicked()
        {
            searching = true;
            Repaint();
            await giphyService.Search(testConsoleSearchQuery);
            searching = false;
            Repaint();
        }

        async private void HandleSearchTrendingClicked()
        {
            searchingTrending = true;
            Repaint();
            await giphyService.Trending(testConsoleTrendingLimit, testConsoleTrendingRating);
            searchingTrending = false;
            Repaint();
        }

        async private void HandleGetRandomClicked()
        {
            gettingRandom = true;
            Repaint();
            await giphyService.Random(testConsoleRandomTag, testConsoleRandomRating);
            gettingRandom = false;
            Repaint();
        }

        async private void HandleGetByIDClicked()
        {
            gettingByID = true;
            Repaint();
            await giphyService.GetByID(testConsoleGetByIDID);
            gettingByID = false;
            Repaint();
        }

        async private void HandleListPacksClicked()
        {
            listingPacks = true;
            Repaint();
            await giphyService.ListPacks();
            listingPacks = false;
            Repaint();
        }

        async private void HandleListStickersInPackClicked()
        {
            listingStickersInPack = true;
            Repaint();
            await giphyService.StickersInPack(testConsoleStickersInPackID,
                testConsoleStickersInPackLimit, testConsoleStickersInPackOffset);
            listingStickersInPack = false;
            Repaint();
        }
    }
}
#endif
