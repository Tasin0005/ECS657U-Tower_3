using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Makes the map using basic cubes and spheres
public static class TowerDefenceSceneBuilder
{
    // Add menu option to generate map, for testing
    [MenuItem("Tools/Tower Defence/Generate Prototype Map")]

    // Builds and saves a complete prototype tower defence map in the active Unity project
    public static void GenerateMap()
    {
        bool confirmed = EditorUtility.DisplayDialog(
            "Generate map?",
            "This will replace the open scene. Save anything important first.",
            "Generate", "Cancel");

        if (confirmed == false)
            return;

        MakeFolder("Scenes");
        MakeFolder("Materials");

        // Create materials with MakeMaterial(), written below 
        Material gridBase = MakeMaterial("GridBase", new Color(0.22f, 0.34f, 0.25f));
        Material grass = MakeMaterial("Grass", new Color(0.43f, 0.67f, 0.37f));
        Material path = MakeMaterial("Path", new Color(0.81f, 0.62f, 0.39f));
        Material stone = MakeMaterial("Stone", new Color(0.48f, 0.53f, 0.60f));
        Material spawn = MakeMaterial("Spawn", new Color(0.53f, 0.23f, 0.71f));
        Material pad = MakeMaterial("PlacementPad", new Color(0.11f, 0.69f, 0.86f));
        Material gold = MakeMaterial("Crystal", new Color(1f, 0.75f, 0.20f));

        // Create a new empty scene to hold the map
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Creates parent object for the map, and child objects for grass and path cubes
        GameObject map = new GameObject("Map");
        MakeCube("GroundFoundation", new Vector3(0, -0.23f, 0), new Vector3(40, 0.2f, 40), gridBase, map.transform);
        GameObject grassGroup = new GameObject("GrassCubes");
        grassGroup.transform.parent = map.transform;
        GameObject pathGroup = new GameObject("PathCubes");
        pathGroup.transform.parent = map.transform;

        // Generate 40x40 grid of grass cubes
        for (int row = 0; row < 40; row++)
        {
            for (int col = 0; col < 40; col++)
            {
                Vector3 position = TilePosition(col, row, -0.1f);
                MakeCube("Grass_" + col + "_" + row, position,
                    new Vector3(0.92f, 0.18f, 0.92f), grass, grassGroup.transform);
            }
        }

        // Pair of row and column coordinates for the two pathways, in order from spawn to fortress
        int[,] leftCorners = {
            {0, 13}, {6, 13}, {6, 4}, {16, 4},
            {16, 12}, {11, 12}, {11, 19}, {17, 19}
        };
        int[,] rightCorners = {
            {39, 26}, {33, 26}, {33, 35}, {23, 35},
            {23, 27}, {28, 27}, {28, 20}, {22, 20}
        };

        MakeRoad(leftCorners, path, pathGroup.transform);
        MakeRoad(rightCorners, path, pathGroup.transform);

        // Create fortress object
        GameObject fortress = new GameObject("Fortress");
        fortress.transform.parent = map.transform;
        MakeCube("FortressBase", new Vector3(0, 1.25f, 0),
            new Vector3(4, 2.5f, 4), stone, fortress.transform);

        // Create four towers at the corners of the fortress
        for (int x = -1; x <= 1; x = x + 2)
        {
            for (int z = -1; z <= 1; z = z + 2)
            {
                MakeCube("Tower", new Vector3(x * 1.6f, 2.2f, z * 1.6f),
                    new Vector3(0.8f, 4.4f, 0.8f), stone, fortress.transform);
            }
        }

        // Create a golden crystal on top of the fortress
        GameObject crystal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crystal.name = "GoldenCrystal";
        crystal.transform.parent = fortress.transform;
        crystal.transform.position = new Vector3(0, 3.15f, 0);
        crystal.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        crystal.GetComponent<Renderer>().sharedMaterial = gold;

        // Create markers for spawn points and wizard placement pads
        GameObject markers = new GameObject("Markers");
        markers.transform.parent = map.transform;
        GameObject spawnGroup = new GameObject("SpawnPoints");
        spawnGroup.transform.parent = markers.transform;
        MakeCube("SpawnA", TilePosition(0, 13, 0.11f),
            new Vector3(1, 0.18f, 1), spawn, spawnGroup.transform);
        MakeCube("SpawnB", TilePosition(39, 26, 0.11f),
            new Vector3(1, 0.18f, 1), spawn, spawnGroup.transform);

        GameObject padGroup = new GameObject("WizardPlacementPads");
        padGroup.transform.parent = markers.transform;
        int[,] padPositions = {
            {3, 11}, {9, 7}, {13, 15},
            {36, 28}, {30, 32}, {26, 24}
        };
        for (int i = 0; i < padPositions.GetLength(0); i++)
        {
            int col = padPositions[i, 0];
            int row = padPositions[i, 1];
            MakeCube("Pad_" + (i + 1), TilePosition(col, row, 0.12f),
                new Vector3(1, 0.18f, 1), pad, padGroup.transform);
        }

        // Creates route objects
        GameObject routes = new GameObject("Routes");
        routes.transform.parent = map.transform;
        Transform leftRoute = MakeWaypoints("LeftRoute", leftCorners, routes.transform);
        Transform rightRoute = MakeWaypoints("RightRoute", rightCorners, routes.transform);

        // Creates a fixed birds-eye camera
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        camera.orthographic = true;
        camera.orthographicSize = 23.5f;
        camera.rect = new Rect(0, 0, 0.78f, 1); // Right side is for the UI
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.1f, 0.15f, 0.19f);
        cameraObject.transform.position = new Vector3(0, 50, 0);
        cameraObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Creates directional light for the scene
        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(45, -35, 0);

