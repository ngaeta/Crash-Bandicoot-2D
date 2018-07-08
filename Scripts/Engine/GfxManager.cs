using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Aiv.Fast2D;
using OpenTK;
using System.IO;

namespace CrashBandicoot
{
    static class GfxManager
    {
        private static Dictionary<string, Texture> textures;
        private static Dictionary<string, Tuple<Texture, List<Animation>>> spritesheets;

        public struct ColorRGB
        {
            public byte Red;
            public byte Green;
            public byte Blue;

            public ColorRGB(byte r, byte g, byte b)
            {
                Red = r;
                Green = g;
                Blue = b;
            }
        }

        static GfxManager()
        {
            textures = new Dictionary<string, Texture>();
            spritesheets = new Dictionary<string, Tuple<Texture, List<Animation>>>();
        }

        public static bool AddTexture(string key, string textureName)
        {
            if (!textures.ContainsKey(key))
            {
                textures.Add(key, new Texture(textureName));
                return true;
            }
            return false;
        }

        public static void RemoveAll()
        {
            textures.Clear();
            spritesheets.Clear();
        }

        public static Texture GetTexture(string textureName)
        {
            if (textures.ContainsKey(textureName))
            {
                return textures[textureName];
            }

            return null;
        }

        public static void AddSpritesheet(string name, Texture t, List<Animation> a)
        {
            if (!spritesheets.ContainsKey(name))
                spritesheets.Add(name, new Tuple<Texture, List<Animation>>(t, a));
        }

        public static Tuple<Texture, List<Animation>> GetSpritesheet(string key)
        {
            if (spritesheets.ContainsKey(key))
            {
                Tuple<Texture, List<Animation>> tupleToCopy = spritesheets[key];
                List<Animation> cloneList = new List<Animation>();

                for (int i = 0; i < tupleToCopy.Item2.Count; i++)
                {
                    cloneList.Add((Animation)tupleToCopy.Item2[i].Clone());
                }

                return new Tuple<Texture, List<Animation>>(tupleToCopy.Item1, cloneList);
            }

            return null;
        }

        private static Animation LoadAnimation(
            XmlNode animationNode, int width, int height)
        {
            XmlNode currNode = animationNode.FirstChild;
            bool loop = bool.Parse(currNode.InnerText);

            currNode = currNode.NextSibling;
            float fps = float.Parse(currNode.InnerText);

            currNode = currNode.NextSibling;
            int rows = int.Parse(currNode.InnerText);

            currNode = currNode.NextSibling;
            int cols = int.Parse(currNode.InnerText);

            currNode = currNode.NextSibling;
            int startX = int.Parse(currNode.InnerText);

            currNode = currNode.NextSibling;
            int startY = int.Parse(currNode.InnerText);

            return new Animation(width, height, cols, rows, fps, loop, startX, startY);
        }

        public static void Load()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets/SpriteSheetConfig.xml");

            XmlNode root = doc.DocumentElement;

            foreach (XmlNode spritesheetNode in root.ChildNodes)
            {
                if (spritesheetNode.NodeType != XmlNodeType.Comment)
                    LoadSpritesheet(spritesheetNode);
            }
        }

        private static void LoadSpritesheet(XmlNode spritesheetNode)
        {
            XmlNode nameNode = spritesheetNode.FirstChild;

            string name = nameNode.InnerText;
            XmlNode filenameNode = nameNode.NextSibling;
            Texture texteure = new Texture(filenameNode.InnerText);
            XmlNode frameNode = filenameNode.NextSibling;

            List<Animation> animations = new List<Animation>();

            if (frameNode.HasChildNodes)
            {
                int width = int.Parse(frameNode.FirstChild.InnerText);
                int height = int.Parse(frameNode.LastChild.InnerText);
                XmlNode animationsNode = frameNode.NextSibling;

                foreach (XmlNode animation in animationsNode)
                {
                    if (animation.NodeType != XmlNodeType.Comment)
                    {
                        animations.Add(LoadAnimation(
                                animation, width, height));
                    }
                }
            }
            else
            {
                animations.Add(new Animation(texteure.Width, texteure.Height));
            }

            AddSpritesheet(name, texteure, animations);
        }