        // Saves the scene and display a message to the user
        EditorSceneManager.SaveScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene(),
            "Assets/Scenes/Prototype.unity");
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Done", "Open Prototype.unity and press Play.", "OK");
    }

    // Converts a grid square into a Unity world position
    private static Vector3 TilePosition(int col, int row, float height)
    {
        return new Vector3(col - 19.5f, height, 19.5f - row);
    }

    // Marks the squares along a route and creates a cube for each road square
    private static void MakeRoad(int[,] corners, Material material, Transform parent)
    {
        bool[,] roadSquares = new bool[40, 40];

        for (int i = 0; i < corners.GetLength(0) - 1; i++)
        {
            int startX = corners[i, 0];
            int startY = corners[i, 1];
            int endX = corners[i + 1, 0];
            int endY = corners[i + 1, 1];

            int moveX = 0;
            int moveY = 0;
            if (endX > startX) moveX = 1;
            if (endX < startX) moveX = -1;
            if (endY > startY) moveY = 1;
            if (endY < startY) moveY = -1;

            // Use the larger axis difference so horizontal and vertical segments are covered fully
            int steps = Mathf.Max(Mathf.Abs(endX - startX), Mathf.Abs(endY - startY));
            for (int step = 0; step <= steps; step++)
            {
                int col = startX + moveX * step;
                int row = startY + moveY * step;
                roadSquares[col, row] = true;
            }
        }

        for (int row = 0; row < 40; row++)
        {
            for (int col = 0; col < 40; col++)
            {
                if (roadSquares[col, row])
                {
                    MakeCube("Path_" + col + "_" + row, TilePosition(col, row, 0),
                        new Vector3(1, 0.12f, 1), material, parent);
                }
            }
        }
    }

    // Creates empty waypoint objects that enemies can follow along a route
    private static Transform MakeWaypoints(string routeName, int[,] corners, Transform parent)
    {
        GameObject route = new GameObject(routeName);
        route.transform.parent = parent;

        for (int i = 0; i < corners.GetLength(0); i++)
        {
            GameObject point = new GameObject("WP_" + i.ToString("00"));
            point.transform.parent = route.transform;
            point.transform.position = TilePosition(corners[i, 0], corners[i, 1], 0);
        }
        return route.transform;
    }

    // Creates a cube with the requested appearance and removes its collider unless it is a placement pad
    private static GameObject MakeCube(string name, Vector3 position, Vector3 size, Material material, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.parent = parent;
        cube.transform.position = position;
        cube.transform.localScale = size;
        cube.GetComponent<Renderer>().sharedMaterial = material;

        if (name.StartsWith("Pad_") == false)
        {
            Object.DestroyImmediate(cube.GetComponent<Collider>());
        }
        return cube;
    }

    // Loads an existing material or creates and saves a new one with the requested colour
    private static Material MakeMaterial(string name, Color colour)
    {
        string file = "Assets/Materials/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(file);
        if (material != null)
            return material;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        if (shader == null)
        {
            Debug.LogError("Could not find a shader for " + name);
            return null;
        }

        material = new Material(shader);
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", colour);
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", colour);
        AssetDatabase.CreateAsset(material, file);
        return material;
    }

    // Creates an Assets subfolder if it doesn't exist
    private static void MakeFolder(string name)
    {
        if (AssetDatabase.IsValidFolder("Assets/" + name) == false)
            AssetDatabase.CreateFolder("Assets", name);
    }
}