        // prendere tutti i nodi image
        // estrarre l'attributo source
        // creare la texture a partire da source
        // creare la spritesheet utilizzando source (come nome ) e la texture creata
        // aggiungere source alla lista da ritornare
        public static List<string> LoadTileSet(string filename, out Dictionary<int, XmlNode> dictProperties)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            List<string> tileNames = new List<string>();

            // Get all nodes with name == "image"
            XmlNodeList imageNodes = doc.SelectNodes("//image");

            for (int i = 0; i < imageNodes.Count; i++)
            {
                string source = imageNodes[i].Attributes["source"].Value;
                tileNames.Add(Path.GetFileNameWithoutExtension(source));
            }

            XmlNodeList tileNodes = doc.SelectNodes("//tile");
            dictProperties = new Dictionary<int, XmlNode>();

            foreach (XmlNode tile in tileNodes)
            {
                if (tile.FirstChild.Name.Equals("properties"))
                {
                    dictProperties.Add(int.Parse(tile.Attributes["id"].Value), tile.FirstChild);
                }
            }

            return tileNames;
        }

        public static void LoadTiledMap(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode root = doc.DocumentElement;
            int cols = int.Parse(root.Attributes["width"].Value);
            int rows = int.Parse(root.Attributes["height"].Value);
            int tileWidth = int.Parse(root.Attributes["tilewidth"].Value);
            int tileHeight = int.Parse(root.Attributes["tileheight"].Value);

            XmlNode tileset = root.FirstChild;
            string source = tileset.Attributes["source"].Value;

            Dictionary<int, XmlNode> dictProperties;
            List<string> tileNames = LoadTileSet("Assets/Tiles/" + source, out dictProperties);

            //create ground without colliison
            XmlNode groundData = tileset.NextSibling.FirstChild;
            CreateTileLayer(groundData, ref tileNames, tileWidth, tileHeight, cols, 1);

            //create water
            XmlNode waterNode = tileset.NextSibling.NextSibling;
            XmlNodeList list = waterNode.ChildNodes;

            foreach (XmlNode objectNode in list)
            {
                InstantiateWater(objectNode, dictProperties);
            }

            //create middleground object
            XmlNode middleGroundObjects = waterNode.NextSibling;
            CreateObjectsLayers(middleGroundObjects.ChildNodes, ref tileNames, DrawManager.Layer.Middleground);


            //create ground with collision

            XmlNode dataGroundWithRigidbody = middleGroundObjects.NextSibling.FirstChild;
            CreateTileLayer(dataGroundWithRigidbody, ref tileNames, tileWidth, tileHeight, cols);

            /* 
             * Create PlayGround Objects

             */

            List<ICheckpointLoadable> objectLoadable = new List<ICheckpointLoadable>();


            //Create player
            XmlNode playerObj = dataGroundWithRigidbody.ParentNode.NextSibling;
            XmlNode playerNode = playerObj.FirstChild;

            Player p = null;

            if (playerNode != null)
            {
                string xString = playerNode.Attributes["x"].Value;
                float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

                string yString = playerNode.Attributes["y"].Value;
                float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

                Vector2 playerPos = new Vector2(xPos + (int.Parse(playerNode.Attributes["width"].Value) / 2), yPos - (int.Parse(playerNode.Attributes["height"].Value) / 2));

                p = new Player(playerPos);
                PlayScene.Player = p;
            }
            else
            {
                p = new Player(new Vector2(40, Game.Window.Height - 260));
                PlayScene.Player = p;
            }

            objectLoadable.Add(p);

            XmlNode crateNode = playerObj.NextSibling;
            list = crateNode.ChildNodes;

            Dictionary<BorderCrate, int> borderCrates = new Dictionary<BorderCrate, int>();
            Dictionary<int, TriggerIronCrate> triggerCrates = new Dictionary<int, TriggerIronCrate>();
            Dictionary<int, TriggerNitroCrate> triggerNitro = new Dictionary<int, TriggerNitroCrate>();
            List<CheckpointCrate> checkpoints = new List<CheckpointCrate>();

            foreach (XmlNode objectNode in list)
            {
                InstantiateCrate(objectNode, dictProperties, ref borderCrates, ref triggerCrates,
                    ref triggerNitro, ref objectLoadable, ref checkpoints);
            }

            foreach (KeyValuePair<BorderCrate, int> b in borderCrates)
            {
                triggerCrates[b.Value].AddCrate(b.Key);
            }

            XmlNode pickableNode = crateNode.NextSibling;
            list = pickableNode.ChildNodes;


            foreach (XmlNode objectNode in list)
            {
                InstantiatePickable(objectNode, dictProperties, ref objectLoadable);
            }

            XmlNode enemiesNode = pickableNode.NextSibling;
            list = enemiesNode.ChildNodes;


            foreach (XmlNode objectNode in list)
            {
                InstantiateEnemies(objectNode, dictProperties, ref objectLoadable);
            }

            XmlNode objectsWalkableNode = enemiesNode.NextSibling;
            list = objectsWalkableNode.ChildNodes;


            foreach (XmlNode objectNode in list)
            {
                InstantiateObjectsWakable(objectNode, dictProperties);
            }

            XmlNode trapNode = objectsWalkableNode.NextSibling;
            list = trapNode.ChildNodes;

            Dictionary<int, ITriggableAction> trapsTriggable = new Dictionary<int, ITriggableAction>();

            foreach (XmlNode objectNode in list)
            {
                InstantiateTraps(objectNode, dictProperties, ref trapsTriggable, ref objectLoadable);
            }

            XmlNode triggersEventNode = trapNode.NextSibling;
            list = triggersEventNode.ChildNodes;

            foreach (XmlNode objectNode in list)
            {
               InstantiateTriggersEvent(objectNode, dictProperties, ref trapsTriggable, ref objectLoadable);
            }

            //create foregroundobjects
            XmlNode foreGroundObjects = triggersEventNode.NextSibling;
            CreateObjectsLayers(foreGroundObjects.ChildNodes, ref tileNames, DrawManager.Layer.Foreground);
            //method to create foreground

            for (int i = 0; i < objectLoadable.Count; i++)
            {
                ICheckpointLoadable loadable = objectLoadable[i];
                CheckpointCrate.AddObjectsToLoad(loadable);
            }

            GameManager.Loadables = objectLoadable;
        }

        private static void CreateObjectsLayers(XmlNodeList objectNodes, ref List<string> tileNames, DrawManager.Layer layer)
        {
            for (int i = 0; i < objectNodes.Count; i++)
            {
                XmlNode objectNode = objectNodes[i];
                string xString = objectNode.Attributes["x"].Value;
                float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

                string yString = objectNode.Attributes["y"].Value;
                float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

                Vector2 pos = new Vector2(xPos + (int.Parse(objectNode.Attributes["width"].Value) / 2), yPos - (int.Parse(objectNode.Attributes["height"].Value) / 2));
                int globalIdCrate = int.Parse(objectNode.Attributes["gid"].Value) - 1;

                GameObject g = new GameObject(pos, tileNames[globalIdCrate]);

                if(objectNode.HasChildNodes)
                {
                    g.GetSprite().FlipX = bool.Parse(objectNode.FirstChild.FirstChild.Attributes["value"].Value);
                }
            }
        }

        private static void CreateTileLayer(XmlNode nodeData, ref List<string> tileNames, int tileWidth,
            int tileHeight, int cols, int typeCreation = 0, DrawManager.Layer layer = DrawManager.Layer.Playground)
        {
            string mapData = nodeData.InnerText;

            string[] mapIndexes = mapData.Split(',');

            for (int i = 0; i <= mapIndexes.Length - 1; i++)
            {
                int index = int.Parse(mapIndexes[i]);
                if (index > 0)
                {
                    string tileName = tileNames[--index];
                    Vector2 pos = new Vector2(
                        tileWidth * (i % cols) + tileWidth / 2,
                        tileHeight * (i / cols) + tileHeight / 2);

                    if (typeCreation == 0)
                        new Ground(pos, tileName, (index == 4 || index == 50) ? false : true);
                    else
                        new Ground(pos, tileName, false);
                }
            }
        }

        private static void InstantiateCrate(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties, ref Dictionary<BorderCrate, int> borderCrates,
            ref Dictionary<int, TriggerIronCrate> triggerCrates, ref Dictionary<int, TriggerNitroCrate> triggerNitros, ref List<ICheckpointLoadable> objectLoadable, ref List<CheckpointCrate> checkpoints)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            Vector2 pos = new Vector2(xPos + (int.Parse(objectNode.Attributes["width"].Value) / 2), yPos - (int.Parse(objectNode.Attributes["height"].Value) / 2));
            int globalIdCrate = int.Parse(objectNode.Attributes["gid"].Value) - 1;

            XmlNode properties = dictProperties[globalIdCrate];
            XmlNodeList childList = properties.ChildNodes;
            XmlNode propertyCrate = SearchNodeWithAttribute(ref childList, "name", "crateType");
            CrateType type = (CrateType)int.Parse(propertyCrate.Attributes["value"].Value);

            Crate c = null;

            switch (type)
            {
                case CrateType.Apple:
                    c = new AppleCrate(pos);
                    break;
                case CrateType.Random:
                    c = new RandomCrate(pos);
                    break;
                case CrateType.Life:
                    c = new LifeCrate(pos);
                    break;
                case CrateType.Aku:
                    c = new AkuAkuCrate(pos);
                    break;
                case CrateType.Bounce_Apple:
                    c = new AppleBounceCrate(pos);
                    break;
                case CrateType.Bounce_Wooden:
                    c = new WoodenBounceCrate(pos);
                    break;
                case CrateType.Bounce_Iron:
                    c = new IronBounceCrate(pos);
                    break;
                case CrateType.Iron:
                    c = new IronCrate(pos);
                    break;
                case CrateType.Trigger_Iron:

                    c = new TriggerIronCrate(pos);
                    XmlNode idCrate = objectNode.FirstChild.LastChild;
                    int id = int.Parse(idCrate.Attributes["value"].Value);
                    triggerCrates.Add(id, (TriggerIronCrate)c);
                    break;
                case CrateType.Tnt:
                    c = new TntCrate(pos);
                    break;
                case CrateType.Nitro:
                    c = new NitroCrate(pos);

                    if (objectNode.HasChildNodes)
                    {
                        XmlNodeList list = objectNode.FirstChild.ChildNodes;
                        XmlNode node = SearchNodeWithAttribute(ref list, "name", "vTriggerNitroId");
                        if (node != null)
                        {
                            int idN = int.Parse(node.Attributes["value"].Value);
                            if (idN > 0)
                                triggerNitros[idN].AddNitroCrate((NitroCrate)c);
                        }
                    }

                    break;
                case CrateType.Border_Crate:

                    XmlNode borderTypeNode = objectNode.FirstChild.FirstChild;
                    CrateType borderType;

                    if (borderTypeNode.Attributes["name"].Value.Equals("borderCrate"))
                    {
                        borderType = (CrateType)int.Parse(borderTypeNode.Attributes["value"].Value);
                    }
                    else
                        borderType = CrateType.Iron;

                    BorderCrate b = new BorderCrate(pos, borderType);
                    borderCrates.Add(b, int.Parse(objectNode.FirstChild.LastChild.Attributes["value"].Value));
                    objectLoadable.Add(b);
                    return;
                case CrateType.Trigger_Iron_Nitro:
                    c = new TriggerNitroCrate(pos);
                    XmlNode idNitroCrate = objectNode.FirstChild.LastChild;
                    int idTrigger = int.Parse(idNitroCrate.Attributes["value"].Value);
                    triggerNitros.Add(idTrigger, (TriggerNitroCrate)c);
                    break;
                case CrateType.Checkpoint_Crate:
                    checkpoints.Add(new CheckpointCrate(pos));
                    return;
            }

            if (c != null)
            {
                objectLoadable.Add(c);
            }
            //if (c != null && objectNode.HasChildNodes)
            //{
            //    if (objectNode.FirstChild.FirstChild.Attributes["name"].Value == "useGravity")
            //        c.UseGroundableGravity = bool.Parse(objectNode.FirstChild.FirstChild.Attributes["value"].Value);
            //}
        }

        private static void InstantiatePickable(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties, ref List<ICheckpointLoadable> objectsLoadable)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            Vector2 pos = new Vector2(xPos + (int.Parse(objectNode.Attributes["width"].Value) / 2), yPos - (int.Parse(objectNode.Attributes["height"].Value) / 2));
            int globalIdCrate = int.Parse(objectNode.Attributes["gid"].Value) - 1;

            XmlNode properties = dictProperties[globalIdCrate];

            PickableType type = (PickableType)int.Parse(properties.FirstChild.Attributes["value"].Value);
            Pickable p = null;

            switch (type)
            {
                case PickableType.Apple:
                    p = new Apple(pos);
                    break;
                case PickableType.ExtraLife:
                    p = new ExtraLife(pos);
                    break;
                case PickableType.Crystal:
                    p = new CrystalPickable(pos);
                    break;
            }

            if (p != null)
                objectsLoadable.Add(p);
        }

        private static void InstantiateEnemies(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties, ref List<ICheckpointLoadable> objectsLoadable)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            Vector2 pos = new Vector2(xPos + (int.Parse(objectNode.Attributes["width"].Value) / 2), yPos - (int.Parse(objectNode.Attributes["height"].Value) / 2));
            int globalIdCrate = int.Parse(objectNode.Attributes["gid"].Value) - 1;

            XmlNode properties = dictProperties[globalIdCrate];

            EnemyType type = (EnemyType)int.Parse(properties.FirstChild.Attributes["value"].Value);
            Enemy e = null;

            switch (type)
            {
                case EnemyType.Gecko:
                    e = new GeckoEnemy(pos);
                    break;
                case EnemyType.Lizard:
                    e = new LizardEnemy(pos);
                    break;
                case EnemyType.BlowGunMan:
                    e = new BlowGunManEnemy(pos);
                    break;
                case EnemyType.Bird:

                    float sightRadius = 370f;

                    if(objectNode.HasChildNodes)
                    {
                        sightRadius = float.Parse(objectNode.FirstChild.FirstChild.Attributes["value"].Value, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    e = new BirdEnemy(pos, sightRadius);
                    break;
            }

            if (e != null)
                objectsLoadable.Add(e);
        }

        private static void InstantiateObjectsWakable(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            Vector2 pos = new Vector2(xPos + (int.Parse(objectNode.Attributes["width"].Value) / 2), yPos - (int.Parse(objectNode.Attributes["height"].Value) / 2));
            int globalIdCrate = int.Parse(objectNode.Attributes["gid"].Value) - 1;

            XmlNode properties = dictProperties[globalIdCrate];
            int indexGround = -1;

            XmlNodeList list = properties.ChildNodes;
            XmlNode propertyPlatform = SearchNodeWithAttribute(ref list, "name", "platformType");

            if (propertyPlatform != null)
                indexGround = int.Parse(propertyPlatform.Attributes["value"].Value);

            if (indexGround == 0)
            {
                list = objectNode.FirstChild.ChildNodes;
                propertyPlatform = SearchNodeWithAttribute(ref list, "name", "platformType");

                if (propertyPlatform != null)
                    indexGround = int.Parse(propertyPlatform.Attributes["value"].Value);
            }

            GroundType type = (GroundType)indexGround;
            Ground g = null;

            switch (type)
            {
                case GroundType.Static:
                    g = new InvisiblePlatform(pos);
                    break;
                case GroundType.Mobile:
                    XmlNode xOffsetNode = SearchNodeWithAttribute(ref list, "name", "xOffset");
                    XmlNode yOffsetNode = SearchNodeWithAttribute(ref list, "name", "yOffset");
                    XmlNode xVel = SearchNodeWithAttribute(ref list, "name", "xVel");
                    XmlNode yVel = SearchNodeWithAttribute(ref list, "name", "yVel");

                    g = new MobilePlatform(pos, GetVectorFromXMlNode(xOffsetNode, yOffsetNode), GetVectorFromXMlNode(xVel, yVel));
                    break;

                case GroundType.Orbital:
                    XmlNode xOffset = SearchNodeWithAttribute(ref list, "name", "xOffset");
                    XmlNode yOffset = SearchNodeWithAttribute(ref list, "name", "yOffset");
                    XmlNode rotationSpeedNode = SearchNodeWithAttribute(ref list, "name", "rotationSpeed");

                    float rotationSpeed = 100f;

                    if (rotationSpeedNode != null)
                    {
                        string rotationString = rotationSpeedNode.Attributes["value"].Value;
                        rotationSpeed = float.Parse(rotationString, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    g = new OrbitalPlatform(pos, GetVectorFromXMlNode(xOffset, yOffset), rotationSpeed);
                    break;
                case GroundType.Branch:
                    g = new BranchPlatform(pos);
                    break;
                case GroundType.Leaf:
                    g = new MobileLeaf(pos, int.Parse(objectNode.FirstChild.FirstChild.Attributes["value"].Value), int.Parse(objectNode.FirstChild.LastChild.Attributes["value"].Value));
                    break;
            }

            if (g is InvisiblePlatform i)
            {
                i.ItDisappearAndAppear = bool.Parse(objectNode.FirstChild.FirstChild.Attributes["value"].Value);
            }
        }

        private static void InstantiateTraps(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties,
            ref Dictionary<int, ITriggableAction> trapsActivable, ref List<ICheckpointLoadable> objectsLoadable)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            Vector2 pos = new Vector2(xPos + (int.Parse(objectNode.Attributes["width"].Value) / 2), yPos - (int.Parse(objectNode.Attributes["height"].Value) / 2));
            int globalIdCrate = int.Parse(objectNode.Attributes["gid"].Value) - 1;

            if (dictProperties.ContainsKey(globalIdCrate))
            {
                XmlNode properties = dictProperties[globalIdCrate];

                TrapType type = (TrapType)int.Parse(properties.FirstChild.Attributes["value"].Value);

                switch (type)
                {
                    case TrapType.Thorn:
                        new Trap(pos);
                        break;
                    case TrapType.IdolHead:
                        new IdolHeadFire(pos);
                        return;
                    case TrapType.StoneWheel:
                        StoneWhellTrap t = new StoneWhellTrap(pos);
                        XmlNode idTrap = objectNode.FirstChild.FirstChild;
                        trapsActivable.Add(int.Parse(idTrap.Attributes["value"].Value), t);
                        objectsLoadable.Add(t);

                        if(idTrap.NextSibling != null)
                        {
                            float xVel = float.Parse(idTrap.NextSibling.Attributes["value"].Value, System.Globalization.CultureInfo.InvariantCulture);
                            t.VelocityOffset = xVel;
                        }
                            
                        break;
                    case TrapType.BranchTrap:
                        new BranchTrap(pos);
                        break;
                }
            }
        }

        private static void InstantiateWater(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            float height = float.Parse(objectNode.Attributes["height"].Value, System.Globalization.CultureInfo.InvariantCulture) / 2;
            float width = float.Parse(objectNode.Attributes["width"].Value, System.Globalization.CultureInfo.InvariantCulture) / 2;

            Vector2 pos = new Vector2(xPos + width, yPos);
            Water w = new Water(pos);
            w.SetScale(new Vector2(width / 100, height / 50f));
        }

        private static void InstantiateTriggersEvent(XmlNode objectNode, Dictionary<int, XmlNode> dictProperties,
            ref Dictionary<int, ITriggableAction> triggablesAction, ref List<ICheckpointLoadable> objectsLoadable)
        {
            string xString = objectNode.Attributes["x"].Value;
            float xPos = float.Parse(xString, System.Globalization.CultureInfo.InvariantCulture);

            string yString = objectNode.Attributes["y"].Value;
            float yPos = float.Parse(yString, System.Globalization.CultureInfo.InvariantCulture);

            float height = float.Parse(objectNode.Attributes["height"].Value, System.Globalization.CultureInfo.InvariantCulture);
            float width = float.Parse(objectNode.Attributes["width"].Value, System.Globalization.CultureInfo.InvariantCulture);

            Vector2 pos = new Vector2(xPos + width / 2, yPos + height / 2);

            Limits attual = CameraManager.CameraLimits;
            TriggerType triggerType = (TriggerType)int.Parse(objectNode.FirstChild.LastChild.Attributes["value"].Value);
            TriggerCheckpointLoadable t = null;

            switch (triggerType)
            {
                case TriggerType.Trigger_Camera:
                    XmlNodeList propertiesChildren = objectNode.FirstChild.ChildNodes;
                    float[] limits = new float[4];
                    bool drawBackground = true;

                    for (int i = 0; i < propertiesChildren.Count - 1; i++)
                    {
                        if (propertiesChildren[i].Attributes["name"].Value.Equals("drawBackground"))
                        {
                            drawBackground = false;
                            continue;
                        }

                        limits[i] = float.Parse(propertiesChildren[i].Attributes["value"].Value, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    Limits limit = new Limits(attual.MinX * limits[1], attual.MaxX * limits[0], attual.MinY * limits[3], attual.MaxY * limits[2]);

                    t = new CameraTriggerLimits(new Rect(pos, null, width, height), limit, drawBackground);
                    break;

                case TriggerType.Trigger_Action:
                    t = new TriggerActiveAction(new Rect(pos, null, width, height), triggablesAction[int.Parse(objectNode.FirstChild.FirstChild.Attributes["value"].Value)]);
                    break;
                case TriggerType.Trigger_Coco:
                    new TriggerSpeakCoco(new Rect(pos, null, width, height));
                    break;
            }

            if (t != null)
            {
                objectsLoadable.Add(t);
            }
        }

        private static Vector2 GetVectorFromXMlNode(XmlNode nodeX, XmlNode nodeY)
        {
            Vector2 vector = Vector2.Zero;

            if (nodeX != null)
                vector.X = int.Parse(nodeX.Attributes["value"].Value);

            if (nodeY != null)
                vector.Y = int.Parse(nodeY.Attributes["value"].Value);

            return vector;
        }


        private static XmlNode SearchNodeWithAttribute(ref XmlNodeList list, string attributesName, string valueName = "")
        {
            foreach (XmlNode node in list)
            {
                if (valueName == "")
                {
                    if (node.Attributes[attributesName] != null)
                    {
                        return node;
                    }
                }

                else if (node.Attributes[attributesName] != null && node.Attributes[attributesName].Value.Equals(valueName))
                {
                    return node;
                }
            }

            return null;
        }
    }
}

